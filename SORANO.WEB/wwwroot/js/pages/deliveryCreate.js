$(document).ready(function () {
    $.fn.select2.defaults.set('theme', 'bootstrap');

    initLocationSelect();
    initSupplierSelect();
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

function initLocationSelect() {
    var selectLocation = $('.select-location');
    var locationId = $('#LocationID');
    var locationName = $('#LocationName');

    selectLocation.select2({
        "language": {
            "noResults": function () {
                return "Места не найдены";
            },
            "searching": function () {
                return "Поиск мест...";
            },
            "errorLoading": function () {
                return "Невозможно загрузить результаты поиска";
            }
        },
        placeholder: "Место поставки",
        minimumInputLength: 0,
        ajax: {
            url: '/Location/GetLocations',
            dataType: 'json',
            type: 'POST',
            delay: 100,
            data: function (params) {
                var queryParameters = {
                    term: params.term
                }
                return queryParameters;
            },
            results: function (data) {
                return {
                    results: $.map(data, function (item) {
                        return {
                            text: item.text,
                            id: item.id,
                            desc: item.desc
                        }
                    })
                };
            },
            cache: true
        },
        escapeMarkup: function (markup) { return markup; },
        templateResult: formatData,
        templateSelection: formatDataSelection
    });

    if (locationId.val() > 0) {
        selectLocation.append($('<option selected></option>').attr('value', locationId.val()).text(locationName.val()));
    }

    selectLocation.on("select2:opening", function () {
        selectLocation.val(null).trigger('change');
    });

    selectLocation.on("change", function () {
        var data = selectLocation.select2('data');
        if (data[0]) {
            locationId.val(data[0].id);
            locationName.val(data[0].text);
        }
        else {
            locationId.val(0);
            locationName.val('');
        }
    });
}

function initSupplierSelect() {
    var selectSupplier = $('.select-supplier');
    var supplierId = $('#SupplierID');
    var supplierName = $('#SupplierName');

    selectSupplier.select2({
        "language": {
            "noResults": function () {
                return "Поставщики не найдены";
            },
            "searching": function () {
                return "Поиск поставщиков...";
            },
            "errorLoading": function () {
                return "Невозможно загрузить результаты поиска";
            }
        },
        placeholder: "Поставщик",
        minimumInputLength: 0,
        ajax: {
            url: '/Supplier/GetSuppliers',
            dataType: 'json',
            type: 'POST',
            delay: 100,
            data: function (params) {
                var queryParameters = {
                    term: params.term
                }
                return queryParameters;
            },
            results: function (data) {
                return {
                    results: $.map(data, function (item) {
                        return {
                            text: item.text,
                            id: item.id,
                            desc: item.desc
                        }
                    })
                };
            },
            cache: true
        },
        escapeMarkup: function (markup) { return markup; },
        templateResult: formatData,
        templateSelection: formatDataSelection
    });

    if (supplierId.val() > 0) {
        selectSupplier.append($('<option selected></option>').attr('value', supplierId.val()).text(supplierName.val()));
    }

    selectSupplier.on("select2:opening", function () {
        selectSupplier.val(null).trigger('change');
    });

    selectSupplier.on("change", function () {
        var data = selectSupplier.select2('data');
        if (data[0]) {
            supplierId.val(data[0].id);
            supplierName.val(data[0].text);
        }
        else {
            supplierId.val(0);
            supplierName.val('');
        }
    });
}

function formatData(data) {
    if (data.loading) return data.text;

    return '<div>' + data.text + '</div><div><small style="color: #95a5a6;">' + data.desc + '</small></div>';
}

function formatDataSelection(data) {
    return data.text;
}

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