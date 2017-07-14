$(document).ready(function () {
    $("[data-toggle=tooltip]").tooltip();
    $("input.input-validation-error").not(".recommendations,.attachments").closest(".form-group").addClass("has-error");
    $("select.input-validation-error").closest(".form-group").addClass("has-error");
    $("span.field-validation-error").closest(".form-group").addClass("has-error");

    $('input[type=file]').not('#main_picture_input').change(function () {
        debugger;
        var id = $(this).attr('id');
        var name = document.getElementById(id).files[0].name;
        var matches = id.match(/\d+$/);
        if (matches) {
            var number = matches[0];
            $('#attachment_name_' + number).val(name);
            $('#Attachments_' + number + '__IsNew').val(true);
        }
    });   
});

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

function getMimeType(num, url) {
    var id = $('#select_type_' + num).val();
    if (id) {
        $.post(url + id, function (mimeType) {
            $('#attachment_input_' + num).attr('accept', mimeType);
        });
    }
}

function initRecomendationsDataTable() {
    var recommendationsDataTable = $("#recommendations-datatable").DataTable({
        responsive: true,
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