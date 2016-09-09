

var skuId = new Array(3);

//chooseResult();
function chooseResult() {
    //已选择显示
    var str = '<em>Selected &nbsp;</em>';
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
    if (len === $('#choose').find(".choose-sku").length) {
        var gid = $("#gid").val();
        var sku = '';
        for (var i = 0; i < skuId.length; i++) {
            sku += ((skuId[i] == undefined ? 0 : skuId[i]) + '_');
        }
        var newSKU = gid;
        for (var i = 0; i < skuId.length; i++) {
            newSKU += ((skuId[i] == undefined ? '_' + 0 : '_' + skuId[i]));
        }
     
        getStock(newSKU);
        if (sku.length === 0) { sku = "0_0_0_"; }
    }
}

function getStock(skuId) {
    $.ajax({
        type: 'get',
        url: '../GetStock',
        data: { skuId: skuId },
        dataType: 'json',
        cache: false,// 开启ajax缓存
        async: false,
        success: function (data) {
            $("#stockProduct").text(data.Stock);
            var stock = parseInt(data.Stock);
            var Status = parseInt(data.Status);
            var isshoweasybt = false;
            if (stock > 0 && Status == 1) {
                isshoweasybt = true;
                $("#stockProductImage").text("In stock");
                $('#choose-btn-buy').show();
                $("#choose-btn-append").removeClass("disabled");
            } else {
                $("#stockProduct").html("0");
                $("#stockProductImage").html("Out of stock");
                $("#stockProductImage").html('<div class="dd"><strong style="color:red;">Tips：Out of stock</strong></div>');
                $("#choose-btn-append").addClass("disabled");
                $('#choose-btn-buy').hide();
            }
            showEasyBuyBt(isshoweasybt);

        },
        error: function (e) {
            //alert(e);
        }
    });

}

//处理轻松购按钮的显示
function showEasyBuyBt(isshow, isComple) {
    var isshowbt = false;
    var iscomple = isComple || false;
    isshowbt = !!isshow;
    if (isshowbt) {
        var easybuybt = $("#choose-btn-easybuy");
        easybuybt.hide();   //不启用轻松购 by DZY[150713]
        if (false) {
            var curisshow = easybuybt.is(":visible");
            if (iscomple) curisshow = true;
            if (isshowbt && curisshow) {
                easybuybt.show();
            } else {
                easybuybt.hide();
            }
        }
    }
}


function show(obj, id) {
    var y = $(obj).offset().top;
    var x = $(obj).offset().left + 170;
    var objDiv = $("#" + id + "");
    $(objDiv).css("display", "block");
    $(objDiv).css("left", x);
    $(objDiv).css("top", y);
}
function hide(obj, id) {
    var objDiv = $("#" + id + "");
    $(objDiv).css("display", "none");
}


