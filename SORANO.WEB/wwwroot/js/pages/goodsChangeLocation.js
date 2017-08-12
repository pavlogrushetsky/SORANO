$(document).ready(function () {
    $.fn.select2.defaults.set('theme', 'bootstrap');
    $('.select-location').select2({
        "language": {
            "noResults": function () {
                return "Места не найдены";
            }
        }
    });
});