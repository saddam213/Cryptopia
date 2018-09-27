$(function () {
	$("#DisableConfirmation, #AddressBookOnly").on("click", function () {
		var action = $("#withdrawSettings-container").data("action");
		var addressBookOnly = $("#AddressBookOnly").is(":checked");
		var disableConfirmation = $("#DisableConfirmation").is(":checked");
		postJson(action, { addressBookOnly: addressBookOnly, disableConfirmation: disableConfirmation }, function (data) {
			showMessage(data)
		});
	});
});
$("#securityTarget").addClass("user-tabtarget");
