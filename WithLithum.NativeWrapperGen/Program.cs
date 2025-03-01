// Copyright (C) 2025 WithLithum.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
using System.CommandLine;
using System.Diagnostics;
using WithLithum.NativeWrapperGen;
using WithLithum.NativeWrapperGen.Generation;
using WithLithum.NativeWrapperGen.Models;

var optNativesFile = new Option<string?>("--natives-file",
    () => null,
    "An 'alloc8or/gta5-natives-data' conforming 'natives.json' file. Uses the one bundled in 'Data' directory if not specified.");

var optConfigFile = new Option<string?>("--config-file",
    () => null,
    "A generator config file. Uses the one bundled in 'Data' directory if not specified.");

const string defaultFileNameFormat = "Natives.{0}.cs";
const string defaultNameSpace = "WithLithum.NativeWrapperGen.Artefact";
const string defaultClassName = "Natives";

var optNameFormat = new Option<string>("--file-name-format",
    () => defaultFileNameFormat,
    description: "The format of the file name. '{0}' will become the natives namespace name.");

var optNameSpace = new Option<string>("--namespace", 
    () => defaultNameSpace, 
    description: "The natives file namespace.");
var optClassName = new Option<string>("--class-name",
    () => defaultClassName,
    description: "The natives file class name.");

var optCountTime = new Option<bool>("--count-time",
    description: "Counts total time cost of generation");

var command = new RootCommand("Generates wrappers for GTA V script commands / natives")
{
    optNativesFile,
    optConfigFile,
    optNameFormat,
    optNameSpace,
    optClassName,
    optCountTime
};

command.SetHandler((nativesFile, configFile, nameFormat, nameSpace, className, countTime) =>
{
    ScriptCommandManifest? information;
    GeneratorSettings? settings;

    try
    {
        information = ConfigFileHelper.LoadManifest(nativesFile);
        settings = ConfigFileHelper.LoadShvdnSettings(configFile);
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

    var multiGenerator = new MultiWrapperFileGenerator(nameFormat ?? defaultFileNameFormat,
    nameSpace ?? defaultNameSpace,
    className ?? defaultClassName,
    settings);

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

}, optNativesFile, optConfigFile, optNameFormat, optNameSpace, optClassName, optCountTime);

command.Invoke(args);