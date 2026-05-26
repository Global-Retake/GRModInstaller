# Global Retake Installer

Lightweight Windows installer for [GRMod](https://github.com/Skysion3/GRMod).

A straightforward Windows installer for Counter-Strike: Global Offensive Legacy players who want the latest GRMod build without doing the download and extraction steps by hand.

The installer checks the current release on GitHub, lets the player choose a full install or patch update, and installs it to the default game folder or any custom path they choose.

## Download

Download the latest installer from the repository's `Releases` page.

## Installation

1. Launch `GlobalRetakeInstaller.exe`
2. Choose `Full install` or `Patch update`
3. Confirm or change the install folder
4. Let the installer download and extract the latest GRMod release

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

## License

This repository is source-available and free to use, modify, and share, but selling the software or paid copies of it is not allowed. See [LICENSE](LICENSE) for the full terms.

## Credits

- [Skysion3/GRMod](https://github.com/Skysion3/GRMod) by Illuminaty and Parker
- [mikkokko/csgo_gc](https://github.com/mikkokko/csgo_gc) by Mikko Kokko

## Branding

Copyright Global Retake, globalretake.com
