<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ServiceQueryDemo.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="Scripts/jquery-1.11.1.min.js"></script>
    <script src="Scripts/services.js"></script>
    <script type="text/javascript">

        services.get('/services/service1.sps/getall?id=1&key=U').done(function(response) {

            //alert(response);

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
