var notificationTemplate = $("#notificationTemplate").html();

function initializeWebsocket() {
	//$.connection.hub.logging = true;
	notificationHub.client.SendNotification = function (notification) {
		sendNotification(notification.Header, notification.Notification, notification.Type);
	};
	notificationHub.client.SendDataNotification = function (notification) {
		$(document).trigger(notification.Event, notification);
		$(document).trigger(notification.Event + "Global", notification);
	};

	$.connection.hub.start({ transport: ['webSockets'] });
}


function sendNotification(header, message, type) {
	var html = Mustache.render(notificationTemplate, {
			header: header,
			message: message,
			type: notificationTypeToText(type),
			icon: notificationTypeToIcon(type)
		});
	$.jGrowl(html, { position: "bottom-right" });
}

function notificationTypeToIcon(type) {
	if (type === 1) {
		return "fa-exclamation-triangle";
	} else if (type === 2) {
		return "fa-times";
	} else if (type === 3) {
		return "fa-check-circle-o";
	}
	return "fa-info-circle";
}
function notificationTypeToText(type) {
	if (type === 1) {
		return "warning";
	} else if (type === 2) {
		return "error";
	} else if (type === 3) {
		return "success";
	}
	return "info";
}



function cancelOrder(tradeId, tradePairId) {
	var data = {
		tradeId: tradeId,
		tradePairId: tradePairId
	};

	$.blockUI({ message: Resources.Layout.BlockCancelOrderMessage });
	getJson('/Exchange/CancelTrade', data, orderCanceled);
}

function cancelAllOrders() {
	confirm(Resources.Layout.ConfirmCancelAllOrdersTitle, Resources.Layout.confirmCancelAllOrdersMessage, function () {
		$.blockUI({ message: Resources.Layout.BlockCancelAllOrdersMessage });
		var data = {};
		getJson('/Exchange/CancelAllTrades', data, allordersCanceled);
	});
}

function cancelTradePairOrders(tradePairId) {
	confirm(Resources.Layout.ConfirmCancelTradePairTitle, Resources.Layout.ConfirmCancelTradePairTitleMessage, function () {
		$.blockUI({ message: Resources.Layout.BlockCancelTradePairMessage });
		var data = { tradePairId: tradePairId };
		getJson('/Exchange/CancelTradePairTrades', data, allordersCanceled);
	});
}

function orderCanceled(response) {
	$.unblockUI();
}

function allordersCanceled(response) {
	$.unblockUI();
}

function htmlEncode(val) {
	return $('<div/>').text(val).html();
}

function scrollToAnchor(aid) {
	var aTag = $("a[name='" + aid + "']");
	if (aTag && aTag.offset()) {
		$('html, body').animate({ scrollTop: aTag.offset().top }, 'fast');
	}
}

$("#theme-switch").on("click", function () {
	var url = $(this).data("action");
	postJson(url, {}, function (data) {
		if (data.Success) {
			changeTheme(data.Message);
		}
	});
});

function changeTheme(theme) {
	if (theme == 'Dark') {
		$("#theme-switch").attr('title', Resources.Layout.ThemeLight);
		$("#theme-switch i").removeClass('icon-moon-night').addClass('icon-lightbulb-idea');
		$('link[title="siteTheme"]').attr({ href: "/Content/theme.Dark.css" });
	}
	else if (theme == 'Light') {
		$("#theme-switch").attr('title', Resources.Layout.ThemeDark);
		$("#theme-switch i").removeClass('icon-lightbulb-idea').addClass('icon-moon-night');
		$('link[title="siteTheme"]').attr({ href: "/Content/theme.Light.css" });
	}
}

