# Agent Instructions

> [!NOTE]
> This file is intended for AI Agents. Human developers should visit [README.md](README.md).

`native-wrapper-gen` is a C# console program that generates wrapper or shim methods for performing native function (script command) calls in a C# Grand Theft Auto V Legacy script mod, from the native function definitions in the [alloc8or's native database](https://github.com/alloc8or/gta5-nativedb-data).

It is designed to work with:

- [Script Hook V .NET](https://github.com/scripthookvdotnet/scripthookvdotnet)
- [RAGE Plugin Hook](https://ragepluginhook.net)

## Project Structure

Components of this project are broken down below in a tree hierarchy:

- `WithLithum.NativeWrapperGen`: The wrapper generator program.
  - `/Data`: Embedded data files used by the wrapper generator.
  - `/Generation`: The wrapper generator logic.
    - `/Hooks`: The logic responsible for generating shim methods for each specific hook.
  - `/Models`: Contains data models for settings and alloc8or native db file.
  - `/Serialization`: Contains System.Text.Json serialization logic.
- `NativeWrapperGenTests`: The unit tests for the wrapper generator.
- `Benchmarks`: Benchmarks for the wrapper generator.
- `wrappers`: Pre-built wrapper package projects.

## Commonly used commands

- Restore: `dotnet restore`
- Build & test: `dotnet build` and `dotnet test`
- Generate pre-built wrapper code: `pwsh .\GenerateNativeWrapper.ps1` (results will be in `bin`)

## Code standards

- Follow the project [style guide](docs/CODESTYLES.md)
- Every hook specific generator MUST inherit from `CSharpGenerator`
