Imports System.IO
Imports System.IO.Compression
Imports System.Text
Imports System.Windows.Forms.VisualStyles.VisualStyleElement

Public Class Form1

    Function CleanPath(filePath As String) As String
        filePath = filePath.Replace("/", Path.DirectorySeparatorChar).Replace("\", Path.DirectorySeparatorChar)
        Return String.Concat(filePath.Split(Path.GetInvalidPathChars())).Trim()
    End Function

    Sub ApplyXor(fs As FileStream, offset As Long, xorKey As Byte, length As Integer)
        Dim originalPosition As Long = fs.Position
        fs.Seek(offset, SeekOrigin.Begin)
        Dim buffer(length - 1) As Byte
        fs.Read(buffer, 0, length)

        For i As Integer = 0 To buffer.Length - 1
            buffer(i) = buffer(i) Xor xorKey
        Next

        fs.Seek(offset, SeekOrigin.Begin)
        fs.Write(buffer, 0, buffer.Length)
        fs.Seek(originalPosition, SeekOrigin.Begin)
    End Sub

    Sub ExtractFile(outputPath As String, fs As FileStream, offset As Integer, compressedSize As Integer, Optional uncompressedSize As Integer = -1)
        Dim originalPosition As Long = fs.Position
        fs.Seek(offset, SeekOrigin.Begin)

        Dim buffer(compressedSize - 1) As Byte
        fs.Read(buffer, 0, compressedSize)

        File.WriteAllBytes(outputPath, buffer)

        fs.Seek(originalPosition, SeekOrigin.Begin)
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If UnpackModeCombo.SelectedIndex = 0 Then
            Dim inputFilePath As String = OpenArchiveDialog.FileName.ToString
            Dim outputFolderPath As String = SaveFolderDialog.SelectedPath.ToString

            If Not File.Exists(inputFilePath) Then
                MessageBox.Show("Input archive file not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            If outputFolderPath = "" Then
                'MessageBox.Show("No output folder selected!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                FolderPathTextBox.Text = Path.Combine(Path.GetDirectoryName(OpenArchiveDialog.FileName), Path.GetFileNameWithoutExtension(OpenArchiveDialog.FileName.ToString) & "-CIExtract")
                outputFolderPath = SaveFolderDialog.SelectedPath.ToString
            End If

            If Not Directory.Exists(outputFolderPath) Then
                Directory.CreateDirectory(outputFolderPath)
            End If

            Using fs As New FileStream(inputFilePath, FileMode.Open, FileAccess.ReadWrite)
                Using br As New BinaryReader(fs)

                    Dim fileCount As Integer = br.ReadInt32()
                    Dim fileType As Integer = 0

                    If fileCount <= 2 Then
                        fileCount = br.ReadInt32()
                        fileType = 1
                    End If


                    For i As Integer = 1 To fileCount
                        Dim currentOffset As Long = fs.Position
                        Dim testbyte As Byte = br.ReadByte()

                        fs.Seek(currentOffset, SeekOrigin.Begin)
                        Dim useXor As Boolean = (testbyte And &H80) <> 0
                        Dim xorKey As Byte = &HCC

                        If useXor Then
                            ApplyXor(fs, currentOffset, xorKey, &H50)
                        End If

                        Dim rawName As Byte() = br.ReadBytes(&H50)
                        Dim fileName As String = Encoding.ASCII.GetString(rawName).Split(Chr(0))(0).Trim()
                        fileName = CleanPath(fileName)

                        If useXor Then
                            ApplyXor(fs, currentOffset, xorKey, &H50)
                        End If

                        Dim fileOffset As Integer = br.ReadInt32()
                        Dim compressedSize As Integer = br.ReadInt32()
                        Dim uncompressedSize As Integer = If(fileType = 0, compressedSize, br.ReadInt32())

                        Dim fullFilePath As String = Path.Combine(outputFolderPath, fileName)
                        Dim directoryPath As String = Path.GetDirectoryName(fullFilePath)

                        If Not String.IsNullOrEmpty(directoryPath) AndAlso Not Directory.Exists(directoryPath) Then
                            Directory.CreateDirectory(directoryPath)
                        End If

                        If uncompressedSize = compressedSize Then
                            ExtractFile(fullFilePath, fs, fileOffset, compressedSize)
                        Else
                            ExtractFile(fullFilePath, fs, fileOffset, compressedSize, uncompressedSize)
                        End If

                    Next
                End Using
            End Using

            MessageBox.Show("Unpacking complete!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

        ElseIf UnpackModeCombo.SelectedIndex = 1 Then
            MessageBox.Show("Chicken Invaders Universe is not supported yet", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else
            MessageBox.Show("No unpack mode is selected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If

        ArchivePathTextBox.SelectionStart = ArchivePathTextBox.Text.Length
        ArchivePathTextBox.SelectionLength = 0
        FolderPathTextBox.SelectionStart = FolderPathTextBox.Text.Length
        FolderPathTextBox.SelectionLength = 0
    End Sub
    
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If UnpackModeCombo.SelectedIndex = 0 Then
            Dim inputFilePath As String = OpenArchiveDialog.FileName.ToString
            Dim outputFolderPath As String = SaveFolderDialog.SelectedPath.ToString

            If Not Directory.Exists(outputFolderPath) Then
                MessageBox.Show("Output folder not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            If Not File.Exists(inputFilePath) Then
                MessageBox.Show("Input archive file not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            File.Copy(inputFilePath, inputFilePath & ".CIExtractBackup")

            Using fs As New FileStream(inputFilePath, FileMode.Open, FileAccess.ReadWrite)
                Using br As New BinaryReader(fs)
                    Using bw As New BinaryWriter(fs)

                        Dim fileCount As Integer = br.ReadInt32()
                        Dim fileType As Integer = 0

                        If fileCount <= 2 Then
                            fileCount = br.ReadInt32()
                            fileType = 1
                        End If

                        For i As Integer = 1 To fileCount
                            Dim currentOffset As Long = fs.Position
                            Dim testbyte As Byte = br.ReadByte()

                            fs.Seek(currentOffset, SeekOrigin.Begin)
                            Dim useXor As Boolean = (testbyte And &H80) <> 0
                            Dim xorKey As Byte = &HCC

                            If useXor Then
                                ApplyXor(fs, currentOffset, xorKey, &H50)
                            End If

                            Dim rawName As Byte() = br.ReadBytes(&H50)
                            Dim fileName As String = System.Text.Encoding.ASCII.GetString(rawName).Split(Chr(0))(0).Trim()
                            fileName = CleanPath(fileName)

                            If useXor Then
                                ApplyXor(fs, currentOffset, xorKey, &H50)
                            End If

                            Dim fileOffset As Integer = br.ReadInt32()
                            Dim compressedSize As Integer = br.ReadInt32()
                            Dim uncompressedSize As Integer = If(fileType = 0, compressedSize, br.ReadInt32())

                            Dim fullFilePath As String = Path.Combine(outputFolderPath, fileName)

                            If File.Exists(fullFilePath) Then
                                Dim newFileBytes = File.ReadAllBytes(fullFilePath)
                                Dim originalPosition As Long = fs.Position

                                fs.Seek(fileOffset, SeekOrigin.Begin)
                                Dim originalFileBytes(compressedSize - 1) As Byte
                                fs.Read(originalFileBytes, 0, compressedSize)

                                If Not newFileBytes.SequenceEqual(originalFileBytes) Then
                                    Dim newSize As Integer = newFileBytes.Length
                                    Dim sizeDifference As Integer = newSize - compressedSize

                                    ' Rewrite header with updated size and adjust offsets if necessary
                                    If sizeDifference > 0 Then
                                        fs.Seek(0, SeekOrigin.End)
                                        Dim newOffset As Integer = CInt(fs.Position)

                                        fs.Seek(currentOffset + &H50, SeekOrigin.Begin)
                                        bw.Write(newOffset)
                                        bw.Write(newSize)
                                        If fileType <> 0 Then
                                            bw.Write(newSize) ' For uncompressed size
                                        End If

                                        ' Write new data at the end of the file
                                        fs.Seek(newOffset, SeekOrigin.Begin)
                                        fs.Write(newFileBytes, 0, newSize)
                                    Else
                                        ' Rewrite data in place if size is smaller or same
                                        fs.Seek(fileOffset, SeekOrigin.Begin)
                                        fs.Write(newFileBytes, 0, newSize)

                                        ' Handle leftover bytes if size decreased
                                        If sizeDifference < 0 Then
                                            Dim padding(-sizeDifference - 1) As Byte
                                            fs.Write(padding, 0, padding.Length)
                                        End If

                                        ' Update header with new size
                                        fs.Seek(currentOffset + &H50, SeekOrigin.Begin)
                                        bw.Write(fileOffset)
                                        bw.Write(newSize)
                                        If fileType <> 0 Then
                                            bw.Write(newSize)
                                        End If
                                    End If
                                End If

                                fs.Seek(originalPosition, SeekOrigin.Begin)
                            End If
                        Next
                    End Using
                End Using
            End Using
            MessageBox.Show("Repacking complete!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

        ElseIf UnpackModeCombo.SelectedIndex = 1 Then
            MessageBox.Show("Chicken Invaders Universe is not supported yet", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Else
            MessageBox.Show("No repack mode is selected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If

    End Sub

    Private Sub BrowseArchiveBtn_Click(sender As Object, e As EventArgs) Handles BrowseArchiveBtn.Click
        If OpenArchiveDialog.ShowDialog = DialogResult.OK Then

            ArchivePathTextBox.Text = OpenArchiveDialog.FileName.ToString

            If SaveFolderDialog.SelectedPath = "" Then
                FolderPathTextBox.Text = Path.Combine(Path.GetDirectoryName(OpenArchiveDialog.FileName), Path.GetFileNameWithoutExtension(OpenArchiveDialog.FileName.ToString) & "-CIExtract")
            End If

            ArchivePathTextBox.SelectionStart = ArchivePathTextBox.Text.Length
            ArchivePathTextBox.SelectionLength = 0
            FolderPathTextBox.SelectionStart = FolderPathTextBox.Text.Length
            FolderPathTextBox.SelectionLength = 0

        End If
    End Sub

    Private Sub SaveFolderBtn_Click(sender As Object, e As EventArgs) Handles SaveFolderBtn.Click
        If SaveFolderDialog.ShowDialog = DialogResult.OK Then
            FolderPathTextBox.Text = SaveFolderDialog.SelectedPath.ToString

            FolderPathTextBox.SelectionStart = FolderPathTextBox.Text.Length
            FolderPathTextBox.SelectionLength = 0

        End If
    End Sub

    Private Sub ArchivePathTextBox_TextChanged(sender As Object, e As EventArgs) Handles ArchivePathTextBox.TextChanged
        OpenArchiveDialog.FileName = ArchivePathTextBox.Text.ToString
    End Sub

    Private Sub FolderPathTextBox_TextChanged(sender As Object, e As EventArgs) Handles FolderPathTextBox.TextChanged
        SaveFolderDialog.SelectedPath = FolderPathTextBox.Text.ToString
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        UnpackModeCombo.SelectedIndex = 0
    End Sub

    Private Sub UnpackModeCombo_SelectedIndexChanged(sender As Object, e As EventArgs) Handles UnpackModeCombo.SelectedIndexChanged
        If UnpackModeCombo.SelectedIndex = 1 Then
            MessageBox.Show("Chicken Invaders Universe is not supported yet", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information)
            UnpackModeCombo.SelectedIndex = 0
        End If
    End Sub

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        Dim url As String = "https://github.com/RavenDS/"
        Process.Start(New ProcessStartInfo(url) With {.UseShellExecute = True})
    End Sub
End Class
