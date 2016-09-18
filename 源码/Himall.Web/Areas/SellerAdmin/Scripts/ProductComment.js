$(function () {
    var tips = $(window.parent.document).find('#UnReplyComments').text().replace('(', '').replace(')', '');
    if (tips && tips > 0) {
        query('false')
    } 
    var status = GetQueryString('status');
    if (status && status > 0) {
        query('false');
    }
    else {
        query('');
    }

    $("#replyContent").blur(function () {
        var content = $("#replyContent").val();
        if (content.length > 300 || !content) {
            $("#commentCotentTip").text("Reply must be shorter than 200 words!");
            $("#replyContent").css({ border: '1px solid #f60' });
            return false;
        }
        else {
            $("#commentCotentTip").text("");
            $("#replyContent").css({ border: '1px solid #ccc' });
        }
    })
});

function ReplyComment(id) {
    $.dialog({
        title: 'Reply',
        lock: true,
        id: 'ReplyComment',
        content: document.getElementById("reply-form"),
        padding: '20px 10px',
        okVal: 'sure',
        init: function () { $("#replyContent").focus();},
        ok: function () {
            //var loading = showLoading();
            var replycontent = $("#replyContent").val();
            if (replycontent == "" || replycontent.length > 200) {
                $("#consultationCotentTip").text("Reply must be shorter than 200 words!");
                $("#replyContent").css({ border: '1px solid #f60' });
                return false;
            }
            var loading = showLoading();
            $.post("./ReplyComment",
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
        NoDataMsg: 'No matching data',
        border: false,
        fit: true,
        fitColumns: true,
        pagination: true,
        idField: "Id",
        pageSize: 10,
        pageNumber: 1,
        queryParams: { isReply: val },
        toolbar: /*"#goods-datagrid-toolbar",*/'',
        columns:
        [[
            { field: "Id", hidden: true },            
            {
                field: "ProductName", title: 'Comments', align: "left", width: 300,
                formatter: function (value, row, index) {
                    var spc = " ";
                    if (row.Color.length > 0) { spc += "color:" + row.Color; }
                    if (row.Size.length > 0) { spc += "，size:" + row.Size; }
                    if (row.Version.length > 0) { spc += "，versions:" + row.Version; }
                    var html = '<a title="' + value + "【" + spc + '】" href="/product/detail/' + row.ProductId + '" target="_blank" href="/product/detail/' + row.ProductId + '"><img style="margin-left:15px;" width="40" height="40" src="' + row.ImagePath + '" /><span class="overflow-ellipsis"style="width:200px">' + value + '</a></span>';
                    return html;
                }
            },
            { field: "CommentContent", title: 'Comment content', align: "left",width:300,
                formatter: function (value, row, index) {
                    var html = '<span title="' + value + '" class="overflow-ellipsis"style="width:300px">' + value + '</span>';
                    return html;
                }},
            { field: "CommentMark", title: 'Grade' },
            { field: "UserName", title: 'Appraiser' },
            { field: "CommentDateStr", title: 'Date' },
            {
                field: "state", title: 'Status',
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
                    html.push("<a onclick=\"detail('" + id + "');\">View reply</a>");
                }
                else
                html.push("<a onclick=\"ReplyComment('" + id + "');\">reply</a>");
                html.push("</span>");
                return html.join("");
            }
        },
       { field: "ReplyContent", hidden: true },

        ]]
    });
}
function detail(id) {
    $.post("./Detail", { id: id }, function (data) {
        var content = data.ConsulationContent == "" ? "None" : data.ConsulationContent;
        $.dialog({
            title: 'View reply',
            lock: true,
            id: 'consultReply',
            width: '400px',
            content: ['<div class="dialog-form">',
                '<div class="form-group">',
                    '<label class="label-inline fl">Comments</label>',
                    '<p class="only-text">' + content + '</p>',
                '</div>',
                '<div class="form-group">',
                    '<label class="label-inline fl">Reply</label>',
                    '<p class="only-text">' + data.ReplyContent + '</p>',
                '</div>',
            '</div>'].join(''),
            padding: '20px 10px',
            okVal: 'Sure',
            ok: function () {
            }
        });
    });

}