$(document).ready(function () {
    $.fn.select2.defaults.set('theme', 'bootstrap');
    $('.select-location-type').select2({
        "language": {
            "noResults": function () {
                return "Типы мест не найдены";
            }
        }
    });
});

function getMimeType(num) {
    getMimeType(num, "Location/GetMimeType?id=");
}