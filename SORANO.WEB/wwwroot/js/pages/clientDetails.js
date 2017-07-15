$(document).ready(function () {
    $('#accordion').on('shown.bs.collapse', function () {
        var active = $("#accordion .in").attr('id');
        localStorage.setItem('client-details-active-panel', active);
        localStorage.removeItem('client-details-all-collapsed');
    });
    $('#accordion').on('hidden.bs.collapse', function () {
        localStorage.setItem('client-details-all-collapsed', true);
        localStorage.removeItem('client-details-active-panel');
    });

    var lastPanel = localStorage.getItem('client-details-active-panel');
    var allCollapsed = localStorage.getItem('client-details-all-collapsed');
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