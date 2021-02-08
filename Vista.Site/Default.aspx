<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Vista.Site._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
     

<script type="text/javascript" src="Scripts/jquery.dataTables.min.js"></script>
<link href="Content/jquery.dataTables.css" rel="stylesheet" type="text/css" />



    <div>
        <asp:Image runat="server" ID="imgRSSImage" />
        <h3><asp:Label runat="server" ID="lblRSSTitle" /></h3>
        
        <p>
            <asp:Label runat="server" ID="lblRSSDescription" />
        </p>
    </div>

    <div class="row">
         <asp:GridView ID="grvRSSFeed" runat="server" CssClass="display compact" AutoGenerateColumns="false">
            <Columns>                
                <asp:HyperLinkField HeaderText="Title" DataTextField="ItemTitle" DataNavigateUrlFields="ItemLink"/>
                <asp:HyperLinkField HeaderText="Description" DataTextField="ItemDescription" DataNavigateUrlFields="ItemLink"/>
                <asp:HyperLinkField HeaderText="Published Date" DataTextField="PublishedDate" DataNavigateUrlFields="ItemLink"/>
            </Columns>
        </asp:GridView>



 <script type="text/javascript">
     $(function () {
         $("[id*=grvRSSFeed]").DataTable(
             {
                 bLengthChange: true,
                 lengthMenu: [[5, 10, -1], [5, 10, "All"]],
                 bFilter: true,
                 bSort: true,
                 bPaginate: true
             });
     });
 </script>

</asp:Content>
