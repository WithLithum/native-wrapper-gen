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
namespace WithLithum.NativeWrapperGen.Generation;

using WithLithum.NativeWrapperGen.Models;

public class MultiWrapperFileGenerator
{
    private readonly string _fileNameFormat;
    private readonly string _nameSpace;
    private readonly string _className;
    private readonly GeneratorSettings _generatorSettings;

    public MultiWrapperFileGenerator(string fileNameFormat, 
        string nameSpace,
        string className,
        GeneratorSettings generatorSettings)
    {
        _fileNameFormat = fileNameFormat;
        _nameSpace = nameSpace;
        _className = className;
        _generatorSettings = generatorSettings;
    }

    public void GenerateComplete(ScriptCommandManifest manifest)
    {
        foreach (var partial in manifest)
        {
            var fullPath = Path.GetFullPath(string.Format(_fileNameFormat,
                MethodNameConverter.SnakeToPascal(partial.Key)),
                Directory.GetCurrentDirectory());

            using var writer = File.CreateText(fullPath);

            var generator = new WrapperFileGenerator(writer,
                _generatorSettings);
            generator.WritePartial(_nameSpace, _className, partial.Value);

            writer.Flush();
        }
    }
}
