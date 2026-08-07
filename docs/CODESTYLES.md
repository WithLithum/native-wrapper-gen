# Code styles

The code styles of this repository generally derives from the [.NET Runtime Coding Styles](https://github.com/dotnet/runtime/blob/main/docs/coding-guidelines/coding-style.md).

The following repository specific rules however will prevail in the case of conflict.

1. Use 4 spaces for indentation, no tabs.
2. Use the Allman-styled braces, where each brace is on a new line.
3. Make fields `readonly` whenever possible.
4. Use file-scoped namespaces.
4. Put `System` namespace first in the using directives.
4. Use `var` when the type is either apparant, built-in or expressly specified on the right-hand side of the assignment.
5. Use language keywords for primitives, instead of BCL types.
3. Use `_camelCase` for _instance_ private or internal fields.
4. Use `s_camelCase` for _static_ private or internal fields. 
5. Use `PascalCase` for public or static fields, properties and methods.
6. Primary constructors are not permitted except in records where parameters are properties.
