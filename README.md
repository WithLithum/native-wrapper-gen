# Native Wrapper Generator

This program generates wrapper methods for native function, to be used with .NET script hook (wrappers) that works with Grand Theft Auto V Legacy.

Currently, the following platforms are supported:

- [Script Hook V .NET](https://github.com/scripthookvdotnet/scripthookvdotnet) and compatibles
- RAGE Plugin Hook

Compared to the [old generator](https://github.com/NativeFx/InteropGenerator) that was made 3 years ago, this generator:

* runs faster (the actual generator cost less than 300ms in most runs, and the process only takes a few seconds)
* Generates C# 7.3 code (no need for `<LangVersion>latest</LangVersion>`)

## Get

Installable archive for pre-built wrappers are available in the [releases](https://github.com/WithLithum/native-wrapper-gen/releases) page. Corresponding developer packages are published on NuGet:

| Platform | Link                                                               |
| -------- | ------------------------------------------------------------------ |
| SHVDN    | [WithLithum.NativeWrapper](https://www.nuget.org/packages/WithLithum.NativeWrapper) |
| RPH      | [WithLithum.NativeWrapper.RagePluginHook](https://www.nuget.org/packages/WithLithum.NativeWrapper.RagePluginHook) |

Pre-built binaries for the native wrapper generator program are not provided. You need to build the project yourself, if you just want the generator.

### Experimental builds

> [!IMPORTANT]
> Experimental builds are produced _automatically_ for every commit (change) in the `trunk` branch.
>
> There is **absolutely no guarantee** that these builds will work with existing scripts, or future experimental builds won't break those built with previous experimental builds.

To get installable archives for experimental builds, click [here](https://github.com/WithLithum/native-wrapper-gen/actions/workflows/create-wrappers.yml) and select the most recent run. You need a GitHub account to download build artefacts.

Developer packages for experimental builds are published in [GitHub Packages](https://github.com/users/WithLithum/packages?repo_name=native-wrapper-gen).

## Usage

### Synopsis

```shell
WithLithum.NativeWrapperGen
  [--natives-file <natives-json-file>]
  [--config-file <config-json-file>]
  [--generator <generator-name>]
  [--file-name-format <file-name-format>]
  [--namespace <namespace>]
  [--class-name <class-name>]
  [--count-time]
```

### Options

* **natives-file**: `--natives-file <natives-json-file>`
  
  Specifies a `natives.json` file. The file must be conforming to the format as provided in the [`alloc8or nativedb-data`](https://github.com/alloc8or/gta5-nativedb-data).

  Defaults to the bundled data file.

* **config-file**: `--config-file <config-file>`

  Specifies the configuration file. For an example, see [the default settings tailored for SHVDN](WithLithum.NativeWrapperGen/Data/SHVDNSettings.json).

  Defaults to read the included settings.

* **generator**: `--generator <generator-name>`

  Specifies the name of the generator to use. The following is available:
  * `shvdn`: for Script Hook V .NET
  * `rph`: for RAGE Plugin Hook

* **file-name-format**: `--file-name-format <file-name-format>`

  Specifies the file name format. `{0}` gets replaced with the native namespace name. For example, `Natives.{0}.cs` becomes `Natives.Misc.cs`, and so on.

  Path can be included as well. Defaults to `Natives.{0}.cs`.

* **namespace**: `--namespace <namespace>`

  The namespace for the generated file. Defaults to `WithLithum.NativeWrapperGen.Artefact`.

* **class-name**: `--class-name <class-name>`

  The class name for the generated file. Defaults to `Natives`.

* **count-time**: `--count-time`

  If specified, counts the time used to run the generator.

## Building

You need a version of .NET SDK that can target .NET 10.0. Download .NET SDK 10.0 [here](https://dotnet.microsoft.com/download/dotnet/10.0).

Any OS can build and run the generator.

### Wrappers

The wrappers targets .NET Framework 4.8 and has to be built on Windows because of this. The wrappers are under `wrappers` directory.

To create wrappers, use PowerShell 7 to run `GenerateNativeWrapper.ps1`. If you have ran this script at least once, you can specify `-NoDownload` to tell the script not to update native definition files.

.NET SDK is still required because the generator will be built and ran.

## Licence

[Apache-2.0](LICENSE.txt)
