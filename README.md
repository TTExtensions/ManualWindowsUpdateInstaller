# Manual Update Installer for Windows 10

This tool allows you to manually install individual updates (and check which updates are available before installing them) on Windows 10 like in previous Windows versions, in contrast to the regular update mechanism that automatically installs all available updates.

The functionality is borrowed from Windows Server's **WUA_SearchDownloadInstall.vbs** script.

![updateinstaller](https://cloud.githubusercontent.com/assets/15179430/12044603/b0706400-ae94-11e5-9ddf-2c6657f8a1e1.png)

Note: For this tool to work you need to disable automatic updates in Group Policy; otherwise Windows Update will still automatically install all available updates.

To to this, open gpedit.msc and go to *Computer Configuration* -> *Administrative Templates* -> *Windows Components* -> *Windows Update* -> *Configure Windows Updates* and set it to **Disabled**:
![gpedit-updates](https://cloud.githubusercontent.com/assets/15179430/12044536/5049c752-ae93-11e5-8b2f-df2acb9f711e.png)

## Running the Manual Update Installer

Please see the topic [Running the Update Installer](https://github.com/TTExtensions/ManualWindowsUpdateInstaller/wiki/Running-the-Update-Installer) for guidance how to build and run the Manual Update Installer.