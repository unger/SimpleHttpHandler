<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="SimpleHttpHandler.Sample.Default" %>
<!DOCTYPE html>
<html class="no-js">
    <head>
        <meta charset="utf-8">
        <title>SimpleHttpHandler Demo</title>
        <meta name="description" content="">
        <meta name="viewport" content="width=device-width">
		
		<link href="css/main.css" rel="stylesheet" />
    </head>
    <body>

		<div>
			<textarea id="paraminput">{ apa: 10, bepa: 12.45, cepa: "string", depa: true, epa: false, fepa: null, gepa: [1, 2, 3] }</textarea>
			<textarea id="paramoutput"></textarea>
		</div>
		<input id="param" type="button" value="param" /><input id="send" type="button" value="send" />
		
		<hr/>

		<div>
			<textarea id="deparaminput">a[]=1&a[]=2&a[][d]=8&a[][e]=9&b=2</textarea>
			<textarea id="deparamoutput"></textarea>
		</div>
		<input id="deparam" type="button" value="deparam" />
		

        <script src="//ajax.googleapis.com/ajax/libs/jquery/1.9.1/jquery.min.js"></script>
        <script src="/js/libs/jquery-deparam.js"></script>
        <script src="/js/main.js"></script>
    </body>
</html>
