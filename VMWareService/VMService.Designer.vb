Imports System.ServiceProcess
Imports System.Diagnostics
Imports System.Runtime.InteropServices

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class VMService
    Inherits System.ServiceProcess.ServiceBase

    'UserService overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    ' The main entry point for the process
    <MTAThread()>
    <System.Diagnostics.DebuggerNonUserCode()>
    Shared Sub Main(ByVal cmdArgs() As String)
        Dim ServicesToRun() As System.ServiceProcess.ServiceBase
        ServicesToRun = New System.ServiceProcess.ServiceBase() {New VMService(cmdArgs)}
        System.ServiceProcess.ServiceBase.Run(ServicesToRun)
    End Sub


    'Required by the Component Designer
    Private components As System.ComponentModel.IContainer


    ' NOTE: The following procedure is required by the Component Designer
    ' It can be modified using the Component Designer.  
    ' Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.EventLog1 = New System.Diagnostics.EventLog()
        CType(Me.EventLog1, System.ComponentModel.ISupportInitialize).BeginInit()
        '
        'VMWareService
        '
        Me.ServiceName = "VMXService"
        CType(Me.EventLog1, System.ComponentModel.ISupportInitialize).EndInit()

    End Sub

    Friend WithEvents EventLog1 As EventLog

    Public Sub New(ByVal cmdArgs() As String)
        MyBase.New()
        InitializeComponent()
        ' This call is required by the designer.
        MyBase.CanPauseAndContinue = True
        MyBase.CanStop = True
        MyBase.CanShutdown = True

        Dim eventSourceName As String = "VMXServiceX"
        Dim logName As String = "VMXServiceLog"
        EventLog1 = New System.Diagnostics.EventLog()
        If (Not System.Diagnostics.EventLog.SourceExists(eventSourceName)) Then
            System.Diagnostics.EventLog.CreateEventSource(eventSourceName, logName)
        End If
        EventLog1.Source = eventSourceName
        EventLog1.Log = logName

        Dim vmxpath As String
        If (cmdArgs.Count() > 0) Then
            vmxpath = cmdArgs(0)
            If My.Computer.FileSystem.FileExists(vmxpath) Then
                If (cmdArgs.Count() > 1) Then
                    If cmdArgs(1) = "True" Then
                        UpdateImagePath(vmxpath)
                        EventLog1.WriteEntry("VMXpath updated")
                    End If
                End If
            Else
                EventLog1.WriteEntry("VMXpath invalid: " + vmxpath, EventLogEntryType.Error)
                Throw New System.Exception("VMXpath invalid: " + vmxpath)
            End If
        Else
            EventLog1.WriteEntry("VMXpath missing", EventLogEntryType.Error)
            Throw New System.Exception("Path to VMX file missing")
        End If
        mVIX = New VIX(vmxpath, EventLog1)
    End Sub
End Class

Public Enum ServiceState
    SERVICE_STOPPED = 1
    SERVICE_START_PENDING = 2
    SERVICE_STOP_PENDING = 3
    SERVICE_RUNNING = 4
    SERVICE_CONTINUE_PENDING = 5
    SERVICE_PAUSE_PENDING = 6
    SERVICE_PAUSED = 7
End Enum

<StructLayout(LayoutKind.Sequential)>
Public Structure ServiceStatus
    Public dwServiceType As Long
    Public dwCurrentState As ServiceState
    Public dwControlsAccepted As Long
    Public dwWin32ExitCode As Long
    Public dwServiceSpecificExitCode As Long
    Public dwCheckPoint As Long
    Public dwWaitHint As Long
End Structure