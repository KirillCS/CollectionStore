$(document).ready(() => {
    const nameInput = document.getElementById("nameInput");
    const themeInput = document.getElementById("themeInput");
    const descriptionInput = document.getElementById("descriptionInput");
    const descriptionPreviewInput = document.getElementById("descriptionPreviewInput");
    const saveInfoBtn = document.getElementById("saveInfoBtn");

    nameInput.addEventListener("input", e => {
        if (!nameInput.value) {
            saveInfoBtn.setAttribute("disabled", "disabled");
        }
        else if (saveInfoBtn.hasAttribute("disabled")) {
            saveInfoBtn.removeAttribute("disabled");
        }
    });

    $("#editInfoModal").on("show.bs.modal", e => {
        nameInput.value = document.getElementById("nameHiddenInput").value;
        themeInput.value = document.getElementById("themeHiddenInput").value;
        descriptionInput.value = document.getElementById("descriptionHiddenInput").value;
        descriptionPreviewInput.innerHTML = marked(descriptionInput.value);
        if (saveInfoBtn.hasAttribute("disabled")) {
            saveInfoBtn.removeAttribute("disabled");
        }
    });
});