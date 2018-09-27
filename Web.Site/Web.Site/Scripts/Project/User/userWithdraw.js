
var datatable;
var table = $("#withdrawals");
var action = table.data("action");
var actionresend = table.data("action-resend");
var actioncancel = table.data("action-cancel");
datatable = table.DataTable({
	"dom": datatableExportLayout,
	"buttons": datatableExportButtons("Withdraw History"),
	"lengthChange": true,
	"lengthMenu": [[15, 50, 500, 5000, 25000], [15, 50, 500, 5000, 25000]],
	"order": [[0, "desc"]],
	"searchDelay": 800,
	"processing": false,
	"bServerSide": true,
	"searching": true,
	"scrollCollapse": true,
	"scrollX": "100%",
	"paging": true,
	"info": true,
	"iDisplayLength": 15,
	"sAjaxSource": action,
	"sServerMethod": "POST",
	"language": {
		"emptyTable": Resources.UserWithdraw.EmptyListMessage,
		"paginate": {
			"next": Resources.General.Next,
			"previous": Resources.General.Previous
		}
	},
	"columnDefs": [{
			"targets": 1,
			"render": function (data, type, full, meta) {
				return '<div style="display:inline-block"><div class="sprite-small small/' + data + '-small.png"></div> ' + data + '</div>';
			}
		},
		{
			"targets": 5,
			"searchable": false,
			"orderable": false,
			"render": function (data, type, full) {
				if (!data && full[4] === 'Unconfirmed') {
					return '<div style="max-width:300px" class="btn-group btn-group-xs btn-group-justified nopad" ><a class="btn btn-xs btn-default" onclick="resendConfirmationEmail(' + full[0] + ');">' + Resources.UserWithdraw.ResendEmailButton + '</a><a class="btn btn-xs btn-warning" onclick="cancelWithdraw(' + full[0] + ');">' + Resources.General.Cancel + '</a></div>';
				}
				return '<div style="text-overflow:ellipsis;white-space:nowrap;overflow:hidden;display:block;max-width:300px">' + data + '</div>';
			}
		},
		{
		"targets": 6,
		"render": function (data, type, full, meta) {
			return '<div style="text-overflow:ellipsis;white-space:nowrap;overflow:hidden;display:block;max-width:200px">' + data + '</div>';
			}
		},
		{
			"targets": 7,
			"searchable": true,
			"orderable": true,
			"render": function (data, type, full, meta) {
				return toLocalTime(data);
			}
		}
	]
});

$("#withdrawsTarget").addClass("user-tabtarget");

function resendConfirmationEmail(withdrawId) {
	confirm(Resources.UserWithdraw.ResendEmailQuestionTitle, Resources.UserWithdraw.ResendEmailQuestion, function() {
		$.blockUI({ message: Resources.UserWithdraw.ResendingEmailMessage });
		getJson(actionresend, { withdrawId: withdrawId }, function(data) {
			$.unblockUI();
			if (data.IsError) {
				notify(Resources.UserWithdraw.ResendEmailErrorMessageTitle, data.Message);
			}
			notify(Resources.UserWithdraw.ResendEmailSuccessMessageTitle, data.Message);
		});
	});
}

function cancelWithdraw(withdrawId) {
	confirm(Resources.UserWithdraw.CancelQuestionTitle, Resources.UserWithdraw.CancelQuestion, function() {
		$.blockUI({ message: Resources.UserWithdraw.CancelingMessage });
		getJson(actioncancel, { withdrawId: withdrawId }, function(data) {
			$.unblockUI();
			if (data.Success) {
				datatable.ajax.reload();
				notify(Resources.UserWithdraw.CancelSuccessMessageTitle, data.Message);
			} else {
				notify(Resources.UserWithdraw.CancelErrorMessageTitle, data.Message);
			}
		});
	});
}