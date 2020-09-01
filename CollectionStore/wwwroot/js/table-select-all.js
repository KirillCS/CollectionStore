function mainCheckBoxClicked() {
    var main = document.getElementsByClassName("js-main-check-box")[0];
    var singleCheckBoxes = document.getElementsByClassName("js-check-box");
    for (var i = 0; i < singleCheckBoxes.length; i++) {
        singleCheckBoxes[i].checked = main.checked;
    }
}
function singleCheckBoxClicked(current) {
    var main = document.getElementsByClassName("js-main-check-box")[0];
    if (!current.checked) main.checked = false;
    else {
        var singleCheckBoxes = document.getElementsByClassName("js-check-box");
        var checked = true;
        for (var i = 0; i < singleCheckBoxes.length; i++) {
            if (!singleCheckBoxes[i].checked) {
                checked = false;
                break;
            }
        }
        main.checked = checked;
    }
}