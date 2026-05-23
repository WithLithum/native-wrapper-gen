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

internal partial class WrapperFileGenerator
{
    private void WriteNameSpaceScopeHeader(string nameSpace)
    {
        _writer.WriteLine("namespace {0}", nameSpace);
        _writer.WriteLine('{');
    }

    private void WriteClassScopeHeader(string className)
    {
        _writer.WriteLine("/// <summary>Provides wrappers for GTA V script commands.</summary>");
        _writer.WriteLine("public static partial class {0}", className);
        _writer.WriteLine('{');
    }

    private void WriteScopeFooter()
    {
        _writer.WriteLine('}');
    }
}