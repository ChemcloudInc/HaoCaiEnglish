
$(function () { 
    var stae1, stae2, stae3;
    var pwdErrMsg = '密码不能为空！';
    $('#firstPwd').blur(function () {
        var d = $(this).val();
        if (d.length < 6) {
            $('#firstPwd').css({ borderColor: '#f60' });
            pwdErrMsg = '密码长度不能少于6位';
            stae2 = '';
        } else {
            $('#firstPwd').css({ borderColor: '#ccc' });
            stae2 = d;
            if ($('#secondPwd').val() != '' && $('#secondPwd').val() == $('#firstPwd').val()) {
                $('#secondPwd').css({ borderColor: '#ccc' });
                stae3 = d;
            }
            else {
                pwdErrMsg = '两次密码不一致！';
            }
        }
    });
    $('#secondPwd').blur(function () {
        var d = $(this).val();
        if (d == $('#firstPwd').val()) {
            $('#secondPwd').css({ borderColor: '#ccc' });
            stae3 = d;
        } else {
            $('#secondPwd').css({ borderColor: '#f60' });
            pwdErrMsg = '两次密码不一致！';
            stae3 = '';
        }
    });
    $('#submitPwd').bind('click', function () {
        //console.log(stae1)
        if (!stae2) {
            $('#firstPwd').css({ borderColor: '#f60' });
            $.dialog.alert(pwdErrMsg);
        }
        if (!stae3) {
            $('#secondPwd').css({ borderColor: '#f60' });
            $.dialog.alert(pwdErrMsg);
        }
        if (stae2 && stae3) {
            var loading = showLoading();
            $.ajax({
                type: 'post',
                url: 'SetPayPwd',
                data: { "pwd": stae3 },
                dataType: "json",
                success: function (data) {
                    loading.close();
                    if (data.success) {
                        $.dialog.succeedTips('设置成功！');
                        pwdflag = 'true';
                        $('#stepone').hide();
                        $('#steptwo').show();
                    }
                }
            });
        }
    });
    $('#btnWithDraw').click(function () {
        var userBalance = parseFloat($('#balanceValue').text());
        if (userBalance <= 0)
        {
            $.dialog.alert('可用金额为零，不能提现！');
            return;
        }
        if (areaName.toLowerCase() == 'm-weixin') {
            $("#covered").addClass("cover");
            //$(".steponeee").height($(".steponeee").width() * 120 / 141);
            if (pwdflag.toLowerCase() == 'true') {
                $('#steptwo').show();
            }
            else {
                $('#stepone').show();
            }
        }
        else {
            $.dialog.alert('请在平台微信公众号内进行提现，或登录平台PC端进行提现');
        }
    });
    $(".steponeee .close").click(function(){
        $(this).parent().hide();
        $("#covered").removeClass("cover");
    });
   

    var page = 1;

    $(window).scroll(function () {
        var scrollTop = $(this).scrollTop();
        var scrollHeight = $(document).height();
        var windowHeight = $(this).height();
        $('#autoLoad').show();
        if (scrollTop + windowHeight >= scrollHeight - 30) {

            loadRecords(++page);
        }
    });
});



function loadRecords(page) {
    var url = 'List';
    $.post(url, { page: page, rows: 15 }, function (result) {
        var html = '';
        var str = '';
        if (result.length > 0) {
            $.each(result, function (i, model) {
                if (parseFloat(model.Amount) > 0)
                {
                    str = '<td style="color:green">' + model.Amount + '</td>';
                }
                else {
                    str = '<td>' + model.Amount + '</td>';
                }
                html = [html
                    , '<tr>'
                    , str
                    , '<td>' + model.CreateTime + '</td>'
                    , '<td>' + model.Remark + '</td>'
                    , '</tr>'
                ].join('');
            });
            $('#ulList').append(html);
        }
        else {
            $('#autoLoad').html('没有更多记录了');
        }
    });

}