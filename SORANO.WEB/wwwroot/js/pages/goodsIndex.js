$(document).ready(function () {
    $.fn.select2.defaults.set('theme', 'bootstrap');

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
});

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
                            name: item.name,
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
            articleName.val(data[0].name);
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
    if (data.loading) return data.name;

    return '<div>' + data.name + '</div>' +
        '<div><small style="color: #95a5a6;">' + data.type + '</small></div>' +
        '<div><small style="color: #95a5a6;">' + data.code + '</small></div>' +
        '<div><small style="color: #95a5a6;">' + data.barcode + '</small></div>';
}

function formatArticleDataSelection(data) {
    return data.name || data.text;
}

function formatArticleTypeData(data) {
    if (data.loading) return data.text;

    return '<div>' + data.parent + '</div><div><small style="color: #95a5a6;">' + data.desc + '</small></div>';
}

function formatArticleTypeDataSelection(data) {
    return data.text;
}