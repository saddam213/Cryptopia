var currentCoin;

var messageTemplate = $("#currencyTemplate").html();


var content = $(".coininfo-content");
var summaryAction = content.data("action");
var updateAction = content.data("update");
var currentView = content.data("view");
var selectedCoin = content.data("coin");
var peerInfoAction = content.data("peerinfo");

function showInformation(currencyId) {
	postJson(summaryAction, { currencyId: currencyId }, function (data) {
		$("#currencyTarget").html(Mustache.render(messageTemplate,
		{
			currencyId: currencyId,
			symbol: data.Symbol,
			name: data.Name,
			starRating: getStarRating(data.StarRating),
			ratingSummary: data.StarRating >= 0 ? String.format(Resources.CoinInfo.InfoRatingText, data.StarRating) : Resources.CoinInfo.InfoRatingUnratedText,
			maxRating: data.MaxRating,
            totalRating: data.TotalRating,
            statusMessage: data.StatusMessage,

            //// Useful flags
            hasWarningBackground: data.ListingStatus === "Active" && data.Status !== "OK",
            hasDangerBackground: data.ListingStatus === "Delisting",

			//// Information
			algo: data.AlgoType,
			network: data.NetworkType,
			block: data.CurrentBlock,
			blockTime: data.BlockTime,
			blockReward: data.BlockReward.toFixed(8),
			totalCoin: data.TotalCoin.toFixed(8),
			posRate: data.PosRate,
			minStakeAge: data.MinStakeAge || Resources.General.NotAwailable,
			maxStakeAge: data.MaxStakeAge || Resources.General.NotAwailable,
			diffRetarget: data.DiffRetarget,

			//// Links
			website: data.Website,
			source: data.Source,
			blockExplorer: data.BlockExplorer,
			launchForum: data.LaunchForum,
			cryptopiaForum: data.CryptopiaForum,

			//// Rating
			totalPremine: data.TotalPremine,
			walletWindows: data.WalletWindows ? Resources.General.Yes : Resources.General.No,
			walletLinux: data.WalletLinux ? Resources.General.Yes : Resources.General.No,
			walletMac: data.WalletMac ? Resources.General.Yes : Resources.General.No,
			walletMobile: data.WalletMobile ? Resources.General.Yes : Resources.General.No,
			walletWeb: data.WalletWeb ? Resources.General.Yes : Resources.General.No,
			ratingBlockEx: data.BlockExplorer ? Resources.General.Yes : Resources.General.No,
			ratingForum: data.CryptopiaForum ? Resources.General.Yes : Resources.General.No,
			ratingWebsite: data.Website ? Resources.General.Yes : Resources.General.No,

			//// Settings
			poolFee: data.PoolFee.toFixed(2),
			tradeFee: data.TradeFee.toFixed(2),
			withdrawFee: data.WithdrawFee.toFixed(8),
			withdrawMin: data.WithdrawMin.toFixed(8),
			withdrawMax: data.WithdrawMax.toFixed(8),
			minConfirmations: data.MinConfirmations,
			tippingExpires: toLocalDate(data.TippingExpires),
			tipMin: data.TipMin.toFixed(8),
			//featuredExpires: toLocalDate(data.FeaturedExpires),
			rewardExpires: toLocalDate(data.RewardsExpires),
		}));


		$(".btn-details-edit").off().on("click", function () {
			var currencyId = $(this).data("id");
			if (currencyId) {
				openModalGet(updateAction, { id: currencyId }, function (data) {
					if (data.Success) {
						showInformation(currencyId);
						datatable.ajax.reload();
					}
				});
			}
		});

		$(".view-info-btn").off().on("click", function (e) {
			$(".view-info").show();
			$(".view-links").hide();
			$(".view-rating").hide();
			$(".view-settings").hide();
			currentView = '';
			updateUrl();
		});

		$(".view-links-btn").off().on("click", function (e) {
			$(".view-info").hide();
			$(".view-links").show();
			$(".view-rating").hide();
			$(".view-settings").hide();
			currentView = 'links';
			updateUrl();
		});

		$(".view-rating-btn").off().on("click", function (e) {
			$(".view-info").hide();
			$(".view-links").hide();
			$(".view-rating").show();
			$(".view-settings").hide();
			currentView = 'rating';
			updateUrl();
		});

		$(".view-settings-btn").off().on("click", function (e) {
			$(".view-info").hide();
			$(".view-links").hide();
			$(".view-rating").hide();
			$(".view-settings").show();
			currentView = 'settings';
			updateUrl();
		});

		if (currentView == 'rating') {
			$(".view-rating-btn").trigger("click");
		}
		if (currentView == 'links') {
			$(".view-links-btn").trigger("click");
		}
		if (currentView == 'settings') {
			$(".view-settings-btn").trigger("click");
		}
		updateUrl();
	});
}

