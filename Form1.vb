Imports System.Windows.Forms

Public Class Form1
    Const gridSize As Integer = 9
    Const mineCount As Integer = 10
    Dim buttons(gridSize - 1, gridSize - 1) As Button
    Dim mines(gridSize - 1, gridSize - 1) As Boolean
    Dim revealed(gridSize - 1, gridSize - 1) As Boolean

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Text = "Minesweeper"
        Me.ClientSize = New Size(gridSize * 50, gridSize * 50)
        InitializeGrid()
        PlaceMines()
    End Sub

    Private Sub InitializeGrid()
        For y = 0 To gridSize - 1
            For x = 0 To gridSize - 1
                Dim btn As New Button()
                btn.Width = 50
                btn.Height = 50
                btn.Left = x * 50
                btn.Top = y * 50
                btn.Tag = New Point(x, y)
                AddHandler btn.MouseDown, AddressOf Cell_Click
                buttons(x, y) = btn
                Me.Controls.Add(btn)
            Next
        Next
    End Sub

    Private Sub PlaceMines()
        Dim rnd As New Random()
        Dim placed As Integer = 0
        While placed < mineCount
            Dim x = rnd.Next(gridSize)
            Dim y = rnd.Next(gridSize)
            If Not mines(x, y) Then
                mines(x, y) = True
                placed += 1
            End If
        End While
    End Sub

    Private Sub Cell_Click(sender As Object, e As MouseEventArgs)
        Dim btn As Button = CType(sender, Button)
        Dim pt As Point = CType(btn.Tag, Point)
        If e.Button = MouseButtons.Right Then
            If btn.Text = "🚩" Then
                btn.Text = ""
            Else
                btn.Text = "🚩"
            End If
        Else
            RevealCell(pt.X, pt.Y)
        End If
    End Sub

    Private Sub RevealCell(x As Integer, y As Integer)
        If x < 0 OrElse y < 0 OrElse x >= gridSize OrElse y >= gridSize Then Return
        If revealed(x, y) Then Return

        revealed(x, y) = True
        Dim btn = buttons(x, y)

        If mines(x, y) Then
            btn.Text = "💣"
            btn.BackColor = Color.Red
            MessageBox.Show("Game Over!")
            RevealAllMines()
            Return
        End If

        Dim count = CountAdjacentMines(x, y)
        If count > 0 Then
            btn.Text = count.ToString()
        Else
            btn.Enabled = False
            For dy = -1 To 1
                For dx = -1 To 1
                    If dx <> 0 OrElse dy <> 0 Then
                        RevealCell(x + dx, y + dy)
                    End If
                Next
            Next
        End If

        btn.Enabled = False
        CheckWin()
    End Sub

    Private Function CountAdjacentMines(x As Integer, y As Integer) As Integer
        Dim count As Integer = 0
        For dy = -1 To 1
            For dx = -1 To 1
                Dim nx = x + dx
                Dim ny = y + dy
                If nx >= 0 AndAlso ny >= 0 AndAlso nx < gridSize AndAlso ny < gridSize Then
                    If mines(nx, ny) Then count += 1
                End If
            Next
        Next
        Return count
    End Function

    Private Sub RevealAllMines()
        For y = 0 To gridSize - 1
            For x = 0 To gridSize - 1
                If mines(x, y) Then
                    buttons(x, y).Text = "💣"
                    buttons(x, y).BackColor = Color.LightGray
                End If
            Next
        Next
    End Sub

    Private Sub CheckWin()
        Dim safeCells = gridSize * gridSize - mineCount
        Dim revealedCount = 0
        For y = 0 To gridSize - 1
            For x = 0 To gridSize - 1
                If revealed(x, y) AndAlso Not mines(x, y) Then
                    revealedCount += 1
                End If
            Next
        Next
        If revealedCount = safeCells Then
            MessageBox.Show("You Win!")
            RevealAllMines()
        End If
    End Sub
End Class
