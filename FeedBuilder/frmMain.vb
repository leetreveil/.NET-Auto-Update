Imports System.IO
Imports System.Xml

Public Class frmMain

#Region " Private constants/variables"
    Private Const DialogFilter As String = "Feed configuration files (*.config)|*.config|All files (*.*)|*.*"
    Private Const DefaultFileName As String = "FeedBuilder.config"
    Private _openDialog As OpenFileDialog
    Private _argParser As ArgumentsParser
#End Region

#Region " Properties"
    Public Property FileName As String
    Public Property ShowGui As Boolean
#End Region

#Region " Loading/Initialization/Lifetime"

    Private Sub frmMain_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        Me.Visible = False

        Dim args As String() = Environment.GetCommandLineArgs
        ' The first arg is the path to ourself
        'If args.Count >= 2 Then
        '    If File.Exists(args(1)) Then
        '        Dim p As New FeedBuilderSettingsProvider()
        '        p.LoadFrom(args(1))
        '        Me.FileName = args(1)
        '    End If
        'End If

        ' The first arg is the path to ourself
        _argParser = New ArgumentsParser(args)
        If _argParser.HasArgs Then
            Me.FileName = _argParser.FileName
            If Not String.IsNullOrEmpty(Me.FileName) Then
                If File.Exists(Me.FileName) Then
                    Dim p As New FeedBuilderSettingsProvider()
                    p.LoadFrom(Me.FileName)
                Else
                    _argParser.ShowGui = True
                    _argParser.Build = False
                End If
            End If
            InitializeFormSettings()
            If _argParser.ShowGui Then Me.Show()
            If _argParser.Build Then
                Build()
            End If
            If Not _argParser.ShowGui Then Me.Close()
        End If

    End Sub

    Private Sub InitializeFormSettings()
        If Not String.IsNullOrEmpty(My.Settings.OutputFolder) AndAlso Directory.Exists(My.Settings.OutputFolder) Then txtOutputFolder.Text = My.Settings.OutputFolder
        If Not String.IsNullOrEmpty(My.Settings.FeedXML) AndAlso Directory.Exists(Path.GetDirectoryName(My.Settings.FeedXML)) Then txtFeedXML.Text = My.Settings.FeedXML
        If Not String.IsNullOrEmpty(My.Settings.BaseURL) Then txtBaseURL.Text = My.Settings.BaseURL

        chkVersion.Checked = My.Settings.CompareVersion
        chkSize.Checked = My.Settings.CompareSize
        chkDate.Checked = My.Settings.CompareDate
        chkHash.Checked = My.Settings.CompareHash

        chkIgnoreSymbols.Checked = My.Settings.IgnoreDebugSymbols
        chkIgnoreVsHost.Checked = My.Settings.IgnoreVsHosting
        chkCopyFiles.Checked = My.Settings.CopyFiles
        chkCleanUp.Checked = My.Settings.CleanUp

        If My.Settings.IgnoreFiles Is Nothing Then My.Settings.IgnoreFiles = New Collections.Specialized.StringCollection
        ReadFiles()
    End Sub

    Private Sub SaveFormSettings()
        If Not String.IsNullOrEmpty(txtOutputFolder.Text.Trim) AndAlso Directory.Exists(txtOutputFolder.Text.Trim) Then My.Settings.OutputFolder = txtOutputFolder.Text.Trim
        If Not String.IsNullOrEmpty(txtFeedXML.Text.Trim) AndAlso Directory.Exists(Path.GetDirectoryName(txtFeedXML.Text.Trim)) Then My.Settings.FeedXML = txtFeedXML.Text.Trim
        If Not String.IsNullOrEmpty(txtBaseURL.Text.Trim) Then My.Settings.BaseURL = txtBaseURL.Text.Trim

        My.Settings.CompareVersion = chkVersion.Checked
        My.Settings.CompareSize = chkSize.Checked
        My.Settings.CompareDate = chkDate.Checked
        My.Settings.CompareHash = chkHash.Checked

        My.Settings.IgnoreDebugSymbols = chkIgnoreSymbols.Checked
        My.Settings.IgnoreVsHosting = chkIgnoreVsHost.Checked
        My.Settings.CopyFiles = chkCopyFiles.Checked
        My.Settings.CleanUp = chkCleanUp.Checked

        With My.Settings.IgnoreFiles
            .Clear()
            For Each thisItem As ListViewItem In lstFiles.Items
                If Not thisItem.Checked Then .Add(thisItem.Text)
            Next
        End With
    End Sub

    Private Sub frmMain_FormClosing(sender As Object, e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        SaveFormSettings()
    End Sub

#End Region
#Region " Commands Events"
    Private Sub cmdBuild_Click(sender As System.Object, e As System.EventArgs) Handles btnBuild.Click
        Build()
    End Sub

    Private Sub btnNew_Click(sender As System.Object, e As System.EventArgs) Handles btnNew.Click
        My.Settings.Reset()
        InitializeFormSettings()
    End Sub

    Private Sub btnOpen_Click(sender As System.Object, e As System.EventArgs) Handles btnOpen.Click
        Dim dlg As OpenFileDialog
        If Me._openDialog Is Nothing Then
            dlg = New OpenFileDialog
            dlg.CheckFileExists = True
            If String.IsNullOrEmpty(Me.FileName) Then
                dlg.FileName = DefaultFileName
            Else
                dlg.FileName = Me.FileName
            End If
            Me._openDialog = dlg
        Else
            dlg = Me._openDialog
        End If
        dlg.Filter = DialogFilter
        Dim result As DialogResult = dlg.ShowDialog()
        If result = Windows.Forms.DialogResult.OK Then
            Dim p As New FeedBuilderSettingsProvider()
            p.LoadFrom(dlg.FileName)
            InitializeFormSettings()
            Me.FileName = dlg.FileName
        End If
    End Sub

    Private Sub btnSave_Click(sender As System.Object, e As System.EventArgs) Handles btnSave.Click
        Save(False)
    End Sub

    Private Sub btnSaveAs_Click(sender As System.Object, e As System.EventArgs) Handles btnSaveAs.Click
        Save(True)
    End Sub

    Private Sub btnRefresh_Click(sender As System.Object, e As System.EventArgs) Handles btnRefresh.Click
        ReadFiles()
    End Sub

#End Region
#Region " Options Events"
    Private Sub cmdOutputFolder_Click(sender As System.Object, e As System.EventArgs) Handles cmdOutputFolder.Click
        fbdOutputFolder.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
        If fbdOutputFolder.ShowDialog(Me) = Windows.Forms.DialogResult.OK Then
            txtOutputFolder.Text = fbdOutputFolder.SelectedPath
            ReadFiles()
        End If
    End Sub

    Private Sub cmdFeedXML_Click(sender As System.Object, e As System.EventArgs) Handles cmdFeedXML.Click
        sfdFeedXML.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
        If sfdFeedXML.ShowDialog(Me) = Windows.Forms.DialogResult.OK Then txtFeedXML.Text = sfdFeedXML.FileName
    End Sub

    Private Sub chkIgnoreSymbols_CheckedChanged(sender As Object, e As System.EventArgs)
        ReadFiles()
    End Sub

    Private Sub chkCopyFiles_CheckedChanged(sender As System.Object, e As System.EventArgs)
        chkCleanUp.Enabled = chkCopyFiles.Checked
        If Not chkCopyFiles.Checked Then chkCleanUp.Checked = False
    End Sub

#End Region
#Region " Helper Methods "
    Private Sub Build()
        Console.WriteLine("Building NAppUpdater feed '{0}'", txtBaseURL.Text.Trim)
        Dim doc As New XmlDocument
        Dim dec As XmlDeclaration = doc.CreateXmlDeclaration("1.0", "utf-8", Nothing)
        Dim feed, tasks, task, conds, cond As XmlElement
        Dim pastFirst As Boolean

        doc.AppendChild(dec)
        feed = doc.CreateElement("Feed")
        If Not String.IsNullOrEmpty(txtBaseURL.Text.Trim) Then feed.SetAttribute("BaseUrl", txtBaseURL.Text.Trim)
        doc.AppendChild(feed)

        tasks = doc.CreateElement("Tasks")

        Console.WriteLine("Processing feed items")
        Dim itemsCopied As Integer = 0
        Dim itemsCleaned As Integer = 0
        Dim itemsSkipped As Integer = 0
        Dim itemsFailed As Integer = 0
        For Each thisItem As ListViewItem In lstFiles.Items
            Dim destFile As String = Path.Combine(Path.GetDirectoryName(txtFeedXML.Text.Trim), thisItem.Text)

            If thisItem.Checked Then
                pastFirst = False

                With CType(thisItem.Tag, FileInfoEx)
                    task = doc.CreateElement("FileUpdateTask")
                    task.SetAttribute("localPath", .FileInfo.Name)

                    conds = doc.CreateElement("Conditions")

                    'Version
                    If chkVersion.Checked Then
                        cond = doc.CreateElement("FileVersionCondition")
                        cond.SetAttribute("what", "below")
                        cond.SetAttribute("version", .FileVersion)
                        conds.AppendChild(cond)
                        pastFirst = True
                    End If

                    'Size
                    If chkSize.Checked Then
                        cond = doc.CreateElement("FileSizeCondition")
                        If pastFirst Then cond.SetAttribute("type", "not")
                        cond.SetAttribute("what", "is")
                        cond.SetAttribute("size", .FileInfo.Length.ToString)
                        conds.AppendChild(cond)
                        pastFirst = True
                    End If

                    'Date
                    If chkDate.Checked Then
                        cond = doc.CreateElement("FileVersionCondition")
                        If pastFirst Then cond.SetAttribute("type", "not")
                        cond.SetAttribute("what", "below")
                        cond.SetAttribute("timestamp", .FileInfo.LastWriteTime.ToFileTime.ToString)
                        conds.AppendChild(cond)
                        pastFirst = True
                    End If

                    'Hash
                    If chkHash.Checked Then
                        cond = doc.CreateElement("FileChecksumCondition")
                        If pastFirst Then cond.SetAttribute("type", "not")
                        cond.SetAttribute("checksumType", "sha256")
                        cond.SetAttribute("checksum", .Hash)
                        conds.AppendChild(cond)
                    End If

                    task.AppendChild(conds)
                    tasks.AppendChild(task)

                    If chkCopyFiles.Checked Then
                        Try
                            If File.Exists(destFile) Then File.Delete(destFile)
                            File.Copy(.FileInfo.FullName, destFile)
                            itemsCopied += 1
                        Catch ex As IOException
                            itemsFailed += 1
                        End Try
                    End If
                End With
            Else
                Try
                    If chkCleanUp.Checked And File.Exists(destFile) Then
                        File.Delete(destFile)
                        itemsCleaned += 1
                    Else
                        itemsSkipped += 1
                    End If
                Catch ex As IOException
                    itemsFailed += 1
                End Try
            End If
        Next
        feed.AppendChild(tasks)
        doc.Save(txtFeedXML.Text.Trim)

        ' open the outputs folder if we're running from the GUI or 
        ' we have an explicit command line option to do so
        If Not _argParser.HasArgs OrElse _argParser.OpenOutputsFolder Then
            OpenOutputsFolder()
        End If
        Console.WriteLine("Done building feed.")
        If itemsCopied > 0 Then Console.WriteLine("{0,5} items copied", itemsCopied)
        If itemsCleaned > 0 Then Console.WriteLine("{0,5} items cleaned", itemsCleaned)
        If itemsSkipped > 0 Then Console.WriteLine("{0,5} items skipped", itemsSkipped)
        If itemsFailed > 0 Then Console.WriteLine("{0,5} items failed", itemsFailed)
    End Sub

    Private Sub OpenOutputsFolder()
        With New Process
            .StartInfo.UseShellExecute = True
            .StartInfo.FileName = Path.GetDirectoryName(txtFeedXML.Text.Trim)
            .Start()
        End With
    End Sub

    Private Function GetImageIndex(ext As String) As Integer
        Select Case ext.Trim("."c)
            Case "bmp"
                Return 1
            Case "dll"
                Return 2
            Case "doc", "docx"
                Return 3
            Case "exe"
                Return 4
            Case "htm", "html"
                Return 5
            Case "jpg", "jpeg"
                Return 6
            Case "pdf"
                Return 7
            Case "png"
                Return 8
            Case "txt"
                Return 9
            Case "wav", "mp3"
                Return 10
            Case "wmv"
                Return 11
            Case "xls", "xlsx"
                Return 12
            Case "zip"
                Return 13
            Case Else
                Return 0
        End Select
    End Function

    Private Sub ReadFiles()
        If Not String.IsNullOrEmpty(txtOutputFolder.Text.Trim) AndAlso Directory.Exists(txtOutputFolder.Text.Trim) Then
            Dim thisItem As ListViewItem
            Dim thisInfo As FileInfoEx

            lstFiles.BeginUpdate()
            lstFiles.Items.Clear()
            For Each thisFile As String In Directory.EnumerateFiles(txtOutputFolder.Text.Trim)
                If (Not IsIgnorable(thisFile)) Then
                    thisInfo = New FileInfoEx(thisFile)
                    With thisInfo
                        thisItem = New ListViewItem(.FileInfo.Name, GetImageIndex(.FileInfo.Extension))
                        thisItem.SubItems.Add(.FileVersion)
                        thisItem.SubItems.Add(.FileInfo.Length.ToString)
                        thisItem.SubItems.Add(.FileInfo.LastWriteTime.ToString)
                        thisItem.SubItems.Add(.Hash)
                        thisItem.Checked = (Not My.Settings.IgnoreFiles.Contains(.FileInfo.Name))
                    End With
                    thisItem.Tag = thisInfo
                    lstFiles.Items.Add(thisItem)
                End If
            Next
            lstFiles.EndUpdate()
        End If
    End Sub

    Private Function IsIgnorable(filename As String) As Boolean
        Dim ext As String = Path.GetExtension(filename)
        If (chkIgnoreSymbols.Checked AndAlso ext = ".pdb") Then Return True
        If (chkIgnoreVsHost.Checked AndAlso filename.ToLower().Contains("vshost.exe")) Then Return True
        Return False
    End Function

    Private Sub Save(forceDialog As Boolean)
        SaveFormSettings()
        If forceDialog OrElse String.IsNullOrEmpty(Me.FileName) Then
            Dim dlg As New SaveFileDialog()
            dlg.Filter = DialogFilter
            dlg.FileName = DefaultFileName
            Dim result As DialogResult = dlg.ShowDialog()
            If result = Windows.Forms.DialogResult.OK Then
                Dim p As New FeedBuilderSettingsProvider()
                p.SaveAs(dlg.FileName)
                Me.FileName = dlg.FileName
            End If
        Else
            Dim p As New FeedBuilderSettingsProvider
            p.SaveAs(Me.FileName)
        End If
    End Sub

#End Region

End Class