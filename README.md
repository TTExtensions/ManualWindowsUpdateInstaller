# Manual Update Installer for Windows 10

This tool allows you to manually install individual updates (and check which updates are available before installing them) on Windows 10 like in previous Windows versions, in contrast to the regular update mechanism that automatically installs all available updates.

The core functionality is borrowed from Windows Server's **WUA_SearchDownloadInstall.vbs** script.

![updateinstaller](https://cloud.githubusercontent.com/assets/15179430/12064360/7105a766-afc1-11e5-89e7-dd7ca9ca256f.png)

#### Turn off Automatic Updates

For this tool to work you will need to turn off Automatic Updates, otherwise Windows Update will still automatically install all available updates. Note however that this currently doesn't seem to be possible with Windows 10 Home.

To to this, run **gpedit.msc** to open the Local Group Policy Editor and go to *Computer Configuration* -> *Administrative Templates* -> *Windows Components* -> *Windows Update* -> *Configure Automatic Updates* and set it to **Disabled**:
![gpedit-updates](https://cloud.githubusercontent.com/assets/15179430/12064384/1c061d76-afc2-11e5-9ca3-d55db83a1e49.png)

## Running the Manual Update Installer

Please see the topic [Running the Update Installer](https://github.com/TTExtensions/ManualWindowsUpdateInstaller/wiki/Running-the-Update-Installer) for guidance how to build and run the Manual Update Installer.