// Set caret position easily in jQuery
// Written by and Copyright of Luke Morton, 2011
// Licensed under MIT
(function ($) {
	// Behind the scenes method deals with browser
	// idiosyncrasies and such
	$.caretTo = function (el, index) {
		if (el.createTextRange) {
			var range = el.createTextRange();
			range.move("character", index);
			range.select();
		} else if (el.selectionStart != null) {
			el.focus();
			el.setSelectionRange(index, index);
		}
	};

	// The following methods are queued under fx for more
	// flexibility when combining with $.fn.delay() and
	// jQuery effects.

	// Set caret to a particular index
	$.fn.caretTo = function (index, offset) {
		return this.queue(function (next) {
			if (isNaN(index)) {
				var i = $(this).val().indexOf(index);

				if (offset === true) {
					i += index.length;
				} else if (offset) {
					i += offset;
				}

				$.caretTo(this, i);
			} else {
				$.caretTo(this, index);
			}

			next();
		});
	};

	// Set caret to beginning of an element
	$.fn.caretToStart = function () {
		return this.caretTo(0);
	};

	// Set caret to the end of an element
	$.fn.caretToEnd = function () {
		return this.queue(function (next) {
			$.caretTo(this, $(this).val().length);
			next();
		});
	};
}(jQuery));


(function ($) {

	/**
		 * Set all elements within the collection to have the same height.
		 */
	$.fn.equalHeight = function () {
		var heights = [];
		$.each(this, function (i, element) {
			$element = $(element);
			var element_height;
			// Should we include the elements padding in it's height?
			var includePadding = ($element.css('box-sizing') == 'border-box') || ($element.css('-moz-box-sizing') == 'border-box');
			if (includePadding) {
				element_height = $element.innerHeight();
			} else {
				element_height = $element.height();
			}
			heights.push(element_height);
		});
		this.css('height', Math.max.apply(window, heights) + 'px');
		return this;
	};

	/**
		 * Create a grid of equal height elements.
		 */
	$.fn.equalHeightGrid = function (columns) {
		var $tiles = this;
		$tiles.css('height', 'auto');
		for (var i = 0; i < $tiles.length; i++) {
			if (i % columns === 0) {
				var row = $($tiles[i]);
				for (var n = 1; n < columns; n++) {
					row = row.add($tiles[i + n]);
				}
				row.equalHeight();
			}
		}
		return this;
	};

	/**
		 * Detect how many columns there are in a given layout.
		 */
	$.fn.detectGridColumns = function () {
		var offset = 0, cols = 0;
		this.each(function (i, elem) {
			var elem_offset = $(elem).offset().top;
			if (offset === 0 || elem_offset === offset) {
				cols++;
				offset = elem_offset;
			} else {
				return false;
			}
		});
		return cols;
	};

	/**
		 * Ensure equal heights now, on ready, load and resize.
		 */
	$.fn.responsiveEqualHeightGrid = function () {
		var _this = this;

		function syncHeights() {
			var cols = _this.detectGridColumns();
			_this.equalHeightGrid(cols);
		}

		$(window).bind('resize load', syncHeights);
		syncHeights();
		return this;
	};

})(jQuery);

