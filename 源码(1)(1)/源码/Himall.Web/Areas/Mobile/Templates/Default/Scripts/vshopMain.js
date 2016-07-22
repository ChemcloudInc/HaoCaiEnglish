$(function () {
    loadVshops(page++);
    initAddFavorite();
});


var page = 1;

$(window).scroll(function () {
    $('#autoLoad').removeClass('hide');
    var scrollTop = $(this).scrollTop();
    var scrollHeight = $(document).height();
    var windowHeight = $(this).height();

    if (scrollTop + windowHeight >= scrollHeight) {
        //alert('执行加载');
        loadVshops(page++);
    }
});


function loadVshops(page) {
    var areaname = areaName;

    var url = '/' + areaname + '/vshop/GetHotShops';
    $.post(url, { page: page, pageSize: 8 }, function (result) {
        var html = '';
        if (result.length > 0) {
            $.each(result, function (i, shop) {
                html += ' <div class="vshop-item">\
                <div class="vshop-head">\
                    <a class="v-logo" href="/'+ areaname + '/vshop/detail/' + shop.id + '"><img src="' + shop.logo + '" /><p>' + shop.name + '</p></a>\
                    <a class="btn btn-primary btn-sm" href="javascript:;" type="addFavorite" shopId="' + shop.shopId + '"> ' + (shop.favorite ? '已收藏' : '收 藏') + '</a>\
                </div>\
                <ul class="v-goods clearfix">';
                $.each(shop.products, function (j, product) {
                    html += '<li>\
                        <a class="p-img" href="/' + areaname + '/product/detail/' + product.id + '"><img src="' + product.image + '" alt=""></a>\
                        <h3><a href="/'+ areaname + '/product/detail/' + product.id + '">' + product.name + '</a></h3>\
                        <p><span>￥' + product.salePrice + '</span></p>\
                    </li>';
                });
                html += '</ul></div>';
            });
            $('#autoLoad').addClass('hide');
            $('#shopList').append(html);
            //autoSizeImage();
        }
        else {
            $('#autoLoad').html('');
            $('#more').html('<a class="btn btn-primary btn-sm" href="/' + areaname + '/VShop/list"> 查看更多微店 </a>');
        }
    });
}



$('body').on('click', 'a[type="addFavorite"]', function () {
    var shopId = $(this).attr('shopId');
    var isAdd = $(this).html().indexOf('已') > -1 ? false : true;
    var returnUrl = '/' + areaName + '/vshop?addFavorite=' + shopId + '&isAdd=' + isAdd;
    checkLogin(returnUrl, function () {
        addFavorite(shopId, isAdd);
    });
});

function initAddFavorite() {
    var shopId = QueryString('addFavorite');
    var isAdd = QueryString('isAdd');
    if (shopId) {//带有addFavorite参数，说明为登录后回调此页面添加收藏
        addFavorite(shopId, isAdd);
    }

}



function addFavorite(shopId, isAdd) {
    var loading = showLoading();
    var method = '';
    var title;
    if (isAdd) {
        method = 'AddFavorite';
        title = '';
    }
    else {
        method = 'DeleteFavorite';
        title = '取消';
    }
    $.post('/' + areaName + '/vshop/' + method, { shopId: shopId }, function (result) {
        loading.close();
        if (result.success) {
            $.dialog.succeedTips(title + '收藏成功!');
            $('a[type="addFavorite"][shopId="' + shopId + '"]').html(isAdd ? '已收藏' : '收 藏');
        }
        else
            $.dialog.errorTips(result.msg);
    });
}