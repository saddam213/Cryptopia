var table = $("#emoticons");
var tableaction = table.data("action");
var previewTemplate = $('#previewTemplate').html();
var emoticonOptionsTemplate = $('#emoticonOptionsTemplate').html();
var createEmoticonAction = table.data("action-create");
var updateEmoticonAction = table.data("action-update");
var deleteEmoticonAction = table.data("action-delete");

var datatable = table.DataTable({
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
	"language": { "emptyTable": "There are no emoticons." },
	"columnDefs": [
		{
			"targets": [3],
			"searchable": false,
			"orderable": true,
			"render": function (data, type, full, meta) {
				return Mustache.render(previewTemplate, {
					name: data,
					path: full[3]
				});
			}
		},
			{
				"targets": [4],
				"searchable": false,
				"orderable": false,
				"render": function (data, type, full, meta) {
					return Mustache.render(emoticonOptionsTemplate, {
						updateEmoticon: 'updateEmoticon(\'' + full[0] + '\')',
						deleteEmoticon: 'deleteEmoticon(\'' + full[0] + '\')'
					});
				}
			}
	],
	"fnDrawCallback": function () {
		$('.preview-popover').popover({
			trigger: 'hover'
		});
	}
});

function createEmoticon() {
	openModalGet(createEmoticonAction, {  }, function (data) {
		showMessage(data);
		if (data && data.Success) {
			datatable.ajax.reload();
		}
	});
}

function updateEmoticon(code) {
	openModalGet(updateEmoticonAction, {code: code}, function (data) {
		showMessage(data);
		if (data && data.Success) {
			datatable.ajax.reload();
		}
	});
}

function deleteEmoticon(code) {
	confirm("Delete Emoticon?", "Are you sure you want to delete this emoticon?", function () {
		$.blockUI({ message: 'Deleting emoticon...' });
		postJson(deleteEmoticonAction, { code: code }, function (data) {
			datatable.ajax.reload();
			$.unblockUI();
			showMessage(data);
		});
	});
}

$("#adminEmoticonTarget").addClass("user-tabtarget");