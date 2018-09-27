$(window).resize(function () {
	var scrollY = $(".news-sidebar").height() + 104;
	$(".twitter-timeline").height(scrollY);
});

$(function () {
	setTimeout(function () {
		$(window).resize();
	}, 1500)
});

