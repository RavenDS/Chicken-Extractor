Imports System.CodeDom.Compiler
Imports System.IO
Imports System.IO.Compression
Imports System.Net
Imports System.Security.Cryptography
Imports System.Text
Imports System.Windows.Forms.VisualStyles.VisualStyleElement


Public Class Form1

    Dim UnpackSuccessCIU As Boolean = False
    Dim RepackSuccessCIU As Boolean = False
    Dim processTGAsuccess As Boolean = False

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        UnpackModeCombo.SelectedIndex = 1
        If Not File.Exists("CIUTable.txt") Then File.WriteAllText("CIUTable.txt", My.Resources.CIUTable)
        TGACheckBox.Checked = True
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        If UnpackModeCombo.SelectedIndex = 0 Then

            UnpackCI()

        ElseIf UnpackModeCombo.SelectedIndex = 1 Then

            If Not File.Exists("CIUTable.txt") Then
                MsgBox("CIU Table not found!" & vbCrLf & vbCrLf & "Please restart the tool, or update table from internet.", MsgBoxStyle.Critical, "Error!")
                Exit Sub
            End If

            UnpackCIU()

            If TGACheckBox.Checked = True And UnpackSuccessCIU = True Then ProcessTGAFiles()

            If UnpackSuccessCIU = True Then MsgBox("Data succesfully unpacked !", MsgBoxStyle.Information, "Done")

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

            RepackCI()

        ElseIf UnpackModeCombo.SelectedIndex = 1 Then

            If Not File.Exists("CIUTable.txt") Then
                MsgBox("CIU Table not found!" & vbCrLf & vbCrLf & "Please restart the tool, or update the table from the internet.", MsgBoxStyle.Critical, "Error!")
                Exit Sub
            End If

            If TGACheckBox.Checked = True Then ProcessTGAFiles()

            RepackCIU()

            If TGACheckBox.Checked = True Then ProcessTGAFiles()

            If RepackSuccessCIU = True Then MsgBox("Data repacked succesfully", MsgBoxStyle.Information, "Success")

            Else
                MessageBox.Show("No repack mode is selected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If

    End Sub
    Private Sub BrowseArchiveBtn_Click(sender As Object, e As EventArgs) Handles BrowseArchiveBtn.Click
        If OpenArchiveDialog.ShowDialog = DialogResult.OK Then

            ArchivePathTextBox.Text = OpenArchiveDialog.FileName.ToString

            'If SaveFolderDialog.SelectedPath = "" Then
            'FolderPathTextBox.Text = Path.Combine(Path.GetDirectoryName(OpenArchiveDialog.FileName), Path.GetFileNameWithoutExtension(OpenArchiveDialog.FileName.ToString) & "-CIExtract")
            'End If

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
    Private Sub UpdTableBtn_Click(sender As Object, e As EventArgs) Handles UpdTableBtn.Click
        If MsgBox("This will update the CIU table from the internet." & vbCrLf & vbCrLf & "Continue ?", MsgBoxStyle.YesNo, "Update table") = MsgBoxResult.No Then
            Exit Sub
        End If

        Dim onlineFileUrl As String = "https://raw.githubusercontent.com/RavenDS/Chicken-Extractor/main/CIUTable.txt"
        Dim localFilePath As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CIUTable.txt")

        Try
            ' download online file to temp location
            Dim tempFilePath As String = Path.GetTempFileName()

            Using client As New WebClient()
                client.DownloadFile(onlineFileUrl, tempFilePath)
            End Using

            ' compare online file with local file
            If Not File.Exists(localFilePath) OrElse Not FilesAreEqual(localFilePath, tempFilePath) Then
                File.Copy(tempFilePath, localFilePath, True)
                MsgBox("The table has been updated with the online version", MsgBoxStyle.Information, "Updated")
            Else
                MsgBox("The table is up-to-date", MsgBoxStyle.Information, "Done")
            End If

            File.Delete(tempFilePath)

        Catch ex As Exception
            MsgBox("An error occurred: " & vbCrLf & ex.Message, MsgBoxStyle.Critical, "Error")
        End Try
    End Sub

    Private Sub ArchivePathTextBox_TextChanged(sender As Object, e As EventArgs) Handles ArchivePathTextBox.TextChanged
        OpenArchiveDialog.FileName = ArchivePathTextBox.Text.ToString
    End Sub

    Private Sub FolderPathTextBox_TextChanged(sender As Object, e As EventArgs) Handles FolderPathTextBox.TextChanged
        SaveFolderDialog.SelectedPath = FolderPathTextBox.Text.ToString
    End Sub
    Private Sub UnpackModeCombo_SelectedIndexChanged(sender As Object, e As EventArgs) Handles UnpackModeCombo.SelectedIndexChanged
        If UnpackModeCombo.SelectedIndex = 0 Then
            TGACheckBox.Enabled = False
        ElseIf UnpackModeCombo.SelectedIndex = 1 Then
            TGACheckBox.Enabled = True
        End If
    End Sub

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        Dim url As String = "https://github.com/RavenDS/"
        Process.Start(New ProcessStartInfo(url) With {.UseShellExecute = True})
    End Sub
    Sub UnpackCI()

        Dim inputFilePath As String = OpenArchiveDialog.FileName.ToString
        Dim outputFolderPath As String = SaveFolderDialog.SelectedPath.ToString

        If Not File.Exists(inputFilePath) Then
            MessageBox.Show("Input archive file not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End If

        If outputFolderPath = "" Then
            FolderPathTextBox.Text = Path.Combine(Path.GetDirectoryName(OpenArchiveDialog.FileName), Path.GetFileNameWithoutExtension(OpenArchiveDialog.FileName.ToString) & "-CIExtract")
            outputFolderPath = SaveFolderDialog.SelectedPath.ToString
        End If

        Try

            If Not Directory.Exists(outputFolderPath) Then
                Directory.CreateDirectory(outputFolderPath)
            Else
                If MsgBox("Folder already exists! Overwrite Data ?", MsgBoxStyle.YesNo, "Folder") = MsgBoxResult.Yes Then
                    Directory.Delete(outputFolderPath, True)
                    Directory.CreateDirectory(outputFolderPath)
                Else
                    Exit Sub
                End If
            End If

            Me.Cursor = Cursors.WaitCursor

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

            Me.Cursor = Cursors.Default
            MessageBox.Show("Unpacking complete!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MsgBox("An error occured: " & vbCrLf & ex.Message, MsgBoxStyle.Critical, "Error")
            Exit Sub
        End Try

    End Sub
    Sub RepackCI()

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

        Try

            If File.Exists(inputFilePath) Then
                For i = 0 To 999
                    If Not File.Exists(inputFilePath & ".CIExtractBackup" & i) Then
                        File.Move(inputFilePath, inputFilePath & ".CIExtractBackup" & i)
                        Exit For
                    End If
                Next
            End If

            Me.Cursor = Cursors.WaitCursor

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

                                    ' rewrite header with updated size & adjust offsets if necessary
                                    If sizeDifference > 0 Then
                                        fs.Seek(0, SeekOrigin.End)
                                        Dim newOffset As Integer = CInt(fs.Position)

                                        fs.Seek(currentOffset + &H50, SeekOrigin.Begin)
                                        bw.Write(newOffset)
                                        bw.Write(newSize)
                                        If fileType <> 0 Then
                                            bw.Write(newSize) ' for uncompressed size
                                        End If

                                        ' write new data at end of file
                                        fs.Seek(newOffset, SeekOrigin.Begin)
                                        fs.Write(newFileBytes, 0, newSize)
                                    Else
                                        ' rewrite data in place if size < or =
                                        fs.Seek(fileOffset, SeekOrigin.Begin)
                                        fs.Write(newFileBytes, 0, newSize)

                                        ' handle leftover bytes if size decrease
                                        If sizeDifference < 0 Then
                                            Dim padding(-sizeDifference - 1) As Byte
                                            fs.Write(padding, 0, padding.Length)
                                        End If

                                        ' update header with new size
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
            Me.Cursor = Cursors.Default
            MessageBox.Show("Repacking complete!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MsgBox("An error occured: " & vbCrLf & ex.Message, MsgBoxStyle.Critical, "Error")
            Exit Sub
        End Try

    End Sub

    Sub UnpackCIU()

        'based on work by Luigi Auriemma https://aluigi.altervista.org/bms/chicken_invaders_universe.bms

        UnpackSuccessCIU = False

        Dim inputfilePath As String = OpenArchiveDialog.FileName.ToString
        Dim offsetTablePath As String = "offsetTable.txt"
        Dim outputFolderPath As String = SaveFolderDialog.SelectedPath.ToString

        If Not File.Exists(inputfilePath) Then
            MessageBox.Show("Input archive file not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            UnpackSuccessCIU = False
            Exit Sub
        End If

        'If outputFolderPath = "" Then
        'FolderPathTextBox.Text = Path.Combine(Path.GetDirectoryName(OpenArchiveDialog.FileName), Path.GetFileNameWithoutExtension(OpenArchiveDialog.FileName.ToString) & "-CIExtract")
        'outputFolderPath = SaveFolderDialog.SelectedPath.ToString
        'End If

        Try

            If File.Exists(offsetTablePath) Then File.Delete(offsetTablePath)

            If Not Directory.Exists(outputFolderPath) Then
                Directory.CreateDirectory(outputFolderPath)
            Else
                If MsgBox("Folder already exists! Overwrite Data ?", MsgBoxStyle.YesNo, "Folder") = MsgBoxResult.Yes Then
                    Directory.Delete(outputFolderPath, True)
                    Directory.CreateDirectory(outputFolderPath)
                Else
                    UnpackSuccessCIU = False
                    Exit Sub
                End If
            End If

            ProgressBar1.Value = 0
            Me.Cursor = Cursors.WaitCursor

            Using fs As New FileStream(inputfilePath, FileMode.Open, FileAccess.Read)
                Using br As New BinaryReader(fs)

                    ' read 4-byte string (TEST)
                    Dim TEST As String = Encoding.ASCII.GetString(br.ReadBytes(4))

                    ' go to beginning of file
                    br.BaseStream.Seek(0, SeekOrigin.Begin)

                    ' check first idstring
                    If Encoding.ASCII.GetString(br.ReadBytes(4)) <> "UVE " Then
                        Throw New Exception("Invalid file format: Missing 'UVE '")
                    End If

                    ' check second idstring
                    If Encoding.ASCII.GetString(br.ReadBytes(4)) <> "WAD " Then
                        Throw New Exception("Invalid file format: Missing 'WAD '")
                    End If

                    ' read DUMMY (unused long value)
                    Dim DUMMY As Integer = br.ReadInt32()

                    ' read file count
                    Dim FILES As Integer = br.ReadInt32()

                    ProgressBar1.Maximum = FILES

                    ' iterate through each file entry
                    For i As Integer = 0 To FILES - 1

                        ' read NAME_CRC value
                        Dim NAME_CRC As Integer = br.ReadInt32()

                        ' convert NAME_CRC to hex string
                        Dim NAME As String = NAME_CRC.ToString("X8")

                        ' call EXTRACT function
                        MakeOffsetTableCIU(NAME.ToLower, br, offsetTablePath)

                        ProgressBar1.Value += 1
                    Next

                    ProgressBar1.Value = 0

                    Dim offsetlines As String() = File.ReadAllLines(offsetTablePath)

                    ProgressBar1.Maximum = offsetlines.Length

                    Using binFile As FileStream = New FileStream(inputfilePath, FileMode.Open, FileAccess.Read)
                        For Each line As String In offsetlines
                            If Not String.IsNullOrWhiteSpace(line) Then

                                ' Parse line into filename, offset, and size
                                Dim parts As String() = line.Split(","c)
                                If parts.Length = 3 Then
                                    Dim fileName As String = parts(0).Trim()
                                    Dim offset As Long = Long.Parse(parts(1).Trim())
                                    Dim size As Integer = Integer.Parse(parts(2).Trim())

                                    ' validate offset and size
                                    If offset >= 0 AndAlso size > 0 AndAlso offset + size <= binFile.Length Then
                                        ' read bytes from .bin
                                        Dim buffer(size - 1) As Byte
                                        binFile.Seek(offset, SeekOrigin.Begin)
                                        binFile.Read(buffer, 0, size)

                                        ' write bytes to new file
                                        Dim outputPath As String = Path.Combine(outputFolderPath, fileName)
                                        File.WriteAllBytes(outputPath, buffer)
                                    Else
                                        Console.WriteLine($"Invalid offset or size for line: {line}")
                                    End If
                                Else
                                    Console.WriteLine($"Invalid format for line: {line}")
                                End If
                            End If
                            ProgressBar1.Value += 1
                        Next
                    End Using
                End Using
            End Using

            NameCIU()
            Me.Cursor = Cursors.Default
            UnpackSuccessCIU = True

        Catch ex As Exception
            UnpackSuccessCIU = False
            MsgBox("An error occured: " & vbCrLf & ex.Message, MsgBoxStyle.Critical, "Error")
            Exit Sub
        End Try

    End Sub

    Function MakeOffsetTableCIU(fileName As String, reader As BinaryReader, tablepath As String)

        ' read OFFSET, compressed size, & actual size
        Dim OFFSET As Integer = reader.ReadInt32()
        Dim ZSIZE As Integer = reader.ReadInt32()
        Dim SIZE As Integer = reader.ReadInt32()

        Using writer As New StreamWriter(tablepath, FileMode.OpenOrCreate)
            If SIZE = ZSIZE Then
                ' write file with SIZE
                writer.WriteLine(fileName & "," & OFFSET & "," & SIZE)
            Else
                ' write file with ZSIZE and SIZE
                writer.WriteLine(fileName & "," & OFFSET & "," & ZSIZE & "," & SIZE)
            End If
        End Using

    End Function

    Sub RepackCIU()

        RepackSuccessCIU = False

        Dim tableFilePath As String = "offsetTable.txt"
        Dim inputFilePath As String = OpenArchiveDialog.FileName.ToString
        Dim directoryPath As String = SaveFolderDialog.SelectedPath.ToString

        If Not File.Exists(tableFilePath) Then
            RepackSuccessCIU = False
            MsgBox("No OffsetTable.txt ! Unpack the main archive again", MsgBoxStyle.Critical, "Error")
            Exit Sub
        End If

        If Not Directory.Exists(directoryPath) Then
            RepackSuccessCIU = False
            MessageBox.Show("Data folder not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End If

        Try

            If File.Exists(inputFilePath) Then
                For i = 0 To 999
                    If Not File.Exists(inputFilePath & ".CIExtractBackup" & i) Then
                        File.Move(inputFilePath, inputFilePath & ".CIExtractBackup" & i)
                        Exit For
                    End If
                Next
                File.Delete(inputFilePath)
            End If

            DeNameCIU()

            ' read & analyze offset table
            Dim offsetTable As List(Of String) = File.ReadAllLines(tableFilePath).ToList()
            Dim updatedTable As New List(Of (Filename As Integer, Offset As Integer, Size As Integer))

            Dim currentOffsetAdjustment As Integer = 0

            For Each line In offsetTable
                Dim lineParts = line.Split(","c)
                Dim filename = Convert.ToInt32(lineParts(0).Trim(), 16) ' filename to hex int32
                Dim offset = Integer.Parse(lineParts(1).Trim())
                Dim size = Integer.Parse(lineParts(2).Trim())

                Dim filePath = Path.Combine(directoryPath, filename.ToString("X8"))

                If File.Exists(filePath) Then
                    Dim actualFileSize = CInt(New FileInfo(filePath).Length)

                    If actualFileSize > size Then
                        ' update size
                        size = actualFileSize

                        ' update offsets for next lines
                        Dim sizeDifference = size - Integer.Parse(lineParts(2).Trim())
                        currentOffsetAdjustment += sizeDifference
                    ElseIf actualFileSize < size Then
                        ' update size
                        size = actualFileSize
                    End If
                Else
                    Console.WriteLine($"Fichier manquant : {filePath}")
                    Continue For
                End If

                ' add
                updatedTable.Add((filename, offset + currentOffsetAdjustment, size))
            Next

            ' write to output.bin
            Using writer As New BinaryWriter(File.Open(inputFilePath, FileMode.Create, FileAccess.Write))

                Dim fileCount As Integer = Directory.GetFiles(directoryPath).Length
                Dim fileCountBytes As Byte() = BitConverter.GetBytes(fileCount)

                ' header as byte array
                Dim staticHeader As Byte() = {
             &H55, &H56, &H45, &H20, ' "UVE " (signature)
             &H57, &H41, &H44, &H20, ' "WAD " (signature)
             &H4, &H0, &H0, &H0      ' static
             }

                ' combine static & file count
                Dim header As Byte() = staticHeader.Concat(fileCountBytes).ToArray()

                ' write header to file
                writer.Write(header)

                ' write updated table
                For Each entry In updatedTable

                    ' reverse bytes of 'filename'
                    Dim filenameBytes = BitConverter.GetBytes(entry.Filename)

                    writer.Write(filenameBytes)

                    ' write offset & size
                    writer.Write(entry.Offset)
                    writer.Write(entry.Size)

                    ' write size (2x same value
                    writer.Write(entry.Size)
                Next

                ' write files contents
                For Each entry In updatedTable
                    Dim filePath = Path.Combine(directoryPath, entry.Filename.ToString("X8"))

                    If File.Exists(filePath) Then
                        Dim fileBytes = File.ReadAllBytes(filePath)
                        writer.Seek(entry.Offset, SeekOrigin.Begin)
                        writer.Write(fileBytes)
                    End If
                Next
            End Using
            NameCIU()
            RepackSuccessCIU = True
        Catch ex As Exception
            RepackSuccessCIU = False
            MsgBox("An error occured: " & vbCrLf & ex.Message, MsgBoxStyle.Critical, "Error")
        End Try

    End Sub



    Private Sub NameCIU()

        Dim sourceFolder As String = SaveFolderDialog.SelectedPath.ToString

        Dim mappingFilePath As String = "CIUTable.txt"

        ' load table
        Dim mappings As New Dictionary(Of String, String)

        ' table to string array
        Dim lines As String() = File.ReadAllLines(mappingFilePath)

        For Each line As String In lines
            Dim parts As String() = line.Split("|")
            If parts.Length = 2 Then
                Dim hash As String = parts(0).Trim()
                Dim newPath As String = parts(1).Trim()
                mappings(hash) = newPath
            End If
        Next

        ' handle files
        For Each filePath As String In Directory.GetFiles(sourceFolder)
            Dim fileName As String = Path.GetFileNameWithoutExtension(filePath)

            If mappings.ContainsKey(fileName) Then
                Dim relativePath As String = mappings(fileName)
                Dim newFileName As String = Path.GetFileName(relativePath)
                Dim relativeDirectory As String = Path.GetDirectoryName(relativePath)

                Dim destinationDirectory As String = Path.Combine(sourceFolder, relativeDirectory)
                Directory.CreateDirectory(destinationDirectory)

                Dim newFilePath As String = Path.Combine(destinationDirectory, newFileName)
                File.Move(filePath, newFilePath)

                Console.WriteLine($"File '{fileName}' renamed as '{newFileName}' & moved to '{destinationDirectory}'.")
            Else
                Console.WriteLine($"No corresponding file for'{fileName}'.")
            End If
        Next

    End Sub

    Private Sub DeNameCIU()

        Dim sourceFolder As String = SaveFolderDialog.SelectedPath.ToString

        Dim mappingFilePath As String = "CIUTable.txt"

        ' load table (reversed)
        Dim mappings As New Dictionary(Of String, String)
        Dim lines As String() = File.ReadAllLines(mappingFilePath)

        For Each line As String In lines
            Dim parts As String() = line.Split("|")
            If parts.Length = 2 Then
                Dim hash As String = parts(0).Trim()
                Dim newPath As String = parts(1).Trim()
                mappings(newPath) = hash
            End If
        Next

        ' handle files
        For Each relativePath As String In mappings.Keys
            Dim filePath As String = Path.Combine(sourceFolder, relativePath)
            If File.Exists(filePath) Then
                Dim hash As String = mappings(relativePath)
                Dim newFilePath As String = Path.Combine(sourceFolder, hash)

                File.Move(filePath, newFilePath)
                Console.WriteLine($"File '{filePath}' renamed to '{newFilePath}'.")
            Else
                Console.WriteLine($"Can't find : '{filePath}'.")
            End If
        Next

        For Each subDir As String In Directory.GetDirectories(sourceFolder)

            Directory.Delete(subDir, True)

        Next

    End Sub

    Private Sub ProcessTGAFiles()

        If SaveFolderDialog.SelectedPath.ToString = "" Then
            Exit Sub
        ElseIf Not Directory.Exists(SaveFolderDialog.SelectedPath.ToString) Then
            Exit Sub
        End If

        Try
            Dim tgaFiles = Directory.EnumerateFiles(SaveFolderDialog.SelectedPath.ToString, "*.tga", SearchOption.AllDirectories)

            ProgressBar1.Value = 0
            ProgressBar1.Maximum = tgaFiles.Count

            Cursor = Cursors.WaitCursor

            ' process each .tga file
            For Each tgaFile In tgaFiles
                ProcessTGA(tgaFile)
                ProgressBar1.Value += 1
            Next

            Cursor = Cursors.Default

        Catch ex As Exception
            If RepackSuccessCIU = True Or UnpackSuccessCIU = True Then
                MsgBox("Files have unpacked, but processsing of TGA files failed: " & vbCrLf & ex.Message & vbCrLf & vbCrLf & "Try to restart without TGA file processing.", MsgBoxStyle.Critical, "Error")
                processTGAsuccess = False
            Else
                MsgBox("An error occured while processing TGA files: " & vbCrLf & ex.Message, MsgBoxStyle.Critical, "Error")
                processTGAsuccess = False
            End If
        End Try

    End Sub

    Sub ProcessTGA(filePath As String)

        'based on work by Luigi Auriemma https://aluigi.altervista.org/bms/chicken_invaders_tga.bms 

        Dim inputFileName As String = Path.GetFileNameWithoutExtension(filePath)
        Dim outputFileName As String = inputFileName & ".process"
        Dim outputFilePath As String = Path.Combine(Path.GetDirectoryName(filePath), outputFileName)

        Try
            Using fs As New FileStream(filePath, FileMode.Open, FileAccess.Read)
                Using br As New BinaryReader(fs)

                    ' create MemoryStream for MEMORY_FILE
                    Using memoryStream As New MemoryStream()
                        Using bw As New BinaryWriter(memoryStream)
                            Dim WIDTH As Integer = br.ReadInt32()
                            Dim HEIGHT As Integer = br.ReadInt32()
                            Dim DEPTH As Integer = br.ReadInt32()

                            If DEPTH = 0 Then
                                ' encoding
                                fs.Seek(0, SeekOrigin.Begin)
                                Dim DUMMY(11) As Byte
                                br.Read(DUMMY, 0, 12) ' skip ID length, color map, and image stuff
                                WIDTH = br.ReadInt16()
                                HEIGHT = br.ReadInt16()
                                DEPTH = br.ReadByte()
                                Dim IMG_DESC As Byte = br.ReadByte()

                                ' read image data
                                Dim imageData(HEIGHT - 1, WIDTH * (DEPTH \ 8) - 1) As Byte
                                For y = 0 To HEIGHT - 1
                                    Dim tempRow(WIDTH * (DEPTH \ 8) - 1) As Byte
                                    br.Read(tempRow, 0, tempRow.Length)
                                    For x = 0 To tempRow.Length - 1
                                        imageData(y, x) = tempRow(x)
                                    Next
                                Next

                                ' flip vertically & Swap BGR to RGB
                                imageData = FlipAndSwapImage(imageData, WIDTH, HEIGHT, DEPTH)

                                ' write to MEMORY_FILE
                                bw.Write(WIDTH)
                                bw.Write(HEIGHT)
                                bw.Write(DEPTH)

                                For y = 0 To HEIGHT - 1
                                    Dim tempRow(WIDTH * (DEPTH \ 8) - 1) As Byte
                                    For x = 0 To tempRow.Length - 1
                                        tempRow(x) = imageData(y, x)
                                    Next
                                    bw.Write(tempRow, 0, tempRow.Length)
                                Next
                            Else
                                ' decoding
                                ' write to MEMORY_FILE
                                bw.Write(CByte(0)) ' ID length
                                bw.Write(CByte(0)) ' color map type
                                If DEPTH = 8 Then
                                    bw.Write(CByte(3)) ' grayscale
                                Else
                                    bw.Write(CByte(2)) ' true-color
                                End If
                                bw.Write(CShort(0)) ' color map origin
                                bw.Write(CShort(0)) ' color map length
                                bw.Write(CByte(0)) ' color map depth
                                bw.Write(CShort(0)) ' X origin
                                bw.Write(CShort(0)) ' Y origin
                                bw.Write(CShort(WIDTH))
                                bw.Write(CShort(HEIGHT))
                                bw.Write(CByte(DEPTH))
                                bw.Write(CByte(0))

                                ' read image data
                                Dim imageData(HEIGHT - 1, WIDTH * (DEPTH \ 8) - 1) As Byte
                                For y = 0 To HEIGHT - 1
                                    Dim tempRow(WIDTH * (DEPTH \ 8) - 1) As Byte
                                    br.Read(tempRow, 0, tempRow.Length)
                                    For x = 0 To tempRow.Length - 1
                                        imageData(y, x) = tempRow(x)
                                    Next
                                Next

                                ' flip vertically & swap BGR to RGB
                                imageData = FlipAndSwapImage(imageData, WIDTH, HEIGHT, DEPTH)

                                For y = 0 To HEIGHT - 1
                                    Dim tempRow(WIDTH * (DEPTH \ 8) - 1) As Byte
                                    For x = 0 To tempRow.Length - 1
                                        tempRow(x) = imageData(y, x)
                                    Next
                                    bw.Write(tempRow, 0, tempRow.Length)
                                Next
                            End If

                            ' save MEMORY_FILE content
                            Dim memoryData As Byte() = memoryStream.ToArray()
                            File.WriteAllBytes(outputFilePath, memoryData)

                            ' append remaining data
                            Dim OFFSET As Long = fs.Position
                            Dim SIZE As Long = fs.Length - OFFSET
                            If SIZE > 0 Then
                                Dim remainingData(SIZE - 1) As Byte
                                fs.Read(remainingData, 0, SIZE)
                                Using appendFs As New FileStream(outputFilePath, FileMode.Append, FileAccess.Write)
                                    appendFs.Write(remainingData, 0, remainingData.Length)
                                End Using
                            End If
                        End Using
                    End Using
                End Using
            End Using

            File.Delete(filePath)
            File.Move(outputFilePath, filePath)

            Console.WriteLine($"Output file saved as: {outputFilePath}")
        Catch ex As Exception
            MsgBox("Exception for TGA file:" & inputFileName & vbCrLf & vbCrLf &
                   "Error:" & ex.Message & vbCrLf & vbCrLf &
                   "Aborting.", MsgBoxStyle.Critical, "Error")
            Exit Sub
        End Try

    End Sub

    Private Function FlipAndSwapImage(imageData(,) As Byte, width As Integer, height As Integer, depth As Integer) As Byte(,)

        Dim bytesPerPixel As Integer = depth \ 8
        Dim flippedImage(height - 1, width * bytesPerPixel - 1) As Byte
        Try
            ' flip image vertical
            For y = 0 To height - 1
                Dim targetRow As Integer = height - 1 - y ' flip vertical
                For x = 0 To width - 1
                    Dim pixelStart As Integer = x * bytesPerPixel

                    ' copy & swap channels for each pixel
                    If bytesPerPixel >= 3 Then
                        ' assuming at least 3 bytes by pixel (BGR/RGB)
                        flippedImage(targetRow, pixelStart) = imageData(y, pixelStart + 2) ' R -> B
                        flippedImage(targetRow, pixelStart + 1) = imageData(y, pixelStart + 1) ' G -> G
                        flippedImage(targetRow, pixelStart + 2) = imageData(y, pixelStart) ' B -> R

                        ' if alpha channel, copy as-is
                        If bytesPerPixel = 4 Then
                            flippedImage(targetRow, pixelStart + 3) = imageData(y, pixelStart + 3) ' A -> A
                        End If
                    Else
                        ' if < 3 bytes per pixel (unlikely for TGA), just copy
                        For b = 0 To bytesPerPixel - 1
                            flippedImage(targetRow, pixelStart + b) = imageData(y, pixelStart + b)
                        Next
                    End If
                Next
            Next

            Return flippedImage
        Catch ex As Exception
            MsgBox("Error while swapping image:" & ex.Message & vbCrLf & vbCrLf &
                   "Aborting.", MsgBoxStyle.Critical, "Error")
            Exit Function
        End Try
    End Function

    ' function to compare two files using a hash
    Private Function FilesAreEqual(filePath1 As String, filePath2 As String) As Boolean
        Using hashAlgorithm As HashAlgorithm = SHA256.Create()
            Dim hash1 As Byte() = ComputeFileHash(filePath1, hashAlgorithm)
            Dim hash2 As Byte() = ComputeFileHash(filePath2, hashAlgorithm)
            Return hash1.SequenceEqual(hash2)
        End Using
    End Function
    Private Function ComputeFileHash(filePath As String, hashAlgorithm As HashAlgorithm) As Byte()
        Using stream As FileStream = File.OpenRead(filePath)
            Return hashAlgorithm.ComputeHash(stream)
        End Using
    End Function

    Function CleanPath(filePath As String) As String
        filePath = filePath.Replace("/", Path.DirectorySeparatorChar).Replace("\", Path.DirectorySeparatorChar)
        Return String.Concat(filePath.Split(Path.GetInvalidPathChars())).Trim()
    End Function

    Sub ApplyXor(fs As FileStream, offset As Long, xorKey As Byte, length As Integer)
        Try
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
        Catch ex As Exception
            MsgBox("Error: " & vbCrLf & ex.Message, MsgBoxStyle.Critical, "Error")
            Exit Sub
        End Try
    End Sub

    Sub ExtractFile(outputPath As String, fs As FileStream, offset As Integer, compressedSize As Integer, Optional uncompressedSize As Integer = -1)
        Try
            If UnpackModeCombo.SelectedIndex = 0 Then
                Dim originalPosition As Long = fs.Position
                fs.Seek(offset, SeekOrigin.Begin)

                Dim buffer(compressedSize - 1) As Byte
                fs.Read(buffer, 0, compressedSize)

                File.WriteAllBytes(outputPath, buffer)

                fs.Seek(originalPosition, SeekOrigin.Begin)
            End If
        Catch ex As Exception
            MsgBox("Error: " & vbCrLf & ex.Message, MsgBoxStyle.Critical, "Error")
            Exit Sub
        End Try
    End Sub

    Private Sub TGACheckBox_CheckedChanged(sender As Object, e As EventArgs) Handles TGACheckBox.CheckedChanged

    End Sub
End Class
