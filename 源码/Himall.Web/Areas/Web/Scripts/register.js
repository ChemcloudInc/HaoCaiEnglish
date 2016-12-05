
$(function () {
    bindCheckCode();

    checkUserName();

    checkPassword();

    checkMobile();

    checkCheckCode();

    bindSubmit();

    $('#regName').focus();
});


function bindSubmit() {
    $('#registsubmit').click(function () {
        var result = checkValid();
        if (result) {
            var username = $('#regName').val(), password = $('#pwd').val();
            var email = $('#cellPhone').val();
            var introducer = $("#introducer").val();
            var loading = showLoading();
            $.post('/Register/RegisterUser', { username: username, password: password, email: email, introducer: introducer }, function (data) {
                loading.close();
                if (data.success) {
                    $.dialog.succeedTips("Register Success！", function () {
                        location.href = '/login';
                    }, 3);
                }
                else {
                    $.dialog.errorTips("Register Failed！" + data.msg);
                }
            });
        }
    });
}

function checkPassword() {

    $('#pwd').focus(function () {
        $('#pwd_info').show();
        $('#pwd_error').removeClass('error').addClass('focus').hide();
    }).blur(function () {
        $('#pwd_info').hide();
        checkPasswordIsValid();
    });

    $('#pwdRepeat').focus(function () {
        $('#pwdRepeat_info').show();
        $('#pwdRepeat_error').removeClass('error').addClass('focus').hide();

    }).blur(function () {
        $('#pwdRepeat_info').hide();
        checkRepeatPasswordIsValid();
    });

}

function checkUserName() {
    $('#regName').change(function () {
        var regName = $.trim($(this).val());
        if (!regName)
            $('#regName_error').show();
        else
            $('#regName_error').hide();
    }).focus(function () {
        $('#regName_info').show();
        $('#regName_error').hide();
    }).blur(function () {
        $('#regName_info').hide();
        checkUsernameIsValid();
    });
}

function bindCheckCode() {
    $('#checkCodeChangeBtn,#checkCodeImg').click(function () {
        var src = $('#checkCodeImg').attr('src');
        $('#checkCodeImg').attr('src', src);
    });
}


function checkValid() {
    
    return checkUsernameIsValid() & checkPasswordIsValid() & checkRepeatPasswordIsValid() & checkCheckCodeIsValid() & checkEmailIsValid() & checkAgreementIsValid();
}


function checkCheckCodeIsValid() {
    var checkCode = $('#checkCode').val();
    var errorLabel = $('#checkCode_error');
    checkCode = $.trim(checkCode);

    var result = false;
    if (checkCode && $('#cellPhone').length == 0) {
        $.ajax({
            type: "post",
            url: "/register/CheckCheckCode",
            data: { checkCode: checkCode },
            dataType: "json",
            async: false,
            success: function (data) {
                if (data.success) {
                    if (data.result) {
                        errorLabel.hide();
                        result = true;
                    }
                    else {
                        errorLabel.html('Verification code error').show();
                    }
                }
                else {
                    $.dialog.errorTips("Verification code validation error", '', 1);
                }
            }
        });
    }
    else if ($('#cellPhone').length > 0&&checkCode) {
        $.ajax({
            type: "post",
            url: "/register/CheckCode",
            data: { pluginId: "Himall.Plugin.Message.Email", code: checkCode, destination: $("#cellPhone").val() },
            dataType: "json",
            async: false,
            success: function (data) {
                if (data.success == true) {
                    errorLabel.hide();
                    result = true;
                }
                else {
                    errorLabel.html('verification code incorrect or timeout').show();
                }
            }
        });
    }
    else {
        errorLabel.html('Please enter verification code').show();
    }
    return result;
}

function checkCheckCode() {
    var errorLabel = $('#checkCode_error');
    $('#checkCode').focus(function () {
        errorLabel.hide();
    }).blur(function () {
        checkCheckCodeIsValid();
    });
}

function checkUsernameIsValid() {
    var result = false;
    var username = $('#regName').val();
    var errorLabel = $('#regName_error');
    var reg = /^[\u4E00-\u9FA5\@A-Za-z0-9\_\-]{4,20}$/;

    if (!username || username == 'username') {
        errorLabel.html('please enter user name').show();
    }
    //else if (!reg.test(username)) {
    //    errorLabel.html('Username must be alphanumeric & between 4-20 characters').show();
    //}
    else {
        $.ajax({
            type: "post",
            url: "/register/CheckUserName",
            data: { username: username },
            dataType: "json",
            async: false,
            success: function (data) {
                if (data.success) {
                    if (!data.result) {
                        errorLabel.hide();
                        result = true;
                    }
                    else {
                        errorLabel.html('UserName ' + username + ' is already').show();
                    }
                }
                else {
                    $.dialog.errorTips("UserName validation error", '', 1);
                }
            }
        });
    }
    return result;
}

function checkPasswordIsValid() {
    var result = false;

    //var reg = /^[\@A-Za-z0-9\!\#\$\%\^\&\*\.\~]{6,22}$/;
    var pwdTextBox = $('#pwd');
    var password = pwdTextBox.val();
    var reg = /^[^\s]{6,20}$/;
    var result = reg.test(password);
    //   var result = password.length >= 6 && password.length <= 20;

    if (!result) {
        $('#pwd_error').addClass('error').removeClass('focus').show();
    }
    else {
        $('#pwd_error').removeClass('error').addClass('focus').hide();
        result = true;
    }
    return result;
}

