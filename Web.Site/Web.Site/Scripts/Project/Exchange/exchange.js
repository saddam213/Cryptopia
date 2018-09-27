var tradechart;
var orderbookChart;
var orderBookChartThrottle = 250;
var distributionChart;
var selectedChart = 'trade';
var selectedSeriesRange = 1;
var selectedCandleGrouping = 60;

var chartTextColor = "#666666"
var chartBorderColor = "#000000"
var chartCrossHairColor = "#000000"
var candlestickLineColor = "#000000"
var candlestickChartUpColor = "#5cb85c";
var candlestickChartDownColor = "#ee5f5b";
var stockPriceChartColor = "#4286f4";
var volumeChartColor = "rgba(0, 0, 0, 0.2)";
var macdChartColor = "#3b7249";
var signalChartColor = "#d8ae13";
var histogramChartUpColor = "#5cb85c";
var histogramChartDownColor = "#ee5f5b";
var smaChartColor = "#4a788c";
var ema1ChartColor = "orange";
var ema2ChartColor = "purple";
var fibonacciChartColor = "#91353e";

var orderTemplate = $('#orderTemplate').html();
var tradeHistoryTemplate = $('#tradeHistoryTemplate').html();
var orderbookTooltipTemplate = $('#orderbookTooltipTemplate').html();

var marketDataSet = [];
var marketSummaryTables = {};

var isSideMenuOpen = true;
var chatModule = new ChatModule('.chat-menu');
var favoriteMarkets = store.get("favorite-market") || [];
var showFavoriteMarkets = store.get("favorite-market-enabled") || false;
var marketTableSortColumn = store.get("market-sort-col") || 5;
var marketTableSortDirection = store.get("market-sort-dir") || "desc";
var balanceTableSortColumn = store.get("balance-sort-col") || 1;
var balanceTableSortDirection = store.get("balance-sort-dir") || "asc";
var disableTradeConfirmationModal = store.get("disable-trade-confirmation") || false;

$("#market-favorite-chk").attr('checked', showFavoriteMarkets);

var sideMenuBalanceTable;
var sideMenuOpenOrdersTable;
var buyOrdersTable;
var sellOrdersTable;
var marketHistoryTable;
var userOpenOrdersTable;
var userOrderHistoryTable;
$('#market-list > tbody').empty();
var marketTable = $('#market-list').DataTable({
	"dom": "<'row'<'col-sm-12'tr>>",
	"order": [[marketTableSortColumn, marketTableSortDirection]],
	"lengthChange": false,
	"processing": false,
	"bServerSide": false,
	"searching": true,
	"paging": false,
	"scrollX": "100%",
	"autoWidth": false,
	"sServerMethod": "POST",
	"info": false,
	"language": {
		"emptyTable": Resources.Exchange.MarketsLoadingMessage,
		"sZeroRecords": Resources.Exchange.MarketsEmptyListMessage,
		"search": "",
		"searchPlaceholder": Resources.Exchange.MarketsSearchPlaceholder,
		"paginate": {
			"previous": Resources.General.Previous,
			"next": Resources.General.Next
		}
	},
	"columnDefs": [
		{ "targets": [1, 2, 7, 8, 9, 10], "visible": false },
		{
			"targets": [0],
			"visible": true,
			"sortable": false,
			"render": function (data, type, full, meta) {
				return '<div class="market-favorite market-favorite-' + full[1] + '" data-marketid="' + full[1] + '"><i class="fa fa-ellipsis-v" aria-hidden="true" style="margin-left:5px"></i></div>';
			}
		},
		{
			"targets": [3],
			"visible": true,
			"render": function (data, type, full, meta) {
                return '<div style="display:inline-block"><div class="sprite-small ' + data + '-small-png"></div> ' + data + '</div>';
			}
		},
		{
			"targets": [4],
			"visible": true,
			"render": function (data, type, full, meta) {
				return '<div class="text-right">' + (+full[9] || 0).toFixed(8) + '</div>';
			}
		},
		{
			"targets": [5],
			"visible": true,
			"render": function (data, type, full, meta) {
				return '<div class="text-right">' + (+data || 0).toFixed(2) + '</div>';
			}
		},
		{
			"targets": [6],
			"visible": true,
			"render": function (data, type, full, meta) {
				return '<div class="text-right ' + (full[4] > 0 ? "text-success" : full[4] < 0 ? "text-danger" : "") + '">' + (+full[4] || 0).toFixed(2) + '%</div>';
			}
		}
	],
	"fnRowCallback": function (nRow, aData) {
		var active = aData[1] == selectedTradePair.TradePairId ? "info text-bold " : "";
		$(nRow)
			.data("name", aData[2])
			.data("tradepairid", aData[1])
			.data("market", aData[3] + '_' + currentBaseMarket)
			.addClass(active + "currencyData-tradepair currencyData-tradepair-" + aData[1]);
	}
});

changeBaseMarket(currentBaseMarket);
if (selectedTradePair.TradePairId) {
	getTradePairInfo(selectedTradePair.TradePairId);
}

var getTradePairDataRequest;
var getUserTradePairDataRequest;
var getTradePairBalanceRequest;
var getTradePairChartRequest;
var getCurrencySummaryRequest;

function getTradePairInfo(tradepairId) {
	tradePairLoadStart();
	getTradePairDataRequest = postJson(actionTradePairData, { tradePairId: tradepairId }, function (result) {
		selectedTradePair = result.TradePair;
		selectedMarket = selectedTradePair.Symbol + "/" + selectedTradePair.BaseSymbol;
		$(".tradepair-basefee").text(result.TradePair.BaseFee.toFixed(2))
		$(".tradepair-basemintrade").text(result.TradePair.BaseMinTrade.toFixed(8));

		updateSelectedChart();
		updateTitle(result.TradePair, false);
		updateTicker(result.Ticker)
		updateStatusMessage(result.TradePair);
		updateBuyOrdersTable(result.Buys);
		updateSellOrdersTable(result.Sells);
		updateMarketHistoryTable(result.History);
		tradePairLoadComplete();
	});

	if (isAuthenticated) {
		createUserOpenOrdersTable();
		createUserOrderHistoryTable();
		updateBalance(tradepairId, false);
		getUserTradePairDataRequest = postJson(actionTradePairUserData, { tradePairId: tradepairId }, function (data) {
			updateUserOpenOrdersTable(data.Open);
			updateUserOrderHistoryTable(data.History)
		});
	}
}

function clearTarget() {
	clearStatusMessage();
	clearTicker();
	clearCharts();
	clearBalance();
	clearBuySellInputs();
	clearBuyOrdersTable();
	clearSellOrdersTable();
	clearMarketHistoryTable();
	clearUserOpenOrdersTable();
	clearUserOrderHistoryTable();
}

function tradePairLoadStart() {
	$(".currencyData-tradepair").attr("disabled", "disabled");
	if (getTradePairDataRequest && getTradePairDataRequest.readyState != 4) {
		getTradePairDataRequest.abort();
	}
	if (getUserTradePairDataRequest && getUserTradePairDataRequest.readyState != 4) {
		getUserTradePairDataRequest.abort();
	}
	if (getTradePairBalanceRequest && getTradePairBalanceRequest.readyState != 4) {
		getTradePairBalanceRequest.abort();
	}
	if (getTradePairChartRequest && getTradePairChartRequest.readyState != 4) {
		getTradePairChartRequest.abort();
	}
	clearTarget();
	$(".dataTables_empty").html('<span><i class="fa fa-spinner fa-pulse"></i> ' + Resources.General.LoadingMessage + '</span>');
}

function tradePairLoadComplete() {
	$(".currencyData-tradepair").removeAttr("disabled");
}

function updateTicker(data) {
	$(".ticker-change").text(data.Change.toFixed(2)).addClass(changeHighlight(data.Change))
	$(".ticker-last").text(data.Last.toFixed(8))
	$(".ticker-high").text(data.High.toFixed(8))
	$(".ticker-low").text(data.Low.toFixed(8))
	$(".ticker-volume").text(data.Volume.toFixed(8))
	$(".ticker-basevolume").text(data.BaseVolume.toFixed(8))
	document.title = data.Last.toFixed(8) + ' ' + selectedTradePair.Symbol + "/" + selectedTradePair.BaseSymbol + ' ' + Resources.Exchange.MarketPageTitle + ' - Cryptopia';
}

function clearTicker() {
	$(".ticker-change").removeClass("text-danger text-success").text("0.00")
	$(".ticker-last, .ticker-high, .ticker-low, .ticker-volume, .ticker-basevolume").text("0.00000000")
}

function updateTitle(data, animate) {
	if (animate) {
		$(".exchangeinfo-container ").fadeTo(200, 0.5, function () {
			$(".tradepair-symbol").text(data.Symbol)
			$(".tradepair-basesymbol").text(data.BaseSymbol)
			$(".exchangeinfo-title").text(data.Name);
			$(".exchangeinfo-title-logo").attr("src", "/Content/Images/Coins/" + data.Symbol + "-medium.png");
		}).fadeTo(200, 1);
	}
	else {
		$(".tradepair-symbol").text(data.Symbol)
		$(".tradepair-basesymbol").text(data.BaseSymbol)
		$(".exchangeinfo-title").text(data.Name);
		$(".exchangeinfo-title-logo").attr("src", "/Content/Images/Coins/" + data.Symbol + "-medium.png");
	}
}


function updateStatusMessage(tradepair) {
	var statusContainer = $("#tradepairStatus");
	var market = tradepair.Symbol + "/" + tradepair.BaseSymbol;
	if (tradepair.Status == 0) {
		if (tradepair.StatusMessage) {
			statusContainer.show();
			statusContainer.find(".alert").addClass("alert-info");
			statusContainer.find("h4").text('Market Information');
			statusContainer.find("p").text(tradepair.StatusMessage);
		}
	}
	else if (tradepair.Status == 1) {
		statusContainer.show();
		statusContainer.find(".alert").addClass("alert-danger");
		statusContainer.find("h4").text('Market Closing');
		statusContainer.find("p").text(tradepair.StatusMessage || (market + " market is closing, please cancel any open orders and withdraw your coins."));
		$(".submit-button").hide();
		$(".submit-button-alert").html("Market Closing").show();
	}
	else if (tradepair.Status == 2) {
		statusContainer.show();
		statusContainer.find(".alert").addClass("alert-warning");
		statusContainer.find("h4").text('Market Paused');
		statusContainer.find("p").text(tradepair.StatusMessage || (market + " trading is currently paused."));
		$(".submit-button").hide();
		$(".submit-button-alert").html("Market Paused").show();
	}
}

function clearStatusMessage() {
	$(".submit-button").show();
	$(".submit-button-alert").hide()
	$("#tradepairStatus").hide().find(".alert").removeClass("alert-info alert-warning alert-danger")
}
//------------------------------------------------------------------------------------------------------------------


//------------------------------------------------------------------------------------------------------------------
// Market List
//------------------------------------------------------------------------------------------------------------------
// Base market Click
$("#wrapper").on("click", ".currencyData-btn", function () {
	var baseMaketBtn = $(this);
	currentBaseMarket = baseMaketBtn.data("currency");
	changeBaseMarket(currentBaseMarket);
	if (marketSummaryView) {
		History.pushState({}, Resources.Exchange.MarketPageTitle + " - Cryptopia", "?baseMarket=" + currentBaseMarket);
	}
})

// Market Click
$("#market-list_wrapper").on("click", ".currencyData-tradepair", function () {

	var marketRow = $(this);
	var market = marketRow.data("market");
	updateTitle({
		Symbol: market.split('_')[0],
		BaseSymbol: currentBaseMarket,
		Name: marketRow.data("name")
	}, true);

	if (marketSummaryView) {
		$('#market-main').show();
		$('#market-summary').hide();
		marketSummaryView = false;
	}

	var tradePair = marketRow.data("tradepairid");
	$(".currencyData-tradepair").removeClass("info text-bold")
	marketRow.addClass("info text-bold")
	getTradePairInfo(marketRow.data("tradepairid"));
	var market = marketRow.data('market');
	History.pushState({}, Resources.Exchange.MarketPageTitle + " - Cryptopia", "?market=" + market);
});

// Market Search
$('#markets-search').keyup(function () {
	marketTable.search($(this).val()).draw();
})

$("#market-list_wrapper .dataTables_scrollHead th").on("click", function () {
	var index = $(this)[0].cellIndex + 2;
	var direction = $(this).hasClass("sorting_asc") ? "asc" : "desc";
	store.set("market-sort-col", index);
	store.set("market-sort-dir", direction);
})

// Enable/Disable favorite markets
$("#market-favorite-chk").click(function () {
	var chkbox = $(this);
	showFavoriteMarkets = chkbox.is(":checked");
	store.set("favorite-market-enabled", showFavoriteMarkets);
	marketTable.draw();
});

// Favorite market clicked
$("#market-list").on("click", ".market-favorite", function (e) {
	e.stopPropagation();
	var cell = $(this);
	var marketId = cell.data('marketid');
	if (cell.hasClass('market-favorite-active')) {
		cell.removeClass('market-favorite-active')
		for (var i = favoriteMarkets.length - 1; i >= 0; i--) {
			if (favoriteMarkets[i] === marketId) {
				favoriteMarkets.splice(i, 1);
				store.set("favorite-market", favoriteMarkets);
			}
		}
	}
	else {
		cell.addClass('market-favorite-active')
		favoriteMarkets.push(marketId);
		store.set("favorite-market", favoriteMarkets);
	}
	if (showFavoriteMarkets) {
		marketTable.draw();
	}
})

// Favorite market filter
function marketFavoriteFilter(settings, data) {
	if (settings.sInstance == "market-list") {
		if (showFavoriteMarkets) {
			for (var i = 0; i < favoriteMarkets.length; i++) {
				if (favoriteMarkets[i] == data[1])
					return true;
			}
			return favoriteMarkets.length == 0;
		}
	}
	return true;
}

// Setup market list
function setupMarketList(refresh) {
	if (refresh) {
		var parentHeight = $(".stackmenu-content").height();
		var headerHeight = $("#market-list_wrapper  .dataTables_scrollHead").height();
		var footerHeight = $("#market-list_wrapper  .dataTables_filter").height();
		var listHeight = parentHeight - (headerHeight + footerHeight);
		$('#market-list_wrapper .dataTables_scrollBody').height(listHeight);

		// Set Favorites
		updateMarketFavorites();

		// Scroll to selected item
		var selection = $("#market-list_wrapper .currencyData-tradepair-" + selectedTradePair.TradePairId);
		if (selection && selection.position()) {
			var padding = listHeight / 6;
			var location = selection.position().top + padding;
			if (location > listHeight) {
				$("#market-list_wrapper .dataTables_scrollBody").scrollTop(selection.position().top - padding)
			}
		}
	}
}

var updateMarketFavoritesTimeout;
function updateMarketFavorites() {
	clearTimeout(updateMarketFavoritesTimeout);
	updateMarketFavoritesTimeout = setTimeout(function () {
		$("#market-list_wrapper .market-favorite").removeClass('market-favorite-active');
		if (favoriteMarkets) {
			for (var i = 0; i < favoriteMarkets.length; i++) {
				$("#market-list_wrapper .market-favorite-" + favoriteMarkets[i]).addClass('market-favorite-active');
			}
		}
	}, 100);
}

