$(document).ready(function () {
    $.fn.select2.defaults.set('theme', 'bootstrap');
    $('.select-article-type').select2({
        "language": {
            "noResults": function () {
                return "Типы артикулов не найдены";
            }
        }
    });
});

function getMimeType(num) {
    getMimeType(num, "Article/GetMimeType?id=");
}