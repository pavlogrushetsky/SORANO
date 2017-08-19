$(document).ready(function () {
    new Morris.Bar({
        element: 'sales-barchart',
        data: [
            { month: 'Январь', value: 20 },
            { month: 'Февраль', value: 25 },
            { month: 'Март', value: 28 },
            { month: 'Апрель', value: 40 },
            { month: 'Май', value: 15 },
            { month: 'Июнь', value: 7 },
            { month: 'Июль', value: 17 },
            { month: 'Август', value: 26 },
            { month: 'Сентябрь', value: 31 },
            { month: 'Октябрь', value: 35 },
            { month: 'Ноябрь', value: 27 },
            { month: 'Декабрь', value: 24 }
        ],
        xkey: 'month',
        ykeys: ['value'],
        labels: ['Продаж'],
        grid: false,
        barColors: ["#2c3e50"],
        resize: true
    });

    new Morris.Bar({
        element: 'sales-linechart',
        data: [
            { month: 'Январь', value: 1020 },
            { month: 'Февраль', value: 1000 },
            { month: 'Март', value: 1500 },
            { month: 'Апрель', value: 2000 },
            { month: 'Май', value: 2200 },
            { month: 'Июнь', value: 2500 },
            { month: 'Июль', value: 2000 },
            { month: 'Август', value: 2700 },
            { month: 'Сентябрь', value: 3200 },
            { month: 'Октябрь', value: 3500 },
            { month: 'Ноябрь', value: 3750 },
            { month: 'Декабрь', value: 4100 }
        ],
        xkey: 'month',
        ykeys: ['value'],
        labels: ['Сумма'],
        grid: false,
        barColors: ["#999999"],
        resize: true
    });

    new Morris.Donut({
        element: 'sales-piechart',
        data: [
            { label: 'Место №1', value: 75 },
            { label: 'Место №2', value: 25 }
        ],
        colors: ["#2c3e50", "#999999", "#95a5a6", "#b4bcc2", "#cccccc"],
        resize: true
    });

    initSalesDataTable();
});

function initSalesDataTable() {
    var table = $("#sales-datatable").DataTable({
        responsive: true,
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

    table.columns().eq(0).each(function (colIdx) {
        $('input', $('#sales-datatable th')[colIdx]).on('keyup change', function () {
            table
                .column(colIdx)
                .search(this.value)
                .draw();
        });
    });
}