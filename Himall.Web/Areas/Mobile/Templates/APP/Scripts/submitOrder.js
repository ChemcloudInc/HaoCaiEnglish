
var paymentShown = false;

$('#submit-order').click(function () {
        var skuIds = QueryString('skuIds');
        var counts = QueryString('counts');
        var action = "SubmitOrder";
        
        var integral = 0;
        if ($("#userIntegrals").is(':checked'))
        {
            integral = $("#userIntegrals").val();
            integral = isNaN(integral) ? 0 : integral;
        }
        var couponIds = "";
        $('input[name="couponIds"]').each(function (i,e) {
            couponIds = couponIds + $(e).val() + ',';
        })
        if (couponIds != '') {
            couponIds = couponIds.substring(0, couponIds.length - 1);
        }
        var recieveAddressId = $('#shippingAddressId').val();
        recieveAddressId = parseInt(recieveAddressId);
        recieveAddressId = isNaN(recieveAddressId) ? null : recieveAddressId;
        if (!recieveAddressId)
            $.dialog.alert('请选择或新建收货地址');
        else {
            var loading = showLoading();
            $.post('/'+areaName + '/Order/SubmitOrder', { skuIds: skuIds, counts: counts, recieveAddressId: recieveAddressId,couponIds:couponIds,integral:integral }, function (result) {
                if (result.success) {
                    if (result.realTotalIsZero) {
                        loading && loading.close();
                        $.dialog.confirm('您确定用积分抵扣全部金额吗？', function () {
                            ajaxRequest({
                                type: 'POST',
                                url: '/' + areaName + '/Order/PayOrderByIntegral',
                                param: { orderIds: result.orderIds.toString() },
                                dataType: 'json',
                                success: function (data) {
                                    if (data.success == true) {
                                        $.dialog.succeedTips('支付成功！', function () {
                                            location.href = '/' + areaName + '/Member/Orders';
                                        },0.5);
                                        
                                    }
                                },
                                error: function (data) { $.dialog.tips('支付失败,请稍候尝试.', null, 0.5); }
                            });
                        });
                    }
                    else {
                        $.post('/' + areaName + '/payment/get', { orderIds: result.orderIds.toString() },
                            function (payments) {
                                loading && loading.close();
                                if (payments.length > 0) {
                                    paymentShown = true;//标记已经显示支付方式
                                    var html = '';
                                    $.each(payments, function (i, payment) {
                                        html += '<a class="btn btn-success btn-block" href="' + payment.url + '">' + payment.name + '</a>';
                                    });
                                    $('#paymentsChooser').html(html);

                                    $('.cover').fadeIn();
                                    $('.custom-dialog').show();
                                }
                                else {
                                    $.dialog.tips('没有可用的支付方式，请稍候再试', function () {
                                        location.href = "/" + areaName + '/member/orders';
                                    });
                                }
                            });
                    }
                }
                else {
                    loading && loading.close();
                    $.dialog.alert(result.msg);
                }
            });
        }
    });

$("#choiceAddr").click(function () {
    var url ='/' + areaName + '/Order/submit?skuIds=' + sku + '&counts=' + counts;
    location.href = '/' + areaName + '/Order/ChooseShippingAddress?returnURL=' + encodeURIComponent(url);
    
});

$("#addaddr").click(function () {
    var url = '/' + areaName + '/Order/submit?skuIds=' + sku + '&counts=' + counts;
    location.href = '/' + areaName + '/Order/EditShippingAddress?addressId=0&returnURL=' + encodeURIComponent(url);
})

$("#userIntegrals").click(function () {
    oldTotal = $("#allTotal").html().replace('¥', '');
    if ($(this).attr('checked')) {
        $("#allTotal").html('¥' + (+parseFloat(oldTotal) - parseFloat($("#integralPerMoney").val())));
    }
    else
        $("#allTotal").html('¥' + (+parseFloat(oldTotal) + parseFloat($("#integralPerMoney").val())));
})

$("#addaddr").click(function () {

})

    $('.cover').click(function () {
        $('.cover').fadeOut();
        $('.custom-dialog').hide();
        if (paymentShown) {//如果已经显示支付方式，则跳转到订单列表页面
            location.href = '/' + areaName + '/Member/Orders';
        }
    });

    if (typeof WeixinJSBridge == "undefined") {
        if (document.addEventListener) {
            document.addEventListener('WeixinJSBridgeReady', onBridgeReady, false);
        } else if (document.attachEvent) {
            document.attachEvent('WeixinJSBridgeReady', onBridgeReady);
            document.attachEvent('onWeixinJSBridgeReady', onBridgeReady);
        }
    }


