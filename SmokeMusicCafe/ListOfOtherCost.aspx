<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ListOfOtherCost.aspx.cs" Inherits="SmokeMusicCafe.ListOfOtherCost" %>

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
        <div class="container-fluid" style="width: 100%; height: 230px">
            <asp:Label ID="lblHeadingPage" runat="server" Text="Page Of Other Cost Name List" class="badge badge-primary" Style="width: 100%; height: 30px; font-size: large; margin: 10px; margin-left: 0px"></asp:Label>
            <div class="container-fluid" style="width: 50%; height: auto; float: left">
                <div class="container-fluid" style="width: 100%; height: auto">
                    <ul style="list-style-type: none;">
                        <li>
                            <asp:Button ID="btnPreviousPage" class="btn btn-outline-success" runat="server" Text="Back" PostBackUrl="MonthlyFixedCost.aspx"></asp:Button>
                        </li>
                        <li>
                            <asp:Label ID="lblProductName" runat="server" Text="Name of Other Cost" class="badge badge-primary" Style="width: 200px; height: 30px; font-size: large; margin: 10px; margin-left: 0px"></asp:Label>
                            <asp:TextBox ID="txtProductName" class="auto" runat="server" AutoCompleteType="Disabled"></asp:TextBox>
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
            </div>
            <div id="visibleDiv" runat="server" class="container-fluid" style="width: 40%; height: auto; float: left">
                <div class="container-fluid" style="width: 100%; height: auto">
                    <ul style="list-style-type: none;">
                        <li>
                            <br />
                        </li>
                        <li>
                            <asp:Label ID="lblName" runat="server" Text="Name of Other Cost" class="badge badge-primary" Style="width: 200px; height: 30px; font-size: large; margin: 10px; margin-left: 0px"></asp:Label>
                            <asp:TextBox ID="txtName" class="auto" runat="server" AutoCompleteType="Disabled"></asp:TextBox>
                        </li>
                        <li>
                            <asp:Button ID="btnDelete" class="btn btn-outline-success" runat="server" Text="Delete" OnClick="DeleteButton_Click"  OnClientClick="return confirm('Are you sure to delete it?');"></asp:Button>
                            <asp:Button ID="btnClear" class="btn btn-outline-success" runat="server" Text="Clear" OnClick="ClearButton_Click"></asp:Button>
                        </li>
                    </ul>
                </div>
                <asp:Label ID="lblSuccessfulDelete" Text="" runat="server" ForeColor="Green" />
                <br />
                <asp:Label ID="lblErrorDelete" Text="" runat="server" ForeColor="Red" />
                <br />
            </div>
        </div>
        <div style="text-align: center; margin-right: auto; margin-left: auto">
            <asp:Label ID="lblProductList" Text="List of Other Cost" runat="server" ForeColor="Red" />
        </div>
        <div class="container-fluid" style="width: 100%; height: 300px; overflow: auto">

            <asp:GridView ID="gvProductName" runat="server" AutoGenerateColumns="false" ShowFooter="true" DataKeyNames="fixed_cost_id" ShowHeaderWhenEmpty="true" Width="100%"
                OnRowCancelingEdit="gvProductName_RowCancelingEdit" OnRowEditing="gvProductName_RowEditing" OnRowUpdating="gvProductName_RowUpdating"
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
                    <asp:TemplateField HeaderText="Name of Other Cost">
                        <ItemTemplate>
                            <asp:Label Text='<%#Eval("fixed_cost_name") %>' runat="server"></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtItemName" Text='<%#Eval("fixed_cost_name") %>' runat="server" AutoCompleteType="Disabled"></asp:TextBox>
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
    </form>
</body>
    <script>
        var ds = null;
        ds = <%=itemName %>
            $(".auto").autocomplete(
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
