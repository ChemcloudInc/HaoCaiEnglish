/// <reference path="../../../Scripts/jqeury.himallLinkage.js" />
var categoryId;
var lastType;
var curType;
$(function () {
    bindTabSwich();
    if (val == '') {
        initGrid();
    }
    initDatePicker();
    bindSearchBtnClick();
    initCategoryLinkage();
    initBrandAutoComplete();
    bindAssociateTemplateBtnClickEvent();

    $('#list').on('click', '.good-down', function () {
        var name = $(this).siblings('.thisName').val();
        var ids = $(this).siblings('.thisId').val();
        $.dialog.confirm('您确定要下架' + (name ? ' “' + name + '” ' : ('这' + ($.isArray(ids) ? ids.length : 1) + '件商品')) + '吗？', function () {
            var loading = showLoading();
            $.post('batchSaleOff', { ids: ids.toString() }, function (result) {
                loading.close();
                if (result.success) {
                    $.dialog.tips('下架商品成功');
                    var pageNo = $("#list").hiMallDatagrid('options').pageNumber;
                    reload(pageNo);
                }
                else
                    $.dialog.alert('下架商品失败!' + result.msg);
            });
        });


    });

    $('#list').on('click', '.good-up', function () {
        var name = $(this).siblings('.thisName').val();
        var ids = $(this).siblings('.thisId').val();
        if (curType == 'saleOff' && AuditOnOff == 0) {
            $.dialog.alert('违规下架的商品，不能再上架!');
            return;
        }
        $.dialog.confirm('您确定要上架' + (name ? ' “' + name + '” ' : ('这' + ($.isArray(ids) ? ids.length : 1) + '件商品')) + '吗？', function () {
            var loading = showLoading();
            $.post('batchOnSale', { ids: ids.toString() }, function (result) {
                loading.close();
                if (result.success) {
                    $.dialog.tips('申请商品上架成功');
                    var pageNo = $("#list").hiMallDatagrid('options').pageNumber;
                    reload(pageNo);
                }
                else
                    $.dialog.alert('申请商品上架失败!' + result.msg);
            });
        });
    });

    $('#list').on('click', '.good-del', function () {
        var name = $(this).siblings('.thisName').val();
        var ids = $(this).siblings('.thisId').val();
        $.dialog.confirm('您确定要删除' + (name ? ' “' + name + '” ' : ('这' + ($.isArray(ids) ? ids.length : 1) + '件商品')) + '吗？', function () {
            var loading = showLoading();
            $.post('Delete', { ids: ids.toString() }, function (result) {
                loading.close();
                if (result.success) {
                    $.dialog.tips('删除商品成功');
                    var pageNo = $("#list").hiMallDatagrid('options').pageNumber;
                    reload(pageNo);
                }
                else
                    $.dialog.alert('删除商品失败!' + result.msg);
            });
        });
    });


    $('#list').on('hover', '.good-share', function () {
        $(this).toggleClass('active');
    });
});

function deleteProduct(ids) {
    $.dialog.confirm('您确定要删除这些商品吗？', function () {
        var loading = showLoading();
        $.post('Delete', { ids: ids.join(',').toString() }, function (result) {
            loading.close();
            if (result.success) {
                $.dialog.tips('删除商品成功');
                var pageNo = $("#list").hiMallDatagrid('options').pageNumber;
                reload(pageNo);
            }
            else
                $.dialog.alert('删除商品失败!' + result.msg);
        });
    });
}

function bindSearchBtnClick() {

    $('#searchButton').click(function (e) {
        searchClose(e);
        search();
    });

}


