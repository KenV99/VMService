Public Class VIX
    Public Shared Property VMXpath As String
    Public Shared Property VIXPath As String
    Private cmdstart, cmdstop, cmdpause, cmdresume, cmdlist As String
    Public Shared Property numVMs_running As Integer
    Public Shared Property VMs_running As ArrayList
    Private Logger As Object

    Public Sub New(Optional ByRef pathVMX = "", Optional logfxn = Nothing)
        VMXpath = pathVMX
        VIXPath = GetVMrunPath()
        Logger = logfxn
        InitializeCmds()
    End Sub
    Private Function GetVMrunPath() As String
        Dim AppPath As String, regKey As Object
        regKey = My.Computer.Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\vmplayer.exe")
        AppPath = regKey.GetValue("Path")
        AppPath = System.IO.Path.GetDirectoryName(AppPath)
        AppPath = System.IO.Path.GetDirectoryName(AppPath)
        AppPath = AppPath + "\VMWare VIX\vmrun.exe"
        Return AppPath
    End Function

    Private Sub InitializeCmds()
        cmdstart = "start """ + VMXpath + """ nogui"
        cmdstop = "stop """ + VMXpath + """ soft"
        cmdpause = "pause """ + VMXpath + """"
        cmdresume = "unpause """ + VMXpath + """"
        cmdlist = "list"
    End Sub

    Public Function VMXStart() As Boolean
        Dim sRet As String
        sRet = RunCommand(cmdstart)
        Return IsRunningStart()
    End Function

    Public Function VMXStop() As Boolean
        Dim sRet As String
        sRet = RunCommand(cmdstop)
        Return Not IsRunning()
    End Function

    Public Function VMXPause() As Boolean
        Dim sRet As String
        sRet = RunCommand(cmdpause)
        Return True
    End Function

    Public Function VMXResume() As Boolean
        Dim sRet As String
        sRet = RunCommand(cmdresume)
        Return True
    End Function

    Private Function RunCommand(arguments As String, Optional log As Boolean = True) As String
        Dim psi As New ProcessStartInfo
        Dim soutput
        psi.FileName = VIXPath
        psi.Arguments = arguments
        psi.RedirectStandardError = True
        psi.RedirectStandardOutput = True
        psi.CreateNoWindow = False
        psi.WindowStyle = ProcessWindowStyle.Hidden
        psi.UseShellExecute = False

        Dim process As Process = Process.Start(psi)
        soutput = process.StandardOutput.ReadToEnd()
        process.WaitForExit(10000)
        If Not process.HasExited Then
            process.Kill()
            soutput += " | Process exceeded timeout and was killed"
        End If
        If soutput <> "" Then
            If log = True Then
                If Logger Is Nothing Then
                    Console.WriteLine(soutput)
                Else
                    Logger.WriteEntry(soutput)
                End If
            End If
        End If
        Return soutput
    End Function
    Public Sub UpdateList()
        Dim soutput As String
        soutput = RunCommand(cmdlist, False)
        Dim lines As String(), nTmp As String()
        lines = soutput.Replace(vbCrLf, "~").Split("~")
        nTmp = lines(0).Split(":")
        numVMs_running = Int(nTmp(1))
        Array.Copy(lines, 1, lines, 0, lines.Length - 1)
        If numVMs_running > 0 Then
            VMs_running = New ArrayList
            For Each line In lines
                If line <> "" Then
                    VMs_running.Add(line)
                End If
            Next
        Else
            VMs_running = New ArrayList
        End If
    End Sub
    Public Function IsRunning() As Boolean
        UpdateList()
        If VMs_running.Contains(VMXpath) Then
            Return True
        Else
            Return False
        End If
    End Function
    Public Function IsRunningStart() As Boolean
        Dim i As Integer
        For i = 1 To 12
            If IsRunning() Then
                Return True
            Else
                Threading.Thread.Sleep(5000)
            End If
        Next
        Return False
    End Function
End Class
