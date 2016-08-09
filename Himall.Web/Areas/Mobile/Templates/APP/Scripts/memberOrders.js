    var orderStatus = QueryString("orderStatus");

    $(function () {
        if (orderStatus == 1)
            $('.clearfix li').eq(1).addClass("active");
        else if (orderStatus == 3)
            $('.clearfix li').eq(2).addClass("active");
        else if (orderStatus == 5)
            $('.clearfix li').eq(3).addClass("active");
        else
            $('.clearfix li').eq(0).addClass("active");
        loadProducts(1, orderStatus);
    });

    function userOrders( status )
    {
        location.href = '/' + areaName + '/Member/Orders?orderStatus=' + status;
    }

    var page = 1;

    $(window).scroll(function () {
        var scrollTop = $(this).scrollTop();
        var scrollHeight = $(document).height();
        var windowHeight = $(this).height();

        if (scrollTop + windowHeight >= scrollHeight) {
            $('#autoLoad').removeClass('hide');
            loadProducts(++page);
        }
    });

    function loadProducts( page, status )
    {
        var url = '/' + areaName + '/Member/GetUserOrders?orderStatus=' + orderStatus;
        $.post(url, { orderStatus: status, pageNo: page, pageSize: 4 }, function (result) {
            $('#autoLoad').addClass('hide');
            var html = '';
            if (result.length > 0) {
                $.each(result, function (i, item) {
                    if (!(status == 5 && item.commentCount != 0)) {
                        html += '<li><a href=""><h6><a>' + item.shopname + '<span class="pull-right">' + item.status + '</span></h6></a>';
                        $.each(item.itemInfo, function (j, orderItem) {
                            html += '<div class="order-goods clearfix"><a href="/' + areaName + '/Order/Detail?id=' + item.id + '"><img width="50" src="' + orderItem.image + '" /><p>' + orderItem.productName + '</p>';
                            html += '<p class="mt10"><span class="red">¥' + orderItem.price + '</span><em class="pull-right">' + orderItem.count + (orderItem.Unit == null ? '' : orderItem.Unit) + '</em></p></a></div>';
                        });
                        html += '<p class="order-text"><a href="/' + areaName + '/order/detail/' + item.id + '"><span>共 ' + item.productCount + ' 件商品</span> <span>实付： ¥' + item.orderTotalAmount + '</span></a></p>';
                        switch (item.orderStatus) {
                            case 1:
                                html += '<p class="order-btn"><a class="btn btn-primary btn-sm" pay orderTotal="'+item.orderTotalAmount+'" orderId="' + item.id + '">付款</a><a class="btn btn-default btn-sm" onclick="CancelOrder(' + item.id + ')">取消订单</a></p>';
                                break;
                            case 3:
                                html += '<p class="order-btn"><a class="btn btn-default btn-sm" href="/' + areaName + '/order/expressInfo?orderId=' + item.id + '">查看物流</a><a class="btn btn-primary btn-sm" onclick="ConfirmOrder(' + item.id + ')">确认收货</a></p>';
                                break;
                            case 5:
                                html += '<p class="order-btn"><a class="btn btn-default btn-sm" href="/' + areaName + '/order/expressInfo?orderId=' + item.id + '">查看物流</a>';
                                if (item.commentCount == 0)
                                    html += '<a class="btn btn-primary btn-sm" href="/' + areaName + '/comment?orderId=' + item.id + '">评论</a>';
                                html += '</p>';
                                break;
                            default:
                                break;
                        }
                    }
                });
                $('.order-list').append(html);
            }
            else {
                $('#autoLoad').html('没有更多订单了').removeClass('hide');
            }
        });
    }

    function ConfirmOrder(orderId) {

        $.dialog.confirm("你确定收到货了吗？", function () { Confirm(orderId); });
    }

    function Confirm(orderId) {
        $.ajax({
            type: 'post',
            url: '/' + areaName + '/Order/ConfirmOrder',
            dataType: 'json',
        data: { orderId: orderId },
        success: function (d) {
            if (d.success) {
                $.dialog.succeedTips("确认成功！", function () {
                    window.location.href = window.location.href;
                }, 1);
            }
            else {
                $.dialog.errorTips("确认失败！", '', 2);
            }
        }
    });
    }

    function CancelOrder(orderId) {
        $.dialog.confirm("确定取消该订单吗？", function () { Cancel(orderId); });
    }
    function Cancel(orderId) {
        $.ajax({
            type: 'post',
            url: '/' + areaName + '/Order/CloseOrder',
            dataType: 'json',
        data: { orderId: orderId },
        success: function (d) {
            if (d.success) {
                $.dialog.succeedTips("取消成功！", function () {
                    window.location.href = window.location.href;
                }, 1);
            }
            else {
                $.dialog.errorTips("取消失败！", '', 2);
            }
        }
    });
    }

   
    $('.order-list').on('click', 'a[pay]', function () {
        var loading = showLoading();
        var orderId = $(this).attr('orderId');
        if ($(this).attr('orderTotal')==0) {
            loading && loading.close();
            $.dialog.confirm('您确定用积分抵扣全部金额吗？', function () {
                ajaxRequest({
                    type: 'POST',
                    url: '/' + areaName + '/Order/PayOrderByIntegral',
                    param: { orderIds: orderId },
                    dataType: 'json',
                    success: function (data) {
                        if (data.success == true) {
                            $.dialog.succeedTips('支付成功！', function () {
                                location.href = '/' + areaName + '/Member/Orders';
                            }, 0.5);

                        }
                    },
                    error: function (data) { $.dialog.tips('支付失败,请稍候尝试.', null, 0.5); }
                });
            });
        }
        else {
            $.post('/' + areaName + '/payment/get', { orderIds: orderId },
                           function (payments) {
                               loading && loading.close();
                               if (payments.length > 0) {
                                   var html = '';
                                   $.each(payments, function (i, payment) {
                                       html += '<a class="btn btn-success btn-block" href="' + payment.url + '">' + payment.name + '</a>';
                                   });
                                   $('#paymentsChooser').html(html);
                                   $('.cover').fadeIn();
                                   $('.custom-dialog').show();
                               }
                               else {
                                   $.dialog.tips('没有可用的支付方式，请稍候再试');
                               }
                           });
        }
    });

    $('.cover').click(function () {
        $('.cover').fadeOut();
        $('.custom-dialog').hide();

    });