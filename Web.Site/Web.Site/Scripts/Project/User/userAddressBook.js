var datatable;
var table = $("#addressBook");
var action = table.data("action");

datatable = table.DataTable({
	"order": [[0, "asc"]],
	"searchDelay": 800,
	"lengthChange": false,
	"processing": false,
	"bServerSide": true,
	"searching": false,
	"scrollCollapse": true,
	"scrollY": "250px",
	"paging": false,
	"info": false,
	"iDisplayLength": 15,
	"sAjaxSource": action,
	"sServerMethod": "GET",
	"language": { "emptyTable": Resources.UserAddressBook.SecurityAddressBookEmptyListMessage },

	"columnDefs": [
		{ "targets": 0, "visible": false },
		{ "targets": 1, "visible": false },
		{
			"targets": 4,
			"searchable": false,
			"orderable": false,
			"render": function (data, type, full, meta) {
				var address = data;
				var addressParts = data.split(':');
				if (addressParts.length == 2) {
					address = addressParts[0] + ", " + full[5] + ": " + addressParts[1]
				}
				return '<span title="' + address + '">' + address + '</span>'
			}
		},
		{ "targets": 5, "visible": false },
		{
			"targets": 6,
			"searchable": false,
			"orderable": false,
			"render": function (data, type, full, meta) {
				return '<i title="' + Resources.UserAddressBook.SecurityAddressBookDeleteHint + '" class="fa fa-times text-danger" style="cursor:pointer;font-size:16px" onclick="deleteAddressBook(' + full[0] + ')"></i>';
			}
		}
	]
});


$("#addAddressBook").on("click", function () {
	var action = $(this).data("action");
	openModalGet(action, null, function () {
		datatable.ajax.reload();
	});
});

function deleteAddressBook(addressId) {
	if (addressId) {
		var action = $("#addressBook").data("delete");
		confirm(Resources.UserAddressBook.SecurityAddressBookDeleteQuestionTitle, Resources.UserAddressBook.SecurityAddressBookDeleteQuestion, function () {
			postJson(action, { AddressBookId: addressId }, function () {
				datatable.ajax.reload();
			});
		});
	}
}