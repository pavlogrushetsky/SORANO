$(document).ready(function () {
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

    var recommendationsTable = $("#recommendations-datatable").DataTable({
        responsive: true,
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

    recommendationsTable.columns().eq(0).each(function (colIdx) {
        $('input', $('#recommendations-datatable th')[colIdx]).on('keyup change', function () {
            recommendationsTable
                .column(colIdx)
                .search(this.value)
                .draw();
        });
    });

    var attachmentsTable = $("#attachments-datatable").DataTable({
        responsive: true,
        "columnDefs": [
            { "orderable": false, "targets": 2 }
        ],
        "pagingType": "numbers",
        "language": {
            "lengthMenu": "Отобразить _MENU_ вложений на странице",
            "zeroRecords": "Вложения отсутствуют",
            "info": "Отображение страницы _PAGE_ из _PAGES_",
            "infoEmpty": "Записи отсутствуют",
            "infoFiltered": "(Отфильтровано из _MAX_ записей)",
            "search": "Поиск"
        },
        "drawCallback": function () {
            $('.pagination').addClass('pagination-sm');
        }
    });

    attachmentsTable.columns().eq(0).each(function (colIdx) {
        $('input', $('#attachments-datatable th')[colIdx]).on('keyup change', function () {
            attachmentsTable
                .column(colIdx)
                .search(this.value)
                .draw();
        });
    });

    var articlesTable = $("#article-datatable").DataTable({
        responsive: true,
        "pagingType": "numbers",
        "language": {
            "lengthMenu": "Отобразить _MENU_ артикулов на странице",
            "zeroRecords": "Артикулы отсутствуют",
            "info": "Отображение страницы _PAGE_ из _PAGES_",
            "infoEmpty": "Записи отсутствуют",
            "infoFiltered": "(Отфильтровано из _MAX_ записей)",
            "search": "Поиск"
        },
        "drawCallback": function () {
            $('.pagination').addClass('pagination-sm');
        }
    });

    articlesTable.columns().eq(0).each(function (colIdx) {
        $('input', $('#article-datatable th')[colIdx]).on('keyup change',
            function () {
                articlesTable
                    .column(colIdx)
                    .search(this.value)
                    .draw();
            });
    });
});