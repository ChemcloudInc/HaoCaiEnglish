$(function () {
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
            { field: "Id", title: 'Id', hidden: true, width: 150 },
            { field: "ProductId", title: 'ProductId', hidden: true, width: 150 },
            {
                field: "ProductName", title: "商品名称", width: 220, align: "left",
                formatter: function (value, row, index) {
                    var html = '<span class="overflow-ellipsis" style="width:200px"><a title="' + value + '" href="/product/detail/' + row.ProductId + '" target="_blank">' + value + '</a></span>';
                    return html;
                }
            },
           { field: "Status", title: "审核状态", width: 80, align: "center" },
            { field: "ShopName", title: "店铺", width: 200, align: "center" },
            { field: "Price", title: "限时购价", width: 80, align: "left" },
            { field: "RecentMonthPrice", title: "近30天均价", width: 80, align: "left" },
            { field: "StartDate", title: "开始时间", width: 90, align: "left" },
            { field: "EndDate", title: "结束时间", width: 90, align: "left" },
            { field: "SaleCount", title: "购买数", width: 50, align: "center" },
        {
            field: "operation", operation: true, title: "操作",
            formatter: function (value, row, index) {
                var id = row.Id.toString();
                var html = ["<span class=\"btn-a\">"];
                if (row.Status == "待审核") {
                    html.push("<a class=\"good-check\" onclick=\"Audit('" + id + "')\">审核</a>");
                } else if (row.Status == "进行中") {
                    html.push("<a class=\"good-check\" onclick=\"Cancel('" + id + "')\">取消</a>");
                }
                html.push("</span>");
                return html.join("");
            }
        }
        ]]
    });

    $('#searchButton').click(function (e) {
        searchClose(e);
        var shopName = $.trim($('#txtShopName').val());
        var productName = $.trim($('#txtProductName').val());
        $("#list").hiMallDatagrid('reload',
            {
                AuditStatus: $("#AuditStatus").val(),
                shopName: shopName,
                productName: productName
            });
    });
});

function Cancel(id) {
    $.dialog.confirm('您确定要取消吗？', function () {
        var loading = showLoading();
        ajaxRequest({
            type: 'POST',
            url: "./CancelItem",
            param: { id: id },
            dataType: "json",
            success: function (data) {
                if (data.success == true) {
                    var pageNo = $("#list").hiMallDatagrid('options').pageNumber;
                    $("#list").hiMallDatagrid('reload', { pageNumber: pageNo });
                    loading.close();
                } else {
                    $.dialog.errorTips(data.msg);
                }
            }, error: function () { 
                loading.close();
            }
        });
    });
}


function Audit(id) {

    $.dialog({
        title: '限时购审核',
        lock: true,
        id: 'goodCheck',
        content: ['<div class="dialog-form">',
            '<div class="form-group">',
                '<p class="help-top">备注</p>',
                '<textarea id="auditMsgBox" class="form-control" cols="40" rows="2" onkeyup="this.value = this.value.slice(0, 50)" ></textarea>\
                 <p id="valid" style="visibility:hidden;color:red">请填写未通过理由</p> ',
            '</div>',
        '</div>'].join(''),
        padding: '10px',
        init: function () { $("#auditMsgBox").focus(); },
        button: [
        {
            name: '通过审核',
            callback: function () {
                auditProduct(id, 2, $('#auditMsgBox').val());
            },
            focus: true
        },
        {
            name: '拒绝',
            callback: function () {

                if (!$.trim($('#auditMsgBox').val())) {
                    $('#valid').css('visibility', 'visible');
                    return false;
                }
                else {
                    $('#valid').css('visibility', 'hidden');
                    auditProduct(id, 3, $('#auditMsgBox').val());
                }
            }
        }]
    });
}

function auditProduct(id, auditState, msg) {
    var loading = showLoading();
    $.post('./AuditItem', { Id: id.toString(), auditState: auditState, message: msg }, function (result) {
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


