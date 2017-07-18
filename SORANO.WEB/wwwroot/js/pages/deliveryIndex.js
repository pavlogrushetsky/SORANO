$(document).ready(function () {
    initDeliveriesDataTable();
});

function initDeliveriesDataTable() {
    var table = $("#deliveries-datatable").DataTable({
        responsive: true,
        "columnDefs": [
            { "orderable": false, "targets": 7 }
        ],
        "pagingType": "numbers",
        "language": {
            "lengthMenu": "Отобразить _MENU_ накладных на странице",
            "zeroRecords": "Накладные отсутствуют",
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
        $('input', $('#deliveries-datatable th')[colIdx]).on('keyup change', function () {
            table
                .column(colIdx)
                .search(this.value)
                .draw();
        });
    });
}