document.querySelectorAll(".js-like-button").forEach(btn => {
    const heart = btn.getElementsByClassName("js-like-heart")[0];

    btn.addEventListener("click", async e => {
        if (heart) {
            currentLikeState = getCurrentLikeState(heart);
            let count = await like();
            if (count >= 0) {
                if (currentLikeState == "like") {
                    heart.innerHTML = '<path class="js-like-dislike" fill-rule="evenodd" d="M8 2.748l-.717-.737C5.6.281 2.514.878 1.4 3.053c-.523 1.023-.641 2.5.314 4.385.92 1.815 2.834 3.989 6.286 6.357 3.452-2.368 5.365-4.542 6.286-6.357.955-1.886.838-3.362.314-4.385C13.486.878 10.4.28 8.717 2.01L8 2.748zM8 15C-7.333 4.868 3.279-3.04 7.824 1.143c.06.055.119.112.176.171a3.12 3.12 0 0 1 .176-.17C12.72-3.042 23.333 4.867 8 15z" />';
                }
                else {
                    heart.innerHTML = '<path class="js-like-like" fill-rule="evenodd" d="M8 1.314C12.438-3.248 23.534 4.735 8 15-7.534 4.736 3.562-3.248 8 1.314z" />';
                }
            }
        }
    });
});

function getCurrentLikeState(heart) {
    if (heart.getElementsByClassName("js-like-like").length) {
        return "like";
    }
    return "dislike";
}
async function like() {
    let itemId = document.getElementById("itemId").value;
    let count = -1;

    const response = await fetch("api/like?itemId=" + itemId, {
        method: "GET",
        headers: { "Accept": "application/json" }
    });
    if (response.ok) {
        count = await response.json();
        setBadge(count);
    }
    return count;
}
function setBadge(count) {
    let countBadge = document.getElementsByClassName("js-like-count-label")[0];
    if (countBadge) {
        countBadge.innerHTML = count;
    }
}