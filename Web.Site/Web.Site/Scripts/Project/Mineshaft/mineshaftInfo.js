
$('#blockHistory').dataTable({
	"order": [[0, "desc"]],
	"lengthChange": false,
	"processing": false,
	"bServerSide": true,
	"searching": false,
	"paging": true,
	"sort": false,
	"scrollCollapse": true,
	"scrollY": "100%",
	"sAjaxSource": "/Mineshaft/GetBlocks",
	"fnServerParams": function (aoData) {
		aoData.push({ "name": "poolId", "value": poolId });
	},
	"sServerMethod": "POST",
	"info": false,
	"language": {
		"emptyTable": "No blocks found."
	},
	"columnDefs": [{
		"targets": 1,
		"searchable": false,
		"orderable": false,
		"render": function (data, type, full, meta) {
			if (data <= 100) {
				return '<span class="text-success" >' + data + '%</span>';
			}
			return '<span class="" >' + data + '%</span>';
		}
	}, {
		"targets": 8,
		"render": function (data, type, full, meta) {
			return toLocalTime(data);
		}
	}],
	"fnRowCallback": function (nRow, aData) {
		if (aData[2] == miningname) {
			$(nRow).addClass("success");
		}
	}
});

$('#payoutHistory').dataTable({
	"order": [[1, "desc"]],
	"lengthChange": false,
	"processing": false,
	"bServerSide": true,
	"searching": false,
	"paging": true,
	"sort": false,
	"scrollCollapse": true,
	"scrollY": "100%",
	"sAjaxSource": "/Mineshaft/GetPayouts",
	"fnServerParams": function (aoData) {
		aoData.push({ "name": "poolId", "value": poolId });
	},
	"sServerMethod": "POST",
	"info": false,
	"language": {
		"emptyTable": "No payments found."
	},
	"columnDefs": [{ "targets": 0, "visible":false },
		{
		"targets": 5,
		"render": function (data, type, full, meta) {
			if (full[3] == 'Unconfirmed') {
				return '<i>Pending</i>';
			}
			else if (full[3] == 'Orphan') {
				return 'N/A';
			}
			return data;
		}
	}, {
		"targets": 6,
		"render": function (data, type, full, meta) {
			return toLocalTime(data);
		}
	}],
	"fnRowCallback": function (nRow, aData) {
	}
});


$('#poolWorkers').dataTable({
	"order": [[0, "asc"]],
	"lengthChange": false,
	"processing": false,
	"bServerSide": true,
	"searching": false,
	"paging": false,
	"sort": false,
	"scrollCollapse": false,
	"scrollY": "324px",
	"sAjaxSource": "/Mineshaft/GetMiners",
	"fnServerParams": function (aoData) {
		aoData.push({ "name": "poolId", "value": poolId });
	},
	"sServerMethod": "POST",
	"info": false,
	"language": {
		"emptyTable": "No active workers found"
	},
	"columnDefs": [{ "targets": 2, "visible": false },
	{
		"targets": 3,
		"iDataSort": 2,
		"render": function (data, type, full, meta) {
			return '<span class="hashrate-user hashrate-user-' + full[1] + '">' + hashrateLabel(full[2]) + '</span>';
		}
	}],
	"fnRowCallback": function (nRow, aData) {
		$(nRow).addClass("hash-sort").attr("data-hash", aData[2]);
		if (aData[1] == miningname) {
			$(nRow).addClass("success");
		}
	}
});


function mineshaftDataUpdate(notification) {
	if (notification.PoolId != poolId)
		return;

	if (notification.Type == 0) {
		updateStatistics(notification);
	}
	else if (notification.Type == 1) {
		getBlockChartData();
		updateBlocks(notification);
	}
	else if (notification.Type == 3) {
		updateUserStatistics(notification);
	}
}

function mineshaftUserDataUpdate(notification) {
	if (notification.PoolId != poolId)
		return;

	if (notification.Type == 2) {
		updatePayouts(notification);
	}
}

function updateBlocks(notification) {
	var historyBody = $("#blockHistory tbody");
	historyBody.find("tr > .dataTables_empty").closest("tr").remove();
	var firstHeight = +historyBody.find("tr:first > td:nth-child(1)").text();
	if (firstHeight > 0) {
		var confirmDelta = +(notification.Height - firstHeight);
		historyBody.find("tr").each(function () {
			var cell = $(this).find("td:nth-child(6)");
			cell.text(+cell.text() + confirmDelta);
		});
	}

	historyBody.prepend(Mustache.render(blockTemplate, {
		highlight: 'greenhighlight',
		userClass: notification.Finder == miningname ? "success" : "",
		block: notification.Height,
		luck: notification.Luck.toFixed(2),
		luckClass: notification.Luck <= 100 ? "text-success" : "",
		finder: notification.Finder,
		amount: notification.Amount.toFixed(8),
		difficulty: notification.Difficulty.toFixed(8),
		confirmations: notification.Confirmations,
		status: notification.Status,
		shares: notification.Shares,
		timestamp: toLocalTime(notification.Timestamp)
	}));

	if (historyBody.find("tr").length > 10) {
		historyBody.find("tr:last").remove();
	}
}

