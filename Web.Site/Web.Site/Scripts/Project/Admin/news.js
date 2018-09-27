var table = $("#news");
var tableaction = table.data("action");
var tableactioncreate = table.data("action-create");
var tableactionupdate = table.data("action-update");
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
	"language": { "emptyTable": "There are no news items." },
	"columnDefs": [{
		"targets": 5,
		"visible": true,
		"searchable": false,
		"orderable": false,
		"render": function (data, type, full, meta) {
			return '<button class="btn btn-default btn-sm pull-right" style="min-width:80px" onclick="updateNews(' + full[0] + ')">Update</button>';
		}
	}],
	"fnDrawCallback": function() {
	}
});

function createNews() {
	openModalGet(tableactioncreate, { }, function (modalData) {
		if (modalData && modalData.Success) {
			datatable.ajax.reload();
		}
		showMessage(modalData);
	});
}

function updateNews(id) {
	openModalGet(tableactionupdate, { id: id }, function (modalData) {
		if (modalData && modalData.Success) {
			datatable.ajax.reload();
		}
		showMessage(modalData);
	});
}


$("#adminNewsTarget").addClass("user-tabtarget");