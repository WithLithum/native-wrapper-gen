# Changelog

All notable changes to this project will be documented in this very file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- Added original Jenkins hash information to all natives that have a Jenkins
  hash.

### Changed

- Return value documentation is no longer added for the natives that return
  `Void`.
- Upgraded `System.CommandLine` to 2.0 Beta 7.

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

[Unreleased]: https://github.com/WithLithum/native-wrapper-gen/compare/v0.1.2...HEAD
[0.1.2]: https://github.com/WithLithum/native-wrapper-gen/compare/v0.1.1...v0.1.2
[0.1.1]: https://github.com/WithLithum/native-wrapper-gen/compare/v0.1.0...v0.1.1
[0.1.0]: https://github.com/WithLithum/native-wrapper-gen/releases/tag/v0.1.0