namespace WithLithum.NativeWrapperGen.Generation;

partial class WrapperFileGenerator
{
    internal void WriteNameSpaceScopeHeader(string nameSpace)
    {
        _writer.WriteLine("namespace {0}", nameSpace);
        _writer.WriteLine('{');
    }

    internal void WriteClassScopeHeader(string className)
    {
        _writer.WriteLine("public static partial class {0}", className);
        _writer.WriteLine('{');
    }

    internal void WriteScopeFooter()
    {
        _writer.WriteLine('}');
    }
}