
var skuId = new Array(3);
var pid = $("#gid").val();

//关注
$(function () {

    //var addFavorite = QueryString('addFavorite');
    //if (addFavorite)
    //    addFavoriteFun();

    returnFavoriteHref = "/" + areaName + "/Product/Detail/" + pid;
    //returnFavoriteHref = encodeURIComponent(returnFavoriteHref);

    $("#favoriteProduct").click(function () {
        checkLogin(returnFavoriteHref,function () {
            addFavoriteFun();
        }, shopid);
    });

    //购买数量加减
    $('.wrap-num .glyphicon-minus').click(function () {
        if (parseInt($('#buy-num').val()) > 1) {
            $('#buy-num').val(parseInt($('#buy-num').val()) - 1);
        }
    });
    $('.wrap-num .glyphicon-plus').click(function () {
        $('#buy-num').val(parseInt($('#buy-num').val()) + 1);
    });

    $("#buy-num").blur(function () {
        checkBuyNum();
    });

    $("#easyBuyBtn").click(function () {
        var has = $("#has").val();
        if (has != 1) return;
        var len = $('#choose .spec .selected').length;
        if (len === $(".spec").length) {
            returnHref = "/" + areaName + "/Product/Detail/" + pid ;
            returnHref = encodeURIComponent(returnHref);
            var sku = getskuid();
            if (checkBuyNum()) {
                var num = $("#buy-num").val();
                num = parseFloatOrZero(num);
                if (num <=0) {
                    $.dialog.errorTips("请输入购买数量！");
                    return false;
                }
                var isLTBuy = false;
                try {
                    isLTBuy = isLimitTimeBuy || false;
                } catch (ex) {
                    isLTBuy = false;
                }
                if (isLTBuy == true) {
                    var maxnum = $("#maxSaleCount").val();
                    maxnum = parseFloatOrZero(maxnum);
                    if (maxnum > 0) {
                        if (num > maxnum) {
                            $.dialog.errorTips("每ID限购" + maxnum + "件！");
                            return false;
                        }
                    } else {
                        if (num < 0) {
                            $.dialog.errorTips("请输入购买数量！");
                            return false;
                        }
                    }
                }
                checkLogin(returnHref, function () {
                    location.href = "/" + areaName + "/Order/Submit?skuIds=" + sku + "&counts=" + num;
                }, shopid);
            }
        }
        else {
            $.dialog.errorTips('请选择商品规格');

        }
    });

    $("#addToCart").click(function () {
        var has = $("#has").val();
        if (has != 1) return;
        var len = $('#choose .spec .selected').length;
        if (len === $(".spec").length) {
            returnHref = "/" + areaName + "/Product/Detail/" + pid ;
            returnHref = encodeURIComponent(returnHref);
            var sku = getskuid();
            if (checkBuyNum()) {
                var num = $("#buy-num").val();
                if (num.length == 0) {
                    $.dialog.errorTips("请输入购买数量！");
                    return false;
                }
                checkLogin(returnHref, function () {
                    addToCart(sku, num);
                }, shopid);
            }

        }
        else {
            $.dialog.errorTips('请选择商品规格');

        }
    });
    LogProduct();
})

function LogProduct() {
    $.ajax({
        type: 'post',
        url: '/' + areaName + '/Product/LogProduct',
        data: { pid: pid },
        dataType: 'json',
        cache: false,// 开启ajax缓存
        success: function (data) {
            if (data) {
                //console.log(data);
            }
        },
        error: function (e) {
            //alert(e);
        }
    });
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

function checkBuyNum() {
    var num = 0;
    var result = true;
    try {
        num = parseInt($("#buy-num").val());
    } catch (ex) {
        num = 0;
    }
    if (num < 1) {
        $.dialog.errorTips('购买数量有误');
        $('#buy-num').val(1);
        result = false;
    }
    return result;
}

function chooseResult() {
    //已选择显示
    var len = $('#choose .spec .selected').length;
    for (var i = 0; i < len; i++) {
        var index = parseInt($('#choose .spec .selected').eq(i).attr('st'));
        skuId[index] = $('#choose .spec .selected').eq(i).attr('cid');
    }

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

function addFavoriteFun() {
    var url;
    var css;
    if ($('#favoriteProduct').attr("class").indexOf("red") >= 0)
    {
        url = "/" + areaName + "/Product/DeleteFavoriteProduct";
        css = 'addfav cell iconfont icon-fav-c';
    }
    else
    {
        url = "/" + areaName + "/Product/AddFavoriteProduct";
        css = 'addfav cell iconfont icon-fav-c red';
    }
    $.post(url, { pid: $("#gid").val() }, function (result) {
        if (result.success == true) {
            $('#favoriteProduct').removeClass().addClass(css);
                $.dialog.succeedTips(result.msg,'',1);
        }
    });
}

function addToCart(sku,num) {
    $.post('/' + areaName + '/cart/AddProductToCart', { skuId: sku, count: num }, function (result) {
        if (result.success == true) {
            $.dialog.succeedTips("添加成功！");
			$('.plus-one').text($('#buy-num').val()).css({ top: 0, opacity: 0, display: 'block' }).animate({ 'top': '-15px', 'opacity': '1' }, 500).fadeOut();
        }
        else
            $.dialog.errorTips(result.msg);
    })

}

function checkFirstSKUWhenHas() {
    $(".spec").each(function () {
        $(this).children("div:first").not(".disabled").find("span:first").trigger("click");
    });
}