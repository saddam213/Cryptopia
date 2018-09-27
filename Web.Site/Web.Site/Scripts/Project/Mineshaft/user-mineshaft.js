var actionWorkers = $('#wrapper').data('action-workers');
var actionupdate = $('#wrapper').data("action-update");
var actionupdatepool = $('#wrapper').data("action-updatepool");
var workerTable = $('#userWorkers').DataTable({
	"order": [[3, "desc"]],
	"lengthChange": false,
	"processing": false,
	"bServerSide": false,
	"searching": false,
	"paging": false,
	"sort": true,
	"info": false,
	"scrollX": false,
	"scrollY": $('.stackmenu-body').height() - 20,
	"sAjaxSource": actionWorkers,
	"sServerMethod": "POST",
	"language": { "emptyTable": "No workers found.", "sZeroRecords": "No workers found." }, //
	"columnDefs": [
		{
			"targets": [0],
			"render": function (data, type, full, meta) {
				return '<a onclick="updateWorker(' + data + ')">' + full[1].replace(username + '.', '') + '</a>';
			}
		},
		{
			"targets": [1],
			"render": function (data, type, full, meta) {
				return full[2];
			}
		},
		{
			"targets": [2],
			"iDataSort": 3,
			"render": function (data, type, full, meta) {
				return '<span class="worker-hashrate-' + full[0] + '">' + hashrateLabel(full[3]) + '</span>';
			}
		},
		{ "targets": [3], "visible": false },
		{
			"targets": [4],
			"render": function (data, type, full, meta) {
				return '<a href="/Mineshaft?Algo=' + full[2] + '&Pool=' + full[5] + '">' + full[5] + '</a>';
			}
		}
	],
	"fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
	},
	"fnDrawCallback": function (oSettings, adata) {
	}
});

function updatePool(id) {
	openModalGet(actionupdatepool, { id: id }, function (data) {
		if (data && data.Success) {
			workerTable.ajax.reload();
			showMessage(data);
		}
	});
}

function updateWorker(id) {
	openModalGet(actionupdate, { id: id }, function (data) {
		if (data && data.Success) {
			workerTable.ajax.reload();
			showMessage(data);
		}
	});
}

var actionRentals = $('#wrapper').data('action-rentals');
$('#userRentals').dataTable({
	"order": [[0, "asc"]],
	"lengthChange": false,
	"processing": false,
	"bServerSide": false,
	"searching": false,
	"paging": false,
	"sort": true,
	"info": false,
	"scrollX": false,
	"scrollY": $('.stackmenu-body').height(),
	"sAjaxSource": actionRentals,
	"sServerMethod": "POST",
	"language": { "emptyTable": "You have no active rentals.", "sZeroRecords": "You have no active rentals." },
	"columnDefs": []
});

$("#workersSearch").keyup(function () {
	var _this = $(this);
	$.each($("#userWorkers tbody").find(".even, .odd"), function () {
		var _row = $(this);
		var data = '';
		$.each(_row.find("td"), function () {
			data += $(this).html();
		});
		if (data.toLowerCase().indexOf($(_this).val().toLowerCase()) == -1)
			_row.hide();
		else
			_row.show();
	});
});

$("#rentalsSearch").keyup(function () {
	var _this = $(this);
	$.each($("#userRentals tbody").find(".even, .odd"), function () {
		var _row = $(this);
		var data = '';
		$.each(_row.find("td"), function () {
			data += $(this).html();
		});
		if (data.toLowerCase().indexOf($(_this).val().toLowerCase()) == -1)
			_row.hide();
		else
			_row.show();
	});
});



mineshaftHub.client.SendUserDataNotification = function (notification) {
	if (typeof mineshaftUserDataUpdate === 'function') {
		mineshaftUserDataUpdate(notification);
	}
	
	if (notification.Type == 4) {
		$('.worker-hashrate-' + notification.Id).html(hashrateLabel(notification.Hashrate));
	}
}

function updateUserWorkers() {
	$('#userWorkers').DataTable().ajax.reload(function () { });
}
function updateRentals() {
	$('#userRentals').DataTable().ajax.reload(function () { });
}


$(".pools-menu-btn").on("click", function () {
	$(".workers-menu, .rentals-menu").hide();
	$(".workers-menu-btn, .rentals-menu-btn").removeClass("active");
	$(".pools-menu").show();
	$(".pools-menu-btn").addClass("active");
});

$(".workers-menu-btn").on("click", function () {
	updateUserWorkers();
	$(".pools-menu, .rentals-menu").hide();
	$(".pools-menu-btn, .rentals-menu-btn").removeClass("active");
	$(".workers-menu").show();
	$(".workers-menu-btn").addClass("active");
});

$(".rentals-menu-btn").on("click", function () {
	updateRentals();
	$(".workers-menu, .pools-menu").hide();
	$(".workers-menu-btn, .pools-menu-btn").removeClass("active")
	$(".rentals-menu").show();
	$(".rentals-menu-btn").addClass("active");
});