// Change BaseMarket
function changeBaseMarket(baseMarket) {
	$(".currencyData-content").hide();
	$("#currencyData-content-" + baseMarket).show();
	$(".currencyData-btn").removeClass("active");
	$(".currencyData-btn-" + baseMarket).addClass("active");

	if (marketSummaryView) {
		if (!marketSummaryTables[baseMarket]) {
			marketSummaryTables[baseMarket] = createSummaryTable(baseMarket);
		}

		if (marketSummaryTables[baseMarket]) {
			marketSummaryTables[baseMarket].clear().draw();

		}
	}

	marketTable.clear().draw();
	$(".dataTables_empty").html('<span><i class="fa fa-spinner fa-pulse"></i> ' + Resources.General.LoadingMessage + '</span>');
	if (getCurrencySummaryRequest && getCurrencySummaryRequest.readyState != 4) {
		getCurrencySummaryRequest.abort();
	}
	getCurrencySummaryRequest = postJson(actionCurrencySummary, { baseMarket: baseMarket }, function (data) {
		marketTable.rows.add(data.aaData).draw();

		if (marketSummaryView) {
			if (marketSummaryTables[baseMarket]) {
				marketSummaryTables[baseMarket].rows.add(data.aaData).draw();
			}
		}
		marketDataSet = data.aaData;
		setupMarketList(true);
		marketTable.columns.adjust();
	});
}

// Update market items
function updateMarketItem(notification) {

	// Update market data in backing array;
	var highlight;
	for (var i = 0; i < marketDataSet.length; i++) {
		var marketData = marketDataSet[i];
		if (marketData[1] == notification.TradePairId) {
			highlight = marketData[9] > notification.Last.toFixed(8) ? "red" : marketData[9] < notification.Last.toFixed(8) ? "green" : "blue";
			marketData[4] = notification.Change.toFixed(2);
			marketData[5] = notification.BaseVolume.toFixed(8);
			marketData[6] = notification.Volume.toFixed(8);
			marketData[7] = notification.High.toFixed(8);
			marketData[8] = notification.Low.toFixed(8);
			marketData[9] = notification.Last.toFixed(8);
			break;
		}
	}

	if (highlight) {
		var marketListRow = $("#market-list .currencyData-tradepair-" + notification.TradePairId);
		marketTable.row(marketListRow).invalidate();
		highlightRemove("#market-list .currencyData-tradepair");
		highlightItem(marketListRow, highlight);
		updateMarketFavorites();

		if (marketSummaryView) {
			var summaryListRow = $("#currencyData-" + currentBaseMarket + " .currencyData-tradepair-" + notification.TradePairId);
			marketSummaryTables[currentBaseMarket].row(summaryListRow).invalidate();
			highlightRemove("#currencyData-" + currentBaseMarket + " .currencyData-tradepair");
			highlightItem(summaryListRow, highlight);

			// Top markets
			var topmarketRowName = "#top-stats .currencyData-tradepair-" + notification.TradePairId;
			var changeclass = notification.Change > 0 ? "text-success" : notification.Change < 0 ? "text-danger" : "";
			$(topmarketRowName + " > td:nth-child(2)").removeClass("text-*").addClass(changeclass).text(notification.Change.toFixed(2) + "%")
			$(topmarketRowName + " > td:nth-child(3)").text(notification.BaseVolume.toFixed(8))
			$(topmarketRowName + " > td:nth-child(4)").text(notification.Volume.toFixed(8))
			$(topmarketRowName + " > td:nth-child(5)").text(notification.High.toFixed(8))
			$(topmarketRowName + " > td:nth-child(6)").text(notification.Low.toFixed(8))
		}
	}
}

// Create BaseMarket summary table
function createSummaryTable(baseMarket) {
	var table = $('#currencyData-' + baseMarket);
	table.find('tbody').empty();
	return table.DataTable({
		"dom": "<'row'<'col-sm-12'tr>>",
		"order": [[5, "desc"]],
		"lengthChange": false,
		"processing": false,
		"bServerSide": false,
		"searching": false,
		"paging": false,
		"scrollCollapse": false,
		"scrollY": "100%",
		"autoWidth": false,
		"info": false,
		"language": {
			"emptyTable": Resources.Exchange.MarketsLoadingMessage,
			"sZeroRecords": Resources.Exchange.MarketsEmptyListMessage,
			"search": "",
			"searchPlaceholder": Resources.Exchange.MarketsSearchPlaceholder,
			"paginate": {
				"previous": Resources.General.Previous,
				"next": Resources.General.Next
			}
		},
		"columnDefs": [
			{ "targets": [0], "visible": false },
			{ "targets": [1], "visible": false },
			{ "targets": [10], "visible": false },
			{
				"targets": [2],
				"render": function (data, type, full, meta) {
                    return '<div style="display:inline-block"><div class="sprite-small ' + full[3] + '-small-png"></div> ' + data + ' (' + full[3] + ')</div>';
				}
			},
			{
				"targets": [3],
				"render": function (data, type, full, meta) {
					return '<a href="/Exchange?market=' + data + '_' + baseMarket + '">' + data + '/' + baseMarket + '</a>';
				}
			},
			{
				"targets": [4],
				"render": function (data, type, full, meta) {
					return '<div class="text-right ' + (data > 0 ? "text-success" : data < 0 ? "text-danger" : "") + '">' + data + '%</div>';
				}
			},
			{
				"targets": [5, 6, 7, 8, 9],
				"render": function (data, type, full, meta) {
					return '<div class="text-right">' + (+data || 0).toFixed(8) + '</div>';
				}
			}],
		"fnRowCallback": function (nRow, aData) {
			$(nRow).addClass("currencyData-tradepair-" + aData[1]);
		}
	});
}

//------------------------------------------------------------------------------------------------------------------


//------------------------------------------------------------------------------------------------------------------
// Side Menu
//------------------------------------------------------------------------------------------------------------------
// SideMenu Main
$(".menu-btn").on("click", function () {
	toggleSideMenu();
})

// SideMenu Markets
$(".exchange-menu-btn").on("click", function () {
	$(".balance-menu, .orders-menu, .chat-menu").hide();
	$(".balance-menu-btn, .orders-menu-btn, .chat-menu-btn").removeClass("active");
	$(".exchange-menu").show();
	$(".exchange-menu-btn").addClass("active");
	if (!isSideMenuOpen) {
		toggleSideMenu();
	}
});

// SideMenu Balances
$(".balance-menu-btn").on("click", function () {
	$(".exchange-menu, .orders-menu, .chat-menu").hide();
	$(".exchange-menu-btn, .orders-menu-btn, .chat-menu-btn").removeClass("active");
	$(".balance-menu").show();
	$(".balance-menu-btn").addClass("active");
	if (!isSideMenuOpen) {
		toggleSideMenu();
	}
	setupBalances();
});

// SideMenu Orders
$(".orders-menu-btn").on("click", function () {
	$(".balance-menu, .exchange-menu, .chat-menu").hide();
	$(".balance-menu-btn, .exchange-menu-btn, .chat-menu-btn").removeClass("active")
	$(".orders-menu").show();
	$(".orders-menu-btn").addClass("active");
	if (!isSideMenuOpen) {
		toggleSideMenu();
	}
	setupOpenOrders();
});

// SideMenu Chat
$(".chat-menu-btn").on("click", function () {
	$(".balance-menu, .exchange-menu, .orders-menu").hide();
	$(".balance-menu-btn, .exchange-menu-btn, .orders-menu-btn").removeClass("active")
	$(".chat-menu").show();
	$(".chat-menu-btn").addClass("active");
	if (!isSideMenuOpen) {
		toggleSideMenu();
	}
	setupChatList();
	enableChat();
});

// SideMenu toggle
function toggleSideMenu() {
	if (isSideMenuOpen) {
		$("#main-wrapper").css({ "min-width": "325px" });
		$("#sidebar-wrapper").animate({ width: '0px', opacity: '0' }, 400);
		$("#main-wrapper").animate({ marginLeft: '35px' }, 400, function () {
			triggerWindowResize();
		});
	} else {
		$("#main-wrapper").css({ "min-width": "725px" });
		$("#sidebar-wrapper").animate({ width: '365px', opacity: '1' }, 400);
		$("#main-wrapper").animate({ marginLeft: '400px' }, 400, function () {
			triggerWindowResize();
		});
	}
	isSideMenuOpen = !isSideMenuOpen;
}
//------------------------------------------------------------------------------------------------------------------


//------------------------------------------------------------------------------------------------------------------
// Signalr Notifications
//------------------------------------------------------------------------------------------------------------------
notificationHub.client.SendTradeDataUpdate = function (notification) {
	if (notification.DataType == 3) {
		updateMarketItem(notification);
	}

	if (notification.TradePairId == selectedTradePair.TradePairId) {
		if (notification.DataType == 0) {
			updateOrderbook(notification);
		}
		else if (notification.DataType == 1) {
			addMarketHistory(notification);
		}
		else if (notification.DataType == 3) {
			updateTicker(notification);
		}
	}
}

notificationHub.client.SendUserTradeDataUpdate = function (notification) {
	if (notification.DataType == 2) {
		if (sideMenuOpenOrdersTable) {
			updateOpenOrders(notification);
		}
	}

	if (notification.DataType == 4) {
		updateBalance(notification.TradePairId, true)
	}

	if (selectedTradePair && notification.TradePairId == selectedTradePair.TradePairId) {
		if (notification.DataType == 1) {
			addUserTradeHistory(notification);
		}
		if (notification.DataType == 2) {
			updateUserOpenOrders(notification);
		}
	}
};

//------------------------------------------------------------------------------------------------------------------


//------------------------------------------------------------------------------------------------------------------
// Balance SideMenu
//------------------------------------------------------------------------------------------------------------------
function setupBalances() {
	if (!sideMenuBalanceTable) {
		sideMenuBalanceTable = $('#userBalances').DataTable({
			"dom": "<'row'<'col-sm-12'tr>>",
			"order": [[balanceTableSortColumn, balanceTableSortDirection]],
			"lengthChange": false,
			"processing": false,
			"bServerSide": false,
			"searching": true,
			"paging": false,
			"sort": true,
			"info": false,
			"scrollX": "100%",
			"sAjaxSource": actionTradeBalances,
			"sServerMethod": "POST",
			"language": {
				"emptyTable": Resources.Exchange.BalanceEmptyListMessage,
				"sZeroRecords": Resources.Exchange.BalanceEmptyListMessage,
				"paginate": {
					"previous": Resources.General.Previous,
					"next": Resources.General.Next
				}
			},
			"columnDefs": [
				{ "targets": [5, 6, 7, 8], "visible": false },
				{
					"targets": [0],
					"visible": true,
					"sortable": false,
					"render": function (data, type, full, meta) {
						var active = full[8] ? " balance-favorite-active" : "";
						return '<div class="balance-favorite balance-favorite-' + data + active + '" data-balanceid="' + data + '"><i class="fa fa-ellipsis-v" aria-hidden="true" style="margin-left:5px"></i></div>';
					}
				},
				{
					"targets": 1,
					"searchable": true,
					"orderable": true,
					"render": function (data, type, full, meta) {
                        return '<div style="display:inline-block;white-space:nowrap"><div class="sprite-small ' + data + '-small-png"></div> ' + data + '</div>';
					}
				},
				{
					"targets": 2,
					"searchable": true,
					"orderable": true,
					"render": function (data, type, full, meta) {
						return '<div class="text-right">' + (+data || 0).toFixed(8) + '</div>';
					}
				},
				{
					"targets": 3,
					"searchable": true,
					"orderable": true,
					"render": function (data, type, full, meta) {
						return '<div class="text-right">' + (+data || 0).toFixed(8) + '</div>';
					}
				},
				{
					"targets": 4,
					"searchable": true,
					"orderable": true,
					"render": function (data, type, full, meta) {
						return '<div class="text-right">' + (+data || 0).toFixed(2) + '</div>';
					}
				}],
			"fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
				$(nRow).addClass("balance-" + aData[1]);
				$(nRow).addClass("balanceid-" + aData[0]);
			},
			"fnDrawCallback": function (oSettings, adata) {
				setupBalanceList();
			}
		});

		$("#userBalances_wrapper .dataTables_scrollHead th").on("click", function (e) {
			var index = $(this)[0].cellIndex;
			var direction = $(this).hasClass("sorting_asc") ? "asc" : "desc";
			store.set("balance-sort-col", index);
			store.set("balance-sort-dir", direction);
		});
	}
	else {
		sideMenuBalanceTable.columns.adjust();
	}
}

$('#balance-search').keyup(function () {
	sideMenuBalanceTable.search($(this).val()).draw();
})

$('#sideMenu-balance-hidezero').click(function () {
	var isChecked = $(this).is(":checked");
	postJson(actionHideZeroBalances, { hide: isChecked });
	showZeroBalances = !isChecked;
	sideMenuBalanceTable.draw();
});

$('#sideMenu-balance-favorites').click(function () {
	var isChecked = $(this).is(":checked");
	postJson(actionShowFavoriteBalances, { show: isChecked });
	showFavoriteBalances = isChecked;
	sideMenuBalanceTable.draw();
});

$("#userBalances").on("click", ".balance-favorite", function (e) {
	e.stopPropagation();
	var balanceid = $(this).data('balanceid');
	var balanceDataSet = sideMenuBalanceTable.rows().data();
	var balanceRow = $('#userBalances .balanceid-' + balanceid)
	for (var i = 0; i < balanceDataSet.length; i++) {
		var balanceData = balanceDataSet[i];
		if (balanceData[0] == balanceid) {
			balanceData[8] = !balanceData[8];
			sideMenuBalanceTable.row(balanceRow).invalidate().draw();
			postJson(actionSetFavoriteBalance, { currencyId: balanceid });
			break;
		}
	}
});

function balanceFilter(settings, data) {
	if (settings.sInstance == "userBalances") {
		var zeroBalance = data[5] == 0;
		var favoriteBalance = data[8] == "true";
		if (showFavoriteBalances) {
			return zeroBalance ? showZeroBalances && favoriteBalance : favoriteBalance;
		}
		return !showZeroBalances && zeroBalance ? false : true;
	}
	return true;
}

function setupBalanceList() {
	var parentHeight = $(".stackmenu-content").height();
	var headerHeight = $("#userBalances_wrapper .dataTables_scrollHead").height();
	var footerHeight = $("#userBalances_wrapper .dataTables_filter").height();
	$('#userBalances_wrapper .dataTables_scrollBody').height(parentHeight - (headerHeight + footerHeight));
}

// Datatable pre-filters
$.fn.dataTable.ext.search.push(balanceFilter);

