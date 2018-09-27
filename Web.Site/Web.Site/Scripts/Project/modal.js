var token = $('input[name="__RequestVerificationToken"]').val();
$.ajaxPrefilter(function(options, originalOptions) {
	if (options.type.toUpperCase() == "POST") {
		options.data = $.param($.extend(originalOptions.data, { __RequestVerificationToken: token }));
	}
});
$.ajaxSetup({ cache: false });

$(document).ajaxError(function(e, xhr) {
	if (xhr.status == 403) {
		var response = JSON.parse(xhr.responseText);
		window.location = response.LogOnUrl;
	}
});

function openModal(url, loading, data, callback) {
	$.ajax({
		type: "post",
		url: url,
		data: data,
		cache: false,
		success: function(data, textStatus, jqXHR) {
			if (data == "") {
				handleError(jqXHR, "ClientNetworkError", "");
			} else {
				$.modal(data, {
					autoResize: true,
					autoPosition: true,
					onClose: function(dialog, result) {
						dialog.container.fadeOut(200, function() {
							dialog.overlay.fadeOut(150, function() {
								$.modal.close();
								if ($.isFunction(callback)) {
									callback(result);
								}
							});
						});
					},
					onOpen: function(dialog) {
						dialog.overlay.fadeIn(150, function() {
							dialog.container.fadeIn(200);
							dialog.data.fadeIn(200);
						});
					}
				});
			}
		},
		error: handleError
	});

}

function openModalGet(url, data, callback) {
	$.ajax({
		type: "GET",
		url: url,
		data: data,
		cache: false,
		success: function(data, textStatus, jqXHR) {
			if (data == "") {
				handleError(jqXHR, "ClientNetworkError", "");
			} else {
				$.modal(data, {
					autoResize: true,
					autoPosition: true,
					onClose: function(dialog, result) {
						dialog.container.fadeOut("slow");
						dialog.data.slideUp("slow", function() {
							dialog.overlay.fadeOut("slow", function() {
								$.modal.close();
								if ($.isFunction(callback)) {
									callback(result);
								}
							});
						});
					},
					onOpen: function(dialog) {
						dialog.overlay.fadeIn("slow", function() {
							dialog.container.fadeIn("slow");
							dialog.data.slideDown("slow", function() {
							});
						});
					}
				});
			}
		},
		error: handleError
	});

}


function confirm(header, message, callbackYes, callbackNo) {
	$('<div class="modalform modal-dialog">    <div class="modal-content">        <div class="modal-header">            <button class="close simplemodal-close" aria-hidden="true" type="button">×</button>            <h4 class="modal-title header"></h4>        </div>        <div class="modal-body">                <span class="message" >  </span>        </div>        <div class="modal-footer">            <button style="width:70px" class="btn btn-info yes " type="button">' + Resources.General.Yes + '</button>            <button style="width:70px" class="btn btn-info no" type="button">' + Resources.General.No + '</button>        </div>    </div></div>')
		.modal({
			onShow: function(dialog) {
				var modal = this;

				$('.header', dialog.data[0]).append(header);
				$('.message', dialog.data[0]).append(message);

				// if the user clicks "yes"
				$('.yes', dialog.data[0]).click(function() {
					// call the callback
					if ($.isFunction(callbackYes)) {
						callbackYes.apply();
					}
					// close the dialog
					modal.close(); // or $.modal.close();
				});

				$('.no', dialog.data[0]).click(function () {
					// call the callback
					if ($.isFunction(callbackNo)) {
						callbackNo.apply();
					}
					// close the dialog
					modal.close(); // or $.modal.close();
				});
			},
			onClose: function(dialog, result) {
				dialog.container.fadeOut("slow");
				dialog.data.slideUp("slow", function() {
					dialog.overlay.fadeOut("slow", function() {
						$.modal.close();
					});
				});
			},
			onOpen: function(dialog) {
				dialog.overlay.fadeIn("slow", function() {
					dialog.container.fadeIn("slow");
					dialog.data.slideDown("slow", function() {
					});
				});
			}
		});
}


