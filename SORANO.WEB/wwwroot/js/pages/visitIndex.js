$(document).ready(function () { 
    loadVisits();

    $(document).on('click', '#refresh-visits', function(e) {
        e.preventDefault();
        $(this).tooltip('hide');
        loadVisits();        
    });

    $(document).on('click', '#btn-add-visit', function () {
        $.get('/Visit/Create', function (data) {
            $('#visitFormBody').html(data);
            initVisitModal();
            $('#visitFormModal').modal('show');
        });
    });
});

function loadVisits() {
    $('#progress-bar').animate({ opacity: 1.0 }, 100, function() {
        $('#visits').load('/Visit/Table', function () {
            $('#visits').show();
            initVisitsDataTable();
            initTooltip();
            $('#progress-bar').animate({ opacity: 0.0 }, 100);
        });
    });   
}

function initVisitsDataTable() {
    var table = $("#visits-datatable").DataTable({
        responsive: true,
        "autoWidth": false,
        "scrollX": false,
        "deferRender": true,
        "aaSorting": [],
        "columnDefs": [
            { "orderable": false, "targets": -1 }
        ],
        "pagingType": "numbers",
        "language": {
            "lengthMenu": "Отобразить _MENU_ посещений на странице",
            "zeroRecords": "Посещения отсутствуют",
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
        $('input', $('th')[colIdx]).on('keyup change',
            function () {
                table
                    .column(colIdx)
                    .search(this.value)
                    .draw();
            });
    });
}