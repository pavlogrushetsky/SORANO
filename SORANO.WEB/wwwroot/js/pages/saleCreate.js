$(document).ready(function () {
    $.fn.select2.defaults.set('theme', 'bootstrap');

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

    initSaleItemsTree();






    $(document).on('click', '#refresh-sale-items', function(e) {
        e.preventDefault();

        $(this).tooltip('hide');

        var parameters = {
            saleId: $('#ID').val(),
            locationId: $("#LocationID").val(),
            selectedOnly: 'False'
        }

        $('#sale-items-groups').load('/Sale/Refresh', parameters, function() {
            initSaleItemsTree();
        });
    });

    $(document).on('click', '#show-selected-sale-items', function(e) {
        e.preventDefault();

        $(this).tooltip('hide');

        var icon = $(this).find('i');

        var showSelected = $('#ShowSelected').val();
        if (showSelected === 'True') {
            showSelected = 'False';
            $('#ShowSelected').val(showSelected);
            $(this).attr('data-original-title', 'Отобразить выбранные товары');
            icon.removeClass('fa-eye-slash');
            icon.addClass('fa-eye');
        } else {
            showSelected = 'True';
            $('#ShowSelected').val(showSelected);
            $(this).attr('data-original-title', 'Отобразить все товары');
            icon.removeClass('fa-eye');
            icon.addClass('fa-eye-slash');
        }            

        var parameters = {
            saleId: $('#ID').val(),
            locationId: $('#LocationID').val(),
            selectedOnly: showSelected
        }

        $('#sale-items-groups').load('/Sale/Refresh', parameters, function () {
            initSaleItemsTree();
        });
    });

    $(document).on('click', '.toggle-sale-item-recommendations', function(e) {
        e.preventDefault();

        var self = $(this);
        self.tooltip('hide');

        var recommendations = self.parent().parent().parent().find('.sale-item-recommendations');
        recommendations.toArray().forEach(function (recommendation) {
            if ($(recommendation).is(':visible')) {
                $(recommendation).hide('fast');
            } else {
                $(recommendation).show('fast');
            }
        });
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
            url: '/Sale/AddGoods',
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
                    }

                    var groupSelectedCountElement = parentGroup.find('.sale-item-group-selected-count');
                    var groupSelectedCountText = groupSelectedCountElement.text();
                    var count = parseInt(groupSelectedCountText) + 1;
                    groupSelectedCountElement.text(count.toString());


                    $('#sale-selected-count').text(response.selectedCount);
                    $('#sale-total-price').text(response.totalPrice + ' ' + response.selectedCurrency);
                }
            }
        });
    });

    $(document).on('click', 'button.add-all-goods', function(e) {
        e.preventDefault();

        var button = $(this);
        button.tooltip('hide');

        var priceInput = button.closest('tr').find('input.sale-items-group-price');
        var price = priceInput.val();
        var formattedPrice = formatDecimal(price);
        priceInput.val(formattedPrice);

        var parameters = {
            saleId: $('#ID').val(),
            goodsIds: button.data('goodsids'),
            price: price
        }

        $.ajax({
            type: 'POST',
            url: '/Sale/AddAllGoods',
            data: parameters,
            dataType: 'json',
            success: function (response) {
                if (response) {
                    button.closest('li').addClass('sale-group-selected');

                    var items = button.closest('li').find('ul > li:not(.sale-item-selected)');
                    items.toArray().forEach(function(item) {
                        var itemPriceInput = $(item).find('input.sale-item-price');
                        itemPriceInput.val(formattedPrice);

                        var itemButton = $(item).find('button.add-goods');
                        itemButton.removeClass('add-goods');
                        itemButton.addClass('remove-goods');
                        itemButton.attr('data-original-title', 'Убрать товар');

                        var itemIcon = itemButton.find('i');
                        itemIcon.removeClass('fa-cart-plus');
                        itemIcon.addClass('fa-times');
                        itemIcon.css('color', '#f39c12');

                        $(item).addClass('sale-item-selected');
                    });

                    var totalCount = button.closest('li').find('.sale-item-group-count').text();
                    button.closest('li').find('.sale-item-group-selected-count').text(totalCount);

                    $('#sale-selected-count').text(response.selectedCount);
                    $('#sale-total-price').text(response.totalPrice + ' ' + response.selectedCurrency);
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
            url: '/Sale/RemoveGoods',
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

                    var groupSelectedCountElement = parentGroup.find('.sale-item-group-selected-count');
                    var groupSelectedCountText = groupSelectedCountElement.text();
                    var count = parseInt(groupSelectedCountText) - 1;
                    groupSelectedCountElement.text(count.toString());

                    $('#sale-selected-count').text(response.selectedCount);
                    $('#sale-total-price').text(response.totalPrice + ' ' + response.selectedCurrency);
                }
            }
        });       
    });

    $(document).on('click', 'button.remove-all-goods', function (e) {
        e.preventDefault();

        var button = $(this);
        button.tooltip('hide');

        var priceInput = button.closest('tr').find('input.sale-items-group-price');
        var price = '0.00';
        priceInput.val(price);

        var parameters = {
            saleId: $('#ID').val(),
            goodsIds: button.data('goodsids')
        }

        $.ajax({
            type: 'POST',
            url: '/Sale/RemoveAllGoods',
            data: parameters,
            dataType: 'json',
            success: function (response) {
                if (response) {

                    button.closest('li').removeClass('sale-group-selected');

                    var items = button.closest('li').find('ul > li.sale-item-selected');
                    items.toArray().forEach(function (item) {
                        var itemPriceInput = $(item).find('input.sale-item-price');
                        itemPriceInput.val(price);

                        var itemButton = $(item).find('button.remove-goods');
                        itemButton.removeClass('remove-goods');
                        itemButton.addClass('add-goods');
                        itemButton.attr('data-original-title', 'Добавить товар');

                        var itemIcon = itemButton.find('i');
                        itemIcon.removeClass('fa-times');
                        itemIcon.addClass('fa-cart-plus');
                        itemIcon.css('color', '#18bc9c');

                        $(item).removeClass('sale-item-selected');
                    });

                    button.closest('li').find('.sale-item-group-selected-count').text('0');

                    $('#sale-selected-count').text(response.selectedCount);
                    $('#sale-total-price').text(response.totalPrice + ' ' + response.selectedCurrency);
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
});

function initSaleItemsTree() {
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