<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProductPurchase.aspx.cs" Inherits="SmokeMusicCafe.ProductPurchase" %>

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
            <asp:Label ID="lblHeadingPage" runat="server" Text="Input Page Of Purchase Information" class="badge badge-primary" style="width:100%; height:30px; font-size:large; margin:10px; margin-left:0px"></asp:Label>
            <ul style="list-style-type:none;">
                <li>
                    <asp:Button ID="btnPreviousPage" class="btn btn-outline-success" runat="server" Text="Back" PostBackUrl="Dashboard.aspx"></asp:Button>
                    <asp:Button ID="btnPurchaseReport" class="btn btn-outline-success" style="float: right; margin-right: 50px; width:230px; height:70px" runat="server" Text="Purchase Report" PostBackUrl="PurchaseReport.aspx"></asp:Button>
                    <asp:Button ID="btnMonthlyFixedCost" class="btn btn-outline-success" style="float: right; margin-right: 50px; width:230px; height:70px" runat="server" Text="Other Cost" PostBackUrl="MonthlyFixedCost.aspx"></asp:Button>
                    <asp:Button ID="btnVendorInformation" class="btn btn-outline-success" style="float: right; margin-right: 50px; width:230px; height:70px" runat="server" Text="Vendor Information" PostBackUrl="VendorInformation.aspx"></asp:Button>
                </li>
                <li>
                    <asp:Label ID="lblDate" runat="server" Text="Date" class="badge badge-primary" style="width:200px; height:30px; font-size:large; margin:10px; margin-left:0px"></asp:Label>
                    <asp:TextBox ID="txtDate" class="clsDate" runat="server" AutoCompleteType="Disabled"></asp:TextBox>
                    <asp:Label ID="errorDate" Text="" runat="server" ForeColor="red" />
                </li>
                <li>
                    <asp:Label ID="lblVendor" runat="server" Text="Vendor" class="badge badge-primary" style="width:200px; height:30px; font-size:large; margin:10px; margin-left:0px"></asp:Label>
                    <asp:RadioButton ID="rbVendor" runat="server" AutoPostBack="true" GroupName="grp3" Text="Vendor" style="margin:10px" OnCheckedChanged="rb_VendorChanged"></asp:RadioButton>
                    <asp:RadioButton ID="rbNoVendor" runat="server" AutoPostBack="true" GroupName="grp3" Text="No Vendor" OnCheckedChanged="rb_VendorChanged"></asp:RadioButton>
                    <asp:Label ID="lblVendorName" runat="server" Text="Vendor Name" class="badge badge-primary" style="width:200px; height:30px; font-size:large; margin:10px; margin-left: 100px" Visible="false"></asp:Label>
                    <asp:TextBox ID="txtVendorName" class="vendorName" runat="server" Visible="false" AutoCompleteType="Disabled"></asp:TextBox>
                    <asp:Label ID="errorVendorName" Text="" runat="server" ForeColor="red" />
                </li>
                <li>
                    <asp:Label ID="lblItemName" runat="server" Text="Items Name" class="badge badge-primary" style="width:200px; height:30px; font-size:large; margin:10px; margin-left:0px"></asp:Label>
                    <asp:TextBox ID="txtItemName" runat="server"></asp:TextBox>
                    <asp:Label ID="errorItemName" Text="" runat="server" ForeColor="red" />
                </li>
                <li>
                    <asp:Label ID="lblUnitType" runat="server" Text="Unit Type" class="badge badge-primary" style="width:200px; height:30px; font-size:large; margin:10px; margin-left:0px"></asp:Label>
                    <asp:DropDownList ID="drpUnitType" runat="server" style="width:180px; font-size:medium">
                        <asp:ListItem Text="Select a Unit" Value="0"></asp:ListItem>
                        <asp:ListItem Text="Pieces" Value="1"></asp:ListItem>
                        <asp:ListItem Text="Kg" Value="2"></asp:ListItem>
                        <asp:ListItem Text="Rim" Value="3"></asp:ListItem>
                        <asp:ListItem Text="Litre" Value="4"></asp:ListItem>
                        <asp:ListItem Text="Others" Value="5"></asp:ListItem>
                    </asp:DropDownList>
                </li>
                <li>
                    <asp:Label ID="lblQyantity" runat="server" Text="Quantity" class="badge badge-primary" style="width:200px; height:30px; font-size:large; margin:10px; margin-left:0px"></asp:Label>
                    <asp:TextBox ID="txtQuantity" runat="server" AutoCompleteType="Disabled"></asp:TextBox>
                </li>
                <li>
                    <asp:Label ID="lblRate" runat="server" Text="Rate" class="badge badge-primary" style="width:200px; height:30px; font-size:large; margin:10px; margin-left:0px"></asp:Label>
                    <asp:TextBox ID="txtRate" runat="server" AutoCompleteType="Disabled"></asp:TextBox>
                </li>
                <li>
                    <asp:Label ID="lblVatType" runat="server" Text="Vat/No Vat" class="badge badge-primary" style="width:200px; height:30px; font-size:large; margin:10px; margin-left:0px"></asp:Label>
                    <asp:RadioButton ID="rbVat" runat="server" AutoPostBack="true" GroupName="grp2" Text="Vat" style="margin:10px" OnCheckedChanged="rb_VatChanged"></asp:RadioButton>
                    <asp:RadioButton ID="rbNoVat" runat="server" AutoPostBack="true" GroupName="grp2" Text="No Vat" OnCheckedChanged="rb_VatChanged"></asp:RadioButton>
                    <asp:Label ID="lblVatAmount" runat="server" Text="Vat Amount" class="badge badge-primary" style="width:200px; height:30px; font-size:large; margin:10px; margin-left: 150px" Visible="false"></asp:Label>
                    <asp:TextBox ID="txtVatAmount" runat="server" Visible="false" AutoCompleteType="Disabled"></asp:TextBox>
                </li>
                <li>
                    <asp:Label ID="lblPaymentType" runat="server" Text="Payment Type" class="badge badge-primary" style="width:200px; height:30px; font-size:large; margin:10px; margin-left:0px"></asp:Label>
                    <asp:RadioButton ID="rbCash" runat="server" AutoPostBack="true" GroupName="grp1" Text="Cash" style="margin:10px" OnCheckedChanged="rb_CheckedChanged"></asp:RadioButton>
                    <asp:RadioButton ID="rbCheque" runat="server" AutoPostBack="true" GroupName="grp1" Text="Cheque" OnCheckedChanged="rb_CheckedChanged"></asp:RadioButton>
                    <asp:RadioButton ID="rbPoint" runat="server" AutoPostBack="true" GroupName="grp1" Text="Point" OnCheckedChanged="rb_CheckedChanged"></asp:RadioButton>
                    <asp:RadioButton ID="rbOthers" runat="server" AutoPostBack="true" GroupName="grp1" Text="Others" OnCheckedChanged="rb_CheckedChanged"></asp:RadioButton>
                    <asp:Label ID="lblRemark" runat="server" Text="Remark" class="badge badge-primary" style="width:200px; height:30px; font-size:large; margin:10px; margin-left:15px" Visible="false"></asp:Label>
                    <asp:TextBox ID="txtRemark" runat="server" Visible="false" AutoCompleteType="Disabled"></asp:TextBox>
                </li>
                <li>
                    <asp:Label ID="lblReceiptNo" runat="server" Text="Receipt No" class="badge badge-primary" style="width:200px; height:30px; font-size:large; margin:10px; margin-left:0px"></asp:Label>
                    <asp:TextBox ID="txtReceiptNo" runat="server" AutoCompleteType="Disabled"></asp:TextBox>
                    <asp:Label ID="lblChequeNo" runat="server" Text="Cheque Number" class="badge badge-primary" style="width:200px; height:30px; font-size:large; margin:10px; margin-left: 84px" Visible="false"></asp:Label>
                    <asp:TextBox ID="txtChequeNo" runat="server" Visible="false" AutoCompleteType="Disabled"></asp:TextBox>
                    <asp:Label ID="lblChequeIssuedDate" runat="server" Text="Issued Date" class="badge badge-primary" style="width:200px; height:30px; font-size:large; margin:10px; margin-left: 10px" Visible="false"></asp:Label>
                    <asp:TextBox ID="txtChequeIssuedDate" runat="server" class="clsDate" Visible="false" AutoCompleteType="Disabled"></asp:TextBox>
                </li>
                <li>
                    <asp:Button ID="AddButton" class="btn btn-outline-success" runat="server" Text="ADD" OnClick="AddButton_Click"></asp:Button>
                    <asp:Button ID="ClearButton" class="btn btn-outline-success" runat="server" Text="Clear" OnClick="ClearButton_Click"></asp:Button>
                    <asp:Label ID="lblSuccessMessage" Text="" runat="server" ForeColor="Green" />
                    <asp:Label ID="lblErrorMessage" Text="" runat="server" ForeColor="Red" />
                </li>
            </ul>
        </div>
        <div class="container-fluid" style="width:100%; height:200px; overflow:auto">
                 
            <asp:GridView ID="gvPurchase" runat="server" AutoGenerateColumns="false" ShowFooter="true" DataKeyNames="purchaseID" ShowHeaderWhenEmpty="true" Width="100%"
                OnRowCancelingEdit="gvPurchase_RowCancelingEdit" OnRowDeleting="gvPurchase_RowDeleting" OnRowEditing="gvPurchase_RowEditing" OnRowUpdating="gvPurchase_RowUpdating"
                
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
                    <asp:TemplateField HeaderText="Purchase Date">
                        <ItemTemplate>
                            <asp:Label Text='<%#Eval("purchase_date") %>' runat="server"></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtPurchaseDate" class="date" Text='<%#Eval("purchase_date") %>' runat="server"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Vendor Name">
                        <ItemTemplate>
                            <asp:Label Text='<%#Eval("vendor_name") %>' runat="server"></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtVendorName" class="vendorName" Text='<%#Eval("vendor_name") %>' runat="server"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Items Name">
                        <ItemTemplate>
                            <asp:Label Text='<%#Eval("item_name") %>' runat="server"></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtItemName" Text='<%#Eval("item_name") %>' runat="server"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Unit Type">
                        <ItemTemplate>
                            <asp:Label Text='<%#Eval("unit_type") %>' runat="server"></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtUnitType" Text='<%#Eval("unit_type") %>' runat="server"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Quantity">
                        <ItemTemplate>
                            <asp:Label Text='<%#Eval("quantity") %>' runat="server"></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtQuantity" Text='<%#Eval("quantity") %>' runat="server"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Rate">
                        <ItemTemplate>
                            <asp:Label Text='<%#Eval("rate") %>' runat="server"></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtRate" Text='<%#Eval("rate") %>' runat="server"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Vat">
                        <ItemTemplate>
                            <asp:Label Text='<%#Eval("vat") %>' runat="server"></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtVat" Text='<%#Eval("vat") %>' runat="server"></asp:TextBox>
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
        var isChanged = false;
        $(function () {
            $('#drpUnitType').focusin(function () {
                if (!isChanged) {
                    // this removes the first item which is your placeholder if it is never changed
                    $(this).find('option:first').hide();
                }
            });
            $('#drpUnitType').change(function () {
                // this marks the selection to have changed
                isChanged = true;
            });
            $('#drpUnitType').focusout(function () {
                if (!isChanged) {
                    // if the control loses focus and there is no change in selection, return the first item
                    $(this).show('<option selected="selected" value="0">Select a Unit</option>');
                }
            });
        });
    </script>
</body>

    <script>
        var item=null;
        item = <%=itemName %>
            $("#txtItemName").autocomplete(
            {
                source: item
            },
            {
                autoFocus: true,
                delay: 0,
                minLength: 1
            })
    </script>

    <script>
        var vendor=null;
        vendor = <%=VendorName %>
            $(".vendorName").autocomplete(
            {
                source: vendor
            },
            {
                autoFocus: true,
                delay: 0,
                minLength: 1
            })
    </script>
</html>

