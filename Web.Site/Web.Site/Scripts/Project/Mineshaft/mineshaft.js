var selectedChart = 'hashrate';
var contentWrapper = $('#wrapper');
var selectedAlgo = contentWrapper.data('summaryalgo');
var currentPoolId = contentWrapper.data('poolid');
var username = $('#wrapper').data('username');
var miningname = $('#wrapper').data('miningname');
$("#baseAlgos", "#pools-menu").change(function () {
	var items;
	var selection = $(this).val();
	selectedAlgo = selection;
	updateAlgoData(selectedAlgo);
	if (selection) {
		$(".mineshaftalgo").removeClass("bold-text");
		$('.mineshaft-btn', "#mineshaftinfo-btn-list").hide();
		items = $('.mineshaft-btn-base-' + selectedAlgo, "#mineshaftinfo-btn-list");

	} else {
		$(".mineshaftalgo").addClass("bold-text");
		items = $(".mineshaft-btn", "#mineshaftinfo-btn-list");
	}

	var _this = $("#pools-search", "#pools-menu");
	var searchText = ($(_this).val() || '').toLowerCase();
	if (searchText && searchText !== '') {
		$.each(items, function () {
			var item = $(this);
			if (item.find('.search-data').text().toLowerCase().indexOf(searchText) == -1)
				item.hide();
			else
				item.show();
		});
	} else {
		items.show();
	}
});

$('.mineshaft-btn').on('click', function () {
	var pool = $(this).data('pool');
	var poolId = $(this).data('poolid');
	var basemarket = $(this).data('basealgo');
	if (pool) {
		selectedAlgo = basemarket;
		getPoolInfo(poolId);
		History.pushState({}, "Cryptopia - Mineshaft", "?pool=" + pool + "&Algo=" + selectedAlgo);
	}
});

var searchDelayTimer;
$("#pools-search", "#pools-menu").on('change keyup paste search', function () {
	var _this = $(this);
	clearTimeout(searchDelayTimer);
	searchDelayTimer = setTimeout(function () {
		var searchValue = $(_this).val();
		if (!searchValue || searchValue == "") {
			$('#baseAlgos', "#pools-menu").val(selectedAlgo);
			$(".mineshaftalgo").removeClass("bold-text");
			$(".mineshaft-btn", "#mineshaftinfo-btn-list").hide();
			$('.mineshaft-btn-base-' + selectedAlgo, "#mineshaftinfo-btn-list").show();
		} else {
			$('#baseAlgos', "#pools-menu").val(null);
			$(".mineshaftalgo").addClass("bold-text");
			var items = $('.mineshaft-btn', "#mineshaftinfo-btn-list");
			items.hide();
			var searchBox = $("#pools-search");
			var searchText = $(searchBox).val().toLowerCase();
			$.each(items, function () {
				var item = $(this);
				if (item.find('.search-data').text().toLowerCase().indexOf(searchText) == -1)
					item.hide();
				else
					item.show();
			});
		}
	}, 500);
});

if (currentPoolId) {
	$("#baseAlgos").val(selectedAlgo).trigger('change');
	getPoolInfo(currentPoolId, true);
} else {
	clearTarget();
	var baseMarket = $('#mainContentResult').data("basealgo")
	getPartial('#mainContentResult', '/Mineshaft/MineshaftSummary?algoType=' + baseMarket, false, function (data) {
		//positionChat();
	});
}



function getPoolInfo(poolId, scroll) {
	clearTarget();
	getPartial('#mainContentResult', '/Mineshaft/GetMineshaftInfo?id=' + poolId, false, function (data) {
		setItem(poolId);
		if (scroll) {
			scrollToItem(poolId);
		}
		//positionChat();
	});
}

function clearTarget() {
	var target = $("#mainContentResult");
	if (target) {
		target.off();
		target.unbind();
		target.empty();
	}
}

function setItem(poolId) {
	$('.mineshaft-btn').removeClass('active');
	$('#mineshaft-btn-' + poolId).addClass('active');
}

