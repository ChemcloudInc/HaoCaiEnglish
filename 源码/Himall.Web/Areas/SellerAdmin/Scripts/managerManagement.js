
$(function () {
    query();

    //添加管理员
    $('.add-manager').click(function () {
        LoadAddBox();
    });
})

function Delete(id) {
    $.dialog.confirm('Determine this record to delete it?', function () {
        var loading = showLoading();
        $.post("./Delete", { id: id }, function (data) {
            loading.close();
            $.dialog.tips(data.msg); query()
        });
    });
}
function BatchDelete() {
    var selectedRows = $("#list").hiMallDatagrid("getSelections");
    var selectids = new Array();

    for (var i = 0; i < selectedRows.length; i++) {
        selectids.push(selectedRows[i].Id);
    }
    if (selectedRows.length == 0) {
        $.dialog.tips("You did not choose any of the options!");
    }
    else {
        $.dialog.confirm('Sure to delete the selected administrator?', function () {
            var loading = showLoading();
            $.post("./BatchDelete", { ids: selectids.join(',') }, function (data) {
                loading.close();
                $.dialog.tips(data.msg); query()
            });
        });
    }
}

function query() {
    $("#list").hiMallDatagrid({
        url: './list',
        nowrap: false,
        rownumbers: true,
        NoDataMsg: 'Not matching data.',
        border: false,
        fit: true,
        fitColumns: true,
        pagination: true,
        idField: "Id",
        pageSize: 10,
        pageNumber: 1,
        queryParams: {},
        toolbar: /*"#goods-datagrid-toolbar",*/'',
        operationButtons: "#batchOperate",
        columns:
        [[
            { checkbox: true, width: 39 },
            { field: "Id", hidden: true },
            { field: "UserName", title: 'Administrator' },
            { field: "CreateDate", title: 'Creation Date' },
            { field: "RoleName", title: 'Privilege Groups' },
        {
            field: "operation", operation: true, title: "Operation",
            formatter: function (value, row, index) {
                var id = row.Id.toString();
                var roleid = row.RoleId.toString();
                var username = row.UserName;
                var realname = row.realName;
                var remark = row.reMark;
                var model = JSON.stringify({ id: id, roleid: roleid, username: username, realname: realname, remark: remark });
                var html = ["<span class=\"btn-a\">"];
                if (row.RoleId != 0) {
                    html.push("<a onclick='Change(" + model + ");'>edit</a>");
                    html.push("<a onclick=\"Delete('" + id + "');\">delete</a>");
                }
                html.push("</span>");
                return html.join("");
            }
        }
        ]]
    });
}

function LoadRoleList(callback) {
    if ($("#RoleId option").length > 0) {
        callback();
        return;
    }
    var loading = showLoading();
    var result = false;
    $.ajax({
        type: 'post',
        url: 'RoleList',
        cache: false,
        async: true,
        data: {},
        dataType: "json",
        success: function (data) {
            loading.close();
            $(data).each(function (index, item) { $("#RoleId").append("<option value=" + item.Id + ">" + item.RoleName + "</option>") });
            callback();
        },
        error: function () {
            loading.close();
        }
    });
}


