var isAdmin = $(".table-header-container").data("admin") === "True";;
var table = $("#users");
var tableaction = table.data("action");
var tableactionView = table.data("action-view");
var tableactionUpdate = table.data("action-update");
var tableactionLock = table.data("action-lock");
var datatable = table.DataTable({
	"order": [[0, "asc"]],
	"searchDelay": 800,
	"lengthChange": false,
	"processing": false,
	"bServerSide": true,
	"searching": true,
	"paging": true,
	"info": true,
	"iDisplayLength": 15,
	"sAjaxSource": tableaction,
	"sServerMethod": "POST",
	"language": { "emptyTable": "There are no users." },
	"columnDefs": [
		{
			"targets": 7,
			"visible": isAdmin,
			"searchable": false,
			"orderable": false,
			"render": function (data, type, full) {
				return '<div style="min-width:145px;max-width:145px" class="btn-group btn-group-sm btn-group-justified pull-right" ><a class="btn btn-default btn-sm btn-lock" data-user="' + full[0] + '">Lock</a><a class="btn btn-default btn-sm btn-view" data-user="' + full[0] + '">View</a><a class="btn btn-primary btn-sm btn-edit" data-user="' + full[0] + '">Edit</a></div>';
			}
		}
	],
	"fnDrawCallback": function () {
		$(".btn-view").on("click", function () {
			var user = $(this).data("user");
			if (user) {
				openModalGet(tableactionView, { username: user });
			}
		});
		$(".btn-edit").on("click", function () {
			var user = $(this).data("user");
			if (user) {
				openModalGet(tableactionUpdate, { username: user }, function (modalData) {
					if (modalData && modalData.Success) {
						datatable.ajax.reload();
					}
					showMessage(modalData);
				});
			}
		});
		$(".btn-lock").on("click", function () {
			var user = $(this).data("user");
			if (user) {
				$.blockUI({ message: "Locking..." });
				postJson(tableactionLock, { username: user }, function (data) {
					if (data && data.Success) {
						datatable.ajax.reload();
					}
					showMessage(data);
					$.unblockUI();
				});
			}
		});
	}
});

$("#adminUserTarget").addClass("user-tabtarget");