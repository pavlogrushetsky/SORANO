$(document).ready(function () {
    $('img').on('click', function () {
        var img = $(this);
        var input = $('#SelectedID');

        if (img.hasClass('selected')) {
            img.removeClass('selected');
            input.val(0);
        } else {
            $('img.selected').removeClass('selected');
            img.addClass('selected');
            var id = img.attr('id');
            input.val(id);
        }
    });
});