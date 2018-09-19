$(document).ready(function () {
    $.fn.select2.defaults.set('theme', 'bootstrap');

    initDateTimePicker('#visit-date');

    initGenericSelect({
        selectElementClass: '.select-location',
        valueElementId: '#LocationID',
        displayElementId: '#LocationName',
        noResultsText: 'Магазины не найдены',
        searchingText: 'Поиск магазинов...',
        errorLoadingText: 'Невозможно загрузить результаты поиска',
        placeholderText: 'Магазин',
        url: '/Location/GetLocations'
    });

    initTooltip();
    
    if ($.fn.dataTableExt !== undefined) { 
        jQuery.extend(jQuery.fn.dataTableExt.oSort, {
            "currency-pre": function (a) {
                return parseFloat(a.split(' ')[0].replace(',', '.'));
            },
            "currency-asc": function (a, b) {
                return a - b;
            },
            "currency-desc": function (a, b) {
                return b - a;
            }
        });
    }

    $("input.input-validation-error").closest(".input-group").addClass("has-error");
    $("select.input-validation-error").closest(".input-group").addClass("has-error");
    $("span.field-validation-error").closest(".input-group").addClass("has-error");
    $("textarea.input-validation-error").closest(".input-group").addClass("has-error");

    $(window).on('resize', function () {
        $('.form-group').each(function () {
            $(this).find('.select2-container').css('width', '100%');
        });
    });

    $('.select-attachment-type').one('change', function () {
        var id = $(this).attr('id');
        var matches = id.match(/\d+$/);
        if (matches) {
            var number = matches[0];
            var typeId = $(this).val();            
            if (typeId) {
                $.post('/AttachmentType/GetMimeTypes?id=' + typeId, function (mimeType) {
                    $('#attachment_input_' + number).attr('accept', mimeType);
                    $('#Attachments_' + number + '__MimeTypes').val(mimeType);
                });
            }
            var data = $(this).select2('data');
            if (data[0]) {
                $('#Attachments_' + number + '__TypeID').val(data[0].id);
                $('#Attachments_' + number + '__TypeName').val(data[0].text);
            }
            else {
                $('#Attachments_' + number + '__TypeID').val(0);
                $('#Attachments_' + number + '__TypeName').val('');
            }
        }       
    });   

    $('input[type=file]').not('#main_picture_input').change(function () {
        var id = $(this).attr('id');
        var name = document.getElementById(id).files[0].name;
        var matches = id.match(/\d+$/);
        if (matches) {
            var number = matches[0];
            $('#attachment_name_' + number).val(name);
            $('#Attachments_' + number + '__IsNew').val(true);
        }
    });   

    $('form').keypress(keypressHandler);

    $('button[type=submit]').on('click',
        function () {
            if ($(this).text().indexOf('Войти') > -1) {
                $(this).text('Вход...');
                $(this).prop('disabled', true);
                $('form').submit();
            }

            if ($(this).text().indexOf('Сохранить') > -1) {
                $(this).text('Сохранение...');
                $(this).prop('disabled', true);    
                $('form').submit();
            }               
        });

    $(document).keydown(function (e) {
        if (e.keyCode === 186 && e.ctrlKey) {
            $('#visitForm').modal('show');
        }
    });

    $('#visitForm').on('shown.bs.modal', function () {
        $(this).find('[autofocus]').focus();
    });
});

function initTooltip() {
    $("[data-toggle=tooltip]").tooltip({
        animated: 'fade',
        container: 'body'
    });
}

function keypressHandler(e) {
    if (e.which === 13 && e.target.className.indexOf('search') !== -1) {
        return false;
    }

    if (e.which === 13) {
        e.preventDefault();
        $(this).blur();
        $('button[type=submit]').focus().click();
    }
}