function bindTabSwich() {
    $('div[type="filter"]').hide();
    $('.nav li').click(function (e) {
        var _t = $(this);
        searchClose(e);
        clearFilter();
        _t.addClass('active').siblings().removeClass('active');
        var params = {};
        var type = _t.attr('type');
        curType = type;
        params.saleStatus = params.auditStatus = '';
        var girdType = 'normal';
        switch (type) {
            case 'onSale':
                params.saleStatus = _t.attr('value');
                params.auditStatus = 2;
                normalFilter();
                break;
            case 'inStock':
                params.saleStatus = _t.attr('value');
                normalFilter();
                break;
            case 'unAudit':
                params.auditStatus = "";
                params.saleStatus = "";
                params.auditStatuses = "1,3";
                girdType = 'audit';
                auditFilter();
                break;
            case 'saleOff':
                girdType = 'infractionSaleOff';
                params.auditStatus = _t.attr('value');
                saleOffFilter();
                break;
            case 'inDraft':
                params.saleStatus = _t.attr('value');
                normalFilter();
                break;

        }

        switch (girdType) {
            case 'normal':
                if (lastType == girdType)
                    $("#list").hiMallDatagrid('reload', params);
                else
                    initGrid(params);
                break;
            case 'audit':
                if (lastType == girdType) {
                    $("#list").hiMallDatagrid('reload', params);
                }
                else
                    initAuditGrid();
                break;
            case 'infractionSaleOff':
                if (lastType == girdType)
                    $("#list").hiMallDatagrid('reload', params);
                else
                    initInfractionSaleOffGrid();
                break;
        }

    });

}

function normalFilter() {
    $('.search-box').removeClass('only-line');
    $('div[saleOff]').hide();
    $('div[audit]').hide();
    $('div[normal]').show();
    var submit = $('form.custom-inline #searchButton');
    submit.prependTo($('div.submit'));

}


function auditFilter() {
    $('.search-box').addClass('only-line');
    $('div[saleOff]').hide();
    $('div[normal]').hide();
    $('div[audit]').show();
    var submit = $('div.submit #searchButton');
    submit.appendTo(submit.parent().parent());
}

function saleOffFilter() {
    $('.search-box').addClass('only-line');
    $('div[normal]').hide();
    $('div[audit]').hide();
    $('div[saleOff]').show();
    var submit = $('div.submit #searchButton');
    submit.appendTo(submit.parent().parent());
}

function clearFilter() {
    $('#brandBox').val('');
    $('#searchBox').val('');
    $('.start_datetime').val('');
    $('.end_datetime').val('');
    categoryId = '';
    $('#category1,#category2,#category3').himallLinkage('reset');
}


function initBrandAutoComplete() {

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

}


function initInfractionSaleOffGrid() {
    lastType = 'infractionSaleOff';
    $("#list").html('');

    //商品表格
    $("#list").hiMallDatagrid({
        url: 'list',
        nowrap: false,
        rownumbers: true,
        NoDataMsg: '没有找到符合条件的商品',
        border: false,
        fit: true,
        fitColumns: true,
        pagination: true,
        idField: "Id",
        pageSize: 9,
        pagePosition: 'bottom',
        pageNumber: 1,
        queryParams: { auditStatus: 4 },
        operationButtons: "#saleOff",
        columns:
        [[
             { checkbox: true, width: 39 },
            {
                field: "Name", title: '商品', width: 450, align: 'left',
                formatter: function (value, row, index) {
                    var html = '<img style="margin-left:15px;" width="40" height="40" src="' + row.Image + '" /><span class="overflow-ellipsis" style="width:300px"><a title="' + value + '" target="_blank" href="/product/detail/' + row.Id + '">' + value + '</a></span>';
                    return html;
                }
            },
        {
            field: "AuditState", title: "违规下架理由", align: "left",
            formatter: function (value, row, index) {
                var text = row.AuditReason;
                return text;
            }
        },
        {
            field: "s", title: "操作", width: 200, align: "center",
            formatter: function (value, row, index) {
                var html = "";
                html = '<span class="btn-a"><input class="thisId" type="hidden" value="' + row.Id + '"/><input class="thisName" type="hidden" value="' + row.Name + '"/>';
                if (!row.IsLimitTimeBuy) {
                    html += '<a class="good-check" href="PublicStepTwo?productId=' + row.Id + '">编辑</a>';
                }
                html += '<a class="good-up">上架</a>';
                html += '<a class="good-del">删除</a></span>';
                return html;
            }
        }
        ]],
        onLoadSuccess: function () {
            initBatchBtnShow();
        }
    });


}