if (!String.linkify) {
	String.prototype.linkify = function () {

		// http://, https://, ftp://
		var urlPattern = /\b(?:https?|ftp):\/\/[a-z0-9-+&@#\/%?=~_|!:,.;]*[a-z0-9-+&@#\/%=~_|]/gim;

		// www. sans http:// or https://
		var pseudoUrlPattern = /(^|[^\/])(www\.[\S]+(\b|$))/gim;

		// Email addresses
		var emailAddressPattern = /[\w.]+@[a-zA-Z_-]+?(?:\.[a-zA-Z]{2,6})+/gim;

		if (this.match(/bitcointalk.org/g)) {
			return '<a rel="noopener noreferrer" target="_blank" href="https://www.youtube.com/watch?v=dQw4w9WgXcQ">'
				+ Resources.Layout.LinkifyForbiddenLink + '</a>';
		}

		return this
			.replace(urlPattern, '<a rel="noopener noreferrer" target="_blank" href="$&">$&</a>')
			.replace(pseudoUrlPattern, '$1<a rel="noopener noreferrer" target="_blank" href="http://$2">$2</a>')
			.replace(emailAddressPattern, '<a rel="noopener noreferrer" target="_blank" href="mailto:$&">$&</a>');
	};
}

String.format = function () {
	var s = arguments[0];
	for (var i = 0; i < arguments.length - 1; i++) {
		var reg = new RegExp("\\{" + i + "\\}", "gm");
		s = s.replace(reg, arguments[i + 1]);
	}
	return s;
};


function getStarRating(ratingValue) {
	var rating = "";
	if (ratingValue === "" || ratingValue == -1) {
		for (i = 0; i < 5; i++) {
			rating += '<i title="' + Resources.Layout.RatingUnrated + '" class="fa fa-star-o"></i>';
		}
		return rating;
	}

	if (ratingValue == 0) {
		for (i = 0; i < 5; i++) {
			rating += '<i title="0/5" class="fa fa-star-o text-danger"></i>';
		}
		return rating;
	}

	for (i = 0; i < 5; i++) {
		var title = Resources.Layout.RatingRated + ' ' + ratingValue + '/5';
		if (i < ratingValue) {			
			if (ratingValue % 1 != 0 && ratingValue - 0.5 == i) {
				if (ratingValue >= 5) {
					rating += '<i title="' + title + '" class="fa fa-star-half-o text-warning"></i>';
				} else if (ratingValue > 2) {
					rating += '<i title="' + title + '" class="fa fa-star-half-o text-success"></i>';
				} else {
					rating += '<i title="' + title + '" class="fa fa-star-half-o text-danger"></i>';
				}
			} else {
				if (ratingValue >= 5) {
					rating += '<i title="' + title + '" class="fa fa-star text-warning"></i>';
				} else if (ratingValue > 2) {
					rating += '<i title="' + title + '" class="fa fa-star text-success"></i>';
				} else {
					rating += '<i title="' + title + '" class="fa fa-star text-danger"></i>';
				}
			}
		} else {
			rating += '<i title="' + title + '" class="fa fa-star-o"></i>';
		}
	}
	return rating;
}


function showMessage(data) {
	if (data && !data.Cancel) {
		var message = data.Message || (Resources.Layout.ErrorContactSupportText + ' <a href="/UserSupport">' + Resources.Layout.ErrorContactSupportLink + '.</a>');
		var alert = $("#message-alert");
		alert.show();
		alert.addClass(data.Success ? "alert-success" : "alert-danger");
		alert.find("p").html(message);
		alert.fadeTo(5000, 500).slideUp(500, function () {
			alert.find("p").html("");
			alert.removeClass("alert-danger alert-success").hide();
		});
	}
}

setInterval(function () {
	$(".servertime-label").text(moment.utc().format("D/MM/YYYY h:mm:ss A"))
}, 1000);

function toLocalTime(time) {
	return moment.utc(time).local().format("D/MM/YYYY h:mm:ss A")
}

function toLocalDate(date) {
	return moment.utc(date).local().format("D/MM/YYYY")
}


$(function () {
	// enable tootips
	$("body").tooltip({ selector: '[data-toggle=tooltip]' });
	$("body").popover({ selector: '[data-toggle=popover]' });

	$('#notification-menu').on('mouseenter', function () {
		getData('/UserNotification/GetNotificationMenu', {}, function (data) {
			var notifications = $("#notification-menu-notifications");
			notifications.empty();
			if (data && data.length === 0) {
				notifications.append("<div style='text-align:center;font-size:11px'><i>" + Resources.Layout.MenuNoNotification + "</i></div>");
				return;
			}
			for (var i in data) {
				notifications.append("<div style='font-size:11px'>" + moment.utc(data[i].Timestamp).local().fromNow() + ": " + data[i].Title + " - " + data[i].Notification + "</div>")
			}
		});
	});

	$('#notification-menu-clear').on('click', function () {
		getJson('/UserNotification/Clear', {}, function (data) {
			$('.notification-menu-count').html('');
			$("#notification-menu-notifications").empty();
		});
	});
	if (authenticated == 'True') {
		updateMessageCount();
	}
});


function updateMessageCount() {
	getData('/UserNotification/GetNotificationCount', {}, function (data) {
		$('.notification-menu-message-count').html(data.MessageCount == 0 ? "" : data.MessageCount > 1000 ? "999+" : data.MessageCount);
		$('.notification-menu-count').html(data.NotificationCount == 0 ? "" : data.NotificationCount > 1000 ? "999+" : data.NotificationCount);
	});
}

function setSelectedNavOption(title) {
	if (title) {
		$('#nav-header').removeClass('active');
		$('.nav-' + title.toLowerCase()).addClass('active');
	}
}

function printObj(object) {
	var simpleObject = {};
	for (var prop in object) {
		if (!object.hasOwnProperty(prop)) {
			continue;
		}
		if (typeof (object[prop]) == 'object') {
			continue;
		}
		if (typeof (object[prop]) == 'function') {
			continue;
		}
		simpleObject[prop] = object[prop];
	}
	return JSON.stringify(simpleObject); // returns cleaned up JSON
};

var datatableExportLayout = "<'row'<'col-sm-6'><'col-sm-6'f>><'row'<'col-sm-12'tr>><'datatable-length-row'l> <'datatable-export-row'B><'datatable-page-row'p><'clearfix'><'datatable-info-row'i>";
function datatableExportButtons(filename) {
	return [{
		extend: 'csvHtml5',
		text: '<span data-toggle="tooltip" data-container="body" title="' + Resources.Layout.ExportCsvButton +'" class="fa fa-file-text-o"></span>',
		filename: filename.replace(" ", "_")
	},
	{
		extend: 'excelHtml5',
		text: '<span data-toggle="tooltip" data-container="body" title="' + Resources.Layout.ExportExcelButton +'" class="fa fa-file-excel-o"></span>',
		filename: filename.replace(" ", "_")
	},
	{
		extend: 'pdfHtml5',
		text: '<span data-toggle="tooltip" data-container="body" title="' + Resources.Layout.ExportPdfButton +'" class="fa fa-file-pdf-o"></span>',
		filename: filename.replace(" ", "_"),
		title: 'Cryptopia ' + filename
	},
	{
		extend: 'copy',
		text: '<span data-toggle="tooltip" data-container="body" title="' + Resources.Layout.ExportClipboardButton +'" class="fa fa-files-o"></span>',
	},
	{
		extend: 'print',
		text: '<span data-toggle="tooltip" data-container="body" title="' + Resources.Layout.ExportPrintButton +'" class="fa fa-print"></span>',
		title: 'Cryptopia ' + filename
	}]
};

function triggerWindowResize() {
	if (typeof (Event) === 'function') {
		// modern browsers
		window.dispatchEvent(new Event('resize'));
	} else {
		// for IE and other old browsers
		// causes deprecation warning on modern browsers
		var evt = window.document.createEvent('UIEvents');
		evt.initUIEvent('resize', true, false, window, 0);
		window.dispatchEvent(evt);
	}

}

var store = new Storage();
function Storage(authenticated) {

	this.get = function (name) {
		return JSON.parse(window.localStorage.getItem(name));
	};

	this.set = function (name, value) {
		window.localStorage.setItem(name, JSON.stringify(value));
	};

	this.clear = function () {
		window.localStorage.clear();
	};
}

function changeHighlight(change) {
	return change > 0 ? "text-success" : change < 0 ? "text-danger" : "";
}

function highlightRow(element, highlight) {
	if (element.hasClass("greenhighlight") || element.hasClass("redhighlight") || element.hasClass("bluehighlight")) {
		element.removeClass("greenhighlight redhighlight bluehighlight").addClass(highlight + "highlight2");
	} else {
		element.removeClass("greenhighlight2 redhighlight2 bluehighlight2").addClass(highlight + "highlight")
	}
}

function highlightRowText(element, highlight) {
	if (element.hasClass("greenhighlighttext") || element.hasClass("redhighlighttext") || element.hasClass("bluehighlighttext")) {
		element.removeClass("greenhighlighttext redhighlighttext bluehighlighttext").addClass(highlight + "highlighttext2");
	} else {
		element.removeClass("greenhighlighttext2 redhighlighttext2 bluehighlighttext2").addClass(highlight + "highlighttext")
	}
}

function highlightItem(element, highlight) {
	if (element.hasClass("info")) {
		highlightRowText(element, highlight);
	}
	else {
		highlightRow(element, highlight);
	}
}

function highlightRemove(selector) {
	$(selector).removeClass("greenhighlighttext2 redhighlighttext2 bluehighlighttext2 greenhighlighttext redhighlighttext bluehighlighttext greenhighlight redhighlight bluehighlight greenhighlight2 redhighlight2 bluehighlight2");
}