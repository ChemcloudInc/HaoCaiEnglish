
$(function () {
    $(".start_datetime").datetimepicker({
        language: 'zh-CN',
        format: 'yyyy-mm-dd',
        autoclose: true,
        weekStart: 1,
        minView: 2
    });
    $(".end_datetime").datetimepicker({
        language: 'zh-CN',
        format: 'yyyy-mm-dd',
        autoclose: true,
        weekStart: 1,
        minView: 2
    });
    $(".start_datetime").click(function () {
        $('.end_datetime').datetimepicker('show');
    });
    $(".end_datetime").click(function () {
        $('.start_datetime').datetimepicker('show');
    });

    $('.start_datetime').on('changeDate', function () {
        if ($(".end_datetime").val()) {
            if ($(".start_datetime").val() > $(".end_datetime").val()) {
                $('.end_datetime').val($(".start_datetime").val());
            }
        }

        $('.end_datetime').datetimepicker('setStartDate', $(".start_datetime").val());
    });
    searchOrder();


});

function search() {
    var enumOrderTypeId = parseInt($("#selEnumOrderType").val());
    switch (enumOrderTypeId) {
        case 1:
            $("#ListAgreement").hide();
            $("#ListReturnOrder").hide();
            $("#ListOrder").show();
            searchOrder();
            break;
        case 0:
            $("#ListAgreement").hide();
            $("#ListOrder").hide();
            $("#ListReturnOrder").show();
            searchReturnOrder();
            break;
        case 2:
            $("#ListReturnOrder").hide();
            $("#ListOrder").hide();
            $("#ListAgreement").show();
            searchPurchaseAgreement();
            break;
    }
}

function searchOrder() {
    $("#ListOrder").hiMallDatagrid({
        url: '../DetailList',
        nowrap: false,
        rownumbers: true,
        NoDataMsg: 'No matching data',
        border: false,
        fit: true,
        fitColumns: true,
        pagination: true,
        idField: "Id",
        pageSize: 15,
        pagePosition: 'bottom',
        pageNumber: 1,
        queryParams: { accountId: accountId, startDate: $("#inputStartDate").val(), endDate: $("#inputEndDate").val(), enumOrderTypeId: $("#selEnumOrderType").val() },
        columns:
        [[
            { field: "OrderTypeDescription", title: "Type", width: 120, align: "center" },
            {
                field: "OrderId", title: "Order no.", width: 120, align: "center",
                formatter: function (value, row, index) {
                    return "<a href='/SellerAdmin/Order/Detail/" + value + "'>" + value + "</a>"
                }
            },
            { field: "ProductActualPaidAmount", title: "Actual paid", width: 80, align: "center" },
            { field: "FreightAmount", title: "Freight", width: 80, align: "center" },
            { field: "CommissionAmount", title: "Commission", width: 80, align: "center" },
            { field: "OrderDate", title: "Order date", width: 180, align: "center" },
            { field: "Date", title: "Date", width: 180, align: "center" }
        ]]
    });
}

function searchReturnOrder() {

    $("#ListReturnOrder").hiMallDatagrid({
        url: '../DetailList',
        nowrap: false,
        rownumbers: true,
        NoDataMsg: 'No matching data',
        border: false,
        fit: true,
        fitColumns: true,
        pagination: true,
        idField: "Id",
        pageSize: 15,
        pagePosition: 'bottom',
        pageNumber: 1,
        queryParams: { accountId: accountId, startDate: $("#inputStartDate").val(), endDate: $("#inputEndDate").val(), enumOrderTypeId: $("#selEnumOrderType").val() },
        columns:
        [[
            { field: "OrderTypeDescription", title: "Type", width: 120, align: "center" },
            {
                field: "OrderId", title: "Order no.", width: 120, align: "center",
                formatter: function (value, row, index) {
                    return "<a href='/SellerAdmin/Order/Detail/" + value + "'>" + value + "</a>"
                }
            },
            { field: "ProductActualPaidAmount", title: "Actual paid", width: 80, align: "center" },
            { field: "FreightAmount", title: "Freight", width: 80, align: "center" },
            { field: "RefundTotalAmount", title: "Refund total amount", width: 80, align: "center" },
            { field: "RefundCommisAmount", title: "Refund commis amount", width: 80, align: "center" },
            { field: "OrderRefundsDates", title: "Refund date", width: 180, align: "center" }
        ]]
    });
}

function searchPurchaseAgreement() {
    $("#ListAgreement").hiMallDatagrid({
        url: '../MetaDetailList',
        nowrap: false,
        rownumbers: true,
        NoDataMsg: 'No matching data',
        border: false,
        fit: true,
        fitColumns: true,
        pagination: true,
        idField: "Id",
        pageSize: 15,
        pagePosition: 'bottom',
        pageNumber: 1,
        queryParams: { accountId: accountId, startDate: $("#inputStartDate").val(), endDate: $("#inputEndDate").val(), enumOrderTypeId: $("#selEnumOrderType").val() },
        columns:
        [[
            {
                field: "OrderTypeDescription", title: "Type", width: 120, align: "center",
                formatter: function (value, row, index) {
                    return 'Marketing services fee';
                }
            },
            { field: "MetaKey", title: "Marketing Type", width: 120, align: "center" },
            { field: "MetaValue", title: "Fee", width: 120, align: "center" },
            {
                field: "DateRange", title: "Service period", width: 120, align: "center"
            }
        ]]
    });
}
