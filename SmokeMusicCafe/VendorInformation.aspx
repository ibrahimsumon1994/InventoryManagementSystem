<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VendorInformation.aspx.cs" Inherits="SmokeMusicCafe.VendorInformation" %>

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
        <div class="container-fluid" style="width: 100%; height: 300px">
            <asp:Label ID="lblHeadingPage" runat="server" Text="Vendor Information Page" class="badge badge-primary" style="width:100%; height:30px; font-size:large; margin:10px; margin-left:0px"></asp:Label>
            <div style="width: 40%; height: 100%; float: left">
                <ul style="list-style-type: none;">
                    <li>
                        <asp:Button ID="btnPreviousPage" class="btn btn-outline-success" runat="server" Text="Back" PostBackUrl="ProductPurchase.aspx"></asp:Button>
                    </li>
                    <li>
                        <asp:Label ID="lblStartDateVendor" runat="server" Text="Start Date" class="badge badge-primary" Style="width: 200px; height: 30px; font-size: large; margin: 10px; margin-left: 0px"></asp:Label>
                        <asp:TextBox ID="txtStartDateVendor" class="date" runat="server" AutoCompleteType="Disabled"></asp:TextBox>
                    </li>
                    <li>
                        <asp:Label ID="lblEndDateVendor" runat="server" Text="End Date" class="badge badge-primary" Style="width: 200px; height: 30px; font-size: large; margin: 10px; margin-left: 0px"></asp:Label>
                        <asp:TextBox ID="txtEndDateVendor" class="date" runat="server" AutoCompleteType="Disabled"></asp:TextBox>
                    </li>
                    <li>
                        <asp:Label ID="lblVendorNameByDate" runat="server" Text="Vendor Name" class="badge badge-primary" Style="width: 200px; height: 30px; font-size: large; margin: 10px; margin-left: 0px"></asp:Label>
                        <asp:TextBox ID="txtVendorNameByDate" class="vendorName" runat="server" AutoCompleteType="Disabled"></asp:TextBox>
                    </li>
                    <li>
                        <asp:Button ID="btnSearchByVendor" class="btn btn-outline-success" runat="server" Text="Search by Vendor" OnClick="SearchButtonByVendor_Click"></asp:Button>
                        <asp:Button ID="btnRefresh" class="btn btn-outline-success" runat="server" Text="Refresh" OnClick="ClearButton_Click"></asp:Button>
                    </li>
                </ul>
                <asp:Label ID="lblSuccessVendor" Text="" runat="server" ForeColor="Green" />
                <br />
                <asp:Label ID="lblErrorVendor" Text="" runat="server" ForeColor="Red" />
            </div>
            <div runat="server" class="container-fluid" style="width: 33%; height: 100%; float: left">
                <div class="container-fluid" style="width: 100%; height: auto">
                    <ul style="list-style-type: none;">
                        <li>
                            <asp:Button ID="btnPrint" class="btn btn-outline-success" Style="width: 120px" runat="server" Text="Print" OnClientClick="doPrint()"></asp:Button>
                            <asp:Button ID="btnDownload" class="btn btn-outline-success" runat="server" Text="Download" OnClick="btnExportToPDF_Click"></asp:Button>
                        </li>
                        <li>
                            <asp:Label ID="lblVendorName" runat="server" Text="Vendor Name" class="badge badge-primary" Style="width: 200px; height: 30px; font-size: large; margin: 10px; margin-left: 0px"></asp:Label>
                            <asp:TextBox ID="txtVendorName" class="vendorName" runat="server" AutoCompleteType="Disabled"></asp:TextBox>
                        </li>
                        <li>
                            <asp:Button ID="AddButton" class="btn btn-outline-success" runat="server" Text="ADD" OnClick="AddButton_Click"></asp:Button>
                            <asp:Button ID="btnDelete" class="btn btn-outline-success" runat="server" Text="Delete" OnClick="DeleteButton_Click" OnClientClick="return confirm('Are you sure to delete it?');"></asp:Button>
                            <asp:Button ID="btnClear" class="btn btn-outline-success" runat="server" Text="Clear" OnClick="ClearButton_Click"></asp:Button>
                        </li>
                    </ul>
                </div>
                <asp:Label ID="lblSuccessfulDelete" Text="" runat="server" ForeColor="Green" />
                <br />
                <asp:Label ID="lblErrorDelete" Text="" runat="server" ForeColor="Red" />
                <br />
            </div>
            <div class="container-fluid" style="width: 25%; height: 100%; float: left">
                <div style="width: 100%; height: 10%; text-align: center; margin-right: auto; margin-left: auto">
                    <asp:Label ID="lblVendorList" Text="Vendor List" runat="server" ForeColor="Red" />
                </div>
                <div class="container-fluid" style="width: 100%; height: 90%; overflow: auto">

                    <asp:GridView ID="gvVendorName" runat="server" AutoGenerateColumns="false" ShowFooter="true" DataKeyNames="vendor_id" ShowHeaderWhenEmpty="true" Width="100%"
                        OnRowCancelingEdit="gvVendorName_RowCancelingEdit" OnRowEditing="gvVendorName_RowEditing" OnRowUpdating="gvVendorName_RowUpdating"
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
                            <asp:TemplateField HeaderText="Vendor Name">
                                <ItemTemplate>
                                    <asp:Label Text='<%#Eval("vendor_name") %>' runat="server"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtVendorName" class="vendorName" Text='<%#Eval("vendor_name") %>' runat="server" AutoCompleteType="Disabled"></asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:ImageButton ImageUrl="~/picture/edit.png" runat="server" CommandName="Edit" ToolTip="Edit" Width="20px" Height="20px" OnClientClick="return confirm('Are you sure to edit it?');" />
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:ImageButton ImageUrl="~/picture/save.png" runat="server" CommandName="Update" OnClientClick="return confirm('Are you sure to save it?');" ToolTip="Update" Width="20px" Height="20px" />
                                    <asp:ImageButton ImageUrl="~/picture/cancel.png" runat="server" CommandName="Cancel" OnClientClick="return confirm('Are you sure to cancel editing it?');" ToolTip="Cancel" Width="20px" Height="20px" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </div>
        <div style="text-align: center; margin-right: auto; margin-left: auto">
            <asp:Label ID="lblDisplayReport" Text="" runat="server" ForeColor="Red" />
        </div>
        <div class="container-fluid" style="width: 100%; height: 200px; overflow: auto">

            <asp:GridView ID="gvPurchaseByVendor" runat="server" AutoGenerateColumns="false" ShowFooter="true" DataKeyNames="purchaseID" ShowHeaderWhenEmpty="true" Width="100%"
                OnRowCancelingEdit="gvPurchaseByVendor_RowCancelingEdit" OnRowDeleting="gvPurchaseByVendor_RowDeleting" OnRowEditing="gvPurchaseByVendor_RowEditing" OnRowUpdating="gvPurchaseByVendor_RowUpdating"
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
                            <asp:ImageButton ImageUrl="~/picture/edit.png" runat="server" CommandName="Edit" ToolTip="Edit" Width="20px" Height="20px" OnClientClick="return confirm('Are you sure to edit it?');" />
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
        var vendor = null;
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
    <script>
        function doPrint() {
            var prtContent = document.getElementById('<%= gvPurchaseByVendor.ClientID %>');
            prtContent.border = 0; //set no border here
            var header = "<div style='float:left; width:100%'><h2 style='margin:0px'>Smoke Music Cafe</h2><p>'Hakam Foundation', Top Floor<br>House-98, Road-11, Block-C<br>Banani C/A, DHAKA-1213<br>(+88) 01750 999 333, (+88) 01750 999 444<br>info@smoke-cafe.com</p></div>";
            var heading = "<div style=' text-align: center; width:100%'><h3>Vendor Wise Purchasing Record's</h3></div>";
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
</body>
</html>
