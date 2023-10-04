Imports System.Data
Imports System.Web.Services
Imports MySql.Data.MySqlClient
Imports System.IO

Partial Class Applicant
    Inherits System.Web.UI.Page
    Dim ConnString As String
    Dim clsMaster As New cls_master
    Dim requestID As String

    Protected Sub Applicant_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Not Request.QueryString("ID") = Nothing Then
            requestID = clsMaster.Decrypt(Request.QueryString("ID").Replace(" ", "+"))
        End If

        ConnString = ConfigurationManager.ConnectionStrings("conn1").ConnectionString
    End Sub

    Private Sub Applicant_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Try
                Purok()
                category()
                profApplicantStatus()
                occupancyStatus()
                If Not requestID = Nothing Then
                    showDetails()
                End If
            Catch ex As Exception
                ClientScript.RegisterStartupScript(Me.[GetType](), "systemMsg", "systemMsg(0, '" & ex.Message.Replace("'", "|") & "');", True)
            End Try
        End If
    End Sub

    Protected Sub Save_Click()
        Try
            Using con As New MySqlConnection(ConnString)
                con.Open()
                Using trans As MySqlTransaction = con.BeginTransaction()
                    Try
                        ' Update cps_test_tbl
                        Dim cpsSql As String = "UPDATE cps_test_tbl SET civilStatus = @civilStatus, shortAdd = @shortAdd, " &
                                           "psID = @psID, vin = @vin, contactNo = @contactNo, occStatus = @occStatus WHERE refID = @cpsID;"

                        Using cpsCmd As New MySqlCommand(cpsSql, con)
                            cpsCmd.Transaction = trans
                            cpsCmd.Parameters.AddWithValue("@cpsID", hdnCPSrefID.Value.Trim)
                            cpsCmd.Parameters.AddWithValue("@civilStatus", dl_AppCivilStatus.Value.Trim)
                            cpsCmd.Parameters.AddWithValue("@shortAdd", txt_AppAddress.Value)
                            cpsCmd.Parameters.AddWithValue("@vin", txt_AppVoters.Value)
                            cpsCmd.Parameters.AddWithValue("@psID", dl_ApppsID.Value.Trim)
                            cpsCmd.Parameters.AddWithValue("@contactNo", txt_AppContact.Value.Trim)
                            cpsCmd.Parameters.AddWithValue("@occStatus", dl_AppoccupancyStatus.Value.Trim)
                            cpsCmd.ExecuteNonQuery()
                        End Using

                        ' Update lho_test_tbl
                        Dim lhoSql As String = "UPDATE lho_test_tbl SET category = @category, applicationID = @applicationID, remarks = @remarks WHERE refID = @refID;"

                        Using lhoCmd As New MySqlCommand(lhoSql, con)
                            lhoCmd.Transaction = trans
                            lhoCmd.Parameters.AddWithValue("@refID", hdnLHOrefID.Value.Trim)
                            lhoCmd.Parameters.AddWithValue("@category", dl_AppCategory.Value)
                            lhoCmd.Parameters.AddWithValue("@remarks", txt_AppRemarks.Value)
                            lhoCmd.Parameters.AddWithValue("@applicationID", dl_profileApplicantStatus.Value)
                            lhoCmd.ExecuteNonQuery()
                        End Using

                        ' Commit the transaction
                        trans.Commit()

                        Session("action") = "U"
                        Response.Redirect("Applicant.aspx")
                    Catch ex As Exception
                        ' Rollback the transaction in case of an error
                        trans.Rollback()
                        Throw ex ' You can handle the exception or display an error message as needed
                    End Try
                End Using
            End Using
        Catch ex As Exception
            ClientScript.RegisterStartupScript(Me.[GetType](), "systemMsg", "systemMsg(0, '" & ex.Message.Replace("'", "|") & "');", True)
        End Try
    End Sub

    Private Sub showDetails()
        Dim htmlStr As New StringBuilder()

        With clsMaster
            ' First, retrieve the applicant's details
            Dim foundRow As DataRow() = .dynamicQuery("SELECT applicationstatus_tbl.StatusName As ApplicationStatus, lho_test_tbl.refID, lho_test_tbl.cpsID, lho_test_tbl.category, fName, mName, lName, suffix, bDate, civilStatus, shortAdd, psID, vin, contactNo, famID, lho_test_tbl.remarks as remarks, lho_test_tbl.applicationID as applicantID, occStatus FROM cps_test_tbl Inner Join occupancy_tbl On occupancy_tbl.ID = cps_test_tbl.occStatus Inner Join lho_test_tbl On lho_test_tbl.cpsID = cps_test_tbl.refID Inner Join applicationstatus_tbl On applicationstatus_tbl.ID = lho_test_tbl.applicationID WHERE lho_test_tbl.ID = '" & requestID & "'").Select


            If foundRow.Length > 0 Then
                btn_add.Visible = False
                ref_Header.InnerText = "Ref ID: " & foundRow(0).Item("refID").ToString
                hdnLHOrefID.Value = foundRow(0).Item("refID").ToString
                hdnCPSrefID.Value = foundRow(0).Item("cpsID").ToString

                ' Determine the badge class based on ApplicationStatus
                Dim applicationStatus As String = foundRow(0).Item("ApplicationStatus").ToString
                Dim badgeClass As String = ""

                Select Case applicationStatus
                    Case "For Requirements"
                        badgeClass = "mx-3 badge badge-warning"
                    Case "For Approval"
                        badgeClass = "mx-3 badge badge-info"
                    Case "Approved"
                        badgeClass = "mx-3 badge badge-success"
                    Case "Disapproved"
                        badgeClass = "mx-3 badge badge-danger"
                    Case Else
                        badgeClass = "mx-3 badge badge-secondary"
                End Select

                ' Set the badge class to the <h2> element
                status_header.Attributes("class") = badgeClass
                status_header.InnerText = applicationStatus

                div_list.Attributes("class") = "x_panel hide"
                div_details.Attributes("class") = "x_panel"
                li_timeline.Visible = True

                dl_AppCategory.Value = foundRow(0).Item("category").ToString
                txt_AppfName.Value = foundRow(0).Item("fName").ToString
                txt_AppmName.Value = foundRow(0).Item("mName").ToString
                txt_ApplName.Value = foundRow(0).Item("lName").ToString
                dl_AppSuffix.Value = foundRow(0).Item("suffix").ToString
                dl_AppCivilStatus.Value = foundRow(0).Item("civilStatus").ToString
                AppbDay.Value = Format(foundRow(0).Item("bDate"), "MM/dd/yyyy")
                txt_AppContact.Value = foundRow(0).Item("contactNo").ToString
                txt_AppVoters.Value = foundRow(0).Item("vin").ToString
                txt_AppAddress.Value = foundRow(0).Item("shortAdd").ToString
                dl_ApppsID.Value = foundRow(0).Item("psID").ToString
                txt_AppRemarks.Value = foundRow(0).Item("remarks").ToString
                dl_profileApplicantStatus.Value = foundRow(0).Item("applicantID").ToString
                dl_AppoccupancyStatus.Value = foundRow(0).Item("occStatus").ToString

                ' Next, retrieve the family details for the same applicant
                Dim familySqlQuery As String = "SELECT CONCAT(COALESCE(fName, ''), CASE WHEN fName IS NOT NULL AND (mName IS NOT NULL OR lName OR suffix IS NOT NULL) THEN ' ' ELSE '' END, COALESCE(mName, ''), CASE WHEN mName IS NOT NULL AND (lName IS NOT NULL OR suffix IS NOT NULL) THEN ' ' ELSE '' END, COALESCE(lName, ''), CASE WHEN lName IS NOT NULL AND suffix IS NOT NULL THEN ' ' ELSE '' END, COALESCE(suffix, '')) AS FullName, date_format (bDate, '%M %d, %Y') As birthDate, occupancy_tbl.occupancyName, FORMAT(famIncome, 2) AS familyIncome, CASE WHEN hhID = 1 THEN 'Yes' ELSE 'No' END AS Household  FROM cps_test_tbl Inner Join occupancy_tbl On occupancy_tbl.ID = cps_test_tbl.occStatus WHERE famID = '" & foundRow(0).Item("famID").ToString() & "'"

                Dim counter As Integer = 1

                For Each row In .dynamicQuery(familySqlQuery).AsEnumerable
                    htmlStr.Append("<tr>")
                    htmlStr.Append("<td style=""width: 1%"" class=""text-center"">" + counter.ToString() + "</td>")
                    htmlStr.Append("<td style=""width: 15%"">" + row.Item("FullName").ToString + "</td>")
                    htmlStr.Append("<td style=""width: 5%"" class=""text-center"">" + row.Item("birthDate").ToString + "</td>")
                    htmlStr.Append("<td style=""width: 5%"" class=""text-center"">" + row.Item("occupancyName").ToString + "</td>")
                    htmlStr.Append("<td style=""width: 8%"" class=""text-center"">" + row.Item("familyIncome").ToString + "</td>")

                    If row.Item("Household").ToString = "Yes" Then
                        htmlStr.Append("<td style=""width: 6%"" class=""text-center""><span class=""badge bg-success"">Yes</span></td>")
                    ElseIf row.Item("Household").ToString = "No" Then
                        htmlStr.Append("<td style=""width: 6%"" class=""text-center""><span class=""badge bg-danger"">No</span></td>")
                    Else
                        htmlStr.Append("<td style=""width: 6%"" class=""text-center"">" + row.Item("Household").ToString + "</td>")
                    End If

                    htmlStr.Append("</tr>")
                    counter += 1
                Next

                ' Assuming you have generated the family details HTML in htmlStr
                Dim familyDetailsHtml As String = htmlStr.ToString()

                ' Inject the HTML into the table body using jQuery
                ScriptManager.RegisterStartupScript(Me, Me.[GetType](), "InjectFamilyData", "$('#familyMembersTableBody').html('" & familyDetailsHtml & "');", True)
            Else
                ClientScript.RegisterStartupScript(Me.[GetType](), "systemMsg", "systemMsg(0, 'Selected record did not exist.');", True)
                Response.Redirect(Master.activePage)
            End If
        End With
    End Sub

    Protected Sub Purok()

        With clsMaster
            dl_ApppsID.Items.Clear()
            For Each row In .dynamicQuery("Select ID, PurokName From ps_tbl Where Status = 1 Order By PurokName").AsEnumerable
                dl_ApppsID.Items.Add(New ListItem(row.Item("PurokName").ToString, row.Item("ID").ToString))
            Next
            dl_ApppsID.Items.Insert(0, New ListItem("Please select 1", ""))


        End With

    End Sub

    Protected Sub category()

        With clsMaster
            dl_AppCategory.Items.Clear()
            For Each row In .dynamicQuery("Select ID, CategoryName From category_tbl Where Status = 1 Order By CategoryName").AsEnumerable
                dl_AppCategory.Items.Add(New ListItem(row.Item("CategoryName").ToString, row.Item("ID").ToString))
            Next
            dl_AppCategory.Items.Insert(0, New ListItem("Select Category", ""))
        End With

    End Sub

    Protected Sub profApplicantStatus()

        With clsMaster
            dl_profileApplicantStatus.Items.Clear()
            For Each row In .dynamicQuery("Select ID, StatusName From applicationstatus_tbl Where Status = 1 Order By StatusName").AsEnumerable
                dl_profileApplicantStatus.Items.Add(New ListItem(row.Item("StatusName").ToString, row.Item("ID").ToString))
            Next
            dl_profileApplicantStatus.Items.Insert(0, New ListItem("Application Status", ""))


        End With

    End Sub

    Protected Sub occupancyStatus()

        With clsMaster
            dl_AppoccupancyStatus.Items.Clear()
            For Each row In .dynamicQuery("Select ID, occupancyName From occupancy_tbl Where Status = 1 Order By occupancyName").AsEnumerable
                dl_AppoccupancyStatus.Items.Add(New ListItem(row.Item("occupancyName").ToString, row.Item("ID").ToString))
            Next
            dl_AppoccupancyStatus.Items.Insert(0, New ListItem("Occupancy Status", ""))


        End With

    End Sub

    <WebMethod>
    Public Shared Function GetCategories() As List(Of ListItem)
        Dim clsMaster2 As New cls_master
        Dim categories As New List(Of ListItem)()

        ' Your code to fetch categories
        For Each row In clsMaster2.dynamicQuery("Select ID, CategoryName From category_tbl Where Status = 1 Order By CategoryName").AsEnumerable
            categories.Add(New ListItem(row.Item("CategoryName").ToString, row.Item("ID").ToString))
        Next
        categories.Insert(0, New ListItem("All", ""))

        Return categories
    End Function

    <WebMethod>
    Public Shared Function GetApplicantStatus() As List(Of ListItem)
        Dim clsMaster2 As New cls_master
        Dim applicantStatus As New List(Of ListItem)()

        ' Your code to fetch categories
        For Each row In clsMaster2.dynamicQuery("Select ID, StatusName From applicationstatus_tbl Where Status = 1 Order By StatusName").AsEnumerable
            applicantStatus.Add(New ListItem(row.Item("StatusName").ToString, row.Item("ID").ToString))
        Next
        applicantStatus.Insert(0, New ListItem("All", ""))

        Return applicantStatus
    End Function

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
    Public Shared Function filterDataTable(_from As String, _to As String, status As String, category As String) As String
        Dim htmlStr As New StringBuilder
        Dim clsMaster As New cls_master

        With clsMaster
            ' Your SQL query with the original date parameters _from and _to
            Dim sqlQuery As String = "SELECT lho_test_tbl.ID, lho_test_tbl.refID, Concat_Ws(' ', cps_test_tbl.fName, cps_test_tbl.mName, cps_test_tbl.lName, cps_test_tbl.suffix)As FullName, " &
                                  " Concat_Ws('', cps_test_tbl.shortAdd, ', ', ps_tbl.PurokName) As FullAddress, " &
                                  " applicationstatus_tbl.StatusName As ApplicationStatus, category_tbl.CategoryName As Category, date_format (lho_test_tbl.CreatedDate, '%M %d, %Y') As ApplicationDate " &
                                  " FROM lho_test_tbl " &
                                  " INNER JOIN cps_test_tbl ON cps_test_tbl.refID = lho_test_tbl.cpsID " &
                                  " INNER JOIN ps_tbl ON ps_tbl.ID = cps_test_tbl.psID " &
                                  " INNER JOIN applicationstatus_tbl ON applicationstatus_tbl.ID = lho_test_tbl.ApplicationID " &
                                  " INNER JOIN category_tbl ON category_tbl.ID = lho_test_tbl.category " &
                                  " WHERE lho_test_tbl.CreatedDate BETWEEN '" & _from & "' AND '" & _to & "'"

            ' Check if "" is selected for status and adjust the SQL query accordingly
            If status <> "" Then
                sqlQuery &= " AND lho_test_tbl.ApplicationID = '" & status & "'"
            End If

            ' Check if "" is selected for category and adjust the SQL query accordingly
            If category <> "" Then
                sqlQuery &= " AND lho_test_tbl.Category = '" & category & "'"
            End If

            For Each row In .dynamicQuery(sqlQuery).AsEnumerable
                htmlStr.Append("<tr>")
                htmlStr.Append("<td style='width: 10%'><a href=""Applicant.aspx?ID=" + .Encrypt(row.Item("ID").ToString) + """ Class=""green""><i class=""fa fa-search bigger-130""></i> &nbsp" + row.Item("refID").ToString.ToUpper + "</a></td>")
                htmlStr.Append("<td style=""width: 20%"">" + row.Item("FullName").ToString + "</td>")
                htmlStr.Append("<td style=""width: 30%"">" + row.Item("FullAddress").ToString + "</td>")
                htmlStr.Append("<td>" + row.Item("Category").ToString + "</td>")
                htmlStr.Append("<td>" + row.Item("ApplicationStatus").ToString + "</td>")
                htmlStr.Append("<td style=""width: 15%"" class=""text-center"">" + row.Item("ApplicationDate").ToString + "</td>")
                htmlStr.Append("</tr>")
            Next
        End With

        Return htmlStr.ToString
    End Function



    '<WebMethod>
    'Public Shared Function filterDataTable(_from As String, _to As String, status As String, category As String) As String
    '    Dim htmlStr As New StringBuilder
    '    Dim clsMaster As New cls_master

    '    With clsMaster
    '        ' Your SQL query with the original date parameters _from and _to
    '        Dim sqlQuery As String = "SELECT citizen_tbl.ID, Concat_Ws(' ', citizen_tbl.FirstName, citizen_tbl.MiddleName, citizen_tbl.LastName, citizen_tbl.Suffix) As FullName, " &
    '                                  " Concat_Ws(' ', citizen_tbl.ShortAddress, ps_tbl.PurokName, ',', brgy_tbl.BarangayName) As Address, " &
    '                                  " applicationstatus_tbl.StatusName As ApplicationStatus, category_tbl.CategoryName As Category, date_format (citizen_tbl.CreatedDate, '%M %d, %Y') As ApplicationDate " &
    '                                  " FROM citizen_tbl " &
    '                                  " INNER JOIN ps_tbl ON ps_tbl.ID = citizen_tbl.PurokID " &
    '                                  " INNER JOIN brgy_tbl ON brgy_tbl.ID = ps_tbl.bID " &
    '                                  " INNER JOIN category_tbl ON category_tbl.ID = citizen_tbl.CategoryID " &
    '                                  " INNER JOIN applicationstatus_tbl ON applicationstatus_tbl.ID = citizen_tbl.ApplicationID " &
    '                                  " WHERE citizen_tbl.ApplicationDate BETWEEN '" & _from & "' AND '" & _to & "'"

    '        ' Check if "" is selected for status and adjust the SQL query accordingly
    '        If status <> "" Then
    '            sqlQuery &= " AND citizen_tbl.ApplicationID = '" & status & "'"
    '        End If

    '        ' Check if "" is selected for category and adjust the SQL query accordingly
    '        If category <> "" Then
    '            sqlQuery &= " AND citizen_tbl.CategoryID = '" & category & "'"
    '        End If

    '        For Each row In .dynamicQuery(sqlQuery).AsEnumerable
    '            htmlStr.Append("<tr>")
    '            htmlStr.Append("<td style='width: 10%'><a href=""Applicant.aspx?ID=" + .Encrypt(row.Item("ID").ToString) + """ Class=""green""><i class=""fa fa-search bigger-130""></i> &nbsp" + row.Item("ID").ToString.ToUpper + "</a></td>")
    '            htmlStr.Append("<td style=""width: 20%"">" + row.Item("FullName").ToString + "</td>")
    '            htmlStr.Append("<td style=""width: 30%"">" + row.Item("Address").ToString + "</td>")
    '            htmlStr.Append("<td>" + row.Item("Category").ToString + "</td>")
    '            htmlStr.Append("<td>" + row.Item("ApplicationStatus").ToString + "</td>")
    '            htmlStr.Append("<td style=""width: 15%"" class=""text-center"">" + row.Item("ApplicationDate").ToString + "</td>")
    '            htmlStr.Append("</tr>")
    '        Next
    '    End With

    '    Return htmlStr.ToString
    'End Function

End Class
