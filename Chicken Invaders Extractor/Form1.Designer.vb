<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        Button1 = New Button()
        Button2 = New Button()
        ArchivePathTextBox = New TextBox()
        BrowseArchiveBtn = New Button()
        OpenArchiveDialog = New OpenFileDialog()
        SaveArchiveDialog = New SaveFileDialog()
        SaveFolderDialog = New FolderBrowserDialog()
        FolderPathTextBox = New TextBox()
        SaveFolderBtn = New Button()
        Label1 = New Label()
        UnpackModeCombo = New ComboBox()
        Label2 = New Label()
        Label3 = New Label()
        LinkLabel1 = New LinkLabel()
        Label4 = New Label()
        ProgressBar1 = New ProgressBar()
        UpdTableBtn = New Button()
        TGACheckBox = New CheckBox()
        SuspendLayout()
        ' 
        ' Button1
        ' 
        Button1.Cursor = Cursors.Hand
        Button1.Location = New Point(14, 310)
        Button1.Name = "Button1"
        Button1.Size = New Size(236, 60)
        Button1.TabIndex = 0
        Button1.Text = "Unpack"
        Button1.UseVisualStyleBackColor = True
        ' 
        ' Button2
        ' 
        Button2.Cursor = Cursors.Hand
        Button2.Location = New Point(270, 310)
        Button2.Name = "Button2"
        Button2.Size = New Size(236, 60)
        Button2.TabIndex = 1
        Button2.Text = "Repack"
        Button2.UseVisualStyleBackColor = True
        ' 
        ' ArchivePathTextBox
        ' 
        ArchivePathTextBox.Location = New Point(14, 135)
        ArchivePathTextBox.Name = "ArchivePathTextBox"
        ArchivePathTextBox.Size = New Size(422, 39)
        ArchivePathTextBox.TabIndex = 2
        ' 
        ' BrowseArchiveBtn
        ' 
        BrowseArchiveBtn.Location = New Point(442, 135)
        BrowseArchiveBtn.Name = "BrowseArchiveBtn"
        BrowseArchiveBtn.Size = New Size(64, 39)
        BrowseArchiveBtn.TabIndex = 3
        BrowseArchiveBtn.Text = "..."
        BrowseArchiveBtn.UseVisualStyleBackColor = True
        ' 
        ' OpenArchiveDialog
        ' 
        OpenArchiveDialog.FileName = "archive.dat"
        ' 
        ' FolderPathTextBox
        ' 
        FolderPathTextBox.Location = New Point(14, 238)
        FolderPathTextBox.Name = "FolderPathTextBox"
        FolderPathTextBox.Size = New Size(422, 39)
        FolderPathTextBox.TabIndex = 4
        ' 
        ' SaveFolderBtn
        ' 
        SaveFolderBtn.Location = New Point(442, 238)
        SaveFolderBtn.Name = "SaveFolderBtn"
        SaveFolderBtn.Size = New Size(64, 39)
        SaveFolderBtn.TabIndex = 5
        SaveFolderBtn.Text = "..."
        SaveFolderBtn.UseVisualStyleBackColor = True
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Location = New Point(76, 33)
        Label1.Name = "Label1"
        Label1.Size = New Size(89, 32)
        Label1.TabIndex = 6
        Label1.Text = "Mode: "
        ' 
        ' UnpackModeCombo
        ' 
        UnpackModeCombo.DropDownStyle = ComboBoxStyle.DropDownList
        UnpackModeCombo.FormattingEnabled = True
        UnpackModeCombo.Items.AddRange(New Object() {"CI2/CI3/CI4/CI5", "CI Universe"})
        UnpackModeCombo.Location = New Point(170, 30)
        UnpackModeCombo.Name = "UnpackModeCombo"
        UnpackModeCombo.Size = New Size(242, 40)
        UnpackModeCombo.TabIndex = 7
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Location = New Point(14, 100)
        Label2.Name = "Label2"
        Label2.Size = New Size(299, 32)
        Label2.TabIndex = 8
        Label2.Text = "Archive (.dat, .hq2x, .222x..)"
        ' 
        ' Label3
        ' 
        Label3.AutoSize = True
        Label3.Location = New Point(14, 203)
        Label3.Name = "Label3"
        Label3.Size = New Size(142, 32)
        Label3.TabIndex = 9
        Label3.Text = "Data Folder:"
        ' 
        ' LinkLabel1
        ' 
        LinkLabel1.AutoSize = True
        LinkLabel1.Cursor = Cursors.Hand
        LinkLabel1.Font = New Font("Segoe UI Semibold", 7.875F, FontStyle.Bold, GraphicsUnit.Point)
        LinkLabel1.Location = New Point(180, 499)
        LinkLabel1.Name = "LinkLabel1"
        LinkLabel1.Size = New Size(92, 30)
        LinkLabel1.TabIndex = 10
        LinkLabel1.TabStop = True
        LinkLabel1.Text = "ravenDS"
        ' 
        ' Label4
        ' 
        Label4.AutoSize = True
        Label4.Font = New Font("Segoe UI", 7.875F, FontStyle.Italic, GraphicsUnit.Point)
        Label4.Location = New Point(266, 499)
        Label4.Name = "Label4"
        Label4.Size = New Size(71, 30)
        Label4.TabIndex = 11
        Label4.Text = "- 2025"
        ' 
        ' ProgressBar1
        ' 
        ProgressBar1.Location = New Point(14, 442)
        ProgressBar1.Name = "ProgressBar1"
        ProgressBar1.Size = New Size(492, 54)
        ProgressBar1.TabIndex = 15
        ' 
        ' UpdTableBtn
        ' 
        UpdTableBtn.Cursor = Cursors.Hand
        UpdTableBtn.Location = New Point(15, 383)
        UpdTableBtn.Name = "UpdTableBtn"
        UpdTableBtn.Size = New Size(235, 46)
        UpdTableBtn.TabIndex = 17
        UpdTableBtn.Text = "Update CIU table"
        UpdTableBtn.UseVisualStyleBackColor = True
        ' 
        ' TGACheckBox
        ' 
        TGACheckBox.AutoSize = True
        TGACheckBox.Cursor = Cursors.Hand
        TGACheckBox.Location = New Point(276, 389)
        TGACheckBox.Name = "TGACheckBox"
        TGACheckBox.Size = New Size(230, 36)
        TGACheckBox.TabIndex = 18
        TGACheckBox.Text = "Process .TGA files"
        TGACheckBox.UseVisualStyleBackColor = True
        ' 
        ' Form1
        ' 
        AutoScaleDimensions = New SizeF(13F, 32F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(520, 538)
        Controls.Add(TGACheckBox)
        Controls.Add(UpdTableBtn)
        Controls.Add(ProgressBar1)
        Controls.Add(Label4)
        Controls.Add(LinkLabel1)
        Controls.Add(Label3)
        Controls.Add(Label2)
        Controls.Add(UnpackModeCombo)
        Controls.Add(Label1)
        Controls.Add(SaveFolderBtn)
        Controls.Add(FolderPathTextBox)
        Controls.Add(BrowseArchiveBtn)
        Controls.Add(ArchivePathTextBox)
        Controls.Add(Button2)
        Controls.Add(Button1)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MaximizeBox = False
        Name = "Form1"
        StartPosition = FormStartPosition.CenterScreen
        Text = "CI Extractor 1.4"
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents Button1 As Button
    Friend WithEvents Button2 As Button
    Friend WithEvents ArchivePathTextBox As TextBox
    Friend WithEvents BrowseArchiveBtn As Button
    Friend WithEvents OpenArchiveDialog As OpenFileDialog
    Friend WithEvents SaveArchiveDialog As SaveFileDialog
    Friend WithEvents SaveFolderDialog As FolderBrowserDialog
    Friend WithEvents FolderPathTextBox As TextBox
    Friend WithEvents SaveFolderBtn As Button
    Friend WithEvents Label1 As Label
    Friend WithEvents UnpackModeCombo As ComboBox
    Friend WithEvents Label2 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents LinkLabel1 As LinkLabel
    Friend WithEvents Label4 As Label
    Friend WithEvents ProgressBar1 As ProgressBar
    Friend WithEvents Button3 As Button
    Friend WithEvents UpdTableBtn As Button
    Friend WithEvents TGACheckBox As CheckBox

End Class
