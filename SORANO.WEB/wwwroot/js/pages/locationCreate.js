$(document).ready(function () {
    $.fn.select2.defaults.set('theme', 'bootstrap');
    initLocationTypeSelect();
    initAttachmentTypeSelect();
});

function initLocationTypeSelect() {
    var selectLocationType = $('.select-location-type');
    var locationTypeId = $('#TypeID');
    var locationTypeName = $('#TypeName');

    selectLocationType.select2({
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
            url: '/LocationType/GetLocationTypes',
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

    if (locationTypeId.val() > 0) {
        selectLocationType.append($('<option selected></option>').attr('value', locationTypeId.val()).text(locationTypeName.val()));
    }

    selectLocationType.on("select2:opening", function () {
        selectLocationType.val(null).trigger('change');
    });

    selectLocationType.on("change", function () {
        var data = selectLocationType.select2('data');
        if (data[0]) {
            locationTypeId.val(data[0].id);
            locationTypeName.val(data[0].text);
        }
        else {
            locationTypeId.val(0);
            locationTypeName.val('');
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