function initAuditGrid() {
    lastType = 'audit';
    $("#list").html('');

    //商品表格
    $("#list").hiMallDatagrid({
        url: 'list',
        nowrap: false,
        rownumbers: true,
        NoDataMsg: '没有找到符合条件的商品',
        border: false,
        fit: true,
        fitColumns: true,
        pagination: true,
        idField: "Id",
        pageSize: 9,
        pagePosition: 'bottom',
        pageNumber: 1,
        queryParams: { auditStatuses: '1,3' },
        operationButtons: "#saleOff",
        columns:
        [[
             { checkbox: true, width: 39 },
             { field: "IsLimitTimeBuy", hidden: true, width: 39 },
            {
                field: "Name", title: '商品', align: 'left',
                formatter: function (value, row, index) {
                    var html = '<img style="margin-left:15px;" width="40" height="40" src="' + row.Image + '" /><span class="overflow-ellipsis" style="width:350px"><a title="' + value + '" href="/product/detail/' + row.Id + '" target="_blank">' + value + '</a></span>';
                    return html;
                }
            },
        {
            field: "AuditState", title: "审核", width: 150, align: "center",
            formatter: function (value, row, index) {
                var text = '';
                if (row.AuditState == 1)
                    text = '等待审核';
                else if (row.AuditState == 3) {
                    text = '<label style="color:red">未通过</label><br />' + row.AuditReason;
                }
                return text;
            }
        },
        {
            field: "s", title: "操作", width: 150, align: "center",
            formatter: function (value, row, index) {
                var html = "";
                html = '<span class="btn-a"><input class="thisId" type="hidden" value="' + row.Id + '"/><input class="thisName" type="hidden" value="' + row.Name + '"/>';
                if (!row.IsLimitTimeBuy) {
                    html += '<a class="good-check" href="PublicStepTwo?productId=' + row.Id + '">编辑</a>';
                }// html += '<a class="good-check" onclick="onSale(' + row.Id + ',\'' + row.Name + '\')">上架</a>';
                html += '<a class="good-down">下架</a>';
                html += '<a class="good-del">删除</a></span>';
                return html;
            }
        }
        ]],
        onLoadSuccess: function () {
            initBatchBtnShow();
            bindAssociateTemplateBtnClickEvent();
        }
    });
}



