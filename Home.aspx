﻿<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="Home.aspx.vb" Inherits="Home" %>

<%@ MasterType VirtualPath="~/MasterPage.master" %>
<%@ Import Namespace="System.Web.UI.WebControls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">


    <div class="card card-header form-group pt-3 pb-2">
        <!--FIRST ROW-->
        <div class="row">
            <!--NEW APPLICANTS-->
            <div class="col-md-3 col-6">
                <div class="small-box bg-primary">
                    <div class="inner">
                        <h3><span id="lblTotalApplicants" runat="server">0</span></h3>
                        <p>Total Applicants</p>
                    </div>

                    <div class="icon">
                        <i class="fas fa-user-plus"></i>
                    </div>
                    <div class="text-center small-box-footer bg-primary-secondary">
                    </div>
                </div>
            </div>

            <!--APPROVED APPLICANTS-->
            <div class="col-md-3 col-6">

                <div class="small-box bg-success">
                    <div class="inner">
                        <h3><span id="lblApprovedApplicants" runat="server">0</span></h3>
                        <p>Approved Applicants</p>
                    </div>
                    <div class="icon">
                        <i class="fas fa-users"></i>
                    </div>
                    <div class="text-center small-box-footer bg-success-secondary">
                    </div>
                </div>
            </div>

            <!--PENDING APPLICANTS-->
            <div class="col-md-3 col-6">
                <div class="small-box bg-warning">
                    <div class="inner">
                        <h3><span id="lblOnProcessApplicants" runat="server">0</span></h3>
                        <p>On-Process Application</p>
                    </div>
                    <div class="icon">
                        <i class="fas fa-clock"></i>
                    </div>
                    <div class="text-center small-box-footer bg-warning-secondary">
                    </div>
                </div>
            </div>

            <!--DECLINED APPLICANTS-->
            <div class="col-md-3 col-6">
                <div class="small-box bg-danger">
                    <div class="inner">
                        <h3><span id="lblDeclinedApplicants" runat="server">0</span></h3>
                        <p>Declined Applicants</p>
                    </div>
                    <div class="icon">
                        <i class="fas fa-user-slash fa-sm"></i>
                    </div>
                    <div class="text-center small-box-footer bg-danger-secondary">
                    </div>
                </div>
            </div>

        </div>

        <div class=" form-group col-md-12 pt-1">
            <hr class="m-0 p-0" />
        </div>

        <!--SECOND ROW-->
        <div class="row form-group">
            <div class="col-md-9">
                <h1 style="font-weight: bolder; font-size: x-large">
                    <i class="fas fa-chart-bar fa-sm"></i>
                     Applicants Summary
                    <small style="font-size: 16px">Daily Progress</small>
                </h1>
            </div>
            <div class="col-md-3">
                <div class="input-group">
                    <input type="text" class="form-control" id="dateRangePick" />
                    <div class="input-group-append">
                        <div class="input-group-text"><i class="fa fa-calendar"></i></div>
                    </div>
                </div>
            </div>
        </div>

        <div class="row justify-content-center pt-1">
            <div class="col-md-12">
                <div class="chart">
                    <canvas id="barChart" style="height: 480px;"></canvas>
                </div>
            </div>
        </div>
    </div>

    <!--DAILY REPORTS-->
    <div class="card card-header form-group pt-3 pb-2">
        <div class="col-md-9">
            <h3 style="font-weight: bolder; font-size: x-large">
                <i class="fas fa-file fa-sm"></i>
                Daily Reports
            </h3>
        </div>

        <div class=" form-group col-md-12 pt-1">
            <hr class="m-0 p-0" />
        </div>

        <div class="row">
            <div class="form-group col-md-7 ">
                <label style="font-weight: normal;">Date:</label>
                <div class="input-group">
                    <input type="text" class="form-control" id="datefilter" value="" />
                    <div class="input-group-append">
                        <div class="input-group-text"><i class="fa fa-calendar"></i></div>
                    </div>
                </div>
            </div>
            <div class="form-group col-md-5 align-self-end">
                <button type="button" id="gen_btn" class="btn btn-success btn-block btn-lg" style="margin-bottom: 1px; font-size: 24px; font-weight: bold">Generate Copy <i class="far fa-copy bigger-110"></i></button>
            </div>
        </div>
    </div>
    <!--DAILY REPORTS PREVIEW (TABLE)-->
    <div class="modal fade" id="preview_modal">
        <div class="modal-lg modal-dialog max-width: 100%; margin: 0 auto;">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title" runat="server" id="h4_title">Preview</h4>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body clearfix">
                    <div class="table-responsive" id="genReport">
                        <table class="table table-bordered">
                            <thead>
                                <tr>
                                    <th>Reference ID No.</th>
                                    <th>Full Name</th>
                                    <th>Address</th>
                                    <th>Application Status</th>
                                    <th style="width: 40px">Category</th>
                                    <th>Application Date</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:PlaceHolder ID="ph_list" runat="server"></asp:PlaceHolder>
                            </tbody>
                        </table>
                    </div>
                </div>
                <div class="modal-footer justify-content-between">
                    <button type="button" class="btn btn-outline-info" data-dismiss="modal">Close</button>
                    <button type="button" onclick="printTable()" class="btn btn-success buttons-print" style="width: 120px" tabindex="0" aria-controls="example1">
                        <span>Print</span>
                        <i class="fas fa-print"></i>
                    </button>
                </div>
            </div>
        </div>
    </div>














    <script>

        function getBackgroundColor(index) {
            const colors = [
                'rgba(40, 167, 69, 0.5)',
                'rgba(255, 193, 7, 0.5)',
                'rgba(220, 53, 69, 0.5)',
            ];
            return colors[index % colors.length];
        }

        function getBorderColor(index) {
            const colors = [
                'RGB(40, 167, 69)',
                'RGB(255, 193, 7)',
                'RGB(220, 53, 69)',
            ];
            return colors[index % colors.length];
        }

        function updateBarChart(startDate, endDate) {
            // Generate an array of dates within the selected date range
            const dates = [];
            let currentDate = moment(startDate);

            while (currentDate.isSameOrBefore(endDate)) {
                dates.push(currentDate.format('MM/DD/YYYY')); // Format the date as "MM/DD/YYYY"
                currentDate.add(1, 'day'); // Move to the next day
            }

            // AJAX request to fetch data from the server
            $.ajax({
                type: 'POST',
                url: 'Home.aspx/GetDataForChart',
                data: JSON.stringify({
                    startDate: startDate.toISOString(),
                    endDate: endDate.toISOString()
                }),
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (response) {
                    const chartData = JSON.parse(response.d);

                    // Prepare datasets for the chart
                    const categories = ['Approved Applicants', 'On-Process Applicants', 'Declined Applicants'];
                    const datasets = categories.map((category, index) => ({
                        label: category.charAt(0).toUpperCase() + category.slice(1), // Capitalize the category name
                        data: dates.map(date => chartData[date + '_' + category]), // Access data using the date + category as the key
                        backgroundColor: getBackgroundColor(index),
                        borderColor: getBorderColor(index),
                        borderWidth: 2,
                        borderRadius: 5,
                        borderSkipped: false
                        ,
                    }));

                    const ctx = document.getElementById('barChart').getContext('2d');

                    // Destroy any existing chart to prevent conflicts
                    if (window.myChart) {
                        window.myChart.destroy();
                    }

                    // Create a new chart
                    window.myChart = new Chart(ctx, {
                        type: 'bar',
                        data: {
                            labels: dates, // Dates as labels on the x-axis
                            datasets: datasets,
                        },
                        options: {
                            responsive: true,
                            maintainAspectRatio: false,
                            scales: {
                                xAxes: {
                                    stacked: false,
                                },
                                yAxes: [{
                                    stacked: false,
                                    ticks: {
                                        stepSize: 1,
                                        beginAtZero: true,
                                        precision: 0,
                                        callback: function (value) {
                                            if (value % 1 === 0) {
                                                return value;
                                            }
                                        }
                                    },
                                }],
                            },
                        },
                    });
                },
                failure: function (response) {
                    systemMsg(0, response.d);
                }
            });
        }
        // GENERATE BUTTON
        $(document).ready(function () {
            // Function to check if the date range picker has a valid value
            function hasValidDateRange() {
                var dateRange = $('#datefilter').val();
                return dateRange !== '' && dateRange.split(' - ').length === 2;
            }
            // Function to show the modal
            function showPreviewModal() {
                $('#preview_modal').modal('show');
                // Add your code to load data into the modal here
            }
            // Add a click event handler to the "Generate" button
            $('#gen_btn').click(function () {
                if (hasValidDateRange()) {
                    // Get the selected date range
                    var dateRange = $('#datefilter').val().split(' - ');

                    // Filter the table rows based on the selected date range
                    filterTableByDateRange(dateRange[0], dateRange[1]);

                    // Show the modal
                    showPreviewModal();
                } else {
                    // Handle invalid date range here
                }
            });

            function filterTableByDateRange(startDate, endDate) {
                // Loop through the table rows and hide/show them based on the date range
                $('tbody tr').each(function () {
                    var applicationDate = $(this).find('td:eq(5)').text(); // Assuming the date is in the 6th column
                    var date = new Date(applicationDate);

                    if (date >= new Date(startDate) && date <= new Date(endDate)) {
                        $(this).show(); // Show the row if it's within the selected range
                    } else {
                        $(this).hide(); // Hide the row if it's outside the selected range
                    }
                });
            }
        });

        // REPORTS DATE RANGE PICKER
        $(function () {

            $('#datefilter').daterangepicker({
                autoUpdateInput: false,
                locale: {
                    cancelLabel: 'Clear'
                },
                ranges: {
                    'Today': [moment(), moment()],
                    'Yesterday': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
                    'Last 7 Days': [moment().subtract(6, 'days'), moment()],
                    'Last 30 Days': [moment().subtract(29, 'days'), moment()],
                    'This Month': [moment().startOf('month'), moment().endOf('month')],
                    'Last Month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
                },

                drops: 'up'
            });

            $('#datefilter').on('apply.daterangepicker', function (ev, picker) {
                $(this).val(picker.startDate.format('MM/DD/YYYY') + ' - ' + picker.endDate.format('MM/DD/YYYY'));
            });

            $('#datefilter').on('cancel.daterangepicker', function (ev, picker) {
                $(this).val('');
            });

        });
        //PRINT FUNCTION

        function printTable() {
            var printContents = document.getElementById('genReport').innerHTML;
            var originalContents = document.body.innerHTML;

            document.body.innerHTML = printContents;

            window.print();
            document.body.innerHTML = originalContents;
            setTimeout(function () {
                location.reload();
            },);
        }

        // CHART DATE RANGE PICKER
        $(function () {
            var start = moment().subtract(6, 'days');
            var end = moment();

            $('#dateRangePick').daterangepicker({
                startDate: start,
                endDate: end,
                ranges: {
                    'Today': [moment(), moment()],
                    'Yesterday': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
                    'Last 7 Days': [moment().subtract(6, 'days'), moment()],
                    'Last 30 Days': [moment().subtract(29, 'days'), moment()],
                    'This Month': [moment().startOf('month'), moment().endOf('month')],
                    'Last Month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
                }
            }, function (selectedStartDate, selectedEndDate) {
                // Update the button text with the selected date range
                $('#dateRangeText').html(selectedStartDate.format('MM/DD/YYYY') + ' - ' + selectedEndDate.format('MM/DD/YYYY'));
                //console.log('Selected start date:', selectedStartDate.format('MM/DD/YYYY'));
                //console.log('Selected end date:', selectedEndDate.format('MM/DD/YYYY'));

                // Update the bar chart with selected date range
                updateBarChart(selectedStartDate, selectedEndDate);
            });

            // Initial chart rendering
            updateBarChart(start, end); // Render with initial date range
        });
    </script>
</asp:Content>