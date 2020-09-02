document.querySelectorAll(".js-additional-fields-add-button").forEach(button => {
    const block = button.closest(".js-additional-fields-block");
    const buttonLabel = block.getElementsByClassName("js-additional-fields-button-label")[0].value;
    const nameField = block.getElementsByClassName("js-additional-fields-name")[0];
    const typeField = block.getElementsByClassName("js-additional-fields-type")[0];

    button.addEventListener("click", e => {
        let table = block.getElementsByClassName("js-additional-fields-table")[0];
        if (!nameField.value) {
            alert("Please, fill the field name.");
        }
        else {
            if (!table) {
                table = createTable();
                block.appendChild(table);
            }
            let row = table.insertRow();
            fillRow(row, nameField.value, typeField.options[typeField.selectedIndex].text, buttonLabel);
            nameField.value = "";
            typeField.selectedIndex = 0;
        }
    });
});

function createTable() {
    let table = document.createElement("table");
    table.classList.add("table", "table-sm", "table-bordered", "text-center", "text-wrap", "js-additional-fields-table");
    table.createTHead().classList.add("thead-light");
    table.tHead.innerHTML = "<tr>\n<th scope=\"col\">Field name</th>\n<th scope=\"col\">Field type</th>\n<th scope=\"col\"></th>\n</tr>"
    table.style = "table-layout:fixed;";
    return table;
}

function fillRow(row, name, type, buttonLabel) {
    row.classList.add("js-additional-fields-table-row");
    let nameRow = row.insertCell(0);
    nameRow.innerHTML = name;
    nameRow.style = "word-wrap: break-word;";
    row.insertCell(1).innerHTML = type;
    row.insertCell(2).appendChild(createButton(buttonLabel));
}

function createButton(label) {
    let button = document.createElement("button");
    button.classList.add("btn", "btn-outline-danger", "btn-sm", "js-additional-fields-remove-button");
    button.innerHTML = label;
    button.addEventListener("click", () => removeButtonClicked(button));
    return button;
}

function removeButtonClicked(button) {
    let table = button.closest(".js-additional-fields-table");
    button.closest(".js-additional-fields-table-row").remove();
    console.log(table.rows.length);
    if (table.rows.length == 1) {
        table.parentNode.removeChild(table);
    }
}