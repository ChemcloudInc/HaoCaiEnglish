$(function () {

    initGrid();
    bindSearchBtnClickEvent();
})



function initGrid() {
    //商品表格
    $("#list").hiMallDatagrid({
        url: 'list',
        nowrap: false,
        rownumbers: true,
        NoDataMsg: '没有找到符合条件的数据',
        border: false,
        fit: true,
        fitColumns: true,
        pagination: true,
        idField: "Id",
        pageSize: 9,
        pagePosition: 'bottom',
        pageNumber: 1,
        queryParams: {},
        operationButtons: null,
        columns:
        [[
             { checkbox: true, width: 39 },
            { field: "Name", title: 'format name', width: 450, align: 'left' },
            { field: "PositionText", title: 'format location', width: 100, align: 'left' },
        {
            field: "s", title: "operation", width: 90, align: "center",
            formatter: function (value, row, index) {
                var html = "";
                html = '<span class="btn-a">\
                    <a class="good-check" href="Add?id=' + row.Id + '">edit</a>';
                html += '<a class="good-check" onclick="del(' + row.Id + ',\'' + row.Name + '\')">delete</a></span>';
                return html;
            },
            styler: function () {
                return 'td-operate';
            }
        }
        ]],
        
    });

}

function bindSearchBtnClickEvent() {
    $('#searchBtn').click(function () {
        search();

    });

}

function del(id,name) {
    $.dialog.confirm('Are you sure delete tenplate \'' + name + "\'?", function () {
        var loading = showLoading();
        $.post('delete', { id: id }, function (result) {
            loading.close();
            if (result.success) {
                $.dialog.succeedTips('delete successful!', function () { search(); });
            }
            else
                $.dialog.errorTips('delete failed!' + result.msg);
        });
    });
}


function search() {
    var position = $('#position').val();
    var name = $.trim($('#name').val());

    $("#list").hiMallDatagrid('reload', { name: name, position: position });

}