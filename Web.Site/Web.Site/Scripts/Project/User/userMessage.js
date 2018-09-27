$("#message-new").on("click", function() {
	var action = $(this).data("action");
	openModalGet(action, {}, function(data) {
		showMessage(data);
		if (data.Success) {
			$("#selection-outbox").trigger("click");
		}
	});
});

$("#message-reply").on("click", function() {
	var action = $(this).data("action");
	var selectedMessage = $("#list-message .active").data("messageid");
	if (selectedMessage) {
		openModalGet(action, { messageId: selectedMessage }, function(data) {
			showMessage(data);
			if (data.Success) {
				$("#selection-outbox").trigger("click");
			}
		});
	}
});

$("#message-report").on("click", function() {
	var action = $(this).data("action");
	var selectedMessage = $("#list-message .active").data("messageid");
	if (selectedMessage) {
		openModalGet(action, { messageId: selectedMessage }, function(data) {
			showMessage(data);
		});
	}
});

$(".inbox-item").on("click", function() {
	itemSelect($(this));
});

$("#message-delete").on("click", function() {
	var action = $(this).data("action");
	var messageid = $("#list-message .inbox-item.active").data("messageid");
	if (messageid) {
		confirm(Resources.UserMessages.MessagesDeleteQuestionTitle, Resources.UserMessages.MessagesDeleteQuestion, function() {
			postJson(action, { messageId: messageid }, function(data) {
				if (data.Success) {
					$("#list-message").find("[data-messageid='" + messageid + "']").remove();
					$("#message-target").empty();
				}
				showMessage(data);
				setEmptyListMessage();
				updateUnreadcount();
			});
		});
	}
});

$("#message-deleteaall").on("click", function() {
	var action = $(this).data("action");
	var isinbox = $("#list-message").data("isinbox");
	var header = (isinbox ? Resources.UserMessages.MessagesDeleteAllInboxQuestionTitle : Resources.UserMessages.MessagesDeleteAllOutboxQuestionTitle);
	var message = (isinbox ? Resources.UserMessages.MessagesDeleteAllInboxQuestion : Resources.UserMessages.MessagesDeleteAllOutboxQuestion);
	confirm(header, message, function() {
		postJson(action, { inbox: isinbox }, function(data) {
			if (data.Success) {
				var messages = isinbox
					? $("#list-message").find("[data-inbox='True']")
					: $("#list-message").find("[data-inbox='False']");
				messages.remove();
				$("#message-target").empty();
			}
			showMessage(data);
			setEmptyListMessage();
			updateUnreadcount();
		});
	});
});

$("#message-search").on("change keyup paste", function() {
	var searchboxVal = $(this).val();
	var isinbox = $("#list-message").data("isinbox");
	var messageData = isinbox
		? $("#list-message").find("[data-inbox='True']")
		: $("#list-message").find("[data-inbox='False']");
	$.each(messageData, function() {
		var currentItem = $(this);
		var searchData = $(currentItem).find(".search-data");
		var searchString = "";
		$.each(searchData, function() {
			searchString += $(this).text() + " ";
		});
		if (searchString.toLowerCase().indexOf(searchboxVal.toLowerCase()) == -1)
			currentItem.hide();
		else
			currentItem.show();
	});
});

$("#selection-inbox").on("click", function() {
	$("#message-target").empty();
	$(".inbox-btns").show();
	$(".message-type").html("Inbox");
	$("#message-search").attr("placeholder", Resources.UserMessages.MessagesInboxTabSearchPlaceholder);
	$("#message-delete, #message-reply, #message-report").attr("disabled", "disabled");
	var messageList = $("#list-message");
	messageList.data("isinbox", true);
	messageList.find(".inbox-item").removeClass("active");
	messageList.find("[data-inbox='True']").show();
	messageList.find("[data-inbox='False']").hide();
	$("#selection-inbox").addClass("btn-info");
	$("#selection-outbox").removeClass("btn-info");
	setEmptyListMessage();
	updateUnreadcount();
}).trigger("click");

