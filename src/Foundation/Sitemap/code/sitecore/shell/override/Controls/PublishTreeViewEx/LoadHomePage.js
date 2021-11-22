jQuery(document).ready(function () {
    var homepagenode = document.querySelector("#PublishTreeList_all #home-page-id");
    if (homepagenode != null) {
        var homepagenodevalue = homepagenode.value;
        var treenodeid = homepagenodevalue.substring(0, homepagenodevalue.lastIndexOf("_"));
        var treeelement = $(treenodeid);
        var homeelement = $(homepagenodevalue);
        var homeid = homepagenodevalue.substring(homepagenodevalue.lastIndexOf("_") + 1);
        Sitecore.Treeview.onTreeGlyphClick(homeelement, treeelement, homeid);
    }
});