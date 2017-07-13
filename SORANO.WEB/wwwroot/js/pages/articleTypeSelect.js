$(document).ready(function () {
    $('.tree li:has(ul)').addClass('parent_li').find(' > span').attr('title', 'Свернуть ветку');
    $('.tree li.parent_li > span').on('click', function () {
        var children = $(this).parent('li.parent_li').find(' > ul > li');
        if (children.is(":visible")) {
            children.hide('fast');
            $(this).attr('title', 'Развернуть ветку').find(' > i').addClass('fa-tags').removeClass('fa-tag');
            $(this).find(' > span').show('fast');
        } else {
            children.show('fast');
            $(this).attr('title', 'Свернуть ветку').find(' > i').addClass('fa-tag').removeClass('fa-tags');
            $(this).find(' > span').hide('fast');
        }
    });
    $('.tree li').on('click', function (e) {
        var id = $(this).attr('id');
        var input = $('#typeID_' + id), val = input.val();
        $('.selectedInput').val("False");
        $('.tree li.selected').removeClass('selected');
        input.val(val === "True" ? "False" : "True");
        $(this).addClass('selected');
        e.stopPropagation();
    });
});