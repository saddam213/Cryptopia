var table = $('#markethistory');
var action = table.data("action");
var actionfeedback = table.data("action-feedback");
var actionmarketitem = table.data("action-marketitem");

table.dataTable({
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
	"sAjaxSource": action,
	"sServerMethod": "POST",
	"language": {
		"emptyTable": Resources.MarketHistory.EmptyListMessage,
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
			"targets": 4,
			"searchable": false,
			"orderable": false,
			"render": function(data, type, full, meta) {
				var rating = getStarRating(data);
				if (rating == undefined) {
					rating = 'No feedback';
				}
				return '<span>' + rating + '</span>';
			}
		},
		{
			"targets": 6,
			"searchable": false,
			"orderable": false,
			"render": function(data, type, full, meta) {
				var rating = getStarRating(data);
				if (rating == undefined) {
					rating = 'No feedback';
				}
				return '<span>' + rating + '</span>';
			}
		},
		{
			"targets": 10,
			"searchable": false,
			"orderable": false,
			"render": function(data, type, full, meta) {
				return '<a href="' + actionmarketitem + "?marketItemId=" + full[0] + '"><i title="' + Resources.MarketHistory.OpenButton + '" class="table-column-icon-info fa fa-info-circle" ></i></a>';
			}
		}
	]
});

$("#marketplaceHistoryTarget").addClass("user-tabtarget");