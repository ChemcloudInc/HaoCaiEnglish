
$(function () {
    var status = GetQueryString('status');
    if (status == 1) {
        typeChoose('1');
    }
    else if (status == 2) {
        typeChoose('2');
    } else {
        typeChoose()
    }       


    function typeChoose(val) {
        
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
            pageSize: 14,
            pagePosition: 'bottom',
            pageNumber: 1,
            queryParams: { orderStatus: val },
            operationButtons: "#orderOperate",
            onLoadSuccess: CheckStatus,
            columns:
			[[
                {
                    checkbox: true, witdh: 35, formatter: function ( value, row, index )
                    {
                        if ( row.OrderStatus == "待付款" || row.OrderStatus == "已关闭" )
                        {
                            return '<input type="checkbox" disabled>';
                        }
                        return '<input type="checkbox">';
                    }
                },
                //{
                //    witdh: 35, title: "选择", formatter: function ()
                //    {
                //        var html = '<div style="text-align:center;" class="hiMallDatagrid-cell-check "><input type="checkbox"></div>';
                //        return html;
                //    }
                //},
				{
				    field: "OrderId", title: '订单号', width: 200, align: "left",
				    formatter: function (value, row, index) {
				        return '<img src="' + row.IconSrc + '" title="' + row.PlatformText + '订单" width="16" />' + value;
				    }
				},
				{ field: "UserName", title: "买家", width: 120, align: "left" },
				{ field: "OrderDate", title: "下单时间", width: 140, align: "left" },
				{
				    field: "TotalPrice", title: "订单总额", width: 120, align: "right",
				    formatter: function (value, row, index) {
				        var html = "<span class='ftx-04'>" + '￥' + value.toFixed(2) + "</span>";
				        return html;				        
				    }
				},
			//{ field: "PaymentTypeName", title: "支付方式", width: 120, align: "left" },
			{
			    field: "OrderStatus", title: "订单状态", width: 100, align: "center",
			    formatter: function (value, row, index) {
			        var html = ["<span class='ordstbox'>"];               
			        switch(row.RefundStats)
			        {
			            case null:
			            case 0:
			                break;
			            case 4:
			                break;
			            default:
			                html.push("<i class='refundico' title='有待处理退款'>有退款</i>");
                            break;
			        }
			        html.push(row.OrderStatus);
			        html.push("</span>");
			        return html.join("");
			    }
			},
			{
			    field: "operation", operation: true, title: "操作",
			    formatter: function (value, row, index) {
			        var id = row.OrderId.toString();
			        var html = ["<span class=\"btn-a\">"];
			        html.push("<a href='./Detail/" + id + "'>查看</a>");
			        if (row.OrderStatus == "待付款") {
			            html.push("<a href='./Detail/" + id + "?&updatePrice=" + true + "'>改价</a>");
			            html.push("<a class=\"good-check\" onclick=\"OpenCloseOrder('" + id + "')\">取消</a>");
			        }
			        if (row.OrderStatus == "待发货") {
			            html.push("<a href='./SendGood?ids=" + id + "'>发货</a>");
			        }
			        html.push("</span>");
			        return html.join("");
			    }
			}
			]]
        });
    }

    function CheckStatus() {
        var status = $(".nav li[class='active']").attr("value");
        if (status && status != '0' && status != '2') {
            $(".tabel-operate").html('');
            $(".td-choose").hide();
        }
        else {
            $(".td-choose").show();
        }
    }

    $('#searchButton').click(function (e) {
        searchClose(e);
        var startDate = $("#inputStartDate").val();
        var endDate = $("#inputEndDate").val();
        var orderId = $.trim($('#txtOrderId').val());
        if (isNaN(orderId)) {
            $.dialog.errorTips("请输入正确的查询订单号"); return false;
        }
        var userName = $.trim($('#txtUserName').val());
        var orderType = $("#orderType").val();
        $(".tabel-operate").find("label").remove();
        $("#list").hiMallDatagrid('reload', { startDate: startDate, endDate: endDate, orderId: orderId, userName: userName, paymentTypeGateway: $("#selectPaymentTypeName").val(), orderType: orderType });
    });


    $('.nav li').click(function (e) {
        try {
            searchClose(e);
            $(this).addClass('active').siblings().removeClass('active');
            if ($(this).attr('type') == 'statusTab') {//状态分类
                $('#txtOrderId').val('');
                $('#txtuserName').val('');
                if ($(this).attr('value') == 0 || $(this).attr('value') == 2) {
                    var html = '<a href="javascript:downloadOrderList()" class="btn btn-default btn-ssm">订单配货表</a><a href="myPreview()" class="btn btn-default btn-ssm">打印发货单</a><a href="printOrders()" class="btn btn-default btn-ssm">打印快递单</a><a href="sendGood()" class="btn btn-default btn-ssm">批量发货</a>';
                    $(".tabel-operate").html('');
                    $(".tabel-operate").append(html);
                }
                else {
                    $(".tabel-operate").children().remove();
                }
                $("#list").hiMallDatagrid('reload', { orderStatus: $(this).attr('value') || null });
            }
        }
        catch (ex) {
            alert();
        }
    });

    $("#aDownloadProductList").attr("href", "./DownloadProductList?ids=" + getSelectedIds())
});

