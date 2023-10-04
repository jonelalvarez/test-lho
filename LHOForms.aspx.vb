Imports System.Data
Imports System.Web.Services
Imports MySql.Data.MySqlClient

Partial Class LHOForms
    Inherits System.Web.UI.Page
    Dim ConnString As String
    Dim clsMaster As New cls_master
    Dim requestID As String


    Private Sub Forms_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Try
                Purok()
                category()
                occupancyStatus()
            Catch ex As Exception
                ClientScript.RegisterStartupScript(Me.[GetType](), "systemMsg", "systemMsg(0, '" & ex.Message.Replace("'", "|") & "');", True)
            End Try
        End If
    End Sub

    Protected Sub Forms_Init(sender As Object, e As EventArgs) Handles Me.Init
        ConnString = ConfigurationManager.ConnectionStrings("conn1").ConnectionString
    End Sub


    Protected Sub Save_Click()
        Try
            Using con As New MySqlConnection(ConnString)
                con.Open()
                Dim sql As String = Nothing

                If Not requestID = Nothing Then
                    ' Update an existing record in cps_test_tbl
                    sql = "UPDATE cps_test_tbl SET civilStatus = @civilStatus, shortAdd = @shortAdd WHERE refID = @cpsID;"
                Else
                    ' Create a new record in lho_test_tbl
                    sql = "INSERT INTO lho_test_tbl (cpsID, category, remarks, applicationID, CreatedDate) VALUES (@cpsID, @category, @remarks, 1, NOW());"

                    ' Also update cps_test_tbl
                    sql += "UPDATE cps_test_tbl SET civilStatus = @civilStatus, shortAdd = @shortAdd, psID=@psID, vin = @vin, contactNo = @contactNo, occStatus = @occStatus WHERE refID = @cpsID;"
                End If

                Using cmd As New MySqlCommand
                    cmd.Parameters.AddWithValue("@cpsID", hdnRefID.Value.Trim)
                    cmd.Parameters.AddWithValue("@category", dl_category.Value)
                    cmd.Parameters.AddWithValue("@remarks", txt_remarks.Value)
                    cmd.Parameters.AddWithValue("@civilStatus", dl_civilStatus.Value.Trim)
                    cmd.Parameters.AddWithValue("@shortAdd", txt_address.Value)
                    cmd.Parameters.AddWithValue("@vin", txt_Voters.Value)
                    cmd.Parameters.AddWithValue("@psID", dl_psID.Value.Trim)
                    cmd.Parameters.AddWithValue("@contactNo", txt_contactNum.Value.Trim)
                    cmd.Parameters.AddWithValue("@occStatus", dl_occupancyStatus.Value.Trim)


                    cmd.Connection = con
                    cmd.CommandText = sql
                    cmd.ExecuteNonQuery()
                End Using

                con.Close()
            End Using

            Session("action") = "C"
            Response.Redirect("Applicant.aspx")
        Catch ex As Exception
            ClientScript.RegisterStartupScript(Me.[GetType](), "systemMsg", "systemMsg(0, '" & ex.Message.Replace("'", "|") & "');", True)
        End Try
    End Sub

    Protected Sub Purok()

        With clsMaster
            dl_psID.Items.Clear()
            For Each row In .dynamicQuery("Select ID, PurokName From ps_tbl Where Status = 1 Order By PurokName").AsEnumerable
                dl_psID.Items.Add(New ListItem(row.Item("PurokName").ToString, row.Item("ID").ToString))
            Next
            dl_psID.Items.Insert(0, New ListItem("Purok/Sitio/Subdivision", ""))
        End With

    End Sub

    Protected Sub category()

        With clsMaster
            dl_category.Items.Clear()
            For Each row In .dynamicQuery("Select ID, CategoryName From category_tbl Where Status = 1 Order By CategoryName").AsEnumerable
                dl_category.Items.Add(New ListItem(row.Item("CategoryName").ToString, row.Item("ID").ToString))
            Next
            dl_category.Items.Insert(0, New ListItem("Select Category", ""))
        End With

    End Sub

    Protected Sub occupancyStatus()

        With clsMaster
            dl_occupancyStatus.Items.Clear()
            For Each row In .dynamicQuery("Select ID, occupancyName From occupancy_tbl Where Status = 1 Order By occupancyName").AsEnumerable
                dl_occupancyStatus.Items.Add(New ListItem(row.Item("occupancyName").ToString, row.Item("ID").ToString))
            Next
            dl_occupancyStatus.Items.Insert(0, New ListItem("Occupancy Status", ""))


        End With

    End Sub

    <WebMethod>
    Public Shared Function CheckUniqueness(cpsID As String) As Boolean
        ' Perform uniqueness check in the database for cpsID
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("conn1").ConnectionString

        Using connection As New MySqlConnection(connectionString)
            connection.Open()
            Dim query As String = "SELECT COUNT(*) FROM lho_test_tbl WHERE cpsID = @cpsID"
            Using command As New MySqlCommand(query, connection)
                command.Parameters.AddWithValue("@cpsID", cpsID)
                Dim count As Integer = Convert.ToInt32(command.ExecuteScalar())
                Return count = 0
            End Using
        End Using
    End Function

    <WebMethod>
    Public Shared Function populateApplicant(name As String) As List(Of ListItem)
        Dim dynamicList As New List(Of ListItem)()
        Dim clsMaster2 As New cls_master


        With clsMaster2

            'CPS
            Dim cpsQuery As String = "SELECT refID, CONCAT_WS(' ', fName, mName, lName, COALESCE(suffix, '')) AS name, 'cps_test_tbl' AS source FROM cps_test_tbl WHERE fName LIKE '%" & name & "%' OR mName LIKE '%" & name & "%' OR lName LIKE '%" & name & "%'"
            Dim cpsResult As DataTable = .dynamicQuery(cpsQuery)
            For Each cpsRow As DataRow In cpsResult.Rows
                dynamicList.Add(New ListItem() With {
                .Value = cpsRow("refID").ToString(),
                .Text = cpsRow("name").ToString()
                })
            Next
        End With

        Return dynamicList
    End Function

    <WebMethod>
    Public Shared Function autoFill(ID As String) As Dictionary(Of String, String)
        Dim dict As New Dictionary(Of String, String)
        Dim clsMaster As New cls_master
        Dim htmlStr As New StringBuilder
        Dim bDate As DateTime

        With clsMaster
            Dim sqlQuery As String = "SELECT fName, mName, lName, suffix, bDate, civilStatus, shortAdd, psID, vin, contactNo, famID, occStatus FROM cps_test_tbl WHERE cps_test_tbl.refID = '" & ID & "'"

            Dim cpsFoundRow As DataRow() = .dynamicQuery(sqlQuery).Select

            If cpsFoundRow.Length > 0 Then
                dict.Add("source", "cps_test_tbl")
                dict.Add("firstName", cpsFoundRow(0).Item("fName").ToString)
                dict.Add("middleName", cpsFoundRow(0).Item("mName").ToString)
                dict.Add("lastName", cpsFoundRow(0).Item("lName").ToString)
                dict.Add("suffix", cpsFoundRow(0).Item("suffix").ToString)
                dict.Add("civilStatus", cpsFoundRow(0).Item("civilStatus").ToString)
                dict.Add("vin", cpsFoundRow(0).Item("vin").ToString)
                dict.Add("psID", cpsFoundRow(0).Item("psID").ToString)
                dict.Add("shortAdd", cpsFoundRow(0).Item("shortAdd").ToString)
                dict.Add("contactNo", cpsFoundRow(0).Item("contactNo").ToString)
                dict.Add("occStatus", cpsFoundRow(0).Item("occStatus").ToString)

                If DateTime.TryParse(cpsFoundRow(0).Item("bDate").ToString(), bDate) Then
                    dict.Add("birthDay", bDate.Date)
                End If

                Dim famID As String = cpsFoundRow(0).Item("famID").ToString()

                Dim counter As Integer = 1

                Dim familySqlQuery As String = "SELECT CONCAT(COALESCE(fName, ''), CASE WHEN fName IS NOT NULL AND (mName IS NOT NULL OR lName IS NOT NULL OR suffix IS NOT NULL) THEN ' ' ELSE '' END, COALESCE(mName, ''), CASE WHEN mName IS NOT NULL AND (lName IS NOT NULL OR suffix IS NOT NULL) THEN ' ' ELSE '' END, COALESCE(lName, ''), CASE WHEN lName IS NOT NULL AND suffix IS NOT NULL THEN ' ' ELSE '' END, COALESCE(suffix, '')) AS FullName, date_format (bDate, '%M %d, %Y') As birthDate, occupancy_tbl.occupancyName, FORMAT(famIncome, 2) AS familyIncome,CASE WHEN hhID = 1 THEN 'Yes' ELSE 'No' END AS Household FROM cps_test_tbl Inner Join occupancy_tbl On occupancy_tbl.ID = cps_test_tbl.occStatus  WHERE famID = '" & famID & "'"


                For Each row In .dynamicQuery(familySqlQuery).AsEnumerable
                    htmlStr.Append("<tr>")
                    htmlStr.Append("<td style=""width: 1%"" class=""text-center"">" + counter.ToString() + "</td>")
                    htmlStr.Append("<td style=""width: 20%"">" + row.Item("FullName").ToString + "</td>")
                    htmlStr.Append("<td style=""width: 8%"" class=""text-center"">" + row.Item("birthDate").ToString + "</td>")
                    htmlStr.Append("<td style=""width: 8%"" class=""text-center"">" + row.Item("occupancyName").ToString + "</td>")
                    htmlStr.Append("<td style=""width: 8%"" class=""text-center"">" + row.Item("familyIncome").ToString + "</td>")

                    ' Add a badge based on the value of Household
                    If row.Item("Household").ToString = "Yes" Then
                        htmlStr.Append("<td style=""width: 6%"" class=""text-center""><span class=""badge bg-success"">Yes</span></td>")
                    ElseIf row.Item("Household").ToString = "No" Then
                        htmlStr.Append("<td style=""width: 6%"" class=""text-center""><span class=""badge bg-danger"">No</span></td>")
                    Else
                        ' Handle other cases if needed
                        htmlStr.Append("<td style=""width: 6%"" class=""text-center"">" + row.Item("Household").ToString + "</td>")
                    End If

                    htmlStr.Append("</tr>")
                    counter += 1
                Next


                ' Close the <tbody> tag
                htmlStr.Append("</tbody>")
                htmlStr.Append("</table>")

                dict.Add("familyMembersHTML", htmlStr.ToString())

            End If
        End With
        Return dict
    End Function


    'Protected Sub Save_Click()
    '    If Not requestID = Nothing Then
    '        CRUD("U")
    '    Else
    '        CRUD("C")
    '    End If
    'End Sub

    'Protected Sub CRUD(_mode As String)
    '    If IsPostBack Then
    '        Try
    '            Using con As New MySqlConnection(ConnString)
    '                con.Open()
    '                Dim sql As String = Nothing

    '                If _mode = "C" Then
    '                    sql = "INSERT INTO lho_test_tbl (ID, cpsID, category, remarks) " &
    '                            "VALUES (@ID, @cpsID, @category, @remarks) " &
    '                            "ON DUPLICATE KEY UPDATE " &
    '                            "cps_test_tbl.civilStatus = @civilStatus;"
    '                ElseIf _mode = "U" Then
    '                    ' Update an existing record in cps_test_tbl
    '                    sql = "UPDATE cps_test_tbl SET civilStatus = @civilStatus, WHERE ID = @ID"
    '                End If

    '                Using cmd As New MySqlCommand
    '                    cmd.Parameters.AddWithValue("@ID", requestID)
    '                    cmd.Parameters.AddWithValue("@category", dl_category.Value)
    '                    cmd.Parameters.AddWithValue("@cpsID", hdnRefID.Value)
    '                    cmd.Parameters.AddWithValue("@civilStatus", dl_civilStatus.Value.Trim)
    '                    cmd.Parameters.AddWithValue("@remarks", txt_remarks.Value.Trim)

    '                    cmd.Connection = con
    '                    cmd.CommandText = sql
    '                    cmd.ExecuteNonQuery()
    '                End Using
    '                con.Close()
    '                con.Dispose()

    '                Session("action") = _mode
    '                Response.Redirect(Request.RawUrl)
    '            End Using
    '        Catch ex As Exception
    '            ClientScript.RegisterStartupScript(Me.[GetType](), "systemMsg", "systemMsg(0, '" & ex.Message.Replace("'", "|") & "');", True)
    '        End Try
    '    End If
    'End Sub


    '<WebMethod>
    'Public Shared Function validateEntry(mode As String, entry As String) As Boolean
    '    Dim clsMaster2 As New cls_master

    '    With clsMaster2
    '        If .dynamicQuery("SELECT ID FROM lho_test_tbl WHERE " + mode + " = '" & entry.Trim & "'").Rows.Count > 0 Then
    '            Return True
    '        Else
    '            Return False
    '        End If
    '    End With

    'End Function

    '<WebMethod>
    'Public Shared Function CheckUniqueness(fName As String, mName As String, lName As String, suffix As String, bDate As String) As Boolean
    '    ' Perform uniqueness check in the database
    '    Dim connectionString As String = ConfigurationManager.ConnectionStrings("conn1").ConnectionString

    '    Using connection As New MySqlConnection(connectionString)
    '        connection.Open()
    '        Dim query As String = "SELECT COUNT(*) FROM lho_test_tbl WHERE " & "fName = @fName AND " & "mName = @mName AND " & "lName = @lName AND " & "suffix = @suffix AND " & "bDate = @formattedBDate"
    '        Using command As New MySqlCommand(query, connection)
    '            command.Parameters.AddWithValue("@fName", fName)
    '            command.Parameters.AddWithValue("@mName", mName)
    '            command.Parameters.AddWithValue("@lName", lName)
    '            command.Parameters.AddWithValue("@suffix", suffix)
    '            command.Parameters.AddWithValue("@formattedBDate", bDate)
    '            Dim count As Integer = Convert.ToInt32(command.ExecuteScalar())
    '            Return count = 0
    '        End Using
    '    End Using
    'End Function

    '<WebMethod>
    'Public Shared Function autoFill(ID As String) As Dictionary(Of String, String)
    '    Dim dict As New Dictionary(Of String, String)
    '    Dim clsMaster As New cls_master
    '    Dim bDate As DateTime

    '    With clsMaster
    '        Dim cpsFoundRow As DataRow() = .dynamicQuery("SELECT fName, mName, lName, suffix, bDate, civilStatus FROM cps_test_tbl WHERE cps_test_tbl.ID = '" & ID & "'").Select
    '        Dim lhoFoundRow As DataRow() = .dynamicQuery("SELECT category, fName, mName, lName, suffix, bDate, vid, civilStatus, contactNum, shortADD, psID, remarks FROM lho_test_tbl WHERE lho_test_tbl.ID = '" & ID & "'").Select

    '        If cpsFoundRow.Length > 0 Then
    '            dict.Add("source", "cps_test_tbl")
    '            dict.Add("firstName", cpsFoundRow(0).Item("fName").ToString)
    '            dict.Add("middleName", cpsFoundRow(0).Item("mName").ToString)
    '            dict.Add("lastName", cpsFoundRow(0).Item("lName").ToString)
    '            dict.Add("suffix", cpsFoundRow(0).Item("suffix").ToString)
    '            dict.Add("civilStatus", cpsFoundRow(0).Item("civilStatus").ToString)


    '            If DateTime.TryParse(cpsFoundRow(0).Item("bDate").ToString(), bDate) Then
    '                dict.Add("birthDay", bDate.Date)
    '            End If

    '        ElseIf lhoFoundRow.Length > 0 Then
    '            dict.Add("source", "lho_test_tbl")
    '            dict.Add("category", lhoFoundRow(0).Item("category").ToString)
    '            dict.Add("firstName", lhoFoundRow(0).Item("fName").ToString)
    '            dict.Add("middleName", lhoFoundRow(0).Item("mName").ToString)
    '            dict.Add("lastName", lhoFoundRow(0).Item("lName").ToString)
    '            dict.Add("suffix", lhoFoundRow(0).Item("suffix").ToString)
    '            dict.Add("vid", lhoFoundRow(0).Item("vid").ToString)
    '            dict.Add("civilStatus", lhoFoundRow(0).Item("civilStatus").ToString)
    '            dict.Add("contactNum", lhoFoundRow(0).Item("contactNum").ToString)
    '            dict.Add("shortADD", lhoFoundRow(0).Item("shortADD").ToString)
    '            dict.Add("psID", lhoFoundRow(0).Item("psID").ToString)
    '            dict.Add("remarks", lhoFoundRow(0).Item("remarks").ToString)

    '            If DateTime.TryParse(lhoFoundRow(0).Item("bDate").ToString(), bDate) Then
    '                dict.Add("birthDay", bDate.Date)
    '            End If
    '        End If
    '    End With

    '    Return dict
    'End Function

End Class