$(document).ready(function () {
    $.fn.select2.defaults.set('theme', 'bootstrap');

    initFilter();

    $(document).on('submit', '#goods-filter-form', clickFilter);
    $(document).on('reset', '#goods-filter-form', clickResetFilter);
    $(document).on('click', '.goods-expand', clickExpand);
    $(document).on('click', '.add-to-cart', clickAddToCart);
    $(document).on('click', '.add-to-cart.btn-success', clickSubmitAddToCart);
});

function clickSubmitAddToCart(e) {
    e.preventDefault();

    var button = $(this);

    button.tooltip('hide');

    var parameters = {
        model: {
            ArticleID: $('#ArticleID').val(),
            ArticleTypeID: $('#ArticleTypeID').val(),
            LocationID: $('#LocationID').val(),
            SearchTerm: $('#SearchTerm').val(),
            Status: $('#Status').val(),
            ShowByPiece: $('#ShowByPiece').val(),
            ShowNumber: $('#ShowNumber').val()
        },
        goods: button.data('goods'),
        saleId: button.data('saleid')
    }

    $('.goods-cards-row').load('/Goods/AddToCart', parameters, function () {
        initFilter();
        initTooltip();
    });
}

function clickAddToCart(e) {
    e.preventDefault();

    var button = $(this);

    button.tooltip('hide');
    button.removeClass('btn-default');
    button.addClass('btn-success');
    button.attr('disabled', 'disabled');
    button.attr('data-original-title', 'Подтвердить добавление в корзину');

    var icon = button.find('i');

    icon.removeClass('fa-cart-arrow-down');
    icon.addClass('fa-check');  

    var row = button.closest('table').find('.row-add-to-cart');
    row.show('fast');

    var input = row.find('.select-sale');
    input.select2({
        "language": {
            "noResults": function () {
                return "Корзины не найдены";
            },
            "searching": function () {
                return "Поиск корзин...";
            },
            "errorLoading": function () {
                return "Невозможно загрузить результаты поиска";
            }
        },
        placeholder: "Корзина",
        minimumInputLength: 0,
        ajax: {
            url: '/Sale/GetSales',
            dataType: 'json',
            type: 'POST',
            delay: 100,
            data: function (params) {
                var queryParameters = {
                    term: params.term,
                    locationId: button.data('locationid')
                }
                return queryParameters;
            },
            results: function (data) {
                return {
                    results: $.map(data, function (item) {
                        return {
                            id: item.id,
                            text: item.text
                        }
                    })
                };
            },
            cache: true
        }
    });     

    input.on("select2:opening", function () {
        input.val(null).trigger('change');
    });

    input.on("change", function () {
        var data = input.select2('data');
        if (data[0]) {
            button.removeAttr('disabled');
            button.attr('data-saleid', data[0].id);
        }
        else {
            button.attr('disabled', 'disabled');
            button.attr('data-saleid', '');
        }
    });
}

function initFilter() {
    initArticleSelect();
    initArticleTypeSelect();
    initGenericSelect({
        selectElementClass: '.select-location',
        valueElementId: '#LocationID',
        displayElementId: '#LocationName',
        noResultsText: 'Склады не найдены',
        searchingText: 'Поиск складов...',
        errorLoadingText: 'Невозможно загрузить результаты поиска',
        placeholderText: 'Склад',
        url: '/Location/GetLocations'
    });
}

function clickFilter(e) {
    e.preventDefault();

    var parameters = {
        ArticleID: $('#ArticleID').val(),
        ArticleTypeID: $('#ArticleTypeID').val(),
        LocationID: $('#LocationID').val(),
        SearchTerm: $('#SearchTerm').val(),
        Status: $('#Status').val(),
        ShowByPiece: $('#ShowByPiece').val(),
        ShowNumber: $('#ShowNumber').val()
    };

    $('.goods-cards-row').load('/Goods/Filter', parameters, function () {
        $('.panel').removeClass('panel-default');
        $('.panel').addClass('panel-info');
        initFilter();
        initTooltip();
    });
}

function clickResetFilter(e) {
    e.preventDefault();

    $('.goods-cards-row').load('/Goods/ClearFilter', function () {
        $('.panel').removeClass('panel-info');
        $('.panel').addClass('panel-default');
        initFilter();
        resetFilter();
        initTooltip();
    });
}

function resetFilter() {    
    $('#SearchTerm').val("");
    $('#Status').val(0);
    $('#ShowByPiece').val("false");
    $('#ShowNumber').val(10);    
    $('.select-article').val(null).trigger('change');
    $('.select-article-type').val(null).trigger('change');   

    var allowChangeLocation = $('#AllowChangeLocation');
    if (allowChangeLocation.val() === 'True') {
        $('.select-location').val(null).trigger('change');
    }
}

