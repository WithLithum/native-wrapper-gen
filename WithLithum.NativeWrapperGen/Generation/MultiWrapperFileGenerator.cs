// SDPX-FileCopyrightText: 2025-2026 WithLithum
// SPDX-License-Identifier: Apache-2.0

using WithLithum.NativeWrapperGen.Models;

namespace WithLithum.NativeWrapperGen.Generation;

public class MultiWrapperFileGenerator
{
    private readonly string _fileNameFormat;
    private readonly string _nameSpace;
    private readonly string _className;
    private readonly GeneratorSettings _generatorSettings;
    private readonly IShimGenerator _shimGenerator;

    public MultiWrapperFileGenerator(string fileNameFormat,
        string nameSpace,
        string className,
        GeneratorSettings generatorSettings,
        IShimGenerator shimGenerator)
    {
        _fileNameFormat = fileNameFormat;
        _nameSpace = nameSpace;
        _className = className;
        _generatorSettings = generatorSettings;
        _shimGenerator = shimGenerator;
    }

    public void GenerateComplete(ScriptCommandManifest manifest)
    {
        foreach (var partial in manifest)
        {
            var fullPath = Path.GetFullPath(string.Format(_fileNameFormat,
                MethodNameConverter.SnakeToPascal(partial.Key)),
                Directory.GetCurrentDirectory());

            using var fileStream = File.Create(fullPath);
            using var bufferedStream = new BufferedStream(fileStream);
            using var writer = new StreamWriter(bufferedStream);

            var generator = new WrapperFileGenerator(writer,
                _generatorSettings,
                _shimGenerator);

            var context = new WrapperSectionContext
            {
                Namespace = partial.Key,
                Commands = partial.Value,
            };
            generator.WritePartial(_nameSpace, _className, in context);

            writer.Flush();
            bufferedStream.Flush();
        }
    }
}
