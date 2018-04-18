$(document).ready(function () {
    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        localStorage.setItem('articles-active-tab', $(e.target).attr('href'));
    });

    var lastTab = localStorage.getItem('articles-active-tab');
    if (lastTab) {
        $('[href="' + lastTab + '"]').tab('show');
    }     

    loadArticleTable();
    loadArticleTypesTree();

    $(document).on('click', '#refresh-articles', function(e) {
        e.preventDefault();
        $(this).tooltip('hide');
        loadArticleTable();        
    });

    $(document).on('click', '#toggle-deleted-articles', function(e) {
        e.preventDefault();
        $(this).tooltip('hide');
        toggleDeletedArticles();
    });

    $(document).on('click', '#refresh-article-types', function (e) {
        e.preventDefault();
        $(this).tooltip('hide');
        loadArticleTypesTree();
    });

    $(document).on('click', '#toggle-deleted-article-types', function (e) {
        e.preventDefault();
        $(this).tooltip('hide');
        toggleDeletedArticleTypes();
    });
    
    $(document).on('click', '#article-types-search-btn', function(e) {
        e.preventDefault();
        $(this).tooltip('hide');
        loadArticleTypesTree();
    });
});

function loadArticleTable() {
    $('#articles-table').hide(100);
    $('#progress-bar').animate({ opacity: 1.0 }, 100, function() {
        $('#articles-table').load('/Article/Table', function () {              
            $('#articles-table').show(100, function () {
                initArticlesDataTable();
                var button = $('#toggle-deleted-articles');
                var icon = button.find('i');
                var showDeleted = $('#article-datatable').data('showdeleted');
                if (showDeleted === 'True') {
                    button.attr('data-original-title', 'Скрыть удалённые артикулы');
                    icon.removeClass('fa-eye');
                    icon.addClass('fa-eye-slash');
                } else {
                    button.attr('data-original-title', 'Отобразить удалённые артикулы');
                    icon.removeClass('fa-eye-slash');
                    icon.addClass('fa-eye');
                }
                initTooltip();
                $('#progress-bar').animate({ opacity: 0.0 }, 100);
            });
        });
    });   
}

function loadArticleTypesTree() {
    $('#article-types').hide(100);
    $('#progress-bar').animate({ opacity: 1.0 }, 100, function () {
        var searchTerm = $('#article-types-search-term').val();
        $('#article-types').load('/ArticleType/Tree', { searchTerm: searchTerm }, function () {
            $('#article-types').show(100, function () {
                initArticleTypesTree();
                var button = $('#toggle-deleted-article-types');
                var icon = button.find('i');
                var showDeleted = $('#article-types-tree').data('showdeleted');
                if (showDeleted === 'True') {
                    button.attr('data-original-title', 'Скрыть удалённые типы артикулов');
                    icon.removeClass('fa-eye');
                    icon.addClass('fa-eye-slash');
                } else {
                    button.attr('data-original-title', 'Отобразить удалённые типы артикулов');
                    icon.removeClass('fa-eye-slash');
                    icon.addClass('fa-eye');
                }
                initTooltip();
                $('#progress-bar').animate({ opacity: 0.0 }, 100);
            });
        });
    });
}

function toggleDeletedArticles() {
    $('#articles-table').hide(100);
    $('#progress-bar').animate({ opacity: 1.0 }, 100, function () {
        $('#articles-table').load('/Article/ToggleDeleted', function () {             
            $('#articles-table').show(100, function () {
                initArticlesDataTable();
                var button = $('#toggle-deleted-articles');
                var icon = button.find('i');
                var showDeleted = $('#article-datatable').data('showdeleted');
                if (showDeleted === 'True') {
                    button.attr('data-original-title', 'Скрыть удалённые артикулы');
                    icon.removeClass('fa-eye');
                    icon.addClass('fa-eye-slash');
                } else {
                    button.attr('data-original-title', 'Отобразить удалённые артикулы');
                    icon.removeClass('fa-eye-slash');
                    icon.addClass('fa-eye');
                }
                initTooltip();
                $('#progress-bar').animate({ opacity: 0.0 }, 100);
            });
        });
    });
}

function toggleDeletedArticleTypes() {
    $('#article-types').hide(100);
    $('#progress-bar').animate({ opacity: 1.0 }, 100, function () {
        var searchTerm = $('#article-types-search-term').val();
        $('#article-types').load('/ArticleType/ToggleDeleted', { searchTerm: searchTerm }, function () {
            $('#article-types').show(100, function () {
                initArticleTypesTree();
                var button = $('#toggle-deleted-article-types');
                var icon = button.find('i');
                var showDeleted = $('#article-types-tree').data('showdeleted');
                if (showDeleted === 'True') {
                    button.attr('data-original-title', 'Скрыть удалённые типы артикулов');
                    icon.removeClass('fa-eye');
                    icon.addClass('fa-eye-slash');
                } else {
                    button.attr('data-original-title', 'Отобразить удалённые типы артикулов');
                    icon.removeClass('fa-eye-slash');
                    icon.addClass('fa-eye');
                }
                initTooltip();
                $('#progress-bar').animate({ opacity: 0.0 }, 100);
            });
        });
    });
}

function initArticleTypesTree() {
    $('.article-types-tree li:has(ul)').addClass('parent-article-type').find(' > table').attr('title', 'Свернуть ветку');
    var types = $('.article-types-tree li.parent-article-type'); 
    $.each(types, function( key, value ) {
        var visible = localStorage.getItem('article-type-visible-' + $(value).attr('id'));
        var children = $(value).find(' > ul');
        if (visible === "true") {
            children.show('fast');
            $(value).find(' > table').attr('title', 'Свернуть ветку').find('i').addClass('fa-tag').removeClass('fa-tags');
            $(value).find(' > table').find(' > table').hide('fast');
        } else {
            children.hide('fast');
            $(value).find(' > table').attr('title', 'Развернуть ветку').find('i').addClass('fa-tag').removeClass('fa-tags');
            $(value).find(' > table').find(' > table').show('fast');
        }   
    });
    $('.article-types-tree li.parent-article-type > table').on('click', function (e) {
        var target = $(e.target);
        if (!target.is("a") && !target.is('i')) {
            var children = $(this).parent('li.parent-article-type').find(' > ul');
            if (children.is(":visible")) {
                children.hide('fast');
                localStorage.setItem('article-type-visible-' + $(this).parent('li.parent-article-type').attr('id'), false);
                $(this).attr('title', 'Развернуть ветку').find('i').addClass('fa-tags').removeClass('fa-tag');
                $(this).find(' > table').show('fast');
            } else {
                children.show('fast');
                localStorage.setItem('article-type-visible-' + $(this).parent('li.parent-article-type').attr('id'), true);
                $(this).attr('title', 'Свернуть ветку').find('i').addClass('fa-tag').removeClass('fa-tags');
                $(this).find(' > table').hide('fast');
            }
            e.stopPropagation();
        }
    });
}