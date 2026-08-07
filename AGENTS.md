# Agent Instructions

> [!NOTE]
> This file is intended for AI Agents. Human developers should visit [README.md](README.md).

`native-wrapper-gen` is a C# console program that generates wrapper or shim methods for performing native function (script command) calls in a C# Grand Theft Auto V Legacy script mod, from the native function definitions in the [alloc8or's native database](https://github.com/alloc8or/gta5-nativedb-data).

It is designed to work with:

- [Script Hook V .NET](https://github.com/scripthookvdotnet/scripthookvdotnet)
- [RAGE Plugin Hook](https://ragepluginhook.net)

## Project Structure

- `WithLithum.NativeWrapperGen`: The wrapper generator program.
- `NativeWrapperGenTests`: The unit tests for the wrapper generator.
- `Benchmarks`: Benchmarks for the wrapper generator.
- `wrappers`: Pre-built wrapper package projects.

## Commonly used commands

- Restore: `dotnet restore`
- Build & test: `dotnet build` and `dotnet test`
- Generate pre-built wrapper code: `pwsh .\GenerateNativeWrapper.ps1` (results will be in `bin`)

## Code standards

- [Style guide](docs/CODESTYLES.md)
