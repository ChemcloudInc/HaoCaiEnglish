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
	        $("#consultationCotentTip").text("Reply must be shorter than 200 words!");
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
            title: 'view reply',
            lock: true,
            id: 'consultReply',
            width: '400px',
            content: ['<div class="dialog-form">',
                '<div class="form-group">',
                    '<label class="label-inline fl">consultation</label><p class="only-text">' + html_decode(data.ConsulationContent) + '</p></div>',
                '<div class="form-group">',
                    '<label class="label-inline fl">reply</label><p class="only-text">' + html_decode(data.ReplyContent) + '</p></div>',
            '</div>'].join(''),
            padding: '20px 10px',
            okVal: 'sure',
            ok: function () {
            }
        });
    });

}

function ReplyConsulation(id)
{
    $.dialog({
        title: 'reply',
        lock: true,
        id: 'ReplyConsulation',
        content: document.getElementById("reply-form"),
        padding: '20px 10px',
        okVal: 'sure',
        init: function () { $("#replyContent").focus(); },
        ok: function () {
            var replycontent = $("#replyContent").val();
            if (replycontent.trim() == "" || replycontent.length > 200) {
                $("#consultationCotentTip").text("Reply must be shorter than 200 words!");
                $("#replyContent").css({ border: '1px solid #f60' });
                return false;
            }
            var loading = showLoading();
            $.post("./ReplyConsultation",
                { id: id, replycontent: replycontent },
                function (data) {
                    loading.close();
                    if (data.success) {
                        $.dialog.succeedTips("Reply successful", function () {
                            $("#replyContent").val("");
                            var pageNo = $("#list").hiMallDatagrid('options').pageNumber;
                            $("#list").hiMallDatagrid('reload', { pageNumber: pageNo });
                        });
                    }
                    else
                        $.dialog.errorTips("Reply failed:" + data.msg);
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
        NoDataMsg: 'Not found any data',
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
                field: "ProductName", title: 'Products', align: "left", width: 350,
                formatter: function (value, row, index) {
                    var html = '<a title="' + value + '" href="/product/detail/' + row.ProductId + '" target="_blank" href="/product/detail/' + row.ProductId + '"><img style="margin-left:15px;" width="40" height="40" src="' + row.ImagePath + '/1_100.png" /><span class="overflow-ellipsis"style="width:200px">' + value + '</a></span>';
                    return html;
                }
            },
            { field: "ConsultationContent", title: 'Consult content', align: "left",width:300 },
            { field: "UserName", title: 'Consultant' },
            { field: "ConsultationDateStr", title: 'Date' },
            {
                field: "state", title: 'State',
                formatter: function (value, row, index) {
                    var html = "";
                    if (row.Status)
                        html += 'replied';
                    else
                        html += 'not reply';
                    return html;
                }
            },
        {
            field: "operation", operation: true, title: "Operation",
            formatter: function (value, row, index) {
                var id = row.Id.toString();
                var html = ["<span class=\"btn-a\">"];
                if (row.Status) {
                    html.push("<a onclick=\"detail('" + id + "');\">view reply</a>");
                }
                else
                    html.push("<a onclick=\"ReplyConsulation('" + id + "');\">reply</a>");
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
