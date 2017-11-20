$(document).ready(function () {
    $('#accordion').on('shown.bs.collapse', function () {
        var active = $("#accordion .in").attr('id');
        localStorage.setItem('goods-details-active-panel', active);
        localStorage.removeItem('goods-details-all-collapsed');
    });
    $('#accordion').on('hidden.bs.collapse', function () {
        localStorage.setItem('goods-details-all-collapsed', true);
        localStorage.removeItem('goods-details-active-panel');
    });

    var lastPanel = localStorage.getItem('goods-details-active-panel');
    var allCollapsed = localStorage.getItem('goods-details-all-collapsed');
    if (lastPanel) {
        $("#accordion .panel-collapse").removeClass('in');
        $("#" + lastPanel).addClass("in");
    }
    if (allCollapsed) {
        $("#accordion .panel-collapse").removeClass('in');
    }

    initRecomendationsDataTable();
    initDeliveryRecommendationsDataTable();
    initDeliveryItemsRecommendationsDataTable();
});

function initDeliveryRecommendationsDataTable() {
    var recommendationsDataTable = $("#delivery-recommendations-datatable").DataTable({
        responsive: true,
        "autoWidth": false,
        "scrollX": false,
        "pagingType": "numbers",
        "language": {
            "lengthMenu": "Отобразить _MENU_ рекомендаций на странице",
            "zeroRecords": "Рекомендации отсутствуют",
            "info": "Отображение страницы _PAGE_ из _PAGES_",
            "infoEmpty": "Записи отсутствуют",
            "infoFiltered": "(Отфильтровано из _MAX_ записей)",
            "search": "Поиск"
        },
        "drawCallback": function () {
            $('.pagination').addClass('pagination-sm');
        }
    });

    recommendationsDataTable.columns().eq(0).each(function (colIdx) {
        $('input', $('#delivery-recommendations-datatable th')[colIdx]).on('keyup change', function () {
            recommendationsDataTable
                .column(colIdx)
                .search(this.value)
                .draw();
        });
    });
}

function initDeliveryItemsRecommendationsDataTable() {
    var recommendationsDataTable = $("#delivery-item-recommendations-datatable").DataTable({
        responsive: true,
        "autoWidth": false,
        "scrollX": false,
        "pagingType": "numbers",
        "language": {
            "lengthMenu": "Отобразить _MENU_ рекомендаций на странице",
            "zeroRecords": "Рекомендации отсутствуют",
            "info": "Отображение страницы _PAGE_ из _PAGES_",
            "infoEmpty": "Записи отсутствуют",
            "infoFiltered": "(Отфильтровано из _MAX_ записей)",
            "search": "Поиск"
        },
        "drawCallback": function () {
            $('.pagination').addClass('pagination-sm');
        }
    });

    recommendationsDataTable.columns().eq(0).each(function (colIdx) {
        $('input', $('#delivery-item-recommendations-datatable th')[colIdx]).on('keyup change', function () {
            recommendationsDataTable
                .column(colIdx)
                .search(this.value)
                .draw();
        });
    });
}