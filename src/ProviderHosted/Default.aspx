<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ProviderHosted.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Provider-Hosted</title>
    <script src="Scripts/libs/jquery-1.11.1.min.js"></script>
    <script src="Scripts/libs/titcs.js"></script>
    <script src="Scripts/libs/titcs.sharepoint.js"></script>
    <script type="text/javascript">

        TITcs.Http.get('/services/avatarservice.sps/get').then(function(response) {

            $('#serviceDone').text(JSON.stringify(response));

        });

        TITcs.Http.get('/services/avatar1service.sps/fail').then(function (response) {

            $('#serviceFail').text(JSON.stringify(response));

        });

        TITcs.Http.get('/services/avatarservice.sps/error').then(function (response) {

            $('#serviceError').text(JSON.stringify(response));

        });

        TITcs.Http.get('/services/avatarservice.sps/businessrule').then(function (response) {

            $('#serviceBusinessRule').text(JSON.stringify(response));

        });

    </script>
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
    </form>
</body>
</html>
