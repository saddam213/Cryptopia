(function() {

	$("#Type").on('change', function() {
		var _this = $(this);
		clearMessages();
		$('.input-submit').show();
		$(".data-input-email, .data-input-google, .data-input-pincode, .data-input-none, .data-input-password, .data-input-question, .data-input-cryptopia").hide();
		var selection = _this.val();
		if (selection === '0') {
			$(".data-input-none").show();
			$('.input-submit').hide();
		}
		if (selection === '1') {
			$(".data-input-email").show();
		}
		if (selection === '2') {
			$(".data-input-google").show();
		}
		if (selection === '3') {
			$(".data-input-pincode").show();
		}
		if (selection === '10') {
			$(".data-input-password").show();
		}
		if (selection === '11') {
			$(".data-input-question").show();
		}
		if (selection === '12') {
			var activated = $("#existing-cryptopia").data("activated");
			if (!activated) {
				$(".input-submit").attr("disabled", "disabled");
			}
			$(".data-input-cryptopia").show();
		}
	}).trigger('change');


	$("#send-email").on('click', function() {
		clearMessages();
		var _this = $(this);

		var email = $("#DataEmail").val();
		if (email) {
			var action = _this.data('action');
			var error = _this.data('error');
			var success = _this.data('success');
			var component = _this.data('component');
			$.blockUI({ message: Resources.General.SendingMessage });
			postJson(action, { componentType: component, dataEmail: email }, function(data) {
				$.unblockUI();
				if (data.Success) {
					$('#send-email').removeClass('btn-danger').addClass('btn-success');
					showSuccess(success);
					return;
				}
				$('#send-email').removeClass('btn-success').addClass('btn-danger');
				showError(error);
			});
		}
	});

	$("#verify-email").on('click', function() {
		clearMessages();
		var _this = $(this);
		var code = $("#code-email").val();
		if (code) {
			var action = _this.data('action');
			var error = _this.data('error');
			var success = _this.data('success');
			var component = _this.data('component');
			postJson(action, { componentType: component, code: code }, function(data) {
				if (data.Success) {
					$('#verify-email').removeClass('btn-danger').addClass('btn-success');
					$(".input-submit").removeAttr("disabled");
					showSuccess(success);
					return;
				}
				$('#verify-email').removeClass('btn-success').addClass('btn-danger');
				showError(error);
			});
		}
	});

	$("#verify-google").on('click', function() {
		clearMessages();
		var _this = $(this);
		var code = $("#code-google").val();
		if (code) {
			var action = _this.data('action');
			var error = _this.data('error');
			var success = _this.data('success');
			var key = _this.data('key');
			postJson(action, { key: key, code: code }, function(data) {
				if (data.Success) {
					$('#verify-google').removeClass('btn-danger').addClass('btn-success');

					$(".input-submit").removeAttr("disabled");
					showSuccess(success);
					return;
				}
				$('#verify-google').removeClass('btn-success').addClass('btn-danger');
				showError(error);
			});
		}
	});

	$("#activate-Cryptopia").on('click', function () {
		clearMessages();
		var _this = $(this);
		var code = $("#CryptopiaCode").val();
		var serialNumber = $("#CryptopiaSerial").val();
		if (code && serialNumber) {
			var action = _this.data('action');
			postJson(action, { serialNumber: serialNumber, code: code }, function (data) {
				if (data.Success) {
					$('#activate-Cryptopia').removeClass('btn-info').addClass('btn-success').html("Activated").attr("disabled", "disabled");
					$(".data-input-cryptopia").attr("readonly", "readonly");
					$(".input-submit").removeAttr("disabled");
					showSuccess(data.Message);
					return;
				}
				showError(data.Message);
			});
		}
	});

	$('.preview-popover').popover({
		trigger: 'hover',
		container: 'body'
	});
})();


function clearMessages() {
	$('button').removeClass('btn-success btn-danger');
	$('#validationMessage').addClass('validation-summary-valid');
	$('#validationMessage').removeClass('validation-summary-errors');
	$('#errorMessage').hide();
	$('#successMessage').hide();
	$('#EmailCode, #GoogleCode, #CryptopiaCode').removeClass('has-success');
}

function showError(message) {
	$('#errorMessage').show();
	$('#successMessage').hide();
	$('#errorMessage > h5').text(message);
}

function showSuccess(message) {
	$('#errorMessage').hide();
	$('#successMessage').show();
	$('#successMessage > h5').text(message);
}