$(document).ready(function () {
    initSalesDataTable();
    initActivitiesDataTable();
});

function initSalesDataTable() {
    var salesTable = $("#sales-datatable").DataTable({
        responsive: true,
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

    salesTable.columns().eq(0).each(function (colIdx) {
        $('input', $('th')[colIdx]).on('keyup change', function () {
            salesTable
                .column(colIdx)
                .search(this.value)
                .draw();
        });
    });
}

function initActivitiesDataTable() {
    var activitiesTable = $("#activities-datatable").DataTable({
        responsive: true,
        "pagingType": "numbers",
        "language": {
            "lengthMenu": "Отобразить _MENU_ действий на странице",
            "zeroRecords": "Действия отсутствуют",
            "info": "Отображение страницы _PAGE_ из _PAGES_",
            "infoEmpty": "Записи отсутствуют",
            "infoFiltered": "(Отфильтровано из _MAX_ записей)",
            "search": "Поиск"
        },
        "drawCallback": function () {
            $('.pagination').addClass('pagination-sm');
        }
    });

    activitiesTable.columns().eq(0).each(function (colIdx) {
        $('input', $('th')[colIdx]).on('keyup change', function () {
            activitiesTable
                .column(colIdx)
                .search(this.value)
                .draw();
        });
    });
}