$(document).ready(function () {
    $('#accordion').on('shown.bs.collapse', function () {
        var active = $("#accordion .in").attr('id');
        localStorage.setItem('article-details-active-panel', active);
        localStorage.removeItem('article-details-all-collapsed');
    });
    $('#accordion').on('hidden.bs.collapse', function () {
        localStorage.setItem('article-details-all-collapsed', true);
        localStorage.removeItem('article-details-active-panel');
    });

    var lastPanel = localStorage.getItem('article-details-active-panel');
    var allCollapsed = localStorage.getItem('article-details-all-collapsed');
    if (lastPanel) {
        $("#accordion .panel-collapse").removeClass('in');
        $("#" + lastPanel).addClass("in");
    }
    if (allCollapsed) {
        $("#accordion .panel-collapse").removeClass('in');
    }       
   
    initRecomendationsDataTable();
    initAttachmentsDataTable();
    initArticlesDataTable();
});