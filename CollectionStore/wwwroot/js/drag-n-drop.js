document.querySelectorAll(".drop-zone__input").forEach((inputElement) => {
    const dropZoneElement = inputElement.closest(".drop-zone");
    const resetButtons = dropZoneElement.closest(".js-picture-field").getElementsByClassName("js-reset-button");
    let promptMessage = dropZoneElement.querySelector(".js-picture-field-prompt-label").value;
    if (!promptMessage) {
        promptMessage = "Drag image here or click to upload";
    }

    for (let btn of resetButtons) {
        btn.addEventListener("click", e => {
            if (inputElement.files.length) {
                inputElement.value = "";
                resetImage(dropZoneElement, promptMessage);
            }
        })
    }
    
    inputElement.addEventListener("change", (e) => {
        if (e.target.files.length) {
            setImage(e.target.files[0], dropZoneElement);
        }
        else {
            resetImage(dropZoneElement, promptMessage);
        }
    });

    dropZoneElement.addEventListener("click", (e) => {
        inputElement.click();
    });

    dropZoneElement.addEventListener("drop", (e) => {
        e.preventDefault();

        if (e.dataTransfer.files.length) {
            inputElement.files = e.dataTransfer.files;
            setImage(inputElement.files[0], dropZoneElement);
        }

        dropZoneElement.classList.remove("drop-zone--over");
    });

    // animation
    dropZoneElement.addEventListener("dragover", (e) => {
        e.preventDefault();
        dropZoneElement.classList.add("drop-zone--over");
    });

    ["dragleave", "dragend"].forEach((type) => {
        dropZoneElement.addEventListener(type, (e) => {
            dropZoneElement.classList.remove("drop-zone--over");
        });
    });
});

function setImage(file, dropZoneElement) {
    if (file.type.startsWith("image/")) {
        let promptElement = dropZoneElement.querySelector(".drop-zone__prompt");
        if (promptElement != null) {
            promptElement.remove();
        }

        let thumbnailElement = dropZoneElement.querySelector(".drop-zone__thumb");
        if (!thumbnailElement) {
            thumbnailElement = document.createElement("div");
            thumbnailElement.classList.add("drop-zone__thumb");
            thumbnailElement.dataset.label = file.name;
            dropZoneElement.appendChild(thumbnailElement);
        }

        const reader = new FileReader();
        reader.readAsDataURL(file);
        reader.onload = () => {
            thumbnailElement.style.backgroundImage = `url('${reader.result}')`;
        };
    }
    else {
        alert("Only images");
    }
}

function resetImage(dropZoneElement, promptMessage) {
    let thumbnailElement = dropZoneElement.querySelector(".drop-zone__thumb");
    if (thumbnailElement) {
        thumbnailElement.remove();
    }

    let promptElement = dropZoneElement.querySelector(".drop-zone__prompt");
    if (!promptElement) {
        promptElement = document.createElement("span");
        promptElement.classList.add("drop-zone__prompt");
        promptElement.innerHTML = promptMessage;
        dropZoneElement.appendChild(promptElement);
    }
}