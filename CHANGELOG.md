# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

##  [vnext]

### Added
- Improved ability to resolve destination types for Map method.

## [0.3.0] - 2020-12-05

### Added
- Support for mapping to method return type.
- Support using Map for arguments to method in the same class.

### Fixed
- The code generator is now actually picked up in consuming projects.

## [0.2.0] - 2020-12-02
### Added
- A matching constructor is used for the result of the Map function when available. Otherwise,
	the empty constructor is used (as before).

## [0.1.0] - 2020-11-29
### Added
- Initial package.
- Types can be mapped to supported destination types using `Map` method.
- Source mapping type can be extended with additional fields using `Extend` method.