function initDateTimePicker(element) {
    $(element).datetimepicker({
        locale: 'ru',
        format: 'DD.MM.YYYY',
        showTodayButton: true,
        showClear: true,
        showClose: true,
        icons: {
            clear: 'fa fa-trash',
            close: 'fa fa-times',
            today: 'fa fa-calendar-check-o'
        },
        tooltips: {
            today: 'Текущая дата',
            clear: 'Очистить выбор',
            close: 'Закрыть окно',
            nextMonth: 'Следующий месяц',
            prevMonth: 'Предыдущий месяц',
            selectMonth: 'Выбрать месяц',
            selectYear: 'Выбрать год',
            selectDecade: 'Выбрать десятилетие',
            nextYear: 'Следующий год',
            nextDecade: 'Следующее десятилетие',
            prevYear: 'Предыдущий год',
            prevDecade: 'Предыдущее десятилетие'
        }
    });
}

function initGenericSelect(model) {
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

    if (valueElement.val() > 0) {
        selectElement.append($('<option selected></option>').attr('value', valueElement.val()).text(displayElement.val()));
    }

    selectElement.on("select2:opening", function () {
        selectElement.val(null).trigger('change');
    });

    selectElement.on("change", function () {
        var data = selectElement.select2('data');
        if (data[0]) {
            valueElement.val(data[0].id);
            displayElement.val(data[0].text);
        }
        else {
            valueElement.val(0);
            displayElement.val('');
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

function initAttachmentTypeSelect() {
    var selectAttachmentTypes = $('.select-attachment-type');

    selectAttachmentTypes.select2({
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
            url: '/AttachmentType/GetAttachmentTypes',
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
                            exts: item.exts
                        }
                    })
                };
            },
            cache: true
        },
        escapeMarkup: function (markup) { return markup; },
        templateResult: formatAttachmentTypeData,
        templateSelection: formatAttachmentTypeDataSelection
    });

    selectAttachmentTypes.each(function () {
        var id = $(this).attr('id');
        var matches = id.match(/\d+$/);
        if (matches) {
            var number = matches[0];
            var typeId = $('#Attachments_' + number + '__TypeID');
            var typeName = $('#Attachments_' + number + '__TypeName');
            if (typeId.val() > 0) {
                $(this).append($('<option selected></option>').attr('value', typeId.val()).text(typeName.val()));
            }
        }     
    });
}

function formatAttachmentTypeData(data) {
    if (data.loading) return data.text;

    return '<div>' + data.text + '</div><div><small style="color: #95a5a6;">' + data.desc + '</small></div><div><small style="color: #95a5a6;">' + data.exts + '</small></div>';
}

function formatAttachmentTypeDataSelection(data) {
    return data.text;
}

function previewMainPicture(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            document.getElementById("mainPicture_info").style.display = 'none';
            document.getElementById("mainPicture_img").style.display = 'inline-block';
            $('#mainPicture_img').attr('src', e.target.result);
        }
        reader.readAsDataURL(input.files[0]);
    }
}

function initRecomendationsDataTable() {
    var recommendationsDataTable = $("#recommendations-datatable").DataTable({
        responsive: true,
        "autoWidth": false,
        "scrollX": false,
        "pagingType": "numbers",
        "language": {
            "lengthMenu": "Отобразить _MENU_ рекомендаций на странице",
            "zeroRecords": "Рекомендации отсутствуют",
            "info": "Отображение страницы _PAGE_ из _PAGES_",
            "infoEmpty": "Записи отсутствуют",
            "infoFiltered": "(Отфильтровано из _MAX_ записей)",
            "search": "Поиск"
        },
        "drawCallback": function () {
            $('.pagination').addClass('pagination-sm');
        }
    });

    recommendationsDataTable.columns().eq(0).each(function (colIdx) {
        $('input', $('#recommendations-datatable th')[colIdx]).on('keyup change', function () {
            recommendationsDataTable
                .column(colIdx)
                .search(this.value)
                .draw();
        });
    });
}