function initGrid(params) {
    lastType = 'normal';
    $("#list").html('');
    normalFilter();

    //商品表格
    $("#list").hiMallDatagrid({
        url: 'list',
        nowrap: false,
        rownumbers: true,
        NoDataMsg: '没有找到符合条件的商品',
        border: false,
        fit: true,
        fitColumns: true,
        pagination: true,
        idField: "Id",
        pageSize: 9,
        pagePosition: 'bottom',
        pageNumber: 1,
        queryParams: params ? params : { saleStatus: 1, auditStatus: 2 },
        operationButtons: "#saleOff",
        columns:
        [[
            { checkbox: true, width: 39 },
             { field: "IsLimitTimeBuy", hidden: true, width: 39 },
            { field: "ProductCode", title: '商品货号', width: 50 },
            {
                field: "Name", title: '商品', width: 300, align: 'left',
                formatter: function (value, row, index) {
                    var html = '<img style="margin-left:15px;" width="40" height="40" src="' + row.Image + '" /><span class="overflow-ellipsis" style="width:220px"><a title="' + value + '" target="_blank" href="/product/detail/' + row.Id + '">' + value + '</a></span>';
                    return html;
                }
            },
        { field: "CategoryName", title: "商家分类", width: 100, align: "left" },
        { field: "BrandName", title: "品牌", width: 90, align: "left" },
         { field: "PublishTime", title: "发布时间", width: 120, align: "left" },
        {
            field: "Price", title: "价格", width: 50, align: "left",
            formatter: function (value, row, index) {
                return '￥' + value.toFixed(2);
            }
        },
        {
            field: "s", title: "操作", align: "center",
            formatter: function (value, row, index) {
                html = '<span class="btn-a"><input class="thisId" type="hidden" value="' + row.Id + '"/><input class="thisName" type="hidden" value="' + row.Name + '"/>';
                if (!row.IsLimitTimeBuy) {
                    html += '<a class="good-check" href="PublicStepTwo?productId=' + row.Id + '">编辑</a>';
                }
                var qzoneurl = "http://sns.qzone.qq.com/cgi-bin/qzshare/cgi_qzshare_onekey?url=";
                var sinaurl = "http://service.weibo.com/share/share.php?source=bookmark&url=";
                var currenturl = encodeURIComponent('http://' + window.location.host + '/product/detail/' + row.Id + '?uid=' + row.Uid);
                if (row.SaleState == 1) {
                    html += '<a class="good-down">下架</a><a class="good-del">删除</a><div class="good-share"><i>分享</i>';
                    html += '<div class="share-box"><a href="javascript:void(0);" onclick="window.open(\'' + qzoneurl + currenturl + '&pics=http://' + window.location.host + encodeURIComponent(row.Image) + '&title=' + row.Name + '\');return false;" title="分享到QQ空间"><img src="/Images/qzone.png" /></a>';
                    html += '<a href="javascript:void(0);" onclick="window.open(\'' + sinaurl + currenturl + '&title=' + row.Name + '&pic=http://' + window.location.host + row.Image + '\');return false;" title="分享到新浪微博"><img src="/Images/weibo.png"/></a>';
                    html += '<a href="javascript:void(0);" title="分享到微信" onclick="showQrCode(\'' + row.QrCode + '\')"><img src="/Images/wx.png"/></a><s></s></div></div></span>';
                }

                else { html += '<a class="good-up">上架</a><a class="good-del">删除</a>'; }



                return html;
            }
        }
        ]],
        onLoadSuccess: function () {
            initBatchBtnShow();
            bindAssociateTemplateBtnClickEvent();
        }
    });


}




function initCategoryLinkage() {

    $('#category1,#category2').himallLinkage({
        url: '../Category/getCategory',
        enableDefaultItem: true,
        defaultItemsText: '全部',
        onChange: function (level, value, text) {
            categoryId = value;
        }
    });

}


function search() {
    var brandName = $.trim($('#brandBox').val());
    var keyWords = $.trim($('#searchBox').val());
    var productCode = $.trim($('#productId').val());
    var shopName = $.trim($('#shopName').val());
    var auditStatuses = $('select[name="auditState"]').val();
    if (!auditStatuses && $('.nav li.active').attr('type') == 'unAudit')
        auditStatuses = '1,3';
    var startDate = $('.start_datetime').val();
    var endDate = $('.end_datetime').val();

    $("#list").hiMallDatagrid('reload', {
        brandName: brandName, keyWords: keyWords,
        categoryId: categoryId, productCode: productCode, shopName: shopName,
        startDate: startDate, endDate: endDate, auditStatuses: auditStatuses
    });
}




function reload(pageNo) {

    $("#list").hiMallDatagrid('reload', { pageNumber: pageNo });
}

function getSelectedIds() {
    var selecteds = $("#list").hiMallDatagrid('getSelections');
    var ids = [];
    $.each(selecteds, function () {
        ids.push(this.Id);
    });
    return ids;
}

function saleOff(ids) {
    $.dialog.confirm('您确定要下架这些商品吗？', function () {
        var loading = showLoading();
        $.post('batchSaleOff', { ids: ids.join(',').toString() }, function (result) {
            loading.close();
            if (result.success) {
                $.dialog.tips('下架商品成功');
                var pageNo = $("#list").hiMallDatagrid('options').pageNumber;
                reload(pageNo);
            }
            else
                $.dialog.alert('下架商品失败!' + result.msg);
        });
    });
}

