
$(function () {
    $(".nav li").each(function () {
        $(this).click(function () {
            $(this).parent().children(".active").removeClass();
            $(this).addClass("active");
        });
    });
    Query(0);
});


function Query(status) {
    $('.nav li').each(function () {
        if ($(this).val() == status) {
            $(this).addClass('active').siblings().removeClass('active');
        }
    });

    InitData(status)

    //$('.nav li').click(function (e) {
    //    searchClose(e);
    //    $(this).addClass('active').siblings().removeClass('active');
    //    if ($(this).attr('type') == 'statusTab') {//状态分类
    //        $("#list").hiMallDatagrid('reload', { status: $(this).attr('value') || null });
    //    }
    //});
}


function InitData(status) {
    //订单表格
    $("#list").hiMallDatagrid({
        url: './list',
        nowrap: true,
        rownumbers: true,
        NoDataMsg: '没有找到符合条件的数据',
        border: true,
        fit: true,
        fitColumns: true,
        pagination: true,
        idField: "Id",
        pageSize: 15,
        pagePosition: 'bottom',
        pageNumber: 1,
        queryParams: { status: status },
        columns:
        [[
            { field: "ShopName", title: "Shop name", width: 120, align: "center" },
            { field: "TimeSlot", title: "Time slot", width: 120, align: "left" },
            { field: "ProductActualPaidAmount", title: "Actual paid amount", width: 120, align: "center" },
            { field: "FreightAmount", title: "Freight amount", width: 60, align: "center" },
            { field: "CommissionAmount", title: "Commission amount", width: 80, align: "center" },
            { field: "RefundAmount", title: "Refund amount", width: 80, align: "center" },
            { field: "RefundCommissionAmount", title: "Refund commission amount", width: 80, align: "center" },
            { field: "AdvancePaymentAmount", title: "Advance payment amount", width: 80, align: "center" },
            { field: "PeriodSettlement", title: "Perid settlement", width: 80, align: "center" },
            { field: "AccountDate", title: "Account date", width: 100, align: "center" },
            {
                field: "operation", width: 150, operation: true, title: "Operation",
                formatter: function (value, row, index) {
                    var html = ["<span class=\"btn-a\">"];
                    html.push("<a href='./AccountDetail/" + row.Id + "'>detail</a>");
                    html.push("</span>");
                    return html.join("");
                }
            }
        ]]
    });

}

