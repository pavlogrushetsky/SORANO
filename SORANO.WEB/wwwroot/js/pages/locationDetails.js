$(document).ready(function () {
    $('#accordion').on('shown.bs.collapse', function () {
        var active = $("#accordion .in").attr('id');
        localStorage.setItem('location-details-active-panel', active);
        localStorage.removeItem('location-details-all-collapsed');
    });
    $('#accordion').on('hidden.bs.collapse', function () {
        localStorage.setItem('location-details-all-collapsed', true);
        localStorage.removeItem('location-details-active-panel');
    });

    var lastPanel = localStorage.getItem('location-details-active-panel');
    var allCollapsed = localStorage.getItem('location-details-all-collapsed');
    if (lastPanel) {
        $("#accordion .panel-collapse").removeClass('in');
        $("#" + lastPanel).addClass("in");
    }
    if (allCollapsed) {
        $("#accordion .panel-collapse").removeClass('in');
    }     

    initRecomendationsDataTable();
    initAttachmentsDataTable();
    initGoodsDataTable();
});

function initGoodsDataTable() {
    var goodsTable = $("#goods-datatable").DataTable({
        responsive: true,
        "columnDefs": [
            { "orderable": false, "targets": 4 }
        ],
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

    goodsTable.columns().eq(0).each(function (colIdx) {
        $('input', $('#goods-datatable th')[colIdx]).on('keyup change',
            function () {
                goodsTable
                    .column(colIdx)
                    .search(this.value)
                    .draw();
            });
    });
}