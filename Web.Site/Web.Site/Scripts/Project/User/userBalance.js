var dataTable;

$("#search-box").keyup(function () {
	dataTable.search($(this).val()).draw();
});

$(".currency-dustbin").on("click", function () {
	var button = $(this);
	var action = button.data("action");
	var currency = button.data("currency");
	var symbol = button.data("symbol");
	if (currency) {
		confirm(Resources.UserBalances.BalancesDustQuestionTitle, String.format(Resources.UserBalances.BalancesDustQuestion, symbol), function () {
			postJson(action, { currencyId: currency }, function (data) {
				showMessage(data);
				if (data && data.Success) {
					$(".balancedata-" + currency).html("0.00000000");
				}
			});
		});
	}
});

$("#chk-hidezero").click(function () {
	var chkbox = $(this);
	var action = chkbox.data("action");
	var hidezero = chkbox.is(":checked");
	postJson(action, { hide: hidezero });
	dataTable.draw();
});

$("#chk-favorites").click(function () {
	var chkbox = $(this);
	var action = chkbox.data("action");
	var showFavorite = chkbox.is(":checked");
	postJson(action, { show: showFavorite });
	dataTable.draw();
});

$(".chk-setfavorite").click(function () {
	var chkbox = $(this);
	var action = chkbox.data("action");
	var favorite = chkbox.is(":checked");
	var currency = chkbox.data("currency");
	postJson(action, { currencyId: currency });
	dataTable.cell($(this).closest("tr"), 7).data(favorite);
	if (!favorite) {
		dataTable.draw();
	}
});

$.fn.dataTable.ext.search.push(
	function (settings, data) {
		var returnVal = true;
		var hidezero = $("#chk-hidezero").is(":checked");
		var showFav = $("#chk-favorites").is(":checked");
		if (showFav) {
			returnVal = data[7] === "true";
		}
		if (hidezero && returnVal) {
			returnVal = (data[3] > 0.00000000);
		}
		return returnVal;
	}
);

dataTable = $("#table-balances").DataTable({
	sDom: "lrtip",
	"order": [],
	"lengthChange": false,
	"processing": false,
	"bServerSide": false,
	"searching": true,
	"scrollCollapse": false,
	"sort": true,
	"pageLength": 15,
	"paging": true,
	"info": false,
	"language": {
		"emptyTable": Resources.UserBalances.BalancesEmptyListMessage,
		"paginate": {
			"next": Resources.General.Next,
			"previous": Resources.General.Previous
		}
	},
	"columnDefs": [
		{ "targets": 0, "searchable": false, "orderable": false },
		{ "targets": 1, "searchable": true, "orderable": true },
		{ "targets": 2, "searchable": false, "orderable": true },
		{ "targets": 3, "searchable": true, "orderable": true },
		{ "targets": 4, "searchable": false, "orderable": true },
		{ "targets": 5, "searchable": false, "orderable": true },
		{ "targets": 6, "searchable": false, "orderable": false },
		{ "targets": 7, "visible": false, "searchable": true, "orderable": false }
	],
	"fnDrawCallback": function () {
		$('.preview-popover').popover({
			trigger: 'hover'
		});
	}
});

$("#balanceTarget").addClass("user-tabtarget");