function updateBalance(tradePairId, isUpdate) {
	if (getTradePairBalanceRequest && getTradePairBalanceRequest.readyState != 4) {
		getTradePairBalanceRequest.abort();
	}
	postJson(actionTradePairBalance, { tradePairId: tradePairId }, function (response) {
		if (!response.IsError) {
			if (isUpdate) {
				if (response.Symbol == selectedTradePair.Symbol) {
					$('#userBalanceSell').html(response.Available.toFixed(8));
				}
				if (response.BaseSymbol == selectedTradePair.BaseSymbol) {
					$('#userBalanceBuy').html(response.BaseAvailable.toFixed(8));
				}
			}
			else {
				$('#userBalanceSell').html(response.Available.toFixed(8));
				$('#userBalanceBuy').html(response.BaseAvailable.toFixed(8));
			}

			if (sideMenuBalanceTable) {
				var found = 0;
				var balanceDataSet = sideMenuBalanceTable.rows().data();
				var balanceRow1 = $('#userBalances .balance-' + response.Symbol)
				var balanceRow2 = $('#userBalances .balance-' + response.BaseSymbol)
				for (var i = 0; i < balanceDataSet.length; i++) {
					var balanceData = balanceDataSet[i];
					if (balanceData[1] == response.Symbol) {
						balanceData[2] = response.Available.toFixed(8);
						balanceData[3] = response.HeldForOrders.toFixed(8);
						sideMenuBalanceTable.row(balanceRow1).invalidate();
						found++;

					}
					else if (balanceData[1] == response.BaseSymbol) {
						balanceData[2] = response.BaseAvailable.toFixed(8);
						balanceData[3] = response.BaseHeldForOrders.toFixed(8);
						sideMenuBalanceTable.row(balanceRow2).invalidate();
						found++;
					}
					if (found == 2) {
						break;
					}
				}
			}
		}
	});
}

function clearBalance() {
	$('#userBalanceSell, #userBalanceSell').html('0.00000000');
}

//------------------------------------------------------------------------------------------------------------------


//------------------------------------------------------------------------------------------------------------------
// OpenOrders SideMenu
//------------------------------------------------------------------------------------------------------------------

function setupOpenOrders() {
	if (!sideMenuOpenOrdersTable) {
		sideMenuOpenOrdersTable = $('#sideMenuOpenOrders').DataTable({
			"dom": "<'row'<'col-sm-12'tr>>",
			"order": [[0, "asc"]],
			"lengthChange": false,
			"processing": false,
			"bServerSide": false,
			"searching": true,
			"paging": false,
			"sort": true,
			"info": false,
			"scrollX": "100%",
			"sAjaxSource": actionUserOpenTrades,
			"sServerMethod": "POST",
			"language": {
				"emptyTable": Resources.Exchange.OrdersEmptyListMessage,
				"sZeroRecords": Resources.Exchange.OrdersEmptyListMessage,
				"paginate": {
					"previous": Resources.General.Previous,
					"next": Resources.General.Next
				}
			},
			"columnDefs": [
				{ "targets": 5, "visible": false },
				{
					"targets": 0,
					"searchable": true,
					"orderable": true,
					"render": function (data, type, full, meta) {
						var market = data.replace("/", "_");
						var symbol = market.split("_")[0];
                        return '<div style="display:inline-block;white-space:nowrap"><div class="sprite-small ' + symbol + '-small-png"></div><a href="/Exchange?market=' + market + '"> ' + data + '</a></div>';
					}
				},
				{
					"targets": 2,
					"searchable": true,
					"orderable": true,
					"render": function (data, type, full, meta) {
						return '<div class="text-right">' + (+data || 0).toFixed(8) + '</div>';
					}
				},
				{
					"targets": 3,
					"searchable": true,
					"orderable": true,
					"render": function (data, type, full, meta) {
						return '<div class="text-right">' + (+data || 0).toFixed(8) + '</div>';
					}
				},
				{
					"targets": 4,
					"searchable": false,
					"orderable": false,
					"render": function (data, type, full, meta) {
						return '<div class="text-center"><i style="font-size:12px" class="trade-item-remove fa fa-times" data-orderid="' + data + '" data-tradepairid="' + full[5] + '" ></i></div>';
					}
				}],
			"fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
				$(nRow).addClass("order-" + aData[4]);
			},
			"fnDrawCallback": function () {
				setupOrderList();
			}
		});
	}
	else {
		sideMenuOpenOrdersTable.columns.adjust();
	}
}

$('#openorders-search').keyup(function () {
	sideMenuOpenOrdersTable.search($(this).val()).draw();
})

function updateOpenOrders(notification) {
	var orderDataSet = sideMenuOpenOrdersTable.rows().data();
	var orderRow = $('#sideMenuOpenOrders .order-' + notification.OrderId)
	if (notification.Action == 1 || notification.Action == 3) {
		for (var i = 0; i < orderDataSet.length; i++) {
			var orderData = orderDataSet[i];
			if (orderData[4] == notification.OrderId) {
				sideMenuOpenOrdersTable.row(orderRow).remove().draw();
				break;
			}
		}
	}
	else if (notification.Action == 2) {
		for (var i = 0; i < orderDataSet.length; i++) {
			var orderData = orderDataSet[i];
			if (orderData[4] == notification.OrderId) {
				orderData[3] = notification.Remaining.toFixed(8)
				sideMenuOpenOrdersTable.row(orderRow).invalidate();
				break;
			}
		}
	}
	else if (notification.Action == 0) {
		var orderType = notification.Type == 0 ? "Buy" : "Sell";
		var marketName = notification.Market;
		sideMenuOpenOrdersTable.row.add([marketName, orderType, notification.Rate.toFixed(8), notification.Remaining.toFixed(8), notification.OrderId, notification.TradePairId]).draw();
	}
}

function setupOrderList() {
	var parentHeight = $(".stackmenu-content").height();
	var headerHeight = $("#sideMenuOpenOrders_wrapper .dataTables_scrollHead").height();
	var footerHeight = $("#sideMenuOpenOrders_wrapper .dataTables_filter").height();
	$('#sideMenuOpenOrders_wrapper .dataTables_scrollBody').height(parentHeight - (headerHeight + footerHeight))
}

//------------------------------------------------------------------------------------------------------------------


//------------------------------------------------------------------------------------------------------------------
// Chat
//------------------------------------------------------------------------------------------------------------------
function setupChatList() {
	var footerHeight = $(".chat-menu .chat-footer").height();
	$(".chat-menu .chat-container").height($(".stackmenu-body").height() - footerHeight)
}

function enableChat() {
	chatModule.initializeChat();
}

function disableChat() {
	chatModule.destroy();
}

//------------------------------------------------------------------------------------------------------------------


//------------------------------------------------------------------------------------------------------------------
// Global
//------------------------------------------------------------------------------------------------------------------
// Datatable pre-filters
$.fn.dataTable.ext.search.push(marketFavoriteFilter);

// Window Events
$(window).resize(function () {
	setupMarketList(true);
	setupChatList();
	setupBalanceList();
	setupOrderList();
	if (marketSummaryView) {
		adjustTableHeaders(marketSummaryTables[currentBaseMarket]);
	}
	else {
		adjustTableHeaders(buyOrdersTable);
		adjustTableHeaders(sellOrdersTable);
		adjustTableHeaders(marketHistoryTable);
		adjustTableHeaders(userOpenOrdersTable);
		adjustTableHeaders(userOrderHistoryTable);
		setSellVolumeIndicator();
		setBuyVolumeIndicator();
		resizeCharts();
	}
});

$(document).on('click', '.dropdown-menu', function (e) {
	e.stopPropagation();
});
//------------------------------------------------------------------------------------------------------------------

function adjustTableHeaders(table) {
	if (table) {
		table.columns.adjust();
	}
}


//------------------------------------------------------------------------------------------------------------------
// OrderBooks
//------------------------------------------------------------------------------------------------------------------

function createSellOrdersTable() {
	if (!sellOrdersTable) {
		$('#sellorders > tbody').empty();
		sellOrdersTable = $('#sellorders').DataTable({
			"order": [[1, "asc"]],
			"lengthChange": false,
			"processing": false,
			"bServerSide": false,
			"searching": false,
			"sort": false,
			"paging": false,
			"info": false,
			"scrollY": "250px",
			"scrollCollapse": false,
			"bAutoWidth": false,
			"language": {
				"emptyTable": Resources.Exchange.HistorySellOrdersEmptyList,
				"paginate": {
					"previous": Resources.General.Previous,
					"next": Resources.General.Next
				}
			},

			"columnDefs": [{
				"targets": [1, 2, 3, 4],
				"orderable": false,
				"render": function (data, type, full, meta) {
					return '<div class="text-right">' + (+data || 0).toFixed(8) + '</div>';
				}
			},
			{
				"targets": 0,
				"orderable": false,
				"render": function (data, type, full, meta) {
					var key = (+full[1]).toFixed(8);
					return '<div class="orderbook-indicator" data-price="' + key + '"><i class="fa fa-ellipsis-v" aria-hidden="true"></i></div>';
				}
			}],
			"fnDrawCallback": function (data) {
				if (data.aoData.length > 0) {
					setUserOrderIndicator();
					setOrderbookSumTotal("#sellorders");
				}
			}
		});
	}
}

function createBuyOrdersTable() {
	if (!buyOrdersTable) {
		$('#buyorders > tbody').empty();
		buyOrdersTable = $('#buyorders').DataTable({
			"order": [[1, "desc"]],
			"lengthChange": false,
			"processing": false,
			"bServerSide": false,
			"searching": false,
			"paging": false,
			"sort": false,
			"info": false,
			"scrollY": "250px",
			"scrollCollapse": false,
			"language": {
				"emptyTable": Resources.Exchange.HistoryBuyOrdersEmptyList,
				"paginate": {
					"previous": Resources.General.Previous,
					"next": Resources.General.Next
				}
			},
			"columnDefs": [{
				"targets": [1, 2, 3, 4],
				"orderable": false,
				"render": function (data, type, full, meta) {
					return '<div class="text-right">' + (+data || 0).toFixed(8) + '</div>';
				}
			},
			{
				"targets": 0,
				"searchable": false,
				"orderable": false,
				"render": function (data, type, full, meta) {
					var key = (+full[1]).toFixed(8);
					return '<div class="orderbook-indicator" data-price="' + key + '"><i class="fa fa-ellipsis-v" aria-hidden="true"></i></div>';
				}
			}],
			"fnDrawCallback": function (data) {
				if (data.aoData.length > 0) {
					setUserOrderIndicator();
					setOrderbookSumTotal("#buyorders");
				}
			}
		});
	}
}

function updateSellOrdersTable(selldata) {
	createSellOrdersTable();
	sellOrdersTable.clear().draw();
	if (selldata && selldata.length > 0) {
		sellOrdersTable.rows.add(selldata).draw();
	}
}

function clearSellOrdersTable() {
	if (sellOrdersTable) {
		sellOrdersTable.clear().draw();
	}
}

function updateBuyOrdersTable(buydata) {
	createBuyOrdersTable();
	buyOrdersTable.clear().draw();
	if (buydata && buydata.length > 0) {
		buyOrdersTable.rows.add(buydata).draw();
	}
}

function clearBuyOrdersTable() {
	if (buyOrdersTable) {
		buyOrdersTable.clear().draw();
	}
}

var setBuyVolumeIndicatorTimeout;
function setBuyVolumeIndicator() {
	clearTimeout(setBuyVolumeIndicatorTimeout);
	setBuyVolumeIndicatorTimeout = setTimeout(function () {
		updateOrderBookVolumeIndicator('Buy');
	}, 250);
}

var setSellVolumeIndicatorTimeout;
function setSellVolumeIndicator() {
	clearTimeout(setSellVolumeIndicatorTimeout);
	setSellVolumeIndicatorTimeout = setTimeout(function () {
		updateOrderBookVolumeIndicator('Sell');
	}, 250);
}

function updateOrderBookVolumeIndicator(orderbook) {
	if (orderbook == 'Buy') {
		var total_buy = $('#orderbook-total-buy').text();
		if (total_buy > 0) {
			$('.panel-container-buy .table-striped > tbody > tr').each(function (index) {
				$(this).children().css({ 'background-size': '0px', 'background-position-x': '0px' })
				var buypercent = $(this).children(':nth-child(5)').text() / total_buy * 100;
				var width = ~~(Math.max(($(this).outerWidth() / 100) * buypercent, 5));
				var c1 = $(this).children(':nth-child(1)'), c2 = $(this).children(':nth-child(2)'), c3 = $(this).children(':nth-child(3)'), c4 = $(this).children(':nth-child(4)'), c5 = $(this).children(':nth-child(5)');
				var c1w = c1.outerWidth(), c2w = c2.outerWidth(), c3w = c3.outerWidth(), c4w = c4.outerWidth(), c5w = c5.outerWidth()
				if (width >= c5w) {
					c5.css({ 'background-size': '100%', 'background-position-x': '0px' });
					if (width >= (c5w + c4w)) {
						c4.css({ 'background-size': '100%', 'background-position-x': '0px' });
						if (width >= (c5w + c4w + c3w)) {
							c3.css({ 'background-size': '100%', 'background-position-x': '0px' });
							if (width >= (c5w + c4w + c3w + c2w)) {
								c2.css({ 'background-size': '100%', 'background-position-x': '0px' });
								if (width >= (c5w + c4w + c3w + c2w + c1w)) {
									c1.css({ 'background-size': '100%', 'background-position-x': '0px' });
								}
								else {
									var s1 = width - (c5w + c4w + c3w + c2w);
									c1.css({ 'background-size': s1 + "px", 'background-position-x': (c1w - s1) + 'px' });
								}
							}
							else {
								var s2 = width - (c5w + c4w + c3w);
								c2.css({ 'background-size': s2 + "px", 'background-position-x': (c2w - s2) + 'px' });
							}
						}
						else {
							var s3 = width - (c5w + c4w);
							c3.css({ 'background-size': s3 + "px", 'background-position-x': (c3w - s3) + 'px' });
						}
					}
					else {
						var s4 = width - c5w;
						c4.css({ 'background-size': s4 + "px", 'background-position-x': (c4w - s4) + 'px' });
					}
				}
				else {
					var s5 = width;
					c5.css({ 'background-size': s5 + "px", 'background-position-x': (c5w - s5) + 'px' });
				}
			});
		}
	}
	else {
		var total_sell = $('#orderbook-total-sell').text();
		if (total_sell > 0) {
			var amount = 0;
			$('.panel-container-sell .table-striped > tbody > tr').each(function (index) {
				$(this).children().css({ 'background-size': '0px', 'background-position-x': '0px' })
				amount = amount + +$(this).children(':nth-child(3)').text();
				var sellpercent = amount / total_sell * 100.0;
				var width = ~~(Math.max(($(this).outerWidth() / 100) * sellpercent, 5));
				var c1 = $(this).children(':nth-child(1)'), c2 = $(this).children(':nth-child(2)'), c3 = $(this).children(':nth-child(3)'), c4 = $(this).children(':nth-child(4)'), c5 = $(this).children(':nth-child(5)');
				var c1w = c1.outerWidth(), c2w = c2.outerWidth(), c3w = c3.outerWidth(), c4w = c4.outerWidth(), c5w = c5.outerWidth()
				if (width >= c1w) {
					c1.css({ 'background-size': '100%', 'background-position-x': '0px' });
					if (width >= (c1w + c2w)) {
						c2.css({ 'background-size': '100%', 'background-position-x': '0px' });
						if (width >= (c1w + c2w + c3w)) {
							c3.css({ 'background-size': '100%', 'background-position-x': '0px' });
							if (width >= (c1w + c2w + c3w + c4w)) {
								c4.css({ 'background-size': '100%', 'background-position-x': '0px' });
								if (width >= (c1w + c2w + c3w + c4w + c5w)) {
									c5.css({ 'background-size': '100%', 'background-position-x': '0px' });
								}
								else {
									var s5 = width - (c1w + c2w + c3w + c4w);
									c5.css({ 'background-size': s5 + "px", 'background-position-x': '0px' });
								}
							}
							else {
								var s4 = width - (c1w + c2w + c3w);
								c4.css({ 'background-size': s4 + "px", 'background-position-x': '0px' });
							}
						}
						else {
							var s3 = width - (c1w + c2w);
							c3.css({ 'background-size': s3 + "px", 'background-position-x': '0px' });
						}
					}
					else {
						var s2 = width - c1w;
						c2.css({ 'background-size': s2 + "px", 'background-position-x': '0px' });
					}
				}
				else {
					var s1 = width;
					c1.css({ 'background-size': s1 + "px", 'background-position-x': '0px' });
				}
			});
		}
	}
}

