; Script generated by the Inno Script Studio Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!
#include <idp.iss>
#define MyAppName "VMService"
#define MyAppVersion "1.0.0.9"
#define MyAppPublisher "KenV99"
#define MyAppExeName "UpdateServiceVMX.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{2B109371-D5F2-4DB2-A1A1-D57BA3A81E01}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={pf}\VMWare\VMService
DefaultGroupName={#MyAppName}
AllowNoIcons=yes
LicenseFile=userdocs:Visual Studio 2015\Projects\VMWareServiceExtra\LICENSE.txt
InfoBeforeFile=userdocs:Visual Studio 2015\Projects\VMWareServiceExtra\Before Install.rtf
InfoAfterFile=userdocs:Visual Studio 2015\Projects\VMWareServiceExtra\After Install.rtf
OutputDir=G:\Ken User\My Documents\Visual Studio 2015\Projects\VMWareServiceExtra\InnoSetup
OutputBaseFilename=setup
Compression=lzma
SolidCompression=yes
UninstallDisplayName=VMService

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "G:\Ken User\My Documents\Visual Studio 2015\Projects\VMWareService\VMWareService\bin\Release\app.publish\VMService.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "LICENSE.txt"; DestDir: "{app}"; Flags: ignoreversion
Source: "README.txt"; DestDir: "{app}"; Flags: ignoreversion
; NOTE: Don't use "Flags: ignoreversion" on any shared system files
Source: "..\bin\Release\app.publish\Application Files\VMService_1_0_0_12\VMService.exe.manifest"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\bin\Release\app.publish\Application Files\VMService_1_0_0_12\VMService.exe.config.deploy"; DestDir: "{app}"; DestName: "VMService.exe.config"; Flags: ignoreversion
Source: "..\..\UpdateServiceVMX\bin\Release\UpdateServiceVMX.vshost.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\..\UpdateServiceVMX\bin\Release\UpdateServiceVMX.vshost.exe.manifest"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\..\UpdateServiceVMX\bin\Release\UpdateServiceVMX.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\..\UpdateServiceVMX\bin\Release\UpdateServiceVMX.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\..\UpdateServiceVMX\bin\Release\UpdateServiceVMX.vshost.exe"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"

[Run]
; Filename: {dotnet40}\InstallUtil.exe; Parameters: "/VMX=""{code:VMXLocation}""  ""{app}\VMWareService.exe"""; Flags: runascurrentuser runhidden

[UninstallRun]
;Filename: {dotnet40}\InstallUtil.exe; Parameters: "/u ""{app}\VMWareService.exe"""; Flags: runascurrentuser runhidden

[Code]
var
  Page: TInputFileWizardPage;

function Framework45IsNotInstalled(): Boolean;
  var
    bSuccess: Boolean;
    regVersion: Cardinal;
    begin
      Result := True;
      bSuccess := RegQueryDWordValue(HKLM, 'Software\Microsoft\NET Framework Setup\NDP\v4\Full', 'Release', regVersion);
      if (True = bSuccess) and (regVersion >= 378389) then begin
        Result := False;
    end;
end;

procedure InitializeWizard;    
    begin
      if Framework45IsNotInstalled() then
      begin
        idpAddFile('http://go.microsoft.com/fwlink/?LinkId=397707', ExpandConstant('{tmp}\NetFrameworkInstaller.exe'));
        idpDownloadAfter(wpReady);
      end;

    begin
    // Create the page
    Page := CreateInputFilePage(wpSelectTasks,
      'Select VMX file to be run', 'Select .vmx file',
      'Select the .vmx file to be used, then click Next.');

    // Add item
    Page.Add('Location of .vmx file:',         // caption
      'Virtual machine files|*.vmx|All files|*.*',    // filters
      '.vmx');                                   // default extension
    end;
end;

function VMXLocation(t: String): String;
  begin
    Result:=Page.Values[0];
  end;

procedure InstallFramework;
var
  StatusText: string;
  ResultCode: Integer;
begin
  StatusText := WizardForm.StatusLabel.Caption;
  WizardForm.StatusLabel.Caption := 'Installing .NET Framework 4.5.2. This might take a few minutes�';
  WizardForm.ProgressGauge.Style := npbstMarquee;
  try
    if not Exec(ExpandConstant('{tmp}\NetFrameworkInstaller.exe'), '/passive /norestart', '', SW_SHOW, ewWaitUntilTerminated, ResultCode) then
    begin
      MsgBox('.NET installation failed with code: ' + IntToStr(ResultCode) + '.', mbError, MB_OK);
    end;
  finally
    WizardForm.StatusLabel.Caption := StatusText;
    WizardForm.ProgressGauge.Style := npbstNormal;

    DeleteFile(ExpandConstant('{tmp}\NetFrameworkInstaller.exe'));
  end;
end;

procedure InstallService;
  var
    iu: string;
    vmx: string;
    svc: string;
    ResultCode : Integer;
  begin
    iu := ExpandConstant('{dotnet40}\InstallUtil.exe');
    vmx := Page.Values[0];
    svc := ExpandConstant('{app}\VMService.exe');
    Exec(iu, '/VMX="' + vmx + '" "' + svc + '"', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
  end;

procedure CurStepChanged(CurStep: TSetupStep);
var
  tmp: String;
begin
  case CurStep of
    ssPostInstall:
      begin
        if Framework45IsNotInstalled() then
        begin
          InstallFramework();
        end;
        InstallService();
      end;
   end;
end;

procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
  var
    iu: string;
    svc: string;
    ResultCode : Integer;
  begin
    If CurUninstallStep = usUninstall then
      begin
        iu := ExpandConstant('{dotnet40}\InstallUtil.exe');
        svc := ExpandConstant('{app}\VMService.exe');
        Exec(iu, '/u "' + svc + '"', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
      end;
  end;

