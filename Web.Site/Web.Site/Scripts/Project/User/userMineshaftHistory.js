var payoutTable = $('#miningPayouts');
var payoutAction = payoutTable.data("action");
var payoutDataTable = payoutTable.DataTable({
	"dom": datatableExportLayout,
	"buttons": datatableExportButtons("Mineshaft History"),
	"lengthChange": true,
	"lengthMenu": [[15, 50, 500, 5000, 50000, -1], [15, 50, 500, 5000, 50000, "All Data"]],
	"order": [[0, "desc"]],
	"searchDelay": 800,
	"processing": false,
	"bServerSide": true,
	"searching": true,
	"scrollCollapse": true,
	"scrollX": "100%",
	"paging": true,
	"info": true,
	"bSort": true,
	"iDisplayLength": 15,
	"sAjaxSource": payoutAction,
	"sServerMethod": "POST",
	"language": {
		"emptyTable": Resources.UserMineshaftHistory.EmptyListMessage,
		"paginate": {
			"next": Resources.General.Next,
			"previous": Resources.General.Previous
		}
	},
	"columnDefs": [{
		"targets": 1,
		"render": function (data, type, full, meta) {
			return '<div style="display:inline-block"><div class="sprite-small small/' + data + '-small.png"></div> ' + data + '</div>';
		}
	},{
		"targets": 6,
		"render": function (data, type, full, meta) {
			if (full[4] == 'Unconfirmed') {
				return String.format('<i>{0}</i>', Resources.UserMineshaftHistory.TransferPendingValue);
			}
			else if (full[4] == 'Orphan') {
				return Resources.General.NotAwailable;
			}
			return data;
		}
	},
	{
		"targets": 7,
		"searchable": true,
		"orderable": true,
		"render": function (data, type, full, meta) {
			return toLocalTime(data);
		}
	}]
});

//var transferTable = $('#miningTransfers');
//var transferAction = transferTable.data("action");
//var transferDataTable = transferTable.DataTable({
//	"order": [[0, "desc"]],
//	"searchDelay": 800,
//	"lengthChange": false,
//	"processing": false,
//	"bServerSide": true,
//	"searching": false,
//	"scrollCollapse": true,
//	"scrollX": "100%",
//	"paging": true,
//	"info": true,
//	"bSort": true,
//	"iDisplayLength": 15,
//	"sAjaxSource": transferAction,
//	"sServerMethod": "POST",
//	"language": { "emptyTable": "You have no mining transfers." },
//	"columnDefs": [{ "targets": [3, 4], "visible": false },
//	{
//		"targets": 1,
//		"render": function (data, type, full, meta) {
//			return '<div style="display:inline-block"><div class="sprite-small small/' + data + '-small.png"></div> ' + data + '</div>';
//		}
//	}]
//});

$("#mineshaftHistoryTarget").addClass("user-tabtarget");