$(document).ready(function () {
    $.fn.select2.defaults.set('theme', 'bootstrap');
    initClientSelect();  
    initArticleSelect();
    initLocationSelect();
});

function initClientSelect() {
    var selectClient = $('.select-client');
    var clientId = $('#ClientID');
    var clientName = $('#ClientName');

    selectClient.select2({
        "language": {
            "noResults": function () {
                return "Клиенты не найдены";
            },
            "searching": function () {
                return "Поиск клиентов...";
            },
            "errorLoading": function () {
                return "Невозможно загрузить результаты поиска";
            }
        },
        placeholder: "Клиент",
        minimumInputLength: 0,
        ajax: {
            url: 'GetClients',
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

    if (clientId.val() > 0) {
        selectClient.append($('<option></option>').attr('value', clientId.val()).text(clientName.val()));
    }

    selectClient.on("select2:opening", function () {
        selectClient.val(null).trigger('change');
    });

    selectClient.on("change", function () {
        var data = selectClient.select2('data');
        if (data[0]) {
            clientId.val(data[0].id);
            clientName.val(data[0].text);
        }
        else {
            clientId.val(0);
            clientName.val('');
        }
    });
}

function initArticleSelect() {
    var selectArticle = $('.select-article');
    var articleId = $('#ArticleID');
    var articleName = $('#ArticleName');
    var locationId = $('#LocationID');

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
            url: 'GetArticles',
            dataType: 'json',
            type: 'POST',
            delay: 100,
            data: function (params) {
                var queryParameters = {
                    term: params.term,
                    locationId: locationId.val()
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

    if (articleId.val() > 0) {
        selectArticle.append($('<option></option>').attr('value', articleId.val()).text(articleName.val()));
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

function initLocationSelect() {
    var selectLocation = $('.select-location');
    var locationId = $('#LocationID');
    var locationName = $('#LocationName');
    var articleId = $('#ArticleID');

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
        placeholder: "Место",
        minimumInputLength: 0,
        ajax: {
            url: 'GetLocations',
            dataType: 'json',
            type: 'POST',
            delay: 100,
            data: function (params) {
                var queryParameters = {
                    term: params.term,
                    articleId: articleId.val()
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

    if (locationId.val() > 0) {
        selectLocation.append($('<option></option>').attr('value', locationId.val()).text(locationName.val()));
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