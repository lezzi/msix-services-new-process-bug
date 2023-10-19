# Overview

This repository contains a reproducible example of an MSIX issue, which leads to a temporarily "corrupted" package state and new processes being blocked from starting.

The test `Installer_TemporaryKey.pfx` certificate is used to sign the MSIX installer (password `Test@123`).

## Steps
1. Publish two different MSIX bundles of the `Installer` project. For example, 1.0.0.0 and 1.1.0.0.
2. Install the lowest version, for example 1.0.0.0.
3. Launch the installed `Desktop Client` application.
 
    ![image](https://github.com/lezzi/msix-services-new-process-bug/assets/5619119/78f2e9b8-7fd8-495a-9ea4-7b0ecbc2da17)
4. Click "Update" and select the higher version MSIX bundle (1.1.0.0).
5. Wait until the "Stop Service" button is enabled.

    ![image](https://github.com/lezzi/msix-services-new-process-bug/assets/5619119/8ae32c38-9c47-47bc-9c70-f6da7d0d1d47)
6. Click "Stop Service" and wait until the "Init WebView2" button is enabled.

   ![image](https://github.com/lezzi/msix-services-new-process-bug/assets/5619119/ef15b52c-4423-4e1a-b637-55c659da05ec)
7. Click "Init WebView2" and observe the error.

   ![image](https://github.com/lezzi/msix-services-new-process-bug/assets/5619119/980ffd5c-b4d8-4d3a-b03b-dc2dccc390cd)


**Actual behavior:** WebView2 cannot be initialized.

**Expected behavior:** WebView2 is properly initialized.

## Additional information
* Logs from the Event Viewer -> Application and Service Logs -> Microsoft -> Windows -> AppModel-Runtime:
  * After the service is stopped:
    * *Failed with 0x5 modifying AppModel Runtime status for package ef97b8f6-483a-4a0c-8fa6-1393d767fbb8_1.0.0.0_x64__yt652g8ggm6tm (current status = 0x200000, desired status = 0x400000).*
  * After attempting to init WebView2:
    * *0x3D0F: Cannot create the process for package <NULL> because an error was encountered while checking the user-level package status. The deployment operation failed because the specified application needs to be registered first.*
* After restarting the app, the package is successfully updated, it is considered registered and WebView2 can be initialized. 
