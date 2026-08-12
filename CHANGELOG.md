# Changelog

All notable changes to this project will be documented in this very file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.3.0-beta.3] - 2026-08-12

### Added

- Added support for reading a `SymbolName` property in native definition files.
  - This property overrides the default symbol name, which is case converted from the provided
    R* native name.
  - This is a Native Wrapper Generator extension.
- Added support for using Cfx.re styled hash only native symbol naming.

### Changed

- Made improvement to the documentation generator and native name casing converter to reduce GC
  pressure.

## [0.3.0-beta.2] - 2026-07-19

### Changed

- C# generators now use `Array.Empty<T>()` for natives that takes no parameters.
- The RAGE Plugin Hook generator now use the static typed `CallByHash` for natives that returns `string`.

## [0.3.0-beta.1] - 2026-06-06

### Added

- Added native function namespace to XML documentation remarks.

### Changed

- Internal refactor to expose namespace information.

### Removed

- Removed unused code.

## [0.3.0-alpha.3] - 2026-05-29

_This release is identical with 0.3.0-alpha.2._

### Fixed

- Corrected an issue with automatic NuGet Gallery publishing.

## [0.3.0-alpha.2] - 2026-05-29

_See the corresponding [`migration guide`](docs/MIGRATION.md#0.3.0) for migrating to this version._

### Added

- Added previously used names of natives in the generated XML documentation.

### Changed

- **BREAKING**: Overhauled how type settings work.
  - Consolidated the previous one-to-one map into 7 properties which maps to each of the types.
  - Default configuration files are no longer bundled in the `Data` directory.
- Buffered streams are now used for writing generated files.
- Simplified the generated XML documentation.

### Other

- Improved the build process of pre-built wrappers.

## [0.3.0-alpha.1] - 2026-05-26

### Added

- Added support for generating wrappers for RAGE Plugin Hook.
- Added wrapper project for RAGE Plugin Hook.

### Changes

- Internal refactor to support reuse of common logics.
- Architectural changes to allow adding support of non-SHVDN platforms.

## [0.2.1] - 2026-05-20

### Fixed

- Fixed missing semicolon at the end of return value variable declaration for
  methods that retrieve pointers.

## [0.2.0] - 2026-05-20 [YANKED]

> [!WARNING]
> This version contains a defect that causes the generated wrappers to fail to
> compile.

### Added

- Added original Jenkins hash information to all natives that have a Jenkins
  hash.

### Changed

- Updated generator tool to .NET 10.
- Made some optimisations allocation-wise.
- Return value documentation is no longer added for the natives that return
  `Void`.
- Updated `System.CommandLine` to 2.0.8.
- Updated the pre-built native wrapper to latest updated Native DB data in early May 2026.

## [0.1.2] - 2025-08-19

### Added

- Added a "README" file for the Native Wrapper NuGet package.

### Other

- Bumped the assembly version number.

## [0.1.1] - 2025-08-17

### Fixed

- Fixed an issue resulted in "CLR type" references in parameter documentation comments becoming the
return type of the method in question.

## [0.1.0] - 2025-03-01

_Initial release._

[0.3.0-beta.3]: https://github.com/WithLithum/native-wrapper-gen/compare/v0.3.0-beta.2...v0.3.0-beta.3
[0.3.0-beta.2]: https://github.com/WithLithum/native-wrapper-gen/compare/v0.3.0-beta.1...v0.3.0-beta.2
[0.3.0-beta.1]: https://github.com/WithLithum/native-wrapper-gen/compare/v0.3.0-alpha.3...v0.3.0-beta.1
[0.3.0-alpha.3]: https://github.com/WithLithum/native-wrapper-gen/compare/v0.3.0-alpha.2...v0.3.0-alpha.3
[0.3.0-alpha.2]: https://github.com/WithLithum/native-wrapper-gen/compare/v0.3.0-alpha.1...v0.3.0-alpha.2
[0.3.0-alpha.1]: https://github.com/WithLithum/native-wrapper-gen/compare/v0.2.1...v0.3.0-alpha.1
[0.2.1]: https://github.com/WithLithum/native-wrapper-gen/compare/v0.2.0...v0.2.1
[0.2.0]: https://github.com/WithLithum/native-wrapper-gen/compare/v0.1.2...v0.2.0
[0.1.2]: https://github.com/WithLithum/native-wrapper-gen/compare/v0.1.1...v0.1.2
[0.1.1]: https://github.com/WithLithum/native-wrapper-gen/compare/v0.1.0...v0.1.1
[0.1.0]: https://github.com/WithLithum/native-wrapper-gen/releases/tag/v0.1.0
