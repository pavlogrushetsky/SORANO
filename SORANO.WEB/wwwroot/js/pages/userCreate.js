$(document).ready(function () {
    $.fn.select2.defaults.set('theme', 'bootstrap');

    initGenericMultipleSelect({
        selectElementClass: '.select-location',
        valueElementId: '#LocationIds',
        displayElementId: '#LocationNames',
        noResultsText: 'Места не найдены',
        searchingText: 'Поиск мест...',
        errorLoadingText: 'Невозможно загрузить результаты поиска',
        placeholderText: 'Места',
        url: '/Location/GetLocations'
    });
});

function initGenericMultipleSelect(model) {
    var selectElement = $(model.selectElementClass);
    var valueElement = $(model.valueElementId);
    var displayElement = $(model.displayElementId);

    selectElement.select2({
        "language": {
            "noResults": function () {
                return model.noResultsText;
            },
            "searching": function () {
                return model.searchingText;
            },
            "errorLoading": function () {
                return model.errorLoadingText;
            }
        },
        placeholder: model.placeholderText,
        minimumInputLength: 0,
        ajax: {
            url: model.url,
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

    //if (valueElement.val() > 0) {
    //    selectElement.append($('<option selected></option>').attr('value', valueElement.val()).text(displayElement.val()));
    //}

    //selectElement.on("select2:opening", function () {
    //    selectElement.val(null).trigger('change');
    //});

    //selectElement.on("change", function () {
    //    var data = selectElement.select2('data');
    //    if (data[0]) {
    //        valueElement.val(data[0].id);
    //        displayElement.val(data[0].text);
    //    }
    //    else {
    //        valueElement.val(0);
    //        displayElement.val('');
    //    }
    //});
}

function formatData(data) {
    if (data.loading) return data.text;

    return '<div>' + data.text + '</div><div><small style="color: #95a5a6;">' + data.desc + '</small></div>';
}

function formatDataSelection(data) {
    return data.text;
}