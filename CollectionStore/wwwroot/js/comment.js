const hubConnection = new signalR.HubConnectionBuilder().withUrl("/comment").build();

hubConnection.on("SendComment", (userName, message, date) => {
    addComment(userName, message, date);
});

hubConnection.on("ConnectionNotify", () => {
    let itemId = document.getElementById("itemId").value;
    hubConnection.invoke("AddConnectionToGroup", itemId);
});

document.querySelectorAll(".js-comment-send-button").forEach(btn => {
    const input = btn.closest(".js-comment-form").getElementsByClassName("js-comment-input")[0];

    btn.addEventListener("click", e => {
        if (input) {
            let itemId = document.getElementById("itemId").value;
            hubConnection.invoke("SendComment", input.value, itemId);
            input.value = "";
        }
    });
});

function addComment(userName, message, date) {
    let commentBlock = document.getElementsByClassName("js-comment-block")[0];
    if (commentBlock) {
        let div = document.createElement("div");
        div.classList.add("px-2");

        let userNameLink = document.createElement("a");
        userNameLink.classList.add("card-link", "font-weight-bold");
        userNameLink.setAttribute("href", `Profile?userName=${userName}`);
        userNameLink.innerHTML = userName;

        let messageDiv = document.createElement("div");
        messageDiv.style = "white-space: pre-wrap";
        messageDiv.classList.add("card-text");
        messageDiv.innerHTML = message;

        let dateDiv = document.createElement("div");
        dateDiv.classList.add("card-text", "text-muted");
        dateDiv.innerHTML = `<small>${date}</small>`;

        div.appendChild(userNameLink);
        div.appendChild(messageDiv);
        div.appendChild(dateDiv);

        commentBlock.appendChild(document.createElement("hr"));
        commentBlock.appendChild(div);
    }
}

hubConnection.start();