function initAttachmentsDataTable() {
    var attachmentsDataTable = $("#attachments-datatable").DataTable({
        responsive: true,
        "autoWidth": false,
        "scrollX": false,
        "columnDefs": [
            { "orderable": false, "targets": 2 }
        ],
        "pagingType": "numbers",
        "language": {
            "lengthMenu": "Отобразить _MENU_ вложений на странице",
            "zeroRecords": "Вложения отсутствуют",
            "info": "Отображение страницы _PAGE_ из _PAGES_",
            "infoEmpty": "Записи отсутствуют",
            "infoFiltered": "(Отфильтровано из _MAX_ записей)",
            "search": "Поиск"
        },
        "drawCallback": function () {
            $('.pagination').addClass('pagination-sm');
        }
    });

    attachmentsDataTable.columns().eq(0).each(function (colIdx) {
        $('input', $('#attachments-datatable th')[colIdx]).on('keyup change', function () {
            attachmentsDataTable
                .column(colIdx)
                .search(this.value)
                .draw();
        });
    });   
}

function initArticlesDataTable() {
    var table = $("#article-datatable").DataTable({
        responsive: true,
        "autoWidth": false,
        "scrollX": false,
        "deferRender": true,
        "aaSorting": [],
        "columnDefs": [
            { "orderable": false, "targets": -1 },
            { type: "currency", "targets": -2 }
        ],
        "pagingType": "numbers",
        "language": {
            "lengthMenu": "Отобразить _MENU_ артикулов на странице",
            "zeroRecords": "Артикулы отсутствуют",
            "info": "Отображение страницы _PAGE_ из _PAGES_",
            "infoEmpty": "Записи отсутствуют",
            "infoFiltered": "(Отфильтровано из _MAX_ записей)",
            "search": "Поиск"
        },
        "drawCallback": function () {
            $('.pagination').addClass('pagination-sm');
        }
    });

    table.columns().eq(0).each(function (colIdx) {
        $('input', $('th')[colIdx]).on('keyup change',
            function () {
                table
                    .column(colIdx)
                    .search(this.value)
                    .draw();
            });
    });
}

function initDeliveryItemsDataTable() {
    var itemsTable = $("#items-datatable").DataTable({
        responsive: true,
        "autoWidth": false,
        "scrollX": false,
        "pagingType": "numbers",
        "language": {
            "lengthMenu": "Отобразить _MENU_ позиций на странице",
            "zeroRecords": "Позиции отсутствуют",
            "info": "Отображение страницы _PAGE_ из _PAGES_",
            "infoEmpty": "Записи отсутствуют",
            "infoFiltered": "(Отфильтровано из _MAX_ записей)",
            "search": "Поиск"
        },
        "drawCallback": function () {
            $('.pagination').addClass('pagination-sm');
        }
    });

    itemsTable.columns().eq(0).each(function (colIdx) {
        $('input', $('#items-datatable th')[colIdx]).on('keyup change',
            function () {
                itemsTable
                    .column(colIdx)
                    .search(this.value)
                    .draw();
            });
    });
}

function initDeliveriesDataTable() {
    var table = $("#deliveries-datatable").DataTable({
        responsive: true,
        "autoWidth": false,
        "scrollX": false,
        "aaSorting": [],
        "deferRender": true,
        "columnDefs": [
            { "orderable": false, "targets": -1 },
            { type: "currency", "targets": -3 },
            { type: "num", "targets": -4 }
        ],
        "pagingType": "numbers",
        "language": {
            "lengthMenu": "Отобразить _MENU_ поставок на странице",
            "zeroRecords": "Поставки отсутствуют",
            "info": "Отображение страницы _PAGE_ из _PAGES_",
            "infoEmpty": "Записи отсутствуют",
            "infoFiltered": "(Отфильтровано из _MAX_ записей)",
            "search": "Поиск"
        },
        "drawCallback": function () {
            $('.pagination').addClass('pagination-sm');
        }
    });

    table.columns().eq(0).each(function (colIdx) {
        $('input', $('#deliveries-datatable th')[colIdx]).on('keyup change', function () {
            table
                .column(colIdx)
                .search(this.value)
                .draw();
        });
    });
}

function isNumeric(value) {
    return !isNaN(parseFloat(value)) && isFinite(value);
}

function toDecimal(value) {
    return parseFloat(value.replace(',', '.'));
}

function formatDecimal(value) {
    return parseFloat(value).toFixed(2).toString().replace(',', '').replace('.', ',');
}