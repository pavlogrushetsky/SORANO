$(document).ready(function () {
    initUsersDataTable();
});

function initUsersDataTable() {
    var table = $("#users-datatable").DataTable({
        responsive: true,
        "columnDefs": [
            { "orderable": false, "targets": 3 },
            { "orderable": false, "targets": 5 }
        ],
        "pagingType": "numbers",
        "language": {
            "lengthMenu": "Отобразить _MENU_ пользователей на странице",
            "zeroRecords": "Пользователи отсутствуют",
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
        $('input', $('th')[colIdx]).on('keyup change', function () {
            table
                .column(colIdx)
                .search(this.value)
                .draw();
        });
    });
}