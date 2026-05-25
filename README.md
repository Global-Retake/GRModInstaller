# Global Retake Installer

Lightweight Windows installer for [GRMod](https://github.com/Skysion3/GRMod).

This project provides a simple classic-style setup application that downloads the latest GRMod release from GitHub and installs it into a Counter-Strike: Global Offensive Legacy folder.

## Features

- Downloads the latest GRMod release directly from `Skysion3/GRMod`
- Supports both `Full install` and `Patch update`
- Defaults to `C:\Program Files (x86)\Steam\steamapps\common\csgo legacy`
- Lets the user choose a custom install folder
- Extracts the selected archive directly into the target folder
- Requests administrator rights for reliable installs inside `Program Files (x86)`
- Auto-detects the Windows UI language
- Includes a built-in language picker inside the installer

## Supported Languages

The installer currently includes these built-in languages:

- Arabic
- Bulgarian
- Catalan
- Chinese Simplified
- Chinese Traditional
- Croatian
- Czech
- Danish
- Dutch
- English
- Finnish
- French
- German
- Greek
- Hungarian
- Italian
- Japanese
- Korean
- Norwegian Bokmal
- Polish
- Portuguese (Brazil)
- Portuguese (Portugal)
- Romanian
- Russian
- Slovak
- Slovenian
- Spanish
- Swedish
- Turkish
- Ukrainian

## How It Works

1. The installer queries the latest GRMod release from GitHub.
2. The user chooses `Full install` or `Patch update`.
3. The archive is downloaded to a temporary folder.
4. The contents are extracted into the selected game directory.
5. Existing files are overwritten during extraction.

## Requirements

- Windows
- Internet connection
- Permission to write to the selected install folder

## Repository Layout

```text
.
|-- README.md
|-- publish.ps1
`-- src/
    `-- GlobalRetakeInstaller/
```

## Build

```powershell
./publish.ps1
```

Published output:

`artifacts\publish\win-x64\GlobalRetakeInstaller.exe`

## Project Notes

- The installer always pulls the latest release at runtime and is not hardcoded to a single GRMod version.
- The application is designed as a single self-contained Windows executable.
- Release metadata and download links are resolved live from GitHub when the installer starts.

## Branding

Copyright Global Retake, globalretake.com