function loadShopInfo() {
	$.ajax({
		type: 'get',
		url: '../GetShopInfo',
		data: { sid: $("#shopid").val(), productId: $("#gid").val() },
		dataType: 'json',
		cache: true,// 开启ajax缓存
		success: function (data) {
			if (data && data.IsSelf == true) {
				var brandLogo = data.BrandLogo;
				var html = "";
				if (brandLogo == "") {
					html += '<dl id="seller"><dt style="color:#fff;background-color:#ff4d50; padding:2px 10px;font-size:14px;">直</dt><span style="font-size:16px;margin-left:80px;">平台直营</span></dl>';
				}
				else {
					html += '<dl id="seller"><a href="/Search/?b_id=' + data.BrandId + '"> <img  height="50" src="' + data.BrandLogo + '"  /> </a></dl><dl style="padding:15px 0 15px 10px"><dt style="color:#fff;background-color:#ff4d50; padding:2px 10px;font-size:14px;">直</dt><span style="font-size:16px;margin-left:80px;">平台直营</span></dl>';
				}
				html += '<dl id="hotline">' + $("#online-service").html() + '</dl>';
				if (data.IsSevenDayNoReasonReturn || data.IsCustomerSecurity || data.TimelyDelivery) {
				    html += '<h3>Serive Support：</h3>';
				    if (data.IsSevenDayNoReasonReturn) {
				        html += '<dl class="pop-ensure"><dt><img src="/Images/SevenDay.jpg">Refund or return without reason in 7 days</dt></dl>';
				    }
				    if (data.IsCustomerSecurity) {
				        html += '<dl class="pop-ensure"><dt><img src="/Images/Security.jpg">Consumer protection services</dt></dl>';
				    }
				    if (data.TimelyDelivery) {
				        html += '<dl class="pop-ensure"><dt><img src="/Images/TimelyDelivery.jpg">Delivery in time</dt></dl>';
				    }
				}
				$("#brand-bar-pop").show().append($(html));
				
			}else{
				var shopinfo = '<dd><a target="_blank" style="color:#222;" href="/Shop/Home/' + data.Id.toString() + '">' + data.Name + '</a></dd>';
				if (showShop) {
					shopinfo = '<dd>' + data.Name + '</dd>';
				}
				var html = '<dl id="seller">' +
					shopinfo +
					'</dl>';
					if (data.CashDeposits > 0) {
					    html += '<dl class="pop-money"><dt>Level:</dt><dd><span title="The seller has paid Deposit' + data.CashDeposits + 'usd">' + data.CashDeposits + 'usd</span></dd></dl>'
					}
					html +='<div id="evaluate-detail">' +
					'<div class="mc">' +
					'<dl >' +
					'<dt>Description Match:</dt>' +
					'<dd title="（Merchant score-Industry average score）/（Merchant industry the highest score - Industry average score）">' +
					'<span class="' + productAndDescriptionColor + '">' + productAndDescription + '</span>' +
					'<i class="' + productAndDescriptionImage + '"></i>' +
					'<em class="' + productAndDescriptionColor + '">' + productAndDescriptionContrast + '</em>' +
					'</dd>' +
					'</dl>' +
					'<dl>' +
					'<dt>Delivery Speed:</dt>' +
					'<dd title="（Industry average score-Merchant score）/（Industry average score-Merchant industry the lowest score）">' +
					'<span class="' + sellerDeliverySpeedColor + '">' + sellerDeliverySpeed + '</span>' +
					'<i class="' + sellerDeliverySpeedImage + '"></i>' +
					'<em class="' + sellerDeliverySpeedColor + '">' + sellerDeliverySpeedContrast + '</em>' +
					'</dd>' +
					'</dl>' +
					'<dl>' +
					'<dt>Service attitude:</dt>' +
					'<dd title="（Industry average score-Merchant score）/（Industry average score-Merchant industry the lowest score）">' +
					 '<span class="' + sellerServiceAttitudeColor + '">' + sellerServiceAttitude + '</span>' +
					'<i class="' + sellerServiceAttitudeImage + '"></i>' +
					 '<em class="' + sellerServiceAttitudeColor + '">' + sellerServiceAttitudeContrast + '</em>' +
					'</dd>' +
					'</dl>' +
					'</div>' +
					'</div>' +
					'<dl id="online-service"><div class="line"></div>' +
					'</dl>' +
					'<dl id="pop-company">' + $("#online-service").html() + '</dl>';
					if (data.IsSevenDayNoReasonReturn || data.IsCustomerSecurity || data.TimelyDelivery) {
					    html += '<h3>Service:</h3>';
					    if (data.IsSevenDayNoReasonReturn) {
					        html += '<dl class="pop-ensure"><dt><a href="/Article/Category"><img src="/Images/SevenDay.jpg">Refund or return without reason in 7 days</a></dt></dl>';
					    }
					    if (data.IsCustomerSecurity) {
					        html += '<dl class="pop-ensure"><dt><a href="/Article/Category"><img src="/Images/Security.jpg">Consumer protection services</a></dt></dl>';
					    }
					    if (data.TimelyDelivery) {
					        html += '<dl class="pop-ensure"><dt><a href="/Article/Category"><img src="/Images/TimelyDelivery.jpg">Delivery in time</a></dt></dl>';
					    }
					}
					html +='<div id="enter-shop">' +
					'<a target="_blank" href="/Shop/Home/' + data.Id + '">Go to shop</a>' +
					'<a href="javascript:addFavorite(' + data.Id + ')">Add Favorite</a>' +
					'</div>';
				$("#brand-bar-pop").show().append($(html));
				if ($('#online-service').html() == '<div class="line"></div>') { $('#online-service').hide() }
				if (showShop) { $("#enter-shop").hide(); }
			}

		},
		error: function (e) {
			//alert(e);
		}
	});
	return;

}

