Imports System.ComponentModel
Imports System.Configuration.Install

Public Class ProjectInstaller
    Public Sub New()
        MyBase.New()

        'This call is required by the Component Designer.
        InitializeComponent()

        'Add initialization code after the call to InitializeComponent

    End Sub
    Protected Overrides Sub OnBeforeInstall(ByVal savedState As IDictionary)
        Dim parameter As String, vmxpath As String
        Dim AppPath As String, regKey As Object
        Try
            vmxpath = Context.Parameters("vmx")
        Catch
            Console.WriteLine("ERROR: VMX not passed in commandline: /VMX=""pathtovmx""")
            Throw New System.Configuration.Install.InstallException("ERROR: VMX not passed in commandline: /VMX=""pathtovmx""")
        End Try
        If Not My.Computer.FileSystem.FileExists(vmxpath) Then
            Console.WriteLine("ERROR: VMX file not found: " + vmxpath)
            Throw New System.Configuration.Install.InstallException("ERROR: VMX not found: " + vmxpath)
        End If
        regKey = My.Computer.Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\vmplayer.exe")
        If regKey IsNot Nothing Then
            AppPath = regKey.GetValue("Path")
        Else
            Console.WriteLine("ERROR: vmplayer.exe not found")
            Throw New System.Configuration.Install.InstallException("ERROR: vmplayer.exe not found")
        End If
        AppPath = System.IO.Path.GetDirectoryName(AppPath)
        AppPath = System.IO.Path.GetDirectoryName(AppPath)
        AppPath = AppPath + "\VMWare VIX\vmrun.exe"
        If Not My.Computer.FileSystem.FileExists(AppPath) Then
            Console.WriteLine("ERROR: VMWare VIX\vmrun.exe not found: " + AppPath)
            Throw New System.Configuration.Install.InstallException("ERROR: VMWare VIX\vmrun.exe Not found")
        End If
        parameter = vmxpath + """ ""False"
        Context.Parameters("assemblypath") = """" + Context.Parameters("assemblypath") + """ """ + parameter + """"
        MyBase.OnBeforeInstall(savedState)
    End Sub
    Protected Overrides Sub OnAfterInstall(ByVal savedState As IDictionary)
        Using serviceController As New System.ServiceProcess.ServiceController(ServiceInstaller1.ServiceName)
            serviceController.Start()
        End Using
        MyBase.OnAfterInstall(savedState)
    End Sub
    Protected Overrides Sub OnBeforeUnInstall(ByVal savedState As IDictionary)
        Try
            Using serviceController As New System.ServiceProcess.ServiceController(ServiceInstaller1.ServiceName)
                serviceController.Stop()
            End Using
        Catch ex As Exception
            'Do nothing - service not started
        End Try

        MyBase.OnBeforeInstall(savedState)
    End Sub
    Protected Overrides Sub OnBeforeRollback(ByVal savedState As IDictionary)
        Try
            Using serviceController As New System.ServiceProcess.ServiceController(ServiceInstaller1.ServiceName)
                serviceController.Stop()
            End Using
        Catch ex As Exception
            'Do nothing - service not started
        End Try
        MyBase.OnBeforeRollback(savedState)
    End Sub
    Private Sub ServiceProcessInstaller1_AfterInstall(sender As Object, e As InstallEventArgs) Handles ServiceProcessInstaller1.AfterInstall

    End Sub

    Private Sub ServiceInstaller1_AfterInstall(sender As Object, e As InstallEventArgs) Handles ServiceInstaller1.AfterInstall

    End Sub
End Class