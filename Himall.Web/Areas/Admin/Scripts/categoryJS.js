function updateOrderOrName(actionName, param) {
    var loading = showLoading();
    ajaxRequest({
        type: 'GET',
        url: "./" + actionName,
        param: param,
        dataType: "json",
        success: function (data) {
            loading.close();
            if (data.Successful == true) {
                $.dialog.tips('更新分类的' + (actionName == 'UpdateOrder' ? '显示顺序' : '名称') + '成功.');
            }
            if ("UpdateOrder" == actionName) {
                location.reload();
            }
        }, error: function () { 
            loading.close();
        }
    });
}

function categoryTextEventBind() {
    var _order = 0;
    var _name = '';

    $('.container').on('focus', '.text-order', function () {
        _order = parseInt($(this).val());
    });
    $('.container').on('focus', '.text-name', function () {
        _name = parseInt($(this).val());
    });

    $('.container').on('blur', '.text-name,.text-order', function () {
        var id = $(this).parent('td').find('.hidden_id').val();
        if ($(this).hasClass('text-order')) {
            if (isNaN($(this).val()) || parseInt($(this).val()) <= 0) {
                $.dialog({
                    title: '更新分类信息',
                    lock: true,
                    width: '400px',
                    padding: '20px',
                    content: ['<div class="dialog-form">您输入的序号不合法,此项只能是大于零的整数.</div>'].join(''),
                    button: [
				    {
				        name: '关闭',
				    }]
                });
                $(this).val(_order);
            } else {
                if (parseInt($(this).val()) === _order) return;
                updateOrderOrName("UpdateOrder", { id: id, order: parseInt($(this).val()) });
            }
        } else {
            if ($(this).val().length === 0) {
                $.dialog({
                    title: '更新分类信息',
                    lock: true,
                    width: '400px',
                    padding: '20px',
                    content: ['<div class="dialog-form">分类名称不能为空.</div>'].join(''),
                    button: [
				    {
				        name: '关闭',
				    }]
                });
                $(this).val(_name);
            }
            else
                updateOrderOrName("UpdateName", { id: id, name: $(this).val() });
        }
    });
}

function initialdeleteCategory() {
    $('.container').on('click', '.delete-classify', function () {
        var id = $(this).parents('td.td-operate').prev('td').find('.hidden_id').val();
        $.dialog.confirm('删除该分类将会同时删除该分类的所有下级分类，您确定要删除吗？', function () {
            var loading = showLoading();
            ajaxRequest({
                type: 'POST',
                url: "./DeleteCategoryById",
                param: { id: id },
                dataType: "json",
                success: function (data) {
                    if (data.Successful == true) {
                        location.href = "./Management";
                    } else {
                        $.dialog.errorTips(data.msg);
                    }
                    loading.close();
                }, error: function () { }
            });
        });
    });
}

function initialBatchDelete() {
    $("#deleteBatch").click(function () {
        var ids = [];

        $('table.category_table tbody tr.level-1').each(function () {
            var curRow = $(this);
            if (curRow.find('input[type=checkbox]').attr('checked') == 'checked') {
                ids.push(curRow.find('input.hidden_id').val());
            }
        });
        if (ids.length == 0) { $.dialog.tips('不能批量删除,因为您没有选中任何分类.'); return; }
        $.dialog.confirm('确定删除选中的所有分类吗？', function () {
            var loading = showLoading();
            ajaxRequest({
                type: "POST",
                url: './BatchDeleteCategory',
                param: { Ids: ids.join('|') },
                dataType: "json",
                success: function (data) {
                    loading.close();
                    if (data.Successful) {
                        location.href = "./Management";
                    } else {
                        $.dialog.errorTips(data.msg, _this);
                    }
                },
                error: function (e) {
                    loading.close();
                    $.dialog.errorTips("删除失败,请重试！", _this);
                }
            });
        });
    });
}
$(function () {
    //新增分类
    categoryTextEventBind();
    //dialogInitial();
    initialdeleteCategory();
    initialBatchDelete();


    $('.check-all').click(function () {
        var checkbox = $('.table').find('input[type=checkbox]');
        if (this.checked) {
            checkbox.each(function () { this.checked = true })
        } else {
            checkbox.each(function () { this.checked = false })
        }
    });


    $('.level-1 .glyphicon').click(function () {
        var p = $(this).parents('.level-1');
        if ($(this).hasClass('glyphicon-plus-sign')) {
            var category = $(this).next('input').val();
            var url = "./GetCategoryByParentId";
            ajaxRequestForCategoryTree(this, category, url, 1);
        } else {
            $(this).removeClass('glyphicon-minus-sign').addClass('glyphicon-plus-sign');
            p.nextUntil('.level-1').remove();
        }
    });

    function ajaxRequestForCategoryTree(target, category, url, layer) {
        var loading = showLoading();
        $.ajax({
            type: 'GET',
            url: url,
            cache: false,
            data: { id: category },
            dataType: "json",
            success: function (data) {
                loading.close();
                if (data.Successfly === true) {
                    var p = $(target).parents('.level-' + layer);
                    if (data.Category.length === 0) { $.dialog.tips('该分类下目前还没有子分类.'); return; }
                    for (var i = 0; i < data.Category.length; i++) {
                        $(target).addClass('glyphicon-minus-sign').removeClass('glyphicon-plus-sign');
                        var left = layer == 1 ? 5 : (layer - 1) * 50;
                        var pix = data.Category[i].Depth === 3 ? '├───' : '└───';
                        var className = "invisible";
                        var sub = ['<tr class="level-' + (layer + 1) + '">',
                            '<td class="td-choose"><input type="checkbox" name=""/></td>',
                            '<td><s class="line" style="margin-left:' + left + 'px">' + pix + '</s>'];
                        if (data.Category[i].Depth !== 3) {
                            sub.push('<span class="glyphicon glyphicon-plus-sign"></span>');
                        }
                        sub.push('<input class="hidden_id" type="hidden" value="' + data.Category[i].Id + '">');
                        sub.push('<input class="text-name" type="text" value="' + data.Category[i].Name + '"/>');
                        sub.push('<input class="text-order" type="text" value="' + data.Category[i].DisplaySequence + '"/></td>');
                        sub.push('<td class="td-operate">');
                        sub.push('<span class="btn-a">');

                        if (data.Category[i].Depth !== 3) {
                            className = "add-classify";
                            sub.push('<a href="./AddByParent?Id=' + data.Category[i].Id + '">新增下级</a>');
                        }
                        sub.push('<a href="./Edit?Id=' + data.Category[i].Id + '">编辑</a><a class="delete-classify">删除</a></span>');
                        sub.push('</td>');
                        sub.push('</tr>');
                        p.after(sub.join(''));
                    }

                    $('.level-' + (layer + 1) + ' .glyphicon').unbind('click').bind('click', function () {
                        var p = $(this).parents('.level-' + (layer + 1));
                        if ($(this).hasClass('glyphicon-plus-sign')) {
                            var category = $(this).next('input').val();
                            var url = "./GetCategoryByParentId";
                            ajaxRequestForCategoryTree(this, category, url, layer + 1);
                        } else {
                            $(this).removeClass('glyphicon-minus-sign').addClass('glyphicon-plus-sign');
                            p.nextUntil('.level-2,.level-1').remove();
                        }

                    });
                }

            },
            error: function () {

            }
        });
    };
});