function updateOrderbook(notification) {
	var table;
	var existingRow;
	if (notification.Action == 1) {
		table = notification.Type == 0 ? "#sellorders" : "#buyorders";
		existingRow = $(table + " > tbody td > div").filter(function (index) {
			return +$(this).text() == notification.Rate;
		}).closest("tr");
		updateOrderbookRow(table, existingRow, notification);
	}
	else if (notification.Action == 0) {

		var isBuyOrder = notification.Type == 0;
		table = isBuyOrder ? "#buyorders" : "#sellorders";
		existingRow = $(table + " > tbody td > div").filter(function (index) {
			return +$(this).text() == notification.Rate;
		}).closest("tr");

		// If no rows exist, create one
		var firstPrice = $(table + " > tbody tr:first > td:nth-child(2) > div").text() || Resources.Exchange.HistorySellOrdersEmptyList
		if (firstPrice === Resources.Exchange.HistorySellOrdersEmptyList || firstPrice === Resources.Exchange.HistoryBuyOrdersEmptyList) {
			$(table + " > tbody tr:first").remove();
			appendOrderbookRow(table, notification);
			return;
		}

		var lastPrice = $(table + " > tbody tr:last > td:nth-child(2) > div").text()
		var existingRowPrice = existingRow.find("td:nth-child(2) > div").text()
		if (existingRow && existingRowPrice == notification.Rate) {
			// update existing
			updateOrderbookRow(table, existingRow, notification);
		}
		else if ((!isBuyOrder && firstPrice > notification.Rate) || (isBuyOrder && notification.Rate > firstPrice)) {
			// less than first
			prependOrderbookRow(table, notification);
		}
		else if ((!isBuyOrder && notification.Rate > +lastPrice) || (isBuyOrder && notification.Rate < +lastPrice)) {
			// more than last
			appendOrderbookRow(table, notification);
		}
		else {
			// somewhere in middle
			insertOrderbookRow(table, notification)
		}
	}
	else if (notification.Action == 3) {
		table = notification.Type == 0 ? "#buyorders" : "#sellorders";
		existingRow = $(table + " > tbody td > div").filter(function (index) {
			return +$(this).text() == notification.Rate;
		}).closest("tr");
		updateOrderbookRow(table, existingRow, notification);
	}
}

function appendOrderbookRow(table, notification) {
	$(table + ' > tbody').append(Mustache.render(orderTemplate, {
		highlight: "greenhighlight",
		price: notification.Rate.toFixed(8),
		amount: notification.Amount.toFixed(8),
		total: (notification.Amount * notification.Rate).toFixed(8)
	}));
	setOrderbookSumTotal(table);
}

function prependOrderbookRow(table, notification) {
	var html = Mustache.render(orderTemplate, {
		highlight: "greenhighlight",
		price: notification.Rate.toFixed(8),
		amount: notification.Amount.toFixed(8),
		total: (notification.Amount * notification.Rate).toFixed(8)
	})
	var row = $(table + ' > tbody').prepend(html);
	setOrderbookSumTotal(table);
}

function insertOrderbookRow(table, notification) {
	var row = $(table + " > tbody td:nth-child(2) > div").filter(function () {
		return notification.Type === 0
			? +$(this).text() < notification.Rate
			: +$(this).text() > notification.Rate;
	}).first().closest("tr");

	var html = Mustache.render(orderTemplate, {
		highlight: "greenhighlight",
		price: notification.Rate.toFixed(8),
		amount: notification.Amount.toFixed(8),
		total: (notification.Amount * notification.Rate).toFixed(8)
	});
	row.before(html);
	setOrderbookSumTotal(table);
}

function updateOrderbookRow(table, existingRow, notification) {
	var amountColumn = existingRow.find("td:nth-child(3) > div");
	var totalColumn = existingRow.find("td:nth-child(4) > div");
	var existingAmount = +amountColumn.text();

	var newAmount = notification.Action == 1 || notification.Action == 3
		? (existingAmount - notification.Amount).toFixed(8)
		: (existingAmount + notification.Amount).toFixed(8);

	var newTotal = (newAmount * notification.Rate).toFixed(8)
	if (isNaN(newAmount) || isNaN(newTotal) || newAmount <= 0 || newTotal <= 0) {
		existingRow.remove();
	}
	else {
		amountColumn.text(newAmount);
		totalColumn.text(newTotal);

		if (notification.Action == 3) {
			highlightRow(existingRow, 'red')
		}
		else if (newAmount > existingAmount) {
			highlightRow(existingRow, 'green');
		}

	}
	setOrderbookSumTotal(table)
}

function setOrderbookSumTotal(table) {
	if (table === '#buyorders') {
		setBuySumTotal();
		//setBuyVolumeIndicator();
	}
	if (table === '#sellorders') {
		setSellSumTotal();
		//setSellVolumeIndicator();
	}
	updateOrderBookChartThrottle();
}

var setSellSumTotalTimeout;
function setSellSumTotal() {
	clearTimeout(setSellSumTotalTimeout);
	setSellSumTotalTimeout = setTimeout(function () {
		calculateOrderbookSum('#sellorders');
	}, 50);
}

var setBuySumTotalTimeout;
function setBuySumTotal() {
	clearTimeout(setBuySumTotalTimeout);
	setBuySumTotalTimeout = setTimeout(function () {
		calculateOrderbookSum('#buyorders');
	}, 50);
}

function calculateOrderbookSum(table) {
	var sumTotal = 0;
	var sumVolume = 0;
	$(table + " > tbody  > tr").each(function () {
		var row = $(this);
		sumTotal += +row.find("td:nth-child(4) > div").text();
		sumVolume += +row.find("td:nth-child(3) > div").text()
		row.find("td:nth-child(5) > div").text(sumTotal.toFixed(8))
	});

	if (table === '#buyorders') {
		$("#orderbook-total-buy").html(sumTotal.toFixed(8));
	}
	if (table === '#sellorders') {
		$("#orderbook-total-sell").html(sumVolume.toFixed(8));
	}
}

//------------------------------------------------------------------------------------------------------------------


//------------------------------------------------------------------------------------------------------------------
// Maket History
//------------------------------------------------------------------------------------------------------------------

function createMarketHistoryTable() {
	if (!marketHistoryTable) {
		$('#markethistory > tbody').empty();
		marketHistoryTable = $('#markethistory').DataTable({
			"order": [[0, 'desc']],
			"lengthChange": false,
			"processing": false,
			"bServerSide": false,
			"sort": false,
			"searching": false,
			"paging": false,
			"info": false,
			"scrollY": "300px",
			"scrollCollapse": false,
			"bAutoWidth": false,
			"language": {
				"emptyTable": Resources.Exchange.HistoryMarketEmtpyList,
				"paginate": {
					"previous": Resources.General.Previous,
					"next": Resources.General.Next
				}
			},
			fnRowCallback: function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
				$(nRow).addClass('history-' + aData[1])
			},
			"columnDefs": [{
				"targets": [2, 3, 4],
				"orderable": false,
				"render": function (data, type, full, meta) {
					return '<div class="text-right">' + (+data || 0).toFixed(8) + '</div>';
				}
			},
			{
				"targets": 0,
				"orderable": false,
				"render": function (data, type, full, meta) {
					return '<div style="margin-left:8px;white-space: nowrap;">' + toLocalTime(data) + '</div>';
				}
			}]
		});
	}
}

function updateMarketHistoryTable(marketData) {
	createMarketHistoryTable();
	marketHistoryTable.clear();
	if (marketData && marketData.length > 0) {
		marketHistoryTable.rows.add(marketData).draw();
	}
}

function clearMarketHistoryTable() {
	if (marketHistoryTable) {
		marketHistoryTable.clear().draw();
	}
}

function addMarketHistory(notification) {
	var marketBody = $("#markethistory tbody");
	marketBody.find("tr > .dataTables_empty").closest("tr").remove();
	var orderType = notification.Type === 0 ? 'Buy' : 'Sell';
	marketBody.prepend(Mustache.render(tradeHistoryTemplate, {
		highlight: notification.Type === 0 ? ('greenhighlight history-' + orderType) : ('redhighlight history-' + orderType),
		time: toLocalTime(notification.Timestamp),
		type: orderType,
		rate: notification.Rate.toFixed(8),
		amount: notification.Amount.toFixed(8),
		total: (notification.Rate * notification.Amount).toFixed(8)
	}));
}
//------------------------------------------------------------------------------------------------------------------


//------------------------------------------------------------------------------------------------------------------
// User Open Orders
//------------------------------------------------------------------------------------------------------------------

function createUserOpenOrdersTable() {
	if (!userOpenOrdersTable) {
		$('#useropenorders > tbody').empty();
		userOpenOrdersTable = $('#useropenorders').DataTable({
			"order": [[0, "desc"]],
			"lengthChange": false,
			"processing": false,
			"bServerSide": false,
			"searching": false,
			"paging": false,
			"sort": true,
			"info": false,
			"scrollY": "300px",
			"scrollCollapse": true,
			"bAutoWidth": false,
			"language": {
				"emptyTable": Resources.Exchange.HistoryMyOpenOrdersEmtpyList,
				"paginate": {
					"previous": Resources.General.Previous,
					"next": Resources.General.Next
				}
			},
			fnRowCallback: function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
				$(nRow).addClass("openorder-" + aData[6]).addClass('history-' + aData[1])
			},
			"columnDefs": [{
				"targets": [2, 3, 4, 5],
				"render": function (data, type, full, meta) {
					return '<div class="text-right">' + (+data || 0).toFixed(8) + '</div>';
				}
			},
			{
				"targets": 0,
				"searchable": true,
				"orderable": true,
				"render": function (data, type, full, meta) {
					return '<div style="margin-left:8px;white-space: nowrap;">' + toLocalTime(data) + '</div>';
				}
			}, {
				"targets": -1,
				"searchable": false,
				"orderable": false,
				"render": function (data, type, full, meta) {
					return '<div class="text-right"><i class="trade-item-remove fa fa-times" data-orderid="' + data + '"></i></div>';
				}
			}],
			"fnDrawCallback": function (data) {
				if (data.aoData.length > 0) {
					setUserOrderIndicator();
				}
			}
		});
	}
}

function updateUserOpenOrdersTable(marketData) {
	userOpenOrdersTable.clear().draw();
	if (marketData && marketData.length > 0) {
		userOpenOrdersTable.rows.add(marketData).draw();
	}
}

function clearUserOpenOrdersTable() {
	if (userOpenOrdersTable) {
		userOpenOrdersTable.clear().draw();
	}
}

$("#useropenorders, #sideMenuOpenOrders").on("click", ".trade-item-remove", function () {
	var orderId = $(this).data("orderid");
	var tradepairId = $(this).data("tradepairid") || selectedTradePair.TradePairId;
	if (orderId > 0 && tradepairId > 0) {
		cancelOrder(orderId, tradepairId);
	}
});

$(".panel-container-useropenorders").on("click", ".trade-items-remove", function () {
	var tradepairId = selectedTradePair.TradePairId;
	if (tradepairId > 0) {
		cancelTradePairOrders(tradepairId);
	}
});

function updateUserOpenOrders(notification) {
	if (notification.Action === 0) {
		var orderType = notification.Type === 0 ? 'Buy' : 'Sell';
		userOpenOrdersTable.row.add([
			toLocalTime(notification.Timestamp),
			orderType,
			notification.Rate.toFixed(8),
			notification.Amount.toFixed(8),
			notification.Remaining.toFixed(8),
			notification.Total.toFixed(8),
			notification.OrderId,
			notification.TradePairId
		]).draw();
	}
	else if (notification.Action === 1 || notification.Action === 3) {
		var orderRow = $("#useropenorders tbody > tr.openorder-" + notification.OrderId);
		userOpenOrdersTable.row(orderRow).remove().draw();
	}
	else if (notification.Action === 2) {
		var ordersDataSet = userOpenOrdersTable.rows().data();
		var orderRow = $('#useropenorders tbody > tr.openorder-' + notification.OrderId)
		for (var i = 0; i < ordersDataSet.length; i++) {
			var orderData = ordersDataSet[i];
			if (orderData[6] == notification.OrderId) {
				orderData[4] = notification.Remaining.toFixed(8);
				userOpenOrdersTable.row(orderRow).invalidate().draw();
				break;
			}
		}

	}
	setUserOrderIndicator();
}

//------------------------------------------------------------------------------------------------------------------


//------------------------------------------------------------------------------------------------------------------
// User Market History
//------------------------------------------------------------------------------------------------------------------

function createUserOrderHistoryTable() {
	if (!userOrderHistoryTable) {
		$('#userorderhistory > tbody').empty();
		userOrderHistoryTable = $('#userorderhistory').DataTable({
			"order": [[0, "desc"]],
			"lengthChange": false,
			"processing": false,
			"bServerSide": false,
			"searching": false,
			"paging": false,
			"sort": false,
			"info": false,
			"scrollY": "300px",
			"scrollCollapse": true,
			"bAutoWidth": false,
			"language": {
				"emptyTable": Resources.Exchange.HistoryMyOrdersEmtpyList,
				"paginate": {
					"previous": Resources.General.Previous,
					"next": Resources.General.Next
				}
			},
			fnRowCallback: function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
				$(nRow).addClass('history-' + aData[1])
			},
			"columnDefs": [{
				"targets": [2, 3, 4],
				"orderable": false,
				"render": function (data, type, full, meta) {
					return '<div class="text-right">' + (+data || 0).toFixed(8) + '</div>';
				}
			},
			{
				"targets": 0,
				"searchable": false,
				"orderable": true,
				"render": function (data, type, full, meta) {
					return '<div style="margin-left:8px;white-space: nowrap;">' + toLocalTime(data) + '</div>';
				}
			}]
		});
	}
}

function updateUserOrderHistoryTable(marketData) {
	userOrderHistoryTable.clear().draw();
	if (marketData && marketData.length > 0) {
		userOrderHistoryTable.rows.add(marketData).draw();
	}
}

function clearUserOrderHistoryTable() {
	if (userOrderHistoryTable) {
		userOrderHistoryTable.clear().draw();
	}
}

