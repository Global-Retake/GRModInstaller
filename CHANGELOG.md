# Changelog

## [v1.1.8] - 2026-07-19

### Added
- Added a non-Defender antivirus fallback flow in the installer.
- If Microsoft Defender cmdlets are unavailable, setup now instructs users to disable their antivirus temporarily, add the selected CS:GO folder to exclusions, and then confirm to proceed.

### Changed
- Updated release download lookup source to `itsIlluMinAty/GRModInstallFiles`.

### Fixed
- Prevented installer failure on systems that do not expose Microsoft Defender PowerShell cmdlets.

## [v1.1.7] - 2026-05-31

### Added
- Updated GitHub release lookup to use `itsIlluMinAty/GRModInstallFiles` continuous release tag.
- Improved Windows asset selection for new `_Full-windows-latest` and `_Patch-windows-latest` release names.

### Fixed
- Corrected installer state file location handling.
- Applied repository metadata updates and cleanup from the latest upstream merge.
- Added stability fixes and release support for the latest GRMod installer flow.
