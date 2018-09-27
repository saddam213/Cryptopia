var table = $("#payments");
var tableaction = table.data("action");
var getPaymentAction = table.data("action-get");
var updatePaymentAction = table.data("action-update");
var paytopiaButtonTemplate = $('#paytopiaButtonTemplate').html();
var datatable = table.DataTable({
	"order": [[4, "asc"], [0, "desc"]],
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
	"language": { "emptyTable": "There are no payments." },
	"columnDefs": [
		{
			"targets": [7, 8],
			"render": function (data, type, full, meta) {
				return toLocalDate(data);
			}
		},
		{
			"targets": 12,
			"searchable": false,
			"orderable": false,
			"render": function (data, type, full, meta) {
				return Mustache.render(paytopiaButtonTemplate, {
					disabled: full[4] == 'Complete' ? "disabled='disabled'" : "",
					getPayment: 'getPayment(\'' + full[0] + '\')',
					updatePayment: 'updatePayment(\'' + full[0] + '\')'
				})
			}
		}
	],
	"fnDrawCallback": function () {
	},
	"fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
		if (aData[4] == 'Pending') {
			$(nRow).addClass('info');
		}
	}
});

function getPayment(id) {
	openModalGet(getPaymentAction, { id: id }, function (data) {
		showMessage(data);
	});
}

function updatePayment(id) {
	openModalGet(updatePaymentAction, { id: id }, function (data) {
		showMessage(data);
		if (data && data.Success) {
			datatable.ajax.reload();
		}
	});
}

$("#adminPaytopiaTarget").addClass("user-tabtarget");