$(".messageitem").on("click", function() {
	$(".messageitem").removeClass("active");
	$(this).addClass("active");
});
$("#notify-dismiss").on("click", function() {
	var action = $(this).data("action");
	if (action) {
		getJson(action, {}, function() {
			$(".unread-notification-count").html("");
		});
	}
});
$("#notify-deleteall").on("click", function() {
	var action = $(this).data("action");
	$.blockUI({ message: Resources.UserNotifications.NotificationsDeletingMessage });
	if (action) {
		getJson(action, {}, function() {
			$.unblockUI();
			$(".notification-unreadcount").html("");
			$("#notification-list").html('<div class="text-center"><i>' + Resources.UserNotifications.NotificationsEmptyListInfoText + '</i></div>');
			$(".unread-notification-count").html("");
		});
	}
});

$(window).off().resize(function() {
	var cheight = $(".profile-sidebar").height() - ($(".notification-toolbar").outerHeight() + $(".settings-page-header-container").outerHeight());
	$("#notification-container").height(cheight);
});
$(window).resize();

$("#notificationTarget").addClass("user-tabtarget");