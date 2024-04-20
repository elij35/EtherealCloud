
//************************* Menu functions: ************************
function downloadFile(id) {
    document.getElementById('fileIdDownloadInput').value = id;
    document.getElementById('downloadForm').submit();
}

function deleteFile(id, fileType) {
    document.getElementById('fileIdInput').value = id;
    document.getElementById('typeInput').value = fileType;
    document.getElementById('deleteForm').submit();
}

function shareFile(id) {
    console.log("Share activated: " + id)
    document.getElementById('fileIdShareInput').value = id;
    document.getElementById('shareForm').submit();
}

function renameFile(id) {
    // Implement rename logic here
    console.log("Renaming file: " + id);
}

function navIntoFolderOrDownload(id, fileType, name) {
    if (fileType === "Folder") {

        document.getElementById('idNavInput').value = id;
        document.getElementById('nameNavInput').value = name;
        document.getElementById('navForm').submit();
    } else {

        downloadFile(id);
    }
}

//************************* On a few pages to do with the top bar: ************************

//Changes the sort image when pressed
function toggleSort() {
    // Submit the form
    document.getElementById('sortForm').submit();
}

//Loads naming folder popup
function showInputBox() {
    document.getElementById('input-box').style.display = 'block';
    document.querySelector('#input-box input[name="foldername"]').focus();
}

//Cancels the folder creation
function cancelFolderCreation() {
    document.getElementById('input-box').style.display = 'none';
}

//************************* Mobile hold tap function: ************************
var holdtapTimer = null;

function tap(id, fileType, name) {
    holdtapTimer = setTimeout(function () {
        tapHoldTimer(id, fileType, name);
    }, 500);

}

function tapCancel() {
    clearTimeout(holdtapTimer);
}

function tapHoldTimer(id, fileType, name) {
    navIntoFolderOrDownload(id, fileType, name);
}


function showButton(id) {
    var button = document.getElementById(id);
    button.style.display = 'inline';
}

function hideButton(holderId) {
    if (matchMedia('(hover: hover)').matches) {
        var holder = document.getElementById(holderId);
        holder.style.display = "none";

    }
}

//************************* Decodes html text: ************************
function decodeHtml(html) {
    var txt = document.createElement("textarea");
    txt.innerHTML = html;
    return txt.value;
}

//************************* Changing sort image orientation: ************************
function toggleImage() {
    var imageElement = document.getElementById('toggleImage');
    if (imageElement.src.endsWith('sort-alpha-down.svg')) {
        imageElement.src = 'images/tools/sort-alpha-up.svg';
    } else {
        imageElement.src = 'images/tools/sort-alpha-down.svg';
    }
}

//************************* Bin page functionality: ************************
function restoreFileFolder(id, fileType) {

    document.getElementById('restoreIdInput').value = id;
    document.getElementById('restoreTypeInput').value = fileType;
    document.getElementById('restoreForm').submit();
}