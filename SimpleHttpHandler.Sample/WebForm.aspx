<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm.aspx.cs" Inherits="SimpleHttpHandler.Sample.WebForm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form action="" method="post">
        
        <input name="Apa" type="text" />

        <input name="Bepa" type="text" />

        <input name="Cepa" type="text" />

        <button value="Submit" />

    </form>
    
    <pre>
        <asp:Literal ID="ResponseOutput" runat="server"></asp:Literal>
    </pre>
</body>
</html>
