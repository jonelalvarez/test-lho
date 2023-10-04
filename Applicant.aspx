<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="Applicant.aspx.vb" Inherits="Applicant" %>

<%@ MasterType VirtualPath="~/MasterPage.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <input runat="server" id="txt_refID" hidden="hidden" />

    <div class="m-0 p-0">
        <div class="page-title pb-3 ">
            <div class="title_left">
                <h3 id="pageTitle"></h3>
            </div>
            <button type="button" class="btn btn-success inline pull-right pr-3 pl-3" style="margin: 5px 0 0 0" id="btn_add" runat="server" onclick="redirectToLHOForms();">
                <i class="fa fa-plus"></i>&nbsp;&nbsp;&nbsp;Add Applicant
            </button>
        </div>

        <div class="clearfix"></div>

        <div class="x_panel card card-header pt-3" id="div_list" runat="server">
            <div class="x_content">
                <div class="card card-body row m-0 p-2 pt-3">
                    <div class="row card-body pt-1">
                        <div class="col-md-4 col-xs-12">
                            <label>Application Date</label>
                            <div class="input-group">
                                <div class="input-group-prepend">
                                    <span class="input-group-text">
                                        <i class="far fa-calendar-alt"></i>
                                    </span>
                                </div>
                                <input type="text" class="form-control float-right" id="reportrange" onchange="generateTable();">
                            </div>
                        </div>
                        <div class="col-md-4 col-xs-12">
                            <label class="control-label">Category</label>
                            <select id="dl_category" class="form-control select2" onchange="generateTable();">
                                <% Dim categories As List(Of ListItem) = GetCategories()
                                    For Each category As ListItem In categories %>
                                <option value="<%= category.Value %>"><%= category.Text %></option>
                                <% Next %>
                            </select>
                        </div>
                        <div class="col-md-4 col-xs-12">
                            <label class="control-label">Application Status</label>
                            <select id="dl_applicationstatus" class="form-control select2" onchange="generateTable();">
                                <% Dim applicantStatus As List(Of ListItem) = GetApplicantStatus()
                                    For Each appStatus As ListItem In applicantStatus %>
                                <option value="<%= appStatus.Value %>"><%= appStatus.Text %></option>
                                <% Next %>
                            </select>
                        </div>

                    </div>
                </div>

                <div class="py-2"></div>

                <div class="table-responsive card card-body">
                    <table id="example1" class="table table-striped table-bordered dataTable">
                        <thead>
                            <tr>
                                <th>Ref ID</th>
                                <th>Full Name</th>
                                <th>Address</th>
                                <th>Category</th>
                                <th>Application Status</th>
                                <th>Application Date</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:PlaceHolder ID="ph_list" runat="server"></asp:PlaceHolder>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>

        <%--ID INFOMATION IN APPLICANT DASHBOARD--%>
        <div class="x_panel hide" id="div_details" runat="server">
            <div class="x_content">
                <div class="tabpanel">
                    <div class="card card-success card-outline card-tabs pt-2">
                        <ul id="myTab1" class="nav nav-tabs bar_tabs justify-content-end" role="tablist">
                            <li class="nav-item">
                                <a class="nav-link active" id="custom-tabs-three-profile-tab" data-toggle="tab" href="#tab_details" role="tab" aria-expanded="true">Details</a>
                            </li>
                            <li class="nav-item">
                                <a class="nav-link" id="li_timeline" runat="server" data-toggle="tab" href="#tab_timeline" role="tab" aria-expanded="false">Timeline</a>
                            </li>
                        </ul>

                        <div class="tab-content">
                            <div class="tab-pane fade show active in card card-header my-0 pb-3" role="tabpanel" id="tab_details">

                                <div class="x_title">
                                    <div class="row">
                                        <div class="col-md-8">
                                            <h2 style="font-weight: bold; font-size: 27px" runat="server" id="ref_Header"></h2>
                                            <%--hidden fields for refID--%>
                                            <input type="hidden" id="hdnLHOrefID" runat="server" />
                                            <input type="hidden" id="hdnCPSrefID" runat="server" />
                                        </div>
                                    </div>

                                    <span visible="false" style="color: #ffffff; font-size: 15px" runat="server" id="span_status">-</span>
                                    <div class="clearfix"></div>
                                </div>


                                <div class="card card-body p-0 pb-3 pt-3">

                                    <h2 style="font-weight: bold; font-size: 27px" runat="server" id="status_header"></h2>

                                    <!--FAMILY INFORMATION-->
                                    <div class="row card-body col-md-12 pt-3 pb-1">
                                        <h1 class="card-body pl-2 my-0 py-0" style="font-weight: bolder; font-size: 15px">Family Background</h1>
                                    </div>

                                    <div class="row card-body col-md-12 pt-2 pb-4 m-0">
                                        <table class="table table-bordered" id="familyMembersTable">
                                            <thead>
                                                <tr>
                                                    <th class="text-center">#</th>
                                                    <th>Full Name</th>
                                                    <th>Birth Date</th>
                                                    <th>Occupancy Status</th>
                                                    <th>Income</th>
                                                    <th class="text-center">Household Head</th>
                                                </tr>
                                            </thead>
                                            <tbody id="familyMembersTableBody"></tbody>
                                        </table>
                                    </div>


                                    <div class="card-body form-group col-md-12 py-0">
                                        <hr class="m-0 p-0" />
                                    </div>

                                    <div class="row form-group m-0 p-0">
                                        <div class="card-body form-group col-md-4 pt-2 m-0">
                                            <label>First Name</label>
                                            <input type="text" id="txt_AppfName" class="form-control required" placeholder="First Name" runat="server" disabled>
                                        </div>

                                        <div class="card-body form-group col-md-4 pt-2 m-0">
                                            <label>Middle Name</label>
                                            <input type="text" id="txt_AppmName" class="form-control" placeholder="Middle Name" minlenght="2" runat="server" disabled>
                                        </div>

                                        <div class="card-body form-group col-md-4 pt-2 m-0">
                                            <label>Last Name</label>
                                            <input type="text" id="txt_ApplName" class="form-control required" placeholder="Last Name" runat="server" disabled>
                                        </div>

                                        <div class="card-body form-group col-md-4 pt-2 m-0">
                                            <label>Suffix</label>
                                            <select id="dl_AppSuffix" class="form-control select2" runat="server" disabled>
                                                <option value="">Suffix</option>
                                                <option value="Jr">Jr</option>
                                                <option value="Sr">Sr</option>
                                                <option value="I">I</option>
                                                <option value="II ">II </option>
                                                <option value="III ">III </option>
                                            </select>
                                        </div>

                                        <div class="card-body form-group col-md-4 pt-2 m-0">
                                            <label>Birthday</label>
                                            <div class="input-group date lastNow" id="lastpublishdate" data-target-input="nearest">
                                                <input type="text" class="form-control datetimepicker-input required" placeholder="Birthday" data-target="#lastpublishdate" id="AppbDay" runat="server" disabled />
                                                <div class="input-group-append" data-target="#lastpublishdate" data-toggle="datetimepicker">
                                                    <div class="input-group-text"><i class="fa fa-calendar"></i></div>
                                                </div>
                                            </div>
                                        </div>

                                        <%--CIVIL STATUS--%>
                                        <div class="card-body form-group col-md-4 pt-2 m-0">
                                            <label>Civil Status</label>
                                            <select id="dl_AppCivilStatus" class="form-control select2 required" runat="server">
                                                <option value="">Civil Status</option>
                                                <option value="Single">Single</option>
                                                <option value="Married">Married</option>
                                                <option value="Widowed">Widowed</option>
                                                <option value="Divorced">Divorced</option>
                                            </select>
                                        </div>

                                        <div class="card-body form-group col-md-4 pt-2 pb-2 m-0">
                                            <label>Contact Number</label>
                                            <input type="text" id="txt_AppContact" class="form-control required phone-mask" placeholder="Contact Number" runat="server">
                                        </div>

                                        <div class="card-body form-group col-md-4 pt-2 pb-2 m-0">
                                            <label>Voter's ID Number</label>
                                            <input type="text" id="txt_AppVoters" class="form-control required" placeholder="Voter's ID Number" runat="server">
                                        </div>

                                        <div class="card-body form-group col-md-4 pt-2 pb-2 m-0">
                                            <label>Occupancy Status</label>
                                            <select id="dl_AppoccupancyStatus" runat="server" class="form-control select2">
                                                <option value="">-</option>
                                            </select>
                                        </div>

                                        <div class="card-body form-group col-md-4 pt-2 pb-2 m-0">
                                            <label>Purok/Sitio/Subdivision</label>
                                            <select id="dl_ApppsID" runat="server" class="form-control select2">
                                                <option value="">-</option>
                                            </select>
                                        </div>

                                        <div class="card-body form-group col-md-8 pt-2 pb-2 m-0">
                                            <label>Address</label>
                                            <textarea id="txt_AppAddress" class="form-control" rows="4" placeholder="Short Address" style="height: 38px; min-height: 38px;" spellcheck="false" runat="server"></textarea>
                                        </div>

                                    </div>

                                    <div class="card-body form-group col-md-12 py-0 pt-3">
                                        <hr class="m-0 p-0" />
                                    </div>

                                    <div class="row form-group pt-0 m-0 p-0">
                                        <div class="card-body form-group col-md-4 m-0 pt-0 pb-2">
                                            <label>Select Category</label>
                                            <select id="dl_AppCategory" class="form-control required select2" runat="server" disabled>
                                            </select>
                                        </div>

                                        <div class="card-body form-group col-md-8 m-0 pt-0 pb-2">
                                            <label>Remarks</label>
                                            <textarea id="txt_AppRemarks" class="form-control" rows="4" placeholder="Remarks" style="height: 38px; min-height: 38px;" spellcheck="false" runat="server"></textarea>
                                        </div>

                                    </div>

                                    <div class="clearfix"></div>

                                    <div class="ln_solid" style="margin-top: 0"></div>

                                </div>

                                <div class="justify-content-md-center row">
                                    <div class="col col-md">
                                        <button type="button" class="btn btn-default pr-4 pl-4" id="btn_back">
                                            <i class="ace-icon fa fa-list icon-on-right bigger-110"></i>&nbsp;&nbsp;&nbsp;Back to List
                                        </button>
                                    </div>

                                    <div class="col col-auto">
                                        <button type="button" class="btn bg-gradient-primary bg-blue pr-3 pl-3" id="btn_set">
                                            <i class="fas fa-check-circle" aria-hidden="true"></i>&nbsp;&nbsp;&nbsp;Set Status

                                        </button>
                                    </div>

                                    <div class="modal fade" id="modal_setappstatus">
                                        <div class="modal-dialog mt-5">
                                            <div class="modal-content">
                                                <div class="modal-header">
                                                    <h1 class="modal-title" style="font-weight: bold; font-size: x-large">Tag Status As</h1>
                                                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                                        <span aria-hidden="true">&times;</span>
                                                    </button>
                                                </div>
                                                <div class="modal-body">
                                                    <div class="col">
                                                        <select id="dl_profileApplicantStatus" class="form-control select2 required" runat="server">
                                                        </select>
                                                    </div>
                                                </div>
                                                <div class="modal-footer justify-content-between">
                                                    <button type="button" class="btn btn-outline-info" data-dismiss="modal">Close</button>
                                                    <button id="Button1" type="button" class="btn bg-gradient-success pr-4 pl-4" onclick="if (!validateForm('required')) return;" onserverclick="Save_Click" runat="server">
                                                        <i class="fa fa-save"></i>&nbsp;&nbsp;&nbsp;Save
                                                    </button>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="col col-auto">
                                        <button type="button" class="btn btn-success pr-3 pl-3" id="btn_save">
                                            <i class="ace-icon fa fa-save icon-on-right bigger-110"></i>&nbsp;&nbsp;&nbsp;Save
                                        </button>
                                    </div>

                                </div>
                            </div>

                            <%--TIMELINE--%>
                            <div role="tabpanel" class="tab-pane" id="tab_timeline">
                                <div class="tab-content">
                                    <div class="tab-pane fade show active in card card-header m-0" role="tabpanel">
                                        <div class="x_title">
                                            <div class="row">
                                                <div class="col-md-8">
                                                    <h2 style="font-weight: bold; font-size: 27px" runat="server">Timeline</h2>
                                                </div>
                                            </div>
                                            <div class="card card-body pb-0 pt-4">
                                                <%--TIMELINE CONTENTS--%>
                                                <div class="row">
                                                    <div class="col-md-12">
                                                        <div class="timeline">

                                                            <div class="time-label">
                                                                <span class="bg-green">28 Sept. 2023</span>
                                                            </div>


                                                            <div>
                                                                <i class="fas fa-check-circle bg-primary"></i>
                                                                <div class="timeline-item">
                                                                    <span class="time"><i class="fas fa-clock"></i> 12:05</span>
                                                                    <h3 class="timeline-header" style="font-weight: bolder">Update Form 
                                                                    </h3>
                                                                    <div class="timeline-body">
                                                                        by: User1
                                                                    </div>
                                                                    <div class="timeline-footer">
                                                                        Status: Tag As Disapproved
                                                                    </div>
                                                                </div>
                                                            </div>

                                                            <div>
                                                                <i class="fas fa-ban bg-red"></i>
                                                                <div class="timeline-item">
                                                                    <h3 class="timeline-header" style="font-weight: bolder">Update Form 
                                            <small style="font-size: 12px; font-style: italic">3:56pm</small>
                                                                    </h3>
                                                                    <div class="timeline-body">
                                                                        by: User1
                                                                    </div>
                                                                    <div class="timeline-footer">
                                                                        Status: Disapproved
                                                                    </div>
                                                                </div>
                                                            </div>

                                                            <div class="time-label">
                                                                <span class="bg-green">25 Sept. 2023</span>
                                                            </div>

                                                            <div>
                                                                <i class="fas fa-file bg-green"></i>
                                                                <div class="timeline-item">
                                                                    <h3 class="timeline-header" style="font-weight: bolder">Create Form 
                                            <small style="font-size: 12px; font-style: italic">1:07pm</small>
                                                                    </h3>
                                                                    <div class="timeline-body">
                                                                        by: User1
                                                                    </div>
                                                                    <div class="timeline-footer">
                                                                    </div>
                                                                </div>
                                                            </div>


                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="col col-md">
                                                <button type="button" class="btn btn-default pr-4 pl-4" id="btn_backlist">
                                                    <i class="ace-icon fa fa-list icon-on-right bigger-110"></i>&nbsp;&nbsp;&nbsp;Back to List
                                                </button>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="modal fade" id="modal_confirm">
            <div class="modal-dialog mt-5">
                <div class="modal-content">
                    <div class="modal-body">
                        <div class="form-group">
                            <h1 class="modal-title" style="font-weight: bold; font-size: x-large">Have you verified that the information in the form is accurate?</h1>
                        </div>
                    </div>

                    <div class="modal-footer justify-content-between">
                        <button type="button" class="btn btn-outline-info" data-dismiss="modal">Close</button>
                        <button id="modal_save" type="button" class="btn bg-gradient-success pr-4 pl-4" onclick="if (!validateForm('required')) return;" onserverclick="Save_Click" runat="server">
                            <i class="fa fa-save"></i>&nbsp;&nbsp;&nbsp;Save
                        </button>
                    </div>
                </div>
            </div>
        </div>

    </div>

    <script>
        $(document).ready(function () {
            // Apply the input mask to the element with the 'phone-mask' class
            $('.phone-mask').inputmask({
                mask: "99999999999",
                showMaskOnFocus: false, // Hide the mask while input is focused
                oncomplete: function () {
                    // This function will be called when the input is complete
                    var inputValue = $(this).val();
                    console.log("Input value: " + inputValue);
                }
            });
        });

        $(document).ready(function () {
            // Target the "Save" button by its ID
            $('#btn_save').click(function () {
                $('#modal_confirm').modal('show');
            });
        });

        $(document).ready(function () {
            // Target the "Save" button by its ID
            $('#btn_set').click(function () {
                $('#modal_setappstatus').modal('show');
            });
        });

        $("#btn_back").click(function () {
            window.location.replace("Applicant.aspx");
        });

        $("#btn_backlist").click(function () {
            window.location.replace("Applicant.aspx");
        });

        $("#btn_add").click(function () {
            $("#btn_add").addClass('hide');
            $("#<%=div_list.ClientID %>").addClass('hide');
            $("#<%=div_details.ClientID %>").removeClass('hide');
        });

        function redirectToLHOForms() {
            // Redirect to the Forms.aspx page
            window.location.href = 'LHOForms.aspx';
        }

        function generateTable() {
            var dateRange = $('#reportrange').val();
            var category = $('#dl_category').val();
            var status = $('#dl_applicationstatus').val();

            if (dateRange && category !== null && status !== null) {
                // Extract the start and end dates from the DateRangePicker input
                var dateParts = dateRange.split(' - ');
                var startDate = moment(dateParts[0], 'MM/DD/YYYY').format('YYYY-MM-DD');
                var endDate = moment(dateParts[1], 'MM/DD/YYYY').format('YYYY-MM-DD');

                var requestData = {
                    _from: startDate,
                    _to: endDate,
                    status: status,
                    category: category
                };

                // Log the requestData to the console for debugging
                console.log("Request Data:", requestData);

                $.ajax({
                    type: "POST",
                    url: "Applicant.aspx/filterDataTable",
                    data: JSON.stringify(requestData),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (r) {
                        console.log("Response Data:", r);
                        // Get the DataTable instance
                        var dataTable = $('#example1').DataTable();
                        dataTable.clear()
                        dataTable.rows.add($(r.d)).draw();
                    }
                });
            }
        }

        $('#reportrange').daterangepicker(
            {
                ranges: {
                    'Today': [moment(), moment()],
                    'Yesterday': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
                    'Last 7 Days': [moment().subtract(6, 'days'), moment()],
                    'Last 30 Days': [moment().subtract(29, 'days'), moment()],
                    'This Month': [moment().startOf('month'), moment().endOf('month')],
                    'Last Month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
                },
                startDate: moment().subtract(29, 'days'),
                endDate: moment()
            },
            function (start, end) {
                $('#reportrange').html(start.format('MMMM D, YYYY') + ' - ' + end.format('MMMM D, YYYY'))
            }
        )
    </script>
</asp:Content>
