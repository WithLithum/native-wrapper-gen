# Native Wrapper Generator

Generates native function wrappers for [Script Hook V .NET](https://github.com/scripthookvdotnet/scripthookvdotnet) and compatible script hook wrappers.

Compared to the [old generator](https://github.com/NativeFx/InteropGenerator) that was written all the way back 3 years ago, this new generator:

* runs faster (the actual generator cost less than half a second, and the process only takes a few seconds)
* generates code that compiles without `<LangVersion>latest</LangVersion>`
* pre-cached `Hash` instances eliminating the need to cast at runtime

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

You need .NET SDK that can target 10.0. Any OS can build and run the generator, but GTA V doesn't work on anywhere outside of Windows without Wine or Proton.

When building, do *not* build the entire Solution, the build will fail. Build the `WithLithum.NativeWrapperGen` project instead.

To build the `NativeWrapper`, run the ps1 script first.

## Licence

[Apache-2.0](LICENSE.txt)