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
            url: 'GetArticleTypes',
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
                            id: item.id
                        }
                    })
                };
            },
            cache: true
        }
    });

    if (articleTypeId.val() > 0) {
        selectArticleType.append($('<option></option>').attr('value', articleTypeId.val()).text(articleTypeName.val()));
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

function getMimeType(num) {
    getMimeType(num, "ArticleType/GetMimeType?id=");
}