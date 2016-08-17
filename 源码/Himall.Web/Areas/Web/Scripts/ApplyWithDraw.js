//$(function () {
//    //if (pwdflag.toLowerCase()=='true')
//    //{//显示二维码
//    //    JumpStep(2);
//    //    checkScanState();
//    //}
//    $('#submitApply').click(function () {
//        var reg = /^[0-9]+([.]{1}[0-9]{1,2})?$/;
//        if (!reg.test($('#inputMoney').val()))
//        {
//            $.dialog.alert("提现金额不能为非数字字符");
//            return;
//        }
//        if (parseFloat(balance) < parseFloat($('#inputMoney').val()))
//        {
//            $.dialog.alert("提现金额不能超出可用金额");
//            return;
//        }
//        if (parseFloat($('#inputMoney').val()) < 1) {
//            $.dialog.alert("提现金额不能小于1");
//            return;
//        }
//        var s1 = document.getElementById("withdrawtype").innerHTML;
//        var s2 = document.getElementById("myaccount").innerHTML;
//        var s3 = document.getElementById("nikename").innerHTML;
//        var loading = showLoading();
//        $.post('ApplyWithDrawSubmit', { withdrawtype: s1, myaccount: s2, nickname: s3, amount: parseFloat($('#inputMoney').val()), pwd: $('#payPwd').val() },
//            function (result) {
//                loading.close();
//                if (result.success)
//                {
//                    $.dialog.succeedTips('提交成功!', function () {
//                        JumpStep(4);
//                    });
//                }
//                else {
//                    $.dialog.errorTips(result.msg);
//                }
//            }
//        );
//    });
//})
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
function JumpStep(step)
{
    $('div[name="stepname"]').hide();
    switch(step)
    {
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
