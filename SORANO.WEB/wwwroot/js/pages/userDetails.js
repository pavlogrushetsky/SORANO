$(document).ready(function () {
    $('#accordion').on('shown.bs.collapse', function () {
        var active = $("#accordion .in").attr('id');
        localStorage.setItem('user-details-active-panel', active);
        localStorage.removeItem('user-details-all-collapsed');
    });
    $('#accordion').on('hidden.bs.collapse', function () {
        localStorage.setItem('user-details-all-collapsed', true);
        localStorage.removeItem('user-details-active-panel');
    });

    var lastPanel = localStorage.getItem('user-details-active-panel');
    var allCollapsed = localStorage.getItem('user-details-all-collapsed');
    if (lastPanel) {
        $("#accordion .panel-collapse").removeClass('in');
        $("#" + lastPanel).addClass("in");
    }
    if (allCollapsed) {
        $("#accordion .panel-collapse").removeClass('in');
    }

    initSalesDataTable();
    initActivitiesDataTable();
});

function initSalesDataTable() {
    var salesTable = $("#sales-datatable").DataTable({
        responsive: true,
        "autoWidth": false,
        "scrollX": false,
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
        $('input', $('#sales-datatable th')[colIdx]).on('keyup change', function () {
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
        "autoWidth": false,
        "scrollX": false,
        "order": [[0, "desc"]],
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
        $('input', $('#activities-datatable th')[colIdx]).on('keyup change', function () {
            activitiesTable
                .column(colIdx)
                .search(this.value)
                .draw();
        });
    });
}