var table = $("#deposits");
var tableaction = table.data("action");
table.DataTable({
	"dom": datatableExportLayout,
	"buttons": datatableExportButtons("Deposit History"),
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
	"sAjaxSource": tableaction,
	"sServerMethod": "POST",
	"language": {
		"emptyTable": Resources.UserDepositHistory.EmptyListMessage,
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

$("#depositsTarget").addClass("user-tabtarget");