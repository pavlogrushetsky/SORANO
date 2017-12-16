$(document).ready(function () {
    $.fn.select2.defaults.set('theme', 'bootstrap');

    initGenericSelect({
        selectElementClass: '.select-location',
        valueElementId: '#TargetLocationID',
        displayElementId: '#TargetLocationName',
        noResultsText: 'Места не найдены',
        searchingText: 'Поиск мест...',
        errorLoadingText: 'Невозможно загрузить результаты поиска',
        placeholderText: 'Место поставки',
        url: '/Location/GetLocations'
    });
});