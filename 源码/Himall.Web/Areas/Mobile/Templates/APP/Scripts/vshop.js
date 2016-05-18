var page = 1;

$(window).scroll(function () {
    $('#autoLoad').removeClass('hide');
    var scrollTop = $(this).scrollTop();
    var scrollHeight = $(document).height();
    var windowHeight = $(this).height();

    if (scrollTop + windowHeight >= scrollHeight) {
        //alert('执行加载');
        loadVShops(page++);
    }
});

$(function () {

    loadVShops(page++);
    initAddFavorite();

});




function loadVShops(page) {
    var areaname = areaName;

    var url = '/' + areaname + '/vshop/list';
    $.post(url, { page: page, pageSize: 8 }, function (result) {
        var html = '';
        if (result.length > 0) {
            $.each(result, function (i, vshop) {
                var tags = vshop.tags ? vshop.tags.split(',') : '';
                var tag1 = tags.length > 0 ? tags[0] : '';
                var tag2 = tags.length > 1 ? tags[1] : '';
                var url = '/' + areaname + '/vshop/detail/' + vshop.id;

                html += '  <div class="vshop-item">\
                <div class="vshop-banner">\
                    <a href="' + url + '"><img src="' + vshop.image + '" /></a>\
                    <a class="btn btn-primary btn-sm"  href="javascript:;" type="addFavorite" shopId="' + vshop.shopId + '">' + (vshop.favorite ? '已收藏' : '收 藏') + '</a>\
                    <div class="vshop-info">\
                        <h3><a href="' + url + '">' + vshop.name + '</a></h3>\
                                 <p><em>' + tag1 + '</em><em>' + tag2 + '</em></p>\
                    </div>\
                </div>\
            </div>';

            });
            $('#autoLoad').addClass('hide');
            $('#shopList').append(html);
        }
        else {
            $('#autoLoad').html('没有更多店铺了');
        }
    });
}


$('#shopList').on('click', 'a[type="addFavorite"]', function () {
    var shopId = $(this).attr('shopId');
    var isAdd = $(this).html().indexOf('已') > -1 ? false : true;
    var returnUrl = '/' + areaName + '/vshop/list?addFavorite=' + shopId + '&isAdd=' + isAdd;
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

function addFavorite(shopId,isAdd) {
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