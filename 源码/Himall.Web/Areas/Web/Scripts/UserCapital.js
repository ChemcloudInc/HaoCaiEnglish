$(function () {
    typeChoose(0);
    $('#ulstatus li').click(function (e) {
        typeChoose($(this).val());
    });
})

    function typeChoose(val) {
        $('#ulstatus li').each(function () {
            var _t = $(this);
            if (_t.val() == val) {
                _t.addClass('active').siblings().removeClass('active');
            }
        });
        var dataColumn = [];
        if (val == 0)
        {
            dataColumn.push({ field: "CreateTime", title: 'Time', width: 120 });
            dataColumn.push({
                field: "Amount", title: 'Income', width: 100, align: 'center',
                formatter: function (value, row, index) {
                    var html = '';
                    if (parseFloat(value) > 0)
                        html = value;
                    return html;
                }
            });
            dataColumn.push({
                field: "Amount1", title: 'Expenditure', width: 100, align: 'center',
                formatter: function (value, row, index) {
                    var html = '';
                    if (parseFloat(row['Amount']) < 0)
                        html = row['Amount'];
                    return html;
                }
            });
            dataColumn.push({
                field: "Remark", title: "Remarks", width: 200, align: "left",
            });
        }
        var url = '/UserCapital/List';
        switch(val)
        {
            case 1:
                dataColumn.push({
                    field: "CreateTime", title: 'Get Date', width: 120});
                dataColumn.push({
                    field: "Amount", title: 'Amount', width: 100, align: 'center',
                });
                break;
            case 2:
                url = '/UserCapital/ChargeList';
                dataColumn.push({ field: "CreateTime", title: 'Recharge Date', width: 120 });
                dataColumn.push({
                    field: "ChargeAmount", title: 'Amount', width: 100, align: 'center',
                });
                dataColumn.push({ field: "ChargeWay", title: 'RechargeType', width: 80 });
                dataColumn.push({ field: "ChargeStatusDesc", title: 'Status', width: 80 });
                dataColumn.push({ field: "Id", title: 'RechargeID', width: 120 });
                dataColumn.push({
                    field: "operate", title: 'operate', width: 80, formatter: function (value, row, index) {
                        var html = [];
                        if (row['ChargeStatus'] == 1) {
                            html.push("<span class=\"btn-a\">");
                            html.push("<a onclick='DoOperate(\"" + row["Id"] + "\")'>Pay</a>");
                            html.push("</span>");
                        }
                        return html.join('');
                    }
                });
                break;
            case 3:
                url = '/UserCapital/ApplyWithDrawList';
                dataColumn.push({ field: "ApplyTime", title: 'WithDraw Date', width: 120 });
                dataColumn.push({
                    field: "ApplyAmount", title: 'Amount', width: 100, align: 'center',
                });
                dataColumn.push({
                    field: "ApplyStatusDesc", title: 'Status', width: 80
                });
                dataColumn.push({ field: "Id", title: 'ID', width: 120 });
                break;
            case 4:
                dataColumn.push({ field: "CreateTime", title: 'Consumption time', width: 120 });
                dataColumn.push({
                    field: "Amount", title: 'Amount', width: 100, align: 'center',
                });
                dataColumn.push({ field: "Id", title: 'ID', width: 120 });
                break;
            case 5:
                dataColumn.push({ field: "CreateTime", title: 'Refund Date', width: 120 });
                dataColumn.push({
                    field: "Amount", title: 'Amount', width: 100, align: 'center',
                });
                dataColumn.push({ field: "Id", title: 'ID', width: 120 });
                break;
        }
        $("#list").empty();
        $("#list").hiMallDatagrid({
            url: url,
            nowrap: false,
            rownumbers: true,
            NoDataMsg: 'No find qualified data',
            border: false,
            fit: true,
            fitColumns: true,
            pagination: true,
            idField: "id",
            pageSize: 15,
            pagePosition: 'bottom',
            pageNumber: 1,
            queryParams: { capitalType: val },
            columns: [dataColumn],
        });
    }
    function DoOperate(ids)
    {
        window.top.open("/Order/ChargePay?orderIds=" + ids, "_self");
    }