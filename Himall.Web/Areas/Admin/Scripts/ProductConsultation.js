/// <reference path="E:\Projects\HiMall\trunk\src\Web\Himall.Web\Scripts/jquery-1.11.1.js" />
/// <reference path="E:\Projects\HiMall\trunk\src\Web\Himall.Web\Scripts/jquery.hiMallDatagrid.js" />

$(function () {
    query();
})

function deleteConsulation(id)
{
    $.dialog.confirm('确定删除该咨询吗？', function () {
        var loading = showLoading();
        $.post("./Delete", { id: id }, function (data) {
            $.dialog.tips(data.msg); query(); loading.close();
        });
    });
}

function detail(id)
{
    $.post("./Detail", { id: id }, function (data) {
        $.dialog({
            title: '查看回复',
            lock: true,
            id: 'consultReply',
            width: '400px',
            content: ['<div class="dialog-form">',
                '<div class="form-group">',
                    '<label class="label-inline fl">咨询</label>',
                    '<textarea  style="resize: none; border:0px;" >' + data.ConsulationContent + '</textarea>',
                '</div>',
                '<div class="form-group">',
                    '<label class="label-inline fl">咨询回复</label>',
                    '<textarea  style="resize: none; border:0px;" >' + data.ReplyContent + '</textarea>',
                '</div>',
            '</div>'].join(''),
            padding: '20px 10px',
            okVal: '确定',
            ok: function () {
            }
        });
    });
   
}

function ShowStatus(obj)
{
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
        queryParams: {isReply:$("#notReply").parent().attr("class")=="active"?"false":""},
        toolbar: /*"#goods-datagrid-toolbar",*/'',
        columns:
        [[
            { field: "Id", hidden: true },            
            {
                field: "ProductName", title: '评价商品', align: "center", width: 300,
                formatter: function (value, row, index) {
                    var html = '<a title="' + value + '" href="/product/detail/' + row.ProductId + '" target="_blank" href="/product/detail/' + row.ProductId + '"><img style="margin-left:15px;" width="40" height="40" src="' + row.ImagePath + '" /><span class="overflow-ellipsis"style="width:200px">' + value + '</a></span>';
                    return html;
                }
            },
            { field: "ConsultationContent", title: '咨询内容', align: "center",width:300,
				formatter: function (value, row, index) {
					return '<span style="width:300px; word-break:break-all;">'+value+'</span>';
				}
			},
            { field: "UserName", title: '咨询人' },
            { field: "Date", title: '咨询日期' }, 
            {
                field: "state", title: '咨询状态',
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
            field: "operation", operation: true, title: "操作",
            formatter: function (value, row, index) {
                var id = row.Id.toString();
                var html = ["<span class=\"btn-a\">"];
                if (row.Status) {
                    html.push("<a onclick=\"detail('"+id+"');\">查看回复</a>");
                }
                html.push("<a onclick=\"deleteConsulation('" + id + "');\">删除</a>");
                html.push("</span>");
                return html.join("");
            }
        }
        ]]
    });
}
