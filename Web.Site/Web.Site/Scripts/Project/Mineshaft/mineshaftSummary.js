var basemarket = $("#summary-wrapper").data("algotype");
$('#featured-pool, #top-pool').height(Math.max($('#top-pool').height(), $('#featured-pool').height()));
$(".algoData-btn").on("click", function () {
	var _this = $(this);
	var algo = _this.data("algo");
	$("#baseAlgos").val(algo).trigger('change');
});
$("#baseAlgos").val(basemarket).trigger('change');

$(function () {
	$('#carousel-example-generic').carousel();
})