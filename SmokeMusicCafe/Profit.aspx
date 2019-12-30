<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Profit.aspx.cs" Inherits="SmokeMusicCafe.Profit" %>

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
        <div class="container-fluid">
            <asp:Label ID="lblHeadingPage" runat="server" Text="Report Page Of Daily/Monthly Profit" class="badge badge-primary" style="width:100%; height:30px; font-size:large; margin:10px; margin-left:0px"></asp:Label>
            <div style="width: 50%; height: 300px; float: left">
                <ul style="list-style-type: none;">
                    <li>
                        <asp:Button ID="btnPreviousPage" class="btn btn-outline-success" runat="server" Text="Back" PostBackUrl="Dashboard.aspx"></asp:Button>
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
                    </li>
                    <li>
                        <asp:Button ID="btnProfitCalculate" class="btn btn-outline-success" runat="server" Text="Profit Summary" OnClick="ProfitCalculate_Click"></asp:Button>
                        <asp:Button ID="btnRefresh" class="btn btn-outline-success" runat="server" Text="Refresh" OnClick="ClearButton_Click"></asp:Button>
                    </li>
                    <li>
                        <asp:Label ID="lblSuccess" Text="" runat="server" ForeColor="Green" />
                        <br />
                        <asp:Label ID="lblError" Text="" runat="server" ForeColor="Red" />
                    </li>
                </ul>
            </div>
        </div>
        <div>
            <div style="float:left; height:300px">
                <ul style="list-style-type: none;">
                    <li>
                        <asp:Label ID="lblDisplayProfitReport" Text="Profit Summary in a specific range" runat="server" ForeColor="Red" />
                    </li>
                    <li style="margin-top: 20px">
                        <asp:Label ID="lblStartDateForProfitShow" runat="server" Text="Start Date" class="badge badge-primary" Style="width: 200px; height: 30px; font-size: large; margin: 10px; margin-left: 0px"></asp:Label>
                        <asp:Label ID="lblStartDateProfit" Text="" runat="server"/>
                    </li>
                    <li>
                        <asp:Label ID="lblEndDateForProfitShow" runat="server" Text="End Date" class="badge badge-primary" Style="width: 200px; height: 30px; font-size: large; margin: 10px; margin-left: 0px"></asp:Label>
                        <asp:Label ID="lblEndDateProfit" Text="" runat="server"/>
                    </li>
                    <li>
                        <asp:Label ID="lblTotalExpenditureShow" runat="server" Text="Total Expenses" class="badge badge-primary" Style="width: 200px; height: 30px; font-size: large; margin: 10px; margin-left: 0px"></asp:Label>
                        <asp:Label ID="lblTotalExpenditureSearch" Text="" runat="server"/>
                    </li>
                    <li>
                        <asp:Label ID="lblTotalSalesShow" runat="server" Text="Total Sales" class="badge badge-primary" Style="width: 200px; height: 30px; font-size: large; margin: 10px; margin-left: 0px"></asp:Label>
                        <asp:Label ID="lblTotalSalesShowSearch" Text="" runat="server"/>
                    </li>
                    <li>
                        <asp:Label ID="lblProfitShow" runat="server" Text="" class="badge badge-primary" Style="width: 200px; height: 30px; font-size: large; margin: 10px; margin-left: 0px"></asp:Label>
                        <asp:Label ID="lblProfitSearch" Text="" runat="server"/>
                    </li>
                </ul>
            </div>
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
    </form>
</body>
</html>