function notify(header, message, callback) {
	$('<div class="modalform modal-dialog">    <div class="modal-content">        <div class="modal-header">            <button class="close simplemodal-close" aria-hidden="true" type="button">×</button>            <h4 class="modal-title header"></h4>        </div>        <div class="modal-body">                <span class="message" >  </span>        </div>        <div class="modal-footer">            <button style="width:70px" class="btn btn-info ok" type="button">' + Resources.General.OK + '</button>                </div>    </div></div>')
		.modal({
			onShow: function(dialog) {
				var modal = this;

				$('.header', dialog.data[0]).append(header);
				$('.message', dialog.data[0]).append(message);

				// if the user clicks "yes"
				$('.ok', dialog.data[0]).click(function() {
					// call the callback
					if ($.isFunction(callback)) {
						callback.apply();
					}
					// close the dialog
					modal.close(); // or $.modal.close();
				});


			},
			onClose: function(dialog, result) {
				dialog.container.fadeOut("slow");
				dialog.data.slideUp("slow", function() {
					dialog.overlay.fadeOut("slow", function() {
						$.modal.close();
					});
				});
			},
			onOpen: function(dialog) {
				dialog.overlay.fadeIn("slow", function() {
					dialog.container.fadeIn("slow");
					dialog.data.slideDown("slow", function() {
					});
				});
			}
		});
}


function getJson(url, vars, callback) {
	return $.ajax({
		url: url,
		cache: false,
		async: true,
		type: "post",
		dataType: 'json',
		data: vars,
		success: function(response, textStatus, jqXHR) {
			if (callback) {
				callback(response);
			}
		},
		error: function(jqXHR, textStatus, errorThrown) {
		}
	});
}

function postJson(url, vars, callback, errorcallback) {
	return $.ajax({
		url: url,
		cache: false,
		async: true,
		type: "POST",
		dataType: 'json',
		data: vars,
		success: function(response, textStatus, jqXHR) {
			if (callback) {
				callback(response);
			}
		},
		error: function(jqXHR, textStatus, errorThrown) {
			if (errorcallback) {
				errorcallback(jqXHR, textStatus, errorThrown);
			}
		}
	});
}


function postHtml(url, vars, callback) {
	return $.ajax({
		url: url,
		cache: false,
		async: true,
		type: "post",
		dataType: 'html',
		data: vars,
		success: function(response, textStatus, jqXHR) {
			if (callback) {
				callback(response);
			}
		},
		error: function(jqXHR, textStatus, errorThrown) {
		}
	});
}

function getJsonSync(url, vars, callback) {
	return $.ajax({
		url: url,
		cache: false,
		async: false,
		type: "post",
		dataType: 'json',
		data: vars,
		success: function(response, textStatus, jqXHR) {
			if (callback) {
				callback(response);
			}
		},
		error: function(jqXHR, textStatus, errorThrown) {
		}
	});
}

function postData(url, vars) {
	return $.ajax({
		url: url,
		cache: false,
		async: false,
		type: "post",
		dataType: 'html',
		data: vars
	});
}

function getData(url, vars, callback) {
	return $.ajax({
		url: url,
		cache: false,
		async: true,
		type: "get",
		dataType: 'json',
		data: vars,
		success: function(response, textStatus, jqXHR) {
			if (callback) {
				callback(response);
			}
		},
		error: function(jqXHR, textStatus, errorThrown) {
		}
	});
}

function getPartial(div, url, showLoading, callback) {
	if (showLoading) {
		$(div).block({ message: Resources.Layout.ModalLoadingMessage });
	}
	$.ajax({
		url: url,
		cache: false,
		async: true,
		type: "GET",
		success: function(response, textStatus, jqXHR) {
			$(div).html(response);
			if (showLoading) {
				$(div).unblock();
			}
			if (callback) {
				callback(response);
			}
		},
		error: function(jqXHR, textStatus, errorThrown) {
			if (showLoading) {
				$(div).unblock();
			}
		}
	});
}


function handleError(jqXHR, textStatus, errorThrown) {
	if (jqXHR.status != 403) {
		$.unblockUI();
		$.modal('<div class="modalform modal-dialog">    <div class="modal-content">        <div class="modal-header">            <button class="close simplemodal-close" aria-hidden="true" type="button">×</button>            <h4 class="modal-title">' + Resources.Layout.ModalInvalidRequestTitle + '</h4>        </div>        <div class="modal-body">            <p>' + Resources.Layout.modalInvalidRequestMessage1 + '</p>            <p>' + Resources.Layout.modalInvalidRequestMessage2 + '</p>        </div>         <div class="modal-footer">            <button style="width:70px" class="btn btn-info simplemodal-close" type="button">' + Resources.General.OK + '</button>        </div>    </div></div>');
	}
}