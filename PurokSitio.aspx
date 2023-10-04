﻿<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="PurokSitio.aspx.vb" Inherits="Departments" %>
<%@ MasterType  virtualPath="~/MasterPage.master"%>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
   <div class="row">
       <div class="col-12">
            <div class="card">
                <div class="card-header">
                    <button type="button" data-target="#modal_add" data-toggle="modal" id="btn_addnew" class="btn bg-gradient-primary">
                                    <i class="ace-icon fa fa-plus-circle bigger-110"></i>
                                    Add New
                    </button>
                </div> 

                <div class="card-body">
                    <div class="table-responsive ">
                        <table id="example1" class="table table-bordered table-striped basicTable">
                          <thead>
                              <tr>
                                <th>ID</th>
                                <th>Purok Sitio</th>
                                <th>Barangay</th>
                                <th>Status</th>
                              </tr>
                          </thead>
                          <tbody>
                            <asp:PlaceHolder ID="ph_list" runat="server"></asp:PlaceHolder>
                          </tbody> 
                        </table> 
                    </div>
                </div> 
            </div> 
       </div> 
   </div>

    <div class="modal fade" id="modal_add">
        <div class="modal-dialog">
          <div class="modal-content">
            <div class="modal-header">
              <h4 class="modal-title" runat="server" id="h4_title">Add New</h4>
              <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                <span aria-hidden="true">&times;</span>
              </button>
            </div>
              <div class="modal-body">

                  <!--DL BARANGAY-->
                  <div class="form-group">
                      <label>Barangay</label>
                      <select id="dl_brgy" runat="server" class="form-control select2">
                          <option value="">-</option>
                      </select>
                  </div>

                  <!--FORM PUROK SITIO-->
                  <div class="form-group">
                      <label>Purok Sitio</label>
                      <div>
                          <input id="txt_name" maxlength="45" class="form-control required" runat="server"/>
                      </div>

                  </div>

                  
                  <!--DL STATUS-->
                  <div class="form-group" id="div_status" runat="server" visible="false">
                      <label>Status</label>
                      <select id="dl_status" runat="server" class="form-control select2">
                          <option value="1">Active</option>
                          <option value="0">Inactive</option>
                      </select>
                  </div>
              </div>
            <div class="modal-footer justify-content-between">
              <button type="button" class="btn btn-outline-dark" data-dismiss="modal">Close</button>
              <button type="button" class="btn btn-outline-success" onclick="if (!validateForm('required')) return;" onserverclick="Save_Click" runat ="server">Save changes</button>
            </div>
          </div>
        </div>
    </div>
    
    <script>
        $('#<%=dl_brgy.ClientID%>').on("change", function (event) {
            $('#<%=txt_name.ClientID%>').val('');
        });

        $('#<%=txt_name.ClientID%>').on("change", function (event) {
            if (this.value != '') {
                $.ajax({
                type: "POST",
                url: documentName + "/validatePurokSitio",
                    data: "{brgyID: '" + $('#<%=dl_brgy.ClientID%>').val() + "', entry: '" + this.value + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (r) {
                    if (r.d == 1) {
                        $('#<%=txt_name.ClientID%>').val('').focus();
                            systemMsg(0, 'Entry record already exists.');
                        }
                    },
                    failure: function (response) {
                        systemMsg(0, response.d);
                    }
                });
            }
        });

    </script>

</asp:Content>
