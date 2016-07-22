function AddWeixinMenu(divId) {

    $.dialog({
        title: '新增二级菜单',
        lock: true,
        id: 'addWeixinMenu',
        content: $("#" + divId).html(),
        padding: '10px',
        button: [
        {
            name: '确认',
            callback: function () {
                if ($("#txtMenuName").val().length <= 0 || $("#txtMenuName").val().length > 7) {
                    $.dialog.tips("菜单名称不能为空在7个字符以内");
                    return false;
                }
                if ($("#mainMenu").val() == null)
                {
                    $.dialog.tips("请选择一级菜单");
                    return false;
                }
                Add($("#txtMenuName").val(), $("#ddlType").val(), $("#mainMenu").val(), $("#menuUrl").val());
            },
            focus: true
        },
        {
            name: '取消',
        }]
    });
}


function AddMainMenu(divId)
{
    $.dialog({
        title: '新增一级菜单',
        lock: true,
        id: 'addWeixinMainMenu',
        content: $("#" + divId).html(),
        padding: '10px',
        button: [
        {
            name: '确认',
            callback: function () {
                if ($("#txtMenuName1").val().length <= 0 || $("#txtMenuName1").val().length > 5) {
                    $.dialog.tips("菜单名称不能为空在5个字符以内");
                    return false;
                }
                Add($("#txtMenuName1").val(), $("#ddlType1").val(), '0', $("#menuUrl1").val());
            },
            focus: true
        },
        {
            name: '取消',
        }]
    });
}


function Add(title, urlType, parentId, url) {
    var loading = showLoading();
    $.post('./AddMenu', { title: title, url: url, parentId: parentId, urlType: urlType }, function (result) {
        loading.close();
        if (result.success) {
            $.dialog.succeedTips("添加成功！");
            location.reload();
        }
        else
            $.dialog.errorTips("添加失败"+result.msg);
    });
}