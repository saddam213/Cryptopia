var table = $("#table-currency");
var tableAction = table.data("action");
var tableActionUpdate = table.data("action-update");
var datatableCurrency = table.DataTable({
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
	"language": { "emptyTable": "There are no currencies." },
	"columnDefs": [{
		"targets": 4,
		"visible": true,
		"render": function (data, type, full, meta) {
			return '<p style="max-width:400px;overflow:hidden;text-overflow:ellipsis;white-space:nowrap;cursor:default" title="' + data + '">' + data + '</p>';
		}
	},
	{
			"targets": 6,
			"searchable": false,
			"orderable": false,
			"render": function (data, type, full, meta) {
				return '<button class="btn btn-default btn-sm pull-right" style="min-width:80px" onclick="updateCurrency(' + full[0] + ')">Update</button>';
			}
		}
	]
});

function updateCurrency(id) {
	openModalGet(tableActionUpdate, { id: id }, function (modalData) {
		if (modalData && modalData.Success) {
			datatableCurrency.ajax.reload();
		}
		showMessage(modalData);
	});
}

$("#adminCurrencyTarget").addClass("user-tabtarget");