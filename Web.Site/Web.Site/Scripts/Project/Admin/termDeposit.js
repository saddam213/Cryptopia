var depositsDatatable;
var depositsTable = $("#termsDeposits");
var depositsAction = depositsTable.data("action");
depositsDatatable = depositsTable.DataTable({
	"order": [[0, "desc"]],
	"searchDelay": 400,
	"processing": false,
	"bServerSide": true,
	"searching": true,
	"scrollCollapse": true,
	"scrollX": "100%",
	"paging": true,
	"info": true,
	"lengthChange": false,
	"iDisplayLength": 15,
	"sAjaxSource": depositsAction,
	"sServerMethod": "POST",
	"language": { "emptyTable": "There are no term deposits." }
});

var paymentDatatable;
var paymentTable = $("#termDepositPayments");
var paymentAction = paymentTable.data("action");
paymentDatatable = paymentTable.DataTable({
	"order": [[0, "desc"]],
	"searchDelay": 400,
	"processing": false,
	"bServerSide": true,
	"searching": true,
	"scrollCollapse": true,
	"scrollX": "100%",
	"paging": true,
	"info": true,
	"lengthChange": false,
	"iDisplayLength": 15,
	"sAjaxSource": paymentAction,
	"sServerMethod": "POST",
	"language": { "emptyTable": "There are no term deposit payments." },
	"columnDefs": [
		{
			"targets": 9,
			"searchable": true,
			"orderable": true,
			"render": function (data, type, full, meta) {
					return '<button class="btn btn-xs btn-primary pull-right" onclick="updatePayment(' + full[0] + ')">Update</button>';
			}
		}]
});


var updateAction = $("#termdeposit-container").data("action-update");
function updatePayment(id) {
	openModalGet(updateAction, { id: id }, function (data) {
		showMessage(data);
		if (data.Success) {
			depositsDatatable.ajax.reload();
			paymentDatatable.ajax.reload();
		}
	});
}


$("#adminTermDepositTarget").addClass("user-tabtarget");