function Change(model) {
    var id = model.id;
    var username = model.username;
    var realname = model.realname;
    var remark = model.remark;
    var roleid = model.roleid;
    LoadRoleList(function () {
        $("#UserName").val(username).attr("disabled", true);
        $("#PassWord").val("");
        $("#confirmPassWord").val("");
        $("#name-prefix").text("");
        $("#realName").val(realname);
        $("#reMark").val(remark);
        if (roleid != 0) {
            $("#RoleId").val(roleid);
            $("#roleGroupDiv").show();
        }
        else {
            $("#roleGroupDiv").hide();
        }
    });

    $.dialog({
        title: 'Modify password',
        lock: true,
        id: 'ChangePwd',
        width: '500px',
        content: document.getElementById("addManagerform"),
        padding: '20px 10px',
        okVal: 'OK',
        init: function () { $("#PassWord").focus(); },
        ok: function () {
            var confirmPassWord = $("#confirmPassWord").val();
            var realName = $("#realName").val();
            var reMark = $("#reMark").val();
            var SelectedRoleId = $("#RoleId").val();
            var password = $("#PassWord").val();
            if (realName == null || realName == "") {
                $.dialog.tips("Username must be filled!！");
                return false;
            }
            if (password != null && password != "") {
                if (password.length < 6) {
                    $.dialog.tips("Length of password is at least 6!");
                    return false;
                }
                if (confirmPassWord != password) {
                    $.dialog.tips("Enter the password twice inconsistent!");
                    return false;
                }
            }
            var loading = showLoading();
            if (SelectedRoleId == null)
                SelectedRoleId = 0;
            $.post("./Change",
                { id: id, password: password, roleid: SelectedRoleId, realName: realName, reMark: reMark },
                function (data) {
                    loading.close();
                    if (data.success) {
                        $.dialog.tips("Successfully modified", function () {
                            //if (roleid != 0 && roleid != SelectedRoleId)
                                query();
                        });
                        $("#password").val("");
                    }
                    else
                        $.dialog.tips("Modify failed:" + data.msg);
                });
        }
    });
}

function LoadAddBox() {
    LoadRoleList(function () {
        $("#UserNameDiv").show();
        $("#name-prefix").text(mainUserName + ":");
        $("#UserName").val("").removeAttr("disabled");
        $("#roleGroupDiv").show();
        $("#PassWord").val("");
        $("#RoleId").val("");
        $("#realName").val("");
        $("#reMark").val("");
    })
    $.dialog({
        title: 'Add administrator',
        id: 'addManager',
        width: '500px',
        content: document.getElementById("addManagerform"),
        lock: true,
        okVal: 'Confirm add',
        init: function () { $("#UserName").focus(); },
        ok: function () {
            var roleId = $("#RoleId").val();
            var username = $("#UserName").val();
            var password = $("#PassWord").val();
            var confirmPassWord = $("#confirmPassWord").val();
            var realName = $("#realName").val();
            var reMark = $("#reMark").val();
            if (realName == null || realName == "") {
                $.dialog.tips("User name must be filled!");
                return false;
            }
            if (confirmPassWord != password) {
                $.dialog.tips("Enter the password twice inconsistent!");
                return false;
            }
            if (roleId == null) {
                $.dialog.tips("Please select the privilege group, if there is no privilege group,Please add!");
                return false;
            }
            if (!CheckAdd(username, password))
                return false;
            AddManage(username, password, roleId, realName, reMark);
        }
    });
}

function AddManage(username, password, roleid, realName, reMark) {
    var loading = showLoading();

    $.ajax({
        type: 'post',
        url: 'Add',
        cache: false,
        async: true,
        data: { UserName: username, PassWord: password, RoleId: roleid, realName: realName, reMark: reMark },
        dataType: "json",
        success: function (data) {
            loading.close();
            if (data.success) {
                $.dialog.tips("Add successfully!");
                $("#addManagerform input").val("");
                query();
            }
            else {
                $.dialog.tips("Add failed!" + data.msg);
            }
        },
        error: function () {
            loading.close();
        }
    });
}


function CheckAdd(username, password) {
    var reg = /^[\u4E00-\u9FA5\@A-Za-z0-9\_\-]{4,20}$/;

    var regpwd = /^[^\s]{6,20}$/;
    var pwdOk = regpwd.test(password);
    if (username.length < 4) {
        $.dialog.tips("User name can not be less than 4 characters");
        return false;
    }
    else if (!reg.test(username)) {
        $.dialog.tips('User name needs 4-20 characters');
        return false;
    }
    else if (!pwdOk) {
        $.dialog.tips("Password needs 6-20 characters, not containing spaces");
        return false;
    }
    var loading = showLoading();
    var result = false;
    $.ajax({
        type: 'post',
        url: 'IsExistsUserName',
        cache: false,
        async: false,
        data: { UserName: username },
        dataType: "json",
        success: function (data) {
            loading.close();
            result = !data.Exists;
            if (data.Exists)
                $.dialog.tips("This user name already exists!");
        },
        error: function () {
            loading.close();
        }
    });
    return result;
}