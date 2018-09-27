var poolsDatatable;
var poolsTable = $("#pools");
var poolsAction = poolsTable.data("action");
var updateAction = poolsTable.data("action-update");
poolsDatatable = poolsTable.DataTable({
	"order": [[0, "desc"]],
	"searchDelay": 400,
	"processing": false,
	"bServerSide": true,
	"searching": true,
	"scrollCollapse": true,
	"scrollX": "100%",
	"paging": true,
	"info": true,
	"lengthChange": false,
	"iDisplayLength": 15,
	"sAjaxSource": poolsAction,
	"sServerMethod": "POST",
	"language": { "emptyTable": "There are no pools." },
	"columnDefs": [{
		"targets": [5, 6],
		"render": function (data, type, full, meta) {
			return toLocalTime(data);
		}
	},
		{
			"targets": 10,
			"searchable": false,
			"orderable": false,
			"render": function (data, type, full, meta) {
				return '<button class="btn btn-xs btn-primary pull-right" style="width:110px" onclick="updatePool(' + full[0] + ')">Update</button>';
			}
		}]
});



var connectionsDatatable;
var connectionsTable = $("#connections");
var connectionsAction = connectionsTable.data("action");
var updateWorkersAction = connectionsTable.data("action-workers");
var updateConnectionAction = connectionsTable.data("action-update");
var connectionOptionsTemplate = $('#connectionOptionsTemplate').html();

connectionsDatatable = connectionsTable.DataTable({
	"order": [[0, "desc"]],
	"searchDelay": 400,
	"processing": false,
	"bServerSide": true,
	"searching": true,
	"scrollCollapse": true,
	"scrollX": "100%",
	"paging": true,
	"info": true,
	"lengthChange": false,
	"iDisplayLength": 15,
	"sAjaxSource": connectionsAction,
	"sServerMethod": "POST",
	"language": { "emptyTable": "There are no connections." },
	"columnDefs": [{
		"targets": 7,
		"searchable": false,
		"orderable": false,
		"render": function (data, type, full, meta) {
			return Mustache.render(connectionOptionsTemplate, {
				updateConnection: 'updateConnection(\'' + full[2] + '\')',
				updatePoolWorker: 'updateWorkerPool(\'' + full[2] + '\')'
			})
		}
	}]
});

var table = $("#payments");
var tableaction = table.data("action");
var datatable = table.DataTable({
	"order": [[0, "desc"]],
	"searchDelay": 800,
	"lengthChange": false,
	"processing": false,
	"bServerSide": true,
	"searching": true,
	"scrollCollapse": true,
	"scrollX": "100%",
	"paging": true,
	"info": true,
	"iDisplayLength": 15,
	"sAjaxSource": tableaction,
	"sServerMethod": "POST",
	"language": { "emptyTable": "There are no transfers." },
	"columnDefs": [],
	"fnDrawCallback": function () {
	}
});

var updateSettingsAction = $("#updateSettingsButton").data("action");
function updatePoolSettings() {
	openModalGet(updateSettingsAction, { }, function (data) {
		showMessage(data);
	});
}

function updatePool(id) {
	openModalGet(updateAction, { id: id }, function (data) {
		showMessage(data);
		if (data && data.Success) {
			poolsDatatable.ajax.reload();
		}
	});
}

function updateConnection(id) {
	openModalGet(updateConnectionAction, { algoType: id }, function (data) {
		showMessage(data);
		if (data && data.Success) {
			connectionsDatatable.ajax.reload();
		}
	});
}

function updateWorkerPool(id) {
	openModalGet(updateWorkersAction, { algoType: id }, function (data) {
		showMessage(data);
		if (data && data.Success) {
			connectionsDatatable.ajax.reload();
		}
	});
}

$("#adminMineshaftTarget").addClass("user-tabtarget");