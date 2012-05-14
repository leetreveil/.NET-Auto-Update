Imports System.IO
Imports System.Text.RegularExpressions

Public Class ArgumentsParser
    Public Property HasArgs As Boolean
    Public Property FileName As String
    Public Property ShowGui As Boolean
    Public Property Build As Boolean
    Public Property OpenOutputsFolder As Boolean

    Public Sub New(args As String())
        For Each arg As String In args
            If arg = Application.ExecutablePath Then Continue For

            arg = CleanArg(arg)
            If arg = "build" Then
                Me.Build = True
                Me.HasArgs = True
            ElseIf arg = "showgui" Then
                Me.ShowGui = True
                Me.HasArgs = True
            ElseIf arg = "openoutputs" Then
                Me.OpenOutputsFolder = True
                Me.HasArgs = True
            ElseIf File.Exists(arg) Then
                Me.FileName = arg
                Me.HasArgs = True
            Else
                Console.WriteLine("Unrecognized arg '{0}'", arg)
            End If

        Next
    End Sub

    Private Function CleanArg(arg As String) As String
        Const pattern1 As String = "^(.*)([=,:](true|0))"
        arg = arg.ToLower()
        If arg.StartsWith("-") OrElse arg.StartsWith("/") Then
            arg = arg.Substring(1)
        End If
        Dim r As New Regex(pattern1)
        arg = r.Replace(arg, "{$1}")
        Return arg
    End Function
End Class
