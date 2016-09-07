$(function () {
    var status = GetQueryString('status');
    if (status && status > 0) {
        typeChoose('1');
    }
    else {
        typeChoose('')
    }

    function typeChoose(val) {
        $('.nav li').each(function () {
            if ($(this).val() == val) {
                $(this).addClass('active').siblings().removeClass('active');
            }
        });
    }

    //组合显示字段
    try {
        showtype = parseInt(showtype, 10);
    } catch (ex) {
        showtype = 0;
    }
    datacols = [[
            { field: "OrderId", title: 'Order no.', width: 120 },
            { field: "ProductName", title: "Products", width: 280, align: "left" },
            { field: "UserName", title: "Buyer", width: 80, align: "left" },
            { field: "ApplyDate", title: "Application date", width: 120, align: "left" },
            {
                field: "Amount", title: "Refund amount", width: 90, align: "left",
                formatter: function (value, row, index) {
                    return '$' + value;
                }
            }]];
    switch (showtype) {
        case 0:
        case 3:
            datacols[0].push({ field: "ReturnQuantity", title: "Return quantity", width: 80, align: "center" });
            break;
    }

    datacols[0] = datacols[0].concat([
        { field: "RefundStatus", title: "Refund status", width: 90, align: "center" },
        {
            field: "operation", operation: true, title: "Operation",
            formatter: function (value, row, index) {
                var html = ["<span class=\"btn-a\">"];
                html.push("<input type=\"hidden\" name=\"rowdata\" id=\"rowdata-" + row.RefundId + "\" value='" + jQuery.toJSON(row) + "'>");
                switch (row.AuditStatus) {
                    case "待商家审核":
                        html.push("<a class=\"good-check\" onclick=\"OpenDealRefund('" + row.RefundId + "')\">审核售后</a>");
                        break;
                    case "待商家收货":
                        html.push("<a class=\"good-check\" onclick=\"OpenConfirmGood('" + row.RefundId + "','" + row.ExpressCompanyName + "','" + row.ShipOrderNumber + "')\">审核售后确认收货</a>");
                        break;
                    default:
                        html.push("<a class=\"good-check\" onclick=\"ShowRefundInfo('" + row.RefundId + "')\">查看原因</a>");
                        break;
                }
                html.push("</span>");
                return html.join("");
            }
        }
    ]);

    //订单表格
    $("#list").hiMallDatagrid({
        url: './list?showtype=' + showtype,
        nowrap: false,
        rownumbers: true,
        NoDataMsg: 'Not found any records.',
        border: false,
        fit: true,
        fitColumns: true,
        pagination: true,
        idField: "RefundId",
        pageSize: 15,
        pagePosition: 'bottom',
        pageNumber: 1,
        queryParams: { auditStatus: status },
        columns: datacols
    });

    $('#searchButton').click(function (e) {
        searchClose(e);
        var startDate = $("#inputStartDate").val();
        var endDate = $("#inputEndDate").val();
        var orderId = $.trim($('#txtOrderId').val());
        var productName = $.trim($('#txtProductName').val());
        var userName = $.trim($('#txtUserName').val());
        $("#list").hiMallDatagrid('reload', { startDate: startDate, endDate: endDate, orderId: orderId, productName: productName, userName: userName });
    })


    $('.nav li').click(function (e) {
        searchClose(e);
        $(this).addClass('active').siblings().removeClass('active');
        if ($(this).attr('type') == 'statusTab') {//状态分类
            $('#txtOrderId').val('');
            $('#txtUserName').val('');
            $("#txtProducdName").val('');
            $("#list").hiMallDatagrid('reload', { auditStatus: $(this).attr('value') || null });
        }
    });
});

function OpenDealRefund(refundId) {
    var dobj = $("#rowdata-" + refundId);
    var data = jQuery.parseJSON(dobj.val());
    var jettisonRadio = "";

    dlgcontent = ['<div class="dialog-form">',
            '<div class="form-group">',
                '<label class="label-inline fl">商品名称</label>',
                '<p class="only-text">' + data.ProductName + '</p>',
            '</div>'];
    dlgcontent = dlgcontent.concat(['<div class="form-group">',
                '<label class="label-inline fl">退款金额</label>',
                '<p class="only-text"><span class="cor-red">￥' + data.Amount + '</span>（实付：' + data.SalePrice + '）</p>',
            '</div>']);
    if (data.RefundMode != 1) {
        if (data.ReturnQuantity > 0) {
            dlgcontent = dlgcontent.concat(['<div class="form-group">',
                        '<label class="label-inline fl">退货数量</label>',
                        '<p class="only-text"><span class="cor-red">' + data.ReturnQuantity + "</span>（购买：" + data.Quantity + "）" + '</p>',
                    '</div>']);
        }
    } else {
        data.ReturnQuantity = 0;
    }
    dlgcontent = dlgcontent.concat([
            '<div class="form-group">',
                '<label class="label-inline fl">原因</label>',
                '<p class="only-text">' + data.Reason.replace(/>/g, '&gt;').replace(/</g, '&lt;') + '</p>',
            '</div>',
            '<div class="form-group">',
                '<label class="label-inline fl">联系人</label>',
                '<p class="only-text">' + data.ContactPerson + "（" + data.ContactCellPhone + "）" + '</p>',
            '</div>',
            '<div class="form-group">',
                '<label class="label-inline fl">期望退款方式</label>',
                '<p class="only-text">' + data.RefundPayType + '</p>',
            '</div>',
            '<div class="form-group">',
                '<textarea class="form-control" cols="56" rows="3" id="txtRefundRemark" placeholder="回复买家"></textarea>',
            '</div>',
        '</div>']);

    var dlgbt = [{
        name: '拒绝',
        callback: function () {
            sellerRemark = $('#txtRefundRemark').val();
            if (sellerRemark.length < 1) {
                alert("请输入拒绝理由！");
                return false;
            }
            DealRefund(data.RefundId, 4, sellerRemark);
        }
    }];
    if (data.ReturnQuantity > 0) {
        dlgbt.push({
            name: '弃货',
            callback: function () {
                DealRefund(data.RefundId, 5, $('#txtRefundRemark').val());
            }
        });
    }

    dlgbt.push({
        name: '同意',
        callback: function () {
            DealRefund(data.RefundId, 2, $('#txtRefundRemark').val());
        },
        focus: true
    });
    dlgbt.push({
        name: '关闭'
    });

    $.dialog({
        title: '退货退款审核',
        lock: true,
        id: 'handlingComplain',
        width: '400px',
        content: dlgcontent.join(''),
        padding: '20px 10px',
        init: function () { $("#txtRefundRemark").focus(); },
        button: dlgbt
    });
}