function scrollToItem(poolId) {
	var btn = $('#mineshaft-btn-' + poolId);
	if (btn) {
		var scrollX = 0;
		if (btn.position()) {
			scrollX = btn.position().top
		}
		var itemSize = btn.height() * 3;
		var scrollH = $('.stackmenu-body').height();
		if (scrollX > (scrollH - itemSize)) {
			$('.stackmenu-body').animate({
				scrollTop: scrollX - itemSize
			});
		}
	}
}





function updateAlgoData(algoType) {
	$(".algoData-content").hide();
	$(".algoData-btn").removeClass("active");
	$("#algoData-content-" + (algoType || "")).show();
	$("#algoData-btn-" + (algoType || "")).addClass("active");
	if ($.fn.dataTable.isDataTable('#algoData-' + (algoType || ""))) {
		$('#algoData-' + (algoType || "")).DataTable().ajax.reload();
	}
	else {
		$('#algoData-' + (algoType || "")).dataTable({
			"order": [[4, "desc"]],
			"lengthChange": false,
			"processing": false,
			"bServerSide": false,
			"searching": false,
			"paging": false,
			"scrollCollapse": true,
			"scrollY": "100%",
			"sAjaxSource": "/Mineshaft/GetAlgoSummary",
			"fnServerParams": function (aoData) {
				aoData.push({ "name": "algoType", "value": algoType });
			},
			"sServerMethod": "POST",
			"info": false,
			"language": {
				"emptyTable": ""
			},
			"columnDefs": [
			{ "targets": [0, 1, 10, 11], "visible": false },
			{
				"targets": [2],
				"render": function (data, type, full, meta) {
					return '<div style="display:inline-block"><div class="sprite-small small/' + data + '-small.png"></div><a href="/Mineshaft?pool=' + data + '&Algo=' + full[3] + '"> ' + full[1] + ' (' + data + ')</a></div>';
				}
			},
				{
					"targets": [4],
					"render": function (data, type, full, meta) {
						return '<span class="minercount-pool-' + full[0] + '">' + data + '</span>';
					}
				},
			{
				"targets": [6],
				"iDataSort": 10,
				"render": function (data, type, full, meta) {
					return '<span class="hashrate-pool-' + full[0] + '">' + hashrateLabel(data) + '</span>';
				}
			},
			{
				"targets": [7],
				"iDataSort": 11,
				"render": function (data, type, full, meta) {
					return '<span class="hashrate-network-' + full[0] + '">' + hashrateLabel(data) + '</span>';
				}
			},
			{
				"targets": [8],
				"render": function (data, type, full, meta) {
					return (+data).toFixed(8);
				}
			}],
			"fnRowCallback": function (nRow, aData) {
				$(nRow).addClass("algoData-pool-" + aData[0]);
			},
			"fnFooterCallback": function (nRow, aaData, iStart, iEnd, aiDisplay) {
				var total = 0;
				for (var i = 0; i < aaData.length; i++) {
					total += +aaData[i][6];
				}
				$("#summary-total-" + (algoType || "")).html(hashrateLabel(total));
			},
			"fnDrawCallback": function () {
				var price = 0;
				var poolRow;
				$('#algoData-' + (algoType || "") + " > tbody  > tr").each(function () {
					var rowPrice = +$(this).find("td:nth-child(7)").text();
					if (rowPrice > price) {
						price = rowPrice;
						poolRow = $(this);
					}
				});

				if (poolRow) {
					$(poolRow).addClass("info");
				}
			}
		});
	}
}


var mineshaftHub = $.connection.mineshaftHub;
mineshaftHub.client.SendDataNotification = function (notification) {
	if (typeof mineshaftDataUpdate === 'function') {
		mineshaftDataUpdate(notification);
	}

	if (notification.Type == 0) {
		var button = $("#mineshaft-btn-" + notification.PoolId);
		button.find(".mineshafthashrate-sm, .mineshafthashrate").text(hashrateLabel(notification.Hashrate));
		$(".hashrate-pool-" + notification.PoolId).text(hashrateLabel(notification.Hashrate));
		$(".hashrate-network-" + notification.PoolId).text(hashrateLabel(notification.NetworkHashrate));
		$(".minercount-pool-" + notification.PoolId).text(notification.UserCount);
	}
}
