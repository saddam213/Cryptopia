var tableReceived = $("#payoutHistory");
var tableReceivedaction = tableReceived.data("action");
tableReceived.DataTable({
	"order": [[0, "desc"]],
	"searchDelay": 800,
	"lengthChange": false,
	"processing": false,
	"bServerSide": true,
	"searching": false,
	"scrollCollapse": true,
	"scrollX": "100%",
	"paging": true,
	"info": true,
	"iDisplayLength": 15,
	"sAjaxSource": tableReceivedaction,
	"sServerMethod": "POST",
	"language": { "emptyTable": Resources.UserShareholder.PayoutsEmptyListMessage },
	"columnDefs": [{
		"targets": 1,
		"render": function (data, type, full, meta) {
			return '<div style="display:inline-block"><div class="sprite-small small/' + data + '-small.png"></div> ' + data + '</div>';
		}
	}]
});



var tablePaytopiaReceived = $("#paytopiaHistory");
var getPaymentAction = tablePaytopiaReceived.data("action-info");
var tablePaytopiaReceivedaction = tablePaytopiaReceived.data("action");
tablePaytopiaReceived.DataTable({
	"order": [[0, "desc"]],
	"searchDelay": 800,
	"lengthChange": false,
	"processing": false,
	"bServerSide": true,
	"searching": false,
	"scrollCollapse": true,
	"scrollX": "100%",
	"paging": true,
	"info": true,
	"iDisplayLength": 15,
	"sAjaxSource": tablePaytopiaReceivedaction,
	"sServerMethod": "POST",
	"language": { "emptyTable": Resources.UserShareholder.PayoutsEmptyListMessage },
	"columnDefs": [{ "targets": [1,2], "visible": false },
	{
		"targets": 3,
		"render": function (data, type, full, meta) {
			return '<div style="display:inline-block"><div class="sprite-small small/' + full[2] + '-small.png"></div> ' + data + '</div>';
		}
	},
	{
		"targets": 7,
		"searchable": false,
		"orderable": false,
		"render": function (data, type, full, meta) {
			return '<button class="btn btn-default btn-xs pull-right" style="min-width:80px" onclick="getPayment(' + full[0] + ')">Info</button>';
		}
	}]
});

function getPayment(id) {
	openModalGet(getPaymentAction, { id: id });
}

$("#shareholderTarget").addClass("user-tabtarget");