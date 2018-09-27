var datatable;
var table = $('#poolWorkers');
var action = table.data("action");
var actionupdate = table.data("action-update");
datatable = table.DataTable({
	"order": [[8, "desc"], [1, "asc"]],
	"searchDelay": 800,
	"lengthChange": false,
	"processing": false,
	"bServerSide": true,
	"searching": true,
	"paging": true,
	"info": false,
	"scrollCollapse": true,
	"scrollX": "100%",
	"iDisplayLength": 15,
	"sAjaxSource": action,
	"sServerMethod": "POST",
	"language": { "emptyTable": "There are no workers configured." },
	"columnDefs": [{
			"targets": 0,
			"visible": false
		},
		{
			"targets": 5,
			"searchable": true,
			"orderable": true,
			"render": function (data, type, full, meta) {
				if (!data) {
					return '<small class="text-danger" style="font-size:10px"><i>No pool selected.</i></small>'
				}
				return data
			}
		},
		{
			"targets": 6,
			"searchable": true,
			"orderable": true,
			"render": function (data, type, full, meta) {
				if (data == -1) {
					return 'Var-Diff Low'
				}
				else if (data == -2) {
					return 'Var-Diff Medium'
				}
				else if (data == -3) {
					return 'Var-Diff High'
				}
				return data
			}
		},
		{
			"targets": 9,
			"searchable": false,
			"orderable": false,
			"render": function (data, type, full, meta) {
				return	'<button class="btn btn-xs btn-primary pull-right" style="width:110px" onclick="updateWorker('+full[0]+')">Update</button>'
			}
		}
	],
	"fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
		if (aData[8] == true) {
			$(nRow).addClass('success');
		}
	}
});

function updateWorker(id) {
	openModalGet(actionupdate, { id: id }, function (data) {
		if (data && data.Success) {
			datatable.ajax.reload();
			showMessage(data);
		}
	});
}

$("#adminPoolWorkerTarget").addClass("user-tabtarget");