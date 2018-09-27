var table = $("#lottoitems");
var tableaction = table.data("action");
var tablecreateaction = table.data("create");
var tableupdateaction = table.data("update");
var tabledeleteaction = table.data("delete");
var datatable = table.DataTable({
	"order": [[5, "desc"]],
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
	"language": { "emptyTable": "There are no lotto items." },
	"columnDefs": [
				{
					"targets": 8,
					"searchable": false,
					"orderable": false,
					"render": function (data, type, full, meta) {
						return '<div style="width:120px" class="btn-group btn-group-sm btn-group-justified pull-right" ><a class="btn btn-default btn-sm btn-update" onclick="updateLottoItem(' + full[0] + ')">Update</a><a class="btn btn-danger btn-sm btn-delete" onclick="deleteLottoItem(' + full[0] + ')">Delete</a></div>';
					}
				}]
});

function createLottoItem() {
	openModalGet(tablecreateaction, {}, function (data) {
		datatable.ajax.reload()
		showMessage(data);
	});
}

function updateLottoItem(id) {
	openModalGet(tableupdateaction, { lottoItemId: id }, function (data) {
		datatable.ajax.reload()
		showMessage(data);
	});
}

function deleteLottoItem(id) {
	confirm("Delete lottoItem?", "Are you sure you want to delete this lotto item?", function () {
		postJson(tabledeleteaction, { lottoItemId: id }, function (data) {
			datatable.ajax.reload()
			showMessage(data);
		});
	});
}

$("#adminLottoTarget").addClass("user-tabtarget");