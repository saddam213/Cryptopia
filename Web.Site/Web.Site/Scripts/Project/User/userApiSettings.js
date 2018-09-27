$("#generateKey").on("click", function() {
	$.unblockUI();
	var action = $(this).data('action');
	postJson(action, {}, function(data) {
		if (data.Success) {
			$("#ApiKey").val(data.Key);
			$("#ApiSecret").val(data.Secret);
		} else {
			$("#apialert").show();
			$("#apialert > p").html(Resources.SecurityApi.SecurityApiGenerationFailedLinkMessage);
			$("#apialert").fadeTo(8000, 500).slideUp(500, function() {
				$("#apialert").hide();
			});
		}
	});
});