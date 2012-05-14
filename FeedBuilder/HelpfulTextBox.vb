Imports System.ComponentModel
Imports System.Windows.Forms
Imports System.Runtime.InteropServices

Public Class HelpfulTextBox
    Inherits TextBox

#Region "Win32 DLL Imports"

    Private Const EM_SETCUEBANNER As Integer = &H1501

    <DllImport("user32.dll", CharSet:=CharSet.Auto)> _
    Private Shared Function SendMessage(hWnd As IntPtr, msg As Integer, wParam As Integer,
                                        <MarshalAs(UnmanagedType.LPWStr)> lParam As String) As Int32
    End Function

#End Region

    Private _helpfulText As String

    <Browsable(True)> _
    <Category("Appearance")> _
    <DefaultValue("")> _
    <Description("The grayed text that appears in the box when the box contains no text and is not focused.")> _
    <RefreshProperties(RefreshProperties.None)> _
    Public Property HelpfulText As String
        Get
            Return _helpfulText
        End Get
        Set(value As String)
            _helpfulText = value
            SetCue()
        End Set
    End Property

    Public Sub New()
        InitializeComponent()
        'Me.SetStyle(ControlStyles.UserPaint, True)
    End Sub

    ''' <summary>
    ''' Actually, the system cue only works for editable (i.e. not read-only) text boxes.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub SetCue()
        SendMessage(Me.Handle, EM_SETCUEBANNER, 0, Me.HelpfulText)
    End Sub

    Protected Overrides Sub OnPaint(e As System.Windows.Forms.PaintEventArgs)
        MyBase.OnPaint(e)
        'If Me.Focused And Not Me.ReadOnly Then Return
        'If String.IsNullOrEmpty(Me.HelpfulText) Then Return
        'If Not String.IsNullOrEmpty(Me.Text) Then Return
        'Dim grayBrush As New SolidBrush(SystemColors.GrayText)
        'e.Graphics.DrawString(Me.HelpfulText, Me.Font, grayBrush, Me.ClientRectangle)
    End Sub

End Class

#Region "Windows Forms stuff"

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class HelpfulTextBox
    Inherits System.Windows.Forms.TextBox

    'Control overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Control Designer
    Private components As System.ComponentModel.IContainer

    ' NOTE: The following procedure is required by the Component Designer
    ' It can be modified using the Component Designer.  Do not modify it
    ' using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        components = New System.ComponentModel.Container()
    End Sub

End Class

#End Region