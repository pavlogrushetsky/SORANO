$(document).ready(function () {
    $.fn.select2.defaults.set('theme', 'bootstrap');

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
        $('#Status').val(true);
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
        } else if (value === '1') {
            $('#DollarRate').prop('readonly', false);
            $('#DollarRate').val('0.00');
            $('#EuroRate').val('');
            $('#EuroRate').prop('readonly', true);
        } else if (value === '2') {
            $('#EuroRate').prop('readonly', false);
            $('#EuroRate').val('0.00');
            $('#DollarRate').val('');
            $('#DollarRate').prop('readonly', true);
        }
        $('#SelectedCurrency').val(value);
    });

    $('.delivery_item_quantity').bind('keyup mouseup',
        function() {
            var id = $(this).attr('id');
            var quantity = $(this).val();
            if (isNumeric(quantity)) {
                var matches = id.match(/\d+$/);
                if (matches) {
                    var number = matches[0];
                    var unitPrice = $('#delivery_item_unitprice_' + number).val();
                    var discount = $('#delivery_item_discount_' + number).val();
                    if (isNumeric(unitPrice) && isNumeric(discount)) {
                        var grossPrice = quantity * unitPrice;
                        var discountPrice = grossPrice - discount;
                        $('#DeliveryItems_' + number + '__GrossPrice').val(formatDecimal(grossPrice));
                        $('#DeliveryItems_' + number + '__DiscountPrice').val(formatDecimal(discountPrice));
                        updateTotalGrossPrice();
                        updateTotalDiscountPrice();
                    }
                }
            }            
        });

    $('.delivery_item_unitprice').bind('keyup mouseup',
        function () {
            var id = $(this).attr('id');
            var unitPrice = $(this).val();
            if (isNumeric(unitPrice)) {
                var matches = id.match(/\d+$/);
                if (matches) {
                    var number = matches[0];
                    var quantity = $('#delivery_item_quantity_' + number).val();
                    var discount = $('#delivery_item_discount_' + number).val();
                    if (isNumeric(quantity) && isNumeric(discount)) {
                        var grossPrice = quantity * unitPrice;
                        var discountPrice = grossPrice - discount;
                        $('#DeliveryItems_' + number + '__GrossPrice').val(formatDecimal(grossPrice));
                        $('#DeliveryItems_' + number + '__DiscountPrice').val(formatDecimal(discountPrice));
                        updateTotalGrossPrice();
                        updateTotalDiscountPrice();
                    }
                }
            }
        });

    $('.delivery_item_discount').bind('keyup mouseup',
        function () {
            var id = $(this).attr('id');
            var discount = $(this).val();
            if (isNumeric(discount)) {
                var matches = id.match(/\d+$/);
                if (matches) {
                    var number = matches[0];
                    var quantity = $('#delivery_item_quantity_' + number).val();
                    var unitPrice = $('#delivery_item_unitprice_' + number).val();
                    if (isNumeric(quantity) && isNumeric(unitPrice)) {
                        var grossPrice = quantity * unitPrice;
                        var discountPrice = grossPrice - discount;
                        $('#DeliveryItems_' + number + '__DiscountPrice').val(formatDecimal(discountPrice));
                        updateTotalDiscount();
                        updateTotalDiscountPrice();
                    }
                }
            }
        });

    updateCurrencyRates();

    updateTotalGrossPrice();
    updateTotalDiscount();
    updateTotalDiscountPrice();
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
    if (value === '0') {
        $('#DollarRate').val('');
        $('#EuroRate').val('');
        $('#DollarRate').prop('readonly', true);
        $('#EuroRate').prop('readonly', true);
        $('#select_currency_uah').prop('checked', true);
        $('#select_currency_usd').prop('checked', false);
        $('#select_currency_eur').prop('checked', false);
    } else if (value === '1') {
        $('#DollarRate').prop('readonly', false);
        $('#EuroRate').val('');
        $('#EuroRate').prop('readonly', true);
        $('#select_currency_uah').prop('checked', false);
        $('#select_currency_usd').prop('checked', true);
        $('#select_currency_eur').prop('checked', false);
    } else if (value === '2') {
        $('#EuroRate').prop('readonly', false);
        $('#DollarRate').val('');
        $('#DollarRate').prop('readonly', true);
        $('#select_currency_uah').prop('checked', false);
        $('#select_currency_usd').prop('checked', false);
        $('#select_currency_eur').prop('checked', true);
    }
}

function updateTotalGrossPrice() {
    var totalGrossPrice = 0;
    $('.delivery_item_grossprice').each(function() {
        var value = $(this).val();
        if (isNumeric(value)) {
            totalGrossPrice += parseFloat(value);
        }
    });
    $('#TotalGrossPrice').val(formatDecimal(totalGrossPrice));
    $('#delivery_totalgrossprice').text(formatDecimal(totalGrossPrice));
}

function updateTotalDiscount() {
    var totalDiscount = 0;
    $('.delivery_item_discount').each(function() {
        var value = $(this).val();
        if (isNumeric(value)) {
            totalDiscount += parseFloat(value);
        }
    });
    $('#TotalDiscount').val(formatDecimal(totalDiscount));
    $('#delivery_totaldiscount').text(formatDecimal(totalDiscount));
}

function updateTotalDiscountPrice() {
    var totalDiscountPrice = 0;
    $('.delivery_item_discountprice').each(function () {
        var value = $(this).val();
        if (isNumeric(value)) {
            totalDiscountPrice += parseFloat(value);
        }
    });
    $('#TotalDiscountPrice').val(formatDecimal(totalDiscountPrice));
    $('#delivery_totaldiscountprice').text(formatDecimal(totalDiscountPrice));
}

function getMimeType(num) {
    getMimeType(num, "Delivery/GetMimeType?id=");
}