$("#selection-outbox").on("click", function() {
	$("#message-target").empty();
	$(".inbox-btns").hide();
	$(".message-type").html("Outbox");
	$("#message-search").attr("placeholder", Resources.UserMessages.MessagesOutboxTabSearchPlaceholder);
	$("#message-delete, #message-reply, #message-report").attr("disabled", "disabled");
	var messageList = $("#list-message");
	messageList.data("isinbox", false);
	messageList.find(".inbox-item").removeClass("active");
	messageList.find("[data-inbox='True']").hide();
	messageList.find("[data-inbox='False']").show();
	$("#selection-outbox").addClass("btn-info");
	$("#selection-inbox").removeClass("btn-info");
	setEmptyListMessage();
	updateUnreadcount();
});

$("#inbox-container").height($(".profile-sidebar").height() - ($(".settings-page-header-container").outerHeight() + $(".message-searchbar").outerHeight() + 10));

function itemSelect(item) {
	var messageList = $("#list-message");
	var action = messageList.data("action");
	var currentItem = item;
	var currentMessageId = currentItem.data("messageid");
	messageList.find(".inbox-item").removeClass("active");
	currentItem.addClass("active");
	$("#message-delete, #message-reply, #message-report").removeAttr("disabled");
	$("#message-target").empty();
	getPartial("#message-target", action + "?messageId=" + currentMessageId, false, function(data) {
		if (currentItem.data("unread") === "True") {
			currentItem.removeClass("text-bold");
			currentItem.find(".msg-icon").removeClass("fa-envelope").addClass("fa-envelope-o");
			currentItem.attr("data-unread", false);
			updateUnreadcount();
		}
	});
}

function updateUnreadcount() {
	var unreadCount = $("#list-message").find(".text-bold").length;
	$(".unread-message-count").html(unreadCount > 0 ? unreadCount : "");
}

function appendoutbox(data) {
	var messageTemplate = $("#messageTemplate").html();
	$("#list-message").prepend(Mustache.render(messageTemplate,
	{
		IsInbound: "False",
		MessageId: data.Id,
		Unread: "False",
		TextClass: "",
		IconClass: "fa-envelope-o",
		Sender: data.Recipiants,
		Time: Resources.UserMessages.MessagesJustNowLabel,
		Subject: data.Subject
	}));
	$(".inbox-item").off("click").on("click", function() {
		itemSelect($(this));
	});
	setEmptyListMessage();
	updateUnreadcount();
};

function appendinbox(data) {
	var messageTemplate = $("#messageTemplate").html();
	$("#list-message").prepend(Mustache.render(messageTemplate,
	{
		IsInbound: "True",
		MessageId: data.Id,
		Unread: "True",
		TextClass: "text-bold",
		IconClass: "fa-envelope",
		Sender: data.Sender,
		Time: Resources.UserMessages.MessagesJustNowLabel,
		Subject: data.Subject
	}));
	$(".inbox-item").off("click").on("click", function() {
		itemSelect($(this));
	});
	setEmptyListMessage();
	updateUnreadcount();
};

function setEmptyListMessage() {
	$("#emptyListMessage-inbox, #emptyListMessage-outbox").hide();
	var messageList = $("#list-message");
	var isinbox = messageList.data("isinbox");
	if (isinbox == true && messageList.find("[data-inbox='True']").length == 0) {
		$("#emptyListMessage-inbox").show();
	} else if (isinbox == false && messageList.find("[data-inbox='False']").length == 0) {
		$("#emptyListMessage-outbox").show();
	}
}

$(document).off("OnInboxMessage").on("OnInboxMessage", function (evt, data) {
	appendinbox(data);
});

$(document).off("OnOutboxMessage").on("OnOutboxMessage", function (evt, data) {
	appendoutbox(data);
});

$("#messageTarget").addClass("user-tabtarget");