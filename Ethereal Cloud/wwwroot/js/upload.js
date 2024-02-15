

function toggleImage() {
    var imageElement = document.getElementById('toggleImage');
    if (imageElement.src.endsWith('sort-alpha-down.svg')) {
        imageElement.src = 'images/tools/sort-alpha-up.svg';
    } else {
        imageElement.src = 'images/tools/sort-alpha-down.svg';
    }
}