
$(function () {
    var status = GetQueryString("status");
    var li = $("li[value='" + status + "']");
    if (li.length > 0) {
        li.addClass('active').siblings().removeClass('active');
    }

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
        idField: "OrderId",
        pageSize: 15,
        pagePosition: 'bottom',
        pageNumber: 1,
        queryParams: { orderStatus: status },
        columns:
        [[
            {
                field: "OrderId", title: '订单号', width: 170,
                formatter: function (value, row, index) {
                    return '<img src="' + row.IconSrc + '" title="' + row.PlatformText + '订单" width="16" />' + value;
                }
            },
            { field: "ShopName", title: "店铺", width: 180, align: "left" },
            { field: "UserName", title: "买家", width: 150, align: "left" },
            { field: "OrderDate", title: "下单时间", width: 150, align: "left" },
            {
                field: "TotalPrice", title: "订单总额", width: 80, align: "right",
                formatter: function (value, row, index) {
                    return '￥' + value.toFixed(2);
                }
            },
        //{ field: "PaymentTypeName", title: "支付方式", width: 80, align: "left" },
        { field: "OrderStatus", title: "订单状态", width: 100, align: "center" },
        {
            field: "operation", operation: true, title: "操作",
            formatter: function (value, row, index) {
                var id = row.OrderId.toString();
                var html = ["<span class=\"btn-a\">"];
                html.push("<a href='./Detail/" + id + "'>查看</a>");
                if (row.OrderStatus == "Wait Pay") {
                    html.push("<a class=\"good-check\" onclick=\"OpenConfirmPay('" + id + "')\">确认收款</a>");
                    html.push("<a class=\"good-check\" onclick=\"OpenCloseOrder('" + id + "')\">取消</a>");
                }
                html.push("</span>");
                return html.join("");
            }
        }
        ]]
    });


    $('#searchButton').click(function (e) {
        searchClose(e);
        var startDate = $("#inputStartDate").val();
        var endDate = $("#inputEndDate").val();
        var orderId = $.trim($('#txtOrderId').val());
        var shopName = $.trim($('#txtShopName').val());
        var userName = $.trim($('#txtUserName').val());
        $("#list").hiMallDatagrid('reload', { startDate: startDate, endDate: endDate, orderId: orderId, shopName: shopName, userName: userName, paymentTypeGateway: $("#selectPaymentTypeName").val() });
    })


    $('.nav li').click(function (e) {
        searchClose(e);
        $(this).addClass('active').siblings().removeClass('active');
        if ( $( this ).attr( 'type' ) == 'statusTab' )
        {//状态分类
            $('#txtOrderId').val('');
            $('#txtShopName').val('');
            $('#txtuserName').val('');
            $( "#list" ).hiMallDatagrid( 'reload', { orderStatus: $( this ).attr( 'value' ) || null, pageNumber: 1, startDate: '', endDate: '', orderId: '', shopName: '', userName: '', paymentTypeGateway: '' } );
        }
    });
});

function OpenConfirmPay(orderId) {

    $.dialog({
        title: '确认收款',
        lock: true,
        id: 'goodCheck',
        content: ['<div class="dialog-form">',
            '<div class="form-group">',
                '<p class="help-top">收款备注</p>',
                '<textarea id="txtPayRemark" class="form-control" cols="40" rows="2" onkeyup="this.value = this.value.slice(0, 50)" ></textarea>\
                 <p id="valid" style="visibility:hidden;color:red">请填写未通过理由</p> ',
            '</div>',
        '</div>'].join(''),
        padding: '10px',
        init: function () { $("#txtPayRemark").focus(); },
        button: [
        {
            name: '确认收款',
            callback: function () {
                ConfirmPay(orderId, $('#txtPayRemark').val());
            },
            focus: true
        }]
    });
}

function OpenCloseOrder(orderId) {
    $.dialog({
        title: '取消订单',
        lock: true,
        id: 'goodCheck',
        content: ['<div class="dialog-form">',
            '<div class="form-group">',
                '<p class="help-top">确认要取消订单吗？取消后订单将会是关闭状态。</p>',
            '</div>',
        '</div>'].join(''),
        padding: '10px',
        button: [
        {
            name: '确认取消',
            callback: function () {
                CloseOrder(orderId);
            },
            focus: true
        }]
    });
}


function ConfirmPay(orderId, payRemark) {
    var loading = showLoading();
    $.post('./ConfirmPay', { orderId: orderId, payRemark: payRemark }, function (result) {
        if (result.success) {
            $.dialog.succeedTips("操作成功！");
            var pageNo = $("#list").hiMallDatagrid('options').pageNumber;
            $("#list").hiMallDatagrid('reload', { pageNumber: pageNo });
        }
        else
            $.dialog.errorTips("操作失败"+result.msg);
        loading.close();
    });
}

function CloseOrder(orderId) {
    var loading = showLoading();
    $.post('./CloseOrder', { orderId: orderId }, function (result) {
        if (result.success) {
            $.dialog.succeedTips("操作成功！");
            var pageNo = $("#list").hiMallDatagrid('options').pageNumber;
            $("#list").hiMallDatagrid('reload', { pageNumber: pageNo });
        }
        else
            $.dialog.errorTips("操作失败");
        loading.close();
    });
}

