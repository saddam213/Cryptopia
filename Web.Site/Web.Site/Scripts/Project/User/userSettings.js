$(function() {
	$('#Theme').on('change', function() {
		$('link[title="siteTheme"]').attr({ href: "/Content/theme." + $("#Theme option:selected").text() + ".css" });
	});

	$("#IgnoreChats").on("change", function() {
		$("#ignore-chat").val($(this).find("option:selected").text());
	});
	$("#IgnoreTips").on("change", function() {
		$("#ignore-tip").val($(this).find("option:selected").text());
	});

	$('#ignore-chat-btn').on('click', function() {
		$('#tipErroralert, #chatErroralert, #tipSuccessalert, #chatSuccessalert').hide();
		var user = $('#ignore-chat').val();
		if (user) {
			var action = $(this).data('action');
			getJson(action, { username: user }, function(data) {
				if (data.Success) {
					$('#chatSuccessalert > p').html(data.Message);
					$("#chatSuccessalert").fadeTo(8000, 500).slideUp(500, function() {
						$("#chatSuccessalert").hide();
					});
					if (data.WasRemoved) {
						$('#IgnoreChats option').filter(function() { return $(this).html() == user; }).remove();
					} else {
						$('#IgnoreChats').append('<option value="' + user + '">' + user + '</option>');
					}
				} else {
					$('#chatErroralert > p').html(data.Message);
					$("#chatErroralert").fadeTo(8000, 500).slideUp(500, function() {
						$("#chatErroralert").hide();
					});
				}
			});
		}
	});

	$('#ignore-tip-btn').on('click', function() {
		$('#tipErroralert, #chatErroralert, #tipSuccessalert, #chatSuccessalert').hide();
		var user = $('#ignore-tip').val();
		if (user) {
			var action = $(this).data('action');
			getJson(action, { username: user }, function(data) {
				if (data.Success) {
					$('#tipSuccessalert > p').html(data.Message);
					$("#tipSuccessalert").fadeTo(8000, 500).slideUp(500, function() {
						$("#tipSuccessalert").hide();
					});
					if (data.WasRemoved) {
						$('#IgnoreTips option').filter(function() { return $(this).html() == user; }).remove();
					} else {
						$('#IgnoreTips').append('<option value="' + user + '">' + user + '</option>');
					}
				} else {
					$('#tipErroralert > p').html(data.Message);
					$("#tipErroralert").fadeTo(8000, 500).slideUp(500, function() { $("#tipErroralert").hide(); });
				}
			});
		}
	});


	$("#settingsTarget").addClass("user-tabtarget");
});
