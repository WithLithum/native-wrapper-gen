// SDPX-FileCopyrightText: 2025-2026 WithLithum
// SPDX-License-Identifier: Apache-2.0

using System.CommandLine;
using System.CommandLine.Parsing;
using System.Diagnostics;
using WithLithum.NativeWrapperGen.Generation;
using WithLithum.NativeWrapperGen.Generation.Hooks;
using WithLithum.NativeWrapperGen.Models;

namespace WithLithum.NativeWrapperGen;

internal static class Program
{
    private static readonly Dictionary<string, Func<GeneratorSettings, IShimGenerator>> GeneratorNames = new()
    {
        { VDotNetGenerator.Id , static settings => new VDotNetGenerator(settings) },
        { RageGenerator.Id, static settings => new RageGenerator(settings) }
    };

    private const string DefaultFileNameFormat = "Natives.{0}.cs";
    private const string DefaultNameSpace = "WithLithum.NativeWrapperGen.Artefact";
    private const string DefaultClassName = "Natives";

    private static readonly Action<OptionResult> FileValidator = ctx =>
    {
        var value = ctx.GetValueOrDefault<string>();
        if (!string.IsNullOrWhiteSpace(value) && !File.Exists(value))
        {
            ctx.AddError($"File '{value}' does not exist");
        }
    };

    private static readonly Option<string?> DefinitionFileOption = new("--natives-file")
    {
        Description = "An 'alloc8or/gta5-natives-data' conforming 'natives.json' file. Uses the one bundled in 'Data' directory if not specified.",
        DefaultValueFactory = _ => null,
        Validators = { FileValidator }
    };

    private static readonly Option<string?> ConfigFileOption = new("--config-file")
    {
        Description = "A generator config file. Uses default settings if not specified.",
        DefaultValueFactory = _ => null,
        Validators = { FileValidator }
    };

    private static readonly Option<string> FileNameFormatOption = new("--file-name-format")
    {
        Description = "The format of the file name. '{0}' will become the natives namespace name.",
        DefaultValueFactory = _ => DefaultFileNameFormat
    };

    private static readonly Option<string> NamespaceOption = new("--namespace")
    {
        Description = "The namespace to put the generated class in.",
        DefaultValueFactory = _ => DefaultNameSpace
    };

    private static readonly Option<string> ClassNameOption = new("--class-name")
    {
        Description = "The name of the generated class.",
        DefaultValueFactory = _ => DefaultClassName
    };

    private static readonly Option<bool> CountTimeOption = new("--count-time")
    {
        Description = "Counts total time cost of generation."
    };

    private static readonly Option<string> GeneratorOption = new("-g", "--generator")
    {
        Description = "Specifies the generator to use.",
        DefaultValueFactory = _ => "shvdn",
        Validators = {
            o => {
                var v = o.GetValueOrDefault<string>();
                if (!string.IsNullOrWhiteSpace(v) && !GeneratorNames.ContainsKey(v))
                {
                    o.AddError("Unknown generator");
                }
            }
        }
    };


    private static int Main(string[] args)
    {
        var command = new RootCommand("Generates wrappers for GTA V script commands / natives")
        {
            Options =
            {
                DefinitionFileOption,
                ConfigFileOption,
                FileNameFormatOption,
                NamespaceOption,
                ClassNameOption,
                CountTimeOption,
                GeneratorOption
            }
        };

        command.SetAction(Execute);

        var parseResult = command.Parse(args);
        return parseResult.Invoke();
    }

    private static void Execute(ParseResult result)
    {
        // Command arguments.
        var defFile = result.GetValue(DefinitionFileOption);
        var configFile = result.GetValue(ConfigFileOption);
        var fileNameFormat = result.GetValue(FileNameFormatOption);
        var nameSpace = result.GetValue(NamespaceOption);
        var className = result.GetValue(ClassNameOption);
        var countTime = result.GetValue(CountTimeOption);
        var generator = result.GetValue(GeneratorOption) ?? VDotNetGenerator.Id;

        // Command logic.
        ScriptCommandManifest? information;
        GeneratorSettings? settings;

        try
        {
            information = ConfigFileHelper.LoadManifest(defFile);
            settings = string.IsNullOrWhiteSpace(configFile)
                ? ConfigFileHelper.GetDefault(generator)
                : ConfigFileHelper.LoadSettingsFile(configFile);
        }
        catch (Exception ex)
        {
            Console.WriteLine("ERROR: failure when loading configuration files");
            Console.WriteLine(ex.ToString());
            Environment.ExitCode = 2;
            return;
        }

        if (information == null || settings == null)
        {
            Console.WriteLine("ERROR: Unable to load configuration files. Do they exist and is not just 'null'?");
            Environment.ExitCode = 1;
            return;
        }

        var hookGenerator = GeneratorNames.GetValueOrDefault(generator)?.Invoke(settings)
            ?? new VDotNetGenerator(settings);

        var multiGenerator = new MultiWrapperFileGenerator(fileNameFormat ?? DefaultFileNameFormat,
            nameSpace ?? DefaultNameSpace,
            className ?? DefaultClassName,
            settings,
            hookGenerator);

        Stopwatch? stopwatch = null;
        if (countTime)
        {
            stopwatch = Stopwatch.StartNew();
        }

        multiGenerator.GenerateComplete(information);

        if (stopwatch != null)
        {
            stopwatch.Stop();
            Console.WriteLine("Generation took {0}ms (or {1})", stopwatch.ElapsedMilliseconds,
                stopwatch.Elapsed);
        }
    }
}
