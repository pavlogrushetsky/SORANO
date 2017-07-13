$(document).ready(function () {
    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        localStorage.setItem('locations-active-tab', $(e.target).attr('href'));
    });

    var lastTab = localStorage.getItem('locations-active-tab');
    if (lastTab) {
        $('[href="' + lastTab + '"]').tab('show');
    }

    var locationsTable = $("#locations-datatable").DataTable({
        responsive: true,
        "columnDefs": [
            { "orderable": false, "targets": 3 }
        ],
        "pagingType": "numbers",
        "language": {
            "lengthMenu": "Отобразить _MENU_ мест на странице",
            "zeroRecords": "Места отсутствуют",
            "info": "Отображение страницы _PAGE_ из _PAGES_",
            "infoEmpty": "Записи отсутствуют",
            "infoFiltered": "(Отфильтровано из _MAX_ записей)",
            "search": "Поиск"
        }
    });

    locationsTable.columns().eq(0).each(function (colIdx) {
        $('input', $('#locations-datatable th')[colIdx]).on('keyup change', function () {
            locationsTable
                .column(colIdx)
                .search(this.value)
                .draw();
        });
    });

    var locationTypesTable = $("#location-types-datatable").DataTable({
        responsive: true,
        "columnDefs": [
            { "orderable": false, "targets": 3 }
        ],
        "pagingType": "numbers",
        "language": {
            "lengthMenu": "Отобразить _MENU_ типов мест на странице",
            "zeroRecords": "Типы мест отсутствуют",
            "info": "Отображение страницы _PAGE_ из _PAGES_",
            "infoEmpty": "Записи отсутствуют",
            "infoFiltered": "(Отфильтровано из _MAX_ записей)",
            "search": "Поиск"
        }
    });

    locationTypesTable.columns().eq(0).each(function (colIdx) {
        $('input', $('#location-types-datatable th')[colIdx]).on('keyup change', function () {
            locationTypesTable
                .column(colIdx)
                .search(this.value)
                .draw();
        });
    });

    $('.pagination').addClass('pagination-sm');
});