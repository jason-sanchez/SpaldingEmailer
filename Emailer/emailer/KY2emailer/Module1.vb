'20170111 - Change relay to relay.catholichealth.net port 25 per Mohammed 12/30/2016

Imports System.Net.Mail
Imports System.IO
Imports System.Configuration


Module Module1

    Dim fileCount As Integer = 0
    Dim strMessage As String = ""
    Dim globalError As Boolean = False
    Dim gblLogString As String = ""
    Dim DFTErrorDir As DirectoryInfo = New DirectoryInfo(System.Configuration.ConfigurationManager.AppSettings("DFTErrorDir"))
    Dim strLogDirectory As String = System.Configuration.ConfigurationManager.AppSettings("ErrorLog").ToString()

    Sub Main()

        Try
            Call process_SpaldingDFT()


            If fileCount > 0 Then
                mailIT(strMessage)
            End If

        Catch ex As Exception
            globalError = True

            gblLogString = gblLogString & "Spalding Emailer Error" & vbCrLf
            gblLogString = gblLogString & ex.Message & vbCrLf
            gblLogString = gblLogString & strMessage & vbCrLf
            writeToLog(gblLogString, 1)

            Exit Sub
        End Try

    End Sub

    Private Sub process_SpaldingDFT()
        Try


            Dim DFTErrorDirStr As String = System.Configuration.ConfigurationManager.AppSettings("DFTErrorDir").ToString()
            strMessage = strMessage & vbCrLf

            strMessage = strMessage & "Spalding DFT Status as of " & CStr(DateTime.Now) & vbCrLf

            Dim DFTerrorFiles As FileInfo() = DFTErrorDir.GetFiles("*.*")


            If DFTerrorFiles.Length = 1 Then
                strMessage = strMessage & "There is " & CStr(DFTerrorFiles.Length) & " file in the backup directory. The Error File is located at... " & DFTErrorDirStr & " & vbCrLf"
            Else
                strMessage = strMessage & "There are " & CStr(DFTerrorFiles.Length) & " files in the backup directory. The Error Files is located at... " & DFTErrorDirStr & " & vbCrLf"
            End If

            fileCount = fileCount + DFTerrorFiles.Length

        Catch ex As Exception

            gblLogString = gblLogString & "process Spalding DFT Error" & vbCrLf
            gblLogString = gblLogString & ex.Message & vbCrLf

            Exit Sub
        End Try
    End Sub
 

    Private Sub writeToLog(ByVal strMsg As String, ByVal eventType As Integer)

        Dim file As System.IO.StreamWriter
        Dim tempLogFileName As String = strLogDirectory & "SpaldingEmailer_log.txt"
        file = My.Computer.FileSystem.OpenTextFileWriter(tempLogFileName, True)
        file.WriteLine(DateTime.Now & " : " & strMsg)
        file.Close()

    End Sub


    Private Sub mailIT(strMessage As String)
        Dim SMTP As New SmtpClient(System.Configuration.ConfigurationManager.AppSettings("email_server").ToString())

        'Dim strServer As String = objIniFile.GetString("Email Settings", "email_server", "(none)")
        Dim strFromAddress As String = System.Configuration.ConfigurationManager.AppSettings("email_from").ToString()
        Dim strToAddress As String = System.Configuration.ConfigurationManager.AppSettings("email_to").ToString()

        Dim message As New MailMessage(strFromAddress, strToAddress)
        Dim username As String = System.Configuration.ConfigurationManager.AppSettings("username").ToString()
        Dim password As String = System.Configuration.ConfigurationManager.AppSettings("password").ToString()
        Dim port As Integer = CInt(System.Configuration.ConfigurationManager.AppSettings("port"))


        '20150629 - changed smtpclient back to relay.jhsmh.root.local
        ' emailSender = New System.Net.Mail.SmtpClient("10.48.9.181")
        'emailSender = New System.Net.Mail.SmtpClient("relay.jhsmh.root.local")
        '20170111 - Change relay to relay.catholichealth.net port 25 per Mohammed 12/30/2016
        'emailSender = New System.Net.Mail.SmtpClient("relay.catholichealth.net")
        'emailSender.Port = 25
        'emailSender.Port = port

        ' ----- Build the content details.
        Try

            SMTP.EnableSsl = True
            SMTP.Credentials = New Net.NetworkCredential(username, password)
            SMTP.Port = port

            message.Subject = "Spalding Production Emailer Message " & CStr(Now)
            message.Body = strMessage

            SMTP.Send(message)

        Catch ex As Exception
            globalError = True

            gblLogString = gblLogString & "MailIt Error" & vbCrLf
            gblLogString = gblLogString & ex.Message & vbCrLf
            writeToLog(gblLogString, 1)
            Exit Sub
        Finally
        End Try
    End Sub



End Module
