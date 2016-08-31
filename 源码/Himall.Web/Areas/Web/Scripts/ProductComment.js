testData = {
    "comments":
    [
      {
          "UserName": "用户名",
          "ReviewContent": "评价内容",
          "ReviewDate": "2014-09-09",
          "ReplyContent": "答复内容",
          "ReplyDate": "2014-10-10",
          "BuyDate": "2014-10-10",
          "ReviewMark": 4  /*评价星级*/
      },
      {
          "UserName": "11111111111",
          "ReviewContent": "评价内容",
          "ReviewDate": "2014-09-09",
          "ReplyContent": "答复内容",
          "ReplyDate": "2014-10-10",
          "BuyDate": "2014-10-10",
          "ReviewMark": 4  /*评价星级*/
      }
    ],
    "totalPage": 5,       /*总页数*/
    "currentPage": 5,      /*当前页码*/
    "goodComment": 125,    /*好评条数*/
    "commentType": 0,      /* 0 1 2 3 */
    "badComment": 7,       /*差评条数*/
    "comment": 123        /*中评条数*/
}

$(function () {
    //-------------- 刷新页面首次渲染数据
    var idList = [],// id存放列表
        uuid = 1,// 用来统计请求次数
        pid = $('#gid').val(),// 商品id
        getAjax = function (url, pid, pageno, commenttype, fn) {

            $.ajax({
                type: 'get',
                url: url + '?pId=' + pid + '&pageNo=' + pageno + '&pageSize=' + 10 + '&commentType=' + commenttype,
                dataType: 'html',
                cache: true,// 开启ajax缓存
                success: function (data) {
                    if (data) {
                        var data = (new Function('return ' + data))();
                        fn(template, data, '#comments-list', commenttype, 'block');
                    }
                },
                error: function (e) {
                    //
                }
            });
        },
        template = [
            '<div id="comment-0" class="mc">',
            '<div class="item">',
            '<div class="user">',
            '<div class="u-icon">',
            '<a target="_blank" title="查看TA的全部评价">',
            '<div class="u-name">',
            '<span class="u-level">',
            '<span class="u-address">',
            '<div class="i-item">',
            '<div class="o-topic">',
            '<span class="date-comment">',
            '<div class="comment-content">'
        ],// 模板数据
        tempData = [],// 暂存数据 随后会立即清空
        render = function (template, data, dom, tag, state) {
            var dom = $(dom),
                str = '',
                i,
                e,
                con = data.comments;
            for (i = 0; e = con[i++];) {

                str += template[1]
                        + template[2]
                            + template[3]
                                + template[4]
                                    + '<img width="50" height="50" src="/Areas/Web/images/avatar.png" title="' + e.UserName + '"/></a></div>'
                                + template[5]
                                     + e.UserName + '</div></div>'
                            + template[8]
                                + template[9]
                                    + '<span class="star sa' + e.ReviewMark + '"></span>'
                                    + template[10] + e.ReplyDate + '</span></div>'
                                + template[11]
                                    + '<dl><dt>心得：</dt><dd>' + e.ReviewContent + '</dd></dl>'
                                    + '<dl><dt>购买日期：</dt><dd>' + e.BuyDate + '</dd></dl>';
                            if (e.ReplyContent != "暂无回复") {
                                str += '<dl class="shop-reply"><dt>商家回复：</dt><dd><div class="content">' + e.ReplyContent + '</></div><div class="date-answer">' + e.ReplyDate + '</div></dd></dl>'
                                    }
                                    str+= '</div></div><div class="corner tl"></div></div>';
            }
            tempData += '<div id="comment-' + tag + '" class="mc" style="display: ' + state + ';">' + str;
            str = '';
            // 当前页码等于1
            if (data.currentPage == 1) {
                if (data.totalPage <= 5) {
                    for (i = 1; i < data.totalPage + 1; i++) {
                        if (i == 1) {
                            str += '<a class="current">' + i + '</a>';
                        } else {
                            str += '<a href="#comments-list" data="' + tag + ',' + pid + '">' + i + '</a>';
                        }
                    }
                } else {
                    for (i = 1; i < 6; i++) {
                        if (i == 1) {
                            str += '<a class="current">' + i + '</a>';
                        } else {
                            str += '<a href="#comments-list" data="' + tag + ',' + pid + '">' + i + '</a>';
                        }
                    }
                }
                if (data.totalPage > 5) {
                    str += '<span>...</span>';
                    if ((data.totalPage - 5) > 1) { str += '<a href="#comments-list" data="' + pid + '">' + (data.totalPage - 1) + '</a>'; }
                    str += '<a href="#comments-list" data="' + tag + ',' + pid + '">' + (data.totalPage) + '</a>';
                }
                if (data.totalPage > 1) {
                    str += '<a class="next" href="#comments-list" data="' + tag + ',' + pid + ',1,1,' + data.totalPage + '">Next</a>';
                }
            } else if (data.currentPage > 1 && data.totalPage < 6) {// 总页数不超过5个
                str += '<a class="prev" href="#comments-list" data="' + tag + ',' + pid + ',' + data.currentPage + ',0">Previous</a>';
                for (i = 1; i < data.totalPage + 1; i++) {
                    if (i == data.currentPage) {
                        str += '<a class="current">' + i + '</a>';
                    } else {
                        str += '<a href="#comments-list" data="' + tag + ',' + pid + '">' + i + '</a>';
                    }
                }
                str += '<a class="next" href="#comments-list" data="' + tag + ',' + pid + ',' + data.currentPage + ',1,' + data.totalPage + '">Next</a>';
            } else if (data.currentPage > 1 && data.totalPage > 5) {
                str += '<a class="prev" href="#comments-list" data="' + tag + ',' + pid + ',' + data.currentPage + ',0">Previous</a>';
                if (data.currentPage < 6) {
                    for (i = 1; i < 6; i++) {
                        if (i == data.currentPage) {
                            str += '<a class="current">' + i + '</a>';
                        } else {
                            str += '<a href="#comments-list" data="' + tag + ',' + pid + '">' + i + '</a>';
                        }
                    }
                    str += '<span>...</span>';

                    if ((data.totalPage - 5) > 1) { str += '<a href="#comments-list" data="' + tag + ',' + pid + '">' + (data.totalPage - 1) + '</a>'; }
                    str += '<a href="#comments-list" data="' + tag + ',' + pid + '">' + (data.totalPage) + '</a>';
                } else if ((data.currentPage - 5) == 1) {
                    for (i = 1; i < 7; i++) {
                        if (i == data.currentPage) {
                            str += '<a class="current">' + i + '</a>';
                        } else {
                            str += '<a href="#comments-list" data="' + tag + ',' + pid + '">' + i + '</a>';
                        }
                    }
                    str += '<a href="#comments-list" data="' + tag + ',' + pid + '">' + (data.currentPage + 1) + '</a>';
                    str += '<a href="#comments-list" data="' + tag + ',' + pid + '">' + (data.currentPage + 2) + '</a>';
                    str += '<span>...</span>';
                    str += '<a href="#comments-list" data="' + tag + ',' + pid + '">' + (data.totalPage - 1) + '</a>';
                    str += '<a href="#comments-list" data="' + tag + ',' + pid + '">' + (data.totalPage) + '</a>';
                } else if ((data.currentPage - 5) > 1) {
                    str += '<a href="#comments-list" data="' + tag + ',' + pid + '">1</a>';
                    str += '<a href="#comments-list" data="' + tag + ',' + pid + '">2</a>';
                    str += '<span>...</span>';
                    str += '<a href="#comments-list" data="' + tag + ',' + pid + '">' + (data.currentPage - 3) + '</a>';
                    str += '<a href="#comments-list" data="' + tag + ',' + pid + '">' + (data.currentPage - 2) + '</a>';
                    str += '<a href="#comments-list" data="' + tag + ',' + pid + '">' + (data.currentPage - 1) + '</a>';
                    str += '<a class="current" data="' + tag + ',' + pid + '">' + data.currentPage + '</a>';
                    str += '<a href="#comments-list" data="' + tag + ',' + pid + '">' + (data.currentPage + 1) + '</a>';
                    str += '<a href="#comments-list" data="' + tag + ',' + pid + '">' + (data.currentPage + 2) + '</a>';
                    str += '<span>...</span>';
                    str += '<a href="#comments-list" data="' + tag + ',' + pid + '">' + (data.totalPage - 1) + '</a>';
                    str += '<a href="#comments-list" data="' + tag + ',' + pid + '">' + (data.totalPage) + '</a>';

                }
                str += '<a class="next" href="#comments-list" data="' + tag + ',' + pid + ',' + data.currentPage + ',1,' + data.totalPage + '">Next</a>';
            }
            tempData += '<div class="cl"><div style="padding:8px 0 0 120px;" class="fl"></div><div id="commentsPage' + tag + '" class="pagin fr">' + str + '</div></div></div>';
            dom.append(tempData);
            tempData = [];
            // 只会执行一次 生成评论条数
            if (uuid === 1) {
                var total = data.goodComment + data.badComment + data.comment,
                    g = data.goodComment,
                    c = data.comment,
                    b = data.badComment,
                    e = Math.round((g / total).toFixed(2) * 100),// 取整 误差为1
                    f = Math.round((c / total).toFixed(2) * 100),
                    j = 100 - e - f,
                    arr = [total, g, c, b],
                    arr1 = [e, f, j];
                $('#id_comment_btn').find('li').each(function (i, e) {
                    $(e).find('a').append('<em>(' + arr[i] + ')</em>');
                });
                $('#i-comment strong').empty().prepend(e + '%');
                $('#praiseRate').empty().prepend(e + '%');
                $('#i-comment .percent').find('span').each(function (i, e) {
                    $(e).html('(' + arr1[i] + '%)');
                });
                $('#i-comment .percent').find('div').each(function (i, e) {
                    $(e).css({ width: arr1[i] + 'px' });
                });
            }
            uuid++;
        };

    getAjax('/Product/GetCommentByProduct', pid, 1, 0, render);//第一次数据渲染
    //render(template, testData, '#comments-list', 0, 'block');//渲染函数

    $('#id_comment_btn').find('li').each(function (i, e) {
        $(e).bind('click', function () {
            var dom = $(this);
            dom.siblings('.curr').removeClass('curr');
            dom.addClass('curr');
            if ($('#comment-' + i).length > 0) {
                $('#comments-list .mc').each(function () {
                    $(this).hide();
                });
                $('#comment-' + i).show();
            } else {
                $('#comments-list .mc').each(function () {
                    $(this).hide();
                });
                getAjax('/Product/GetCommentByProduct', pid, 1, i, render);// 按需载入评论数据 数据只生成一次 i代表类型
                //render(template, testData, '#comments-list', i, 'block');// 渲染数据
            }
            return false;// 阻止冒泡
        });
    });
    $('#comments-list').bind('click', function (e) {
        var t = e.target,
            pageno = (($(t).html()) >> 0),
            arr = (($(t).attr('data')) || '').split(',');
        if (pageno) {
            // 点击数字按钮
            $('#comment-' + arr[0]).remove();
            //render(template, testData, '#comments-list', arr[0], 'block');// 渲染数据
            getAjax('/Product/GetCommentByProduct', arr[1], pageno, arr[0], render);
        } else {
            if (arr[3] == 0) {
                // 上一步
                if ((arr[2] - 1) > 0) {// 当前页不是第一页
                    $('#comment-' + arr[0]).remove();
                    getAjax('/Product/GetCommentByProduct', arr[1], (+arr[2] - 1), arr[0], render);// arr[1]商品  arr[2]当前页  arr[0]类型
                    //render(template, testData, '#comments-list', arr[0], 'block');// 渲染数据
                }
            }
            if (arr[3] == 1) {
                // 下一步
                if (arr[2] < arr[4]) {// 当前页小于总页数
                    $('#comment-' + arr[0]).remove();
                    getAjax('/Product/GetCommentByProduct', arr[1], (+arr[2] + 1), arr[0], render);// arr[1]商品  arr[2]当前页  arr[0]类型
                    //render(template, testData, '#comments-list', arr[0], 'block');// 渲染数据
                }
            }
        }
    });
});