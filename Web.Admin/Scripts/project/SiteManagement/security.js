var tableLockedUsers = $("#lockedUserTable");
var tableLockedUsersAction = tableLockedUsers.data("action");
var actionUnlockUser = tableLockedUsers.data("action-unlock");

var tableOptionsLayout = "<'row'<'col-sm-12'f>>" +
	"<'row'<'col-sm-12'tr>>" +
	"<'row'<'col-sm-5'i><'col-sm-7'p>>"

var datatableLockedUsers = tableLockedUsers.DataTable({
	"dom": tableOptionsLayout,
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
	"sAjaxSource": tableLockedUsersAction,
	"sServerMethod": "POST",
	"language": {
		"emptyTable": "No locked users found..",
		"search": ""
	},
	"columnDefs": [
		{
			"targets": 3,
			"searchable": false,
			"orderable": false,
			"render": function(data, type, full) {
				return '<span><input type="checkbox" id="sendEmail-' + full[0] + '" /> Send Email</span>';
			}
		},
		{
			"targets": 4,
			"searchable": false,
			"orderable": false,
			"render": function(data, type, full) {
				return '<button class="btn btn-default btn-sm pull-right btn-unlock" style="min-width:130px" data-user="' + full[0] + '">Unlock</button>';
			}
		}
	],
	"fnDrawCallback": function() {
		$(".btn-unlock").on("click", function() {
			var button = $(this);
			var user = button.data("user");
			var sendEmail = $("#sendEmail-" + user).is(":checked");
			if (user) {
				$.blockUI({ message: "Unlocking..." });
				postJson(actionUnlockUser, { username: user, sendemail: sendEmail }, function(data) {
					if (data && data.Success) {
						datatableLockedUsers.ajax.reload();
					}
					showMessage(data);
					$.unblockUI();
				});
			}
		});
	}
});

var tableUserLogons = $("#userLogons");
var tableUserLogonsAction = tableUserLogons.data("action");
var datatableUserLogons = tableUserLogons.DataTable({
	"order": [[2, "desc"]],
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
	"sAjaxSource": tableUserLogonsAction,
	"sServerMethod": "POST",
	"language": { "emptyTable": "No logons." },
	"columnDefs": []
});

$("#adminSecurityTarget").addClass("user-tabtarget");