Imports System.Data.OleDb
Public Class Form1

    Dim con As New OleDb.OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=c:\users\ncuti\Documents\Dictionary.accdb")

    Private Sub txt_search_GotFocus(sender As Object, e As EventArgs) Handles txt_search.GotFocus
        If txt_search.Text = "search" Then
            txt_search.Text = ""
        End If
    End Sub

    Private Sub txt_search_LostFocus(sender As Object, e As EventArgs) Handles txt_search.LostFocus
        If txt_search.Text = "" Then
            txt_search.Text = "search"
        End If
    End Sub

    Sub DisplayData(ByVal table_name As String, ByVal data_grid_name As DataGridView, ByVal sort_item As String)
        Try
            Dim sql As String
            Dim cmd As New OleDb.OleDbCommand
            Dim dt As New DataTable
            Dim da As New OleDb.OleDbDataAdapter
            sql = "select * from " + table_name + " order by " + sort_item
            cmd.Connection = con
            cmd.CommandText = sql
            da.SelectCommand = cmd
            da.Fill(dt)
            data_grid_name.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            data_grid_name.DataSource = dt
            data_grid_name.Columns(0).Visible = False
            data_grid_name.Columns(1).Width = 300
        Catch ex As Exception
            MsgBox(ex.Message)
        Finally
            con.Close()
        End Try
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Call DisplayData("dictionary", DataGridView1, "Word")
    End Sub

    Function checkDuplicate(ByVal word As String)
        Dim sqlQRY As String = "SELECT COUNT(*) AS numRows FROM dictionary WHERE Word = '" & word & "'"
        Dim queryResult As Integer

        con.Open()

        Dim com As New OleDb.OleDbCommand(sqlQRY, con)
        queryResult = com.ExecuteScalar()

        con.Close()

        If queryResult > 0 Then
            MsgBox("Word is already exist")
            Return False
        Else
            Return True
        End If
    End Function

    Private Sub btn_add_Click(sender As Object, e As EventArgs) Handles btn_add.Click
        If txt_meaning.Text = "" Or txt_word.Text = "" Then
            MsgBox("Please fill all fields!")

        ElseIf checkDuplicate(txt_word.Text) Then
            Try
                Dim sql As String
                Dim cmd As New OleDb.OleDbCommand
                Dim i As Integer
                con.Open()
                sql = "INSERT INTO dictionary (Word, Meaning) VALUES ('" & txt_word.Text & "', '" & txt_meaning.Text & "');"
                cmd.Connection = con
                cmd.CommandText = sql
                i = cmd.ExecuteNonQuery
                If i > 0 Then
                    MsgBox("Successfull Added!")
                    ''clear fields
                    txt_meaning.Text = ""
                    txt_word.Text = ""
                    Call DisplayData("dictionary", DataGridView1, "Word")
                Else
                    MsgBox("Unable to insert into dictionary")
                End If

            Catch ex As Exception
                MsgBox(ex.Message)
            Finally
                con.Close()

            End Try
        End If
    End Sub

    Sub searchProduct(ByVal table_name As String, ByVal data_grid_name As DataGridView, ByVal word As String, ByVal word_field As String)
        Try
            Dim sql As String
            Dim cmd As New OleDb.OleDbCommand
            Dim dt As New DataTable
            Dim da As New OleDb.OleDbDataAdapter
            sql = "select * from " + table_name + " where " + word_field + " like '%" & word & "%' order by " + word_field
            cmd.Connection = con
            cmd.CommandText = sql
            da.SelectCommand = cmd
            dt.Clear()

            da.Fill(dt)
            data_grid_name.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            data_grid_name.DataSource = dt
            'data_grid_name.Columns(0).Visible = False
        Catch ex As Exception
            MsgBox(ex.Message)
        Finally
            con.Close()
        End Try
    End Sub

    Private Sub txt_search_TextChanged(sender As Object, e As EventArgs) Handles txt_search.TextChanged
        Call searchProduct("dictionary", DataGridView1, txt_search.Text, "Word")
    End Sub
End Class
