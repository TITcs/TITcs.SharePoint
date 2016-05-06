<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ProviderHosted.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Provider-Hosted</title>
    <script src="Scripts/libs/jquery-1.11.1.min.js"></script>
    <script src="Scripts/libs/titcs.js"></script>
    <script src="Scripts/libs/titcs.sharepoint.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <h3>Done</h3>
        <div id="serviceDone"></div>
        <h3>Fail</h3>
        <div id="serviceFail"></div>
        <h3>Error</h3>
        <div id="serviceError"></div>
        <h3>Business Rule</h3>
        <div id="serviceBusinessRule"></div>
        <h3>Is Internet Explorer</h3>
        <div id="isIE"></div>
    </form>
    <script src="Scripts/app.js"></script>
</body>
</html>
