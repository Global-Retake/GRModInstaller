# Global Retake Installer

Lightweight Windows installer for GRMod.

---

## Badges

![Release](https://img.shields.io/github/v/release/Global-Retake/GRModInstaller?style=for-the-badge)
![Downloads](https://img.shields.io/github/downloads/Global-Retake/GRModInstaller/total?style=for-the-badge)
![Stars](https://img.shields.io/github/stars/Global-Retake/GRModInstaller?style=for-the-badge)
![Forks](https://img.shields.io/github/forks/Global-Retake/GRModInstaller?style=for-the-badge)
![Issues](https://img.shields.io/github/issues/Global-Retake/GRModInstaller?style=for-the-badge)
![Platform](https://img.shields.io/badge/platform-windows-0078D6?style=for-the-badge&logo=windows)

---

## Overview

A lightweight Windows installer for [GRMod](https://github.com/itsIlluMinAty/GRMod).

GRMod brings back the 2023 experience to CSGO!

The installer automatically handles:
- Release detection
- Installation
- Patch updates
- Cleanup and uninstall support

---

## Download

Download the latest release:

[![Download Latest Release](https://img.shields.io/badge/DOWNLOAD-LATEST%20RELEASE-2ea44f?style=for-the-badge&logo=windows)](https://github.com/Global-Retake/GRModInstaller/releases/latest)

---

## Installation

1. Run `GlobalRetakeInstaller.exe`
2. Select installation type:
   - Full install
   - Patch update
3. Choose installation directory
4. Confirm antivirus exclusions
   - If Microsoft Defender is available, exclusions are configured automatically.
   - If another antivirus is detected, follow the prompt to disable AV temporarily, add the selected CS:GO folder to exclusions, then continue.
5. Wait for download and extraction to complete

---

## Uninstallation

1. Run the installer
2. Select installed GRMod directory
3. Click Uninstall
4. Confirm removal

The uninstaller will:
- Remove installed files tracked by the installer
- Restore original `csgo.exe` if replaced
- Remove Defender exclusions added by the installer

---

## Screenshots

### Main Window
![Main Window](assets/screenshots/installer-home.png)

### Download Process
![Download Process](assets/screenshots/installer-options.png)

### Installation Complete
![Installation Complete](assets/screenshots/installer-progress.png)

---

## Features

- Automatically fetches latest GRMod release from GitHub
- Full install and patch update support
- Default CS:GO Legacy path detection
- Custom install directory support
- Microsoft Defender exclusion handling with manual antivirus fallback
- Safe file extraction into target directory
- Backup system for `csgo.exe` (`csgo.exe.old`)
- Built-in uninstaller with rollback support
- Local state tracking for installed files
- Administrator privilege support
- Multi-language support
- Automatic Windows language detection

---

## Supported Languages

Arabic, Bulgarian, Catalan, Chinese Simplified, Chinese Traditional, Croatian, Czech, Danish, Dutch, English, Finnish, French, German, Greek, Hungarian, Italian, Japanese, Korean, Norwegian Bokmål, Polish, Portuguese (Brazil), Portuguese (Portugal), Romanian, Russian, Slovak, Slovenian, Spanish, Swedish, Turkish, Ukrainian

---

## How It Works

1. Queries latest GRMod release from GitHub API
2. User selects install mode
3. Defender exclusions are applied
4. Installer downloads release archive
5. Files are extracted into game directory
6. Existing files are safely overwritten
7. `csgo.exe` is backed up if replaced
8. Local state file is created for uninstall

---

## Requirements

- Windows 10 or newer
- Internet connection
- Write permissions to install directory
- Administrator rights (recommended for Program Files)

---

## Community

Join the official Discord server:

[![Discord](https://img.shields.io/discord/1436143382723297350?cacheSeconds=60&style=for-the-badge&logo=discord&color=5865F2)](https://discord.gg/zawBAWuYEg)

---

## Credits

- GRMod by itsIlluMinAty: https://github.com/itsIlluMinAty/GRMod
- csgo_gc by Mikko Kokko: https://github.com/mikkokko/csgo_gc

---

## License

This project is licensed under MIT with Commons Clause.

You may use, modify, and redistribute it under the license terms.

Selling the software, or paid distribution where value is primarily derived from this software, is not permitted.

See LICENSE for full details.

---

## Branding

Global Retake  
globalretake.com