function addUserTradeHistory(notification) {
	var historyBody = $("#userorderhistory tbody");
	historyBody.find("tr > .dataTables_empty").closest("tr").remove();
	var orderType = notification.Type === 0 ? 'Buy' : 'Sell';
	historyBody.prepend(Mustache.render(tradeHistoryTemplate, {
		highlight: notification.Type === 0 ? ('greenhighlight history-' + orderType) : ('redhighlight history-' + orderType),
		time: toLocalTime(notification.Timestamp),
		type: orderType,
		rate: notification.Rate.toFixed(8),
		amount: notification.Amount.toFixed(8),
		total: (notification.Rate * notification.Amount).toFixed(8)
	}));
}

//------------------------------------------------------------------------------------------------------------------



//------------------------------------------------------------------------------------------------------------------
// Buy Sell actions
//------------------------------------------------------------------------------------------------------------------
$("#buysubmit").on("click", function () {
	var _this = $(this);
	var price = new Decimal($("#buyprice").val());
	var amount = new Decimal($("#buyamount").val());
	var total = new Decimal($("#buytotal").val());
	var balance = new Decimal($('#userBalanceBuy').text());
	var minTrade = new Decimal(selectedTradePair.BaseMinTrade);
	if (price.lessThan(0.00000001)) {
		sendNotification(Resources.Exchange.TradeNotificationTitle, String.format(Resources.Exchange.TradeMinPriceError,  '0.00000001'), 2);
		return;
	}
	if (amount.lessThan(0.00000001)) {
		sendNotification(Resources.Exchange.TradeNotificationTitle, String.format(Resources.Exchange.TradeMinPriceError, '0.00000001'), 2);
		return;
	}
	if (total.lessThan(minTrade)) {
		sendNotification(Resources.Exchange.TradeNotificationTitle, String.format(Resources.Exchange.TradeMinTotalError, selectedTradePair.BaseMinTrade, selectedTradePair.BaseSymbol), 2);
		return;
	}
	if (balance.isZero() || total.greaterThan(balance)) {
		sendNotification(Resources.Exchange.TradeNotificationTitle, String.format(Resources.Exchange.TradeInsufficientFundsError, selectedTradePair.BaseSymbol), 2);
		return;
	}

	var data = {
		IsBuy: true,
		Price: price.toFixed(8),
		Amount: amount.toFixed(8),
		TradePairId: selectedTradePair.TradePairId
	};

	$(".buysell-button-loading").show();
	$("#sellsubmit, #buysubmit").attr("disabled", "disabled");
	sendNotification(Resources.Exchange.TradeNotificationTitle, Resources.Exchange.TradeBuyOrderSubmittedMessage);
	postJson(actionSubmitTrade, data, function (result) {
		$(".buysell-button-loading").hide();
		$("#sellsubmit, #buysubmit").removeAttr("disabled", "disabled");
		if (result.Message) {
			sendNotification(Resources.Exchange.TradeNotificationTitle, result.Message, 2);
		}
	});
});

$("#sellsubmit").on("click", function () {
	var _this = $(this);
	var price = new Decimal($("#sellprice").val());
	var amount = new Decimal($("#sellamount").val());
	var total = new Decimal($("#selltotal").val());
	var balance = new Decimal($('#userBalanceSell').text());
	var minTrade = new Decimal(selectedTradePair.BaseMinTrade);
	if (price.lessThan(0.00000001)) {
		sendNotification(Resources.Exchange.TradeNotificationTitle, String.format(Resources.Exchange.TradeMinPriceError, '0.00000001'), 2);
		return;
	}
	if (amount.lessThan(0.00000001)) {
		sendNotification(Resources.Exchange.TradeNotificationTitle, String.format(Resources.Exchange.TradeMinPriceError, '0.00000001'), 2);
		return;
	}
	if (total.lessThan(minTrade)) {
		sendNotification(Resources.Exchange.TradeNotificationTitle, String.format(Resources.Exchange.TradeMinTotalError, selectedTradePair.BaseMinTrade, selectedTradePair.BaseSymbol), 2);
		return;
	}
	if (balance.isZero() || amount.greaterThan(balance)) {
		sendNotification(Resources.Exchange.TradeNotificationTitle, String.format(Resources.Exchange.TradeInsufficientFundsError, selectedTradePair.BaseSymbol), 2);
		return;
	}

	var data = {
		IsBuy: false,
		Price: price.toFixed(8),
		Amount: amount.toFixed(8),
		TradePairId: selectedTradePair.TradePairId
	};

	$(".buysell-button-loading").show();
	$("#sellsubmit, #buysubmit").attr("disabled", "disabled");
	sendNotification(Resources.Exchange.TradeNotificationTitle, Resources.Exchange.TradeSellOrderSubmittedMessage);
	postJson(actionSubmitTrade, data, function (result) {
		$(".buysell-button-loading").hide();
		$("#sellsubmit, #buysubmit").removeAttr("disabled", "disabled");
		if (result.Message) {
			sendNotification(Resources.Exchange.TradeNotificationTitle, result.Message, 2);
		}
	})
});

$("#buyamount").on("keyup paste change", function () {
	var price = new Decimal($("#buyprice").val());
	var amount = new Decimal($(this).val());
	var totalInput = $("#buytotal");
	var total = price.mul(amount);
	totalInput.val(total.toFixed(8));
})

$("#buyprice").on("keyup paste change", function () {
	var price = new Decimal($(this).val());
	var amount = new Decimal($("#buyamount").val());
	var total = price.mul(amount);
	var totalInput = $("#buytotal");
	totalInput.val(total.toFixed(8));
})

$("#buytotal").on("keyup paste change", function () {
	var total = new Decimal($(this).val());
	var price = new Decimal($("#buyprice").val());
	var amount = total.div(price);
	var amountInput = $("#buyamount");
	amountInput.val(amount.toFixed(8));
})

$("#sellamount").on("keyup paste change", function () {
	var price = new Decimal($("#sellprice").val());
	var amount = new Decimal($(this).val());
	var totalInput = $("#selltotal");
	totalInput.val(price.mul(amount).toFixed(8))
})

$("#sellprice").on("keyup paste change", function () {
	var price = new Decimal($(this).val());
	var amount = new Decimal($("#sellamount").val());
	var totalInput = $("#selltotal");
	totalInput.val(price.mul(amount).toFixed(8))
})

$("#selltotal").on("keyup paste change", function () {
	var total = new Decimal($(this).val());
	var price = new Decimal($("#sellprice").val());
	var amount = total.div(price);
	var amountInput = $("#sellamount");
	amountInput.val(amount.toFixed(8))
})

$("#buyamount, #buyprice, #sellamount, #sellprice").on("keyup change", function () {
	truncateInputDecimals($(this), 8);
	calculateFee(true);
})

$("#buyamount, #buyprice, #sellamount, #sellprice").on("blur", function () {
	truncateInputDecimals($(this), 0);
	calculateFee(true);
})

$("#buynettotal, #sellnettotal").on("blur", function () {
	truncateInputDecimals($(this), 0);
	calculateFee(false);
})

function truncateInputDecimals(input, decimals) {
	var value = new Decimal(input.val());
	if (value.dp() >= decimals) {
		input.val(value.toFixed(8));
	}
}

function calculateFee(includeTotal) {
	if (!selectedTradePair) {
		return;
	}

	var tradeFee = new Decimal(selectedTradePair.BaseFee);
	var minTrade = new Decimal(selectedTradePair.BaseMinTrade);

	var buyprice = new Decimal($("#buyprice").val());
	var buyamount = new Decimal($("#buyamount").val());
	var buytotal = buyprice.mul(buyamount);
	var buyfee = buytotal.div(100).mul(tradeFee);
	var buynettotal = buytotal.plus(buyfee);
	$('#buyfee').val(buyfee.toFixed(8));
	$('#buytotal').val(buytotal.toFixed(8));
	if (includeTotal) {
		$('#buynettotal').val(buynettotal.toFixed(8, Decimal.ROUND_UP));
	}
	$('#buysubmit').prop('disabled', buynettotal.lessThan(minTrade));

	var sellprice = new Decimal($("#sellprice").val());
	var sellamount = new Decimal($("#sellamount").val());
	var selltotal = sellprice.mul(sellamount);
	var sellfee = selltotal.div(100).mul(tradeFee);
	var sellnettotal = selltotal.minus(sellfee);
	$('#sellfee').val(sellfee.toFixed(8));
	$('#selltotal').val(selltotal.toFixed(8));
	if (includeTotal) {
		$('#sellnettotal').val(sellnettotal.toFixed(8, Decimal.ROUND_UP));
	}
	$('#sellsubmit').prop('disabled', sellnettotal.lessThan(minTrade));
}

function clearBuySellInputs() {
	$("#buyamount, #buyprice, #buytotal, #sellamount, #sellprice, #selltotal").val((0).toFixed(8));
	calculateFee(true);
}

$("#buynettotal").on("keyup paste change", function () {
	var total = new Decimal($(this).val());
	var price = new Decimal($("#buyprice").val());
	if (total.greaterThan(0) && price.greaterThan(0)) {
		var fee = new Decimal(0.2).div(100).plus(1);
		var totalfee = total.div(fee);
		var amount = totalfee.div(price);
		$("#buyamount").val(amount.toFixed(8));
		calculateFee(false);
	}
});

$("#sellnettotal").on("keyup paste change", function () {
	var total = new Decimal($(this).val());
	var price = new Decimal($("#sellprice").val());
	if (total.greaterThan(0) && price.greaterThan(0)) {
		var fee = new Decimal(99.8).div(100);
		var totalfee = total.div(fee);
		var amount = totalfee.div(price);
		$("#sellamount").val(amount.toFixed(8));
		calculateFee(false);
	}
});

$("#buyorders").on("click", "tr", function () {
	var orderbookRow = $(this);
	var selectedPrice = orderbookRow.find("td:nth-child(2)").text();

	var volume = 0;
	$("#buyorders > tbody  > tr").each(function () {
		var row = $(this);
		var rowprice = +row.find("td:nth-child(2)").text();
		if (rowprice >= selectedPrice) {
			volume += +row.find("td:nth-child(3)").text();
		}
	});

	var volumeValue = new Decimal(volume);
	var selectedPriceValue = new Decimal(selectedPrice);
	$("#buyprice, #sellprice").val(selectedPriceValue.toFixed(8))
	$("#buyamount, #sellamount").val(volumeValue.toFixed(8));
	calculateFee(true);
});

$("#sellorders").on("click", "tr", function () {
	var orderbookRow = $(this);
	var selectedPrice = orderbookRow.find("td:nth-child(2)").text();

	var volume = 0;
	$("#sellorders > tbody  > tr").each(function () {
		var row = $(this);
		var rowprice = +row.find("td:nth-child(2)").text();
		if (rowprice <= selectedPrice) {
			volume += +row.find("td:nth-child(3)").text();
		}
	});

	var volumeValue = new Decimal(volume);
	var selectedPriceValue = new Decimal(selectedPrice);
	$("#buyprice, #sellprice").val(selectedPriceValue.toFixed(8))
	$("#buyamount, #sellamount").val(volumeValue.toFixed(8));
	calculateFee(true);
});

$("#userBalanceBuy").on("click", function () {
	var balance = new Decimal($(this).text());
	var price = new Decimal($("#buyprice").val());
	if (price.greaterThan(0) && balance.greaterThan(0)) {
		var fee = new Decimal(0.2).div(100).plus(1);
		var totalfee = balance.div(fee);
		var amount = totalfee.div(price);
		$("#buyamount").val(amount.toFixed(8));
		calculateFee(true);
	}
});

$("#userBalanceSell").on("click", function () {
	var balance = new Decimal($(this).text());
	var price = new Decimal($("#sellprice").val());
	if (price.greaterThan(0) && balance.greaterThan(0)) {
		$("#sellamount").val(balance.toFixed(8))
		calculateFee(true);
	}
});

$("#sell-first-amount").on("click", function () {
	var firstAmount = $("#buyorders > tbody > tr:first > td:nth-child(3)").text();
	if (firstAmount) {
		$("#sellamount, #buyamount").val(firstAmount).trigger("change");
	}
});

$("#sell-first-price").on("click", function () {
	var firstPrice = $("#buyorders > tbody > tr:first > td:nth-child(2)").text();
	if (firstPrice) {
		$("#sellprice, #buyprice").val(firstPrice).trigger("change");
	}
});

$("#sell-total-amount").on("click", function () {
	var firstprice = $("#buyorders > tbody > tr:first > td:nth-child(2)").text();
	if (firstprice) {
		$("#sellprice").val(firstprice);
		$("#userBalanceSell").trigger("click");
	}
});

$("#buy-first-amount").on("click", function () {
	var firstAmount = $("#sellorders > tbody > tr:first > td:nth-child(3)").text();
	if (firstAmount) {
		$("#buyamount, #sellamount").val(firstAmount).trigger("change");
	}
});

$("#buy-first-price").on("click", function () {
	var firstprice = $("#sellorders > tbody > tr:first > td:nth-child(2)").text();
	if (firstprice) {
		$("#buyprice, #sellprice").val(firstprice).trigger("change");
	}
});

$("#buy-total-amount").on("click", function () {
	var firstprice = $("#sellorders > tbody > tr:first > td:nth-child(2)").text();
	if (firstprice) {
		$("#buyprice").val(firstprice);
		$("#userBalanceBuy").trigger("click");
	}
});

var setUserOrderIndicatorTimeout;
function setUserOrderIndicator() {
	clearTimeout(setUserOrderIndicatorTimeout);
	setUserOrderIndicatorTimeout = setTimeout(function () {
		updateUserOrderIndicator();
	}, 200);
}

function updateUserOrderIndicator() {
	var orderbooks = $('.orderbook-table > tbody > tr > td:nth-child(1)');
	orderbooks.find(".orderbook-indicator").removeClass("orderbook-indicator-active")
	$("#useropenorders > tbody > tr > td:nth-child(3) > div").each(function () {
		orderbooks.find('[data-price="' + $(this).text() + '"]').addClass("orderbook-indicator-active");
	});
}

//------------------------------------------------------------------------------------------------------------------







//------------------------------------------------------------------------------------------------------------------
// Charts
//------------------------------------------------------------------------------------------------------------------

$(".chart-option-chart").on("click", function () {
	selectedChart = 'trade'
	$(".chart-option-btn").removeClass("active")
	$("#chart-orderbook, #chart-distribution, .chart-options-dropdown").hide();
	$("#chart-trade, .chart-options-dropdown-trade").show();

	$(this).addClass("active")
	updateTradeChart();
});


$(".chart-option-orderbook").on("click", function () {
	selectedChart = 'orderbook'
	$(".chart-option-btn").removeClass("active")
	$("#chart-trade, #chart-distribution, .chart-options-dropdown").hide();
	$("#chart-orderbook, .chart-options-dropdown-orderbook").show();

	$(this).addClass("active")
	updateOrderBookChart();
});


$(".chart-option-distribution").on("click", function () {
	selectedChart = 'distribution'
	$(".chart-option-btn").removeClass("active")
	$("#chart-orderbook, #chart-trade, .chart-options-dropdown").hide();
	$("#chart-distribution, .chart-options-dropdown-distribution").show();
	$(this).addClass("active")
	updateDistributionChart();
});

