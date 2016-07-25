

var skuId = new Array(3);

//chooseResult();
function chooseResult() {
    //已选择显示
    var str = '<em>已选择&nbsp;</em>';
    var len = $('#choose li .dd .selected').length;
    for (var i = 0; i < len; i++) {
        if (i < len - 1)
            str += '<strong>“' + $('#choose li .dd .selected a').eq(i).text() + '”</strong>，';
        else
            str += '<strong>“' + $('#choose li .dd .selected a').eq(i).text() + '”</strong>';
        var index = parseInt($('#choose li .dd .selected').eq(i).attr('st'));
        skuId[index] = $('#choose li .dd .selected').eq(i).attr('cid');
    }
    //console.log(skuId);
    $('#choose-result .dd').html(str)

    //请求Ajax获取价格
    if (len === $(".choose-sku").length) {
        var gid = $("#gid").val();
        var sku = '';
        for (var i = 0; i < skuId.length; i++) {
            sku += ((skuId[i] == undefined ? 0 : skuId[i]) + '_');
        }
        if (sku.length === 0) { sku = "0_0_0_"; }
    }
}

//转换0
function parseFloatOrZero(n) {
    result = 0;
    try {
        if (n.length < 1) n = 0;
        if (isNaN(n)) n = 0;
        result = parseFloat(n);
    } catch (ex) {
        result = 0;
    }
    return result;
}

$(function () {

    $("#buy-num").blur(function () {
        var max = parseInt($("#maxSaleCount").val());
        if (parseInt($('#buy-num').val()) < 0) {
            $.dialog.errorTips('购买数量必须大于零');
            $('#buy-num').val(1);
        }
        if (parseInt($('#buy-num').val()) > max) {
            $.dialog.errorTips('每个ID限购 ' + max + '件');
            $('#buy-num').val(max);
        }
    });

    //购买数量加减
    $('.wrap-input .btn-reduce').click(function () {
        if (parseInt($('#buy-num').val()) > 1) {
            $('#buy-num').val(parseInt($('#buy-num').val()) - 1);
        }
    });
    $('.wrap-input .btn-add').click(function () {
        var max = parseInt($("#maxSaleCount").val());
        if (max < parseInt($('#buy-num').val()) + 1) {
            $.dialog.errorTips('每个ID限购 ' + max +'件');
        } else {
            $('#buy-num').val(parseInt($('#buy-num').val()) + 1);
        }
        //alert(parseInt($('#buy-num').val())+1)
    });

    $("#easyBuyBtn").click(function () {
        var has = $("#has").val();
        var dis = $(this).parent("#choose-btn-append").hasClass('disabled');
        if (has != 1 || dis) return;
        var len = $('#choose li .dd .selected').length;
        if (len === $(".choose-sku").length) {
            var sku = getskuid();
            var num = $("#buy-num").val();
            location.href = "/Order/EasyBuyToOrder?skuId=" + sku + "&count=" + num;
            //   alert('SKUId：'+sku+'，购买数量：'+num);
        } else {
            $.dialog.errorTips('请选择商品规格');

        }
    });

    function checkLogin(callBack) {
        var memberId = $.cookie('Himall-User');
        if (memberId) {
            callBack();
        }
        else {
            $.fn.login({}, function () {
                callBack(function () { location.reload(); });
            }, './', '', '/Login/Login');
        }
    }



    //导航切换
    $('.tab .comment-li').click(function () {
        $('#product-detail .mc').hide();
        $(this).addClass('curr').siblings().removeClass('curr');
        $(document).scrollTop($('#comment').offset().top - 52);
    });
    $('.tab .consult-li').click(function () {
        $('#product-detail .mc').hide();
        $(this).addClass('curr').siblings().removeClass('curr');
        $(document).scrollTop($('#consult').offset().top - 52);
    });
    $('.tab .goods-li').click(function () {
        $('#product-detail .mc').show();
        $(this).addClass('curr').siblings().removeClass('curr');
        $(document).scrollTop($('#product-detail').offset().top);
    });

    //导航浮动
    $(window).scroll(function () {
        if ($(document).scrollTop() >= $('#product-detail').offset().top)
            $('#product-detail .mt').addClass('nav-fixed');
        else
            $('#product-detail .mt').removeClass('nav-fixed');
    });


    $("#shopInSearch").click(function () {
        var start = isNaN(parseFloat($("#sp-price").val())) ? 0 : parseFloat($("#sp-price").val());
        var end = isNaN(parseFloat($("#sp-price1").val())) ? 0 : parseFloat($("#sp-price1").val());
        var shopid = $("#shopid").val();

        var keyword = $("#sp-keyword").val();
        if (keyword.length === 0 && start == end) {
            $.dialog.errorTips('请输入关键字或者价格区间');
            return;
        }
        location.href = "/Shop/Search?pageNo=1&sid=" + shopid + "&keywords=" + keyword + "&startPrice=" + start + "&endPrice=" + end;
    });

    //关注商品
    $("#justBuy").click(function () {
        checkLogin(function (func) {
            justBuy(func);
        });
    });



    function justBuy(callBack) {
        var has = $("#has").val();
        var dis = $("#justBuy").hasClass('disabled');
        if (has != 1 || dis) return;
        if (dis)
            return false;
        var len = $('#choose li .dd .selected').length;
        if (len === $(".choose-sku").length) {
            var sku = getskuid();
            var num = $("#buy-num").val();
            $.post('/LimitTimeBuy/CheckLimitTimeBuy', { skuIds: sku, counts: num }, function (result) {
                if (result.success) {
                    location.href = "/Order/SubmitByProductId?skuIds=" + sku + "&counts=" + num;
                } else if(result.remain<=0){
                    $.dialog.errorTips("亲，限购" + result.maxSaleCount + "件，不能再买了哦");
                } else {
                    $.dialog.errorTips("亲，限购" + result.maxSaleCount + "件，您最多还能买" + result.remain + "件");
                }
            });
        } else {
            $.dialog.errorTips("请选择商品规格！");
        }
    }

    function checkLogin(callBack) {
        var memberId = $.cookie('Himall-User');
        if (memberId) {
            callBack();
        }
        else {
            $.fn.login({}, function () {
                callBack(function () { location.reload(); });
            }, '', '', '/Login/Login');
        }
    }


    function getskuid() {
        var gid = $("#gid").val();
        var sku = '';
        for (var i = 0; i < skuId.length; i++) {
            sku += ((skuId[i] == undefined ? 0 : skuId[i]) + '_');
        }
        if (sku.length === 0) { sku = "0_0_0_"; }
        sku = gid + '_' + sku.substring(0, sku.length - 1);
        return sku;
    }


    //加入购物车
    //$("#justBuy").click(function () {
    //    var has = $("#has").val();
    //    var dis = $(this).hasClass('disabled');
    //    if (has != 1 || dis) return;
    //    var len = $('#choose li .dd .selected').length;
    //    if (len === $(".choose-sku").length) {
    //        var sku = getskuid();
    //        var num = $("#buy-num").val();
    //        $.post('/LimitTimeBuy/CheckLimitTimeBuy', { skuIds: sku, counts: num }, function (result) {
    //            if (result.success) {
    //                location.href = "/Order/SubmitByProductId?skuIds=" + sku + "&counts=" + num;
    //            } else {
    //                $.dialog.errorTips("该限时购活动，您最多还可以购买 "+result.remain+" 件");
    //            }
    //        });
    //    } else {
    //        $.dialog.errorTips("请选择商品规格！");
    //    }
    //});
});