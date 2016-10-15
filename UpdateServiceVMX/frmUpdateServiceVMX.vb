Imports System.IO
Imports System.ServiceProcess
Imports Microsoft.Win32

Public Class frmUpdateService

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles btnBrowse.Click
        Dim openFileDialog1 As New OpenFileDialog(), fn As String, strPath As String
        openFileDialog1.Filter = "VM Files|*.vmx"
        openFileDialog1.Title = "Select a VMX File"
        Try
            strPath = Path.GetDirectoryName(tbFileName.Text)
        Catch
            strPath = My.Computer.FileSystem.SpecialDirectories.MyDocuments
        End Try
        openFileDialog1.InitialDirectory = strPath
        openFileDialog1.Multiselect = False

        If openFileDialog1.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
            fn = openFileDialog1.FileName
            If My.Computer.FileSystem.FileExists(fn) Then
                tbFileName.Text = fn
            Else
                MessageBox.Show("The chosen file does not exist", "Update Service VMX", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If
        End If
    End Sub

    Public Sub UpdateImagePath(vmxpath As String)
        Dim regKey As Object, imagePath As String, imagePathS As String, args As String(), servicepath As String, newImagePath As String
        regKey = My.Computer.Registry.LocalMachine.OpenSubKey("SYSTEM\CurrentControlSet\Services\VMXService", True)
        If regKey IsNot Nothing Then
            'Dim perms As New PermissionSet(CType(Nothing, PermissionSet))
            'perms.AddPermission(New RegistryPermission(PermissionState.Unrestricted))
            'perms.PermitOnly()
            imagePath = regKey.GetValue("ImagePath")
            imagePathS = imagePath.Replace(""" """, "~")
            args = imagePathS.Split("~")
            servicepath = args(0)
            newImagePath = servicepath + """ """ + vmxpath + """ ""False"""
            regKey.SetValue("ImagePath", newImagePath, RegistryValueKind.String)
        End If
    End Sub

    Public Function RetrieveImagePath() As String
        Dim regKey As Object, imagePath As String, imagePathS As String, args As String()
        regKey = My.Computer.Registry.LocalMachine.OpenSubKey("SYSTEM\CurrentControlSet\Services\VMXService")
        If regKey IsNot Nothing Then
            imagePath = regKey.GetValue("ImagePath")
            imagePathS = imagePath.Replace(""" """, "~")
            args = imagePathS.Split("~")
            Return args(1)
        Else
            Return ""
        End If
    End Function

    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        If My.Computer.FileSystem.FileExists(tbFileName.Text) Then
            Dim result As Boolean = StopService()
            UpdateImagePath(tbFileName.Text)
            MessageBox.Show("Path Updated", "Update Service VMX", MessageBoxButtons.OK, MessageBoxIcon.Information)
            If result = True Then
                StartService()
                Me.Close()
            Else
                MessageBox.Show("The service could not be stopped" & vbCrLf & "Please manually stop and restart it", "Update Service VMX",
                 MessageBoxButtons.OK, MessageBoxIcon.Information)
                Me.Close()
            End If
        Else
                Dim resp = MessageBox.Show("The specified file does not exist", "Update Service VMX", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error)
            If resp = DialogResult.Cancel Then
                Me.Close()
            End If
        End If
    End Sub

    Private Sub frmUpdateService_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        Dim fn As String = RetrieveImagePath()
        If fn = "" Then
            MessageBox.Show("VMX Service is not installed. Program will Exit", "Update Service VMX", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.Close()
        Else
            If Not My.Computer.FileSystem.FileExists(fn) Then
                MessageBox.Show("The currently configured file does not exist", "Update Service VMX", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If
            tbFileName.Text = fn
        End If
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.Close()
    End Sub
    Private Function StopService() As Boolean
        Dim sc As New ServiceController("VMXService"), x As ServiceControllerStatus
        If sc.Status() <> ServiceControllerStatus.Stopped Then
            Me.Cursor = Cursors.WaitCursor
            sc.Stop()
            Dim stopped As Boolean
            Dim timeout = TimeSpan.FromMilliseconds(30000)
            Try
                sc.WaitForStatus(ServiceControllerStatus.Stopped, timeout)
                stopped = True
            Catch ex As TimeoutException
                stopped = False
            End Try
            Me.Cursor = Cursors.Default
            Return stopped
        Else
            Return True
        End If
    End Function
    Private Sub StartService()
        Dim sc As New ServiceController("VMXService")
        Dim result = MessageBox.Show("Do you want to start the service with the newly updated VMX?", "Update Service VMX",
           MessageBoxButtons.YesNo, MessageBoxIcon.Information)
        If result = DialogResult.Yes Then
            Me.Cursor = Cursors.WaitCursor
            sc.Start()
            Dim running As Boolean
            Dim timeout = TimeSpan.FromMilliseconds(30000)
            Try
                sc.WaitForStatus(ServiceControllerStatus.Running, timeout)
                running = True
            Catch ex As TimeoutException
                running = False
            End Try
            Me.Cursor = Cursors.Default
            If running Then
                MessageBox.Show("Service restarted successfully", "Update Service VMX",
           MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                MessageBox.Show("Failed to restart service. Please check event logs", "Update Service VMX",
           MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        End If
    End Sub
End Class