function downloadProductList() {
    var ids = getSelectedIds();
    if (ids.length <= 0) {
        $.dialog.tips('请至少选择一个订单');
        return false;
    }

    loadIframeURL("./DownloadProductList?ids=" + ids.toString());
}

function downloadOrderList() {
    var ids = getSelectedIds();
    if (ids.length <= 0) {
        $.dialog.tips('请至少选择一个订单');
        return false;
    }

    window.open("/SellerAdmin/Order/DownloadOrderList?ids=" + ids.toString());
    //loadIframeURL("/SellerAdmin/Order/DownloadOrderList?ids=" + ids.toString());
}

function loadIframeURL(url) {
    var iFrame;
    iFrame = document.createElement("iframe");
    iFrame.setAttribute("src", url);
    iFrame.setAttribute("style", "display:none;");
    iFrame.setAttribute("height", "0px");
    iFrame.setAttribute("width", "0px");
    iFrame.setAttribute("frameborder", "0");
    document.body.appendChild(iFrame);
    // 发起请求后这个iFrame就没用了，所以把它从dom上移除掉
    //iFrame.parentNode.removeChild(iFrame);
    iFrame = null;
}

function myPreview() {
    var orderIds = getSelectedIds();
    if (orderIds.length <= 0) {
        $.dialog.tips('请至少选择一个订单');
        return false;
    }

    var LODOP = getLodop(document.getElementById('LODOP_OB'), document.getElementById('LODOP_EM'));
    var strBodyStyle = "<style>body{margin:0; padding:0;font-family: 'microsoft yahei',Helvetica;font-size:12px;color: #333;}.table-hd{ margin:0;line-height:30px; float:left; background: #f5f5f5;padding:0 10px;  margin-top:30px;}.table-hd strong{font-size:14px;font-weight:normal; float:left}.table-hd span{ font-weight:normal; font-size:12px;float:right}table{border: 1px solid #ddd;width:100%;border-collapse: collapse;border-spacing: 0; font-size:12px; float:left}table th,table td{border:1px solid #ddd;padding: 8px; text-align:center}table th{border-top:0;}</style>";
    $.post('./GetOrderPrint', { ids: orderIds.toString() }, function (result) {
        if (result.success) {
            var strFormHtml = strBodyStyle + "<body>" + result.msg + "</body>"; //这里的”divdiv1“是标签的名称。
            //LODOP.PRINT_INIT("高青公路养护");
            LODOP.SET_PRINT_PAGESIZE(1, 0, 0, "A4"); //A4纸张纵向打印
            //LODOP.SET_PRINT_STYLE("FontSize", 9);
            LODOP.ADD_PRINT_HTM("0%", "0%", "100%", "100%", strFormHtml);//四个数值分别表示Top,Left,Width,Height
            LODOP.PREVIEW(); //打印预览
            //LODOP.PRINT(); //直接打印
        }
    });    
}

function sendGood() {
    var orderIds = getSelectedIds();
    if (orderIds.length <= 0) {
        $.dialog.tips('请至少选择一个订单');
        return false;
    }

    location.href = "./SendGood?ids=" + orderIds.toString();
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

function CloseOrder(orderId) {
    var loading = showLoading();
    $.post('./CloseOrder', { orderId: orderId }, function (result) {
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

function getSelectedIds() {
    var selecteds = $("#list").hiMallDatagrid('getSelections');
    var ids = [];
    $.each(selecteds, function () {
        ids.push(this.OrderId);
    });
    return ids;
}

function printOrders() {
    var ids = getSelectedIds();
    if (ids.length == 0)
        $.dialog.tips('请至少选择一个订单');
    else
        location.href = "print?orderIds=" + ids.toString();
}