function clearCharts() {
	clearTradeChart();
	clearOrderBookChart();
	clearDistributionChart();
}

function updateSelectedChart() {
	if (selectedChart == 'trade') {

		updateTradeChart();
	}
	else if (selectedChart == 'distribution') {
		updateDistributionChart();
	}
}

function resizeCharts() {
	resizeTradeChart();
	resizeOrderBookChart();
	resizeDistributionChart();
}

//------------------------------------------------------------------------------------------------------------------














//------------------------------------------------------------------------------------------------------------------
// OrderBook Chart
//------------------------------------------------------------------------------------------------------------------
function createOrderBookChart() {
	if (!orderbookChart) {
		orderbookChart = new Highcharts.Chart({
			chart: {
				type: 'area',
				zoomType: 'xy',
				renderTo: "depthdata",
				height: fullChart ? 554 : 354,
				backgroundColor: 'transparent',
				margin: [25, 0, 15, 0],
				animation: enableChartAnimations
			},
			title: {
				text: ''
			},
			legend: {
				enabled: false
			},
			exporting: {
				enabled: false
			},
			xAxis: {
				type: "linear",
				labels: {
					format: '{value:.8f}',
					y: 15,
					autoRotationLimit: 0,
					padding: 10,
					overflow: false,
					rotation: 0
				},
				crosshair: true,
				tickLength: 0,
				maxPadding: 0,
				minPadding: 0,
				allowDecimals: true,
				endOnTick: true,
			},
			yAxis: [{
				type: "linear",
				labels: {
					format: '{value:.8f}',
					align: 'right',
					x: -3,
					y: 0,
					enabled: true
				},
				offset: 0,
				lineWidth: 1,
				tickPosition: 'inside',
				opposite: true,
				showFirstLabel: false,
				showLastLabel: false,
				maxPadding: 0,
				minPadding: 0,
				endOnTick: false,
				showLastLabel: true
			}, {
				type: "linear",
				labels: {
					format: '{value:.8f}',
					align: 'left',
					x: 3,
					y: 0,
					enabled: true
				},
				offset: 0,
				linkedTo: 0,
				lineWidth: 1,
				tickPosition: 'inside',
				opposite: false,
				showFirstLabel: false,
				showLastLabel: false,
				maxPadding: 0,
				minPadding: 0,
				endOnTick: false,
				showLastLabel: true
			}],
			credits: {
				enabled: false
			},
			tooltip: {
				changeDecimals: 8,
				valueDecimals: 8,
				followPointer: false,
				formatter: function () {
					var tooltipHtml = Mustache.render(orderbookTooltipTemplate, {
						Price: this.x.toFixed(8),
						Volume: (this.y / this.x).toFixed(8),
						Depth: this.y.toFixed(8),
						Symbol: selectedTradePair.Symbol,
						BaseSymbol: selectedTradePair.BaseSymbol
					});
					return tooltipHtml;
				}
			},
			series: [{
				name: 'Buy',
				data: [],
				color: "#5cb85c",
				fillOpacity: 0.5,
				lineWidth: 1,
				marker: {
					enabled: false,
				},
				yAxis: 0
			}, {
				name: 'Sell',
				color: "#d9534f",
				fillOpacity: 0.5,
				data: [],
				lineWidth: 1,
				marker: {
					enabled: false,
				},
				yAxis: 0
			}]
		});
	}
}

$('[name="chart-orderbook-options"]').on("click", function () {
	var selection = $(this).val();
	orderBookChartPercent = selection;
	updateOrderBookChart();
})

function updateOrderBookChart() {
	createOrderBookChart();
	var buydata = [];
	$("#buyorders tbody > tr").each(function () {
		var row = $(this);
		var price = +row.find("td:nth-child(2)").text();
		var sum = +row.find("td:nth-child(5)").text();
		if (price && sum)
			buydata.push([price, sum])
	});

	var selldata = [];
	$("#sellorders tbody > tr").each(function () {
		var row = $(this);
		var price = +row.find("td:nth-child(2)").text();
		var sum = +row.find("td:nth-child(5)").text();
		if (price && sum)
			selldata.push([price, sum])
	});

	var xAxisMin = 0;
	var xAxisMax = 0;
	if (buydata.length > 0 && selldata.length > 0) {
		var middleValue = (+selldata[0][0] + +buydata[0][0]) / 2;

		if (orderBookChartPercent == 25 && buydata.length >= 4 && selldata.length >= 4) {
			buydata = buydata.splice(0, buydata.length / 4);
			selldata = selldata.splice(0, selldata.length / 4);
			xAxisMin = buydata[buydata.length - 1][0];
			xAxisMax = selldata[selldata.length - 1][0];
		}
		else if (orderBookChartPercent == 50 && buydata.length >= 2 && selldata.length >= 2) {
			buydata = buydata.splice(0, buydata.length / 2);
			selldata = selldata.splice(0, selldata.length / 2);
			xAxisMin = buydata[buydata.length - 1][0];
			xAxisMax = selldata[selldata.length - 1][0];
		}
		else if (orderBookChartPercent == 100) {
			xAxisMin = buydata[buydata.length - 1][0];
			xAxisMax = selldata[selldata.length - 1][0];
		}
		else {
			xAxisMin = Math.max(buydata[buydata.length - 1][0], middleValue * 0.1);
			xAxisMax = Math.min(selldata[selldata.length - 1][0], middleValue * 1.8);
		}
		buydata.reverse();
	};

	if (buydata.length == 0 && selldata.length == 0) {
		$(".chart-orderbook-nodata").show();
		return;
	}
	if (orderbookChart) {
		orderbookChart.showLoading();
		orderbookChart.series[0].setData(buydata, false, false, false);
		orderbookChart.series[1].setData(selldata, false, false, false);
		orderbookChart.xAxis[0].setExtremes(xAxisMin, xAxisMax, false, false, false);
		orderbookChart.reflow();
		orderbookChart.hideLoading();
		orderbookChart.update({ chart: { height: fullChart ? 554 : 354, width: $("#depthdata").width() } }, true);
	}
}

function clearOrderBookChart() {
	if (orderbookChart) {
		orderbookChart.series[0].setData([[0, 0]], false, false, false);
		orderbookChart.series[1].setData([[0, 0]], false, false, false);
		orderbookChart.update({ chart: { height: fullChart ? 554 : 354, width: $("#depthdata").width() } }, true);
	}
}

function resizeOrderBookChart() {
	if (orderbookChart) {
		orderbookChart.reflow();
		orderbookChart.update({ chart: { height: fullChart ? 554 : 354, width: $("#depthdata").width() } }, true);
	}
}

var updateOrderBookChartThrottleTimeout;
function updateOrderBookChartThrottle() {
	if (selectedChart == "orderbook") {
		clearTimeout(updateOrderBookChartThrottleTimeout);
		updateOrderBookChartThrottleTimeout = setTimeout(function () {
			updateOrderBookChart();
		}, orderBookChartThrottle);
	}
}

//------------------------------------------------------------------------------------------------------------------




//------------------------------------------------------------------------------------------------------------------
// Distribution Chart
//------------------------------------------------------------------------------------------------------------------

function createDistributionChart() {
	if (!distributionChart) {
		distributionChart = new Highcharts.Chart({
			chart: {
				type: 'column',
				renderTo: "distributiondata",
				height: fullChart ? 540 : 340,
				backgroundColor: 'transparent',
				margin: [0, 0, 0, 0],
			},
			title: {
				text: ''
			},
			credits: {
				enabled: false
			},
			exporting: {
				enabled: false
			},
			xAxis: {
				labels: {
					enabled: false,
				},
				crosshair: true,
				tickLength: 0,
				maxPadding: 0,
				minPadding: 0,
			},
			yAxis: [{
				type: "linear",
				labels: {
					format: '{value:.8f}',
					align: 'right',
					x: -3,
					y: 0
				},
				title: {
					enabled: false
				},
				offset: 0,
				lineWidth: 2,
				tickPosition: 'inside',
				opposite: true,
				showFirstLabel: false,
				showLastLabel: false,
				maxPadding: 0,
				minPadding: 0,
				lineColor: 'transparent'
			}],
			tooltip: {
				headerFormat: '<span></span>',
				pointFormatter: function () {
					return '<span  style="white-space:nowrap">' + this.y.toFixed(8) + ' ' + selectedTradePair.Symbol + '</span>';
				},
				shared: true,
				useHTML: true
			},
			plotOptions: {
				column: {
					pointPadding: 0,
					borderWidth: 0
				}
			},
			series: [{
				name: ' ',
				showInLegend: false,
				minPointLength: 10,
				data: []
			}]
		});
	}
}

$('[name="chart-distribution-options"]').on("click", function () {
	distributionChartCount = $(this).val();
	updateDistributionChart();
})

function updateDistributionChart() {
	createDistributionChart();
	if (distributionChart) {
		$(".chart-distribution-loading").show();
		getData(actionDistributionChart, { currencyId: selectedTradePair.CurrencyId, count: distributionChartCount }, function (chartData) {
			var data = chartData ? chartData.Distribution : [];
			if (data.length == 0) {
				$(".chart-distribution-nodata").show();
				return;
			}
			if (distributionChart) {
				distributionChart.showLoading();
				distributionChart.series[0].setData(data);
				distributionChart.reflow();
				distributionChart.hideLoading();
				distributionChart.update({ chart: { height: fullChart ? 540 : 340, width: $("#distributiondata").width() } }, true);
			}
			$(".chart-distribution-loading").hide();
		});
	}
}

function clearDistributionChart() {
	if (distributionChart) {
		distributionChart.series[0].setData([]);
	}
}

function resizeDistributionChart() {
	if (distributionChart) {
		distributionChart.reflow();
		distributionChart.update({ chart: { height: fullChart ? 540 : 340, width: $("#distributiondata").width() } }, true);
	}
}

//------------------------------------------------------------------------------------------------------------------

//------------------------------------------------------------------------------------------------------------------
// Trade Chart
//------------------------------------------------------------------------------------------------------------------

