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
        $.dialog.confirm('Are you sure you put' + (name ? ' “' + name + '” ' : ('the ' + ($.isArray(ids) ? ids.length : 1) + ' products')) + ' out of shelves?', function () {
            var loading = showLoading();
            $.post('batchSaleOff', { ids: ids.toString() }, function (result) {
                loading.close();
                if (result.success) {
                    $.dialog.tips('Succeed in puttinging it out of shelves!');
                    var pageNo = $("#list").hiMallDatagrid('options').pageNumber;
                    reload(pageNo);
                }
                else
                    $.dialog.alert('Fail to put it out of shelves!' + result.msg);
            });
        });


    });

    $('#list').on('click', '.good-up', function () {
        var name = $(this).siblings('.thisName').val();
        var ids = $(this).siblings('.thisId').val();
        if (curType == 'saleOff' && AuditOnOff == 0) {
            $.dialog.alert('Faile to put the product in shelves !');
            return;
        }
        $.dialog.confirm('Are you sure put' + (name ? ' “' + name + '” ' : ('the ' + ($.isArray(ids) ? ids.length : 1) + ' products')) + 'in shelves?', function () {
            var loading = showLoading();
            $.post('batchOnSale', { ids: ids.toString() }, function (result) {
                loading.close();
                if (result.success) {
                    $.dialog.tips('Succeed in applying for putting the products in shelves!');
                    var pageNo = $("#list").hiMallDatagrid('options').pageNumber;
                    reload(pageNo);
                }
                else
                    $.dialog.alert('Fail to apply for putting the products in shelves!' + result.msg);
            });
        });
    });

    $('#list').on('click', '.good-del', function () {
        var name = $(this).siblings('.thisName').val();
        var ids = $(this).siblings('.thisId').val();
        $.dialog.confirm('Are you sure delete ' + (name ? ' “' + name + '” ' : ('the ' + ($.isArray(ids) ? ids.length : 1) + ' products')) + '?', function () {
            var loading = showLoading();
            $.post('Delete', { ids: ids.toString() }, function (result) {
                loading.close();
                if (result.success) {
                    $.dialog.tips('Succeed in deleting the products!');
                    var pageNo = $("#list").hiMallDatagrid('options').pageNumber;
                    reload(pageNo);
                }
                else
                    $.dialog.alert('Fail to delete the products!' + result.msg);
            });
        });
    });


    $('#list').on('hover', '.good-share', function () {
        $(this).toggleClass('active');
    });
});

