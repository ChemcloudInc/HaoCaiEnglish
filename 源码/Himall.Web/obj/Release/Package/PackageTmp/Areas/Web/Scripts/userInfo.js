var currentUser;


$(function () {
    initUserInfo();
});

function initUserInfo() {
    try {
        $.ajax({
            type: 'post',
            url: '/userinfo/GetCurrentUserInfo',
            cache: false,
            async: true,
            data: {},
            dataType: "json",
            success: function (result) {
                if (result.success) {
                    $("#sayhello").html("Hi! " + result.name);
                    $(".login-bt .btn").hide();
                    $("#loginOut").show();
                }
                else {
                    $(".login-bt .btn").show();
                    $("#loginOut").hide();
                }
            },
            error: function () {
            }
        });


        //$.post('/userinfo/GetCurrentUserInfo', {}, function (result) {
        //    if (result.success) {
        //        $("#sayhello").html("Hi! " + result.name);
        //        $(".login-bt .btn").hide();
        //        $("#loginOut").show();
        //    }
        //    else {
        //        $(".login-bt .btn").show();
        //        $("#loginOut").hide();
        //    }
        //});
    }
    catch (e) {
        $("#sayhello").html("Hi! 你好");
        $(".login-bt .btn").show();
    }
}

function logout() {
    $.removeCookie('Himall-User', { path: '/' });
    $.removeCookie('Himall-SellerManager', { path: "/" });
    location.reload();
}