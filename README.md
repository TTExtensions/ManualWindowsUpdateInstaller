# Manual Update Installer for Windows 10

This tool allows you to manually install individual updates (and check which updates are available before installing them) on Windows 10 like in previous Windows versions, in contrast to the regular update mechanism that automatically installs all available updates.

The core functionality is borrowed from Windows Server's **WUA_SearchDownloadInstall.vbs** script.

![updateinstaller](https://cloud.githubusercontent.com/assets/15179430/12078740/801d4894-b21e-11e5-8950-e70aef5f6a31.png)

#### Turn off Automatic Updates

For this tool to work you will need to turn off Automatic Updates, otherwise Windows Update will still automatically install all available updates. Note however that this currently doesn't seem to be possible with Windows 10 Home.

**WARNING:** Disabling Automatic Updates means you will not get any automatic notification of new Security Updates!

To to this, run **gpedit.msc** to open the Local Group Policy Editor and go to *Computer Configuration* -> *Administrative Templates* -> *Windows Components* -> *Windows Update* -> *Configure Automatic Updates* and set it to **Disabled**.

## Running the Manual Update Installer

Please see the topic [Running the Update Installer](https://github.com/TTExtensions/ManualWindowsUpdateInstaller/wiki/Running-the-Update-Installer) for guidance how to build and run the Manual Update Installer.

## Alternative: Show or Hide Updates

An alternative to using the Manual Update Installer is to use the [Show or hide updates (wushowhide.diagcab)](https://support.microsoft.com/kb/3073930) tool from Microsoft. This allows you to hide specific updates, then run Windows Update which downloads and installs the non-hidden updates.

Note: Updates that have been hidden with wushowhide.diagcab will still be shown in the Manual Update Installer.

![showhideupdates](https://cloud.githubusercontent.com/assets/15179430/12079446/bb0c4ae8-b239-11e5-991b-8bea59b09e53.png)
