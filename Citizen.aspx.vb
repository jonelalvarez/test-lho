Imports System.Data
Imports System.Web.Services
Imports MySql.Data.MySqlClient
Imports System.IO

Partial Class Citizen
    Inherits System.Web.UI.Page
    Dim ConnString As String
    Dim clsMaster As New cls_master
    Dim requestID As String

    Protected Sub Citizen_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Not Request.QueryString("ID") = Nothing Then
            requestID = clsMaster.Decrypt(Request.QueryString("ID").Replace(" ", "+"))
        End If

        ConnString = ConfigurationManager.ConnectionStrings("conn1").ConnectionString
    End Sub

    Private Sub Citizen_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Try
                Purok()
                Barangay()
                If Not requestID = Nothing Then
                    showDetails()
                End If
            Catch ex As Exception
                ClientScript.RegisterStartupScript(Me.[GetType](), "systemMsg", "systemMsg(0, '" & ex.Message.Replace("'", "|") & "');", True)
            End Try
        End If
    End Sub

    Private Sub showDetails()
        With clsMaster
            Dim foundRow As DataRow() = .dynamicQuery("SELECT ID, FirstName, MiddleName, LastName, Suffix, Birthdate, ShortAddress, Case When citizen_tbl.Status = '1' Then 'Active' Else 'Inactive' End as Status, PurokID" &
           "                      From citizen_tbl where ID = '" & requestID & "'").Select
            If foundRow.Length > 0 Then
                btn_add.Visible = False

                h2_header.InnerText = "Ref No: " & foundRow(0).Item("ID").ToString
                div_list.Attributes("class") = "x_panel hide"
                div_details.Attributes("class") = "x_panel"
                li_timeline.Visible = True


                txt_fName.Value = foundRow(0).Item("FirstName").ToString
                txt_mName.Value = foundRow(0).Item("MiddleName").ToString
                txt_lName.Value = foundRow(0).Item("LastName").ToString
                dl_suffix.Value = foundRow(0).Item("Suffix").ToString
                txt_bdate.Value = Format(foundRow(0).Item("Birthdate"), "MM/dd/yyyy")
                txt_address.Value = foundRow(0).Item("ShortAddress").ToString
                dl_status.Value = foundRow(0).Item("status").ToString
                dl_psID.Value = foundRow(0).Item("PurokID").ToString

                ClientScript.RegisterStartupScript(Me.[GetType](), "showModalAdd", "showModalAdd();", True)
            Else
                ClientScript.RegisterStartupScript(Me.[GetType](), "systemMsg", "systemMsg(0, 'Selected record did not exists.');", True)
                Response.Redirect(Master.activePage)
            End If
        End With
    End Sub

    Protected Sub Save_Click()
        If Not requestID = Nothing Then
            CRUD("U")
        Else
            CRUD("C")
        End If
    End Sub

    Protected Sub CRUD(_mode As String)
        If IsPostBack Then
            Try
                Using con As New MySqlConnection(ConnString)
                    con.Open()
                    Dim sql As String = Nothing

                    If _mode = "C" Then
                        sql = "Insert into citizen_tbl (ID, FirstName, MiddleName, LastName, Suffix, Birthdate, ShortAddress, PurokID) values (@ID, @FirstName, @MiddleName, @LastName, @Birthdate, @ShortAddress, @PurokID)"
                    ElseIf _mode = "U" Then
                        sql = "Update citizen_tbl SET FirstName = @FirstName, MiddleName = @MiddleName, LastName = @LastName, Suffix = @Suffix, Birthdate = @Birthdate, ShortAddress =  @ShortAddress, status = @status, PurokID = @PurokID where ID = @ID"
                    End If

                    Using cmd As New MySqlCommand
                        cmd.Parameters.AddWithValue("@ID", requestID)
                        cmd.Parameters.AddWithValue("@FirstName", txt_fName.Value.Trim)
                        cmd.Parameters.AddWithValue("@MiddleName", txt_mName.Value.Trim)
                        cmd.Parameters.AddWithValue("@LastName", txt_lName.Value.Trim)
                        cmd.Parameters.AddWithValue("@Suffix", dl_suffix.Value.Trim)
                        cmd.Parameters.AddWithValue("@Birthdate", Date.Parse(txt_bdate.Value))
                        cmd.Parameters.AddWithValue("@ShortAddress", txt_address.Value.Trim)
                        cmd.Parameters.AddWithValue("@status", dl_status.Value)
                        cmd.Parameters.AddWithValue("@PurokID", dl_psID.Value)

                        cmd.Connection = con
                        cmd.CommandText = sql
                        cmd.ExecuteNonQuery()
                    End Using
                    con.Close()
                    con.Dispose()

                    Session("action") = _mode
                    Response.Redirect(Request.RawUrl)
                End Using
            Catch ex As Exception
                ClientScript.RegisterStartupScript(Me.[GetType](), "systemMsg", "systemMsg(0, '" & ex.Message.Replace("'", "|") & "');", True)
            End Try
        End If
    End Sub

    Protected Sub Purok()

        With clsMaster
            dl_psID.Items.Clear()
            For Each row In .dynamicQuery("Select ID, PurokName From ps_tbl Where Status = 1 Order By PurokName").AsEnumerable
                dl_psID.Items.Add(New ListItem(row.Item("PurokName").ToString, row.Item("ID").ToString))
            Next
            dl_psID.Items.Insert(0, New ListItem("Please select 1", ""))


        End With

    End Sub
    Protected Sub Barangay()

        With clsMaster
            dl_brgyFilter.Items.Clear()
            For Each row In .dynamicQuery("Select ID, BarangayName From brgy_tbl Where Status = 1 Order By BarangayName").AsEnumerable
                dl_brgyFilter.Items.Add(New ListItem(row.Item("BarangayName").ToString, row.Item("ID").ToString))
            Next
            dl_brgyFilter.Items.Insert(0, New ListItem("All", ""))


        End With

    End Sub

    <WebMethod>
    Public Shared Function validateEntry(mode As String, entry As String) As Boolean
        Dim clsMaster2 As New cls_master

        With clsMaster2
            If .dynamicQuery("Select ID from citizen_tbl where " + mode + " = '" & entry.Trim & "'").Rows.Count > 0 Then
                Return True
            Else
                Return False
            End If
        End With

    End Function

    <WebMethod>
    Public Shared Function LoadList(status As String, brgyFilter As String, mode As Integer, wildcard As String) As String
        Dim clsMaster2 As New cls_master
        Dim htmlStr As New StringBuilder
        Dim last10 As String = Nothing

        With clsMaster2
            Dim filterList As New List(Of String)

            If mode = 1 Then
                filterList.Add("citizen_tbl.Status = '" & status & "'")

                If brgyFilter = "Last10" Then
                    last10 = " ORDER BY citizen_tbl.ID DESC LIMIT 10 "
                ElseIf Not brgyFilter = "" Then
                    filterList.Add("CASE WHEN brgy_tbl.ID IS NULL THEN ps_tbl.bID ELSE brgy_tbl.ID END = '" & brgyFilter & "'")
                End If
            ElseIf mode = 0 Then
                filterList.Add("Concat_Ws(' ', citizen_tbl.FirstName, citizen_tbl.MiddleName, citizen_tbl.LastName, citizen_tbl.Suffix) Like '%" & wildcard & "%'")
            End If

            Dim sqlQuery As String = "Select citizen_tbl.ID, Concat_Ws(' ', citizen_tbl.FirstName, citizen_tbl.MiddleName, citizen_tbl.LastName, citizen_tbl.Suffix) As FullName, Date_Format(citizen_tbl.Birthdate, '%Y-%d-%m') As Birthday, floor(TimestampDiff(YEAR, citizen_tbl.Birthdate, Now())) As Age, " &
             " Case When citizen_tbl.Sex = '1' Then 'Male' Else 'Female' End As Sex, Concat_Ws(' ', citizen_tbl.ShortAddress, ps_tbl.PurokName, ',', brgy_tbl.BarangayName) As Address, Case When citizen_tbl.Status = '1' Then 'Active' Else 'Inactive' End As Status From citizen_tbl Inner Join " &
             " ps_tbl On ps_tbl.ID = citizen_tbl.PurokID Inner Join brgy_tbl On brgy_tbl.ID = ps_tbl.bID"

            ' Check if filterList is not empty before adding the WHERE clause
            If filterList.Count > 0 Then
                sqlQuery &= " WHERE " & String.Join(" And ", filterList)
            End If

            ' Add the last10 clause after the WHERE clause
            sqlQuery &= last10

            ' Order the results by ID in descending order
            sqlQuery &= " ORDER BY citizen_tbl.ID DESC"

            For Each row In .dynamicQuery(sqlQuery).AsEnumerable
                htmlStr.Append("<tr>")
                htmlStr.Append("<td style=""width: 10%""><a href=""Citizen.aspx?ID=" & .Encrypt(row.Item("ID").ToString) & """ Class=""blue""><i Class=""fa fa-search""></i> &nbsp" & row.Item("ID").ToString.ToUpper & "</a></td>")
                htmlStr.Append("<td style=""width: 20%"">" & row.Item("FullName").ToString & "</td>")
                htmlStr.Append("<td style=""width: 5%"">" & row.Item("Age").ToString & "</td>")
                htmlStr.Append("<td style=""width: 10%"">" & row.Item("Birthday").ToString & "</td>")
                htmlStr.Append("<td style=""width: 5%"">" & row.Item("Sex").ToString & "</td>")
                htmlStr.Append("<td style=""width: 20%"">" & row.Item("Address").ToString.ToUpper & "</td>")
                htmlStr.Append("<td style=""width: 8%"" class=""text-center"">" & row.Item("Status").ToString & "</td>")
                htmlStr.Append("</tr>")
            Next
        End With

        Return htmlStr.ToString()
    End Function

End Class