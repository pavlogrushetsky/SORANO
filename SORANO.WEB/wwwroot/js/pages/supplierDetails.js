﻿$(document).ready(function () {
    $('#accordion').on('shown.bs.collapse', function () {
        var active = $("#accordion .in").attr('id');
        localStorage.setItem('article-details-active-panel', active);
        localStorage.removeItem('article-details-all-collapsed');
    });
    $('#accordion').on('hidden.bs.collapse', function () {
        localStorage.setItem('article-details-all-collapsed', true);
        localStorage.removeItem('article-details-active-panel');
    });

    var lastPanel = localStorage.getItem('article-details-active-panel');
    var allCollapsed = localStorage.getItem('article-details-all-collapsed');
    if (lastPanel) {
        $("#accordion .panel-collapse").removeClass('in');
        $("#" + lastPanel).addClass("in");
    }
    if (allCollapsed) {
        $("#accordion .panel-collapse").removeClass('in');
    }

    initRecomendationsDataTable();
    initAttachmentsDataTable();
    initDeliveriesDataTable();
});

function initDeliveriesDataTable() {
    var deliveriesTable = $("#deliveries-datatable").DataTable({
        responsive: true,
        "pagingType": "numbers",
        "language": {
            "lengthMenu": "Отобразить _MENU_ поставок на странице",
            "zeroRecords": "Поставки отсутствуют",
            "info": "Отображение страницы _PAGE_ из _PAGES_",
            "infoEmpty": "Записи отсутствуют",
            "infoFiltered": "(Отфильтровано из _MAX_ записей)",
            "search": "Поиск"
        },
        "drawCallback": function () {
            $('.pagination').addClass('pagination-sm');
        }
    });

    deliveriesTable.columns().eq(0).each(function (colIdx) {
        $('input', $('#items-datatable th')[colIdx]).on('keyup change',
            function () {
                deliveriesTable
                    .column(colIdx)
                    .search(this.value)
                    .draw();
            });
    });
}