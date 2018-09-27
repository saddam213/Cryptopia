
function postJson(url, vars, callback, errorcallback) {
	$.ajax({
		url: url,
		cache: false,
		async: true,
		type: "POST",
		dataType: 'json',
		data: vars,
		success: function (response, textStatus, jqXHR) {
			if (callback) {
				callback(response);
			}
		},
		error: function (jqXHR, textStatus, errorThrown) {
			if (errorcallback) {
				errorcallback(jqXHR, textStatus, errorThrown);
			}
		}
	});
}





function openModalGet(url, data, callback) {
	$.ajax({
		type: "GET",
		url: url,
		data: data,
		cache: false,
		success: function (data, textStatus, jqXHR) {
			if (data == "") {
				handleError(jqXHR, "ClientNetworkError", "");
			} else {
				$.modal(data, {
					autoResize: true,
					autoPosition: true,
					onClose: function (dialog, result) {
						dialog.container.fadeOut("fast");
						dialog.data.fadeOut("fast", function () {
							dialog.overlay.fadeOut("fast", function () {
								$.modal.close();
								if ($.isFunction(callback)) {
									callback(result);
								}
							});
						});
					},
					onOpen: function (dialog) {
						dialog.overlay.fadeIn("fast", function () {
							dialog.container.fadeIn("fast");
							dialog.data.fadeIn("fast", function () {
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
	$('<div class="modalform modal-dialog">    <div class="modal-content">        <div class="modal-header">            <button class="close simplemodal-close" aria-hidden="true" type="button">×</button>            <h4 class="modal-title header"></h4>        </div>        <div class="modal-body">                <span class="message" >  </span>        </div>        <div class="modal-footer">            <button style="width:70px" class="btn btn-info yes " type="button">Yes</button>            <button style="width:70px" class="btn btn-info no" type="button">No</button>        </div>    </div></div>')
		.modal({
			onShow: function (dialog) {
				var modal = this;

				$('.header', dialog.data[0]).append(header);
				$('.message', dialog.data[0]).append(message);

				// if the user clicks "yes"
				$('.yes', dialog.data[0]).click(function () {
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
			onClose: function (dialog, result) {
				dialog.container.fadeOut("slow");
				dialog.data.slideUp("slow", function () {
					dialog.overlay.fadeOut("slow", function () {
						$.modal.close();
					});
				});
			},
			onOpen: function (dialog) {
				dialog.overlay.fadeIn("slow", function () {
					dialog.container.fadeIn("slow");
					dialog.data.slideDown("slow", function () {
					});
				});
			}
		});
}


function notify(header, message, callback) {
	$('<div class="modalform modal-dialog">    <div class="modal-content">        <div class="modal-header">            <button class="close simplemodal-close" aria-hidden="true" type="button">×</button>            <h4 class="modal-title header"></h4>        </div>        <div class="modal-body">                <span class="message" >  </span>        </div>        <div class="modal-footer">            <button style="width:70px" class="btn btn-info ok" type="button">Ok</button>                </div>    </div></div>')
		.modal({
			onShow: function (dialog) {
				var modal = this;

				$('.header', dialog.data[0]).append(header);
				$('.message', dialog.data[0]).append(message);

				// if the user clicks "yes"
				$('.ok', dialog.data[0]).click(function () {
					// call the callback
					if ($.isFunction(callback)) {
						callback.apply();
					}
					// close the dialog
					modal.close(); // or $.modal.close();
				});


			},
			onClose: function (dialog, result) {
				dialog.container.fadeOut("slow");
				dialog.data.slideUp("slow", function () {
					dialog.overlay.fadeOut("slow", function () {
						$.modal.close();
					});
				});
			},
			onOpen: function (dialog) {
				dialog.overlay.fadeIn("slow", function () {
					dialog.container.fadeIn("slow");
					dialog.data.slideDown("slow", function () {
					});
				});
			}
		});
}

var token = $('input[name="__RequestVerificationToken"]').val();
$.ajaxPrefilter(function (options, originalOptions) {
	if (options.type.toUpperCase() == "POST") {
		options.data = $.param($.extend(originalOptions.data, { __RequestVerificationToken: token }));
	}
});
$.ajaxSetup({ cache: false });

$(document).ajaxError(function (e, xhr) {
	if (xhr.status == 403) {
		var response = JSON.parse(xhr.responseText);
		window.location = response.LogOnUrl;
	}
});

function handleError(jqXHR, textStatus, errorThrown) {
	alert(errorThrown)
	if (jqXHR.status != 403) {
		$.unblockUI();
		$.modal('<div class="modalform modal-dialog">    <div class="modal-content">        <div class="modal-header">            <button class="close simplemodal-close" aria-hidden="true" type="button">×</button>            <h4 class="modal-title">Invalid Request</h4>        </div>        <div class="modal-body">            <p>Your request was not formed properly. Please refresh the page and try again.</p>            <p>If the problem continues, please contact support.</p>        </div>         <div class="modal-footer">            <button style="width:70px" class="btn btn-info simplemodal-close" type="button">OK</button>        </div>    </div></div>');
	}
}






















function showMessage(data) {
	if (data && !data.Cancel) {
		var message = data.Message || 'An error occured, if problem persists please contact <a href="/UserSupport">Cryptopia Support.</a>';
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

function getStarRating(ratingValue) {
	var rating = "";
	if (ratingValue === "" || ratingValue == -1) {
		for (i = 0; i < 5; i++) {
			rating += '<i title="Unrated" class="fa fa-star-o"></i>';
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
		if (i < ratingValue) {
			if (ratingValue % 1 != 0 && ratingValue - 0.5 == i) {
				if (ratingValue >= 5) {
					rating += '<i title="Rated ' + ratingValue + '/5" class="fa fa-star-half-o text-warning"></i>';
				} else if (ratingValue > 2) {
					rating += '<i title="Rated ' + ratingValue + '/5" class="fa fa-star-half-o text-success"></i>';
				} else {
					rating += '<i title="Rated ' + ratingValue + '/5" class="fa fa-star-half-o text-danger"></i>';
				}
			} else {
				if (ratingValue >= 5) {
					rating += '<i title="Rated ' + ratingValue + '/5" class="fa fa-star text-warning"></i>';
				} else if (ratingValue > 2) {
					rating += '<i title="Rated ' + ratingValue + '/5" class="fa fa-star text-success"></i>';
				} else {
					rating += '<i title="Rated ' + ratingValue + '/5" class="fa fa-star text-danger"></i>';
				}
			}
		} else {
			rating += '<i title="Rated ' + ratingValue + '/5" class="fa fa-star-o"></i>';
		}
	}
	return rating;
}