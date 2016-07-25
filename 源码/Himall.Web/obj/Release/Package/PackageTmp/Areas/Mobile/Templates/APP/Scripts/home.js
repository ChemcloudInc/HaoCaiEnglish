
var page = 1;

$(window).scroll(function () {
    var scrollTop = $(this).scrollTop();
    var scrollHeight = $(document).height();
    var windowHeight = $(this).height();

    if (scrollTop + windowHeight >= scrollHeight - 30) {
        loadProducts(++page);
    }
});


function loadProducts(page) {
    var areaname = areaName;

    var url = getAreaPath() + '/home/LoadProducts';
    $.post(url, { page: page, pageSize: 8 }, function (result) {
        var html = '';
        if (result.length > 0) {
            $.each(result, function (i, product) {
                html += ' <li>\
                <a class="p-img" href="/' + areaname + '/product/detail/' + product.id + '"><img src="' + product.image + '" alt=""></a>\
                <i>' + (((product.price / product.marketPrice) * 10).toFixed("1")) + '折</i>\
                <h3><a>' + product.name + '</a></h3>\
                <p><span>￥' + product.price.toFixed("2") + '</span><s>￥' + product.marketPrice.toFixed("2") + '</s></p>\
            </li>';
            });
            $('#productList').append(html);
            square($('.goods-list .p-img'));
        }
        else {
            $('#autoLoad').html('没有更多商品了');
        }
    });
}