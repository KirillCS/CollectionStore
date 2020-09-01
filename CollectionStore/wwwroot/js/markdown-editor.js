document.querySelectorAll(".js-markdown-input").forEach(input => {
    const preview = input.closest(".js-markdown-block").querySelector(".js-markdown-preview");
    input.addEventListener("input", e => {
        preview.innerHTML = marked(input.value);
    });
});