/// <reference path="E:\Projects\HiMall\trunk\src\Web\Himall.Web\Scripts/jquery-1.11.1.js" />
/// <reference path="E:\Projects\HiMall\trunk\src\Web\Himall.Web\Scripts/jquery.hiMallDatagrid.js" />

$(function () {
    var tips=$(window.parent.document).find('#UnReplyConsultations').text().replace('(','').replace(')','');
	if(tips&&tips>0){
		query('false')
	}else{
		query('')
	}

	$("#replyContent").blur(function () {
	    var content = $("#replyContent").val();
	    if(content.length>300||!content)
	    {
	        $("#consultationCotentTip").text("回复内容在200个字符以内！");
	        $("#replyContent").css({ border: '1px solid #f60' });
	        return false;
	    }
	    else
	    {
	        $("#consultationCotentTip").text("");
	        $("#replyContent").css({ border: '1px solid #ccc' });
	    }
	})
	   
	
})


function detail(id) {
    $.post("./Detail", { id: id }, function (data) {
        $.dialog({
            title: '查看回复',
            lock: true,
            id: 'consultReply',
            width: '400px',
            content: ['<div class="dialog-form">',
                '<div class="form-group">',
                    '<label class="label-inline fl">咨询</label><p class="only-text">' + html_decode(data.ConsulationContent) + '</p></div>',
                '<div class="form-group">',
                    '<label class="label-inline fl">咨询回复</label><p class="only-text">' + html_decode(data.ReplyContent) + '</p></div>',
            '</div>'].join(''),
            padding: '20px 10px',
            okVal: '确定',
            ok: function () {
            }
        });
    });

}

function ReplyConsulation(id)
{
    $.dialog({
        title: '回复咨询',
        lock: true,
        id: 'ReplyConsulation',
        content: document.getElementById("reply-form"),
        padding: '20px 10px',
        okVal: '确定',
        init: function () { $("#replyContent").focus(); },
        ok: function () {
            var replycontent = $("#replyContent").val();
            if (replycontent.trim() == "" || replycontent.length > 200) {
                $("#consultationCotentTip").text("回复内容在200个字符以内！");
                $("#replyContent").css({ border: '1px solid #f60' });
                return false;
            }
            var loading = showLoading();
            $.post("./ReplyConsultation",
                { id: id, replycontent: replycontent },
                function (data) {
                    loading.close();
                    if (data.success) {
                        $.dialog.succeedTips("回复成功", function () {
                            $("#replyContent").val("");
                            var pageNo = $("#list").hiMallDatagrid('options').pageNumber;
                            $("#list").hiMallDatagrid('reload', { pageNumber: pageNo });
                        });
                    }
                    else
                        $.dialog.errorTips("回复失败:" + data.msg);
                });
        }
    });
}



function query(val) {
	$('.nav li').each(function() {
		if($(this).attr('flag')==val){
			$(this).addClass('active').siblings().removeClass('active');
		}
	});
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
        queryParams: { isReply: val},
        toolbar: /*"#goods-datagrid-toolbar",*/'',
        columns:
        [[
            { field: "Id", hidden: true },
            {
                field: "ProductName", title: '评价商品', align: "left", width: 350,
                formatter: function (value, row, index) {
                    var html = '<a title="' + value + '" href="/product/detail/' + row.ProductId + '" target="_blank" href="/product/detail/' + row.ProductId + '"><img style="margin-left:15px;" width="40" height="40" src="' + row.ImagePath + '/1_100.png" /><span class="overflow-ellipsis"style="width:200px">' + value + '</a></span>';
                    return html;
                }
            },
            { field: "ConsultationContent", title: '咨询内容', align: "left",width:350 },
            { field: "UserName", title: '咨询人' },
            { field: "ConsultationDateStr", title: '日期' },
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
                    html.push("<a onclick=\"detail('" + id + "');\">查看回复</a>");
                }
                else
                    html.push("<a onclick=\"ReplyConsulation('" + id + "');\">回复</a>");
                html.push("</span>");
                return html.join("");
            }
        }, { field: "ReplyContent", hidden: true },
        ]]
    });
}

function html_decode(str) {
    var s = "";
    if (str.length == 0) return "";
	s = str.replace(/<[^>]+>/g,"");
    return s;
}
