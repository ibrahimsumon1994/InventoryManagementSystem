<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FixedCostReport.aspx.cs" Inherits="SmokeMusicCafe.FixedCostReport" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no" />
    <link rel="stylesheet" type="text/css" href="css/bootstrap.css" />
    <link rel="stylesheet" type="text/css" href="css/bootstrap.min.css" />
    <link rel="stylesheet" href="//code.jquery.com/ui/1.12.1/themes/base/jquery-ui.css">
    <link rel="stylesheet" href="/resources/demos/style.css">
    <script src="js/bootstrap.js" type="text/javascript"></script>
    <script src="js/bootstrap.min.js" type="text/javascript"></script>
    <script src="js/jquery-3.4.1.js" type="text/javascript"></script>
    <script src="jquery-ui-eggplant/jquery-ui.js" type="text/javascript"></script>
</head>
<body style="background-image: url(picture1/background.jpeg); background-repeat: no-repeat; background-size: cover">

    <form id="form1" runat="server">
        <div class="container-fluid">
            <asp:Label ID="lblHeadingPage" runat="server" Text="Report Page Of Other Cost" class="badge badge-primary" Style="width: 100%; height: 30px; font-size: large; margin: 10px; margin-left: 0px"></asp:Label>
            <div style="width: 50%; height: auto; float: left">
                <ul style="list-style-type: none;">
                    <li>
                        <asp:Button ID="btnPreviousPage" class="btn btn-outline-success" runat="server" Text="Back" PostBackUrl="MonthlyFixedCost.aspx"></asp:Button>
                    </li>
                    <li>
                        <asp:Label ID="lblStartDate" runat="server" Text="Start Date" class="badge badge-primary" Style="width: 200px; height: 30px; font-size: large; margin: 10px; margin-left: 0px"></asp:Label>
                        <asp:TextBox ID="txtStartDate" class="date" runat="server" AutoCompleteType="Disabled"></asp:TextBox>
                    </li>
                    <li>
                        <asp:Label ID="lblEndDate" runat="server" Text="End Date" class="badge badge-primary" Style="width: 200px; height: 30px; font-size: large; margin: 10px; margin-left: 0px"></asp:Label>
                        <asp:TextBox ID="txtEndDate" class="date" runat="server" AutoCompleteType="Disabled"></asp:TextBox>
                    </li>
                    <li>
                        <asp:Button ID="btnSearchByDate" class="btn btn-outline-success" runat="server" Text="Search by Date" OnClick="SearchButtonByDate_Click"></asp:Button>
                        <asp:Button ID="btnClear" class="btn btn-outline-success" runat="server" Text="Refresh" OnClick="ClearButton_Click"></asp:Button>
                    </li>
                </ul>
                <asp:Label ID="lblSuccessMessage" Text="" runat="server" ForeColor="Green" />
                <br />
                <asp:Label ID="lblErrorMessage" Text="" runat="server" ForeColor="Red" />
            </div>
            <div style="width: 50%; height: auto; float: left">
                <ul style="list-style-type: none;">
                    <li>
                        <asp:Button ID="btnPrint" class="btn btn-outline-success" Style="width: 120px" runat="server" Text="Print" OnClientClick="doPrint()"></asp:Button>
                        <asp:Button ID="btnDownload" class="btn btn-outline-success" runat="server" Text="Download" OnClick="btnExportToPDF_Click"></asp:Button>
                    </li>
                    <li>
                        <asp:Label ID="lblStartDateProduct" runat="server" Text="Start Date" class="badge badge-primary" Style="width: 200px; height: 30px; font-size: large; margin: 10px; margin-left: 0px"></asp:Label>
                        <asp:TextBox ID="txtStartDateProduct" class="date" runat="server" AutoCompleteType="Disabled"></asp:TextBox>
                    </li>
                    <li>
                        <asp:Label ID="lblEndDateProduct" runat="server" Text="End Date" class="badge badge-primary" Style="width: 200px; height: 30px; font-size: large; margin: 10px; margin-left: 0px"></asp:Label>
                        <asp:TextBox ID="txtEndDateProduct" class="date" runat="server" AutoCompleteType="Disabled"></asp:TextBox>
                    </li>
                    <li>
                        <asp:Label ID="lblName" runat="server" Text="Name" class="badge badge-primary" Style="width: 200px; height: 30px; font-size: large; margin: 10px; margin-left: 0px"></asp:Label>
                        <asp:TextBox ID="txtName" runat="server" AutoCompleteType="Disabled"></asp:TextBox>
                    </li>
                    <li>
                        <asp:Button ID="btnSearchByProduct" class="btn btn-outline-success" runat="server" Text="Search by Product" OnClick="SearchButtonByProduct_Click"></asp:Button>
                        <asp:Button ID="btnRefresh" class="btn btn-outline-success" runat="server" Text="Refresh" OnClick="ClearButton_Click"></asp:Button>
                    </li>
                </ul>
                <asp:Label ID="lblSuccessProduct" Text="" runat="server" ForeColor="Green" />
                <br />
                <asp:Label ID="lblErrorProduct" Text="" runat="server" ForeColor="Red" />
            </div>
        </div>
        <div style="text-align: center; margin-right: auto; margin-left: auto">
            <asp:Label ID="lblDisplayReport" Text="" runat="server" ForeColor="Red" />
        </div>
        <br />
        <div class="container-fluid" style="width: 100%; height: 500px; overflow: auto">

            <asp:GridView ID="gvFixedCostReport" runat="server" AutoGenerateColumns="false" ShowFooter="true" DataKeyNames="fixed_id" ShowHeaderWhenEmpty="true" Width="100%"
                OnRowCancelingEdit="gvFixedCostReport_RowCancelingEdit" OnRowDeleting="gvFixedCostReport_RowDeleting" OnRowEditing="gvFixedCostReport_RowEditing" OnRowUpdating="gvFixedCostReport_RowUpdating"
                
                BackColor="White" BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px" CellPadding="3">
                <%--Theme Properties--%>
                <FooterStyle BackColor="White" ForeColor="#000066" />
                <HeaderStyle BackColor="#006699" Font-Bold="True" ForeColor="White" />
                <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Left" />
                <RowStyle ForeColor="#000066" />
                <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
                <SortedAscendingCellStyle BackColor="#F1F1F1" />
                <SortedAscendingHeaderStyle BackColor="#007DBB" />
                <SortedDescendingCellStyle BackColor="#CAC9C9" />
                <SortedDescendingHeaderStyle BackColor="#00547E" />

                <Columns>
                    <asp:TemplateField HeaderText="Date">
                        <ItemTemplate>
                            <asp:Label Text='<%#Eval("fixed_cost_date") %>' runat="server"></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtDate" class="date" Text='<%#Eval("fixed_cost_date") %>' runat="server"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Name">
                        <ItemTemplate>
                            <asp:Label Text='<%#Eval("fixed_cost_name") %>' runat="server"></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtName" Text='<%#Eval("fixed_cost_name") %>' runat="server"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Amount">
                        <ItemTemplate>
                            <asp:Label Text='<%#Eval("amount", "{0:N2}") %>' runat="server"></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtAmount" Text='<%#Eval("amount", "{0:N2}") %>' runat="server"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Payment Type">
                        <ItemTemplate>
                            <asp:Label Text='<%#Eval("payment_type") %>' runat="server"></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtPaymentType" Text='<%#Eval("payment_type") %>' runat="server"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Receipt No">
                        <ItemTemplate>
                            <asp:Label Text='<%#Eval("receipt_no") %>' runat="server"></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtReceiptNo" Text='<%#Eval("receipt_no") %>' runat="server"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Cheque No">
                        <ItemTemplate>
                            <asp:Label Text='<%#Eval("cheque_no") %>' runat="server"></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtChequeNo" Text='<%#Eval("cheque_no") %>' runat="server"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Issued Date">
                        <ItemTemplate>
                            <asp:Label Text='<%#Eval("issued_date") %>' runat="server"></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtIssuedDate" class="date" Text='<%#Eval("issued_date") %>' runat="server"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Remark">
                        <ItemTemplate>
                            <asp:Label Text='<%#Eval("remark") %>' runat="server"></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtRemark" Text='<%#Eval("remark") %>' runat="server"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:ImageButton ImageUrl="~/picture/edit.png" runat="server" CommandName="Edit" ToolTip="Edit" Width="20px" Height="20px"  OnClientClick="return confirm('Are you sure to edit it?');"/>
                            <asp:ImageButton ImageUrl="~/picture/delete.png" runat="server" CommandName="Delete" OnClientClick="return confirm('Are you sure to delete it?');" ToolTip="Delete" Width="20px" Height="20px" />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:ImageButton ImageUrl="~/picture/save.png" runat="server" CommandName="Update" OnClientClick="return confirm('Are you sure to save it?');" ToolTip="Update" Width="20px" Height="20px" />
                            <asp:ImageButton ImageUrl="~/picture/cancel.png" runat="server" CommandName="Cancel" OnClientClick="return confirm('Are you sure to cancel editing it?');" ToolTip="Cancel" Width="20px" Height="20px" />
                        </EditItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
        <script>
            $(document).ready(function () {
                $(function () {
                    $('.date').datepicker(
                        {
                            numberOfMonths: 1,
                            changeYear: true,
                            changeMonth: true,
                            showWeek: true,
                            weekHeader: "Week no",
                            showOtherMonths: true,
                            minDate: new Date(1900, 0, 1),
                            maxDate: new Date(2050, 11, 31),
                        });
                });
            });
        </script>
        <script>
            var ds = null;
            ds = <%=Name %>
                $("#txtName").autocomplete(
                    {
                        source: ds
                    },
                    {
                        autoFocus: true,
                        delay: 0,
                        minLength: 1
                    })
        </script>
        <script>
            $(document).ready(function () {
                $('.onlymonth').datepicker({
                    changeMonth: true,
                    changeYear: true,
                    showButtonPanel: true,
                    onClose: function (dateText, inst) {
                        $(this).datepicker('setDate', new Date(inst.selectedYear, inst.selectedMonth, 1));
                    }
                });
            });
        </script>
        <script>
            function doPrint() {
                var prtContent = document.getElementById('<%= gvFixedCostReport.ClientID %>');
                prtContent.border = 0; //set no border here
                var header = "<div style='float:left; width:100%'><h2 style='margin:0px'>Smoke Music Cafe</h2><p>'Hakam Foundation', Top Floor<br>House-98, Road-11, Block-C<br>Banani C/A, DHAKA-1213<br>(+88) 01750 999 333, (+88) 01750 999 444<br>info@smoke-cafe.com</p></div>";
                var heading = "<div style=' text-align: center; width:100%'><h3>Monthly Fixed Cost Record's</h3></div>";
                var total_amount = "<div><h3>In Words(Taka): <%=InWords %> Taka only.</h3></div>";
                var para = "<p>Looking for your kind consideration.</p>";
                var approve = "<p style='text-align: center'><b>Approved By</b></p><br><br>";
                var sign = "<div style='width:100%'><ul style='list-style-type: none; float:left; width:33%'><li>Signature Of Manager(Accounts)</li></ul><ul style='list-style-type: none; float:left; width:33%'><li>Head Of Operation</li></ul><ul style='list-style-type: none; float:left'><li>CEO</li></ul></div><br>";
                var footer = "<br><p style='text-align:center'>www.smoke-cafe.com</p>";
                var WinPrint = window.open('', '', 'left=100,top=100,width=1000,height=1000,toolbar=0,scrollbars=1,status=0,resizable=1');
                WinPrint.document.write(header);
                WinPrint.document.write(heading);
                WinPrint.document.write(prtContent.outerHTML);
                WinPrint.document.write(total_amount);
                WinPrint.document.write(para);
                WinPrint.document.write(approve);
                WinPrint.document.write(sign);
                WinPrint.document.write(footer);
                WinPrint.document.close();
                WinPrint.focus();
                WinPrint.print();
                WinPrint.close();
            }
        </script>
    </form>
</body>
</html>
