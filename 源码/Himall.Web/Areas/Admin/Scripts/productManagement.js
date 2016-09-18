/// <reference path="../../../Scripts/jqeury.himallLinkage.js" />
var categoryId;
$(function () {


    $('#category1,#category2,#category3').himallLinkage({
        url: '../category/getCategory',
        enableDefaultItem: true,
        defaultItemsText: '全部',
        onChange: function (level, value, text) {
            categoryId = value;
        }
    });
    var status = GetQueryString("status");
    var li = $("li[value='" + status + "']");
    if (li.length > 0) {
        typeChoose(status)
    } else {
        typeChoose('')
    }

    function typeChoose(val) {
        $('.nav li').each(function () {
            var _t = $(this);
            if (_t.val() == val) {
                _t.addClass('active').siblings().removeClass('active');
            }
        });

        $("#list").hiMallDatagrid({
            url: './list',
            nowrap: false,
            rownumbers: true,
            NoDataMsg: '没有找到符合条件的数据',
            border: false,
            fit: true,
            fitColumns: true,
            pagination: true,
            idField: "id",
            pageSize: 10,
            pagePosition: 'bottom',
            pageNumber: 1,
            queryParams: { auditStatus: val },
            //operationButtons: (parseInt(val) == "" ? "#saleOff" : null),
            operationButtons: "#saleOff",
            columns:
			[[
				{
				    checkbox: true, width: 39,
				},
				{ field: "productCode", title: '商品货号', width: 50 },
				 { field: "shopName", title: "店铺", align: "left", width: 100 },
				{
				    field: "name", title: '商品', width: 280, align: 'left',
				    formatter: function (value, row, index) {
				        var html = '<img style="margin-left:15px;" width="40" height="40" src="' + row.imgUrl + '" /><span class="overflow-ellipsis" style="width:200px"><a title="' + value + '" href="/product/detail/' + row.id + '" target="_blank" href="' + row.url + '">' + value + '</a></span>';
				        return html;
				    }
				},
			{ field: "categoryName", title: "分类", width: 85, align: "left" },
			{ field: "brandName", title: "品牌", width: 55, align: "left" },
			{
			    field: "price", title: "价格", width: 70, align: "left",
			    formatter: function (value, row, index) {
			        return '$' + value.toFixed(2);
			    }
			},
			{ field: "state", title: "状态", width: 80, align: "left" },
            {
                field: "v", title: "预览", align: "center",
                formatter: function (value, row, index) {
                    var html = '<a class="glyphicon glyphicon-eye-open view-mobile-product" title="预览微店" data-url="/m/product/detail/' + row.id + '?sv=True" style="font-size:18px;text-decoration: none; cursor:pointer;"></a>';
                    return html;
                },
            },
			{
			    field: "s", title: "操作", width: 90, align: "center",
			    formatter: function (value, row, index) {
			        var html = "";
			        html += '<span class="btn-a">';
			        if (row.auditStatus == 1 && row.saleStatus == 1)//仅未审核的商品需要审核
			            html += '<a class="good-check" onclick="audit(' + row.id + ')">审核</a>';
			        else if (row.auditStatus == 2)//
			            html += '<a class="good-break" onclick="infractionSaleOffDialog(' + row.id + ')">违规下架</a>';
			        else if (row.auditStatus >= 3) {
			            html += '<a class="good-break" onclick="$.dialog.tips(\'' + row.auditReason.replace(/\'/g, "’").replace(/\"/g, "“") + '\');">查看原因</a>';
			        }
			        html += '</span>';
			        return html;
			    }
			}
			]],
            onLoadSuccess: function () {
                OprBtnShow(val);
            }
        });
    }

    function OprBtnShow(val) {
        if (val == 2) {
            $(".check-all").parent().show();
            $(".td-choose").show();
            $("#saleOff").show();
        }
        else if (val != "" && val != null) {
            $(".check-all").parent().hide();
            $(".td-choose").hide();
            $("#saleOff").hide();
        }
        else {
            $(".check-all").parent().show();
            $(".td-choose").show();
            $("#saleOff").show();
        }
        if (val == 4) {
            $("#infractionSaleOffBtn").hide();
        }
        if (val == 1) {
            $("#infractionSaleOffBtn").hide();
        }
    }



    //autocomplete
    $('#brandBox').autocomplete({
        source: function (query, process) {
            var matchCount = this.options.items;//返回结果集最大数量
            $.post("../brand/getBrands", { "keyWords": $('#brandBox').val() }, function (respData) {
                return process(respData);
            });
        },
        formatItem: function (item) {
            if (item.envalue != null) {
                return item.value + "(" + item.envalue + ")";
            }
            return item.value;
        },
        setValue: function (item) {
            return { 'data-value': item.value, 'real-value': item.key };
        }
    });

    $('#searchButton').click(function (e) {
        search();
        searchClose(e);
    })


    $('.nav li').click(function (e) {
        searchClose(e);
        $(this).addClass('active').siblings().removeClass('active');
        if ($(this).attr('type') == 'statusTab') {//状态分类
            $('#brandBox').val('');
            $('#searchBox').val('');
            $('#divAudit').hide();
            $('#divList').show();
            typeChoose($(this).attr('value') || null);
            // $("#list").hiMallDatagrid('reload', { auditStatus: $(this).attr('value') || null, brandName: '', keyWords: ''});
        }
        else if ($(this).attr('type') == 'audit-on-off') {
            $.post('GetProductAuditOnOff', {}, function (result) {
                if (result.value == 1) {
                    $('#radio1').attr('checked', 'checked');
                }
                else {
                    $('#radio2').attr('checked', 'checked');
                }
            });
            $('#divAudit').show();
            $('#divList').hide();
        }
    });
    $('#btnSubmit').click(function () {
        var data = $('#radio1').get(0).checked== true ? 1 : 0;
        $.post('SaveProductAuditOnOff', { value: data }, function (result) {
            if (result.success) {
                $.dialog.succeedTips("提交成功！");
            }
            else {
                $.dialog.errorTips("提交出现异常！");
            }
        });
    });

});

function batchInfractionSaleOff() {
    var productIds = getSelectedIds();
    if (productIds.length == 0) {
        $.dialog.errorTips("请选择通过审核的商品");
        return;
    }
    infractionSaleOffDialog(productIds);
}


function getSelectedIds() {
    var selecteds = $("#list").hiMallDatagrid('getSelections');
    var ids = [];
    $.each(selecteds, function () {
        if (this.state == "销售中") {
            ids.push(this.id);
        }
    });
    return ids;
}


function search() {
    var brandName = $.trim($('#brandBox').val());
    var keyWords = $.trim($('#searchBox').val());
    var productId = $.trim($('#productId').val());
    var shopName = $.trim($('#shopName').val());
    $("#list").hiMallDatagrid('reload', { brandName: brandName, keyWords: keyWords, categoryId: categoryId, productCode: productId, shopName: shopName });
}

function audit(productId) {

    $.dialog({
        title: '商品审核',
        lock: true,
        id: 'goodCheck',
        content: ['<div class="dialog-form">',
            '<div class="form-group">',
                '<p class="help-top">备注</p>',
                '<textarea id="auditMsgBox" class="form-control" cols="40" rows="2"  ></textarea>\
                 <p id="valid" style="visibility:hidden;color:red">请填写未通过理由</p><p id="validateLength" style="visibility:hidden;color:red">备注在40字符以内</p> ',
            '</div>',
        '</div>'].join(''),
        padding: '10px',
        init: function () { $("#auditMsgBox").focus(); },
        button: [
        {
            name: '通过审核',
            callback: function () {
                if ($("#auditMsgBox").val().length > 40) {
                    $('#validateLength').css('visibility', 'visible');
                    return false;
                }
                auditProduct(productId, 2);
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
                else if ($("#auditMsgBox").val().length > 40) {
                    $('#validateLength').css('visibility', 'visible');
                    return false;
                }
                else {
                    $('#valid').css('visibility', 'hidden');
                    auditProduct(productId, 3, $('#auditMsgBox').val());
                }
            }
        }]
    });
}



function infractionSaleOffDialog(productIds) {
    $.dialog({
        title: '违规下架',
        lock: true,
        id: 'infractionSaleOff',
        content: ['<div class="dialog-form">',
            '<div class="form-group">',
                '<p class="help-top">下架理由</p>',
                '<textarea id="infractionSaleOffMsgBox" class="form-control" cols="40" rows="2" onkeyup="this.value = this.value.slice(0, 50)" ></textarea>\
                 <p id="valid" style="visibility:hidden;color:red">请填写下架理由</p> ',
            '</div>',
        '</div>'].join(''),
        padding: '10px',
        init: function () { $("#infractionSaleOffMsgBox").focus(); },
        button: [
        {
            name: '违规下架',
            callback: function () {
                if (!$.trim($('#infractionSaleOffMsgBox').val())) {
                    $('#valid').css('visibility', 'visible');
                    return false;
                }
                else {

                    $('#valid').css('visibility', 'hidden');
                    auditProduct(productIds, 4, $('#infractionSaleOffMsgBox').val());

                }
            },
            focus: true
        },
        {
            name: '取消',
        }]
    });
}


function auditProduct(productIds, auditState, msg) {
    var loading = showLoading();
    $.post('./BatchAudit', { productIds: productIds.toString(), auditState: auditState, message: msg }, function (result) {
        if (result.success) {
            $.dialog.succeedTips("操作成功！");
            var pageNo = $("#list").hiMallDatagrid('options').pageNumber;
            $("#list").hiMallDatagrid('reload', { pageNumber: pageNo });
        }
        else {
            $.dialog.errorTips("操作失败");
        }
        loading.close();
    });
}
