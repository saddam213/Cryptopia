

$('#hashrateChart').highcharts({
	chart: {
		zoomType: 'x',
		backgroundColor: 'transparent'
	},
	title: {
		text: ''
	},
	xAxis: {
		type: 'datetime'
	},
	yAxis: {
		title: {
			text: 'Pool Hashrate'
		},
		min: 0
	},
	legend: {
		enabled: false
	},
	credits: {
		enabled: false
	},
	tooltip: {
		crosshairs: [true, true],
		shared: true,
		headerFormat: '<span style="font-size:14px;font-weight:bold">Pool Hashrate</span><br /><span>Time: <b>{point.key}</b>  </span><br/>',
		pointFormatter: function () {
			return '<span style="font-size:16px;color:'+this.series.color+'">\u25CF</span><span> ' + this.series.name + ': <b>' + hashrateLabel(this.y) + '</b> </span><br/>'
		}
	},
	plotOptions: {
		areaspline: {
			marker: {
				enabled:false
			},
			lineWidth: 1,
			states: {
				hover: {
					lineWidth: 1
				}
			},
			threshold: null
		}
	},

	series: [{
		type: 'areaspline',
		name: 'Pool Hashrate',
		//color: "#5cb85c",
		fillOpacity: 0.5,
		data: []
	},
	{
		type: 'areaspline',
		name: 'Your Hashrate',
		color: "#434348",
		lineColor: "#7cb5ec",
		fillOpacity: 0.3,
		data: []
	}]
});


$('#blockChart').highcharts({
	chart: {
		type: 'spline',
		backgroundColor: 'transparent'
	},
	title: {
		text: ''
	},
	legend: {
		enabled: false
	},
	xAxis: {
		labels: {
			enabled: true,
		},
		type:'category',
		allowDecimals: false,
	},
	yAxis: {
		title: {
			text: 'Number Of Shares'
		},
		labels: {
			formatter: function () {
				return this.value;
			}
		},
		min: 0
	},
	tooltip: {
		crosshairs: [true,true],
		shared: true,
		headerFormat: '<span style="font-size:14px;font-weight:bold">Block Shares</span><br /><span>Block height: <b>{point.key}</b>  </span><br/>',
		pointFormat: '<span style="font-size:16px;color:{series.color}">\u25CF</span><span> {series.name}: <b>{point.y:.2f}</b> </span><br/>'
	},
	plotOptions: {
		spline: {
		}
	},
	series: [{
		name: 'Actual Shares',
		marker: {
			symbol: 'circle'
		},
		data: []

	}, {
		name: 'Expected Shares',
		marker: {
			symbol: 'circle'
		},
		data: []
	}]
});

function getHashrateChartData() {
	getData(hashrateChartAction, { poolId: poolId }, function (data) {
		updateHashrateChart(data);
	});
}

setInterval(function () {
	getHashrateChartData();
}, 120000);

$('.chart-option').on('click', function () {
	$('.chart-option').removeClass('active');
	$(this).addClass('active');
	var selected = $(this).data('type');
	if (selected === 'hashrate') {
		selectedChart = 'hashrate';
		$('.hashrateChart').show();
		$('.blockChart, .somethingChart').hide();
		getHashrateChartData();
	}
	else if (selected === 'block') {
		selectedChart = 'block';
		$('.blockChart').show();
		$('.hashrateChart, .somethingChart').hide();
		getBlockChartData();
	}
});

function getBlockChartData() {
	getData(blockChartAction, { poolId: poolId }, function (data) {
		updateBlockChart(data);
	});
}

function setVisibleChart() {
	$('.chart-option').removeClass('active');
	if (selectedChart === 'hashrate') {
		$('.chart-option-hashrate').trigger('click');
	}
	else if (selectedChart === 'block') {
		$('.chart-option-block').trigger('click');
	}
}

function updateHashrateChart(chartData) {
	var pooldata = chartData.PoolData || [];
	var userdata = chartData.UserData || [];
	var chart = $('#hashrateChart').highcharts();
	if (chart) {
		chart.showLoading();
		chart.series[0].setData(pooldata)
		if (userdata.length > 0) {
			chart.series[1].setData(userdata)
		}
		chart.reflow();
		chart.hideLoading();
	}
}

function updateBlockChart(chartData) {
	var data = chartData.ActualData || [];
	var estdata = chartData.EstimatedData || [];
	var tickdata = chartData.BlockHeightData || [];
	var blockChart = $('#blockChart').highcharts();
	if (blockChart) {
		blockChart.showLoading();
		blockChart.series[0].setData(data);
		blockChart.series[1].setData(estdata);
		blockChart.xAxis[0].update({ categories: tickdata }, true);
		blockChart.reflow();
		blockChart.hideLoading();
	}
}