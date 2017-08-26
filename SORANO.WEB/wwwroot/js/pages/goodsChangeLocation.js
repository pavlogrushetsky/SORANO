$(document).ready(function () {
    $.fn.select2.defaults.set('theme', 'bootstrap');

    initLocationSelect();
});

function initLocationSelect() {
    var selectLocation = $('.select-location');
    var locationId = $('#TargetLocationID');
    var locationName = $('#TargetLocationName');
    var currentLocationId = $('#CurrentLocationID');

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
                    except: currentLocationId.val()
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