function onSale(ids) {
    $.dialog.confirm('您确定要上架这些商品吗？', function () {
        var loading = showLoading();
        $.post('batchOnSale', { ids: ids.join(',').toString() }, function (result) {
            loading.close();
            if (result.success) {
                $.dialog.tips('上架商品成功');
                var pageNo = $("#list").hiMallDatagrid('options').pageNumber;
                reload(pageNo);
            }
            else
                $.dialog.alert('上架商品失败!' + result.msg);
        });
    });
}

function initBatchBtnShow() {
    var type = $('.nav li.active').attr('type');
    if (type == 'onSale') {//当前为销售中tab
        $('#batchSaleOff')
            .show()
            .unbind('click')
            .click(function () {
                var ids = getSelectedIds();
                if (ids.length > 0)
                    saleOff(ids);
                else
                    $.dialog.tips('请至少选择一件商品');
            });

    }
    else if (type == 'inStock' || type == 'saleOff') {
        $('#batchOnSale')
           .show()
           .unbind('click')
           .click(function () {
               var ids = getSelectedIds();
               if (ids.length > 0)
                   onSale(ids);
               else
                   $.dialog.tips('请至少选择一件商品');
           });
    }

    if (type == 'unAudit' || type == 'saleOff')
        $('#associateTemplate').hide();
    else
        $('#associateTemplate').show();
    $('#batchDelete')
        .unbind('click')
        .click(function () {
            var ids = getSelectedIds();
            if (ids.length > 0)
                deleteProduct(ids);
            else
                $.dialog.tips('请至少选择一件商品');
        });

}


function initDatePicker() {

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

}


function bindTemplate(ids) {

    $.dialog({
        title: '关联版式',
        lock: true,
        id: 'addArticleSort',
        content: ['<div class="dialog-form">',
            '<div class="form-group">',
                '<label class="label-inline" for="">顶部</label><select id="top" class="form-control input-sm"></select>',
            '</div>',
            '<div class="form-group">',
                '<label class="label-inline" for="">底部</label>',
                '<select class="form-control input-sm" id="bottom"></select>',
            '</div>',
        '</div>'].join(''),
        padding: '10px',
        okVal: '保存',
        ok: function () {
            var close = false;
            var topTemplateId = $('#top').val();
            var bottomTemplateId = $('#bottom').val();
            var loading = showLoading();
            $.ajax({
                type: "post",
                url: "bindTemplates",
                data: { topTemplateId: topTemplateId, bottomTemplateId: bottomTemplateId, ids: ids.toString() },
                dataType: "json",
                async: false,
                success: function (result) {
                    loading.close();
                    if (result.success) {
                        $.dialog.tips('关联成功');
                        clearGridSelect();
                        close = true;
                    }
                    else
                        $.dialog.alert('关联失败!' + result.msg);
                },
                error: function () {
                    loading.close();
                    $.dialog.alert('关联失败:请求出错');
                }
            });
            return close;

        }
    });

    $.post('../ProductDescriptionTemplate/getAll', {}, function (result) {
        var top = $('#top');
        var bottom = $('#bottom');
        top.append('<option value="0">不选择</option>');
        bottom.append('<option value="0">不选择</option>');
        $.each(result, function () {
            if (this.Position == 1)
                top.append('<option value="' + this.Id + '">' + this.Name + '</option>');
            else
                bottom.append('<option value="' + this.Id + '">' + this.Name + '</option>');
        });

    });

}

function clearGridSelect() {
    $("#list").hiMallDatagrid('clearSelections');
}


function bindAssociateTemplateBtnClickEvent() {

    $('#associateTemplate').click(function () {

        var selectedIds = getSelectedIds();
        var ids = getSelectedIds();
        if (ids.length > 0)
            bindTemplate(ids);
        else
            $.dialog.tips('请至少选择一件商品');
    });
}
