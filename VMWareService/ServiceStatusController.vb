Imports System.Runtime.InteropServices
Public Class ServiceStatusController : Implements IDisposable
    Private successCode, failCode As Integer
    Private oServiceStatus As ServiceStatus, oServiceHandle As IntPtr
    Declare Auto Function SetServiceStatus Lib "advapi32.dll" (ByVal handle As IntPtr, ByRef serviceStatus As ServiceStatus) As Boolean

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    Public Sub New(serviceFunction As Integer, serviceHandle As IntPtr)
        Dim initialCode As Integer
        Dim oServiceStatus As ServiceStatus = New ServiceStatus()
        oServiceStatus.dwWaitHint = 100000
        oServiceHandle = serviceHandle
        Select Case serviceFunction
            Case ServiceFunctions.SERVICE_START
                initialCode = ServiceState.SERVICE_START_PENDING
                successCode = ServiceState.SERVICE_RUNNING
                failCode = ServiceState.SERVICE_STOPPED
            Case ServiceFunctions.SERVICE_STOP
                initialCode = ServiceState.SERVICE_STOP_PENDING
                successCode = ServiceState.SERVICE_STOPPED
                failCode = ServiceState.SERVICE_RUNNING
            Case ServiceFunctions.SERVICE_PAUSE
                initialCode = ServiceState.SERVICE_PAUSE_PENDING
                successCode = ServiceState.SERVICE_PAUSED
                failCode = ServiceState.SERVICE_RUNNING
            Case ServiceFunctions.SERVICE_RESUME
                initialCode = ServiceState.SERVICE_CONTINUE_PENDING
                successCode = ServiceState.SERVICE_RUNNING
                failCode = ServiceState.SERVICE_PAUSED
        End Select
        UpdateStatus(initialCode)
    End Sub
    Private Sub UpdateStatus(code As Integer)
        oServiceStatus.dwCurrentState = code
        SetServiceStatus(oServiceHandle, oServiceStatus)
    End Sub
    Public Sub Fail()
        UpdateStatus(failCode)
    End Sub
    Public Sub Success()
        UpdateStatus(successCode)
    End Sub
    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                oServiceStatus = Nothing
                oServiceHandle = Nothing
                ' TODO: dispose managed state (managed objects).
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        ' TODO: uncomment the following line if Finalize() is overridden above.
        ' GC.SuppressFinalize(Me)
    End Sub
#End Region


End Class
Public Enum ServiceFunctions
    SERVICE_START = 1
    SERVICE_STOP = 2
    SERVICE_PAUSE = 3
    SERVICE_RESUME = 4
End Enum
