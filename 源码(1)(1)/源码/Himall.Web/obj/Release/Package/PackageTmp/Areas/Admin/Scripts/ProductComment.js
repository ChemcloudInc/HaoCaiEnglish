$(function () {
    query();
})

function deleteComment(id) {
    $.dialog.confirm('确定删除该评论吗？', function () {
        var loading = showLoading();
        $.post("./Delete", { id: id }, function (data) { $.dialog.tips(data.msg); query(); loading.close(); });
    });
}

function ShowStatus(obj) {
    $(".container ul li").removeClass("active");
    $(obj).parent().attr("class", "active");
    query();
}

function query() {
    $("#list").hiMallDatagrid({
        url: './list',
        nowrap: false,
        rownumbers: true,
        NoDataMsg: '没有找到符合条件的数据',
        border: false,
        fit: true,
        fitColumns: true,
        pagination: true,
        idField: "Id",
        pageSize: 10,
        pageNumber: 1,
        queryParams: { isReply: $("#notReply").parent().attr("class") == "active" ? "false" : "" },
        toolbar: /*"#goods-datagrid-toolbar",*/'',
        columns:
        [[
            { field: "Id", hidden: true },            
            {
                field: "ProductName", title: '评价商品', align: "center", width: 300,
                formatter: function (value, row, index) {
                    var spc = " ";
                    if (row.Color.length > 0) { spc += "颜色：" + row.Color; }
                    if (row.Size.length > 0) { spc += "，尺寸：" + row.Size; }
                    if (row.Version.length > 0) { spc += "，版本：" + row.Version; }
                    var html = '<a title="' + value +"【"+spc+ '】" href="/product/detail/' + row.ProductId + '" target="_blank" href="/product/detail/' + row.ProductId + '"><img style="margin-left:15px;" width="40" height="40" src="' + row.ImagePath + '" /><span class="overflow-ellipsis"style="width:200px">'+ value + '</a></span>';                    
                    return html;
                }
            },
            { field: "CommentContent", title: '评价内容', align: "center",width:220,
				formatter: function (value, row, index) {
					return '<span style="width:350px; word-break:break-all;">'+value+'</span>';
				}
			},
            { field: "CommentMark", title: '商品评分' },
            { field: "UserName", title: '评价人' },
            { field: "Date", title: '评价日期', width: 100, },
            {
                field: "state", title: '状态', width: 80,
                formatter: function (value, row, index) {
                    var html = "";
                    if (row.Status)
                        html += '已回复';
                    else
                        html += '未回复';
                    return html;
                }
            },
        {
            field: "operation", operation: true, title: "操作", width: 90,
            formatter: function (value, row, index) {
                var id = row.Id.toString();
                var html = ["<span class=\"btn-a\">"];
                if (row.Status) {
                    html.push("<a onclick=\"detail('" + id + "');\">查看回复</a>");
                }
                html.push("<a onclick=\"deleteComment('" + id + "');\">清空评价</a>");
                html.push("</span>");
                return html.join("");
            }
        }
        ]]
    });
}
function detail(id) {
    $.post("./Detail", { id: id }, function (data) {
        $.dialog({
            title: '查看回复',
            lock: true,
            id: 'consultReply',
            width: '400px',
            content: ['<div class="dialog-form">',
                '<div class="form-group">',
                    '<label class="label-inline fl">评论</label>',
                    '<p class="only-text">' + data.ConsulationContent + '</p>',
                '</div>',
                '<div class="form-group">',
                    '<label class="label-inline fl">评论回复</label>',
                    '<p class="only-text">' + data.ReplyContent + '</p>',
                '</div>',
            '</div>'].join(''),
            padding: '20px 10px',
            okVal: '确定',
            ok: function () {
            }
        });
    });

}