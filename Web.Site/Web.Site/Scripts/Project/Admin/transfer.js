var table = $("#transfers");
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
	"fnDrawCallback": function() {
	}
});

$("#adminTransferTarget").addClass("user-tabtarget");