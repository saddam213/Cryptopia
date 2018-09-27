$(function () {
	var client = new ZeroClipboard(document.getElementById("copy-icon"));
	var client2 = new ZeroClipboard(document.getElementById("copy-icon2"));
	var client3 = new ZeroClipboard(document.getElementById("copy-icon3"));

	if (!addressData) {
		$.blockUI({ message: Resources.DepositDisplay.CreatingAddressMessage });
		postJson(createAction, { currencyId: currencyId }, function (data) {
			$.unblockUI();
			if (!data.Success) {
				$("#errorMessage").show();
				$("#errorText").html(data.Message)
			} else {
				setupAddress(data.QrCode, data.AddressData, data.AddressData2, data.AddressData3)
			}
		});
	}
	else {
		setupAddress(qrCodeData, addressData, addressData2, addressData3)
	}

	$('#depositAddress, #depositAddress2, #depositAddress3').click(function () {
		$(this).select();
	});

	$("[name=fiatDepositOptions]").click(function () {
		var val = $(this).val();
		$('.fiatDepositOptions').hide();
		$(".fiatDepositOption" + val).show();
		$("#instructions").hide();
		if (val == "Bank") {
			$("#instructions").show();
		}
	});

	function setupAddress(qrcodeData, addressData, addressData2, addressData3) {
		$("#addressContent").show();
		$("#depositAddress").val(addressData);
		$("#depositAddress2").val(addressData2);
		$("#depositAddress3").val(addressData3);

		client.on("ready", function (readyEvent) {
			client.on("aftercopy", function (event) {
				$('#addressContainer').addClass("has-success")
				var title = $('#addressLabel').html();
				$('#addressLabel').html(String.format(Resources.DepositDisplay.CopiedToClipboardMessage, title));
			});
		});

		if (client2) {
			client2.on("ready", function (readyEvent) {
				client2.on("aftercopy", function (event) {
					$('#addressContainer2').addClass("has-success")
					var title = $('#addressLabel2').html();
					$('#addressLabel2').html(String.format(Resources.DepositDisplay.CopiedToClipboardMessage, title));
				});
			});
		}

		if (client3) {
			client3.on("ready", function (readyEvent) {
				client3.on("aftercopy", function (event) {
					$('#addressContainer3').addClass("has-success")
					var title = $('#addressLabel3').html();
					$('#addressLabel3').html(String.format(Resources.DepositDisplay.CopiedToClipboardMessage, title));
				});
			});
		}

		if (qrcodeData) {
			$('#qr-code').qrcode({
				text: qrcodeData,
				width: 250,
				height: 250
			});
		}
	}
});
