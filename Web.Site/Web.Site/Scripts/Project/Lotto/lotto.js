
var tablehistory = $("#history");
var tablehistoryaction = tablehistory.data("action");
var historyDataTable = tablehistory.dataTable({
	"order": [[9, "desc"]],
	"lengthChange": false,
	"processing": false,
	"bServerSide": true,
	"searching": true,
	"paging": true,
	"info": true,
	"iDisplayLength": 20,
	"sAjaxSource": tablehistoryaction,
	"sServerMethod": "POST",
	"language": { "emptyTable": Resources.Lotto.LottoHistoryEmptyListMessage },
});

var tableusertickets = $("#usertickets");
var tableuserticketsaction = tableusertickets.data("action");
var userTicketDataTable = tableusertickets.DataTable({
	"order": [[0, "desc"]],
	"lengthChange": false,
	"processing": false,
	"bServerSide": true,
	"searching": true,
	"paging": true,
	"info": true,
	"iDisplayLength": 10,
	"sAjaxSource": tableuserticketsaction,
	"sServerMethod": "POST",
	"language": { "emptyTable": Resources.Lotto.LottoMyActiveEmptyListMessage },
});

var tableuserhistory = $("#userhistory");
var tableuserhistoryaction = tableuserhistory.data("action");
var userHistoryDataTable = tableuserhistory.dataTable({
	"order": [[0,"desc"]],
	"lengthChange": false,
	"processing": false,
	"bServerSide": true,
	"searching": true,
	"paging": true,
	"info": true,
	"iDisplayLength": 20,
	"sAjaxSource": tableuserhistoryaction,
	"sServerMethod": "POST",
	"language": { "emptyTable": Resources.Lotto.LottoMyPrizeEmptyListMessage },
});

var createTicketAction = $("#lotto-container").data("create");
function openPaymentDialog(itemid) {
	openModalGet(createTicketAction, { id: itemid }, function (data) {
		showMessage(data);
		if (data && data.Success) {
			userTicketDataTable.ajax.reload()
			var tickets = $("#tickets-" + itemid);
			var yourTickets = $("#tickets-yours-" + itemid);
			tickets.text(+tickets.text() + data.Tickets)
			yourTickets.text(+yourTickets.text() + data.Tickets)
		}
	});
}


