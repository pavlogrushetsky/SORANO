$(document).ready(function () {
    initAttachmentTypesDataTable();
});

function initAttachmentTypesDataTable() {
    var table = $("#attachment-types-datatable").DataTable({
        responsive: true,
        "columnDefs": [
            { "orderable": false, "targets": 5 }
        ],
        "pagingType": "numbers",
        "language": {
            "lengthMenu": "Отобразить _MENU_ типов на странице",
            "zeroRecords": "Типы вложений отсутствуют",
            "info": "Отображение страницы _PAGE_ из _PAGES_",
            "infoEmpty": "Записи отсутствуют",
            "infoFiltered": "(Отфильтровано из _MAX_ записей)",
            "search": "Поиск"
        },
        "drawCallback": function () {
            $('.pagination').addClass('pagination-sm');
        }
    });

    table.columns().eq(0).each(function (colIdx) {
        $('input', $('#attachment-types-datatable th')[colIdx]).on('keyup change', function () {
            table
                .column(colIdx)
                .search(this.value)
                .draw();
        });
    });
}