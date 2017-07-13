$(document).ready(function () {
    $("input.input-validation-error").not(".recommendations,.attachments").closest(".form-group").addClass("has-error");
    $("select.input-validation-error").closest(".form-group").addClass("has-error");

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

function getMimeType(num) {
    var id = $('#select_type_' + num).val();
    if (id) {
        $.post("LocationType/GetMimeType?id=" + id, function (mimeType) {
            $('#attachment_input_' + num).attr('accept', mimeType);
        });
    }
}