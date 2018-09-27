$("#btn-sitesettings").on("click", function() {
	var action = $(this).data("action");
	if (action) {
		openModalGet(action, null, function(modalData) {
			showMessage(modalData);
		});
	}
});

$("#adminSettingsTarget").addClass("user-tabtarget");