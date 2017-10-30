$(document).ready(function () {
    $.fn.select2.defaults.set('theme', 'bootstrap');

    initGenericSelect({
        selectElementClass: '.select-article',
        valueElementId: '#ArticleID',
        displayElementId: '#ArticleDescription',
        noResultsText: 'Артикулы не найдены',
        searchingText: 'Поиск артикулов...',
        errorLoadingText: 'Невозможно загрузить результаты поиска',
        placeholderText: 'Артикул',
        url: '/Article/GetArticles'
    });

    initAttachmentTypeSelect();
});