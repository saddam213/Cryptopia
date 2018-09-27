$(function () {

	$("#AddressBook").on("change", function () {
		var rawaddress = $(this).val();
		if (!rawaddress) {
			$("#AddressData, #AddressData2, #AddressDataReadonly, #AddressDataReadonly2").val(null);
		}
		var split = rawaddress.split(':');
		$("#AddressData, #AddressDataReadonly").val(split[0]);
		if (split.length == 2) {
			$("#AddressData2, #AddressDataReadonly2").val(split[1]);
		}
	})

	$("#AddressData, #AddressData2").on("keyup cut paste", function () {
		$("#AddressBook").val(null);
	})

	$(".form-horizontal").on("submit", function () {
		var _this = $(this);
		var valid = _this.valid();
		if (valid) {
			$("#submit, #cancel").attr("disabled", "disabled");
			$("#submit").html(String.format('<span><i class="fa fa-spinner fa-pulse fa-fw"></i> {0}</span>', Resources.General.ProcessingMessage));
		}
	});

	$("#Amount").on("change keyup paste", function () {
		var _this = $(this);
		var netAmount = 0;
		var totalFees = +fees;
		var amount = +_this.val() || 0;
		if (feeType == "Percent") {
			totalFees = amount - ((amount / 100) * fees);
			netAmount = amount - totalFees;
		}
		else {
			netAmount = amount - fees;
		}
		$("#feeamount").html(totalFees.toFixed(decimals));
		$("#netamount").html(netAmount.toFixed(decimals));
		$('#netamount').removeClass('text-success text-danger');
		$('#netamount').addClass((netAmount > totalFees) ? 'text-success' : 'text-danger');
	});


	$("#Amount").on("change", function () {
		var _this = $(this);
		_this.val((+_this.val()).toFixed( decimals))
	});


	$("#balance").on("click", function () {
		var _this = $(this);
		$("#Amount").val(_this.html()).trigger("change");
	});

	$("[name=fiatOptions]").click(function () {
		var val = $(this).val();
		$('.fiatOptions').hide();
		$(".fiatOption" + val).show();
		if (val == "Bank") {
			$("#AddressData").mask("99-9999-9999999-99?9");
		}
		else {
			$("#AddressData").unmask();
		}
	});

	$("#fiatOptionBank").trigger("click");
	
	$('#sendtfacode').on('click', function () {
		$('#tfaCodeMessage').hide();
		$('#tfaCodeMessage').removeClass("alert-success alert-danger");
		$.blockUI({ message: Resources.WithdrawCreate.SendingCodeMessage });
		getJson(tfaCodeAction, { componentType: 20 }, function (data) {
			$.unblockUI();
			var message = '';
			$('#tfaCodeMessage').show();
			if (data.Success) {
				$('#tfaCodeMessage').addClass("alert-success");
				$('#tfaCodeMessage > p').html(Resources.WithdrawCreate.SendingCodeSuccessMessage);
			}
			else {
				$('#tfaCodeMessage').addClass("alert-danger");
				$('#errorMessage > p').html(String.format(Resources.WithdrawCreate.withdrawCreateSendingCodeFailureMessage,
 					                                      String.Format('<a href="/Support">{0}</a>', Resources.GeneralCryptopiaSupportLink)));
			}
		});
	});

});