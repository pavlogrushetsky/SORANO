$(document).ready(function () {
    $("[data-toggle=tooltip]").tooltip({
        animated: 'fade',
        container: 'body'
    });

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
});

function keypressHandler(e) {
    if (e.which === 13) {
        e.preventDefault();
        $(this).blur();
        $('button[type=submit]').focus().click();
    }
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
        "columnDefs": [
            { "orderable": false, "targets": 5 }
        ],
        "order": [[0, "desc"]],
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

function isNumeric(n) {
    return !isNaN(parseFloat(n)) && isFinite(n);
}

function formatDecimal(value) {
    return parseFloat(value).toFixed(2).toString().replace(/,/g, '');
}