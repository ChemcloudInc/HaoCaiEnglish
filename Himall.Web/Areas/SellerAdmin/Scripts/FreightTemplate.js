$(function () {
    LoadData();
});
function LoadData() {
    $("#list").hiMallDatagrid({
        url: 'List',
        nowrap: false,
        rownumbers: true,
        NoDataMsg: '没有运费模版',
        border: false,
        fit: true,
        fitColumns: true,
        pagination: false,
        idField: "Id",
        pageSize: 15,
        pageNumber: 1,
        queryParams: {},
        toolbar: /*"#goods-datagrid-toolbar",*/'',
        columns:
        [[
            { field: "Id", hidden: true },
            { field: "Name", title: '模版名称' },

        {
            field: "operation", operation: true, title: "操作",width:300,
            formatter: function (value, row, index) {
                var id = row.Id;
                var html = ["<span class=\"btn-a\"><a href='Add?id=" + id + "'>查看详情</a></span>"];
                html.push("<span class=\"btn-a\"><a  href='Add?id=" + id + "'>编辑</a></span>");
                html.push("<span class=\"btn-a\"><a  onclick='DeleteTemplate(" + id + ")'>删除</a></span>");
                return html.join("");
            }
        }
        ]]
    });
};
function DeleteTemplate(id) {
    $.dialog.confirm('确认删除此模板吗？', function () {
        var loading = showLoading();
        $.post('DeleteTemplate', { id: id }, function (result) {
            loading.close();
            if (result.successful) {
                $.dialog.tips('删除成功！');
                LoadData();
            }
            else {
                $.dialog.errorTips(result.msg);
            }
        });
    });
    
}

