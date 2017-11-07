$(document).ready(function () {
    $.fn.select2.defaults.set('theme', 'bootstrap');

    initArticleSelect();
    initAttachmentTypeSelect();

    $('.calculate').bind('keyup mouseup',
        function () {
            var quantity = $('#Quantity').val();
            var unitPrice = $('#UnitPrice').val();
            var discount = $('#Discount').val();
            if (isNumeric(unitPrice)) {
                var grossPrice = quantity * unitPrice;
                $('#GrossPrice').val(formatDecimal(grossPrice));
            }
            if (isNumeric(unitPrice) && isNumeric(discount)) {
                var discountPrice = quantity * unitPrice - discount;
                $('#DiscountedPrice').val(formatDecimal(discountPrice));
            }
        });

    $('button[type=submit]').on('click', function () {
        var unitPrice = $('#UnitPrice').val();
        $('#UnitPrice').val(formatDecimal(unitPrice));
        var discount = $('#Discount').val();
        $('#Discount').val(formatDecimal(discount));
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
        templateResult: formatData,
        templateSelection: formatDataSelection
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

function formatData(data) {
    if (data.loading) return data.name;

    return '<div>' + data.name + '</div>' +
        '<div><small style="color: #95a5a6;">' + data.type + '</small></div>' +
        '<div><small style="color: #95a5a6;">' + data.code + '</small></div>' +
        '<div><small style="color: #95a5a6;">' + data.barcode + '</small></div>';
}

function formatDataSelection(data) {
    return data.name || data.text;
}