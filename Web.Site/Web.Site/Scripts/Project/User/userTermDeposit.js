var paymentDatatable;
var paymentTable = $("#paymentTermsDeposit");
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
	"language": {
		"emptyTable": Resources.TermDeposit.PaymentEmptyList,
		"paginate": {
			"next": Resources.General.Next,
			"previous": Resources.General.Previous
		}
	}
});

var closedDatatable;
var closedTable = $("#closedTermsDeposit");
var closedAction = closedTable.data("action");
closedDatatable = closedTable.DataTable({
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
	"sAjaxSource": closedAction,
	"sServerMethod": "POST",
	"language": {
		"emptyTable": Resources.TermDeposit.ClosedEmptyListMessage,
		"paginate": {
			"next": Resources.General.Next,
			"previous": Resources.General.Previous
		}
	}
});

$("#termDepositStatusTarget").addClass("user-tabtarget");
