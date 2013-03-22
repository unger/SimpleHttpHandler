(function ($) {

	$("#param").on("click", function () {
		var obj = JSON.parse($("#paraminput").val());
		$("#paramoutput").val(decodeURIComponent($.param(obj)));
	});


	$("#deparam").on("click", function () {
		var obj = $.deparam($("#deparaminput").val());
		$("#deparamoutput").val(JSON.stringify(obj));
	});


})(jQuery);

/*
(function ($) {

	$.ajax({
		dataType: "json",
		url: '/Handler.ashx/ReturnJson',
		data: { apa: 10, bepa: 12.45, cepa: "string", depa: true, epa: false, fepa: null, gepa: [1, 2, 3] },
		success: function (data, textStatus, jqXHR) {
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
*/