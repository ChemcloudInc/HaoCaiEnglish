//咨询
testData1 = {
    "successful": true,
    "consults": [
      {
          "UserName": "007",
          "ConsultationContent": "ConsultationContent",
          "ConsultationDate": "2014-09-08 12:00:00",
          "ReplyContent": "6665487",
          "ReplyDate": "2014-09-19 12:00:00"
      }
    ],
    "totalPage": 72,
    "currentPage": 14
};
$(function(){
        var pid = $('#gid').val(),// 商品id
            getAjax = function (url, pid, currentPage, fn) {
                $.ajax({
                    type: 'get',
                    url: url + '?pId=' + pid + '&pageNo=' + currentPage + '&pageSize=' + 10,
                    dataType: 'html',
                    cache: true,// 开启ajax缓存
                    success: function (data) {
                        if (data) {
                            var data = (new Function('return ' + data))();
                            fn(data);
                        }
                    },
                    error: function (e) {
                        //
                    }
                });
            },
            tempData = [],// 暂存数据 随后会立即清空
            render = function (data) {
                var str = '', i, e;
                for (i = 0; e = data.consults[i++];) {
                    str += '<div class="item"><div class="user"><span class="u-name">Customer：' + e.UserName + '</span>'
                       + '<span class="date-ask">' + e.ConsultationDate + '</span>'
                       + '<dl class="ask"><dt>Content：</dt><dd>' + html_decode(e.ConsultationContent) + '</dd></dl>';
                    
                    if (e.ReplyContent != "No Reply") {
                        str += '<dl class="answer"><dt>Seller Reply：</dt><dd><div class="content">' + html_decode(e.ReplyContent) + '</></div><div class="date-answer">' + e.ReplyDate + '</div></dd></dl>';
                    }
                        str+='</div></div>';
                }
                $('#consult-0').html(str);
                str = '<div class="cl"><div id="commentsPage0" class="pagin fr">';
                // 当前页码等于1
                if (data.currentPage == 1) {
                    if (data.totalPage <= 5) {
                        for (i = 1; i < data.totalPage + 1; i++) {
                            if (i == 1) {
                                str += '<a class="current">' + i + '</a>';
                            } else {
                                str += '<a href="#consult" data="' + pid + '">' + i + '</a>';
                            }
                        }
                    } else {
                        for (i = 1; i < 6; i++) {
                            if (i == 1) {
                                str += '<a class="current">' + i + '</a>';
                            } else {
                                str += '<a href="#consult" data="' + pid + '">' + i + '</a>';
                            }
                        }
                    }
                    if (data.totalPage > 5) {
                        str += '<span>...</span>';
                        if ((data.totalPage - 5) > 1) { str += '<a href="#consult" data="' + pid + '">' + (data.totalPage - 1) + '</a>'; }
                        str += '<a href="#consult" data="' + pid + '">' + (data.totalPage) + '</a>';
                    }

                    if (data.totalPage > 1) {
                        str += '<a class="next" href="#consult" data="' + pid + ',1,1,' + data.totalPage + '">Next</a>';
                    }
                } else if (data.currentPage > 1 && data.totalPage < 6) {// 总页数不超过5个
                    str += '<a class="prev" href="#consult" data="' + pid + ',' + data.currentPage + ',0">Previous</a>';
                    for (i = 1; i < data.totalPage + 1; i++) {
                        if (i == data.currentPage) {
                            str += '<a class="current">' + i + '</a>';
                        } else {
                            str += '<a href="#consult" data="' + pid + '">' + i + '</a>';
                        }
                    }
                    str += '<a class="next" href="#consult" data="' + pid + ',' + data.currentPage + ',1,' + data.totalPage + '">Next</a>';
                } else if (data.currentPage > 1 && data.totalPage > 5) {
                    str += '<a class="prev" href="#consult" data="' + pid + ',' + data.currentPage + ',0">Previous</a>';
                    if (data.currentPage < 6) {
                        for (i = 1; i < 6; i++) {
                            if (i == data.currentPage) {
                                str += '<a class="current">' + i + '</a>';
                            } else {
                                str += '<a href="#consult" data="' + pid + '">' + i + '</a>';
                            }
                        }
                        str += '<span>...</span>';

                        if ((data.totalPage - 5) > 1) { str += '<a href="#consult" data="' + pid + '">' + (data.totalPage - 1) + '</a>'; }
                        str += '<a href="#consult" data="' + pid + '">' + (data.totalPage) + '</a>';
                    } else if ((data.currentPage - 5) == 1) {
                        for (i = 1; i < 7; i++) {
                            if (i == data.currentPage) {
                                str += '<a class="current">' + i + '</a>';
                            } else {
                                str += '<a href="#consult" data="' + pid + '">' + i + '</a>';
                            }
                        }
                        str += '<a href="#consult" data="' + pid + '">' + (data.currentPage + 1) + '</a>';
                        str += '<a href="#consult" data="' + pid + '">' + (data.currentPage + 2) + '</a>';
                        str += '<span>...</span>';
                        str += '<a href="#consult" data="' + pid + '">' + (data.totalPage - 1) + '</a>';
                        str += '<a href="#consult" data="' + pid + '">' + (data.totalPage) + '</a>';
                    } else if ((data.currentPage - 5) > 1) {
                        str += '<a href="#consult" data="' + pid + '">1</a>';
                        str += '<a href="#consult" data="' + pid + '">2</a>';
                        str += '<span>...</span>';
                        str += '<a href="#consult" data="' + pid + '">' + (data.currentPage - 3) + '</a>';
                        str += '<a href="#consult" data="' + pid + '">' + (data.currentPage - 2) + '</a>';
                        str += '<a href="#consult" data="' + pid + '">' + (data.currentPage - 1) + '</a>';
                        str += '<a class="current" data="' + pid + '">' + data.currentPage + '</a>';
                        str += '<a href="#consult" data="' + pid + '">' + (data.currentPage + 1) + '</a>';
                        str += '<a href="#consult" data="' + pid + '">' + (data.currentPage + 2) + '</a>';
                        str += '<span>...</span>';
                        str += '<a href="#consult" data="' + pid + '">' + (data.totalPage - 1) + '</a>';
                        str += '<a href="#consult" data="' + pid + '">' + (data.totalPage) + '</a>';

                    }
                    str += '<a class="next" href="#consult" data="' + pid + ',' + data.currentPage + ',1,' + data.totalPage + '">Next</a>';
                }
                str += '</div></div>';
                $('#consult-0').append(str);
            };
        getAjax('/Product/GetConsultationByProduct', pid, 1, render);
        //render(testData1);
        $('#consult').bind('click', function (e) {
            var t = e.target,
                pageno = (($(t).html()) >> 0),
                arr = (($(t).attr('data')) || '').split(',');
            //console.log(arr);
            if (pageno) {
                // 点击数字按钮
                //render(testData1);// 渲染数据
                getAjax('/Product/GetConsultationByProduct', pid, pageno, render);
            } else {
                if (arr[2] == 0) {
                    // 上一步
                    if ((arr[1] - 1) > 0) {// 当前页不是第一页

                        getAjax('/Product/GetConsultationByProduct', arr[0], (+arr[1] - 1), render);// arr[0]商品id  arr[1]当前页
                        //render(testData1);// 渲染数据
                    }
                }
                if (arr[2] == 1) {
                    // 下一步
                    if (arr[1] < arr[3]) {// 当前页小于总页数
                        getAjax('/Product/GetConsultationByProduct', arr[0], (+arr[2] + 1), render);// arr[0]商品id  arr[2]当前页
                        //render(testData1);// 渲染数据
                    }
                }
            }
        });
});

function html_decode(str) {
    var s = "";
    if (str.length == 0) return "";
	s = str.replace(/<[^>]+>/g,"");
    /*s = str.replace(/&amp;/g, "&");
    s = s.replace(/&lt;/g, "<");
    s = s.replace(/&gt;/g, ">");
    s = s.replace(/&nbsp;/g, " ");
    s = s.replace(/&#39;/g, "\'");
    s = s.replace(/&quot;/g, "\"");
    s = s.replace(/<br\/>/g, "\n");*/
    return s;
}