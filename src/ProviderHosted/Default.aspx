<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ProviderHosted.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Provider-Hosted</title>
    <script src="Scripts/libs/jquery-1.11.1.min.js"></script>
    <script src="Scripts/titcs/titcs.min.js"></script>
    <script type="text/javascript">

        TITcs.Http.get('/services/avatarservice.sps/get').done(function(response) {

            $('#result').text(JSON.stringify(response));

        }).error(function(a, b, c) {
            $('#result').text(JSON.stringify(a));
        });

    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div id="result">
    
    </div>
    </form>
</body>
</html>
