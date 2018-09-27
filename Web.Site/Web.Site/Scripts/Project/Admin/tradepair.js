var table = $("#table-tradepair");
var tableAction = table.data("action");
var tableActionUpdate = table.data("action-update");
var tableActionCreate = table.data("action-create");
var datatable = table.DataTable({
	"order": [[1, "asc"]],
	"searchDelay": 800,
	"lengthChange": false,
	"processing": false,
	"bServerSide": true,
	"searching": true,
	"paging": true,
	"pagingType": "simple",
	"scrollCollapse": true,
	"scrollX": "100%",
	"info": true,
	"iDisplayLength": 15,
	"sAjaxSource": tableAction,
	"sServerMethod": "POST",
	"language": { "emptyTable": "There are no trade pairs." },
	"columnDefs": [{
		"targets": 5,
		"visible": true,
		"render": function (data, type, full, meta) {
			return '<p style="max-width:300px;overflow:hidden;text-overflow:ellipsis;white-space:nowrap;cursor:default" title="' + data + '">' + data + '</p>';
		}
	},
	{
		"targets": 6,
		"visible": true,
		"searchable": false,
		"orderable": false,
		"render": function (data, type, full, meta) {
			return '<button class="btn btn-default btn-sm pull-right" style="min-width:80px" onclick="updateTradePair(' + full[0] + ')">Update</button>';
		}
	}]
});

function updateTradePair(id) {
	openModalGet(tableActionUpdate, { id: id }, function (modalData) {
		if (modalData && modalData.Success) {
			datatable.ajax.reload();
		}
		showMessage(modalData);
	});
}

function createTradePair() {
	openModalGet(tableActionCreate, {}, function (modalData) {
		if (modalData && modalData.Success) {
			datatable.ajax.reload();
		}
		showMessage(modalData);
	});
}

$("#adminTradePairTarget").addClass("user-tabtarget");