var table = $("#referralTable");
var tableaction = table.data("action");

var tableOptionsLayout = "<'row'<'col-sm-12'f>>" +
	"<'row'<'col-sm-12'tr>>" +
	"<'row'<'col-sm-5'i><'col-sm-7'p>>"

var datatable = table.DataTable({
	"dom": tableOptionsLayout,
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
		"emptyTable": "There are no referrals.",
		"search" : ""
	},
	"columnDefs": [
		//{ "targets": 0, "searchable": false, "orderable": true },
		//{ "targets": 3, "searchable": false, "orderable": true },
		//{ "targets": 4, "searchable": false, "orderable": true },
		//{ "targets": 5, "searchable": false, "orderable": true },
		//{ "targets": 7, "searchable": false, "orderable": true },
		//{ "targets": 8, "searchable": false, "orderable": true }
	],
	"fnDrawCallback": function() {
	}
});

$("#adminReferralTarget").addClass("user-tabtarget");