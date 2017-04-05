<%@ Page AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="DeerInformation.WebForms.WebForm1" Language="C#" %>
<%@ Register TagPrefix="rsweb" Namespace="Microsoft.Reporting.WebForms" Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
<form id="Form1" runat="server">
　　<asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
　　<asp:UpdatePanel ID="uid"  runat="server">
　　　　<ContentTemplate>
        　<rsweb:ReportViewer ID="ReportViewer1" runat="server" AsyncRendering="False" 
    　　　　Height="100%" Width="100%" ></rsweb:ReportViewer>
　　　　</ContentTemplate>
　　</asp:UpdatePanel>
</form>
</body>
</html>
