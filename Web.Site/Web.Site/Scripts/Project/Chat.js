function ChatModule(container) {

	var chatHub;
	var _this = this;
	var chatWindow = null;
	var chatHistoryLoaded = false;
	var chatEmoticonsLoaded = false;
	var chatTable = $(container).find(".chat-messages tbody");

	var chatBotTemplate = $("#chatBotTemplate").html();
	var chatUserTemplate = $("#chatUserTemplate").html();
	var chatTipbotTemplate = $("#chatTipbotTemplate").html();

	var onlineCountInterval;

	this.initializeChat = function() {
		if (!chatEmoticonsLoaded) {
			$.getJSON('/Chat/EmoticonSet', function (json) {
				var smilies = {};
				for (var key in json) {
					if (json.hasOwnProperty(key)) {
						var item = json[key];
						smilies[item.Code] = [item.Path, item.Name];
					};
				}
				emotify.emoticons(cdnImagePath + '/Content/Images/Emoticonset/', smilies);
				chatEmoticonsLoaded = true;
				loadChatHistory();
			});
		}
		else {
			loadChatHistory();
		}

		if (!chatHub) {
			$.connection.hub.stop()

			chatHub = $.connection.chatHub;
			// receive chat message from server
			chatHub.client.broadcastMessage = function (chatdata) {
				var shouldScroll = shouldChatScroll();
				addChatMessage(chatdata);
				if (shouldScroll == true) {
					scrollChat();
				}
			};

			// server has sent a request to remove chat post
			chatHub.client.broadcastRemoval = function (chatmsgid) {
				$('.chat-messages tbody > tr.chat-msgid-' + chatmsgid).remove();
			};

			// receive online count and time from server
			chatHub.client.broadcastOnlineCount = function (count, time) {
				$('.chat-messages tr').each(function () {
					$(this).find('small').each(function () {
						$(this).html(timeSince(new Date(time), new Date($(this).data('val'))));
					});
				});
				$('.chat-online-count').html(count);
			};

			$.connection.hub.start({ transport: ['webSockets'] }).done(function () {
				clearInterval(onlineCountInterval);
				onlineCountInterval = setInterval(function () {
					_this.getOnlineCount();
				}, 30 * 1000);
				_this.getOnlineCount();
			});
		}
		else {
			_this.getOnlineCount();
		}
	}

	this.detroy = function () {
		clearInterval(onlineCountInterval);
	}
	
	this.refreshPopout = function () {
		settings = "width=480, height=640, scrollbars=no, location=no, directories=no, status=no, menubar=no, toolbar=no, resizable=yes, dependent=no";
		chatWindow = window.open('/Chat/Index', 'Trollbox', settings);
		chatWindow.focus();
	}

	this.reload = function () {
		chatHistoryLoaded = false;
		chatEmoticonsLoaded = false;
		_this.initializeChat();
	}

	this.sendChatMessage = function (scroll) {
		var inputBox = $(container).find('.chat-message');
		var msg = inputBox.val();
		if (msg.length !== 0) {
			postJson('/Chat/SendChatMessage', { message: msg });
			inputBox.val('').focus();
			if (scroll) {
				scrollChat();
			}
		}
	}

	this.removeChatPost = function(chatId) {
		confirm(Resources.Chat.RemovePostQuestionTitle, Resources.Chat.RemovePostQuestion, function () {
			postJson('/Chat/RemoveChatPost', { chatId: chatId });
		});
	}

	this.getOnlineCount = function() {
		postJson('/Chat/GetOnlineCount');
	}

	this.banChatUser = function(user) {
		openModalGet('/Chat/BanChatUser', { chathandle: user });
	}

	this.ignoreChatUser = function(user) {
		getJson('/UserSettings/SubmitChatIgnore', { username: user }, function (data) {
			notify(Resources.Chat.IgnoreNotificationTitle, data.Message);
		});
	}

	this.tipCurrency = function() {
		openModal('/Balance/TipCurrency', Resources.General.LoadingMessage, {}, function () { });
	}

	this.ignoreTipUser = function(user) {
		getJson('/UserSettings/SubmitTipIgnore', { username: user }, function (data) {
			notify(Resources.Chat.TipIgnoreNotificationTitle, data.Message);
		});
	}

	this.sendChatKarma = function(chatHandle, messageId) {
		getJson('/Karma/SubmitChatKarma', { chatHandle: chatHandle, chatMessageId: messageId }, function (data) {
			var encodedMsg = htmlEncode(data.Message);
			if (data.Success) {
				$('.karma-' + data.User).html(data.Count);
			}
			sendNotification(Resources.Chat.SendKarmaNotificationTitle, encodedMsg, data.Success ? 0 : 2);
		});
	}

	this.sendTipKarma = function(chatHandle, messageId) {
		getJson('/Karma/SubmitTipKarma', { chatHandle: chatHandle, chatMessageId: messageId }, function (data) {
			var encodedMsg = htmlEncode(data.Message);
			if (data.Success) {
				$('.karma-' + data.User).html(data.Count);
			}
			sendNotification(Resources.Chat.TipKarmaNotificationTitle, encodedMsg, data.Success ? 0 : 2);
		});
	}

	this.emoticonPicker = function () {
		openModalGet('/Chat/Emoticons');
	}
	

	$(container).on("click", ".chat-send", function () {
		_this.sendChatMessage(true);
	});

	$(container).on("keyup", ".chat-message", function (event) {
		if (event.keyCode == 13 && !event.shiftKey) {
			_this.sendChatMessage(true);
			return false;
		}
	});

	$(container).on("click", ".chat-item-username", function () {
		var inputBox = $(container).find('.chat-message');
		if (inputBox) {
			inputBox.val(inputBox.val() + '@' + $(this).text() + ', ');
			inputBox.focus();
			inputBox.caretToEnd();
		}
	});

	$(container).on("click", ".chat-remove-message", function () {
		_this.removeChatPost($(this).data("messageid"));
	});
	
	$(container).on("click", ".chat-karma-message", function () {
		var user = $(this).data("user")
		var messageId = $(this).data("messageid");
		_this.sendChatKarma(user, messageId);
	});

	$(container).on("click", ".chat-karma-tip", function () {
		var user = $(this).data("user")
		var messageId = $(this).data("messageid");
		_this.sendTipKarma(user, messageId);
	});

	$(container).on("click", ".chat-ban-user", function () {
		var user = $(this).data("user")
		_this.banChatUser(user);
	});

	$(container).on("click", ".chat-ignore-user", function () {
		var user = $(this).data("user")
		_this.ignoreChatUser(user);
	});

	$(container).on("click", ".chat-ignore-tip", function () {
		var user = $(this).data("user")
		_this.ignoreTipUser(user);
	});

	$(container).on("click", ".chat-option-tip", function () {
		_this.tipCurrency();
	});


	$(container).on("click", ".chat-option-emoticons", function () {
		_this.emoticonPicker();
	});

	$(container).on("click", ".chat-refresh, .chat-popout", function () {
		_this.refreshPopout();
	});

	function loadChatHistory() {
		if (chatHistoryLoaded) {
			scrollChat();
		} else {
			postJson('/Chat/GetChatHistory', {}, function (chathistory) {
				var messages = [];
				if (chathistory.data) {
					chatTable.empty();
					for (i = 0; i < chathistory.data.length; i++) {
						addChatMessage(chathistory.data[i]);
					}
				}

				$('.chat-messages tr td').each(function () {
					$(this).find('small').each(function () {
						$(this).html(timeSince(new Date(chathistory.time), new Date($(this).data('val'))));
					});
				});
				scrollChat();
				chatHistoryLoaded = true;
			});
		}
	}

	function scrollChat() {
		var chatPanel = $(container).find('.chat-container');
		if (chatPanel && chatPanel[0]) {
			chatPanel.scrollTop(chatPanel[0].scrollHeight);
		}
	}

	function shouldChatScroll() {
		var chatPanel = $(container).find('.chat-container');
		if (chatPanel && chatPanel[0]) {
			if (chatPanel.scrollTop() + 70 >= chatPanel[0].scrollHeight - chatPanel.innerHeight()) {
				return true;
			}
		}
		return false;
	}

	function addChatMessage(chatdata) {
		var ismine = chatdata.UserName == authenticatedUserName;
		if (chatdata.IsBot) {
			chatTable.append(Mustache.render(chatBotTemplate, {
				Id: chatdata.Id,
				UserName: chatdata.UserName,
				ChatHandle: chatdata.ChatHandle,
				Message: emotify(chatdata.Message.linkify()),
				IsAdmin: authenticatedAdmin == "True" ? "" : "none",
				IsMine: chatdata.ChatHandle == authenticatedChatHandle || authenticated == "False" ? "none" : ""
			}));
		}
		else if (chatdata.IsTipBot) {
			chatTable.append(Mustache.render(chatTipbotTemplate, {
				Id: chatdata.Id,
				UserName: chatdata.UserName,
				ChatHandle: chatdata.ChatHandle,
				Message: emotify(chatdata.Message.linkify()),
				IsAdmin: authenticatedAdmin == "True" ? "" : "none",
				IsMine: chatdata.ChatHandle == authenticatedChatHandle || authenticated == "False" ? "none" : ""
			}));
		}
		else {
			var username = "@" + authenticatedChatHandle + ","
			var highlight = chatdata.Message.indexOf(username) > -1;
			var flair = getFlair(chatdata.Flair);
			chatTable.append(Mustache.render(chatUserTemplate, {
				Id: chatdata.Id,
				UserName: chatdata.UserName,
				ChatHandle: chatdata.ChatHandle,
				Flair: flair,
				KarmaTotal: chatdata.KarmaTotal,
				Timestamp: chatdata.Timestamp,
				Avatar: chatdata.Avatar,
				Message: emotify(chatdata.Message.linkify()),
				Highlight: highlight ? "chat-highlight-usertouser" : "",
				IsAdmin: authenticatedAdmin == "True" ? "" : "none",
				IsMine: ismine || authenticated == "False" ? "none" : ""
			}));
		}
	}

	function getFlair(flairCss) {
		var flair = '';
		if (flairCss) {
			var flairItems = flairCss.split("|");
			for (k = 0; k < flairItems.length; k++) {
				flair += '<i style="padding-right:2px" class="' + flairItems[k] + '" ></i>'
			}
		}
		return flair;
	}
	
	function timeSince(time1, time2) {
		var val;
		var seconds = Math.floor((time1 - time2) / 1000);

		if (seconds < 60) {
			val = Math.floor(seconds);
			if (val > 3) {
				return String.format(Resources.Chat.SinceSecondsLabel, Math.floor(seconds));
			}
			return Resources.Chat.SinceSecondLabel;
		}
		if (seconds < 3600) {
			val = Math.floor((seconds / 60));
			if (val > 2) {
				return String.format(Resources.Chat.SinceMinutesLabel, val);
			}
			return Resources.Chat.SinceMinuteLabel;
		}
		if (seconds < 86400) {
			val = Math.floor(((seconds / 60) / 60));
			if (val > 1) {
				return String.format(Resources.Chat.SinceHoursLabel, val);
			}
			return Resources.Chat.SinceHourLabel;
		}
		return Resources.Chat.SinceAgesLabel;
	}
}