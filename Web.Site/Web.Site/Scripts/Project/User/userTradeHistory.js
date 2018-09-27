var datatable;
var table = $("#tradehistory");
var action = table.data("action");
datatable = table.DataTable({
	"dom": datatableExportLayout,
	"buttons": datatableExportButtons("Trade History"),
	"lengthChange": true,
	"lengthMenu": [[15, 50, 500, 5000, 25000], [15, 50, 500, 5000, 25000]],
	"order": [[0, "desc"]],
	"searchDelay": 800,
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
		"emptyTable": Resources.TradeHistory.EmptyListMessage,
		"paginate": {
			"next": Resources.General.Next,
			"previous": Resources.General.Previous
		}
	},
	"columnDefs": [
		{
			"targets": 1,
			"searchable": true,
			"orderable": true,
			"render": function(data) {
				return '<a href="/Exchange?market=' + data.replace("/", "_") + '">' + data + "</a>";
			}
		},
		{
			"targets": 7,
			"searchable": true,
			"orderable": true,
			"render": function (data, type, full, meta) {
				return toLocalTime(data);
			}
		}
	],
	"fnDrawCallback": function() {

	}
});

$("#tradeHistoryTarget").addClass("user-tabtarget");