$(function () {

	$(".form-horizontal").on("submit", function () {
		var _this = $(this);
		var valid = _this.valid();
		if (valid) {
			$("#submit, #cancel").attr("disabled", "disabled");
			$("#submit").html(String.format('<span><i class="fa fa-spinner fa-pulse fa-fw"></i>{0}</span>', Resources.General.ProcessingMessage));
		}
	});

	$("#Amount").on("change keyup paste", function () {
		var _this = $(this);
		var amount = +_this.val() || 0;
		$("#netamount").html(amount.toFixed(8));
		$('#netamount').removeClass('text-success text-danger');
		$('#netamount').addClass((amount > 0) ? 'text-success' : '');
	});

	$("#balance").on("click", function () {
		var _this = $(this);
		$("#Amount").val(_this.html()).trigger("change");
	});

	$('#sendtfacode').on('click', function () {
		$('#tfaCodeMessage').hide();
		$('#tfaCodeMessage').removeClass("alert-success alert-danger");
		$.blockUI({ message: Resources.TransferCreate.SendingCodeMessage });
		getJson(tfaCodeAction, { componentType: 21 }, function (data) {
			$.unblockUI();
			var message = '';
			$('#tfaCodeMessage').show();
			if (data.Success) {
				$('#tfaCodeMessage').addClass("alert-success");
				$('#tfaCodeMessage > p').html(Resources.TransferCreate.SendingCodeSuccessMessage);
			}
			else {
				$('#tfaCodeMessage').addClass("alert-danger");
				$('#errorMessage > p').html(String.format(Resources.TransferCreate.SendingCodeFailureMessage,
					                                      String.format('<a href="/Support">{0}</a>', Resources.General.CryptopiaSupportLink)));
			}
		});
	});

	$('#UserName').on("keyup paste change", function () {
		$("#submit").attr("disabled", "disabled");
		$('#search-container').removeClass('has-error has-success');
		$('#check').removeClass('btn-primary btn-danger btn-success').addClass('btn-primary').html(String.format('<span>{0}</span>', Resources.TransferCreate.VerifyUserButton));
	})

	$('#check').on('click', function () {
		$('#transfer-content').hide();
		$('#transfer-message').show();
		$("#submit").attr("disabled", "disabled");
		var username = $('#UserName').val();
		$('#search-container').removeClass('has-error has-success');
		$('#check').removeClass('btn-primary btn-danger btn-success').addClass('btn-primary');
		if (username) {
			getJson('/Transfer/CheckUser/', { userName: username }, function (data) {
				if (data.Success) {
					$("#submit").removeAttr("disabled");
					$('#check').addClass('btn-success').html(String.format('<span><i class="fa fa-check fa-lg"></i>{0}</span>', Resources.TransferCreate.VerifyUserDoneButton));
					$('#search-container').addClass('has-success');
					$('#transfer-content').show();
					$('#transfer-message').hide();
					$('#transfer-receiver .profile-usertitle-name').html(username)
					$('#transfer-avatar').attr('src', '/Content/Images/Avatar/' + username + '.png')
				}
				else {
					$('#check').addClass('btn-danger').html(String.format('<span><i class="fa fa-times fa-lg"></i>{0}</span>', Resources.TransferCreate.VerifyUserNotFoundButton));
					$('#search-container').addClass('has-error');
				}
			});
		}
	
	});

	if (searchParam) {
		$('#check').trigger("click");
	}
});