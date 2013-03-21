<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="SimpleHttpHandler.Sample.Default" %>
<!DOCTYPE html>
<html class="no-js">
    <head>
        <meta charset="utf-8">
        <title>SimpleHttpHandler Demo</title>
        <meta name="description" content="">
        <meta name="viewport" content="width=device-width">
    </head>
    <body>
        <script src="//ajax.googleapis.com/ajax/libs/jquery/1.9.1/jquery.min.js"></script>
		
		
		<script>
			(function ($) {
				
				$.ajax({
					dataType: "json",
					url: '/Handler.ashx/ReturnJson',
					data: { apa: 10, bepa: 12.45, cepa: "string", depa: true, epa: false, fepa: null, gepa: [1, 2, 3] },
					success: function(data, textStatus, jqXHR) {
						console.log(data);
					}
				});

				$.ajax({
					dataType: "json",
					type: "POST",
					url: '/Handler.ashx/ReturnJson',
					data: { apa: 10, bepa: 12.45, cepa: "string", depa: true, epa: false, fepa: null, gepa: [1, 2, 3] },
					success: function (data, textStatus, jqXHR) {
						console.log(data);
					}
				});

			})(jQuery);
		</script>

    </body>
</html>
