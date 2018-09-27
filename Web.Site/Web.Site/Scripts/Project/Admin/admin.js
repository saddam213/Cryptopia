$(function() {
	$('a[data-toggle="tab"]').on("shown.bs.tab", function(e) {
		var section = $(this).data("section");
		if (section) {
			setSelectedTab(section);
		}
	});
	$('#tabcontrol a[data-section="' + $("#tabcontrol").data("default") + '"]').trigger("click");
});

var lastSectionTarget;

function setSelectedTab(tabName) {
	var seletcedTab = $('#tabcontrol a[data-section="' + tabName + '"]');
	if (seletcedTab) {
		if (lastSectionTarget) {
			var lastTarget = $(lastSectionTarget);
			if (lastTarget) {
				lastTarget.removeClass("user-tabtarget");
				lastTarget.off();
				lastTarget.unbind();
				lastTarget.empty();
			}
		}
		var target = seletcedTab.data("div");
		var action = seletcedTab.data("action");
		var section = seletcedTab.data("section");
		getPartial(target, action, false);

		lastSectionTarget = target;
		History.pushState({}, "Cryptopia - " + section, section);
	}
}
