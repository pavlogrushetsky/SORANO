﻿$(document).ready(function () {
    $.fn.select2.defaults.set('theme', 'bootstrap');
    $('.select-client').select2({
        "language": {
            "noResults": function () {
                return "Клиенты не найдены";
            }
        }
    });
    $('.select-article').select2({
        "language": {
            "noResults": function () {
                return "Артикулы не найдены";
            }
        }
    });
});