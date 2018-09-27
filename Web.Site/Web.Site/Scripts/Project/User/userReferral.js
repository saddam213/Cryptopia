var table = $("#referrals");
var tableaction = table.data("action");
table.DataTable({
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
	"language": {
		"emptyTable": Resources.UserReferral.EmptyListMessage,
		"paginate": {
			"next": Resources.General.Next,
			"previous": Resources.General.Previous
		}
	},
	"columnDefs": [{
			"targets": 6,
			"searchable": true,
			"orderable": true,
			"render": function (data, type, full, meta) {
				return toLocalTime(data);
			}
		}]
});

$("#referralHistoryTarget").addClass("user-tabtarget");