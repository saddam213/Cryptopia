var table = $("#withdrawals");
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
	"language": { "emptyTable": "There are no withdrawals." },
	"columnDefs": [{
		"targets": [5,6],
		"render": function (data, type, full, meta) {
			return '<div style="text-overflow:ellipsis;white-space:nowrap;overflow:hidden;display:block;max-width:200px">' + data + '</div>';
		}
	}],
	"fnDrawCallback": function() {
	}
});

$("#adminWithdrawTarget").addClass("user-tabtarget");