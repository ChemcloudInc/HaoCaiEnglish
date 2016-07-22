//testData = {
//    "comments":
//    [
//      {
//          "UserName": "用户名",
//          "ReviewContent": "评价内容",
//          "ReviewDate": "2014-09-09",
//          "ReplyContent": "答复内容",
//          "ReplyDate": "2014-10-10",
//          "BuyDate": "2014-10-10",
//          "ReviewMark": 4  /*评价星级*/
//      },
//      {
//          "UserName": "11111111111",
//          "ReviewContent": "评价内容",
//          "ReviewDate": "2014-09-09",
//          "ReplyContent": "答复内容",
//          "ReplyDate": "2014-10-10",
//          "BuyDate": "2014-10-10",
//          "ReviewMark": 4  /*评价星级*/
//      }
//    ],
//    "totalPage": 5,       /*总页数*/
//    "currentPage": 5,      /*当前页码*/
//    "goodComment": 125,    /*好评条数*/
//    "commentType": 0,      /* 0 1 2 3 */
//    "badComment": 7,       /*差评条数*/
//    "comment": 123        /*中评条数*/
//}

//$(function () {
//    //-------------- 刷新页面首次渲染数据
//    var idList = [],// id存放列表
//        uuid = 1,// 用来统计请求次数
//        pid = $('#gid').val(),// 商品id
//        getAjax = function (url, pid, fn) {
//            $.ajax({
//                type: 'get',
//                url: url + '?pId=' + pid,
//                dataType: 'html',
//                cache: true,// 开启ajax缓存
//                success: function (data) {
//                    if (data) {
//                        var data = (new Function('return ' + data))();
//                        fn(template, data, '#comments-list', commenttype, 'block');
//                    }
//                },
//                error: function (e) {
//                    //
//                }
//            });
//        },
//        tempData = [],// 暂存数据 随后会立即清空
//        render = function (template, data, dom, tag, state) {
//            var dom = $(dom),
//                str = '',
//                i,
//                e,
//                con = data.comments;
//            for (i = 0; e = con[i++];) {
//                str += template[1]
//                        + template[2]
//                            + template[3]
//                                + template[4]
//                                    + '<span class="name">"'+e.UserName+'"</span>'
//                                    //+ '<img width="50" height="50" src="http://jss.jd.com/outLinkServicePoint/25471507-2e9a-4188-8db7-9af4651bbd60.jpg" title="' + e.UserName + '"/></a></div>'
//                                + template[5]
//                                    + '<a target="_blank">' + e.UserName + '</a></div></div>'
//                            + template[8]
//                                + template[9]
//                                    + '<span class="star sa' + e.ReviewMark + '"></span>'
//                                    + template[10] + e.ReplyDate + '</span></div>'
//                                + template[11]
//                                    + '<dl><dt>心得:</dt><dd>' + e.ReviewContent + '</dd></dl>'
//                                    + '<dl><dt>购买日期：</dt><dd>' + e.BuyDate + '</dd></dl>'
//                                    + '<dl class="shop-reply"><dt>商家回复：</dt><dd>' + e.ReplyContent + '<br /><span>' + e.ReplyDate + '</span></dd></dl></div></div>'
//                                    + '<div class="corner tl"></div></div>';
//            }
//            tempData += '<div id="comment-' + tag + '" class="mc" style="display: ' + state + ';">' + str;
//            }
//            tempData += '<div class="cl"><div style="padding:8px 0 0 120px;" class="fl"></div><div id="commentsPage' + tag + '" class="pagin fr">' + str + '</div></div></div>';
//            dom.append(tempData);
//            tempData = [];

//    getAjax('/Product/GetCommentByProduct', pid, render);//第一次数据渲染
//    //render(template, testData, '#comments-list', 0, 'block');//渲染函数

//});


$(function () {
    getComment();
});

function getComment() {
    var pid = $('#gid').val();
    var str = '';
    $.ajax({
        type: 'get',
        url: "/" + areaName + "/Product/GetCommentByProduct?pId=" + pid,
        dataType: 'json',
        cache: true,// 开启ajax缓存
        success: function (data) {
            if (data) {
                $.each(data.comments, function (index, items) {
                    str += '<li><div><span class="name">' + items.UserName + '</span>';
                    for (j = 0; j < items.ReviewMark; j++) {
                        str += '<i class="glyphicon glyphicon-star"></i>';
                    }
                    str += '<em>' + items.ReviewDate + '</em>'
                    str += '</div>'
                    str += '<p>' + items.ReviewContent + '</p></li>'

                })
                str += '<li class="more"><a href="/' + areaName + '/Product/ProductComment?pId='+pid+'">查看全部商品评价</a></li>'
            }
        },
        complete: function () {
            $("#commentReview").append(str);
        }
    });
}