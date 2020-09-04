document.querySelectorAll(".js-additional-fields-add-button").forEach(button => {
    const block = button.closest(".js-additional-fields-block");
    const nameField = block.getElementsByClassName("js-additional-fields-name")[0];
    const typeField = block.getElementsByClassName("js-additional-fields-type")[0];
    let buttonLabel = getLabel(block, "js-additional-fields-button-label", "Remove");
    let nameLabel = getLabel(block, "js-additional-fields-name-label", "Field name");
    let typeLabel = getLabel(block, "js-additional-fields-type-label", "Field type");;

    button.addEventListener("click", e => {
        let table = block.getElementsByClassName("js-additional-fields-table")[0];
        if (!nameField.value) {
            alert("Please, fill the field name.");
        }
        else {
            if (!table) {
                table = createTable(nameLabel, typeLabel);
                block.appendChild(table);
            }
            let row = table.insertRow();
            fillRow(row, nameField.value, typeField, buttonLabel);
            nameField.value = "";
            typeField.selectedIndex = 0;
        }
    });
});

function getLabel(block, className, defaultValue) {
    return (block.getElementsByClassName(className)[0].value ?
        block.getElementsByClassName(className)[0].value : 
        defaultValue);
}

function createTable(nameLabel, typeLabel) {
    let table = document.createElement("table");
    table.classList.add("table", "table-sm", "table-bordered", "text-center", "text-wrap", "js-additional-fields-table");
    table.createTHead().classList.add("thead-light");
    table.tHead.innerHTML = `<tr>\n<th scope=\"col\">${nameLabel}</th>\n<th scope=\"col\">${typeLabel}</th>\n<th scope=\"col\"></th>\n</tr>`
    table.style = "table-layout:fixed;";
    return table;
}

function fillRow(row, name, typeField, buttonLabel) {
    row.classList.add("js-additional-fields-table-row");
    let nameCell = row.insertCell(0);
    nameCell.innerHTML = `${name}<input type="hidden" name="FieldNames" value="${name}"/>`;
    nameCell.style = "word-wrap: break-word;";
    let typeCell = row.insertCell(1);
    typeCell.innerHTML = `${typeField.options[typeField.selectedIndex].text}<input type="hidden" name="FieldTypesIds" value="${typeField.value}"/>`;
    typeCell.style = "word-wrap: break-word;";
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