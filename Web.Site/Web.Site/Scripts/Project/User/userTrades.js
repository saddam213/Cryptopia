var datatable;
var table = $('#trades');
var action = table.data("action");
var actionexchange = table.data("action-exchange");
datatable = table.DataTable({
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
	"sAjaxSource": action,
	"sServerMethod": "POST",
	"language": {
		"emptyTable": Resources.UserTrades.EmptyListMessage,
		"paginate": {
			"next": Resources.General.Next,
			"previous": Resources.General.Previous
		}
	},
	"columnDefs": [
		{ "targets": 1, "visible": false },
		{
			"targets": 2,
			"searchable": true,
			"orderable": true,
			"render": function(data, type, full, meta) {
				return '<a href="' + actionexchange + '?market=' + data.replace("/", "_") + '">' + data + '</a>';
			}
		},
		{
			"targets": 9,
			"searchable": true,
			"orderable": true,
			"render": function (data, type, full, meta) {
				return toLocalTime(data);
			}
		},
		{
			"targets": 10,
			"searchable": false,
			"orderable": false,
			"render": function(data, type, full, meta) {
				return '<div><i class="trade-item-remove fa fa-times" onclick="cancelOrder(' + full[0] + ', ' + full[1] + ');"></i></div>';
			}
		}
	]
});

$("#tradesTarget").addClass("user-tabtarget");

function orderCanceled(response) {
	$.unblockUI();
	datatable.ajax.reload();
}

function allordersCanceled(response) {
	$.unblockUI();
	datatable.ajax.reload();
}