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
