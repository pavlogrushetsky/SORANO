$(document).ready(function () {
    $.fn.select2.defaults.set('theme', 'bootstrap');

    initGenericSelect({
        selectElementClass: '.select-location',
        valueElementId: '#LocationID',
        displayElementId: '#LocationName',
        noResultsText: 'Магазины не найдены',
        searchingText: 'Поиск магазинов...',
        errorLoadingText: 'Невозможно загрузить результаты поиска',
        placeholderText: 'Магазин',
        url: '/Location/GetLocations'
    });
});