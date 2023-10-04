Imports System.Data
Imports MySql.Data.MySqlClient
Imports System.Configuration
Imports Newtonsoft.Json
Imports System.Data.SqlClient
Imports System.Web.Services
Imports System.Web.Script.Serialization
Imports System.Web.UI.WebControls
Imports Mysqlx.XDevAPI.Relational

Partial Class Home
    Inherits System.Web.UI.Page
    Dim ConnString As String
    Dim clsMaster As New cls_master
    Dim requestID As String

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        ConnString = ConfigurationManager.ConnectionStrings("conn1").ConnectionString
        LoadApplicantCounts()
        AppLoadList()
        If Not Page.IsPostBack Then

        End If
    End Sub

    Private Sub LoadApplicantCounts()
        ' Define your connection string from the Web.config file
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("conn1").ConnectionString

        ' Create a SQL connection
        Using connection As New MySqlConnection(connectionString)
            ' Define your SQL query with CASE WHEN statement
            Dim query As String = "SELECT " &
               "COUNT(*) As TotalApplicants," &
               "SUM(CASE WHEN lho_test_tbl.ApplicationID = 3 THEN 1 ELSE 0 END) As ApprovedApplicants," &
               "SUM(CASE WHEN lho_test_tbl.ApplicationID IN (1,2) THEN 1 ELSE 0 END) As OnProcessApplicants," &
               "SUM(CASE WHEN lho_test_tbl.ApplicationID = 4 THEN 1 ELSE 0 END) As DeclinedApplicants " &
               " From lho_test_tbl"

            ' Create a SQL command
            Using command As New MySqlCommand(query, connection)
                connection.Open()

                ' Execute the SQL command and fetch the counts
                Using reader As MySqlDataReader = command.ExecuteReader()
                    If reader.Read() Then
                        ' Bind the counts to your card elements
                        lblTotalApplicants.InnerText = reader("TotalApplicants").ToString()
                        lblApprovedApplicants.InnerText = reader("ApprovedApplicants").ToString()
                        lblOnProcessApplicants.InnerText = reader("OnProcessApplicants").ToString()
                        lblDeclinedApplicants.InnerText = reader("DeclinedApplicants").ToString()
                    End If
                End Using
            End Using
        End Using
    End Sub

    Private Sub AppLoadList()
        Dim htmlStr As New StringBuilder


        With clsMaster
            For Each row In .dynamicQuery("SELECT lho_test_tbl.ID, lho_test_tbl.refID, Concat_Ws(' ', cps_test_tbl.fName, cps_test_tbl.mName, cps_test_tbl.lName, cps_test_tbl.suffix)As FullName, " &
                                  " Concat_Ws('', cps_test_tbl.shortAdd, ', ', ps_tbl.PurokName) As FullAddress, " &
                                  " applicationstatus_tbl.StatusName As ApplicationStatus, category_tbl.CategoryName As Category, date_format (lho_test_tbl.CreatedDate, '%M %d, %Y') As ApplicationDate " &
                                  " FROM lho_test_tbl " &
                                  " INNER JOIN cps_test_tbl ON cps_test_tbl.refID = lho_test_tbl.cpsID " &
                                  " INNER JOIN ps_tbl ON ps_tbl.ID = cps_test_tbl.psID " &
                                  " INNER JOIN applicationstatus_tbl ON applicationstatus_tbl.ID = lho_test_tbl.ApplicationID " &
                                  " INNER JOIN category_tbl ON category_tbl.ID = lho_test_tbl.category").AsEnumerable

                htmlStr.Append("<tr>")
                htmlStr.Append("<td style='width: 10%'>" + row.Item("refID").ToString.ToUpper + "</a></td>")
                htmlStr.Append("<td style=""width: 30%"">" + row.Item("FullName").ToString + "</td>")
                htmlStr.Append("<td style=""width: 40%"">" + row.Item("FullAddress").ToString + "</td>")
                htmlStr.Append("<td>" + row.Item("ApplicationStatus").ToString + "</td>")
                htmlStr.Append("<td>" + row.Item("Category").ToString + "</td>")
                htmlStr.Append("<td style=""width: 15%"" class=""text-center"">" + row.Item("ApplicationDate").ToString + "</td>")
                htmlStr.Append("</tr>")
            Next
        End With
        ph_list.Controls.Add(New Literal() With {.Text = htmlStr.ToString()})
    End Sub


    <WebMethod>
    Public Shared Function GetDataForChart(startDate As Date, endDate As Date) As String
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("conn1").ConnectionString
        Dim connection As New MySqlConnection(connectionString)
        Dim chartData As New Dictionary(Of String, Integer)()

        Try
            connection.Open()
            Dim query As String = "SELECT CreatedDate, " &
                    "SUM(CASE WHEN ApplicationID = 3 THEN 1 ELSE 0 END) AS approved_applicants, " &
                    "SUM(CASE WHEN ApplicationID IN (1,2) THEN 1 ELSE 0 END) AS pending_applicants, " &
                    "SUM(CASE WHEN ApplicationID = 4 THEN 1 ELSE 0 END) AS declined_applicants " &
                    "FROM lho_test_tbl " &
                    "WHERE CreatedDate >= @startDate AND CreatedDate <= @endDate " &
                    "GROUP BY CreatedDate " &
                    "ORDER BY CreatedDate"

            Dim cmd As New MySqlCommand(query, connection)
            cmd.Parameters.AddWithValue("@startDate", startDate)
            cmd.Parameters.AddWithValue("@endDate", endDate)

            Dim reader As MySqlDataReader = cmd.ExecuteReader()

            While reader.Read()
                Dim dateValue As Date = Convert.ToDateTime(reader("CreatedDate"))
                Dim approvedApplicants As Integer = Convert.ToInt32(reader("approved_applicants"))
                Dim pendingApplicants As Integer = Convert.ToInt32(reader("pending_applicants"))
                Dim declinedApplicants As Integer = Convert.ToInt32(reader("declined_applicants"))

                ' Assuming your date format is "MM/DD/YYYY"
                Dim dateString As String = dateValue.ToString("MM/dd/yyyy")

                ' Add data to the dictionary
                chartData(dateString + "_Approved Applicants") = approvedApplicants
                chartData(dateString + "_On-Process Applicants") = pendingApplicants
                chartData(dateString + "_Declined Applicants") = declinedApplicants
            End While

            reader.Close()

            ' Serialize chartData to JSON
            Dim serializer As New JavaScriptSerializer()
            Dim jsonData As String = serializer.Serialize(chartData)

            Return jsonData

        Catch ex As Exception
            ' Handle any exceptions here and return an error message in JSON format
            Dim errorResponse As New Dictionary(Of String, String) From {{"error", ex.Message}}
            Dim serializer As New JavaScriptSerializer()
            Dim errorJson As String = serializer.Serialize(errorResponse)

            Return errorJson
        Finally
            connection.Close()
        End Try

    End Function
End Class