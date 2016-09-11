$(function () {
    if (pwdflag.toLowerCase() == 'true') {//显示二维码
        JumpStep(2);
        checkScanState();
    }
    $('#submitApply').click(function () {
        var reg = /^[0-9]+([.]{1}[0-9]{1,2})?$/;
        if (!reg.test($('#inputMoney').val())) {
            $.dialog.alert("The characters of amount must be alphanumeric");
            return;
        }
        if (parseFloat(balance) < parseFloat($('#inputMoney').val())) {
            $.dialog.alert("The amount of withdrawals must be less than the balance");
            return;
        }
        if (parseFloat($('#inputMoney').val()) < 1) {
            $.dialog.alert("Withdraw amount must be greater than 1");
            return;
        }
        var loading = showLoading();
        $.post('ApplyWithDrawSubmit', { withdrawtype: $('#withdrawtype').text(), myaccount: $('#myaccount').text(), nickname: $('#nikename').text(), amount: parseFloat($('#inputMoney').val()), pwd: $('#payPwd').val() },
            function (result) {
                loading.close();
                if (result.success) {
                    $.dialog.succeedTips('Commit Successful!', function () {
                        JumpStep(4);
                    });
                }
                else {
                    $.dialog.errorTips(result.msg);
                }
            }
        );
    });
})
var opid = '';
function checkScanState() {
    $.getJSON('/ScanState/GetState', { sceneid: sceneid }, function (result) {
        if (result.success) {
            opid = result.data.OpenId;
            $('#nikename').text(result.data.NickName);
            $.dialog.succeedTips('扫码成功!', function () {
                JumpStep(3);
            });
        }
        else {
            setTimeout(checkScanState, 0);
        }
    });
}
function JumpStep(step) {
    $('div[name="stepname"]').hide();
    switch (step) {
        case 1:
            $('#stepnav').hide();
            $('#step' + step).show();
            break;
        case 2:
            $('#stepnav').show();
            $('#step' + step).show();
            break;
        case 3:
            $('#stepnav').show();
            $('div[name="step3"]').removeClass('todo').addClass('active');
            $('div[name="step3"]').prev().removeClass('active').addClass('done');
            $('#step' + step).show();
            break;
        case 4:
            $('#stepnav').show();
            $('div[name="step4"]').removeClass('todo').addClass('active');
            $('div[name="step4"]').prev().removeClass('active').addClass('done');
            $('#step' + step).show();
            break;
    }
}
