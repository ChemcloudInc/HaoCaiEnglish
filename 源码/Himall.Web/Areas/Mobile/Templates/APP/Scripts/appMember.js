$(document).ready(function () {
    // 登录
    var username = getCookie("HIMALL_USER");
    if (username == undefined || username == "") {// 如果web端没登录
        loadIframeURL("hishop://webGetSession/loadSessionid/null");
    }
});

function loadSessionid() {
    loadIframeURL("hishop://webLogin/openLogin/null");
}