$(document).ready(function () {
    $('#accordion').on('shown.bs.collapse', function () {
        var active = $("#accordion .in").attr('id');
        localStorage.setItem('sale-details-active-panel', active);
        localStorage.removeItem('sale-details-all-collapsed');
    });
    $('#accordion').on('hidden.bs.collapse', function () {
        localStorage.setItem('sale-details-all-collapsed', true);
        localStorage.removeItem('sale-details-active-panel');
    });

    var lastPanel = localStorage.getItem('sale-details-active-panel');
    var allCollapsed = localStorage.getItem('sale-details-all-collapsed');
    if (lastPanel) {
        $("#accordion .panel-collapse").removeClass('in');
        $("#" + lastPanel).addClass("in");
    }
    if (allCollapsed) {
        $("#accordion .panel-collapse").removeClass('in');
    }

    initRecomendationsDataTable();
    initAttachmentsDataTable();
    initSaleItemsDataTable();
});

function initSaleItemsDataTable() {
    var table = $("#sale-items-datatable").DataTable({
        responsive: true,
        "autoWidth": false,
        "scrollX": false,
        "pagingType": "numbers",
        "language": {
            "lengthMenu": "Отобразить _MENU_ товаров на странице",
            "zeroRecords": "Товары отсутствуют",
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
        $('input', $('#sale-items-datatable th')[colIdx]).on('keyup change', function () {
            table
                .column(colIdx)
                .search(this.value)
                .draw();
        });
    });
}