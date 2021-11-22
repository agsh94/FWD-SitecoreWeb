function scCustomDropLink() {

}
scCustomDropLink.prototype.selectDropdownClick = function (elem, event) {
    event.stopPropagation();
    this.toggle(elem.nextSibling);
}
scCustomDropLink.prototype.selectItem = function (elem, event) {
    var value = elem.getAttribute("data-value");
    var html = elem.querySelector("p").innerHTML;
    var image = "";
    if (elem.querySelector("img") != null && elem.querySelector("img") != "undefined") {
        image =  elem.querySelector("img").getAttribute("src");
    }
    var parentWrapper = elem.parentElement.parentElement;
    parentWrapper.querySelector(".custom-droplink-section-btn p").innerHTML = html;
    if (parentWrapper.querySelector(".custom-droplink-section-btn img") == null || parentWrapper.querySelector(".custom-droplink-section-btn img") == "undefined") {
        var wrapper = parentWrapper.querySelector(".custom-droplink-section-btn");
        var spanelement = document.createElement("span");
        spanelement.classList.add("svg-image-container");
        var newItem = document.createElement("img");
        newItem.setAttribute("src", image);
        spanelement.appendChild(newItem);
        wrapper.insertBefore(spanelement, wrapper.childNodes[0]);
    } else {
       parentWrapper.querySelector(".custom-droplink-section-btn img").setAttribute("src", image);
    }
    parentWrapper.querySelector(".custom-droplink").value = value;

}
scCustomDropLink.prototype.toggle = function (elem) {
    if (elem.style.display==="" || elem.style.display === "none") {
        elem.style.display = "block";
    } else {
        elem.style.display = "none";
    }
}
document.addEventListener("click", function (event) {
    document.querySelectorAll(".custom-droplink-section").forEach(e => {
        e.style.display = "none";
    });
});
var scCustomDropLink = new scCustomDropLink();
