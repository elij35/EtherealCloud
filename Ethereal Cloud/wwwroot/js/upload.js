var logger = "@ViewData["logger"]";
if (logger !== null) {
    console.log(decodeHtml(logger));
}

function decodeHtml(html) {
    var txt = document.createElement("textarea");
    txt.innerHTML = html;
    return txt.value;
}