<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="SmokeMusicCafe.Login" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .auto-style43 {
            width: 100%;
        }
        .auto-style44 {
            text-align:center;
            width:100%;
            color:blue;
        }
        .auto-style46 {
            text-align:right;
            width:50%;
        }
        .secclmn {
            width: 50%;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table class="auto-style43">
        <tr>
            <td style="text-align:center" colspan="2">
                <asp:Label ID="lblHeader" runat="server" Text="Log In" class="badge badge-primary" style="width:200px; height:50px; font-size:xx-large"></asp:Label>
            </td>
        </tr>
        <tr>
            <td style="height:50px">

            </td>
        </tr>
        <tr>
            <td class="auto-style46">
                <asp:Label ID="lblUsername" runat="server" Text="User Name" class="badge badge-primary" style="width:200px; height:30px; font-size:large"></asp:Label>
            </td>
            <td class="secclmn">
                <asp:TextBox ID="txtUserName" AutoCompleteType="Disabled" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="auto-style46">
                <asp:Label ID="lblPassword" runat="server" Text="Password" class="badge badge-primary" style="width:200px; height:30px; font-size:large"></asp:Label>
            </td>
            <td class="secclmn">
                <asp:TextBox ID="txtPassword" AutoCompleteType="Disabled" runat="server" TextMode="Password"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="auto-style46">
                <asp:Button ID="btnLogin" runat="server" class="btn btn-outline-success" Text="Login" OnClick="btnLogin_Click" />
            </td>
            <td class="secclmn">
                <asp:Button ID="btnRefresh" runat="server" class="btn btn-outline-success" Text="Refresh" OnClick="btnRefresh_Click" />&nbsp; &nbsp;
                <asp:Label ID="lblerror" runat="server" ForeColor="#CC0000"></asp:Label>
            </td>
        </tr>
    </table>
</asp:Content>
