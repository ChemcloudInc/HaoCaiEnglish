$(function () {
    query();
    $("#searchBtn").click(function () { query(); });
    AutoComplete();
})

function Delete(id) {
    $.dialog.confirm('确定删除该用户吗？', function () {
        var loading = showLoading();
        $.post("./Delete", { id: id }, function (data) { $.dialog.tips(data.msg); query(); loading.close();});
    });
}
function Lock(id) {
    $.dialog.confirm('确定冻结该用户吗？', function () {
        var loading = showLoading();
        $.post("./Lock", { id: id }, function (data) { $.dialog.tips(data.msg); query(); loading.close(); });
    });
}
function UnLock(id) {
    $.dialog.confirm('确定重新激活该用户吗？', function () {
        var loading = showLoading();
        $.post("./UnLock", { id: id }, function (data) { $.dialog.tips(data.msg); query(); loading.close(); });
    });
}

function BatchLock() {
    var selectedRows = $("#list").hiMallDatagrid("getSelections");
 

    if (selectedRows.length == 0) {
        $.dialog.tips("你没有选择任何选项！");
    }
    else {
        var selectids=new Array();
        for (var i = 0; i < selectedRows.length; i++) {
            selectids.push(selectedRows[i].Id);
        }
        $.dialog.confirm('确定冻结选择的用户吗？', function () {
            var loading = showLoading();
            $.post("./BatchLock", { ids: selectids.join(',') }, function (data) { $.dialog.tips(data.msg); query(); loading.close(); });
        });
    }
}

function BatchDelete() {
    var selectedRows = $("#list").hiMallDatagrid("getSelections");
    var selectids = new Array();

    for (var i = 0; i < selectedRows.length; i++) {
        selectids.push(selectedRows[i].Id);
    }
    if (selectedRows.length == 0) {
        $.dialog.tips("你没有选择任何选项！");
    }
    else {
        $.dialog.confirm('确定删除选择的用户吗？', function () {
            var loading = showLoading();
            $.post("./BatchDelete", { ids: selectids.join(',') }, function (data) { $.dialog.tips(data.msg); query(); loading.close(); });
        });
    }
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
        queryParams: { keyWords: $("#autoTextBox").val() },
        toolbar: /*"#goods-datagrid-toolbar",*/'',
        operationButtons:"#batchOperate",
        columns:
        [[
            //{ checkbox: true, width: 39 },
            { field: "Id", hidden: true },
            { field: "UserName", title: '会员名' },
            {  field: "StrLastLoginDate", title: '最后登录日期'  },
{ field: "CellPhone", title: '手机' },
{ field: "StrCreateDate", title: '创建日期' },
            {
                field: "Disabled", title: '状态',
                formatter: function (value, row, index) {
                    var html = "";
                    if (row.Disabled)
                        html += '冻结';
                    else
                        html += '正常';
                    return html;
                }
            },
        {
            field: "operation", operation: true, title: "操作",
            formatter: function (value, row, index) {
                var id = row.Id.toString();
                var html = ["<span class=\"btn-a\">"];
                html.push("<a onclick=\"Show('" + id + "');\">查看</a>");
                html.push("<a onclick=\"ChangePassWord('" + id + "');\">修改密码</a>");
                //if (row.Disabled)
                //    html.push("<a onclick=\"UnLock('" + id + "');\">解冻</a>");
                //    else
                //    html.push("<a onclick=\"Lock('" + id + "');\">冻结</a>");
                //html.push("<a onclick=\"Delete('" + id + "');\">删除</a>");

                html.push("</span>");
                return html.join("");
            }
        }
        ]]
    });
}

function ChangePassWord(id)
{
    $.dialog({
        title: '修改密码',
        lock: true,
        id: 'ChangePwd',
        width: '400px',
        content: document.getElementById("dialogform"),
        padding: '20px 10px',
        okVal: '确定',
        init: function () { $("#password").focus();},
        ok: function () {
            var passwpord = $("#password").val();
            if (passwpord.length < 6) {
                $.dialog.errorTips("密码长度至少是6位！");
                return false;
            }
            var loading = showLoading();
            $.post("./ChangePassWord", { id: id, password: passwpord }, function (data) { $.dialog.tips(data.msg); $("#password").val(""); loading.close(); });
        }
    });

}


function AutoComplete() {
    //autocomplete
    $('#autoTextBox').autocomplete({
        source: function (query, process) {
            var matchCount = this.options.items;//返回结果集最大数量
            $.post("./getMembers", { "keyWords": $('#autoTextBox').val()}, function (respData) {
                return process(respData);
            });
        },
        formatItem: function (item) {
            return item.value;
        },
        setValue: function (item) {
            return { 'data-value': item.value, 'real-value': item.key };
        }
    });
}