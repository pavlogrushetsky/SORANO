//$(window).load(function() {
//    var posReader = localStorage["posStorage"];
//    if (posReader) {
//        $(document).scrollTop(posReader);
//        localStorage.removeItem("posStorage");
//    }
//});

$(document).ready(function () {
    $.fn.select2.defaults.set('theme', 'bootstrap');

    //$('button.add-goods, button.remove-goods').on('click', function() {
    //    localStorage["posStorage"] = $(document).scrollTop();
    //});

    initAttachmentTypeSelect();

    initDateTimePicker('#pick-date');

    initGenericSelect({
        selectElementClass: '.select-location',
        valueElementId: '#LocationID',
        displayElementId: '#LocationName',
        noResultsText: 'Магазины не найдены',
        searchingText: 'Поиск магазинов...',
        errorLoadingText: 'Невозможно загрузить результаты поиска',
        placeholderText: 'Магазин',
        url: '/Location/GetLocations'
    });

    initGenericSelect({
        selectElementClass: '.select-client',
        valueElementId: '#ClientID',
        displayElementId: '#ClientName',
        noResultsText: 'Клиенты не найдены',
        searchingText: 'Поиск клиентов...',
        errorLoadingText: 'Невозможно загрузить результаты поиска',
        placeholderText: 'Клиент',
        url: '/Client/GetClients'
    });

    initGoodsSelect();

    //for (var i = 0, len = localStorage.length; i < len; ++i) {
    //    var key = '#' + localStorage.key(i);
    //    if (key.startsWith('#sale_group_')) {
    //        var children = $(key).parent('li').find(' > ul > li');
    //        var value = localStorage.getItem(localStorage.key(i));
    //        if (!children.is(':visible') && value === 'true') {
    //            children.show('fast');
    //            $(key).attr('title', 'Свернуть ветку');
    //            $(key).find(' > table').hide('fast');
    //        }
    //    }
    //}   

    $('.sale-items-groups-tree > ul > li > table').attr('title', 'Свернуть ветку');
    $('.sale-items-groups-tree > ul > li > table').on('click', function (e) {       
        var target = $(e.target);       
        if (!target.is("input") && !target.is('button') && !target.is('i')) {
            var children = $(this).parent('li').find(' > ul > li');
            if (children.is(":visible")) {
                //localStorage.setItem($(this).attr('id'), 'false');
                children.hide('fast');
                $(this).attr('title', 'Развернуть ветку');
                $(this).find(' > table').show('fast');
            } else {
                //localStorage.setItem($(this).attr('id'), 'true');
                children.show('fast');
                $(this).attr('title', 'Свернуть ветку');
                $(this).find(' > table').hide('fast');
            }
            e.stopPropagation();
        }
    });









    $(document).on('click', 'button.add-goods', function (e) {
        e.preventDefault();

        var button = $(this);
        button.tooltip('hide');

        var input = button.closest('tr').find('input.sale-item-price');
        var price = input.val();
        input.val(formatDecimal(price));

        var parameters = {
            saleId: $('#ID').val(),
            goodsId: button.data('goodsid'),
            price: price
        }

        $.ajax({
            type: 'POST',
            url: '/Sale/AddGoods_1',
            data: parameters,
            dataType: 'json',
            success: function (response) {
                if (response) {

                    button.removeClass('add-goods');
                    button.addClass('remove-goods');
                    button.attr('data-original-title', 'Убрать товар');

                    var icon = button.find('i');
                    icon.removeClass('fa-cart-plus');
                    icon.addClass('fa-times');
                    icon.css('color', '#f39c12');

                    button.closest('li').addClass('sale-item-selected');

                    var parentGroup = button.closest('li').closest('ul').closest('li');
                    var notSelected = parentGroup.find('ul > li:not(.sale-item-selected)');
                    if (notSelected.length === 0) {
                        parentGroup.addClass('sale-group-selected');
                        var parentGroupButton = parentGroup.find('button.remove-all-goods, button.add-all-goods');
                        parentGroupButton.attr('data-original-title', 'Убрать все товары');
                        var parentGroupIcon = parentGroupButton.find('i');
                        parentGroupIcon.removeClass('fa-cart-plus');
                        parentGroupIcon.addClass('fa-times');
                        parentGroupIcon.css('color', '#f39c12');
                    }
                }
            }
        });
    });

    $(document).on('click', 'button.remove-goods', function (e) {
        e.preventDefault();

        var button = $(this);
        button.tooltip('hide');

        var input = button.closest('tr').find('input.sale-item-price');
        input.val('0.00');

        var parameters = {
            saleId: $('#ID').val(),
            goodsId: button.data('goodsid')
        }

        $.ajax({
            type: 'POST',
            url: '/Sale/RemoveGoods_1',
            data: parameters,
            dataType: 'json',
            success: function (response) {
                if (response) {

                    button.removeClass('remove-goods');
                    button.addClass('add-goods');
                    button.attr('data-original-title', 'Добавить товар');

                    var icon = button.find('i');
                    icon.removeClass('fa-times');
                    icon.addClass('fa-cart-plus');
                    icon.css('color', '#18bc9c');

                    button.closest('li').removeClass('sale-item-selected');

                    var parentGroup = button.closest('li').closest('ul').closest('li');
                    parentGroup.removeClass('sale-group-selected');
                    var parentGroupButton = parentGroup.find('button.remove-all-goods, button.add-all-goods');
                    parentGroupButton.attr('data-original-title', 'Добавить все товары');
                    var parentGroupIcon = parentGroupButton.find('i');
                    parentGroupIcon.removeClass('fa-times');
                    parentGroupIcon.addClass('fa-cart-plus');
                    parentGroupIcon.css('color', '#18bc9c');
                }
            }
        });
    });











    //initSaleItemsDataTable();   

    var allowChangeLocation = $("#AllowChangeLocation");
    if (allowChangeLocation.val() !== 'True')
        $(".select-location").attr('disabled', 'disabled');

    $('input[type=radio][name=select_currency]').change(function () {
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

    //$('.sale-items-groups-tree > ul > li > table').attr('title', 'Свернуть ветку');
    //$('.sale-items-groups-tree > ul > li > table').on('click', function (e) {
    //    var target = $(e.target);
    //    if (!target.is("input") && !target.is('button') && !target.is('i')) {
    //        var children = $(this).parent('li').find(' > ul > li');
    //        if (children.is(":visible")) {
    //            children.hide('fast');
    //            $(this).attr('title', 'Развернуть ветку');
    //            $(this).find(' > table').show('fast');
    //        } else {
    //            children.show('fast');
    //            $(this).attr('title', 'Свернуть ветку');
    //            $(this).find(' > table').hide('fast');
    //        }
    //        e.stopPropagation();
    //    }        
    //});
});

function initGoodsSelect() {
    var selectElement = $(".select-goods");

    selectElement.select2({
        "language": {
            "noResults": function () {
                return "Товары не найдены";
            },
            "searching": function () {
                return "Поиск товаров...";
            },
            "errorLoading": function () {
                return "Невозможно загрузить результаты поиска";
            }
        },
        placeholder: "Товар",
        minimumInputLength: 0,
        ajax: {
            url: '/Goods/GetGoods',
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

function initSaleItemsDataTable() {
    var table = $("#sale-items-datatable").DataTable({
        responsive: true,
        "autoWidth": false,
        "scrollX": false,
        "columnDefs": [
            { "orderable": false, "targets": -1 },
            { "width": "10%", "targets": 0 },
            { "width": "35%", "targets": 1 },
            { "width": "15%", "targets": 2 },
            { "width": "15%", "targets": 3 },
            { "width": "15%", "targets": 4 },
            { "width": "10%", "targets": 5 }
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
        $('input', $('#sale-items-datatable th')[colIdx]).on('keyup change', function () {
            table
                .column(colIdx)
                .search(this.value)
                .draw();
        });
    });
}