function createTradeChart() {
	//if (!tradechart) {
	tradechart = new Highcharts.StockChart({
		chart: {
			height: fullChart ? 554 : 354,
			backgroundColor: 'transparent',
			renderTo: 'chartdata',
			animation: enableChartAnimations,
			panning: false,
			margin: [0, 0, 15, 0],
			alignTicks: false,
			events: {
				redraw: function (e) {
				}
			},
		},
		credits: {
			enabled: false
		},
		navigator: {
			adaptToUpdatedData: false,
		},
		scrollbar: {
			liveRedraw: false
		},
		exporting: {
			enabled: false
		},

		xAxis: {
			tickPosition: 'inside',
			endOnTick: true,
			startOnTick: true,
			crosshair: {
				snap: true,
				width: 1,
				zIndex: 100
			},
			events: {
				afterSetExtremes: function (e) {
					var axisData = tradechart.yAxis[0].getExtremes();
					if (axisData.dataMax != axisData.dataMin) {
						var diff = axisData.dataMax - axisData.dataMin;
						var padding = (diff / 100.00000000) * 5;
						tradechart.yAxis[0].update({
							floor: axisData.dataMin - padding,
							ceiling: axisData.dataMax + padding,
						}, true);
						setTimeout(function () {
							toggleFibonacci(fibonacciChart);
						}, 100)
					}
				}
			}
		},
		yAxis: [
			// PRICE/CANDLESTICK AXIS
			{
				labels: {
					format: '{value:.8f}',
					align: 'right',
					x: -2
				},
				title: {
					text: '',
					enabled: false
				},
				height: fullChart ? 300 : 225,
				offset: 0,
				lineWidth: 0.5,
				allowDecimals: true,
				endOnTick: false,
				startOnTick: false,
				showLastLabel: true,
				showFirstLabel: true,
				tickPosition: 'inside',
				events: {
					afterSetExtremes: function (e) {

					}
				}
			},
			// VOLUME AXIS
			{
				labels: {
					format: '{value:.8f}',
					align: 'right',
					x: -3,
					enabled: false
				},
				title: {
					text: 'Volume',
					enabled: false
				},
				offset: 0,
				endOnTick: false,
				startOnTick: false,
				height: fullChart ? 300 : 225,
				lineWidth: 1,
				tickPosition: 'inside',
				gridLineWidth: 0,
			},
			//MACD AXIS
			{
				labels: {
					format: '{value:.8f}',
					align: 'right',
					x: -2
				},
				title: {
					text: 'MACD',
					enabled: false
				},
				top: 360,
				height: fullChart ? 100 : 0,
				offset: 0,
				maxPadding: 0,
				minPadding: 0,
				lineWidth: 1,
				gridLineWidth: 1,
				tickPosition: 'inside'
			}],

		series: [
			// TRADEPRICE CHART
			{
				name: 'StockPrice',
				type: 'line',
				color: stockPriceChartColor,
				id: 'primary',
				yAxis: 0,
				showInLegend: false,
				lineWidth: stockPriceChart ? 1 : 0,
				animation: enableChartAnimations,
				turboThreshold: 100,
				showInNavigator: true,
				dataGrouping: {
					enabled: false
				},
				marker: {
					enabled: false,
					states: {
						hover: {
							enabled: false,
						}
					}
				},
				states: {
					hover: {
						enabled: false,
					}
				},
				tooltip: {
					pointFormatter: function () {
						$('#chart-info-price').html(this.y.toFixed(8));
					}
				}
			},
			// CANDLESTICK CHART
			{
				type: 'candlestick',
				name: selectedMarket,
				yAxis: 0,
				color: candlestickChartDownColor,
				upColor: candlestickChartUpColor,
				upLineColor: candlestickLineColor,
				lineColor: candlestickLineColor,
				showInLegend: false,
				lineWidth: 0.5,
				animation: enableChartAnimations,
				turboThreshold: 100,
				showInNavigator: false,
				visible: candlestickChart,
				dataGrouping: {
					enabled: false
				},
				marker: {
					enabled: false,
					states: {
						hover: {
							enabled: false,
						}
					}
				},
				states: {
					hover: {
						enabled: false,
					}
				},
				tooltip: {
					pointFormatter: function () {
						$('#chart-info-open').html(this.open.toFixed(8));
						$('#chart-info-high').html(this.high.toFixed(8));
						$('#chart-info-low').html(this.low.toFixed(8));
						$('#chart-info-close').html(this.close.toFixed(8));
						$('#chart-info-date').html(moment.utc(this.x).local().format("D/MM hh:mm"));
					}
				}
			},
			// VOLUME CHART
			{
				type: 'column',
				color: volumeChartColor,
				name: '',
				yAxis: 1,
				zIndex: 0,
				showInLegend: false,
				//lineWidth: 4,
				animation: enableChartAnimations,
				turboThreshold: 0,
				showInNavigator: false,
				visible: volumeChart,
				dataGrouping: {
					enabled: false
				},
				marker: {
					enabled: false,
					states: {
						hover: {
							enabled: false,
						}
					}
				},
				tooltip: {
					pointFormatter: function () {
						$('#chart-info-volume').html(this.y.toFixed(8));
						$('#chart-info-basevolume').html((+this.basev || 0).toFixed(8));
					}
				}
			},
			// SMA CHART
			{
				name: 'SMA',
				linkedTo: 'primary',
				showInLegend: true,
				type: 'trendline',
				algorithm: 'SMA',
				color: smaChartColor,
				periods: smaChartValue,
				visible: smaChart,
				showInLegend: false,
				lineWidth: 0.5,
				animation: enableChartAnimations,
				turboThreshold: 100,
				showInNavigator: false,
				enableMouseTracking: false,
				marker: {
					enabled: false,
					states: {
						hover: {
							enabled: false,
						}
					}
				},
				tooltip: {
					pointFormatter: function () {
						$('#chart-info-SMA').html(this.y.toFixed(8));
					}
				}
			},
			// EMA1 CHART
			{
				name: 'EMA 1',
				linkedTo: 'primary',
				showInLegend: true,
				type: 'trendline',
				algorithm: 'EMA',
				color: ema1ChartColor,
				periods: ema1ChartValue,
				visible: ema1Chart,
				showInLegend: false,
				lineWidth: 0.5,
				turboThreshold: 100,
				animation: enableChartAnimations,
				turboThreshold: 0,
				showInNavigator: false,
				enableMouseTracking: false,
				marker: {
					enabled: false,
					states: {
						hover: {
							enabled: false,
						}
					}
				},
				tooltip: {
					pointFormatter: function () {
						$('#chart-info-EMA1').html(this.y.toFixed(8));
					}
				}
			},
			// EMA2 CHART
			{
				name: 'EMA 2',
				linkedTo: 'primary',
				showInLegend: true,
				type: 'trendline',
				algorithm: 'EMA',
				color: ema2ChartColor,
				periods: ema2ChartValue,
				visible: ema2Chart,
				showInLegend: false,
				turboThreshold: 100,
				enableMouseTracking: false,
				lineWidth: 0.5,
				animation: enableChartAnimations,
				turboThreshold: 0,
				showInNavigator: false,
				marker: {
					enabled: false,
					states: {
						hover: {
							enabled: false,
						}
					}
				},
				tooltip: {
					pointFormatter: function () {
						$('#chart-info-EMA2').html(this.y.toFixed(8));
					}
				}
			},
			// MACD CHART
			{
				name: 'MACD',
				linkedTo: 'primary',
				yAxis: 2,
				showInLegend: true,
				type: 'trendline',
				algorithm: 'MACD',
				color: macdChartColor,
				showInLegend: false,
				lineWidth: 0.5,
				turboThreshold: 100,
				animation: enableChartAnimations,
				turboThreshold: 1000,
				showInNavigator: false,
				visible: macdChart,
				marker: {
					enabled: false,
					states: {
						hover: {
							enabled: false,
						}
					}
				},
				tooltip: {
					pointFormatter: function () {
						$('#chart-info-macd').html(this.y.toFixed(8));
					}
				}
			},
			// SIGNAL LINE CHART
			{
				name: 'Signal line',
				linkedTo: 'primary',
				yAxis: 2,
				showInLegend: true,
				type: 'trendline',
				algorithm: 'signalLine',
				color: signalChartColor,
				showInLegend: false,
				lineWidth: 0.5,
				turboThreshold: 100,
				animation: enableChartAnimations,
				turboThreshold: 1000,
				showInNavigator: false,
				visible: signalChart,
				marker: {
					enabled: false,
					states: {
						hover: {
							enabled: false,
						}
					}
				},
				tooltip: {
					pointFormatter: function () {
						$('#chart-info-signal').html(this.y.toFixed(8));
					}
				}
			},
			// HISTOGRAM CHART
			{
				name: 'Histogram',
				linkedTo: 'primary',
				yAxis: 2,
				color: histogramChartUpColor,
				negativeColor: histogramChartDownColor,
				showInLegend: true,
				type: 'histogram',
				showInLegend: false,
				lineWidth: 0.5,
				turboThreshold: 100,
				animation: enableChartAnimations,
				turboThreshold: 1000,
				showInNavigator: false,
				visible: histogramChart,
				marker: {
					enabled: false,
					states: {
						hover: {
							enabled: false,
						}
					}
				},
				tooltip: {
					pointFormatter: function () {
						$('#chart-info-histogram').html(this.y.toFixed(8));
					}
				}
			}
		],
		tooltip: {
			animation: enableChartAnimations,
			style: { "display": "none" }
		},
		rangeSelector: {
			inputEnabled: false,
			allButtonsEnabled: false,
			buttons: [{
				type: 'day',
				count: 1,
				text: '',//'Day',
				dataGrouping: {
					forced: true,
					enabled: true
				}
			},
			{
				type: 'day',
				count: 2,
				text: '',//'2 day',
				dataGrouping: {
					forced: true,
					enabled: true
				}
			},
			{
				type: 'week',
				count: 1,
				text: '',//'1 Week',
				dataGrouping: {
					forced: true,
					enabled: true
				}
			},
			{
				type: 'week',
				count: 2,
				text: '',//'Week',
				dataGrouping: {
					forced: true,
					enabled: true
				}
			},
			{
				type: 'month',
				text: '',//'Month',
				count: 1,
				dataGrouping: {
					forced: true,
					enabled: true,
					units: [['hour', [1]]]
				}
			},
			{
				type: 'month',
				text: '',//'3 Month',
				count: 3,
				dataGrouping: {
					forced: true,
					enabled: true,
					units: [['hour', [1]]]
				}
			},
			{
				type: 'month',
				text: '',//'3 Month',
				count: 6,
				dataGrouping: {
					forced: true,
					enabled: true,
					units: [['hour', [1]]]
				}
			},
			{
				type: 'all',
				text: '',//'All',
				dataGrouping: {
					forced: true,
					enabled: true,
					units: [['hour', [1]]]
				}
			}],
			buttonTheme: {
				width: 0,
				height: 0,
			},
			labelStyle: {
				fontSize: '1px'
			},
			selected: 0,
			inputStyle: {
				"background": "red"
			}
		},
	});
	//}
}

function updateTradeChart() {
	createTradeChart();
	updateSeriesRange(selectedSeriesRange);
}

function clearTradeChart() {
	if (tradechart) {
		tradechart.series[0].setData([[0, 0, 0, 0, 0, 0]], false, false, false) // PriceChart
		tradechart.series[1].setData([[0, 0, 0, 0, 0, 0]], false, false, false) // CandleStick
		tradechart.series[2].setData([[0, 0]], false, false, false) // Volume
		tradechart.redraw();
	}
}

function resizeTradeChart() {
	if (tradechart) {
		tradechart.reflow();
		setBorders();
	}
}

function updateChart(chartData) {
	$(".chart-loading").show();
	var cdata = chartData ? chartData.Candle : [[0, 0, 0, 0, 0, 0]];
	var vdata = chartData ? chartData.Volume : [[0, 0]];
	if (tradechart) {
		$(".chart-nodata").hide();
		tradechart.series[0].setData(cdata, false, false, false) // PriceChart
		tradechart.series[1].setData(cdata, false, false, false) // CandleStick
		tradechart.series[2].setData(vdata, false, false, false) // Volume
		tradechart.redraw(false);
		tradechart.rangeSelector.clickButton(selectedSeriesRange, true, false);
		setBorders()
		$(".chart-loading").hide();
		if (cdata.length == 1 && cdata[0][0] == 0) {
			$(".chart-nodata").show();
		}
	}
}

function updateChartData(dataRange, dataGrouping) {
	selectedSeriesRange = dataRange;
	selectedCandleGrouping = getCandleGrouping(dataRange, dataGrouping);
	$(".chart-nodata").hide();
	$(".chart-loading").show();

	if (getTradePairChartRequest && getTradePairChartRequest.readyState != 4) {
		getTradePairChartRequest.abort();
	}
	getTradePairChartRequest = getData(actionTradeChart, { tradePairId: selectedTradePair.TradePairId, dataRange: selectedSeriesRange, dataGroup: selectedCandleGrouping }, function (data) {
		updateChart(data);
	});
}

$("#chart-options").on("click", ".chart-options-dropdown-trade .chart-extras", function () {
	var _this = $(this);
	var parent = _this.closest(".chart-extras-container");
	var series = parent.data("series");
	var button = parent.find(".chart-extras-update");
	var input = parent.find(".chart-extras-value");
	var value = input.val() > 0 ? input.val() : 1;
	var isChecked = _this.is(":checked");
	if (isChecked) {
		input.removeAttr("disabled");
		button.removeAttr("disabled");
		toggleSeries(series, value, true);
		return;
	}
	input.attr("disabled", "disabled");
	button.attr("disabled", "disabled");
	toggleSeries(series, value, false);
});

$("#chart-options").on("click", ".chart-options-dropdown-trade  .chart-extras-update", function () {
	var _this = $(this);
	var parent = _this.closest(".chart-extras-container");
	var series = parent.data("series");
	var input = parent.find(".chart-extras-value");
	var value = input.val() > 0 ? input.val() : 1;
	toggleSeries(series, value, true);
});

$("#chart-options").on("click", ".chart-options-save", function () {
	saveChartSettings();
});

function toggleFullChart() {
	if (macdChart || signalChart || histogramChart) {
		fullChart = true;
		if (tradechart.yAxis[0].height != 300) {
			tradechart.yAxis[0].update({ height: 300 }, false);
			tradechart.yAxis[1].update({ height: 300 }, false);
			tradechart.yAxis[2].update({ height: 100 }, false);
			tradechart.update({ chart: { height: 554 } }, false);


			$(".chart-container").height(565)
			tradechart.redraw(true);
			toggleFibonacci(fibonacciChart);
			setBorders();
		}
		return;
	}
	fullChart = false;
	if (tradechart.yAxis[0].height != 225) {
		tradechart.yAxis[0].update({ height: 225 }, false);
		tradechart.yAxis[1].update({ height: 225 }, false);
		tradechart.yAxis[2].update({ height: 0 }, false);
		tradechart.update({ chart: { height: 354 } }, false);


		$(".chart-container").height(365)
		tradechart.redraw(true);
		toggleFibonacci(fibonacciChart);
		setBorders();
	}
}

$(".chart-range-group").on("click", ".btn-default", function () {
	var range = $(this).data("range");
	$(".chart-range-group > .btn-default").removeClass("active");
	$(this).addClass("active");
	updateSeriesRange(range);
});

$(".chart-candles-group").on("click", ".btn-default", function () {
	var candles = $(this).data("candles");
	$(".chart-candles-group > .btn-default").removeClass("active");
	$(this).addClass("active");
	updateChartData(selectedSeriesRange, candles);
});

function updateSeriesRange(range) {
	if (tradechart) {
		$(".chart-candles-group > .btn-default").removeClass("active").attr("disabled", "disabled");
		if (range == 0) {
			$(".chart-candles-btn15, .chart-candles-btn30, .chart-candles-btn60, .chart-candles-btn120").removeAttr("disabled");
		}
		else if (range == 1) {
			$(".chart-candles-btn15, .chart-candles-btn30, .chart-candles-btn60, .chart-candles-btn120").removeAttr("disabled");
		}
		else if (range == 2) {
			$(".chart-candles-btn60, .chart-candles-btn120, .chart-candles-btn240, .chart-candles-btn720").removeAttr("disabled");
		}
		else if (range == 3) {
			$(".chart-candles-btn120, .chart-candles-btn240, .chart-candles-btn720").removeAttr("disabled");
		}
		else if (range == 4) {
			$(".chart-candles-btn240, .chart-candles-btn720, .chart-candles-btn1440").removeAttr("disabled");
		}
		else if (range == 5) {
			$(".chart-candles-btn240, .chart-candles-btn720, .chart-candles-btn1440, .chart-candles-btn10080").removeAttr("disabled");
		}
		else if (range == 6) {
			$(".chart-candles-btn720, .chart-candles-btn1440, .chart-candles-btn10080").removeAttr("disabled");
		}
		else if (range == 7) {
			$(".chart-candles-btn1440, .chart-candles-btn10080").removeAttr("disabled");
		}
		$(".chart-range-group > .btn-default").removeClass("active");
		$(".chart-range-btn" + range).addClass("active");
		updateChartData(range, selectedCandleGrouping);
		$(".chart-candles-btn" + selectedCandleGrouping).addClass("active");
	}
}

function getCandleGrouping(dataRange, dataGrouping) {
	if (dataRange == 0 && dataGrouping > 120) {
		return 30;
	}
	if (dataRange == 1 && dataGrouping > 120) {
		return 60;
	}
	if (dataRange == 2 && (dataGrouping > 720 || dataGrouping < 60)) {
		return 120;
	}
	if (dataRange == 3 && (dataGrouping > 720 || dataGrouping < 120)) {
		return 120;
	}
	if (dataRange == 4 && (dataGrouping > 1440 || dataGrouping < 240)) {
		return 240;
	}
	if (dataRange == 5 && (dataGrouping < 240)) {
		return 720;
	}
	if (dataRange == 6 && (dataGrouping < 720)) {
		return 720;
	}
	if (dataRange == 7 && (dataGrouping < 1440)) {
		return 1440;
	}
	return dataGrouping;
}

function toggleSeries(id, value, show) {
	if (id == 0) {
		toggleStockPrice(show);
	}
	if (id == 1) {
		toggleCandleStick(show);
	}
	if (id == 2) {
		toggleVolume(show);
	}
	if (id == 3) {
		smaChartValue = value;
		toggleSMA(show, value);
	}
	if (id == 4) {
		ema1ChartValue = value;
		toggleEMA1(show, value);
	}
	if (id == 5) {
		ema2ChartValue = value;
		toggleEMA2(show, value);
	}
	if (id == 6) {
		toggleMACD(show);
	}
	if (id == 7) {
		toggleSignal(show);
	}
	if (id == 8) {
		toggleHistogram(show);
	}
	if (id == 9) {
		toggleFibonacci(show);
	}
}

function toggleCandleStick(show) {
	candlestickChart = show;
	if (show) {
		tradechart.series[1].show();
		$(".chart-candlestick-item").show();
		toggleFibonacci(fibonacciChart);
		return;
	}
	tradechart.series[1].hide();
	$(".chart-candlestick-item").hide();
	toggleFibonacci(fibonacciChart);
}

function toggleStockPrice(show) {
	stockPriceChart = show;
	if (show) {
		tradechart.series[0].update({ lineWidth: 1 });
		$(".chart-stockprice-item").show();
		return;
	}
	tradechart.series[0].update({ lineWidth: 0 });
	$(".chart-stockprice-item").hide();
}

function toggleVolume(show) {
	volumeChart = show;
	if (show) {
		tradechart.series[2].show();
		$(".chart-volume-item").show();
		return;
	}
	tradechart.series[2].hide();
	$(".chart-volume-item").hide();
}

function toggleSMA(show, period) {
	smaChart = show;
	smaChartValue = period;
	tradechart.series[3].update({ periods: smaChartValue });
	if (show) {
		tradechart.series[3].show();
		$(".chart-sma-item").show();
		return;
	}
	tradechart.series[3].hide();
	$(".chart-sma-item").hide();
}

