Public Class VMService
    Dim mVIX As VIX
    Protected Overrides Sub OnStart(ByVal args() As String)
        MyBase.OnStart(args)
        Using ssc As New ServiceStatusController(ServiceFunctions.SERVICE_START, Me.ServiceHandle)
            Dim success As Boolean
            EventLog1.WriteEntry("Attempting Start")
            success = mVIX.VMXStart()
            If success Then
                ssc.Success()
                EventLog1.WriteEntry("Service Started")
            Else
                ssc.Fail()
                EventLog1.WriteEntry("Service Failed to Start", EventLogEntryType.Error)
            End If
        End Using
    End Sub
    Protected Overrides Sub OnStop()
        MyBase.OnStop()
        Using ssc As New ServiceStatusController(ServiceFunctions.SERVICE_STOP, Me.ServiceHandle)
            Dim success As Boolean
            success = mVIX.VMXStop()
            If success Then
                ssc.Success()
                EventLog1.WriteEntry("Service Stopped")
            Else
                ssc.Fail()
                EventLog1.WriteEntry("Service Failed to Stop", EventLogEntryType.Error)
            End If
        End Using
    End Sub

    Protected Overrides Sub OnContinue()
        MyBase.OnContinue()
        Using ssc As New ServiceStatusController(ServiceFunctions.SERVICE_RESUME, Me.ServiceHandle)
            Dim success As Boolean
            success = mVIX.VMXPause()
            ssc.Success()
            EventLog1.WriteEntry("Service Resumed")
        End Using
    End Sub

    Protected Overrides Sub OnPause()
        MyBase.OnPause()
        Using ssc As New ServiceStatusController(ServiceFunctions.SERVICE_PAUSE, Me.ServiceHandle)
            Dim success As Boolean
            success = mVIX.VMXResume()
            ssc.Success()
            EventLog1.WriteEntry("Service Paused")
        End Using
    End Sub

    Protected Overrides Sub OnShutdown()
        OnStop()
        MyBase.OnShutdown()
    End Sub
    Public Sub UpdateImagePath(vmxpath As String)
        Dim regKey As Object, imagePath As String, imagePathS As String, args As String(), servicepath As String, newImagePath As String
        regKey = My.Computer.Registry.LocalMachine.OpenSubKey("SYSTEM\CurrentControlSet\Services\VMWareService")
        If regKey IsNot Nothing Then
            imagePath = regKey.GetValue("ImagePath")
            imagePathS = imagePath.Replace(""" """, "~")
            args = imagePathS.Split("~")
            servicepath = args(0)
            newImagePath = servicepath + """ """ + vmxpath + """ ""False"""
            regKey.SetValue("ImagePath", newImagePath)
        End If
    End Sub
    'Sub Test()
    '    UpdateImagePath("G:\Ken User\My Documents\Virtual Machines\ES-UServer\ES-UServer.vmx")
    '    Dim bRet As Boolean
    '    mVIX = New VIX("G:\Ken User\My Documents\Virtual Machines\ES-UServer\ES-UServer.vmx")
    '    bRet = mVIX.VMXStart()
    '    Stop
    '    bRet = mVIX.IsRunning()
    '    bRet = mVIX.VMXStop
    '    Stop
    'End Sub
End Class