function query()
{
    //订单表格
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
        pageSize: 15,
        pagePosition: 'bottom',
        pageNumber: 1,
        queryParams: {},
        columns:
        [[
            { field: "OrderId", title: '订单号', width: 120 },
            { field: "ShopName", title: "店铺", width: 150, align: "center" },
            { field: "UserName", title: "评价会员", width: 80, align: "center" },
            { field: "PackMark", title: "商品包装满意度", width: 120, align: "center" },
            { field: "DeliveryMark", title: "送货速度满意度", width: 120, align: "center" },
            { field: "ServiceMark", title: "配送人员的服务满意度", width: 130, align: "center" },
            { field: "CommentDate", title: "评价日期", width: 70, align: "center" },
            {
                field: "operation", operation: true, title: "操作",
                formatter: function (value, row, index) {
                    var id = row.OrderId.toString();
                    var html = ["<span class=\"btn-a\">"];
                    html.push("<a onclick=\"deleteOrderComment('" + row.Id + "');\">删除</a>");
                    html.push("</span>");
                    return html.join("");
                }
            }
        ]]
    });
}


$(function () {

    query();

    $('#searchButton').click(function (e) {
        searchClose(e);
        var startDate = $("#inputStartDate").val();
        var endDate = $("#inputEndDate").val();
        var orderId = $.trim($('#txtOrderId').val());
        var shopName = $.trim($('#txtShopName').val());
        var productName = $.trim($('#txtProductName').val());
        var userName = $.trim($('#txtUserName').val());
        $("#list").hiMallDatagrid('reload', { startDate: startDate, endDate: endDate, orderId: orderId, shopName: shopName, productName: productName, userName: userName });
    })
});

function deleteOrderComment(id) {
    $.dialog.confirm('确定删除该评价吗？', function () {
        var loading = showLoading();
        $.post("./Delete", { id: id }, function (data) { loading.close(); $.dialog.tips(data.msg); query(); });
        var pageNo = $("#list").hiMallDatagrid('options').pageNumber;
        $("#list").hiMallDatagrid('reload', { pageNumber: pageNo });
    });
}

