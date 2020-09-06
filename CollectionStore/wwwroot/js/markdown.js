document.querySelectorAll(".js-markdown-element").forEach(element => {
    element.innerHTML = marked(element.innerHTML);
    element.addEventListener("change", e => {
        element.innerHTML = marked(element.innerHTML);
    });
});