
$(function () {
    bindSubmitBtn();

    bindCheckCode();

    initUsenameBox();
    $('#loginname').focus();
});

function bindSubmitBtn() {
    $('#loginsubmit').click(function () {
        submit();
    });

    document.onkeydown = function () {
        if (event.keyCode == 13) {
            submit();
        }
    }
}

function initUsenameBox() {
    var defaultUsername = $.cookie('Himall-DefaultUserName');
    if (defaultUsername) {
        $('#loginname').val(defaultUsername);
        $('#password').focus();
    }
}

function submit() {
    var result = checkUsername() & checkPassword() & checkCheckCode();
    if (result) {
        var username = $('#loginname').val();
        var password = $('#password').val();
        var checkCode = $('#checkCodeBox').val();
        var keep = $('#autoLogin').attr('checked');
        keep = keep ? true : false;

        var loading = showLoading();
        $.post('/Login/Login', { username: username, password: password, checkCode: checkCode, keep: keep }, function (data) {
            loading.close();
            if (data.success) {//登录成功
                $.cookie('Himall-DefaultUserName', username, { path: "/", expires: 365 });
                var returnUrl = decodeURIComponent(QueryString('returnUrl')).replace('&amp;', '&');
                if (returnUrl)
                    location.href = returnUrl;
                else if (data.IsChildSeller == true) {
                    location.href = "/sellerAdmin";
                }
                else {
                    location.href = '/'; //跳转至买家中心
                }
            }
            else {
                var isFirstShowCheckcode = false;
                refreshCheckCode();
                if (data.errorTimes > data.minTimesWithoutCheckCode) {//需要验证码
                    if ($('#checkCodeArea').css('display') == 'none') {
                        isFirstShowCheckcode = true;
                        $('#checkCode_error').html(data.msg).show();
                    }

                    $('#checkCodeArea').show();
                    $('#autoentry').css('margin-top', 0);
                }
                else {
                    $('#checkCodeArea').hide();
                    $('#autoentry').removeAttr('style');
                }
                if (!isFirstShowCheckcode) {
                    $('#loginpwd_error').html(data.msg).show();
                    $('#password').focus();
                }
                else
                    $('#checkCodeBox').focus();

            }
        });
    }
}

function checkCheckCode() {
    var result = false;
    if ($('#checkCodeArea').css('display') == 'none')
        result = true;
    else {
        var checkCode = $('#checkCodeBox').val();
        var errorLabel = $('#checkCode_error');
        if (checkCode && checkCode.length == 4) {
            $.ajax({
                type: "post",
                url: "/login/checkCode",
                data: { checkCode: checkCode },
                dataType: "json",
                async: false,
                success: function (data) {
                    if (data.success) {
                        result = true;
                        errorLabel.hide();
                    }
                    else {
                        $('#checkCodeBox').focus();
                        errorLabel.html('Verification Code Error').show();
                    }
                }
            });
        }
        else {
            $('#checkCodeBox').focus();
            if (!checkCode)
                errorLabel.html('Please enter verification code').show();
            else
                errorLabel.html('Verification Code Error').show();
        }
    }
    return result;
}

function checkUsername() {
    var result = false;
    var username = $('#loginname').val();
    var loginError = $('#loginname_error');
    if (!username) {
        loginError.html('Please enter username').show();
    }
    else {
        result = true;
        loginError.hide();
    }
    return result;
}

function checkPassword() {
    var result = false;
    var password = $('#password').val();
    var passwordError = $('#loginpwd_error');
    if (!password) {
        passwordError.html('please enter password').show();
    }
    else {
        result = true;
        passwordError.hide();
    }
    return result;
};

function refreshCheckCode() {
    var path = $('#checkCodeImg').attr('src').split('?')[0];
    path += '?time=' + new Date().getTime();
    $('#checkCodeImg').attr('src', path);
    $('#checkCodeBox').val('');
}

function bindCheckCode() {
    $('#checkCodeImg,#checkCodeChange').click(function () {
        refreshCheckCode();
    });
}


function bindFocus() {
    $('#password').keydown(function () {
        $('#loginpwd_error').hide();
    });

    $('#loginname').keydown(function () {
        $('#loginpwd_error').hide();
    });

    $('#checkCodeBox').keydown(function () {
        $('#checkCode_error').hide();
    });

}

