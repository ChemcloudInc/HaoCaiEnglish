function initialdeleteCategory() {
    $('.container').on('click', '.delete-classify', function () {
        var id = $(this).parents('td.td-operate').prev('td').find('.hidden_id').val();
        $.dialog.confirm('Do you confirm to delete this category', function () {
            ajaxRequest({
                type: 'POST',
                url: "./DeleteWithDraw",
                param: { id: id },
                dataType: "json",
                success: function (data) {
                    if (data.Successful == true) {
                        location.href = "./Index";
                    }
                }, error: function () { }
            });
        }, function () {
            $.dialog.tips('Cancel');
        });
    });
}

function InitialDialog(option) {
    $.dialog({
        title: option.title,
        lock: true,
        id: 'addAtrr',
        content: ['<div class="dialog-form">',
            '<div class="form-group">',
                '<label class="label-inline" for="">Category Name</label><input value="' + option.name + '" id="newCategoryName" class="form-control input-sm" type="text" >',
                '<p id="nameErrorMsg" class="help-block">category name is required</p>',
            '</div>',
            '<div class="form-group">',
                '<label class="label-inline" for="">Parent Category</label>',
                '<select class="form-control input-sm" id="categoryDrop"></select>',
            '</div>',
        '</div>'].join(''),
        init: function () {
            $("#newCategoryName").focus();
            $.ajax({
                type: 'GET',
                url: './GetCategoryDrop',
                cache: false,
                data: { id: option.id },
                dataType: "json",
                success: function (data) {
                    if (data.successful == true) {
                        var drop = $("#categoryDrop");
                        var html = [], cate = data.category;
                        for (var i = 0; i < cate.length; i++) {
                            var selected = cate[i].Selected == true ? "selected" : "";
                            html.push('<option ' + selected + ' value="' + cate[i].Value + '">' + cate[i].Text + '</option>');
                        }
                        $(drop).append($(html.join('')));
                    }

                }
            });
        },
        padding: '20px 10px',
        okVal: 'Save',
        ok: function () {
            var len = $("#newCategoryName").val().length;
            if (len > 5 || len <= 0) {
                $("#nameErrorMsg").css('color', 'red');
                $("#newCategoryName").focus();
                return false;
            }
            var name = $("#newCategoryName").val();
            var pId = parseInt($("#categoryDrop option:selected").val());

            var loading = showLoading();
            $.ajax({
                type: 'POST',
                url: './CreateCategory',
                cache: false,
                data: { name: name, pId: pId },
                dataType: "json",
                success: function (data) {
                    loading.close();
                    if (data.successful == true) {
                        location.reload();
                    }
                    else {
                        $.dialog.errorTips(data.msg);
                    }
                }
            });

        }
    });
}