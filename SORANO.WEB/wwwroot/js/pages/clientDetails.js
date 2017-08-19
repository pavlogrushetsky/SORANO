$(document).ready(function () {
    $('#accordion').on('shown.bs.collapse', function () {
        var active = $("#accordion .in").attr('id');
        localStorage.setItem('client-details-active-panel', active);
        localStorage.removeItem('client-details-all-collapsed');
    });
    $('#accordion').on('hidden.bs.collapse', function () {
        localStorage.setItem('client-details-all-collapsed', true);
        localStorage.removeItem('client-details-active-panel');
    });

    var lastPanel = localStorage.getItem('client-details-active-panel');
    var allCollapsed = localStorage.getItem('client-details-all-collapsed');
    if (lastPanel) {
        $("#accordion .panel-collapse").removeClass('in');
        $("#" + lastPanel).addClass("in");
    }
    if (allCollapsed) {
        $("#accordion .panel-collapse").removeClass('in');
    }

    initRecomendationsDataTable();
    initAttachmentsDataTable();
    initPurchasesDataTable();
});

function initPurchasesDataTable() {
    var purchasesTable = $("#purchases-datatable").DataTable({
        responsive: true,
        "pagingType": "numbers",
        "language": {
            "lengthMenu": "Отобразить _MENU_ покупок на странице",
            "zeroRecords": "Покупки отсутствуют",
            "info": "Отображение страницы _PAGE_ из _PAGES_",
            "infoEmpty": "Записи отсутствуют",
            "infoFiltered": "(Отфильтровано из _MAX_ записей)",
            "search": "Поиск"
        },
        "drawCallback": function () {
            $('.pagination').addClass('pagination-sm');
        }
    });

    purchasesTable.columns().eq(0).each(function (colIdx) {
        $('input', $('#purchases-datatable th')[colIdx]).on('keyup change',
            function () {
                purchasesTable
                    .column(colIdx)
                    .search(this.value)
                    .draw();
            });
    });
}