function deleteProduct(ids) {
    $.dialog.confirm('Are you sure delete these products?', function () {
        var loading = showLoading();
        $.post('Delete', { ids: ids.join(',').toString() }, function (result) {
            loading.close();
            if (result.success) {
                $.dialog.tips('Succeed in deleting these products!');
                var pageNo = $("#list").hiMallDatagrid('options').pageNumber;
                reload(pageNo);
            }
            else
                $.dialog.alert('Fail to delete these products!' + result.msg);
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
        NoDataMsg: 'Not found any produts',
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
                field: "Name", title: 'Products', width: 450, align: 'left',
                formatter: function (value, row, index) {
                    var html = '<img style="margin-left:15px;" width="40" height="40" src="' + row.Image + '" /><span class="overflow-ellipsis" style="width:300px"><a title="' + value + '" target="_blank" href="/product/detail/' + row.Id + '">' + value + '</a></span>';
                    return html;
                }
            },
        {
            field: "AuditState", title: "Reasons for out of Stock", align: "left",
            formatter: function (value, row, index) {
                var text = row.AuditReason;
                return text;
            }
        },
        {
            field: "s", title: "Operation", width: 200, align: "center",
            formatter: function (value, row, index) {
                var html = "";
                html = '<span class="btn-a"><input class="thisId" type="hidden" value="' + row.Id + '"/><input class="thisName" type="hidden" value="' + row.Name + '"/>';
                if (!row.IsLimitTimeBuy) {
                    html += '<a class="good-check" href="PublicStepTwo?productId=' + row.Id + '">Edit</a>';
                }
                html += '<a class="good-up">put in shelves</a>';
                html += '<a class="good-del">delete</a></span>';
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
        NoDataMsg: 'Not found any produts',
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
                field: "Name", title: 'Products', align: 'left',
                formatter: function (value, row, index) {
                    var html = '<img style="margin-left:15px;" width="40" height="40" src="' + row.Image + '" /><span class="overflow-ellipsis" style="width:350px"><a title="' + value + '" href="/product/detail/' + row.Id + '" target="_blank">' + value + '</a></span>';
                    return html;
                }
            },
        {
            field: "AuditState", title: "Auditing", width: 150, align: "center",
            formatter: function (value, row, index) {
                var text = '';
                if (row.AuditState == 1)
                    text = 'wait for audit';
                else if (row.AuditState == 3) {
                    text = '<label style="color:red">did not pass</label><br />' + row.AuditReason;
                }
                return text;
            }
        },
        {
            field: "s", title: "Operation", width: 150, align: "center",
            formatter: function (value, row, index) {
                var html = "";
                html = '<span class="btn-a"><input class="thisId" type="hidden" value="' + row.Id + '"/><input class="thisName" type="hidden" value="' + row.Name + '"/>';
                if (!row.IsLimitTimeBuy) {
                    html += '<a class="good-check" href="PublicStepTwo?productId=' + row.Id + '">Edit</a>';
                }// html += '<a class="good-check" onclick="onSale(' + row.Id + ',\'' + row.Name + '\')">put on selves</a>';
                html += '<a class="good-down">remove</a>';
                html += '<a class="good-del">delete</a></span>';
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
        NoDataMsg: 'Not found any produts',
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
            { field: "ProductCode", title: 'Number', width: 50 },
            {
                field: "Name", title: 'Product', width: 300, align: 'left',
                formatter: function (value, row, index) {
                    var html = '<img style="margin-left:15px;" width="40" height="40" src="' + row.Image + '" /><span class="overflow-ellipsis" style="width:220px"><a title="' + value + '" target="_blank" href="/product/detail/' + row.Id + '">' + value + '</a></span>';
                    return html;
                }
            },
        { field: "CategoryName", title: "Category", width: 100, align: "left" },
        { field: "BrandName", title: "Brand", width: 90, align: "left" },
         { field: "PublishTime", title: "Release time", width: 120, align: "left" },
        {
            field: "Price", title: "Price", width: 50, align: "left",
            formatter: function (value, row, index) {
                return '$' + value.toFixed(2);
            }
        },
        {
            field: "s", title: "Operation", align: "center",
            formatter: function (value, row, index) {
                html = '<span class="btn-a"><input class="thisId" type="hidden" value="' + row.Id + '"/><input class="thisName" type="hidden" value="' + row.Name + '"/>';
                if (!row.IsLimitTimeBuy) {
                    html += '<a class="good-check" href="PublicStepTwo?productId=' + row.Id + '">Edit</a>';
                }
                var qzoneurl = "http://sns.qzone.qq.com/cgi-bin/qzshare/cgi_qzshare_onekey?url=";
                var sinaurl = "http://service.weibo.com/share/share.php?source=bookmark&url=";
                var currenturl = encodeURIComponent('http://' + window.location.host + '/product/detail/' + row.Id + '?uid=' + row.Uid);
                if (row.SaleState == 1) {
                    html += '<a class="good-down">remove</a><a class="good-del">delete</a><div class="good-share"><i>share</i>';
                    html += '<div class="share-box"><a href="javascript:void(0);" onclick="window.open(\'' + qzoneurl + currenturl + '&pics=http://' + window.location.host + encodeURIComponent(row.Image) + '&title=' + row.Name + '\');return false;" title="share in qq_zone"><img src="/Images/qzone.png" /></a>';
                    html += '<a href="javascript:void(0);" onclick="window.open(\'' + sinaurl + currenturl + '&title=' + row.Name + '&pic=http://' + window.location.host + row.Image + '\');return false;" title="share in sina blog"><img src="/Images/weibo.png"/></a>';
                    html += '<a href="javascript:void(0);" title="share in WeChat" onclick="showQrCode(\'' + row.QrCode + '\')"><img src="/Images/wx.png"/></a><s></s></div></div></span>';
                }

                else { html += '<a class="good-up">put on shelves</a><a class="good-del">delete</a>'; }



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
    $.dialog.confirm('Are you sure you will put these products out of shelves?', function () {
        var loading = showLoading();
        $.post('batchSaleOff', { ids: ids.join(',').toString() }, function (result) {
            loading.close();
            if (result.success) {
                $.dialog.tips('Succeed in puttinging the products out of shelves!');
                var pageNo = $("#list").hiMallDatagrid('options').pageNumber;
                reload(pageNo);
            }
            else
                $.dialog.alert('Fail to put these products out of shelves!' + result.msg);
        });
    });
}

function onSale(ids) {
    $.dialog.confirm('Are you sure you will put these products on shelves?', function () {
        var loading = showLoading();
        $.post('batchOnSale', { ids: ids.join(',').toString() }, function (result) {
            loading.close();
            if (result.success) {
                $.dialog.tips('Succeed in putting these products on shelves!');
                var pageNo = $("#list").hiMallDatagrid('options').pageNumber;
                reload(pageNo);
            }
            else
                $.dialog.alert('Fail to put these products on shelves!' + result.msg);
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
                    $.dialog.tips('Please select one product at least!');
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
                   $.dialog.tips('Please select one product at least!');
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
                $.dialog.tips('Please select one product at least!');
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
        title: 'Related format',
        lock: true,
        id: 'addArticleSort',
        content: ['<div class="dialog-form">',
            '<div class="form-group">',
                '<label class="label-inline" for="">Top</label><select id="top" class="form-control input-sm"></select>',
            '</div>',
            '<div class="form-group">',
                '<label class="label-inline" for="">Bottom</label>',
                '<select class="form-control input-sm" id="bottom"></select>',
            '</div>',
        '</div>'].join(''),
        padding: '10px',
        okVal: 'Save',
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
                        $.dialog.tips('Relate successfully!');
                        clearGridSelect();
                        close = true;
                    }
                    else
                        $.dialog.alert('Relate unsuccessfully!' + result.msg);
                },
                error: function () {
                    loading.close();
                    $.dialog.alert('Relate unsuccessfully,error!');
                }
            });
            return close;

        }
    });

    $.post('../ProductDescriptionTemplate/getAll', {}, function (result) {
        var top = $('#top');
        var bottom = $('#bottom');
        top.append('<option value="0">not choose</option>');
        bottom.append('<option value="0">not choose</option>');
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
            $.dialog.tips('Please select one product at least!');
    });
}
