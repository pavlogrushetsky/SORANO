$(document).ready(function () {
    initSalesDataTable();
});

function initSalesDataTable() {
    var table = $("#sales-datatable").DataTable({
        responsive: true,
        "autoWidth": false,
        "scrollX": false,
        "columnDefs": [
            { "orderable": false, "targets": -1 }
        ],
        "pagingType": "numbers",
        "language": {
            "lengthMenu": "Отобразить _MENU_ продаж на странице",
            "zeroRecords": "Продажи отсутствуют",
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
        $('input', $('#sales-datatable th')[colIdx]).on('keyup change', function () {
            table
                .column(colIdx)
                .search(this.value)
                .draw();
        });
    });
}