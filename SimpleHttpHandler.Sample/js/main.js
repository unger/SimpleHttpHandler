(function ($) {

	$("#param").on("click", function () {
		var obj = eval("(function() { return " + $("#paraminput").val() + "; })();");

		//var obj = JSON.parse($("#paraminput").val());
		$("#paramoutput").val(decodeURIComponent($.param(obj)));
	});


	$("#deparam").on("click", function () {
		var obj = $.deparam($("#deparaminput").val());
		$("#deparamoutput").val(JSON.stringify(obj));
	});


})(jQuery);


(function ($) {

	var dummyData = { apa: 10, bepa: 12.45, cepa: "string", depa: true, epa: false, fepa: null, gepa: [1, 2, 3] };

	$('#send').on("click", function() {
		var obj = eval("(function() { return " + $("#paraminput").val() + "; })();");

		$.ajax({
			dataType: "json",
			url: '/Handlers/Handler.ashx/ReturnJson',
			data: obj,
			success: function (data, textStatus, jqXHR) {
				console.log(data);

				$("#paramoutput").val(JSON.stringify(data));
			}
		});
		
	});


	/*$.ajax({
		dataType: "json",
		type: "POST",
		url: '/Handler.ashx/ReturnJson',
		data: { apa: 10, bepa: 12.45, cepa: "string", depa: true, epa: false, fepa: null, gepa: [1, 2, 3] },
		success: function (data, textStatus, jqXHR) {
			console.log(data);
		}
	});*/

})(jQuery);