function checkRepeatPasswordIsValid() {
    var result = false;

    //var reg = /^[\@A-Za-z0-9\!\#\$\%\^\&\*\.\~]{6,22}$/;
    var pwdRepeatTextBox = $('#pwdRepeat');
    var repeatPassword = pwdRepeatTextBox.val(), password = $('#pwd').val();
    //var result = reg.test(password);

    var result = repeatPassword == password;

    if (!result) {
        $('#pwdRepeat_error').addClass('error').removeClass('focus').show();
    }
    else {
        $('#pwdRepeat_error').removeClass('error').addClass('focus').hide();
        result = true;
    }
    return result;
}

function checkAgreementIsValid() {
    var result = false;
    var errorLabel = $('#checkAgreement_error');
    if ($("#readme").attr("checked") == "checked") {
        errorLabel.hide();
        result = true;
    } else {
        errorLabel.html('Please carefully read and agree Terms and Conditions').show();
    }
    return result;
}

function reloadImg() {
    $("#checkCodeImg").attr("src", "/Register/GetCheckCode?_t=" + Math.round(Math.random() * 10000));
}

function checkMobile() {
    $('#cellPhone').change(function () {
        var cellPhone = $.trim($(this).val());
        if (!cellPhone)
            $('#cellPhone_error').show();
        else
            $('#cellPhone_error').hide();
    }).focus(function () {
        $('#cellPhone_info').show();
        $('#cellPhone_error').hide();
    }).blur(function () {
        $('#cellPhone_info').hide();
       // checkMobileIsValid();
    });
}

function checkEmailIsValid() {

    if ($('#cellPhone').length == 0) {
        return true;
    }
    var result = false;
    var cellPhone = $('#cellPhone').val();
    var errorLabel = $('#cellPhone_error');
    var reg = /^0?(13|15|18|14|17)[0-9]{9}$/;

    if (!cellPhone || cellPhone == 'Email') {
        errorLabel.html('Please enter email address').show();
    }
    else {
        $.ajax({
            type: "post",
            url: "/register/CheckEmail",
            data: { email: cellPhone },
            dataType: "json",
            async: false,
            success: function (data) {
                if (data.result == false) {
                    errorLabel.hide();
                    result = true;
                }
                else {
                    errorLabel.html('Email ' + cellPhone + ' is used').show();
                }
            }
        });
    }
    return result;
}

var delayTime = 60;
var delayFlag = true;
function countDown() {
    delayTime--;
    $("#sendMobileCode").attr("disabled", "disabled");
    $("#dyMobileButton").html('get it again after' + delayTime + 'seconds');
    if (delayTime == 1) {
        delayTime = 60;
        $("#mobileCodeSucMessage").removeClass().empty();
        $("#dyMobileButton").html("Get email verification code");
        $("#cellPhone_error").addClass("hide");
        $("#sendMobileCode").removeClass().addClass("btn").removeAttr("disabled");
        delayFlag = true;
    } else {
        delayFlag = false;
        setTimeout(countDown, 1000);
    }
}

function sendMobileCode() {
    $('#cellPhone_error').hide();
    if ($("#sendMobileCode").attr("disabled")) {
        return;
    }
    var errorLabel = $('#cellPhone_error');
    var mobile = $("#cellPhone").val();
    var reg = /^0?(13|15|18|14|17)[0-9]{9}$/;
    if (!mobile) {
        $("#cellPhone_error").removeClass().addClass("error").html("Please enter Mobile Number");
        $("#cellPhone_error").show();
        return;
    }
    /*if (!reg.test(mobile)) {
        $("#cellPhone_error").removeClass().addClass("error").html("手机号码格式有误，请输入正确的手机号");
        $("#cellPhone_error").show();
        return;
    }*/
    //$('#checkCode').removeClass("highlight2");
    // 检测手机号码是否存在
    //$.post('/Register/CheckMobile', { mobile: mobile }, function (data) {
    //    if (data.result == false) {
    //        errorLabel.hide();
    //        sendmCode();
    //    }
    //    else {
    //        errorLabel.html('手机号码 ' + mobile + ' 已经被占用').show();
    //    }
    //});

}
// 邮箱注册发送验证码target
function sendMobileCode() {
    if ($("#sendMobileCode").attr("disabled") || delayFlag == false) {
        return;
    }
  //  $("#sendMobileCode").attr("disabled", "disabled");

    jQuery.ajax({
        type: "post",
        url: "/Register/SendCode?pluginId=Himall.Plugin.Message.Email&destination=" + $("#cellPhone").val(),
        success: function (result) {
            
            if (result.success) {
                $("#cellPhone_error").hide();
                $("#dyMobileButton").html("get it again after 60 seconds");
                setTimeout(countDown, 1000);
                $("#sendMobileCode").removeClass().addClass("btn").attr("disabled", "disabled");
                $("#checkCode").removeAttr("disabled");
            } else {
                $.dialog.errorTips('Failed to send verification code,' + result.msg);
            }
        }
    });
}
   