function loadShopCate() {

    $.ajax({
        type: 'get',
        url: '../GetShopCate',
        data: { gid: $("#gid").val() },
        dataType: 'json',
        cache: true,// 开启ajax缓存
        success: function (data) {
            if (data) {
                //console.log(data);
                var html = '';
                for (var i = 0; i < data.length; i++) {
                    var text = '<dl><dt><a href="/Shop/Search?cid=' + data[i].Id + '&sid=' + $('#shopid').val() + '&pageNo=1" target="_blank"><s></s>' + data[i].Name + '</a></dt>';
                    if (data[i].SubCategory.length > 0) {
                        for (var j = 0; j < data[i].SubCategory.length; j++) {
                            text += '<dd><a href="/Shop/Search?cid=' + data[i].SubCategory[j].Id + '&sid=' + $('#shopid').val() + '&pageNo=1" target="_blank">' + data[i].SubCategory[j].Name + '</a></dd>';
                        }
                    }
                    text += '</dl>';
                    html += text;
                }
                $("#shopCateDiv").empty().append($(html));
            }
        },
        error: function (e) {
            //alert(e);
        }
    });
}

function loadHotSaleProduct() {
    $.ajax({
        type: 'get',
        url: '../GetHotSaleProduct',
        data: { sid: $("#shopid").val() },
        dataType: 'json',
        cache: true,// 开启ajax缓存
        success: function (data) {
            if (data) {
                //console.log(data);
                var html = '';
                for (var i = 0; i < data.length; i++) {
                    var text = '<li class="fore1">' +
                    '<div class="p-img"><a href="/Product/Detail/' + data[i].Id.toString() + '" target="_blank"><img alt="' + data[i].Name + '" src="' + data[i].ImgPath + '/1_220.png" /></a></div>' +
                            '<div class="p-name"><a href="/Product/Detail/' + data[i].Id.toString() + '" target="_blank" title="">' + data[i].Name + '</a></div>' +
                            '<div class="p-info p-bfc">' +
                                '<div class="p-count fl"><s>' + (i + 1).toString() + '</s><b>Top Seller' + data[i].SaleCount.toString() + '</b></div>' +
                                '<div class="p-price fr"><strong>￥+' + data[i].Price.toString + '</strong></div>' +
                            '</div>' +
                        '</li>';
                    html += text;
                }
                $("#hotsaleDiv").empty().append($(html));
            }
        },
        error: function (e) {
            //alert(e);
        }
    });
}


