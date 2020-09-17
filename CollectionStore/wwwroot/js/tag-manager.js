document.querySelectorAll(".js-tag-manager-input").forEach(input => {
    const container = input.closest(".js-tag-manager-container");
    
    input.addEventListener("keypress", event => {
        if (event.keyCode == 13) {
            event.preventDefault();
            if (input.value) {
                addTag(input.value, container);
                input.value = "";
            }
        }
    });
});

function addTag(content, container) {
    let list = container.getElementsByClassName("js-tag-manager-list")[0];
    if (!list) {
        list = addList(container);
    }
    addTagToList(content, list);
}

function addList(container) {
    let list = document.createElement("ul");
    list.classList.add("list-inline", "js-tag-manager-list");
    container.appendChild(list);
    return list;
}

function addTagToList(content, list) {
    let tag = document.createElement("li");
    tag.classList.add("list-inline-item", "m-1", "js-tag-manager-tag");

    let badge = document.createElement("div");
    badge.classList.add("badge", "badge-secondary", "p-1");

    let span = document.createElement("span");
    span.style = "vertical-align: middle;";
    span.innerHTML = content;

    badge.appendChild(span);
    badge.appendChild(createRemoveButton());

    tag.appendChild(badge);

    list.appendChild(tag);
    return tag;
}

function createRemoveButton() {
    let btn = document.createElementNS("http://www.w3.org/2000/svg", "svg");
    btn.onclick = (event) => {
        let tag = event.target.closest(".js-tag-manager-tag");
        tag.remove();
    };
    btn.classList.add("ml-1");
    btn.style = "cursor: pointer; vertical-align: middle;";
    setRemoveButtonAttributes(btn);
    btn.innerHTML = '<path fill-rule="evenodd" d="M16 8A8 8 0 1 1 0 8a8 8 0 0 1 16 0zM5.354 4.646a.5.5 0 1 0-.708.708L7.293 8l-2.647 2.646a.5.5 0 0 0 .708.708L8 8.707l2.646 2.647a.5.5 0 0 0 .708-.708L8.707 8l2.647-2.646a.5.5 0 0 0-.708-.708L8 7.293 5.354 4.646z" />';
    return btn;
}

function setRemoveButtonAttributes(btn) {
    btn.setAttribute("width", "1.2em");
    btn.setAttribute("height", "1.2em");
    btn.setAttribute("viewBox", "0 0 16 16");
    btn.setAttribute("fill", "currentColor");
}

function removeTag(btn) {
    let tag = btn.closest(".js-tag-manager-tag");
    console.log(tag);
}