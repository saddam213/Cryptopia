var table = $("#bannedUsers");
var tableaction = table.data("action");
var bannedDatatable = table.DataTable({
	"order": [[0, "desc"]],
	"searchDelay": 800,
	"lengthChange": false,
	"processing": false,
	"bServerSide": true,
	"searching": true,
	"scrollCollapse": true,
	"scrollX": "100%",
	"paging": true,
	"info": true,
	"iDisplayLength": 15,
	"sAjaxSource": tableaction,
	"sServerMethod": "POST",
	language: {
		search: "_INPUT_",
		searchPlaceholder: "Search...",
		emptyTable: "There are no banned users."
	},
	"columnDefs": []
});

var tableReceived = $("#chatHistory");
var tableReceivedaction = tableReceived.data("action");
tableReceived.DataTable({
	"order": [[0, "desc"]],
	"searchDelay": 800,
	"lengthChange": false,
	"processing": false,
	"bServerSide": true,
	"searching": true,
	"paging": true,
	"info": true,
	"iDisplayLength": 15,
	"sAjaxSource": tableReceivedaction,
	"sServerMethod": "POST",
	language: {
		search: "_INPUT_",
		searchPlaceholder: "Search...",
		emptyTable: "There is no chat history."
	},
	"columnDefs": [
		{
			"targets": 3,
			"searchable": true,
			"orderable": true,
			"render": function(data, type, full, meta) {
				return '<p style="white-space:pre-wrap">' + data + "</p>";
			}
		}
	]
});

$("#chatbotSend").on("click", function() {
	var action = $(this).data("action");
	var sender = $("#ChatbotSender").val();
	var msg = $("#ChatbotMsg").val();
	if (msg) {
		postJson(action, { sender: sender, message: msg }, function() {
			$("#ChatbotMsg").val("");
		});
	}
});

$("#chatBanSend").on("click", function() {
	var action = $(this).data("action");
	var user = $("#chatBanList").val();
	var time = $("#chatBanTime").val();
	var message = $("#chatBanMessage").val();
	var bantype = $("#chatBanType").val();
	if (user) {
		postJson(action, { BanUserId: user, Seconds: time, Message: message, BanType: bantype }, function() {
			$("#chatBanTime").val(0);
			$("#chatBanMessage").val("");
			bannedDatatable.ajax.reload();
		});
	}
});

$("#adminChatTarget").addClass("user-tabtarget");