function LogProduct() {
    $.ajax({
        type: 'post',
        url: '../LogProduct',
        data: { pid: $("#gid").val() },
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

function loadHotConcernedProduct() {
    $.ajax({
        type: 'get',
        url: '../GetHotConcernedProduct',
        data: { sid: $("#shopid").val() },
        dataType: 'json',
        cache: true,// 开启ajax缓存
        success: function (data) {
            if (data) {
                //console.log(data);
                var html = '';
                for (var i = 0; i < data.length; i++) {
                    var text = '<li class="fore1">' +
                    '<div class="p-img"><a href="/Product/Detail/' + data[i].Id.toString() + '" target="_blank"><img alt="' + data[i].Name + '" src="' + data[i].ImgPath + '/1_220.png" /></a></div>' +
                            '<div class="p-name"><a href="/Product/Detail/' + data[i].Id.toString() + '" target="_blank" title="">' + data[i].Name + '</a></div>' +
                            '<div class="p-info p-bfc">' +
                                '<div class="p-count fl"><s>' + (i + 1).toString() + '</s><b>Liked ' + data[i].SaleCount.toString() + '</b></div>' +
                                '<div class="p-price fr"><strong>$+' + data[i].Price.toString + '</strong></div>' +
                            '</div>' +
                        '</li>';
                    html += text;
                }
                $("#hotConcerned").empty().append($(html));
            }
        },
        error: function (e) {
            //alert(e);
        }
    });
}


function loadGetProductDesc() {
    $.ajax({
        type: 'get',
        url: '../GetProductDesc',
        data: { pid: $("#gid").val() },
        dataType: 'json',
        cache: true,// 开启ajax缓存
        success: function (data) {
            if (data) {
                //console.log(data);
                $("#product-html").append($(data.DescriptionPrefix));
                $("#product-html").append($(data.ProductDescription));
                $("#product-html").append($(data.DescriptiondSuffix));
            }
        },
        error: function (e) {
            //alert(e);
        }
    });
}

function loadGetEnableBuyInfo() {
    if ($('#IsExpiredShop').val()) {
        $('#summary-price').html('<div class="dd"><strong style="font-size:25px;color:red;">The Shop expired！</strong></div>');
        $('#choose-btn-buy').hide();
        $("#choose-btn-append").addClass("disabled");
    } else {
        $.ajax({
            type: 'get',
            url: '../GetEnableBuyInfo',
            data: { gid: $("#gid").val() },
            dataType: 'json',
            cache: true,// 开启ajax缓存
            async: false,//同步
            success: function (data) {
                if (data) {
                    //console.log(data);
                    if (!data.hasSKU) {
                        $("#stockProductImage").html("Out of stock");
                        $("#stockProduct").html("0");
                    }
                    else if (data.IsOnSale === false) {
                        $('#summary-price').html('<div class="dd"><strong style="font-size:25px;color:red;">This product sold out！</strong></div>');
                        $("#stockProductImage").html('<div class="dd"><strong style="color:red;">Out of stock</strong></div>');
                    }
                    else {
                        $("#choose-btn-append").removeClass("disabled");
                        $('#choose-btn-buy').show();
                    }
                    if (data.IsOnSale && data.Logined == 1 && data.hasQuick === 1 && data.hasSKU) {
                        showEasyBuyBt(true, true);
                    }

                }

            },
            error: function (e) {
                //alert(e);
            }
        });
    }


}



function loadGetCommentsNumber() {
    $.ajax({
        type: 'get',
        url: '../GetCommentsNumber',
        data: { gid: $("#gid").val() },
        dataType: 'json',
        cache: true,// 开启ajax缓存
        success: function (data) {
            if (data) {
                $("#Comments").html(data.Comments.toString());
                $("#CommentsU").html("(" + data.Comments.toString() + ")");
                $("#ConsU").html("(" + data.Consultations.toString() + ")");
            }
        },
        error: function (e) {
            //alert(e);
        }
    });
}
function loadProductAttr() {
    $.ajax({
        type: 'get',
        url: '../GetProductAttr',
        data: { pid: $("#gid").val() },
        dataType: 'json',
        cache: true,// 开启ajax缓存
        success: function (data) {
            if (data) {
                //console.log(data);
                var html = '';
                for (var i = 0; i < data.length; i++) {
                    var values = "";
                    for (var j = 0; j < data[i].AttrValues.length; j++) {
                        values += data[i].AttrValues[j].Name + ",";
                    }
                    values = values.substr(0, values.length - 1)
                    var text = '<li>' + data[i].Name + '：' + values + '</li>';

                    html += text;
                }
                //console.log(data);
                $("#detail-list").append($(html));
            }
        },
        error: function (e) {
            //alert(e);
        }
    });
}

$(function () {
	$('#sp-hot-sale .mt span').hover(function () {
        $(this).addClass('cur').siblings().removeClass();
        $(this).parent().siblings().children().eq($(this).index()).show().siblings().hide();
    });
	
    $("#OrderNow").click(bindToSettlement);
    $("#buy-num").blur(function () {
        checkBuyNum();
        //if(parseInt($('#buy-num').val())>parseInt($("#stockProduct").html()))
        //{
        //    $.dialog.errorTips('不能大于库存数量');
        //    $('#buy-num').val(parseInt($("#stockProduct").html()));
        //    return false;
        //}
        calcFreight();
    });

    //购买数量加减
    $('.wrap-input .btn-reduce').click(function () {
        if (parseInt($('#buy-num').val()) > 1) {
            $('#buy-num').val(parseInt($('#buy-num').val()) - 1);
        }
        calcFreight();
    });
    $('.wrap-input .btn-add').click(function () {

        $('#buy-num').val(parseInt($('#buy-num').val()) + 1);

        calcFreight();
        //alert(parseInt($('#buy-num').val())+1)
    });

    $("#easyBuyBtn").click(function () {
        if (parseInt($('#buy-num').val()) > parseInt($("#stockProduct").html())) {
            $.dialog.errorTips('Buy quantity can not bigger than stock');
            $('#buy-num').val(parseInt($("#stockProduct").html()));
            return false;
        }
        var has = $("#has").val();
        var dis = $(this).parent("#choose-btn-append").hasClass('disabled');
        if (has != 1 || dis) return;
        var len = $('#choose li .dd .selected').length;
        if (len === $('#choose').find(".choose-sku").length) {
            var sku = getskuid();
            if (checkBuyNum()) {
                var num = $("#buy-num").val();
                location.href = "/Order/EasyBuyToOrder?skuId=" + sku + "&count=" + num;
                //   alert('SKUId：'+sku+'，购买数量：'+num);
            }
        } else {
            $.dialog.errorTips('Please select specification');

        }
    });

    loadGetEnableBuyInfo();
    loadShopCate();
    loadShopInfo();
    loadHotSaleProduct();
    loadHotConcernedProduct();
    loadProductAttr();
    loadGetProductDesc();
    loadGetCommentsNumber();
    LogProduct();

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
        Search();
    });

    $('#sp-keyword').keydown(function (e) {
        if (e.keyCode == 13) {
            Search();
        }
    });

    $('#sp-price').keydown(function (e) {
        if (e.keyCode == 13) {
            Search();
        }
    });

    $('#sp-price1').keydown(function (e) {
        if (e.keyCode == 13) {
            Search();
        }
    });


    //关注商品
    $("#choose-btn-coll .btn-coll").click(function () {
        checkLogin(function (func) {
            addFavoriteFun(func);
        });
    });


    function Search() {
        var start = isNaN(parseFloat($("#sp-price").val())) ? 0 : parseFloat($("#sp-price").val());
        var end = isNaN(parseFloat($("#sp-price1").val())) ? 0 : parseFloat($("#sp-price1").val());
        var shopid = $("#shopid").val();

        var keyword = $("#sp-keyword").val();
        if (keyword.length === 0 && start == end) {
            $.dialog.errorTips('please enter keywords or price ranges');
            return;
        }
        location.href = "/Shop/Search?pageNo=1&sid=" + shopid + "&keywords=" + keyword + "&startPrice=" + start + "&endPrice=" + end;
    }

    function addFavoriteFun(callBack) {
        $.post('/Product/AddFavoriteProduct', { pid: $("#gid").val() }, function (result) {
            if (result.successful == true) {
                if (result.favorited == true) {
                    $.dialog.tips(result.mess);
                } else {
                    $.dialog.succeedTips(result.mess);
                }
                (function () { callBack && callBack(); })();
            }

        });
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



    //加入购物车
    $("#InitCartUrl").click(function (e) {
        if (parseInt($('#buy-num').val()) > parseInt($("#stockProduct").html())) {
            $.dialog.errorTips('Buy quantity can not bigger than stock');
            $('#buy-num').val(parseInt($("#stockProduct").html()));
            return false;
        }
        var has = $("#has").val();
        var dis = $(this).parent("#choose-btn-append").hasClass('disabled');
        if (has != 1 || dis) return;
        var len = $('#choose li .dd .selected').length;
        if (len === $('#choose').find(".choose-sku").length) {
            var sku = getskuid();
            if (checkBuyNum()) {
                var num = $("#buy-num").val();
				var loading = showLoading();
				$.ajax({
					type: 'POST',
					url: "/cart/AddProductToCart?skuId=" + sku + "&count=" + num,
					dataType: 'json',
					success: function (data) {
						loading.close();
						if (data.success == true) {
							var cartOffset = $("#right_cart").offset(),
								h=$(document).scrollTop();
								flySrc = $('#spec-list li').first().find('img')[0].src,
								flyer = $('<img class="cart-flyer" src="'+flySrc+'"/>');
							flyer.fly({
								start: {
									left: e.pageX,
									top: e.pageY-h-30
								},
								end: {
									left: cartOffset.left,
									top: cartOffset.top-h+30,
									width: 20,
									height: 20
								},
								onEnd: function(){
									this.destory(); //移除dom 
									refreshCartProducts();
								} 
							});
							
							
							//$.dialog.succeedTips('添加成功')
						} else {
							loading.close();
							$.dialog.errorTips(data.msg);
						}
					},
					error: function (e) {
						//loading.close();
						$.dialog.errorTips('Add to cart failed');
					}
				});
            }
        } else {
            $.dialog.errorTips("Please select specification！");
        }
    });
	
	/*$(".goodss").click(function() {
          
		var shopOffset = $("#shop").offset();
		var cloneDiv = $(this).parents("td").find("div").clone();
		var proOffset = $(this).parents("td").find("div").offset();
		cloneDiv.css({ "position": "absolute", "top": proOffset.top, "left": proOffset.left });
		$(this).parents("td").find("div").parent().append(cloneDiv);

		cloneDiv.animate({
			left: shopOffset.left,
			top: shopOffset.top
		},"slow");
	});*/

    //加入购物车
    $("#justBuy").click(function () {
        if (parseInt($('#buy-num').val()) > parseInt($("#stockProduct").html())) {
            $.dialog.errorTips('Buy quantity can not bigger than stock');
            $('#buy-num').val(parseInt($("#stockProduct").html()));
            return false;
        }
        var has = $("#has").val();
        var dis = $(this).hasClass('disabled');
        if (has != 1 || dis) return;
        var len = $('#choose li .dd .selected').length;
        if (len === $('#choose').find(".choose-sku").length) {
            var sku = getskuid();
            if (checkBuyNum()) {
                var num = $("#buy-num").val();
                location.href = "/Order/SubmitByProductId?skuIds=" + sku + "&counts=" + num;
                //alert('SKUId：'+sku+'，购买数量：'+num);
            }
        } else {
            $.dialog.errorTips("Please select specification！");
        }
    });

    //自营店产品详情跟普通店铺不一样显示
    changeShowView();

    if ($('#choose').find(".choose-sku").length == 0) {
        var skuIdZero = $("#gid").val() + "_0_0_0";
        getStock(skuIdZero);
    }
	
	
	if($('.p-group-child li').length>3){
		$('.group-arrow').show();
	}
	
	var groupPage = 1;
	var groupI = 3; //每版放3个图片 
	var groupContent = $(".p-group-child-box");
	var groupContentList = $(".p-group-child");
	var groupHeight = groupContent.height();
	var groupLen = groupContent.find("li").length;
	var groupPageCount = Math.ceil(groupLen / groupI); 
	//向后 按钮  
	$(".group-arrow-next").click(function () {
		if (!groupContentList.is(":animated")) {
			if (groupPage == groupPageCount) {
				groupContentList.animate({ top: '0px' }, 300);
				groupPage = 1;
			} else {
				groupContentList.animate({ top: '-=' + groupHeight }, 300);
				groupPage++;
			}
		}
	});
	//往前 按钮  
	$(".group-arrow-pre").click(function () {
		if (!groupContentList.is(":animated")) {
			if (groupPage == 1) {
				groupContentList.animate({ top: '-=' + groupHeight * (groupPageCount-1) }, 300);
				groupPage = groupPageCount;
			} else {
				groupContentList.animate({ top: '+=' + groupHeight }, 300);
				groupPage--;
			}
		}
		
	});
});

function changeShowView() {
    if (isSellerAdminProdcut == "true") {
        $("#sp-search,#sp-category").show();
        $("#shopname").attr("href", "#")
        bindOfficialView();
    }
    else {
        $("#sp-brand-official,#sp-category-official").hide();
    }
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
        $.dialog.errorTips('Buy quantity error');
        $('#buy-num').val(1);
        result = false;
    }
    return result;
}

function bindOfficialView() {
    loadShopCateOfficial();
    loadBrandOfficial();
}

function loadBrandOfficial() {

    $.ajax({
        type: 'get',
        url: '../GetBrandOfficial',
        data: { gid: $("#gid").val() },
        dataType: 'json',
        cache: true,// 开启ajax缓存
        success: function (data) {
            if (data) {
                $("#brandDivOfficial").empty();
                //console.log(data);
                var html = '';
                var dataLength = data.length;
                for (var i = 0; i < data.length; i++) {
                    html += '<dd style="width:33%;float:left;text-align:center;"><a href="/Search/?b_id=' + data[i].Id + '" target="_blank"><s></s>' + data[i].Name + '</a></dd>';
                }

                $("#brandDivOfficial").append($(html));
            }
        },
        error: function (e) {
            //alert(e);
        }
    });
}

function loadShopCateOfficial() {

    $.ajax({
        type: 'get',
        url: '../GetShopCateOfficial',
        data: { gid: $("#gid").val() },
        dataType: 'json',
        cache: true,// 开启ajax缓存
        success: function (data) {
            if (data) {
                $("#shopCateDivOfficial").empty();
                //console.log(data);
                var html = '';
                var dataLength = data.length;
                for (var i = 0; i < data.length; i++) {
                    html += '<dd style="width:50%;float:left;text-align:center;"><a href="/Search/?cid=' + data[i].Id + '" target="_blank"><s></s>' + data[i].Name + '</a></dd>';
                }

                $("#shopCateDivOfficial").append($(html));
            }
        },
        error: function (e) {
            //alert(e);
        }
    });
}

function bindToSettlement() {
    if (parseInt($('#buy-num').val()) > parseInt($("#stockProduct").html())) {
        $.dialog.errorTips('buy quantity can not bigger than stock');
        $('#buy-num').val(parseInt($("#stockProduct").html()));
        return false;
    }
    var has = $("#has").val();
    var dis = $(this).parent("#choose-btn-append").hasClass('disabled');
    if (has != 1 || dis) return;
    var len = $('#choose li .dd .selected').length;
    if (len === $('#choose').find(".choose-sku").length) {

        var memberId = $.cookie('Himall-User');

        if (memberId) {

            var sku = getskuid();
            if (checkBuyNum()) {
                var num = $("#buy-num").val();


                var url = "/Order/SubmitByProductId?skuIds=" + sku + "&counts=" + num;
                //var loading = showLoading();
                //ajaxRequest({
                //    type: 'POST',
                //    url: "/cart/AddToCartForOrderNow?skuId=" + sku + "&count=" + num,
                //    dataType: "json",
                //    success: function (data) {
                //        loading.close();
                //        if (data.success == true) {

                //            location.href = '/order/submit?' + 'cartItemIds=' + data.value;
                //        } else {
                //            $.dialog.errorTips(data.msg);
                //        }
                //    }, error: function () {
                //        loading.close();
                //    }
                //});

                location.href = url;


                //location.href = "/cart/addToCart?skuId=" + sku + "&count=" + num;

                //location.href = '/order/submit?' + 'cartItemIds=' + str;
            }
        }
        else {

            $.fn.login({}, function () {
                location.href = window.location.href;
            }, '', '', '/Login/Login');
        }


        //   alert('SKUId：'+sku+'，购买数量：'+num);
    } else {
        $.dialog.errorTips("Please select specification！");
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


function addFavorite(shopId) {
    checkLogin(function (func) {
        addFavoriteFun(shopId, func);
    });
}

function addFavoriteFun(shopId, callBack) {

    var loading = showLoading();
    $.post('/Product/AddFavorite', { shopId: shopId }, function (result) {
        loading.close();
        if (result.success)
            $.dialog.succeedTips('Add to Wishlist Success', function () { callBack && callBack(); });
        else
            $.dialog.succeedTips(result.msg, function () { callBack && callBack(); });

    });
}

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


//组合购
$(function(){
	var minCollTotal,
		maxCollTotal,
		minSaleTotal,
		maxSaleTotal;
	if($('.p-group-list').length>0)
		GroupPriceChange();
	
	$('.p-group-child input').change(function() {
		GroupPriceChange();
	});

	
});

function GroupPriceChange(){
	var mCheck=$('.p-group-main input:checked')
	minCollTotal=mCheck.data('mincollprice');
	maxCollTotal=mCheck.data('maxcollprice');
	minSaleTotal=mCheck.data('minsaleprice');
	maxSaleTotal=mCheck.data('maxsaleprice');
		
	$('.p-group-child input:checked').each(function() {
		minCollTotal+=$(this).data('mincollprice');
		maxCollTotal+=$(this).data('maxcollprice');
		minSaleTotal+=$(this).data('minsaleprice');
		maxSaleTotal+=$(this).data('maxsaleprice');
	});
	if(minCollTotal!=maxCollTotal)
		$('#collTotalPrice').text('$'+minCollTotal.toFixed(2)+'-'+maxCollTotal.toFixed(2));
	else
		$('#collTotalPrice').text('$'+minCollTotal.toFixed(2));
	if(minSaleTotal!=maxSaleTotal)
		$('#saleTotalPrice').text(minSaleTotal.toFixed(2)+'-'+maxSaleTotal.toFixed(2));
	else
		$('#saleTotalPrice').text(minSaleTotal.toFixed(2));
	if(minSaleTotal-minCollTotal!=maxSaleTotal-maxCollTotal)
		$('#groupPriceMinus').text('$'+(minSaleTotal-minCollTotal).toFixed(2)+'-'+(maxSaleTotal-maxCollTotal).toFixed(2))
	else
		$('#groupPriceMinus').text('$'+(minSaleTotal-minCollTotal).toFixed(0))
}



function CollocationBuy() {
	var chk = $(".collochk:checked");
	var pids = "";
	var colloPids="";
	if (chk.length < 2) {
		$.dialog.errorTips("Please select！");
		return;
	}
	else {
		chk.each(function (index, item) {
			if (index < chk.length - 1) {
				pids += $(item).val() + ",";
				colloPids += $(item).data("collpid") + ",";
			}
			else {
				pids += $(item).val();
				colloPids += $(item).data("collpid");
			}
		});
	}
	$.ajax({
		url: "/Product/GetCollocationProducts",
		data: {productIds: pids,colloPids:colloPids},
		async: false,
		success: function (data) {
			$("#addCollocation").html(data);
		}
	});
	$.dialog({
	    title: 'Collocation Buy',
		width: 908,
		padding:0,
		id: 'Collocation',
		content: document.getElementById("addCollocation"),
		lock: true,
		okVal: 'Determine to buy packages',
		init: function () {
			var groupPrice=0;
			for(var i=0;i<$('.group-item').length;i++){
				groupPrice+=parseFloat($('.product-price').eq(i).text());
			}
			$('.group-price span').data('groupprice',groupPrice).text(groupPrice.toFixed(2));
			
			var len=$('.group-item').length;
			for(var i=0;i<len;i++){
				setSKUGroup($('.product-item').eq(i).data('productid'),$('.SKUgroup'+i),$('.product-item').eq(i).data('colloproductid'));
			}
			
			
			
			$('#groupCounts').keyup(function() {
				var groupPrice=$('.group-price span').data('groupprice');
				this.value=this.value.replace(/[^0-9]+/,'');
				if (this.value == '' || this.value<1) {
					this.value='1';
				}
				$('.group-price span').text((this.value*groupPrice).toFixed(2));
			});
			
		},
		ok: function () {
			//创建订单页面
			var flag=1,
				groupSkuids='',
				collpids='',
				groupcounts='';
			$('.group-item').each(function() {
				$(this).removeClass('error');
				if($(this).find('.selected').length!=$(this).find(".choose-sku").length){
					$(this).addClass('error');
					flag=2;
				}
				if(parseInt($(this).find('.group-stock').text())<parseInt($('#groupCounts').val())){
					$(this).addClass('error');
					flag=3;
				}
				groupSkuids+=$(this).find('.group-skuId').val()+',';
				collpids+=$(this).find('.product-item').data('colloproductid')+',';
				groupcounts+=$('#groupCounts').val()+','
			});
			if(flag==2){
				$.dialog.errorTips("please select specification");
				return false;
			}
			if(flag==3){
				$.dialog.errorTips("Buy quantity>stock");
				return false;
			}
			
			groupSkuids=groupSkuids.substring(0, groupSkuids.length - 1);
			collpids=collpids.substring(0, collpids.length - 1);
			groupcounts=groupcounts.substring(0, groupcounts.length - 1);
			$('#skuids').val(groupSkuids);
			$('#counts').val(groupcounts);
			$('#collpids').val(collpids);
			checkLogin(function () {
                $('#CollProducts').submit();
			})
			
		}
	});
}