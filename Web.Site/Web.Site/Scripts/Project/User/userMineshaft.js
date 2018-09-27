var datatable;
var table = $('#workers');
var action = table.data("action");
var actionmineshaft = table.data("action-mineshaft");
var actioncreate = table.data("action-create");
var actionupdate = table.data("action-update");
var actiondelete = table.data("action-delete");
var actionupdatepool = table.data("action-updatepool");
var workerOptionsTemplate = $('#workerOptionsTemplate').html();
datatable = table.DataTable({
	"order": [[8, "desc"], [3, "desc"], [1, "asc"]],
	"searchDelay": 800,
	"lengthChange": false,
	"processing": false,
	"bServerSide": false,
	"searching": false,
	"paging": false,
	"info": false,
	"scrollCollapse": true,
	"scrollX": "100%",
	"scrollY": "800px",
	"iDisplayLength": 10,
	"sAjaxSource": action,
	"sServerMethod": "POST",
	"language": { "emptyTable": "You have no workers configured." },
	"columnDefs": [
		{
		"targets": [0,6],
		"visible": false
	},
		{
			"targets": 3,
			"searchable": true,
			"orderable": true,
			"render": function (data, type, full, meta) {
				return hashrateLabel(data);
			}
		},
		{
			"targets": 5,
			"searchable": true,
			"orderable": true,
			"render": function (data, type, full, meta) {
				if (!data) {
					return '<small class="text-danger" style="font-size:10px"><i>No pool selected.</i></small>'
				}
				return '<a href="/Mineshaft?Algo=' + full[2] + '&Pool=' + data + '">' + data + '</a>'
			}
		},
		//{
		//	"targets": 8,
		//	"visible": false
		//}
		{
			"targets": 9,
			"searchable": false,
			"orderable": false,
			"render": function (data, type, full, meta) {
				return Mustache.render(workerOptionsTemplate, {
					updatePool: 'updatePool(' + full[0] + ')',
					updateWorker: 'updateWorker(' + full[0] + ')',
					deleteWorker: 'deleteWorker(' + full[0] + ')',
				})
			}
		}
	],
	"fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
		if (aData[8] == true) {
			$(nRow).addClass('success');
		}
	}
});




function createWorker() {
	openModalGet(actioncreate, {}, function (data) {
		if (data && data.Success) {
			datatable.ajax.reload();
			showMessage(data);
		}
	});
}

function updateWorker(id) {
	openModalGet(actionupdate, { id: id }, function (data) {
		if (data && data.Success) {
			datatable.ajax.reload();
			showMessage(data);
		}
	});
}

function updatePool(id) {
	openModalGet(actionupdatepool, { id: id }, function (data) {
		if (data && data.Success) {
			datatable.ajax.reload();
			showMessage(data);
		}
	});
}

function deleteWorker(workerId) {
	confirm("Delete Worker?", "Are you sure you want to delete this worker?", function () {
		$.blockUI({ message: 'Deleting worker...' });
		postJson(actiondelete, { workerId: workerId }, function (data) {
			datatable.ajax.reload();
			$.unblockUI();
			showMessage(data);
		});
	});
}

$("#minersTarget").addClass("user-tabtarget");