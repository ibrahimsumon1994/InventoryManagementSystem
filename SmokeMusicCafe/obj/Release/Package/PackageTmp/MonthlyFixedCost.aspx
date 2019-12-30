<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MonthlyFixedCost.aspx.cs" Inherits="SmokeMusicCafe.MonthlyFixedCost" %>

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
<body style="background-image:url(picture1/background.jpeg); background-repeat: no-repeat; background-size:cover">
    
    <form id="form1" runat="server">        
        <div class="container-fluid" style="width:100%; height:auto">
            <ul style="list-style-type:none;">
                <li>
                    <asp:Button ID="btnPreviousPage" class="btn btn-outline-success" runat="server" Text="Back" PostBackUrl="ProductPurchase.aspx"></asp:Button>
                    <asp:Button ID="btnMonthlyFixedCostReport" class="btn btn-outline-success" style="float: right; margin-right: 100px; margin-top:60px; width:230px; height:70px" runat="server" Text="Fixed Cost Report" PostBackUrl="FixedCostReport.aspx"></asp:Button>
                </li>
                <li>
                    <asp:Label ID="lblDate" runat="server" Text="Date" class="badge badge-primary" style="width:200px; height:30px; font-size:large; margin:10px; margin-left:0px"></asp:Label>
                    <asp:TextBox ID="txtDate" class="clsDate" runat="server" AutoCompleteType="Disabled"></asp:TextBox>
                    <asp:Label ID="errorDate" Text="" runat="server" ForeColor="red" />
                </li>
                <li>
                    <asp:Label ID="lblName" runat="server" Text="Name" class="badge badge-primary" style="width:200px; height:30px; font-size:large; margin:10px; margin-left:0px"></asp:Label>
                    <asp:TextBox ID="txtName" runat="server"></asp:TextBox>
                    <%--<asp:DropDownList ID="drpItemName" runat="server" style="width:180px; font-size:medium">
                        
                    </asp:DropDownList>--%>

                </li>
                <li>
                    <asp:Label ID="lblAmount" runat="server" Text="Amount" class="badge badge-primary" style="width:200px; height:30px; font-size:large; margin:10px; margin-left:0px"></asp:Label>
                    <asp:TextBox ID="txtAmount" runat="server" AutoCompleteType="Disabled"></asp:TextBox>
                </li>
                <li>
                    <asp:Label ID="lblPaymentType" runat="server" Text="Payment Type" class="badge badge-primary" style="width:200px; height:30px; font-size:large; margin:10px; margin-left:0px"></asp:Label>
                    <asp:RadioButton ID="rbCash" runat="server" AutoPostBack="true" GroupName="grp1" Text="Cash" style="margin:10px" OnCheckedChanged="rb_CheckedChanged"></asp:RadioButton>
                    <asp:RadioButton ID="rbCheque" runat="server" AutoPostBack="true" GroupName="grp1" Text="Cheque" OnCheckedChanged="rb_CheckedChanged"></asp:RadioButton>
                    <asp:Label ID="lblRemark" runat="server" Text="Remark" class="badge badge-primary" style="width:200px; height:30px; font-size:large; margin:10px; margin-left:135px" Enabled="false"></asp:Label>
                    <asp:TextBox ID="txtRemark" runat="server" Enabled="false" AutoCompleteType="Disabled"></asp:TextBox>
                </li>
                <li>
                    <asp:Label ID="lblReceiptNo" runat="server" Text="Receipt No" class="badge badge-primary" style="width:200px; height:30px; font-size:large; margin:10px; margin-left:0px"></asp:Label>
                    <asp:TextBox ID="txtReceiptNo" runat="server" AutoCompleteType="Disabled"></asp:TextBox>
                    <asp:Label ID="lblChequeNo" runat="server" Text="Cheque Number" class="badge badge-primary" style="width:200px; height:30px; font-size:large; margin:10px; margin-left: 84px" Enabled="false"></asp:Label>
                    <asp:TextBox ID="txtChequeNo" runat="server" Enabled="false" AutoCompleteType="Disabled"></asp:TextBox>
                    <asp:Label ID="lblChequeIssuedDate" runat="server" Text="Issued Date" class="badge badge-primary" style="width:200px; height:30px; font-size:large; margin:10px; margin-left: 84px" Enabled="false"></asp:Label>
                    <asp:TextBox ID="txtChequeIssuedDate" runat="server" class="clsDate" Enabled="false" AutoCompleteType="Disabled"></asp:TextBox>
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
                 
            <asp:GridView ID="gvMonthlyFixedCost" runat="server" AutoGenerateColumns="false" ShowFooter="true" DataKeyNames="fixed_id" ShowHeaderWhenEmpty="true" Width="100%"
                OnRowCancelingEdit="gvMonthlyFixedCost_RowCancelingEdit" OnRowDeleting="gvMonthlyFixedCost_RowDeleting" OnRowEditing="gvMonthlyFixedCost_RowEditing" OnRowUpdating="gvMonthlyFixedCost_RowUpdating"
                

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
                            <asp:TextBox ID="txtDate" class="clsDate" Text='<%#Eval("fixed_cost_date") %>' runat="server"></asp:TextBox>
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
                            <asp:Label Text='<%#Eval("amount") %>' runat="server"></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtAmount" Text='<%#Eval("amount") %>' runat="server"></asp:TextBox>
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
                            <asp:ImageButton ImageUrl="~/picture/edit.png" runat="server" CommandName="Edit" ToolTip="Edit" Width="20px" Height="20px" />
                            <asp:ImageButton ImageUrl="~/picture/delete.png" runat="server" CommandName="Delete" ToolTip="Delete" Width="20px" Height="20px" />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:ImageButton ImageUrl="~/picture/save.png" runat="server" CommandName="Update" ToolTip="Update" Width="20px" Height="20px" />
                            <asp:ImageButton ImageUrl="~/picture/cancel.png" runat="server" CommandName="Cancel" ToolTip="Cancel" Width="20px" Height="20px" />
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
</body>

    <script>
        var ds=null;
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
</html>

