$(function () {
    //商品表格
    $("#topicGrid").hiMallDatagrid({
        url: './list',
        nowrap: false,
        rownumbers: true,
        NoDataMsg: 'No matching topics',
        border: false,
        fit: true,
        fitColumns: true,
        pagination: true,
        idField: "id",
        pageSize: 16,
        pagePosition: 'bottom',
        pageNumber: 1,
        queryParams: { auditStatus: 1 },
        columns:
        [[
            {
                field: "name", title: 'Topic name', align: "center"
            },
            {
                field: "tags", title: 'Label', align: "center", width: 300
            },
            {
                field: "s", title: "Operation", align: "center",width:300,
                formatter: function (value, row, index) {
                    var html = '<span class="btn-a"><input type="hidden" value="' + row.url + '" id="url' + row.id + '" />';
                    html += '<a class="good-check" href="./save/' + row.id + '">edit</a><a class="good-check" onclick="deleteTopic(' + row.id + ',\'' + row.name + '\')">delete</a>';
                    html += '<a onclick="copyurl(\'' + row.id + '\')" class=\"good-check copyurlbt\">copy link</a>';
                    //html += "<a href='" + row.url + "' target='_blank'>预览</a></span>";
                    html += '<a class="glyphicon glyphicon-eye-open view-mobile-shop" title="预览" data-url="' + row.url + '" style="font-size:18px;text-decoration: none; cursor:pointer;"></a>'
                    html += '</span>';
                    return html;
                },
                styler: function () {
                    return 'td-operate';
                }
            }
        ]]
    });


    $('#searchButton').click(function () {
        var titleKeyword = $('#titleKeyword').val();
        var tagsKeyword = $('#tagsKeyword').val();
        $("#topicGrid").hiMallDatagrid('reload', { tagsKeyword: tagsKeyword, titleKeyword: titleKeyword });
    });
});

function copyurl( id )
{
    url = $('#url'+id).val();
    $.dialog( {
        title: '专题链接',
        lock: true,
        id: 'goodCheck',
        content: ['<div class="dialog-form">',
            '<div class="form-group">',
                '<input type="text" id="txturl" value="' + url + '" class="form-control" style="width:300px"/>',
            '</div>',
        '</div>'].join( '' ),
        padding: '10px',
        init: function () { $( "#txturl" ).focus(); }
    } );
}
function initzclip()
{
    $(".copyurlbt").zclip({
        path: '/scripts/ZeroClipboard.swf', //记得把ZeroClipboard.swf引入到项目中 
        copy: function () {
            return "I am copy";
        },
        afterCopy: function () {
            alert("Copied");
        }
    });
}


function deleteTopic(id, name) {
    $.dialog.confirm('Are you sure you want to delete ' + name + ' ?', function () {
        var loading = showLoading();
        $.post('./delete', { id: id }, function (result) {
            loading.close();
            if (result.success) {
                $.dialog.tips('Delete successfully', function () {

                    var pageNo = $("#topicGrid").hiMallDatagrid('options').pageNumber;
                    $("#topicGrid").hiMallDatagrid('reload', { pageNumber: pageNo });
                });
            }
            else
                $.dialog.alert('Delete failed!' + result.msg);
        }, "json");
    });
}

function copyToClipboard(dom) {
    var _t = $(dom);
    var txt = _t.attr("data-id");
    var text = $("#url" + txt).val();
    _t.zclip({
        path: '/scripts/ZeroClipboard.swf', //记得把ZeroClipboard.swf引入到项目中 
        copy: function () {
            return text;
        },
        afterCopy: function () {
            alert("Copied");
        }
    });
}