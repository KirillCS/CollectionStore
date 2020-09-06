document.querySelectorAll(".js-markdown-input").forEach(input => {
    const preview = input.closest(".js-markdown-block").querySelector(".js-markdown-preview");
    preview.innerHTML = marked(input.value);
    input.addEventListener("input", e => {
        preview.innerHTML = marked(input.value);
    });
});