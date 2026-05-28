# Code styles

The code styles of this repository generally derives from the [.NET Runtime Coding Styles](https://github.com/dotnet/runtime/blob/main/docs/coding-guidelines/coding-style.md).

The following repository specific rules however will prevail in the case of conflict.

## Formatting

- Four spaces for indentation, no tabs.
- Allman-styled braces.

## Symbols

- Use `_camelCase` for _instance_ private or internal fields. Make them `readonly` whenever possible.
- Use `PascalCase` for public or static fields, properties and methods. Make them `readonly` whenever possible.
