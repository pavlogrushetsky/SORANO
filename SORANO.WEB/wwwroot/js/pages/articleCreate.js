$(document).ready(function () {
    $.fn.select2.defaults.set('theme', 'bootstrap');
    initArticleTypeSelect();
});

function initArticleTypeSelect() {
    var selectArticleType = $('.select-article-type');
    var articleTypeId = $('#TypeID');
    var articleTypeName = $('#TypeName');

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
        templateResult: formatData,
        templateSelection: formatDataSelection
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

function formatData(data) {
    if (data.loading) return data.text;

    return '<div>' + data.parent + '</div><div><small style="color: #95a5a6;">' + data.desc + '</small></div>';
}

function formatDataSelection(data) {
    return data.text;
}

function getMimeType(num) {
    getMimeType(num, "Article/GetMimeType?id=");
}