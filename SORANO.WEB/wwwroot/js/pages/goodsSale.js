$(document).ready(function () {
    $.fn.select2.defaults.set('theme', 'bootstrap');
    initClientSelect();   

    $('.select-article').select2({
        "language": {
            "noResults": function () {
                return "Артикулы не найдены";
            }
        }
    });
    $('.select-location').select2({
        "language": {
            "noResults": function () {
                return "Места не найдены";
            }
        }
    });
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