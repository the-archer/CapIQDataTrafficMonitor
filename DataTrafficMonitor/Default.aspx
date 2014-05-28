<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="DataTrafficMonitor._Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <p>
        Demo&nbsp; 
        <asp:Button ID="Button2" runat="server" onclick="Button2_Click" 
            Text="Current" />
    </p>
&nbsp;&nbsp;&nbsp;<asp:TextBox ID="TextBox1" runat="server" Width="35px"></asp:TextBox>
    &nbsp;&nbsp;&nbsp;/
    <asp:TextBox ID="TextBox2" runat="server" Width="35px"></asp:TextBox>
    /
    <asp:TextBox ID="TextBox3" runat="server" Width="35px"></asp:TextBox>
&nbsp;
    <asp:TextBox ID="TextBox4" runat="server" Width="35px"></asp:TextBox>
&nbsp;h :
    <asp:TextBox ID="TextBox5" runat="server" Width="35px"></asp:TextBox>
&nbsp;m&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
    <asp:DropDownList ID="DropDownList2" runat="server" DataSourceID="DTM" 
        DataTextField="metric_name" DataValueField="metric_name" Height="25px" 
        Width="145px" 
    onselectedindexchanged="DropDownList2_SelectedIndexChanged">
    </asp:DropDownList>
    <asp:SqlDataSource ID="DTM" runat="server" 
        ConnectionString="<%$ ConnectionStrings:dtmConnectionString2 %>" 
        SelectCommand="SELECT [metric_name] FROM [metrics_tbl] ORDER BY [metric_id]">
    </asp:SqlDataSource>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
    <asp:Button ID="Button1" runat="server" onclick="Button1_Click1" 
        Text="Get Data" />
    <br />
    <br />
    <br />
    <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
    <asp:GridView ID="GridView1" runat="server" CellPadding="4" ForeColor="#333333" 
        GridLines="None" onselectedindexchanged="GridView1_SelectedIndexChanged" 
        Width="327px">
        <AlternatingRowStyle BackColor="White" />
        <EditRowStyle BackColor="#2461BF" />
        <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
        <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
        <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
        <RowStyle BackColor="#EFF3FB" HorizontalAlign="Center" />
        <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
        <SortedAscendingCellStyle BackColor="#F5F7FB" />
        <SortedAscendingHeaderStyle BackColor="#6D95E1" />
        <SortedDescendingCellStyle BackColor="#E9EBEF" />
        <SortedDescendingHeaderStyle BackColor="#4870BE" />
    </asp:GridView>
</asp:Content>
