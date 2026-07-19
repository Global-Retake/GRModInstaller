## v1.1.8

### Added
- Added a non-Defender antivirus fallback flow in the installer.
- If Microsoft Defender cmdlets are unavailable, setup now instructs users to disable antivirus temporarily, add the selected CS:GO folder to exclusions, and confirm before continuing.

### Changed
- Updated release download lookup source to itsIlluMinAty/GRModInstallFiles.

### Fixed
- Prevented installation failure on systems where Microsoft Defender PowerShell cmdlets are unavailable.
