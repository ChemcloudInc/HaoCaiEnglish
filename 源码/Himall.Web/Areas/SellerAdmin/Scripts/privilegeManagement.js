/// <reference path="E:\Projects\HiMall\trunk\src\Web\Himall.Web\Scripts/jquery-1.11.1.js" />
/// <reference path="E:\Projects\HiMall\trunk\src\Web\Himall.Web\Scripts/jquery.hiMallDatagrid.js" />


$(function () {
    query();
})

function Delete(id) {
    $.dialog.confirm('Determine the record to delete it?', function () {
        $.post("./Delete", { id: id }, function (data) { $.dialog.tips(data.msg); query() });
    });
}

function query() {
    $("#list").hiMallDatagrid({
        url: './list',
        nowrap: false,
        rownumbers: true,
        NoDataMsg: 'No matching data',
        border: false,
        fit: true,
        fitColumns: true,
        pagination: false,
        idField: "Id",
        queryParams: {},
        toolbar: /*"#goods-datagrid-toolbar",*/'',
        columns:
        [[
            { field: "Id", hidden: true },
            { field: "Name", title: 'Privilege groups name' },
           
        {
            field: "operation", operation: true, title: "Operation",
            formatter: function (value, row, index) {
                var id = row.Id.toString();
                var html = ["<span class=\"btn-a\">"];
                html.push("<a href='./Edit/" + id + "'>edit</a>");
                html.push("<a onclick=\"Delete('" + id + "');\">delete</a>");
                html.push("</span>");
                return html.join("");
            }
        }
        ]]
    });
}
