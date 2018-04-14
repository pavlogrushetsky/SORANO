$(document).ready(function () {
    $.fn.select2.defaults.set('theme', 'bootstrap');
    initAttachmentTypeSelect();

    initGenericSelect({
        selectElementClass: '.select-location',
        valueElementId: '#LocationID',
        displayElementId: '#LocationName',
        noResultsText: 'Места не найдены',
        searchingText: 'Поиск мест...',
        errorLoadingText: 'Невозможно загрузить результаты поиска',
        placeholderText: 'Место поставки',
        url: '/Location/GetLocations'
    });

    initGenericSelect({
        selectElementClass: '.select-supplier',
        valueElementId: '#SupplierID',
        displayElementId: '#SupplierName',
        noResultsText: 'Поставщики не найдены',
        searchingText: 'Поиск поставщиков...',
        errorLoadingText: 'Невозможно загрузить результаты поиска',
        placeholderText: 'Поставщик',
        url: '/Supplier/GetSuppliers'
    });

    initDeliveryItemsDataTable();

    $('.submit-delivery').on('click', function () {
        $('#IsSubmitted').val(true);
    });

    $('#pick-delivery-date').datetimepicker({
        locale: 'ru',
        format: 'DD.MM.YYYY',
        showTodayButton: true,
        showClear: true,
        showClose: true,
        icons: {
            clear: 'fa fa-trash',
            close: 'fa fa-times',
            today: 'fa fa-calendar-check-o'
        },
        tooltips: {
            today: 'Текущая дата',
            clear: 'Очистить выбор',
            close: 'Закрыть окно',
            nextMonth: 'Следующий месяц',
            prevMonth: 'Предыдущий месяц',
            selectMonth: 'Выбрать месяц',
            selectYear: 'Выбрать год',
            selectDecade: 'Выбрать десятилетие',
            nextYear: 'Следующий год',
            nextDecade: 'Следующее десятилетие',
            prevYear: 'Предыдущий год',
            prevDecade: 'Предыдущее десятилетие'
        }
    });

    $('#pick-payment-date').datetimepicker({
        locale: 'ru',
        format: 'DD.MM.YYYY',
        showTodayButton: true,
        showClear: true,
        showClose: true,
        icons: {
            clear: 'fa fa-trash',
            close: 'fa fa-times',
            today: 'fa fa-calendar-check-o'
        },
        tooltips: {
            today: 'Текущая дата',
            clear: 'Очистить выбор',
            close: 'Закрыть окно',
            nextMonth: 'Следующий месяц',
            prevMonth: 'Предыдущий месяц',
            selectMonth: 'Выбрать месяц',
            selectYear: 'Выбрать год',
            selectDecade: 'Выбрать десятилетие',
            nextYear: 'Следующий год',
            nextDecade: 'Следующее десятилетие',
            prevYear: 'Предыдущий год',
            prevDecade: 'Предыдущее десятилетие'
        }
    });

    $('input[type=radio][name=select_currency]').change(function() {
        var value = $(this).val();
        if (value === '0') {
            $('#DollarRate').val('');
            $('#EuroRate').val('');
            $('#DollarRate').prop('readonly', true);
            $('#EuroRate').prop('readonly', true);    
            $('#SelectedCurrency').val('₴');
        } else if (value === '1') {
            $('#DollarRate').prop('readonly', false);
            $('#DollarRate').val('0.00');
            $('#EuroRate').val('');
            $('#EuroRate').prop('readonly', true);
            $('#SelectedCurrency').val('$');
        } else if (value === '2') {
            $('#EuroRate').prop('readonly', false);
            $('#EuroRate').val('0.00');
            $('#DollarRate').val('');
            $('#DollarRate').prop('readonly', true);
            $('#SelectedCurrency').val('€');
        }        
    });

    updateCurrencyRates();
});

function initDeliveryItemsDataTable() {
    var table = $("#delivery-items-datatable").DataTable({
        responsive: true,
        "autoWidth": false,
        "scrollX": false,
        "columnDefs": [
            { "orderable": false, "targets": 7 }
        ],
        "order": [[0, "asc"]],
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

    table.columns().eq(0).each(function (colIdx) {
        $('input', $('#delivery-items-datatable th')[colIdx]).on('keyup change', function () {
            table
                .column(colIdx)
                .search(this.value)
                .draw();
        });
    });
}

function updateCurrencyRates() {
    var value = $('#SelectedCurrency').val();
    if (value === '₴') {
        $('#DollarRate').val('');
        $('#EuroRate').val('');
        $('#DollarRate').prop('readonly', true);
        $('#EuroRate').prop('readonly', true);
        $('#select_currency_uah').prop('checked', true);
        $('#select_currency_usd').prop('checked', false);
        $('#select_currency_eur').prop('checked', false);
    } else if (value === '$') {
        $('#DollarRate').prop('readonly', false);
        $('#EuroRate').val('');
        $('#EuroRate').prop('readonly', true);
        $('#select_currency_uah').prop('checked', false);
        $('#select_currency_usd').prop('checked', true);
        $('#select_currency_eur').prop('checked', false);
    } else if (value === '€') {
        $('#EuroRate').prop('readonly', false);
        $('#DollarRate').val('');
        $('#DollarRate').prop('readonly', true);
        $('#select_currency_uah').prop('checked', false);
        $('#select_currency_usd').prop('checked', false);
        $('#select_currency_eur').prop('checked', true);
    }
}