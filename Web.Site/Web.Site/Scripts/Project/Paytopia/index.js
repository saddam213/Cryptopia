$(".paytopia-buynow").on("click", function () {
	var action = $(this).data("action");
	if (action) {
		openModalGet(action, {}, function (data) {
			showMessage(data);
		});
	}
});