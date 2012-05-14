<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMain
	Inherits System.Windows.Forms.Form

	'Form overrides dispose to clean up the component list.
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

	'Required by the Windows Form Designer
	Private components As System.ComponentModel.IContainer

	'NOTE: The following procedure is required by the Windows Form Designer
	'It can be modified using the Windows Form Designer.  
	'Do not modify it using the code editor.
	<System.Diagnostics.DebuggerStepThrough()> _
	Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.tlpMain = New System.Windows.Forms.TableLayoutPanel()
        Me.lstFiles = New System.Windows.Forms.ListView()
        Me.colFilename = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colVersion = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colSize = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colDate = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colHash = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.imgFiles = New System.Windows.Forms.ImageList(Me.components)
        Me.fbdOutputFolder = New System.Windows.Forms.FolderBrowserDialog()
        Me.sfdFeedXML = New System.Windows.Forms.SaveFileDialog()
        Me.tsMain = New System.Windows.Forms.ToolStrip()
        Me.btnNew = New System.Windows.Forms.ToolStripButton()
        Me.btnOpen = New System.Windows.Forms.ToolStripButton()
        Me.btnSave = New System.Windows.Forms.ToolStripButton()
        Me.btnSaveAs = New System.Windows.Forms.ToolStripButton()
        Me.tsSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.btnRefresh = New System.Windows.Forms.ToolStripButton()
        Me.btnBuild = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripContainer1 = New System.Windows.Forms.ToolStripContainer()
        Me.grpSettings = New System.Windows.Forms.GroupBox()
        Me.chkCleanUp = New System.Windows.Forms.CheckBox()
        Me.chkCopyFiles = New System.Windows.Forms.CheckBox()
        Me.lblIgnore = New System.Windows.Forms.Label()
        Me.lblMisc = New System.Windows.Forms.Label()
        Me.lblCompare = New System.Windows.Forms.Label()
        Me.chkHash = New System.Windows.Forms.CheckBox()
        Me.chkDate = New System.Windows.Forms.CheckBox()
        Me.chkSize = New System.Windows.Forms.CheckBox()
        Me.chkVersion = New System.Windows.Forms.CheckBox()
        Me.lblBaseURL = New System.Windows.Forms.Label()
        Me.chkIgnoreVsHost = New System.Windows.Forms.CheckBox()
        Me.chkIgnoreSymbols = New System.Windows.Forms.CheckBox()
        Me.cmdFeedXML = New System.Windows.Forms.Button()
        Me.lblFeedXML = New System.Windows.Forms.Label()
        Me.cmdOutputFolder = New System.Windows.Forms.Button()
        Me.lblOutputFolder = New System.Windows.Forms.Label()
        Me.txtBaseURL = New FeedBuilder.HelpfulTextBox()
        Me.txtFeedXML = New FeedBuilder.HelpfulTextBox()
        Me.txtOutputFolder = New FeedBuilder.HelpfulTextBox()
        Me.tlpMain.SuspendLayout()
        Me.tsMain.SuspendLayout()
        Me.ToolStripContainer1.ContentPanel.SuspendLayout()
        Me.ToolStripContainer1.TopToolStripPanel.SuspendLayout()
        Me.ToolStripContainer1.SuspendLayout()
        Me.grpSettings.SuspendLayout()
        Me.SuspendLayout()
        '
        'tlpMain
        '
        Me.tlpMain.ColumnCount = 3
        Me.tlpMain.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75.0!))
        Me.tlpMain.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.tlpMain.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75.0!))
        Me.tlpMain.Controls.Add(Me.lstFiles, 0, 1)
        Me.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tlpMain.Location = New System.Drawing.Point(0, 205)
        Me.tlpMain.Name = "tlpMain"
        Me.tlpMain.RowCount = 3
        Me.tlpMain.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 12.0!))
        Me.tlpMain.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpMain.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 3.0!))
        Me.tlpMain.Size = New System.Drawing.Size(580, 181)
        Me.tlpMain.TabIndex = 1
        '
        'lstFiles
        '
        Me.lstFiles.CheckBoxes = True
        Me.lstFiles.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.colFilename, Me.colVersion, Me.colSize, Me.colDate, Me.colHash})
        Me.tlpMain.SetColumnSpan(Me.lstFiles, 3)
        Me.lstFiles.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstFiles.Location = New System.Drawing.Point(0, 12)
        Me.lstFiles.Margin = New System.Windows.Forms.Padding(0)
        Me.lstFiles.Name = "lstFiles"
        Me.lstFiles.Size = New System.Drawing.Size(580, 166)
        Me.lstFiles.SmallImageList = Me.imgFiles
        Me.lstFiles.TabIndex = 0
        Me.lstFiles.UseCompatibleStateImageBehavior = False
        Me.lstFiles.View = System.Windows.Forms.View.Details
        '
        'colFilename
        '
        Me.colFilename.Text = "Filename"
        Me.colFilename.Width = 200
        '
        'colVersion
        '
        Me.colVersion.Text = "Version"
        Me.colVersion.Width = 80
        '
        'colSize
        '
        Me.colSize.Text = "Size"
        Me.colSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.colSize.Width = 80
        '
        'colDate
        '
        Me.colDate.Text = "Date"
        Me.colDate.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.colDate.Width = 120
        '
        'colHash
        '
        Me.colHash.Text = "Hash"
        Me.colHash.Width = 300
        '
        'imgFiles
        '
        Me.imgFiles.ImageStream = CType(resources.GetObject("imgFiles.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imgFiles.TransparentColor = System.Drawing.Color.Transparent
        Me.imgFiles.Images.SetKeyName(0, "file_extension_other.png")
        Me.imgFiles.Images.SetKeyName(1, "file_extension_bmp.png")
        Me.imgFiles.Images.SetKeyName(2, "file_extension_dll.png")
        Me.imgFiles.Images.SetKeyName(3, "file_extension_doc.png")
        Me.imgFiles.Images.SetKeyName(4, "file_extension_exe.png")
        Me.imgFiles.Images.SetKeyName(5, "file_extension_htm.png")
        Me.imgFiles.Images.SetKeyName(6, "file_extension_jpg.png")
        Me.imgFiles.Images.SetKeyName(7, "file_extension_pdf.png")
        Me.imgFiles.Images.SetKeyName(8, "file_extension_png.png")
        Me.imgFiles.Images.SetKeyName(9, "file_extension_txt.png")
        Me.imgFiles.Images.SetKeyName(10, "file_extension_wav.png")
        Me.imgFiles.Images.SetKeyName(11, "file_extension_wmv.png")
        Me.imgFiles.Images.SetKeyName(12, "file_extension_zip.png")
        '
        'fbdOutputFolder
        '
        Me.fbdOutputFolder.Description = "Select your projects output folder:"
        '
        'sfdFeedXML
        '
        Me.sfdFeedXML.DefaultExt = "xml"
        Me.sfdFeedXML.Filter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*"
        Me.sfdFeedXML.Title = "Select the location to save your NauXML file:"
        '
        'tsMain
        '
        Me.tsMain.Dock = System.Windows.Forms.DockStyle.None
        Me.tsMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.btnNew, Me.btnOpen, Me.btnSave, Me.btnSaveAs, Me.tsSeparator1, Me.btnRefresh, Me.btnBuild})
        Me.tsMain.Location = New System.Drawing.Point(0, 0)
        Me.tsMain.Name = "tsMain"
        Me.tsMain.Size = New System.Drawing.Size(580, 25)
        Me.tsMain.Stretch = True
        Me.tsMain.TabIndex = 2
        Me.tsMain.Text = "Commands"
        '
        'btnNew
        '
        Me.btnNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.btnNew.Image = CType(resources.GetObject("btnNew.Image"), System.Drawing.Image)
        Me.btnNew.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnNew.Name = "btnNew"
        Me.btnNew.Size = New System.Drawing.Size(23, 22)
        Me.btnNew.Text = "&New"
        '
        'btnOpen
        '
        Me.btnOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.btnOpen.Image = CType(resources.GetObject("btnOpen.Image"), System.Drawing.Image)
        Me.btnOpen.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnOpen.Name = "btnOpen"
        Me.btnOpen.Size = New System.Drawing.Size(23, 22)
        Me.btnOpen.Text = "&Open"
        '
        'btnSave
        '
        Me.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.btnSave.Image = CType(resources.GetObject("btnSave.Image"), System.Drawing.Image)
        Me.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(23, 22)
        Me.btnSave.Text = "&Save"
        '
        'btnSaveAs
        '
        Me.btnSaveAs.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.btnSaveAs.Image = CType(resources.GetObject("btnSaveAs.Image"), System.Drawing.Image)
        Me.btnSaveAs.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnSaveAs.Name = "btnSaveAs"
        Me.btnSaveAs.Size = New System.Drawing.Size(48, 22)
        Me.btnSaveAs.Text = "SaveAs"
        '
        'tsSeparator1
        '
        Me.tsSeparator1.Name = "tsSeparator1"
        Me.tsSeparator1.Size = New System.Drawing.Size(6, 25)
        '
        'btnRefresh
        '
        Me.btnRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.btnRefresh.Image = CType(resources.GetObject("btnRefresh.Image"), System.Drawing.Image)
        Me.btnRefresh.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnRefresh.Name = "btnRefresh"
        Me.btnRefresh.Size = New System.Drawing.Size(50, 22)
        Me.btnRefresh.Text = "Refresh"
        '
        'btnBuild
        '
        Me.btnBuild.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.btnBuild.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.btnBuild.Image = CType(resources.GetObject("btnBuild.Image"), System.Drawing.Image)
        Me.btnBuild.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnBuild.Name = "btnBuild"
        Me.btnBuild.Size = New System.Drawing.Size(38, 22)
        Me.btnBuild.Text = "Build"
        '
        'ToolStripContainer1
        '
        '
        'ToolStripContainer1.ContentPanel
        '
        Me.ToolStripContainer1.ContentPanel.Controls.Add(Me.tlpMain)
        Me.ToolStripContainer1.ContentPanel.Controls.Add(Me.grpSettings)
        Me.ToolStripContainer1.ContentPanel.Size = New System.Drawing.Size(580, 386)
        Me.ToolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ToolStripContainer1.Location = New System.Drawing.Point(12, 8)
        Me.ToolStripContainer1.Name = "ToolStripContainer1"
        Me.ToolStripContainer1.Size = New System.Drawing.Size(580, 411)
        Me.ToolStripContainer1.TabIndex = 3
        Me.ToolStripContainer1.Text = "ToolStripContainer1"
        '
        'ToolStripContainer1.TopToolStripPanel
        '
        Me.ToolStripContainer1.TopToolStripPanel.Controls.Add(Me.tsMain)
        '
        'grpSettings
        '
        Me.grpSettings.Controls.Add(Me.chkCleanUp)
        Me.grpSettings.Controls.Add(Me.chkCopyFiles)
        Me.grpSettings.Controls.Add(Me.lblIgnore)
        Me.grpSettings.Controls.Add(Me.lblMisc)
        Me.grpSettings.Controls.Add(Me.lblCompare)
        Me.grpSettings.Controls.Add(Me.chkHash)
        Me.grpSettings.Controls.Add(Me.chkDate)
        Me.grpSettings.Controls.Add(Me.chkSize)
        Me.grpSettings.Controls.Add(Me.chkVersion)
        Me.grpSettings.Controls.Add(Me.txtBaseURL)
        Me.grpSettings.Controls.Add(Me.lblBaseURL)
        Me.grpSettings.Controls.Add(Me.chkIgnoreVsHost)
        Me.grpSettings.Controls.Add(Me.chkIgnoreSymbols)
        Me.grpSettings.Controls.Add(Me.cmdFeedXML)
        Me.grpSettings.Controls.Add(Me.txtFeedXML)
        Me.grpSettings.Controls.Add(Me.lblFeedXML)
        Me.grpSettings.Controls.Add(Me.cmdOutputFolder)
        Me.grpSettings.Controls.Add(Me.txtOutputFolder)
        Me.grpSettings.Controls.Add(Me.lblOutputFolder)
        Me.grpSettings.Dock = System.Windows.Forms.DockStyle.Top
        Me.grpSettings.Location = New System.Drawing.Point(0, 0)
        Me.grpSettings.Name = "grpSettings"
        Me.grpSettings.Padding = New System.Windows.Forms.Padding(12, 8, 12, 8)
        Me.grpSettings.Size = New System.Drawing.Size(580, 205)
        Me.grpSettings.TabIndex = 1
        Me.grpSettings.TabStop = False
        Me.grpSettings.Text = "Settings:"
        '
        'chkCleanUp
        '
        Me.chkCleanUp.AutoSize = True
        Me.chkCleanUp.Checked = True
        Me.chkCleanUp.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkCleanUp.Location = New System.Drawing.Point(293, 145)
        Me.chkCleanUp.Name = "chkCleanUp"
        Me.chkCleanUp.Size = New System.Drawing.Size(134, 17)
        Me.chkCleanUp.TabIndex = 17
        Me.chkCleanUp.Text = "Clean Unselected Files"
        Me.chkCleanUp.UseVisualStyleBackColor = True
        '
        'chkCopyFiles
        '
        Me.chkCopyFiles.AutoSize = True
        Me.chkCopyFiles.Checked = True
        Me.chkCopyFiles.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkCopyFiles.Location = New System.Drawing.Point(146, 145)
        Me.chkCopyFiles.Name = "chkCopyFiles"
        Me.chkCopyFiles.Size = New System.Drawing.Size(141, 17)
        Me.chkCopyFiles.TabIndex = 16
        Me.chkCopyFiles.Text = "Copy Files with NauXML"
        Me.chkCopyFiles.UseVisualStyleBackColor = True
        '
        'lblIgnore
        '
        Me.lblIgnore.AutoSize = True
        Me.lblIgnore.Location = New System.Drawing.Point(15, 174)
        Me.lblIgnore.Name = "lblIgnore"
        Me.lblIgnore.Size = New System.Drawing.Size(40, 13)
        Me.lblIgnore.TabIndex = 15
        Me.lblIgnore.Text = "Ignore:"
        '
        'lblMisc
        '
        Me.lblMisc.AutoSize = True
        Me.lblMisc.Location = New System.Drawing.Point(15, 146)
        Me.lblMisc.Name = "lblMisc"
        Me.lblMisc.Size = New System.Drawing.Size(32, 13)
        Me.lblMisc.TabIndex = 15
        Me.lblMisc.Text = "Misc:"
        '
        'lblCompare
        '
        Me.lblCompare.AutoSize = True
        Me.lblCompare.Location = New System.Drawing.Point(15, 118)
        Me.lblCompare.Name = "lblCompare"
        Me.lblCompare.Size = New System.Drawing.Size(52, 13)
        Me.lblCompare.TabIndex = 14
        Me.lblCompare.Text = "Compare:"
        '
        'chkHash
        '
        Me.chkHash.AutoSize = True
        Me.chkHash.Checked = True
        Me.chkHash.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkHash.Location = New System.Drawing.Point(320, 117)
        Me.chkHash.Name = "chkHash"
        Me.chkHash.Size = New System.Drawing.Size(51, 17)
        Me.chkHash.TabIndex = 13
        Me.chkHash.Text = "Hash"
        Me.chkHash.UseVisualStyleBackColor = True
        '
        'chkDate
        '
        Me.chkDate.AutoSize = True
        Me.chkDate.Location = New System.Drawing.Point(265, 117)
        Me.chkDate.Name = "chkDate"
        Me.chkDate.Size = New System.Drawing.Size(49, 17)
        Me.chkDate.TabIndex = 12
        Me.chkDate.Text = "Date"
        Me.chkDate.UseVisualStyleBackColor = True
        '
        'chkSize
        '
        Me.chkSize.AutoSize = True
        Me.chkSize.Location = New System.Drawing.Point(213, 117)
        Me.chkSize.Name = "chkSize"
        Me.chkSize.Size = New System.Drawing.Size(46, 17)
        Me.chkSize.TabIndex = 11
        Me.chkSize.Text = "Size"
        Me.chkSize.UseVisualStyleBackColor = True
        '
        'chkVersion
        '
        Me.chkVersion.AutoSize = True
        Me.chkVersion.Checked = True
        Me.chkVersion.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkVersion.Location = New System.Drawing.Point(146, 117)
        Me.chkVersion.Margin = New System.Windows.Forms.Padding(3, 3, 3, 8)
        Me.chkVersion.Name = "chkVersion"
        Me.chkVersion.Size = New System.Drawing.Size(61, 17)
        Me.chkVersion.TabIndex = 10
        Me.chkVersion.Text = "Version"
        Me.chkVersion.UseVisualStyleBackColor = True
        '
        'lblBaseURL
        '
        Me.lblBaseURL.AutoSize = True
        Me.lblBaseURL.Location = New System.Drawing.Point(15, 89)
        Me.lblBaseURL.Name = "lblBaseURL"
        Me.lblBaseURL.Size = New System.Drawing.Size(59, 13)
        Me.lblBaseURL.TabIndex = 8
        Me.lblBaseURL.Text = "Base URL:"
        '
        'chkIgnoreVsHost
        '
        Me.chkIgnoreVsHost.AutoSize = True
        Me.chkIgnoreVsHost.Checked = True
        Me.chkIgnoreVsHost.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkIgnoreVsHost.Location = New System.Drawing.Point(293, 173)
        Me.chkIgnoreVsHost.Name = "chkIgnoreVsHost"
        Me.chkIgnoreVsHost.Size = New System.Drawing.Size(103, 17)
        Me.chkIgnoreVsHost.TabIndex = 7
        Me.chkIgnoreVsHost.Text = "VS Hosting Files"
        Me.chkIgnoreVsHost.UseVisualStyleBackColor = True
        '
        'chkIgnoreSymbols
        '
        Me.chkIgnoreSymbols.AutoSize = True
        Me.chkIgnoreSymbols.Checked = True
        Me.chkIgnoreSymbols.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkIgnoreSymbols.Location = New System.Drawing.Point(146, 173)
        Me.chkIgnoreSymbols.Name = "chkIgnoreSymbols"
        Me.chkIgnoreSymbols.Size = New System.Drawing.Size(100, 17)
        Me.chkIgnoreSymbols.TabIndex = 7
        Me.chkIgnoreSymbols.Text = "Debug Symbols"
        Me.chkIgnoreSymbols.UseVisualStyleBackColor = True
        '
        'cmdFeedXML
        '
        Me.cmdFeedXML.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdFeedXML.Location = New System.Drawing.Point(539, 53)
        Me.cmdFeedXML.Name = "cmdFeedXML"
        Me.cmdFeedXML.Size = New System.Drawing.Size(26, 23)
        Me.cmdFeedXML.TabIndex = 5
        Me.cmdFeedXML.Text = "..."
        Me.cmdFeedXML.UseVisualStyleBackColor = True
        '
        'lblFeedXML
        '
        Me.lblFeedXML.AutoSize = True
        Me.lblFeedXML.Location = New System.Drawing.Point(15, 58)
        Me.lblFeedXML.Name = "lblFeedXML"
        Me.lblFeedXML.Size = New System.Drawing.Size(98, 13)
        Me.lblFeedXML.TabIndex = 3
        Me.lblFeedXML.Text = "Feed NauXML File:"
        '
        'cmdOutputFolder
        '
        Me.cmdOutputFolder.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdOutputFolder.Location = New System.Drawing.Point(539, 22)
        Me.cmdOutputFolder.Name = "cmdOutputFolder"
        Me.cmdOutputFolder.Size = New System.Drawing.Size(26, 23)
        Me.cmdOutputFolder.TabIndex = 2
        Me.cmdOutputFolder.Text = "..."
        Me.cmdOutputFolder.UseVisualStyleBackColor = True
        '
        'lblOutputFolder
        '
        Me.lblOutputFolder.AutoSize = True
        Me.lblOutputFolder.Location = New System.Drawing.Point(15, 27)
        Me.lblOutputFolder.Name = "lblOutputFolder"
        Me.lblOutputFolder.Size = New System.Drawing.Size(110, 13)
        Me.lblOutputFolder.TabIndex = 0
        Me.lblOutputFolder.Text = "Project Output Folder:"
        '
        'txtBaseURL
        '
        Me.txtBaseURL.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtBaseURL.HelpfulText = "Where you will upload the feed and update files for distribution to clients"
        Me.txtBaseURL.Location = New System.Drawing.Point(146, 86)
        Me.txtBaseURL.Margin = New System.Windows.Forms.Padding(3, 3, 3, 8)
        Me.txtBaseURL.Name = "txtBaseURL"
        Me.txtBaseURL.Size = New System.Drawing.Size(387, 20)
        Me.txtBaseURL.TabIndex = 9
        '
        'txtFeedXML
        '
        Me.txtFeedXML.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtFeedXML.BackColor = System.Drawing.Color.White
        Me.txtFeedXML.HelpfulText = "The file your application downloads to determine if there are updates"
        Me.txtFeedXML.Location = New System.Drawing.Point(146, 55)
        Me.txtFeedXML.Margin = New System.Windows.Forms.Padding(3, 3, 3, 8)
        Me.txtFeedXML.Name = "txtFeedXML"
        Me.txtFeedXML.Size = New System.Drawing.Size(387, 20)
        Me.txtFeedXML.TabIndex = 4
        '
        'txtOutputFolder
        '
        Me.txtOutputFolder.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtOutputFolder.BackColor = System.Drawing.Color.White
        Me.txtOutputFolder.HelpfulText = "The folder that contains the files you want to distribute"
        Me.txtOutputFolder.Location = New System.Drawing.Point(146, 24)
        Me.txtOutputFolder.Margin = New System.Windows.Forms.Padding(3, 3, 3, 8)
        Me.txtOutputFolder.Name = "txtOutputFolder"
        Me.txtOutputFolder.Size = New System.Drawing.Size(387, 20)
        Me.txtOutputFolder.TabIndex = 1
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(604, 431)
        Me.Controls.Add(Me.ToolStripContainer1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MinimumSize = New System.Drawing.Size(620, 440)
        Me.Name = "frmMain"
        Me.Padding = New System.Windows.Forms.Padding(12, 8, 12, 12)
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Feed Builder"
        Me.tlpMain.ResumeLayout(False)
        Me.tsMain.ResumeLayout(False)
        Me.tsMain.PerformLayout()
        Me.ToolStripContainer1.ContentPanel.ResumeLayout(False)
        Me.ToolStripContainer1.TopToolStripPanel.ResumeLayout(False)
        Me.ToolStripContainer1.TopToolStripPanel.PerformLayout()
        Me.ToolStripContainer1.ResumeLayout(False)
        Me.ToolStripContainer1.PerformLayout()
        Me.grpSettings.ResumeLayout(False)
        Me.grpSettings.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents tlpMain As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents fbdOutputFolder As System.Windows.Forms.FolderBrowserDialog
    Friend WithEvents sfdFeedXML As System.Windows.Forms.SaveFileDialog
    Friend WithEvents imgFiles As System.Windows.Forms.ImageList
    Friend WithEvents lstFiles As System.Windows.Forms.ListView
    Friend WithEvents colFilename As System.Windows.Forms.ColumnHeader
    Friend WithEvents colVersion As System.Windows.Forms.ColumnHeader
    Friend WithEvents colSize As System.Windows.Forms.ColumnHeader
    Friend WithEvents colDate As System.Windows.Forms.ColumnHeader
    Friend WithEvents colHash As System.Windows.Forms.ColumnHeader
    Friend WithEvents tsMain As System.Windows.Forms.ToolStrip
    Friend WithEvents btnNew As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnOpen As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnSave As System.Windows.Forms.ToolStripButton
    Friend WithEvents tsSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents btnRefresh As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripContainer1 As System.Windows.Forms.ToolStripContainer
    Friend WithEvents grpSettings As System.Windows.Forms.GroupBox
    Friend WithEvents chkCleanUp As System.Windows.Forms.CheckBox
    Friend WithEvents chkCopyFiles As System.Windows.Forms.CheckBox
    Friend WithEvents lblIgnore As System.Windows.Forms.Label
    Friend WithEvents lblMisc As System.Windows.Forms.Label
    Friend WithEvents lblCompare As System.Windows.Forms.Label
    Friend WithEvents chkHash As System.Windows.Forms.CheckBox
    Friend WithEvents chkDate As System.Windows.Forms.CheckBox
    Friend WithEvents chkSize As System.Windows.Forms.CheckBox
    Friend WithEvents chkVersion As System.Windows.Forms.CheckBox
    Friend WithEvents txtBaseURL As HelpfulTextBox
    Friend WithEvents lblBaseURL As System.Windows.Forms.Label
    Friend WithEvents chkIgnoreVsHost As System.Windows.Forms.CheckBox
    Friend WithEvents chkIgnoreSymbols As System.Windows.Forms.CheckBox
    Friend WithEvents cmdFeedXML As System.Windows.Forms.Button
    Friend WithEvents txtFeedXML As HelpfulTextBox
    Friend WithEvents lblFeedXML As System.Windows.Forms.Label
    Friend WithEvents cmdOutputFolder As System.Windows.Forms.Button
    Friend WithEvents txtOutputFolder As HelpfulTextBox
    Friend WithEvents lblOutputFolder As System.Windows.Forms.Label
    Friend WithEvents btnSaveAs As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnBuild As System.Windows.Forms.ToolStripButton

End Class
