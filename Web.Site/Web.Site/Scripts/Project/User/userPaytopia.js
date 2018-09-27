var table = $("#payments");
var tableaction = table.data("action");
var getPaymentAction = table.data("action-get");
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
	"language": {
		"emptyTable": Resources.UserPaytopia.EmptyListMessage,
		"paginate": {
			"next": Resources.General.Next + '!',
			"previous": Resources.General.Previous
		}
	},
	"columnDefs": [
		{
			"targets": [5, 6],
			"visible": false,
			"searchable": false,
			"orderable": false
		},
		{
			"targets": [7, 8],
			"render": function (data, type, full, meta) {
				return toLocalDate(data);
			}
		},
		{
			"targets": [9],
			"render": function (data, type, full, meta) {
				if (full[4] == "Pending") {
					return String.format("<i>{0}</i>", Resources.UserPaytopia.TransferIdPendingValue);
				}
				else if (data == 0) {
					return String.format("<i class='text-danger'>{0}</i>", Resources.UserPaytopia.TransferIdErrorValue);
				}
				return data;
			}
		},
		{
			"targets": [10],
			"render": function (data, type, full, meta) {
				if (full[4] == "Refunded" && data > 0) {
					return data
				}
				else if (full[4] == "Refunded" && data == 0) {
					return String.format("<i class='text-danger'>{0}</i>", Resources.UserPaytopia.RefundIdErrorValue);
				}
				return String.format("<i>{0}</i>", Resources.General.NotAwailable);
			}
		},
		{
			"targets": [12],
			"searchable": false,
			"orderable": false,
			"render": function (data, type, full, meta) {
				return '<button class="btn btn-default btn-xs pull-right" style="width:100px" onclick="getPayment(' + full[0] + ')">' + Resources.UserPaytopia.InfoButton + '</button>';
			}
		}
	],
	"fnDrawCallback": function () {
	}
});

function getPayment(id) {
	openModalGet(getPaymentAction, { id: id }, function (data) {
		showMessage(data);
	});
}

$("#paytopiaTarget").addClass("user-tabtarget");