function updateUrl() {
	var url = "?coin=DOT";
	if (currentCoin != '') {
		url = "?coin=" + currentCoin;
	}
	if (currentView != '') {
		url += '&view=' + currentView;
	}
	History.pushState({}, "Cryptopia - " + Resources.CoinInfo.PageTitle, url);
}

var scroll = true;
var initialLoad = true;
var table = $("#currencyInfo");
var tableaction = table.data("action");
var datatable = table.DataTable({
	"order": [[1, "asc"]],
	"lengthChange": false,
	"processing": false,
	"bServerSide": false,
	"searching": true,
	"scrollY": "0px",
	"scrollX": false,
	"paging": false,
	"info": true,
	"ajax": tableaction,
	"sServerMethod": "GET",
	"language": { "emptyTable": Resources.CoinInfo.InfoCurrencyListEmptyMessage },
	"columnDefs": [
		{ "targets": 0, "orderable": false, "visible": false },
		{ "targets": 2, "searchable": true, "orderable": false, "visible": false },
		{ "targets": 9, "orderable": false, "visible": false },
		{
			"targets": 1,
			"render": function (data, type, full, meta) {
				return '<div style="display:inline-block"><div class="sprite-small small/' + full[2] + '-small.png"></div> ' + data + ' (' + full[2] + ')</div>';
			}
		},
		{
			"targets": 3,
			"searchable": false,
			"orderable": true,
			"type": "title-numeric",
			"render": function (data, type, full, meta) {
				return '<span title="' + data + '"></span><span>' + getStarRating(data) + '</span>';
			}
		},
			{
				"targets": 6,
				"searchable": false,
				"type": "title-numeric",
				"render": function (data, type, full, meta) {
					return '<span title="' + data + '"></span><button class="btn btn-xs btn-info" style="width:110px" onclick="getPeerInfoModal(' + full[0] + ')">Connections: ' + data + '</button>';
				}
			},
		{
			"targets": 8,
			"render": function (data, type, full, meta) {
				if (full[9]) {
                    return '<span style="white-space:nowrap;cursor:pointer" class="btn-maintenance" data-message="' + full[9] + '">' + data + ' <i style="height:100%;vertical-align:middle;" class="fa fa-info-circle text-info"></i></span>';
				}
				return data;
			}
		}
	],
	"fnRowCallback": function (nRow, aData) {
        var row = $(nRow);

        if (aData[8] != 'OK') {
            row.addClass("warning");
        }

        if (aData[10] === 'Delisting') {
            row.removeClass("warning");
            row.addClass("danger");
        }

		row.off().on("click", function () {
			$("#currencyInfo tr").removeClass("info");
			$(this).addClass("info");
			if (currentCoin != aData[2]) {
				currentCoin = aData[2];
				showInformation(aData[0]);
			}
		});
		if (initialLoad == true && aData[2] == selectedCoin) {
			initialLoad = false;
			row.trigger("click");
		}
	},
	"fnDrawCallback": function () {
		$(".btn-maintenance").off().on("click", function (e) {
			notify(Resources.CoinInfo.InfoWalletStatusMessageTitle, $(this).data("message") || Resources.CoinInfo.InfoWalletStatusMaintenanceMessage);
		});
		scrollToItem();
	}
});

function getPeerInfoModal(currencyId) {
	openModalGet(peerInfoAction, { id: currencyId });
}

function scrollToItem() {
	var btn = $("#currencyInfo tr.info");
	if (btn.length > 0 && scroll) {
		scroll = false;
		var scrollX = 0;
		if (btn.position()) {
			scrollX = btn.position().top
		}
		var itemSize = btn.height() * 3;
		var scrollH = $('.dataTables_scrollBody').height();
		if (scrollX > (scrollH - itemSize)) {
			$('.dataTables_scrollBody').animate({
				scrollTop: scrollX - itemSize
			});
		}
	}
}

$(window).resize(function () {
	var scrollY = $("#table-panel").height() - 135;
	$(".dataTables_scrollBody").height(scrollY);
});
$(window).resize();