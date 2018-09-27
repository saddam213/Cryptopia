$(function () {
	$('#Symbol').select2({
		placeholder: Resources.Deposit.SelectCurrencyLabel,
		allowClear: false,
		templateResult: formatState,
		templateSelection: formatState
	});

	$('#Symbol').on("change", function () {
		var url = displayAction + '?currency='
		var selection = $(this).val();
		if (selection) {
			$("#next").removeAttr("disabled");
			$("#next").attr("href", url + selection + ((returnUrl) ? "&returnUrl=" + returnUrl : ""));
		}
	});

	function formatState(state) {
		if (!state.id) { return state.text; }
		var $state = $(
			'<div><div style="margin-top:2px;margin-right:3px" class="sprite-small small/' + state.element.value + '-small.png"></div>' + state.text + '</div>'
		);
		return $state;
	};
});