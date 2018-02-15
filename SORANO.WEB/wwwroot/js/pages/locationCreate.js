$(document).ready(function () {
    $.fn.select2.defaults.set('theme', 'bootstrap');

    initGenericSelect({
        selectElementClass: '.select-location-type',
        valueElementId: '#TypeID',
        displayElementId: '#TypeName',
        noResultsText: 'Типы не найдены',
        searchingText: 'Поиск типов...',
        errorLoadingText: 'Невозможно загрузить результаты поиска',
        placeholderText: 'Тип',
        url: '/LocationType/GetLocationTypes'
    });

    initAttachmentTypeSelect();
});