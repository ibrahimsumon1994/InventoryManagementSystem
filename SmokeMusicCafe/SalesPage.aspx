<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SalesPage.aspx.cs" Inherits="SmokeMusicCafe.SalesPage" %>

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
<body>
    <form id="form1" runat="server">
        <div class="container-fluid" style="width:100%; height:auto">
            <asp:Label ID="lblHeadingPage" runat="server" Text="Input Page Of Sales Information" class="badge badge-primary" style="width:100%; height:30px; font-size:large; margin:10px; margin-left:0px"></asp:Label>
            <ul style="list-style-type:none;">
                <li>
                    <asp:Button ID="btnPreviousPage" class="btn btn-outline-success" runat="server" Text="Back" PostBackUrl="Dashboard.aspx"></asp:Button>
                    <asp:Button ID="btnSalesReport" class="btn btn-outline-success" style="float: right; margin-right: 100px; margin-top:60px; width:230px; height:70px" runat="server" Text="Sales Report" PostBackUrl="SalesReport.aspx"></asp:Button>              
                </li>
                <li>
                    <asp:Label ID="lblDate" runat="server" Text="Date" class="badge badge-primary" style="width:200px; height:30px; font-size:large; margin:10px; margin-left:0px"></asp:Label>
                    <asp:TextBox ID="txtDate" class="clsDate" runat="server" AutoCompleteType="Disabled"></asp:TextBox>
                    <asp:Label ID="errorDate" Text="" runat="server" ForeColor="red" />
                </li>
                <li>
                    <asp:Label ID="lblCash" runat="server" Text="Cash" class="badge badge-primary" style="width:200px; height:30px; font-size:large; margin:10px; margin-left:0px"></asp:Label>
                    <asp:RadioButton ID="rbCash" runat="server" AutoPostBack="true" GroupName="grp1" Text="Yes" style="margin:10px" OnCheckedChanged="rb_CashChanged"></asp:RadioButton>
                    <asp:RadioButton ID="rbNoCash" runat="server" AutoPostBack="true" GroupName="grp1" Text="No" OnCheckedChanged="rb_CashChanged"></asp:RadioButton>
                    <asp:Label ID="lblCashAmount" runat="server" Text="Cash Amount" class="badge badge-primary" style="width:210px; height:30px; font-size:large; margin:10px; margin-left: 150px" Visible="false"></asp:Label>
                    <asp:TextBox ID="txtCashAmount" runat="server" Visible="false" AutoCompleteType="Disabled"></asp:TextBox>
                </li>
                <li>
                    <asp:Label ID="lblBank" runat="server" Text="Bank" class="badge badge-primary" style="width:200px; height:30px; font-size:large; margin:10px; margin-left:0px"></asp:Label>
                    <asp:RadioButton ID="rbBank" runat="server" AutoPostBack="true" GroupName="grp2" Text="Yes" style="margin:10px" OnCheckedChanged="rb_BankChanged"></asp:RadioButton>
                    <asp:RadioButton ID="rbNoBank" runat="server" AutoPostBack="true" GroupName="grp2" Text="No" OnCheckedChanged="rb_BankChanged"></asp:RadioButton>
                    <asp:Label ID="lblBankAmount" runat="server" Text="Bank Amount" class="badge badge-primary" style="width:210px; height:30px; font-size:large; margin:10px; margin-left: 150px" Visible="false"></asp:Label>
                    <asp:TextBox ID="txtBankAmount" runat="server" Visible="false" AutoCompleteType="Disabled"></asp:TextBox>
                </li>
                <li>
                    <asp:Label ID="lblVat" runat="server" Text="Vat" class="badge badge-primary" style="width:200px; height:30px; font-size:large; margin:10px; margin-left:0px"></asp:Label>
                    <asp:RadioButton ID="rbVat" runat="server" AutoPostBack="true" GroupName="grp3" Text="Yes" style="margin:10px" OnCheckedChanged="rb_VatChanged"></asp:RadioButton>
                    <asp:RadioButton ID="rbNoVat" runat="server" AutoPostBack="true" GroupName="grp3" Text="No" OnCheckedChanged="rb_VatChanged"></asp:RadioButton>
                    <asp:Label ID="lblVatAmount" runat="server" Text="Vat Amount" class="badge badge-primary" style="width:210px; height:30px; font-size:large; margin:10px; margin-left: 150px" Visible="false"></asp:Label>
                    <asp:TextBox ID="txtVatAmount" runat="server" Visible="false" AutoCompleteType="Disabled"></asp:TextBox>
                </li>
                <li>
                    <asp:Label ID="lblServiceCharge" runat="server" Text="Service Charge" class="badge badge-primary" style="width:200px; height:30px; font-size:large; margin:10px; margin-left:0px"></asp:Label>
                    <asp:RadioButton ID="rbServiceCharge" runat="server" AutoPostBack="true" GroupName="grp4" Text="Yes" style="margin:10px" OnCheckedChanged="rb_ServiceChargeChanged"></asp:RadioButton>
                    <asp:RadioButton ID="rbNoServiceCharge" runat="server" AutoPostBack="true" GroupName="grp4" Text="No" OnCheckedChanged="rb_ServiceChargeChanged"></asp:RadioButton>
                    <asp:Label ID="lblServiceChargeAmount" runat="server" Text="Service Charge Amount" class="badge badge-primary" style="width:210px; height:30px; font-size:large; margin:10px; margin-left: 150px" Visible="false"></asp:Label>
                    <asp:TextBox ID="txtServiceChargeAmount" runat="server" Visible="false" AutoCompleteType="Disabled"></asp:TextBox>
                </li>
                <li>
                    <asp:Label ID="lblRemark" runat="server" Text="Remark" class="badge badge-primary" style="width:200px; height:30px; font-size:large; margin:10px; margin-left:0px"></asp:Label>
                    <asp:TextBox ID="txtRemark" runat="server" AutoCompleteType="Disabled"></asp:TextBox>
                </li>
                <li>
                    <asp:Button ID="AddButton" class="btn btn-outline-success" runat="server" Text="ADD" OnClick="AddButton_Click"></asp:Button>
                    <asp:Button ID="ClearButton" class="btn btn-outline-success" runat="server" Text="Clear" OnClick="ClearButton_Click"></asp:Button>
                </li>
            </ul>
        </div>
        <asp:Label ID="lblSuccessMessage" Text="" runat="server" ForeColor="Green" />
        <br />
        <asp:Label ID="lblErrorMessage" Text="" runat="server" ForeColor="Red" />
        <br />
        <div class="container-fluid" style="width:100%; height:200px; overflow:auto">
                 
            <asp:GridView ID="gvSales" runat="server" AutoGenerateColumns="false" ShowFooter="true" DataKeyNames="salesID" ShowHeaderWhenEmpty="true" Width="100%"
                OnRowCancelingEdit="gvSales_RowCancelingEdit" OnRowDeleting="gvSales_RowDeleting" OnRowEditing="gvSales_RowEditing" OnRowUpdating="gvSales_RowUpdating"
                
                BackColor="White" BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px" CellPadding="3">
                <%--Theme Properties--%>
                <FooterStyle BackColor="White" ForeColor="#000066" />
                <HeaderStyle BackColor="#006699" Font-Bold="True" ForeColor="White"/>
                <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Left" />
                <RowStyle ForeColor="#000066" />
                <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
                <SortedAscendingCellStyle BackColor="#F1F1F1" />
                <SortedAscendingHeaderStyle BackColor="#007DBB" />
                <SortedDescendingCellStyle BackColor="#CAC9C9" />
                <SortedDescendingHeaderStyle BackColor="#00547E" />

                <Columns>
                    <asp:TemplateField HeaderText="Sales Date">
                        <ItemTemplate>
                            <asp:Label Text='<%#Eval("sales_date") %>' runat="server"></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtSalesDate" class="date" Text='<%#Eval("sales_date") %>' runat="server"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Cash Amount">
                        <ItemTemplate>
                            <asp:Label Text='<%#Eval("cash_amount") %>' runat="server"></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtCashAmount" Text='<%#Eval("cash_amount") %>' runat="server"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Bank Amount">
                        <ItemTemplate>
                            <asp:Label Text='<%#Eval("bank_amount") %>' runat="server"></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtBankAmount" Text='<%#Eval("bank_amount") %>' runat="server"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Vat Amount">
                        <ItemTemplate>
                            <asp:Label Text='<%#Eval("vat_amount") %>' runat="server"></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtVatAmount" Text='<%#Eval("vat_amount") %>' runat="server"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Service Charge Amount">
                        <ItemTemplate>
                            <asp:Label Text='<%#Eval("service_charge_amount") %>' runat="server"></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtServiceChargeAmount" Text='<%#Eval("service_charge_amount") %>' runat="server"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Total Amount">
                        <ItemTemplate>
                            <asp:Label Text='<%#Eval("total_amount", "{0:N2}") %>' runat="server"></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtTotalAmount" Text='<%#Eval("total_amount", "{0:C}") %>' runat="server"></asp:TextBox>
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
    </form>
    <script>
        $(document).ready(function () {
            $(function () {
                $(".clsDate").datepicker(
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
                $(".clsDate").blur(function () {
                    val = $(this).val();
                    val1 = Date.parse(val);
                    if (isNaN(val1) == true && val !== '') {
                        $("#errorDate").text('Date is not valid');
                    }
                    else {
                        $("#errorDate").text('');
                    }
                });
            });
        });
    </script>
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
                $(".date").blur(function () {
                    val = $(this).val();
                    val1 = Date.parse(val);
                    if (isNaN(val1) == true && val !== '') {
                        $("#lblErrorMessage").text('Date is not valid');
                    }
                    else {
                        $("#lblErrorMessage").text('');
                    }
                });
            });
        });
    </script>
    <script>
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(getme);
        function getme() {
            $('.date').datepicker({
                numberOfMonths: 1,
                changeYear: true,
                changeMonth: true,
                showWeek: true,
                weekHeader: "Week no",
                showOtherMonths: true,
                minDate: new Date(1900, 0, 1),
                maxDate: new Date(2050, 11, 31),
            });
            $(".date").blur(function () {
                val = $(this).val();
                val1 = Date.parse(val);
                if (isNaN(val1) == true && val !== '') {
                    $("#lblErrorMessage").text('Date is not valid');
                }
                else {
                    $("#lblErrorMessage").text('');
                }
            });
        }
    </script>
</body>
</html>