function clickExpand(e) {
    e.preventDefault(e);

    var button = $(this);

    button.tooltip('hide');

    var articleId = button.data('articleid');
    var articleTypeId = button.data('articletypeid');
    var locationId = button.data('locationid');
    var status = button.data('status');

    $('#ArticleID').val(articleId);
    $('#ArticleName').val(button.data('articlename'));
    $('#ArticleTypeID').val(articleTypeId);
    $('#ArticleTypeName').val(button.data('articletypename'));
    $('#LocationID').val(locationId);
    $('#LocationName').val(button.data('locationname'));
    $('#Status').val(status);
    $('#ShowByPiece').val('true');

    var parameters = {
        ArticleID: articleId,
        ArticleTypeID: articleTypeId,
        LocationID: locationId,
        Status: status,
        ShowByPiece: true,
        SearchTerm: $('#SearchTerm').val(),
        ShowNumber: $('#ShowNumber').val()
    }

    $('.goods-cards-row').load('/Goods/Expand', parameters, function () {
        $('.panel').removeClass('panel-default');
        $('.panel').addClass('panel-info');
        initFilter();
        initTooltip();
    });
}

function initArticleSelect() {
    var selectArticle = $('.select-article');
    var articleId = $('#ArticleID');
    var articleName = $('#ArticleName');
    selectArticle.select2({
        "language": {
            "noResults": function () {
                return "Артикулы не найдены";
            },
            "searching": function () {
                return "Поиск артикулов...";
            },
            "errorLoading": function () {
                return "Невозможно загрузить результаты поиска";
            }
        },
        placeholder: "Артикул",
        minimumInputLength: 0,
        ajax: {
            url: '/Article/GetArticles',
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
                            id: item.id,
                            text: item.text,
                            type: item.type,
                            code: item.code,
                            barcode: item.barcode
                        }
                    })
                };
            },
            cache: true
        },
        escapeMarkup: function (markup) { return markup; },
        templateResult: formatArticleData,
        templateSelection: formatArticleDataSelection
    });

    if (articleId.val() > 0) {
        selectArticle.append($('<option selected></option>').attr('value', articleId.val()).text(articleName.val()));
    }

    selectArticle.on("select2:opening", function () {
        selectArticle.val(null).trigger('change');
    });

    selectArticle.on("change", function () {
        var data = selectArticle.select2('data');
        if (data[0]) {
            articleId.val(data[0].id);
            articleName.val(data[0].text);
        }
        else {
            articleId.val(0);
            articleName.val('');
        }
    });
}

function initArticleTypeSelect() {
    var selectArticleType = $('.select-article-type');
    var articleTypeId = $('#ArticleTypeID');
    var articleTypeName = $('#ArticleTypeName');

    selectArticleType.select2({
        "language": {
            "noResults": function () {
                return "Типы не найдены";
            },
            "searching": function () {
                return "Поиск типов...";
            },
            "errorLoading": function () {
                return "Невозможно загрузить результаты поиска";
            }
        },
        placeholder: "Тип",
        minimumInputLength: 0,
        ajax: {
            url: '/ArticleType/GetArticleTypes',
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
                            desc: item.desc,
                            parent: item.parent
                        }
                    })
                };
            },
            cache: true
        },
        escapeMarkup: function (markup) { return markup; },
        templateResult: formatArticleTypeData,
        templateSelection: formatArticleTypeDataSelection
    });

    if (articleTypeId.val() > 0) {
        selectArticleType.append($('<option selected></option>').attr('value', articleTypeId.val()).text(articleTypeName.val()));
    }

    selectArticleType.on("select2:opening", function () {
        selectArticleType.val(null).trigger('change');
    });

    selectArticleType.on("change", function () {
        var data = selectArticleType.select2('data');
        if (data[0]) {
            articleTypeId.val(data[0].id);
            articleTypeName.val(data[0].text);
        }
        else {
            articleTypeId.val(0);
            articleTypeName.val('');
        }
    });
}

function formatArticleData(data) {
    if (data.loading) return data.text;

    return '<div>' + data.text + '</div>' +
        '<div><small style="color: #95a5a6;">' + data.type + '</small></div>' +
        '<div><small style="color: #95a5a6;">' + data.code + '</small></div>' +
        '<div><small style="color: #95a5a6;">' + data.barcode + '</small></div>';
}

function formatArticleDataSelection(data) {
    return data.text;
}

function formatArticleTypeData(data) {
    if (data.loading) return data.text;

    return '<div>' + data.parent + '</div><div><small style="color: #95a5a6;">' + data.desc + '</small></div>';
}

function formatArticleTypeDataSelection(data) {
    return data.text;
}