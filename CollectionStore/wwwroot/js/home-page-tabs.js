$(document).ready(() => {
    const tabId = sessionStorage.getItem("tabId");
    if (tabId) {
        $(`#${tabId}`).tab('show');
    }

    $('a[data-toggle="tab"]').on("show.bs.tab", e => sessionStorage.setItem("tabId", e.target.id));
})