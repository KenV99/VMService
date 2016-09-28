DISCLAIMER
The author of this program is not afilliated with VMWare, Inc. and makes no claims, explicit or implicit that this program is endorsed by or has any relation to VMWare, Inc. It requires the separate installation of both VMWare Workstation Player and the appropriate VMWare VIX API in order to function properly. Both of there are distributed by VMWare, Inc. 

This FREE program allows the user to run a VMWare(tm) virtual machine as a service.
PREREQUISITES:
Microsoft .NET Framework 4.5.2 or higher
VMWare Workstation Player
VMWare VIX (Do not use the VIX for version 12.5 as it will not work, VIX for 12.1 will work with 12.x versions up to 12.5. You will need to create a free VMWare account to access the link: https://my.vmware.com/group/vmware/free#desktop_end_user_computing/vmware_workstation_player/12_0|PLAYER-1211|drivers_tools )

It will run under Local System account and will reload at reboot, before any user logs in.
Due to the above, the VMWare Player GUI will not be accessible, so ensure the VM works exactly as you desire before attempting to have it start as a service.
The VM can be paused, resumed and stopped from the services.msc control panel.
The user can change which VM is loaded by starting the service manually after stopping it.
Before starting it, change the 'Start parameters' to:
“path_to_vmx" true
substituting "path_to_vmx" to the full path for the vmx file that you wish to use (i.e. “C:\Username\My Documents\Virtual Machines\myVM\myVM.vmx”). The second parameter, ‘true’ instructs the service to update its startup to always use the new path. Manually starting without the second parameter or with the second parameter as ‘false’ will not change the default startup VM.


After the installation has completed, check the following:
Start services.msc and look for the VMX Service and that it is has started
Use the Event Viewer to look under 'Application and Service Logs' for the VMX Service for log messages.
Use the Event Viewer to look under 'Windows Log’ -> ‘Application’ for additional information.
Open the directory into which you installed the files and look at the contents of the file ‘VMService.InstallLog’.
