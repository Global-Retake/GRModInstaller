# Global Retake Installer

Lightweight Windows installer for [GRMod](https://github.com/Skysion3/GRMod).

A straightforward Windows installer for Counter-Strike: Global Offensive Legacy players who want the latest GRMod build without doing the download and extraction steps by hand.

The installer checks the current release on GitHub, lets the player choose a full install or patch update, adds the install and temp folders to Microsoft Defender exclusions, and installs it to the default game folder or any custom path they choose.

## Download

Download the single-file `GlobalRetakeInstaller.exe` from the repository's `Releases` page.

## Installation

1. Launch `GlobalRetakeInstaller.exe`
2. Choose `Full install` or `Patch update`
3. Confirm or change the install folder
4. Let the installer add Microsoft Defender exclusions for the install folder and installer temp folder
5. Let the installer download and extract the latest GRMod release

## Uninstall

1. Launch `GlobalRetakeInstaller.exe`
2. Select the same GRMod folder that was installed through this app
3. Click `Uninstall`
4. Confirm the removal

The uninstall flow removes the files tracked by the installer and rolls back any Defender exclusions that the installer itself added.

## Screenshots

### Main Window

![Global Retake Installer main window](assets/screenshots/installer-home.png)

### Download Progress

![Global Retake Installer downloading the latest GRMod release](assets/screenshots/installer-options.png)

### Completed Install

![Global Retake Installer successful install dialog](assets/screenshots/installer-progress.png)

## Features

- Downloads the latest GRMod release directly from `Skysion3/GRMod`
- Supports both `Full install` and `Patch update`
- Defaults to `C:\Program Files (x86)\Steam\steamapps\common\csgo legacy`
- Lets the user choose a custom install folder
- Adds Microsoft Defender exclusions for the selected install folder and the installer's temp workspace
- Extracts the selected archive directly into the target folder
- Renames an existing `csgo.exe` to `csgo.exe.old` before replacing it, so the original launcher can be restored later
- Includes a built-in uninstall option that removes tracked files and reverses installer-added exclusions
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
3. The installer adds Microsoft Defender exclusions for the selected install folder and its temp workspace.
4. The archive is downloaded to a temporary folder.
5. The contents are extracted into the selected game directory.
6. Existing files are overwritten during extraction.
7. If the release replaces `csgo.exe`, the existing launcher is renamed to `csgo.exe.old` first.
8. The installer writes a small local state file so the uninstall option can remove tracked files later and restore the original launcher.

## Requirements

- Windows
- Internet connection
- Permission to write to the selected install folder

## Publishing

Release builds should be shipped as a single-file Windows executable for easier distribution.

Build the release exe with:

```powershell
dotnet publish .\src\GlobalRetakeInstaller\GlobalRetakeInstaller.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:EnableCompressionInSingleFile=true -p:DebugSymbols=false -p:DebugType=None -o .\dist\single-file
```

The ship-ready file is:

- `dist\single-file\GlobalRetakeInstaller.exe`

## License

This repository is source-available and free to use, modify, and share, but selling the software or paid copies of it is not allowed. See [LICENSE](LICENSE) for the full terms.

## Credits

- [Skysion3/GRMod](https://github.com/Skysion3/GRMod) by Illuminaty and Parker
- [mikkokko/csgo_gc](https://github.com/mikkokko/csgo_gc) by Mikko Kokko

## Branding

Copyright Global Retake, globalretake.com
