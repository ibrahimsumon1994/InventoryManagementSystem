<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="SmokeMusicCafe.Dashboard" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
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
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Label ID="lblCurrentMonthExpense" runat="server" Text="Current Month's Expense" class="badge badge-primary" Style="width: 300px; height: 30px; font-size: large; margin: 10px; margin-left: 0px"></asp:Label>
    <asp:Label ID="txtCurrentMonthExpense" runat="server" Text=""></asp:Label>
    <asp:Button ID="btnExpenditureReport" class="btn btn-outline-success" style="float: right; margin-right: 100px; margin-top:60px; width:230px; height:70px" runat="server" Text="Expenditure Report" PostBackUrl="ExpenditureReport.aspx"></asp:Button>
    <br />
    <br />
    <asp:Label ID="lblTodayExpense" runat="server" Text="Today's Expense" class="badge badge-primary" Style="width: 300px; height: 30px; font-size: large; margin: 10px; margin-left: 0px"></asp:Label>
    <asp:Label ID="txtTodayExpense" runat="server" Text=""></asp:Label>
</asp:Content>
