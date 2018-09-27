var datatable;
var table = $('#openmarket');
var action = table.data("action");
var actionedit = table.data("action-edit");
var actioncancel = table.data("action-cancel");
var actionmarketitem = table.data("action-marketitem");
datatable = table.DataTable({
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
	"iDisplayLength": 6,
	"sAjaxSource": action,
	"sServerMethod": "POST",
	"language": {
		"emptyTable": Resources.MarketItems.EmptyListMessage,
		"paginate": {
			"next": Resources.General.Next,
			"previous": Resources.General.Previous
		}
	},
	"columnDefs": [
		{
			"targets": 1,
			"searchable": true,
			"orderable": true,
			"render": function(data, type, full, meta) {
				return '<a title="' + data + '" class="ellipsis" style="max-width:300px;" href="' + actionmarketitem + '?marketItemId=' + full[0] + '">' + data + '</a>';
			}
		},
		{
			"targets": 6,
			"searchable": false,
			"orderable": false,
			"render": function(data, type, full, meta) {
				return '<a href="' + actionedit + '?marketItemId=' + full[0] + '"><i title="' + Resources.MarketItems.EditListingButton + '" class="fa fa-pencil table-column-icon"></i></a>';
			}
		},
		{
			"targets": 7,
			"searchable": false,
			"orderable": false,
			"render": function(data, type, full, meta) {
				return '<div><i title="' + Resources.MarketItems.CancelListingButton + '" class="fa fa-times table-column-icon text-danger" onclick="cancelMarketItem(' + full[0] + ');"></i></div>';
			}
		}
	]
});

function cancelMarketItem(marketItemId) {
	confirm(Resources.MarketItems.CancelListingQuestionTitle, Resources.MarketItems.CancelListingQuestion, function() {
		$.blockUI({ message: Resources.MarketItems.CancelingMessage });
		postJson(actioncancel, { marketItemId: marketItemId }, function(data) {
			$.unblockUI();
			if (data.Success) {
				datatable.ajax.reload();
			}
		});
	});
}

$("#marketItemsTarget").addClass("user-tabtarget");