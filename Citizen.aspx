<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="Citizen.aspx.vb" Inherits="Citizen" %>

<%@ MasterType VirtualPath="~/MasterPage.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <input hidden="hidden" id="txt_refID" runat="server" />
    <div>
        <div class="page-title">
            <div class="title_left">
                <h3 id="pageTitle"></h3>
            </div>
            <button type="button" class="btn btn-success inline pull-right pr-3 pl-3" style="margin: 5px 0 0 0" id="btn_add" runat="server" onclick="redirectToCPSForms();">
                <i class="fa fa-plus"></i>&nbsp;&nbsp;&nbsp;Add New
            </button>

        </div>

        <div class="clearfix"></div>

        <div role="alert" id="div_formAlert" runat="server" class="hide">
            <button type="button" class="close" data-dismiss="alert" aria-label="Close">
                <span aria-hidden="true">×</span>
            </button>
            <strong>Alert!</strong> <span id="span_alert" runat="server"></span>
        </div>

        <div class="clearfix"></div>

        <div class="x_panel" id="div_list" runat="server">
            <div class="x_content">

                <div class="tabpanel" style="margin-top:15px;">
                    <div class="card card-success card-outline card-tabs pt-1">

                        <ul id="myTab" class="nav nav-tabs bar_tabs justify-content-end" role="tablist">
                            <li class="nav-item">
                                <a class="nav-link active" id="custom-tabs-three-profile-tab" data-toggle="tab" href="#tab_filterCategory" role="tab" aria-expanded="false">Filter By Category</a>
                            </li>
                            <li class="nav-item">
                                <a class="nav-link" id="A1" runat="server" data-toggle="tab" href="#tab_filterAll" role="tab" aria-expanded="true">Filter By Name or Ref No</a>
                            </li>
                        </ul>

                        <div class="card card-body row m-0 p-2 pt-3">
                            <div class="tab-content">
                                <div role="tabpanel" class="tab-pane fade  in" id="tab_filterAll">
                                    <div class="container" style="padding: 10px">
                                        <div class="card-body col-md-12 has-feedback">
                                            <label>Search By Keyword</label>
                                            <div class="input-group">
                                                <input type="search" id="txt_wildcard" class="form-control form-control-md">
                                                <div class="input-group-append">
                                                    <button type="button" class="btn btn-md btn-default" runat="server" onchange="generateReg(this);">
                                                        <i class="fa fa-search"></i>
                                                    </button>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div role="tabpanel" class="tab-pane active fade show in" id="tab_filterCategory">
                                    <div class="container" style="padding: 10px">
                                        <div class="row">
                                            <div class="col-md-6 col-xs-12 form-group">
                                                <label>Barangay</label>
                                                <div class="sel2">
                                                    <select id="dl_brgyFilter" class="form-control select2" runat="server" onchange="generateTable(1);">
                                                        <%--<option value="All">All</option>--%>
                                                    </select>
                                                </div>
                                            </div>

                                            <div class="col-md-6 col-xs-12 form-group">
                                                <label class="control-label">Status</label>
                                                <div class="sel2">
                                                    <select id="dl_statusFilter" class="form-control select2" onchange="generateTable(1);">
                                                        <option value="1">Active</option>
                                                        <option value="0">Inactive</option>
                                                    </select>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="py-0"></div>

                <div class="table-responsive card card-body">
                    <table id="example1" class="table table-striped table-bordered dataTable">
                        <thead>
                            <tr>
                                <th>Ref No</th>
                                <th>Name</th>
                                <th>Age</th>
                                <th>Birth Date</th>
                                <th>Sex</th>
                                <th>Address</th>
                                <th>Status</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:PlaceHolder ID="PlaceHolder1" runat="server"></asp:PlaceHolder>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>

        <div class="x_panel hide" id="div_details" runat="server">
            <div class="x_title">
                <h2 runat="server" id="h2_header">Add New</h2>
                <span class="hide" style="color: #ffffff; font-size: 15px" runat="server" id="span_status">-</span>
                <div class="clearfix"></div>
            </div>

            <div class="x_content">
                <div class="tabpanel">
                    <ul id="myTab1" class="nav nav-tabs bar_tabs justify-content-end" role="tablist">
                        <li class="nav-item">
                            <a class="nav-link active" id="custom-tabs-three-profile" data-toggle="tab" href="#tab_details" role="tab" aria-expanded="true">Details</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" id="li_timeline" runat="server" data-toggle="tab" href="#tab_timeline" role="tab" aria-expanded="false">Timeline</a>
                        </li>
                    </ul>

                    <div class="tab-content">
                        <div role="tabpanel" class="tab-pane fade active show in" id="tab_details">
                            <div class="container">


                                <div class="row">
                                    <div class="form-group col-md-3 col-xs-12">
                                        <label class="control-label">First Name</label>
                                        <small style="font-style: italic">(no special chacters)</small>
                                        <input id="txt_fName" runat="server" class="form-control" onchange="validateName(this)" required="required" />
                                    </div>
                                    <div class="form-group col-md-3 col-xs-12">
                                        <label class="control-label">Middle Name</label>
                                        <small style="font-style: italic">(no special chacters and optional)</small>
                                        <input id="txt_mName" runat="server" minlength="2" class="form-control" onchange="validateName(this)" />
                                    </div>
                                    <div class="form-group col-md-3 col-xs-12">
                                        <label class="control-label">Last Name</label>
                                        <small style="font-style: italic">(no special chacters)</small>
                                        <input id="txt_lName" runat="server" class="form-control" onchange="validateName(this)" required="required" />
                                    </div>
                                    <div class="form-group col-md-3 col-xs-12">
                                        <label class="control-label">Suffix</label>
                                        <small style="font-style: italic">(optional)</small>
                                        <div class="sel2">
                                            <select id="dl_suffix" class="form-control select2" onchange="validateName(this)" runat="server">
                                                <option value="-">-</option>
                                                <option value="JR">JRA</option>
                                                <option value="SR">SR</option>
                                                <option value="JR">JR</option>
                                                <option value="I">I</option>
                                                <option value="II">II</option>
                                                <option value="III">III</option>
                                                <option value="IV">IV</option>
                                                <option value="V">V</option>
                                                <option value="VI">VI</option>
                                                <option value="VII">VII</option>
                                            </select>
                                        </div>
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="form-group col-md-3 col-xs-12">
                                        <label>BirthDate</label>
                                        <div class="input-group date lastNow" id="lastpublishdate" data-target-input="nearest">
                                            <input type="text" class="form-control datetimepicker-input requiredPage" data-target="#lastpublishdate" id="txt_bdate" runat="server" />
                                            <div class="input-group-append" data-target="#lastpublishdate" data-toggle="datetimepicker">
                                                <div class="input-group-text"><i class="fa fa-calendar"></i></div>
                                            </div>

                                        </div>
                                    </div>


                                    <div class="form-group col-md-3 col-xs-12">
                                        <label class="control-label">Sex</label>
                                        <div class="sel2">
                                            <select id="dl_sex" class="form-control select2" required="required" runat="server">
                                                <option value="">-</option>
                                                <option value="0">Female</option>
                                                <option value="1">Male</option>
                                            </select>
                                        </div>
                                    </div>

                                    <div class="form-group col-md-3 col-xs-12">
                                        <label class="control-label">Civil Status</label>
                                        <div class="sel2">
                                            <select id="dl_civilStatus" class="form-control select2" required="required" runat="server">
                                                <option value="">-</option>
                                                <option value="Single">Single</option>
                                                <option value="Married">Married</option>
                                                <option value="Widow/Widower">Widow/Widower</option>
                                                <option value="Separated/Annulled">Separated/Annulled</option>
                                            </select>
                                        </div>
                                    </div>

                                    <div class="form-group col-md-3 col-xs-12">
                                        <label class="control-label">Contact No </label>
                                        <small style="font-style: italic">(09xxxxxxxxx)</small>
                                        <input id="txt_contactNo" runat="server" maxlength="11" minlength="11" class="form-control allownumericwithoutdecimal" required="required" />
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="form-group col-md-6 col-xs-12">
                                        <label>Address Details</label>
                                        <div>
                                            <textarea type="text" id="txt_address" required="required" runat="server" class="form-control"></textarea>
                                        </div>
                                    </div>
                                    <div class="form-group col-md-6 col-xs-12">
                                        <label class="control-label">Purok</label>
                                        <div class="sel2">
                                            <select id="dl_psID" class="form-control select2" required="required" runat="server">
                                                <option value="">-</option>
                                            </select>
                                        </div>
                                    </div>
                                </div>
                                <hr />
                                <div class="row card-body col-md-12 pt-3 pb-1">
                                    <h1 class="card-body pl-2 my-0 py-0" style="font-weight: bolder; font-size: 15px">Family Background</h1>
                                </div>

                                <div class="row card-body col-md-12 pt-2 pb-4 m-0">
                                    <table class="table table-bordered" id="familyMembersTable">
                                        <thead>
                                            <tr>
                                                <th>Reference ID</th>
                                                <th>Full Name</th>
                                                <th>Birth Date</th>
                                                <th>Civil Status</th>
                                                <th>Income</th>
                                                <th class="text-center">Actions</th>
                                            </tr>
                                        </thead>
                                        <tbody id="familyMembersTableBody"></tbody>
                                    </table>
                                </div>

                                <div class="form-group">
                                    <label class="control-label">Status</label>
                                    <div class="sel2">
                                        <select id="dl_status" class="form-control select2" required="required" runat="server">
                                            <option value="1">ACTIVE</option>
                                            <option value="0">INACTVE</option>
                                        </select>
                                    </div>
                                </div>


                                <div class="clearfix"></div>

                                <div class="ln_solid" style="margin-top: 0"></div>

                                <div class="form-group text-right">
                                    <button type="button" class="btn btn-default pull-left" id="btn_back">
                                        <i class="ace-icon fa fa-list icon-on-right bigger-110"></i>Back to List
                                    </button>

                                    <button type="button" class="btn btn-success" runat="server" id="btn_save">
                                        Save <i class="ace-icon fa fa-save icon-on-right bigger-110"></i>
                                    </button>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div role="tabpanel" class="tab-pane" id="tab_timeline">
                        <ul class="list-unstyled timeline">
                            <asp:PlaceHolder ID="ph_timeline" runat="server"></asp:PlaceHolder>
                        </ul>
                    </div>
                </div>
            </div>

        </div>
    </div>



    <script>
        $("#btn_back").click(function () {
            window.location.replace("Citizen.aspx");
        });

        $("#btn_add").click(function () {
            $("#btn_add").addClass('hide');
            $("#<%=div_list.ClientID %>").addClass('hide');
            $("#<%=div_details.ClientID %>").removeClass('hide');
        });

        function redirectToCPSForms() {
            window.location.href = 'CPSForms.aspx';
        }

        function generateTable(mode) {
            var wildcard = $('#txt_wildcard').val();
            var brgyFilter = $('#<%=dl_brgyFilter.ClientID%>').val();
            var statusFilter = $('#dl_statusFilter').val();

            if (((mode == 0 && wildcard != '') || (mode == 1 && statusFilter != '')) && $('#<%=txt_refID.ClientID%>').val() == '') {
                $.ajax({
                    type: "POST",
                    url: "Citizen.aspx/LoadList",
                    data: "{status: '" + statusFilter + "', brgyFilter: '" + brgyFilter + "', mode: '" + mode + "', wildcard: '" + wildcard + "'}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (r) {
                        console.log("Response Data:", r);
                        console.log(brgyFilter);
                        // Get the DataTable instance
                        var dataTable = $('#example1').DataTable();
                        dataTable.clear()
                        dataTable.rows.add($(r.d)).draw();
                    }
                });
            }
        }

    </script>
</asp:Content>
