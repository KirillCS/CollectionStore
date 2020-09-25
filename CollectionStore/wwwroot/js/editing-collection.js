$(document).ready(() => {
    let nameInput = document.getElementById("nameInput");
    let descriptionInput = document.getElementById("descriptionInput");
    let saveNameBtn = document.getElementById("saveNameBtn");

    nameInput.addEventListener("input", e => {
        if (!nameInput.value) {
            saveNameBtn.setAttribute("disabled", "disabled");
        }
        else if (saveNameBtn.hasAttribute("disabled")) {
            saveNameBtn.removeAttribute("disabled");
        }
    });

    $("#editNameModal").on("show.bs.modal", e => {
        nameInput.value = document.getElementById("nameHiddenInput").value;
        descriptionInput.value = document.getElementById("descriptionHiddenInput").value;
        if (saveNameBtn.hasAttribute("disabled")) {
            saveNameBtn.removeAttribute("disabled");
        }
    });
});