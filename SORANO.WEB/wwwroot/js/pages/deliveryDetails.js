$(document).ready(function () {
    $('#accordion').on('shown.bs.collapse', function () {
        var active = $("#accordion .in").attr('id');
        localStorage.setItem('delivery-details-active-panel', active);
        localStorage.removeItem('delivery-details-all-collapsed');
    });
    $('#accordion').on('hidden.bs.collapse', function () {
        localStorage.setItem('delivery-details-all-collapsed', true);
        localStorage.removeItem('delivery-details-active-panel');
    });

    var lastPanel = localStorage.getItem('delivery-details-active-panel');
    var allCollapsed = localStorage.getItem('delivery-details-all-collapsed');
    if (lastPanel) {
        $("#accordion .panel-collapse").removeClass('in');
        $("#" + lastPanel).addClass("in");
    }
    if (allCollapsed) {
        $("#accordion .panel-collapse").removeClass('in');
    }

    initRecomendationsDataTable();
    initAttachmentsDataTable();
    initDeliveryItemsDataTable();
});