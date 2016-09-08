
$(function () {
    var status = GetQueryString("status");
    var li = $("li[value='" + status + "']");
    if (li.length > 0) {
        typeChoose('5')
    } else {
        typeChoose('')
    }

    //订单表格

    function typeChoose(val) {
        $('.nav li').each(function () {
            if ($(this).val() == val) {
                $(this).addClass('active').siblings().removeClass('active');
            }
        });
        //组合显示字段
            try {
                showtype = parseInt(showtype, 10);
            } catch (ex) {
                showtype = 0;
            }
        datacols = [[
                { field: "OrderId", title: '订单号', width: 100 },
                    { field: "ShopName", title: "店铺", width: 120, align: "left" },
                    {
                        field: "ProductName", title: "商品", width: 280, align: "left",
                        formatter: function (value, row, index) {
                            var html = ""
                            if (row.RefundMode == 1) {
                                html = "订单所有商品";
                            } else {
                                html = '<img style="margin-left:15px;" width="40" height="40" src="' + row.ThumbnailsUrl + '"/>' + '<span class="overflow-ellipsis" style="width:200px"><a title="' + value + '" href="/product/detail/' + row.ProductId + '" target="_blank">' + value + '</a></span>';
                            }
                            return html;
                        }
                    },
                    { field: "UserName", title: "买家", width: 80, align: "left" },
                    { field: "ApplyDate", title: "申请日期", width: 70, align: "left" },
                    {
                        field: "Amount", title: "退款", width: 90, align: "left",
                        formatter: function (value, row, index) {
                            var html = "<span class='ftx-04'>" + '￥' + value + "</span>";
                            return html;
                        }
                    }]];
        switch (showtype) {
            case 0:
            case 3:
                datacols[0].push({ field: "ReturnQuantity", title: "退货", width: 50, align: "left" });
                break;
        }

        datacols[0] = datacols[0].concat([
            { field: "AuditStatus", title: "处理状态", width: 100, align: "left" },
            {
                field: "operation", operation: true, title: "操作", width: 100,
                formatter: function (value, row, index) {
                    var html = ["<span class=\"btn-a\">"];
                    html.push("<input type=\"hidden\" name=\"rowdata\" id=\"rowdata-" + row.RefundId + "\" value='" + jQuery.toJSON(row) + "'>");
                    if (row.AuditStatus == "Wait Audit" || row.AuditStatus == "Wait Confirm") {
                        html.push("<a class=\"good-check\" onclick=\"OpenConfirmRefund('" + row.RefundId + "')\">确认退款</a>");
                    } else {
                        html.push("<a class=\"good-check\" onclick=\"OpenRefundReason('" + row.RefundId + "')\">查看原因</a>");
                    }
                    html.push("</span>");
                    return html.join("");
                }
            }
        ]);

        $("#list").hiMallDatagrid({            
            url: './list?showtype=' + showtype,
            nowrap: true,
            rownumbers: true,
            NoDataMsg: '没有找到符合条件的数据',
            border: false,
            fit: true,
            fitColumns: true,
            pagination: true,
            idField: "RefundId",
            pageSize: 15,
            pagePosition: 'bottom',
            pageNumber: 1,
            queryParams: { auditStatus: val },
            columns: datacols
        });
    }

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


    $('.nav li').click(function (e) {
        searchClose(e);
        $(this).addClass('active').siblings().removeClass('active');
        if ($(this).attr('type') == 'statusTab') {//状态分类
            $('#txtOrderId').val('');
            $('#txtShopName').val('');
            $('#txtUserName').val('');
            $("#txtProducdName").val('');
            $("#list").hiMallDatagrid('reload', { auditStatus: $(this).attr('value') || null });
        }
    });
});