function toggleEMA1(show, period) {
	ema1Chart = show;
	ema1ChartValue = period;
	tradechart.series[4].update({ periods: ema1ChartValue });
	if (show) {
		tradechart.series[4].show();
		$(".chart-ema1-item").show();
		return;
	}
	tradechart.series[4].hide();
	$(".chart-ema1-item").hide();
}

function toggleEMA2(show, period) {
	ema2Chart = show;
	ema2ChartValue = period;
	tradechart.series[5].update({ periods: ema2ChartValue });
	if (show) {
		tradechart.series[5].show();
		$(".chart-ema2-item").show();
		return;
	}
	tradechart.series[5].hide();
	$(".chart-ema2-item").hide();
}

function toggleMACD(show) {
	if (show) {
		macdChart = true;
		tradechart.series[6].show();
		toggleFullChart();
		$(".chart-macd-item").show();
		return;
	}
	macdChart = false;
	tradechart.series[6].hide();
	toggleFullChart();
	$(".chart-macd-item").hide();
}

function toggleSignal(show) {
	if (show) {
		signalChart = true;
		tradechart.series[7].show();
		toggleFullChart();
		$(".chart-signal-item").show();
		return;
	}
	signalChart = false;
	tradechart.series[7].hide();
	toggleFullChart();
	$(".chart-signal-item").hide();
}

function toggleHistogram(show) {
	if (show) {
		histogramChart = true;
		tradechart.series[8].show();
		toggleFullChart();
		$(".chart-histogram-item").show();
		return;
	}
	histogramChart = false;
	tradechart.series[8].hide();
	toggleFullChart();
	$(".chart-histogram-item").hide();
}

function toggleFibonacci(show) {
	fibonacciChart = show;
	if (show && (candlestickChart || stockPriceChart)) {
		var low;
		var high;
		var axisData = tradechart.yAxis[0].getExtremes();
		var hasHighLow = axisData.dataMax != axisData.dataMin;

		if (hasHighLow) {
			var series = candlestickChart ? 1 : 0;
			var count = tradechart.series[series].points.length;

			if (candlestickChart) {
				for (var i = count; i > 0; i--) {
					var point = tradechart.series[1].points[i];
					if (point == null) {
						continue;
					}
					if ((!low || point.x < low) && (point.low == axisData.dataMin)) {
						low = point.x;
					}
					if (!high && (point.high == axisData.dataMax)) {
						high = point.x;
					}
				}
			}

			if (stockPriceChart && !candlestickChart) {
				for (var i = count; i > 0; i--) {
					var point = tradechart.series[0].points[i];
					if (point == null) {
						continue;
					}
					if ((!low || point.x < low) && (point.y == axisData.dataMin)) {
						low = point.x;
					}
					if (!high && (point.y == axisData.dataMax)) {
						high = point.x;
					}
				}
			}

			var total = axisData.dataMax - axisData.dataMin;
			var percent = total / 100.00000000;
			var first = tradechart.yAxis[0].toPixels(axisData.dataMin);
			var second = tradechart.yAxis[0].toPixels(axisData.dataMin + (percent * 23.6));
			var third = tradechart.yAxis[0].toPixels(axisData.dataMin + (percent * 38.2));
			var fourth = tradechart.yAxis[0].toPixels(axisData.dataMin + (percent * 50));
			var fifth = tradechart.yAxis[0].toPixels(axisData.dataMin + (percent * 61.8));
			var sixth = tradechart.yAxis[0].toPixels(axisData.dataMax);
			var path1 = ['M', tradechart.plotLeft, first, 'L', tradechart.plotLeft + tradechart.plotWidth, first];
			var path2 = ['M', tradechart.plotLeft, second, 'L', tradechart.plotLeft + tradechart.plotWidth, second];
			var path3 = ['M', tradechart.plotLeft, third, 'L', tradechart.plotLeft + tradechart.plotWidth, third];
			var path4 = ['M', tradechart.plotLeft, fourth, 'L', tradechart.plotLeft + tradechart.plotWidth, fourth];
			var path5 = ['M', tradechart.plotLeft, fifth, 'L', tradechart.plotLeft + tradechart.plotWidth, fifth];
			var path6 = ['M', tradechart.plotLeft, sixth, 'L', tradechart.plotLeft + tradechart.plotWidth, sixth];

			var diagonal;
			if (low && high) {
				var lowPoint = tradechart.xAxis[0].toPixels(low)
				var highPoint = tradechart.xAxis[0].toPixels(high)
				diagonal = ['M', highPoint, sixth, 'L', lowPoint, first];
			}

			var options = {
				'stroke-width': 0.5,
				stroke: fibonacciChartColor,
				zIndex: 100
			}

			var labelOptions = {
				zIndex: 100,
				css: {
					fontSize: "11px",
					color: fibonacciChartColor
				}
			}

			if (tradechart.fib1) {
				tradechart.fib1.attr({ d: path1 });
				tradechart.fib1Label.attr({ y: first - 2, text: "0%" });
			} else {
				tradechart.fib1 = tradechart.renderer.path(path1).attr(options).add();
				tradechart.fib1Label = tradechart.renderer.text("0%", 0, first - 2).css(labelOptions.css).attr(labelOptions).add();
			}

			if (tradechart.fib2) {
				tradechart.fib2.attr({ d: path2 });
				tradechart.fib2Label.attr({ y: second - 2, text: "23.6%" });
			} else {
				tradechart.fib2 = tradechart.renderer.path(path2).attr(options).add();
				tradechart.fib2Label = tradechart.renderer.text("23.6%", 0, second - 2).css(labelOptions.css).attr(labelOptions).add();
			}

			if (tradechart.fib3) {
				tradechart.fib3.attr({ d: path3 });
				tradechart.fib3Label.attr({ y: third - 2, text: "38.2%" });
			} else {
				tradechart.fib3 = tradechart.renderer.path(path3).attr(options).add();
				tradechart.fib3Label = tradechart.renderer.text("38.2%", 0, third - 2).css(labelOptions.css).attr(labelOptions).add();
			}

			if (tradechart.fib4) {
				tradechart.fib4.attr({ d: path4 });
				tradechart.fib4Label.attr({ y: fourth - 2, text: "50%" });
			} else {
				tradechart.fib4 = tradechart.renderer.path(path4).attr(options).add();
				tradechart.fib4Label = tradechart.renderer.text("50%", 0, fourth - 2).css(labelOptions.css).attr(labelOptions).add();
			}

			if (tradechart.fib5) {
				tradechart.fib5.attr({ d: path5 });
				tradechart.fib5Label.attr({ y: fifth - 2, text: "61.8%" });
			} else {
				tradechart.fib5 = tradechart.renderer.path(path5).attr(options).add();
				tradechart.fib5Label = tradechart.renderer.text("61.8%", 0, fifth - 2).css(labelOptions.css).attr(labelOptions).add();
			}

			if (tradechart.fib6) {
				tradechart.fib6.attr({ d: path6 });
				tradechart.fib6Label.attr({ y: sixth - 2, text: "100%" });
			} else {
				tradechart.fib6 = tradechart.renderer.path(path6).attr(options).add();
				tradechart.fib6Label = tradechart.renderer.text("100%", 0, sixth - 2).css(labelOptions.css).attr(labelOptions).add();
			}

			if (tradechart.fibd) {
				tradechart.fibd.attr({ d: diagonal });
			} else {
				tradechart.fibd = tradechart.renderer.path(diagonal).attr(options).add();
			}
		}
		return;
	}
	if (tradechart.fibd) {
		tradechart.fibd.attr({ d: [] });
		tradechart.fib1.attr({ d: [] });
		tradechart.fib2.attr({ d: [] });
		tradechart.fib3.attr({ d: [] });
		tradechart.fib4.attr({ d: [] });
		tradechart.fib5.attr({ d: [] });
		tradechart.fib6.attr({ d: [] });
		tradechart.fib1Label.attr({ y: 0, text: "" });
		tradechart.fib2Label.attr({ y: 0, text: "" });
		tradechart.fib3Label.attr({ y: 0, text: "" });
		tradechart.fib4Label.attr({ y: 0, text: "" });
		tradechart.fib5Label.attr({ y: 0, text: "" });
		tradechart.fib6Label.attr({ y: 0, text: "" });
	}
}

function setBorders() {
	var options = {
		'stroke-width': 0.2,
		stroke: chartBorderColor,
		zIndex: 200
	}
	var topline = ['M', tradechart.plotLeft, 32, 'L', tradechart.plotLeft + tradechart.plotWidth, 32];
	if (tradechart.topBorder) {
		tradechart.topBorder.attr({ d: topline });
	} else {
		tradechart.topBorder = tradechart.renderer.path(topline).attr(options).add();
	}
	if (fullChart) {
		var bottomline = ['M', tradechart.plotLeft, 336, 'L', tradechart.plotLeft + tradechart.plotWidth, 336];
		var bottomline2 = ['M', tradechart.plotLeft, 360, 'L', tradechart.plotLeft + tradechart.plotWidth, 360];
		if (tradechart.bottomBorder) {
			tradechart.bottomBorder.attr({ d: bottomline });
			tradechart.bottomBorder2.attr({ d: bottomline2 });
		} else {
			tradechart.bottomBorder = tradechart.renderer.path(bottomline).attr(options).add();
			tradechart.bottomBorder2 = tradechart.renderer.path(bottomline2).attr(options).add();
		}
	}
	else {
		if (tradechart.bottomBorder) {
			tradechart.bottomBorder.attr({ d: [] });
			tradechart.bottomBorder2.attr({ d: [] });
		}
	}
}

function drawHorizontalCrosshair(event) {
	var x = event.pageX,
		y = event.offsetY;
	path = ['M', tradechart.plotLeft, y, 'L', tradechart.plotLeft + tradechart.plotWidth, y];

	var value;
	var candleY = y - tradechart.plotTop;
	var candleH = tradechart.yAxis[0].len;
	var macdH = tradechart.yAxis[2].len;

	if (candleY >= 0 && candleY <= candleH) {
		value = tradechart.yAxis[0].toValue(y).toFixed(8);
	}
	else if (candleY >= 325 && candleY <= (325 + macdH)) {
		value = tradechart.yAxis[2].toValue(y).toFixed(8);
	}
	else {
		if (tradechart.crossLines && tradechart.crossLabel) {
			tradechart.crossLabel.attr({ y: 0, text: '' });
			tradechart.crossLines.attr({ d: [] });
		}
		return;
	}

	if (!value) {
		return;
	}

	if (tradechart.crossLines) {
		tradechart.crossLines.attr({ d: path });
	} else {
		tradechart.crossLines = tradechart.renderer.path(path).attr({
			'stroke-width': 0.2,
			stroke: chartCrossHairColor,
			zIndex: 100
		}).add();
	}

	if (tradechart.crossLabel) {
		tradechart.crossLabel.attr({ x: (tradechart.plotWidth - 2), y: y - 2, text: value });
	} else {
		tradechart.crossLabel = tradechart.renderer.text(value, tradechart.plotWidth - 2, y - 2).css({
			fontSize: '11px',
			color: chartTextColor
		}).attr({
			zIndex: 100,
			align: 'right'
		}).add();
	}
}

function saveChartSettings() {
	var settings = '';
	settings += volumeChart ? "1," : "0,";
	settings += stockPriceChart ? "1," : "0,";
	settings += candlestickChart ? "1," : "0,";
	settings += macdChart ? "1," : "0,";
	settings += signalChart ? "1," : "0,";
	settings += histogramChart ? "1," : "0,";
	settings += fibonacciChart ? "1," : "0,";
	settings += smaChart ? ("1:" + smaChartValue + ",") : ("0:" + smaChartValue + ",");
	settings += ema1Chart ? ("1:" + ema1ChartValue + ",") : ("0:" + ema1ChartValue + ",");
	settings += ema2Chart ? ("1:" + ema2ChartValue + ",") : ("0:" + ema2ChartValue + ",");
	settings += (distributionChartCount + ",");
	settings += (orderBookChartPercent);
	postJson(actionUpdateChartSettings, { settings: settings }, function (data) {
		notify(data.Success ? Resources.Exchange.InfoSettingsSavedMessage : Resources.Exchange.InfoSettingsFailedMessage, data.Message);
	});
}

$("#chartdata").mousemove(drawHorizontalCrosshair);
$(".chart-container").height(fullChart ? 565 : 365);
$('#chart-extras-candlestick')[0].checked = candlestickChart;
$('#chart-extras-candlestick').parent().find("label > .fa-circle").css({ color: candlestickChartUpColor })
$('#chart-extras-stockprice')[0].checked = stockPriceChart;
$('#chart-extras-stockprice').parent().find("label > .fa-circle").css({ color: stockPriceChartColor })
$('#chart-extras-volume')[0].checked = volumeChart;
$('#chart-extras-volume').parent().find("label > .fa-circle").css({ color: volumeChartColor })
$('#chart-extras-macd')[0].checked = macdChart;
$('#chart-extras-macd').parent().find("label > .fa-circle").css({ color: macdChartColor })
$('#chart-extras-signal')[0].checked = signalChart;
$('#chart-extras-signal').parent().find("label > .fa-circle").css({ color: signalChartColor })
$('#chart-extras-histogram')[0].checked = histogramChart;
$('#chart-extras-histogram').parent().find("label > .fa-circle").css({ color: histogramChartUpColor })
$('#chart-extras-fibonacci')[0].checked = fibonacciChart;
$('#chart-extras-fibonacci').parent().find("label > .fa-circle").css({ color: fibonacciChartColor })
$('#chart-extras-sma')[0].checked = smaChart;
$('#chart-extras-sma-value').val(smaChartValue);
$("#chart-extras-sma-value")[0].disabled = !smaChart;
$('#chart-extras-sma').parent().find("label > .fa-circle").css({ color: smaChartColor })
$('#chart-extras-ema1')[0].checked = ema1Chart;
$('#chart-extras-ema1-value').val(ema1ChartValue);
$("#chart-extras-ema1-value")[0].disabled = !ema1Chart;
$('#chart-extras-ema1').parent().find("label > .fa-circle").css({ color: ema1ChartColor })
$('#chart-extras-ema2')[0].checked = ema2Chart;
$('#chart-extras-ema2-value').val(ema2ChartValue);
$("#chart-extras-ema2-value")[0].disabled = !ema2Chart;
$('#chart-extras-ema2').parent().find("label > .fa-circle").css({ color: ema2ChartColor })
$('.chart-options-dropdown-orderbook [value="' + orderBookChartPercent + '"]')[0].checked = true;
$('.chart-options-dropdown-distribution [value="' + distributionChartCount + '"]')[0].checked = true;
if (candlestickChart) { $(".chart-candlestick-item").show(); }
if (volumeChart) { $(".chart-volume-item").show(); }
if (stockPriceChart) { $(".chart-stockprice-item").show(); }
if (histogramChart) { $(".chart-histogram-item").show(); }
if (fibonacciChart) { $(".chart-fibonacci-item").show(); }
if (macdChart) { $(".chart-macd-item").show(); }
if (signalChart) { $(".chart-signal-item").show(); }
if (smaChart) { $(".chart-sma-item").show(); }
if (ema1Chart) { $(".chart-ema1-item").show(); }
if (ema2Chart) { $(".chart-ema2-item").show(); }

//------------------------------------------------------------------------------------------------------------------








