﻿$(document).ready(function () {
    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        localStorage.setItem('articles-active-tab', $(e.target).attr('href'));
    });

    var lastTab = localStorage.getItem('articles-active-tab');
    if (lastTab) {
        $('[href="' + lastTab + '"]').tab('show');
    }

    $('.article-types-tree li:has(ul)').addClass('parent-article-type').find(' > table').attr('title', 'Свернуть ветку');
    $('.article-types-tree li.parent-article-type > table').on('click', function (e) {
        var target = $(e.target);
        if (!target.is("a")) {
            var children = $(this).parent('li.parent-article-type').find(' > ul > li');
            if (children.is(":visible")) {
                children.hide('fast');
                $(this).attr('title', 'Развернуть ветку').find('i').addClass('fa-tags').removeClass('fa-tag');
                $(this).find(' > table').show('fast');
            } else {
                children.show('fast');
                $(this).attr('title', 'Свернуть ветку').find('i').addClass('fa-tag').removeClass('fa-tags');
                $(this).find(' > table').hide('fast');
            }
            e.stopPropagation();
        }              
    });
    
    var activeInfoId = localStorage.getItem('article-types-active-info');
    if (activeInfoId) {
        $(activeInfoId).show('fast');
    }

    $('.toggle-article-type-info').click(function(e) {
        e.preventDefault();

        var self = $(this);
        var thisInfo = self.parent().parent().parent().find('.article-type-info');
        var tree = $('.article-types-tree');
        var id = thisInfo.attr('id');

        if (thisInfo.is(':visible')) {
            thisInfo.hide('fast');
        } else {
            tree.find('.article-type-info').hide('fast');
            thisInfo.show('fast');
            localStorage.setItem('article-types-active-info', '#' + id);
        }
    });

    initArticlesDataTable();
});

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
            $('.pagination').first().addClass('pagination-sm');
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