function OpenConfirmRefund(refundId) {
    var dobj = $("#rowdata-" + refundId);
    var data = jQuery.parseJSON(dobj.val());
    $.dialog({
        title: '确认退款',
		width:500,
        lock: true,
        id: 'goodCheck',
        content: ['<div class="dialog-form">',
            '<div class="form-group">',
                '<label class="label-inline fl">原因</label>',
                '<p class="help-top">' + data.Reason.replace(/>/g, '&gt;').replace(/</g, '&lt;') + '</p>',
            '</div>',
             '<div class="form-group">',
                '<label class="label-inline fl">联系人</label>',
                '<p class="help-top">' + data.ContactPerson.replace(/>/g, '&gt;').replace(/</g, '&lt;') + '</p>',
           ' </div>',
            '<div class="form-group">',
                '<label class="label-inline fl">联系方式</label>',
                '<p class="help-top">' + data.ContactCellPhone + '</p>',
           ' </div>',
            '<div class="form-group">',
                '<label class="label-inline fl">退款金额</label>',
                '<p class="only-text"><span class="cor-red">' + data.Amount + '</span></p>',
           ' </div>',
            '<div class="form-group">',
                '<label class="label-inline fl">退款方式</label>',
                '<p class="help-top">' + data.RefundPayType + '</p>',
           ' </div>',

		   '<div class="form-group">',
		   		'<label class="label-inline fl">退款备注</label>',
                '<textarea id="txtRefundRemark" class="form-control" cols="40" rows="2"></textarea>\
                <span class="field-validation-error" id="orderRefundCotentTip"></span> ',
            '</div>',
        '</div>'].join(''),
        padding: '10px',
        init: function () { $("#txtRefundRemark").focus(); },
        button: [
        {
            name: '确认退款',
            callback: function () {
                var replycontent = $("#txtRefundRemark").val();
                if (replycontent.length > 200) {
                    $("#orderRefundCotentTip").text("回复内容在200个字符以内");
                    $("#txtRefundRemark").css({ border: '1px solid #f60' });
                    return false;
                }
                ConfirmRefund(refundId, $('#txtRefundRemark').val());
            },
            focus: true
        }]
    });
}

function OpenRefundReason(refundId) {
    var dobj = $("#rowdata-" + refundId);
    var data = jQuery.parseJSON(dobj.val());

    dlgcontent = ['<div class="dialog-form">',
            '<div class="form-group">',
                '<label class="label-inline fl">原因</label>',
                '<p class="help-top">' + data.Reason.replace(/>/g, '&gt;').replace(/</g, '&lt;') + '</p>',
            '</div>'];

    dlgcontent = dlgcontent.concat(['<div class="form-group">',
                '<label class="label-inline fl">联系人</label>',
                '<p class="help-top">' + data.ContactPerson.replace(/>/g, '&gt;').replace(/</g, '&lt;') + '</p>',
           ' </div>',
            '<div class="form-group">',
                '<label class="label-inline fl">联系方式</label>',
                '<p class="help-top">' + data.ContactCellPhone + '</p>',
           ' </div>',
            '<div class="form-group">',
                '<label class="label-inline fl">退款金额</label>',
                '<p class="only-text"><span class="cor-red">' + data.Amount + '</span></p>',
           ' </div>',
            '<div class="form-group">',
                '<label class="label-inline fl">退款方式</label>',
                '<p class="help-top">' + data.RefundPayType + '</p>',
           ' </div>']);
    if (data.SellerRemark) {
        dlgcontent = dlgcontent.concat([
                '<div class="form-group">',
                    '<label class="label-inline fl">商家处理</label>',
                    '<p class="help-top">' + data.SellerRemark.replace(/>/g, '&gt;').replace(/</g, '&lt;') + '</p>',
               ' </div>']);
    }
    if (data.ManagerRemark) {
        dlgcontent = dlgcontent.concat([
                '<div class="form-group">',
                    '<label class="label-inline fl">平台备注</label>',
                    '<p class="help-top">' + data.ManagerRemark.replace(/>/g, '&gt;').replace(/</g, '&lt;') + '</p>',
               ' </div>']);
    }
    dlgcontent = dlgcontent.concat(['</div>']);

    $.dialog({
        title: '查看原因',
        lock: true,
        id: 'goodCheck',
        width: '400px',
        content: dlgcontent.join(''),
        padding: '20px 10px'
    });
}


function ConfirmRefund(refundId, managerRemark) {
    var loading = showLoading();
    $.post('./ConfirmRefund', { refundId: refundId, managerRemark: managerRemark }, function (result) {
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