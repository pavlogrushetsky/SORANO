$(document).ready(function () {
    $('#accordion').on('shown.bs.collapse', function () {
        var active = $("#accordion .in").attr('id');
        localStorage.setItem('delivery-details-active-panel', active);
        localStorage.removeItem('delivery-details-all-collapsed');
    });
    $('#accordion').on('hidden.bs.collapse', function () {
        localStorage.setItem('delivery-details-all-collapsed', true);
        localStorage.removeItem('delivery-details-active-panel');
    });

    var lastPanel = localStorage.getItem('delivery-details-active-panel');
    var allCollapsed = localStorage.getItem('delivery-details-all-collapsed');
    if (lastPanel) {
        $("#accordion .panel-collapse").removeClass('in');
        $("#" + lastPanel).addClass("in");
    }
    if (allCollapsed) {
        $("#accordion .panel-collapse").removeClass('in');
    }

    initRecomendationsDataTable();
    initAttachmentsDataTable();
    initDeliveryItemsDataTable();
});

function initDeliveryItemsDataTable() {
    var itemsTable = $("#items-datatable").DataTable({
        responsive: true,
        "pagingType": "numbers",
        "language": {
            "lengthMenu": "Отобразить _MENU_ позиций на странице",
            "zeroRecords": "Позиции отсутствуют",
            "info": "Отображение страницы _PAGE_ из _PAGES_",
            "infoEmpty": "Записи отсутствуют",
            "infoFiltered": "(Отфильтровано из _MAX_ записей)",
            "search": "Поиск"
        },
        "drawCallback": function () {
            $('.pagination').addClass('pagination-sm');
        }
    });

    itemsTable.columns().eq(0).each(function (colIdx) {
        $('input', $('#items-datatable th')[colIdx]).on('keyup change',
            function () {
                itemsTable
                    .column(colIdx)
                    .search(this.value)
                    .draw();
            });
    });
}