function ShowRefundInfo(refundId) {
    var dobj = $("#rowdata-" + refundId);
    var data = jQuery.parseJSON(dobj.val());
    var jettisonRadio = "";

    dlgcontent = ['<div class="dialog-form">',
            '<div class="form-group">',
                '<label class="label-inline fl">商品名称</label>',
                '<p class="only-text">' + data.ProductName + '</p>',
            '</div>'];
    dlgcontent = dlgcontent.concat(['<div class="form-group">',
                '<label class="label-inline fl">退款金额</label>',
                '<p class="only-text"><span class="cor-red">￥' + data.Amount + '</span>（实付：' + data.SalePrice + '）</p>',
            '</div>']);
    if (data.RefundMode != 1) {
        if (data.ReturnQuantity > 0) {
            dlgcontent = dlgcontent.concat(['<div class="form-group">',
                        '<label class="label-inline fl">退货数量</label>',
                        '<p class="only-text"><span class="cor-red">' + data.ReturnQuantity + "</span>（购买：" + data.Quantity + "）" + '</p>',
                    '</div>']);
        }
    } else {
        data.ReturnQuantity = 0;
    }
    dlgcontent = dlgcontent.concat([
            '<div class="form-group">',
                '<label class="label-inline fl">原因</label>',
                '<p class="only-text">' + data.Reason.replace(/>/g, '&gt;').replace(/</g, '&lt;') + '</p>',
            '</div>',
            '<div class="form-group">',
                '<label class="label-inline fl">联系人</label>',
                '<p class="only-text">' + data.ContactPerson + "（" + data.ContactCellPhone + "）" + '</p>',
            '</div>',
            '<div class="form-group">',
                '<label class="label-inline fl">期望退款方式</label>',
                '<p class="only-text">' + data.RefundPayType + '</p>',
            '</div>']);
    if (data.SellerRemark) {
                    dlgcontent = dlgcontent.concat([
                            '<div class="form-group">',
                                '<label class="label-inline fl">商家备注</label>',
                                '<p class="help-top">' + data.SellerRemark.replace(/>/g, '&gt;').replace(/</g, '&lt;') + '</p>',
                           ' </div>']);
                }
    if (data.ManagerRemark) {
        dlgcontent = dlgcontent.concat(['<div class="form-group">',
                                '<label class="label-inline fl">平台备注</label>',
                                '<p class="only-text">' + data.ManagerRemark.replace(/>/g, '&gt;').replace(/</g, '&lt;') + '</p>',
                            '</div>']);
    }
    dlgcontent = dlgcontent.concat(['<div class="form-group">',
                '<label class="label-inline fl">当前状态</label>',
                '<p class="only-text"><span class="cor-red">' + data.RefundStatus + '</span></p>',
            '</div>',
            '</div>']);

    var dlgbt = [{
        name: '关闭'
    }];

    $.dialog({
        title: '查看退款申请',
        lock: true,
        id: 'handlingComplain',
        width: '400px',
        content: dlgcontent.join(''),
        padding: '20px 10px',
        init: function () { $("#txtRefundRemark").focus(); },
        button: dlgbt
    });
}

function OpenConfirmGood(refundId, expressCompanyName, shipOrderNumber) {
    $.dialog({
        title: '确认收货',
        lock: true,
        id: 'goodCheck',
        content: ['<div class="dialog-form">',
            '<div class="form-group">',
                 '<p class="help-top">物流公司：' + expressCompanyName + '</p>',
                 '<p class="help-top">物流单号：' + shipOrderNumber + '</p>',
                '<p class="help-top">确认已经收到订单的退货了吗？</p>',
            '</div>',
        '</div>'].join(''),
        padding: '10px',
        button: [
        {
            name: '确认收货',
            callback: function () {
                ConfirmGood(refundId);
            },
            focus: true
        }]
    });
}

function DealRefund(refundId, auditStatus, sellerRemark) {
    var loading = showLoading();
    $.post('./DealRefund', { refundId: refundId, auditStatus: auditStatus, sellerRemark: sellerRemark }, function (result) {
        loading.close();
        if (result.success) {
            $.dialog.succeedTips("操作成功！");
            var pageNo = $("#list").hiMallDatagrid('options').pageNumber;
            $("#list").hiMallDatagrid('reload', { pageNumber: pageNo });
        }
        else
            $.dialog.errorTips("操作失败");
    });
}

function ConfirmGood(refundId) {
    var loading = showLoading();
    $.post('./ConfirmRefundGood', { refundId: refundId }, function (result) {
        loading.close();
        if (result.success) {
            $.dialog.succeedTips("操作成功！");
            var pageNo = $("#list").hiMallDatagrid('options').pageNumber;
            $("#list").hiMallDatagrid('reload', { pageNumber: pageNo });
        }
        else
            $.dialog.errorTips("操作失败");
    });
}
