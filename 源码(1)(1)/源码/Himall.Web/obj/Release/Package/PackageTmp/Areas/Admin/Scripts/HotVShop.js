$(function () {
    initGrid();
});

$('#searchButton').click(function (e) {
    var vshopName = $('#vshopName').val();
    var sTime = $('#inputStartDate').val();
    var eTime = $('#inputEndDate').val();
    $("#list").hiMallDatagrid('reload', { vshopName: vshopName, startTime: sTime, endTime: eTime });
    searchClose(e);
});

function initGrid() {
    $("#list").hiMallDatagrid({
        url: '/admin/VShop/GetHotShop',
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
        columns:
        [[
             {
                 field: "name", title: '微店名称', align: "left"
             },
            {
                field: "squence", title: '排序', align: "left", formatter: function (value, row, index) {
                    return '<input class="text-order" type="text" orivalue="' + value + '" hpId="' + row.id + '" name="sequence" value="' + value + '">';
                }
            },
            {
                field:"addTime",title:'添加时间',align:"left"
            },
            {
                field: "creatTime", title: '创建时间', align: "left"
            },
            
            {
                field: "visiteNum", title: '进店浏览量', align: "left"
            },
            {
                field: "buyNum", title: "成交量", align: "left"
            },
            {
                field: "s", title: "操作", align: "center",
                formatter: function (value, row, index) {
                    var html = "";
                    html += '<span class="btn-a"><a class="good-setTopshop">替换</a>';
                    html += '<input class="thisId" type="hidden" value="' + row.id + '"/><input class="thisName" type="hidden" value="' + row.name + '"/>';
                    html += '<a class="good-down">置为普通</a></span>';
                    return html;
                },
            }
        ]]
    });
}
$('#list').on('blur', 'input[name="sequence"]', function () {
    var id = $(this).attr('hpId');
    var sequence = $(this).val();
    var sequence = parseInt(sequence);
    if (isNaN(sequence)) {
        $.dialog.errorTips('数字格式不正确');
        $(this).val($(this).attr('orivalue'));
    }
    else {
        var loading = showLoading();
        $.post('../VShop/UpdateSequence', { id: id, sequence: sequence }, function (data) {
            loading.close();
            if (data.success) {
                $(this).attr('orivalue', sequence);
                // $.dialog.tips('更新显示顺序成功');
            }
            else
                $.dialog.errorTips('更新显示顺序失败！' + result.msg);
        });
    }
});


$('#list').on('click', '.good-down', function () {
    var name = $(this).siblings('.thisName').val();
    var id = $(this).siblings('.thisId').val();
    $.dialog.confirm('是否将' + name + '移除热门微店?', function () {
        var loading = showLoading();
        $.post('../VShop/DeleteHotVShop', { vshopId: id }, function (result) {
            loading.close();
            if (result.success) {
                $.dialog.tips("已将" + name + "移除热门微店");
                window.location.reload();
            }
            else
                $.dialog.errortips('删除失败！' + result.msg);
        })
    })
})


$('#list').on('click', '.good-setTopshop', function () {
    var oldVShopId = $(this).siblings('.thisId').val();
    replaceHotVShop(oldVShopId);
})

$('#searchReplaceVShopButton').click(function () {
    var vshopName = $('#titleKeyword').val();
    $("#repaceList").hiMallDatagrid('reload', { vshopName: vshopName, vshopType: 0 });
});


function replaceHotVShop(oldVShopId) {
    $.dialog({
        title: '替换微店',
        lock: true,
        width: 550,
        padding: '10px',
        id: 'replaceHotVShopDialog',
        content: $('#replace-HotVShop')[0],
    });

    //商品表格
    $("#repaceList").hiMallDatagrid({
        url: '/admin/VShop/GetVshops',
        nowrap: false,
        rownumbers: true,
        NoDataMsg: '没有找到符合条件的数据',
        border: false,
        fit: true,
        fitColumns: true,
        pagination: true,
        idField: "id",
        pageSize: 16,
        pagePosition: 'bottom',
        pageNumber: 1,
        queryParams: { vshopType: 0 },
        columns:
        [[
            {
                field: "name", title: '微店名称', align: "left"
            },
            {
                field: "categoryName", title: '经营类目', align: "left"
            },
            {
                field: "creatTime", title: '创建时间', align: "left"
            },
            {
                field: "visiteNum", title: '进店浏览量', align: "left"
            },
            {
                field: "buyNum", title: "成交量", align: "left"
            },
            {
                field: "s", title: "操作", align: "center",
                formatter: function (value, row, index) {
                    var html = "";
                    html += '<span class="btn-a"><a class="good-setTopshop">替换</a>';
                    html += '<input class="oldVShopId" type="hidden" value="' + oldVShopId + '"/><input class="newVShopId" type="hidden" value="' + row.id + '"/></span>';
                    return html;
                },
            }
        ]]
    });
}


$('#repaceList').on('click', '.good-setTopshop', function () {
    var oldId = $(this).siblings('.oldVShopId').val();
    var newId = $(this).siblings('.newVShopId').val();
    var loading = showLoading();
    $.post('../VShop/ReplaceHotShop', { oldVShopId: oldId, newHotVShopId: newId }, function (result) {
        loading.close();
        if (result.success) {
            $.dialog.succeedTips("替换成功");
            window.location.reload();
        }
        else
            $.dialog.errorTips('删除失败！' + result.msg);
    })
})