function updatePayouts(notification) {
	var historyBody = $("#payoutHistory tbody");
	historyBody.find("tr > .dataTables_empty").closest("tr").remove();
	historyBody.prepend(Mustache.render(payoutTemplate, {
		highlight: 'greenhighlight',
		id: notification.Id,
		block: notification.Block,
		amount: notification.Amount.toFixed(8),
		status: notification.Status,
		shares:notification.Shares,
		transferId: notification.TransferId,
		timestamp: toLocalTime(notification.Timestamp)
	}));

	if (historyBody.find("tr").length > 10) {
		historyBody.find("tr:last").remove();
	}
}


function updateStatistics(notification) {
	$("#difficulty").html(notification.NetworkDifficulty)
	$("#currentBlock").html(notification.CurrentBlock)
	$("#lastBlock").html(notification.LastPoolBlock)
	$("#estShares").html(notification.EstimatedShares.toFixed(2))
	$("#validShares").html(notification.ValidShares.toFixed(2))
	$("#invalidShares").html(notification.InvalidShares.toFixed(2))
	$("#poolEfficiency").html(getEfficiency(notification.ValidShares, notification.InvalidShares))
	$("#hashrate-pool").html(hashrateLabel(notification.Hashrate))
	$("#hashrate-network").html(hashrateLabel(notification.NetworkHashrate))


	$("#lastTime").html(notification.LastBlockTime ? moment.utc(notification.LastBlockTime).local().fromNow() : 'N/A')
	$("#estTime").html(getEstimatedTime(notification.UserCount,notification.EstimatedTime));
	

	var progressClass = notification.BlockProgress > 100 ? notification.BlockProgress < 200 ? "warning" : "danger" : "success";
	var progressWidth = notification.BlockProgress > 100 ? 100 : notification.BlockProgress;
	$("#blockProgress").css({ "width": progressWidth + "%" })
	$("#blockProgress").removeClass("progress-bar-warning progress-bar-danger progress-bar-success").addClass("progress-bar-" + progressClass)
	$("#blockProgressLabel").html(notification.BlockProgress + "%")
	$("#totalMiners").html(notification.UserCount)
}

function getEstimatedTime(usersOnline, seconds) {
	if (usersOnline == 0) {
		return 'N/A';
	}
	if (seconds > 0) {
		return moment.utc().add(seconds, 'seconds').local().fromNow();
	}
	return 'less than a minute';
}

function updateUserStatistics(notification) {
	var userHashrate = $('.hashrate-user-' + notification.MiningHandle);
	userHashrate.html(hashrateLabel(notification.Hashrate));
	$('.validShares-user-' + notification.MiningHandle).html(notification.ValidShares);
	$('.invalidShares-user-' + notification.MiningHandle).html(notification.InvalidShares);


	var workerBody = $("#poolWorkers tbody");
	if (workerBody.find(".hashrate-user-" + notification.MiningHandle).length == 0) {
		if (notification.Hashrate > 0) {
			workerBody.find("tr > .dataTables_empty").closest("tr").remove();
			workerBody.prepend(Mustache.render(workerTemplate, {
				highlight: 'greenhighlight' + (notification.MiningHandle == miningname ? ' success' : ''),
				username: notification.MiningHandle,
				hashrateraw: notification.Hashrate,
				hashrate: hashrateLabel(notification.Hashrate)
			}));
		}
	}

	if (notification.Hashrate == 0) {
		workerBody.find(".hashrate-user-" + notification.MiningHandle).closest("tr").remove();
		if (workerBody.find('tr').length == 0) {
			workerBody.prepend('<tr class="odd"><td class="dataTables_empty" valign="top" colspan="3">No active workers found</td></tr>');
		}
	}

	if (workerBody.find("tr > .dataTables_empty").length == 0) {
		workerBody.find('.hash-sort').sort(function (a, b) {
			return +b.dataset.hash - +a.dataset.hash;
		}).appendTo(workerBody);

		workerBody.find('tr').each(function (i, element) {
			$(element).find("td:first").text(i + 1);
		});
	}
}

function getUserInfo() {

	getData("/Mineshaft/GetMineshaftUserInfo?id=" + poolId, { id: poolId }, function (data) {
		//alert(JSON.stringify(data))
		$("#hashrate-user").html(hashrateLabel(data.Hashrate || 0))
		$("#userValidShares").html(data.ValidShares.toFixed(2))
		$("#userInvalidShares").html(data.InvalidShares.toFixed(2))
		$("#userConfirmed").html(data.Confirmed.toFixed(8))
		$("#userUnconfirmed").html(data.Unconfirmed.toFixed(8))
		$("#userEfficiency").html(getEfficiency(data.ValidShares, data.InvalidShares));
		$("#userActiveWorkers").html(data.ActiveWorkerCount)
	});
}

function getEfficiency(valid, invalid) {
	if (valid > 0 && invalid > 0) {
		if (valid > invalid) {
			return (100 - ((invalid / valid) * 100)).toFixed(2);
		}
		return (100 - ((valid / invalid) * 100)).toFixed(2);
	}
	else if (valid > 0 && invalid == 0) {
		return (100).toFixed(2);
	}
	return (0).toFixed(2);
}

getUserInfo();
$("#lastTime").html(moment.utc($("#lastTime").html()).local().fromNow());
$("#estTime").html(getEstimatedTime($("#totalMiners").html(), $("#estTime").html()));


function changeUserPool(poolId) {
	openModalGet('/Mineshaft/ChangeUserPool', { poolId: poolId }, function (data) {
		showMessage(data);
		if (typeof updateUserWorkers === 'function') {
			updateUserWorkers();
		}
	});
}

function gettingStarted(poolId) {
	openModalGet('/Mineshaft/GettingStarted', { poolId: poolId });
}