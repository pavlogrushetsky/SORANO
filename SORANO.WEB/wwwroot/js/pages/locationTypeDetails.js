$(document).ready(function () {
    $('#accordion').on('shown.bs.collapse', function () {
        var active = $("#accordion .in").attr('id');
        localStorage.setItem('location-types-details-active-panel', active);
        localStorage.removeItem('location-types-details-all-collapsed');
    });
    $('#accordion').on('hidden.bs.collapse', function () {
        localStorage.setItem('location-types-details-all-collapsed', true);
        localStorage.removeItem('location-types-details-active-panel');
    });

    var lastPanel = localStorage.getItem('location-types-details-active-panel');
    var allCollapsed = localStorage.getItem('location-types-details-all-collapsed');
    if (lastPanel) {
        $("#accordion .panel-collapse").removeClass('in');
        $("#" + lastPanel).addClass("in");
    }
    if (allCollapsed) {
        $("#accordion .panel-collapse").removeClass('in');
    }

    initRecomendationsDataTable();

    initAttachmentsDataTable();
});