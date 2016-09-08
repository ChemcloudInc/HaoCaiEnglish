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
                    case "Wait Audit":
                        html.push("<a class=\"good-check\" onclick=\"OpenDealRefund('" + row.RefundId + "')\">after sale</a>");
                        break;
                    case "Wait Receiving":
                        html.push("<a class=\"good-check\" onclick=\"OpenConfirmGood('" + row.RefundId + "','" + row.ExpressCompanyName + "','" + row.ShipOrderNumber + "')\">Confirm receving</a>");
                        break;
                    default:
                        html.push("<a class=\"good-check\" onclick=\"ShowRefundInfo('" + row.RefundId + "')\">view</a>");
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
                '<label class="label-inline fl">Product name</label>',
                '<p class="only-text">' + data.ProductName + '</p>',
            '</div>'];
    dlgcontent = dlgcontent.concat(['<div class="form-group">',
                '<label class="label-inline fl">Amount</label>',
                '<p class="only-text"><span class="cor-red">$' + data.Amount + '</span>（Actual paid ' + data.SalePrice + '）</p>',
            '</div>']);
    if (data.RefundMode != 1) {
        if (data.ReturnQuantity > 0) {
            dlgcontent = dlgcontent.concat(['<div class="form-group">',
                        '<label class="label-inline fl">Quantity</label>',
                        '<p class="only-text"><span class="cor-red">' + data.ReturnQuantity + "</span>（Buy:" + data.Quantity + "）" + '</p>',
                    '</div>']);
        }
    } else {
        data.ReturnQuantity = 0;
    }
    dlgcontent = dlgcontent.concat([
            '<div class="form-group">',
                '<label class="label-inline fl">Reasons</label>',
                '<p class="only-text">' + data.Reason.replace(/>/g, '&gt;').replace(/</g, '&lt;') + '</p>',
            '</div>',
            '<div class="form-group">',
                '<label class="label-inline fl">Contact</label>',
                '<p class="only-text">' + data.ContactPerson + "（" + data.ContactCellPhone + "）" + '</p>',
            '</div>',
            '<div class="form-group">',
                '<label class="label-inline fl">Pay type</label>',
                '<p class="only-text">' + data.RefundPayType + '</p>',
            '</div>',
            '<div class="form-group">',
                '<textarea class="form-control" cols="56" rows="3" id="txtRefundRemark" placeholder="Reply to buyer"></textarea>',
            '</div>',
        '</div>']);

    var dlgbt = [{
        name: 'Refuse',
        callback: function () {
            sellerRemark = $('#txtRefundRemark').val();
            if (sellerRemark.length < 1) {
                alert("Please enter reasons to refuse.");
                return false;
            }
            DealRefund(data.RefundId, 4, sellerRemark);
        }
    }];
    if (data.ReturnQuantity > 0) {
        dlgbt.push({
            name: 'Give up goods',
            callback: function () {
                DealRefund(data.RefundId, 5, $('#txtRefundRemark').val());
            }
        });
    }

    dlgbt.push({
        name: 'Agree',
        callback: function () {
            DealRefund(data.RefundId, 2, $('#txtRefundRemark').val());
        },
        focus: true
    });
    dlgbt.push({
        name: 'Close'
    });

    $.dialog({
        title: 'Refund Audit',
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
                '<label class="label-inline fl">Product name</label>',
                '<p class="only-text">' + data.ProductName + '</p>',
            '</div>'];
    dlgcontent = dlgcontent.concat(['<div class="form-group">',
                '<label class="label-inline fl">Refund amount</label>',
                '<p class="only-text"><span class="cor-red">$' + data.Amount + '</span>（Actual paid:' + data.SalePrice + '）</p>',
            '</div>']);
    if (data.RefundMode != 1) {
        if (data.ReturnQuantity > 0) {
            dlgcontent = dlgcontent.concat(['<div class="form-group">',
                        '<label class="label-inline fl">Quantity</label>',
                        '<p class="only-text"><span class="cor-red">' + data.ReturnQuantity + "</span>（Buy:" + data.Quantity + "）" + '</p>',
                    '</div>']);
        }
    } else {
        data.ReturnQuantity = 0;
    }
    dlgcontent = dlgcontent.concat([
            '<div class="form-group">',
                '<label class="label-inline fl">Reasons</label>',
                '<p class="only-text">' + data.Reason.replace(/>/g, '&gt;').replace(/</g, '&lt;') + '</p>',
            '</div>',
            '<div class="form-group">',
                '<label class="label-inline fl">Contact</label>',
                '<p class="only-text">' + data.ContactPerson + "（" + data.ContactCellPhone + "）" + '</p>',
            '</div>',
            '<div class="form-group">',
                '<label class="label-inline fl">Pay type</label>',
                '<p class="only-text">' + data.RefundPayType + '</p>',
            '</div>']);
    if (data.SellerRemark) {
                    dlgcontent = dlgcontent.concat([
                            '<div class="form-group">',
                                '<label class="label-inline fl">Seller remarks</label>',
                                '<p class="help-top">' + data.SellerRemark.replace(/>/g, '&gt;').replace(/</g, '&lt;') + '</p>',
                           ' </div>']);
                }
    if (data.ManagerRemark) {
        dlgcontent = dlgcontent.concat(['<div class="form-group">',
                                '<label class="label-inline fl">Platform remarks</label>',
                                '<p class="only-text">' + data.ManagerRemark.replace(/>/g, '&gt;').replace(/</g, '&lt;') + '</p>',
                            '</div>']);
    }
    dlgcontent = dlgcontent.concat(['<div class="form-group">',
                '<label class="label-inline fl">Current status</label>',
                '<p class="only-text"><span class="cor-red">' + data.RefundStatus + '</span></p>',
            '</div>',
            '</div>']);

    var dlgbt = [{
        name: 'Close'
    }];

    $.dialog({
        title: 'View refund application',
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
        title: 'Confirm receipt',
        lock: true,
        id: 'goodCheck',
        content: ['<div class="dialog-form">',
            '<div class="form-group">',
                 '<p class="help-top">Logistics company:' + expressCompanyName + '</p>',
                 '<p class="help-top">Shipment number:' + shipOrderNumber + '</p>',
                '<p class="help-top">Return order confirmation has been received yet?</p>',
            '</div>',
        '</div>'].join(''),
        padding: '10px',
        button: [
        {
            name: 'Confirm receipt',
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
            $.dialog.succeedTips("Successful operation!");
            var pageNo = $("#list").hiMallDatagrid('options').pageNumber;
            $("#list").hiMallDatagrid('reload', { pageNumber: pageNo });
        }
        else
            $.dialog.errorTips("Operation failed!");
    });
}

function ConfirmGood(refundId) {
    var loading = showLoading();
    $.post('./ConfirmRefundGood', { refundId: refundId }, function (result) {
        loading.close();
        if (result.success) {
            $.dialog.succeedTips("Successful operation!");
            var pageNo = $("#list").hiMallDatagrid('options').pageNumber;
            $("#list").hiMallDatagrid('reload', { pageNumber: pageNo });
        }
        else
            $.dialog.errorTips("Operation failed!");
    });
}
