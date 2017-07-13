$(document).ready(function () {
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

    $('.pagination').first().addClass('pagination-sm');
});