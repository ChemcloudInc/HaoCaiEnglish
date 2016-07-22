
var editor;

$(function () {
    initRichTextEditor();
    bindSubmitBtnClickEvent();
});



function initRichTextEditor() {

    editor = UE.getEditor('contentContainer');

    //eidtor.addListener('contentChange', function () {
    //    $('#contentError').hide();
    //});

}

function bindSubmitBtnClickEvent() {
    $('#submit').click(function () {
        submit();
    });
}


function submit() {
    var name = $.trim($('input[name="name"]').val());
    var content = editor.getContent();
    var reg = /^[a-zA-Z0-9\u4e00-\u9fa5]+$/;

    if (!name || name.length > 30 || reg.test(name) == false) {
        $('input[name="name"]').css({ border: '1px solid #f60' });
        $('input[name="name"]').focus();
        return false;
    } else {
        $('input[name="name"]').css({ border: '1px solid #ccc' });
    }
    if (!content) {
        $('#edui1').css({ border: '1px solid #f60' });
        $('#edui1').focus();
    }

    if (!name || name.length > 30 || reg.test(name) == false || !content) return false;

    var data = $('form').serialize();
    var loading = showLoading();
    $.post('add', data, function (result) {
        loading.close();
        if (result.success) {
            $.dialog.succeedTips('保存成功');
            setTimeout(function () { location.href = 'management'; }, 1300);
        }
        else
            $.dialog.errorTips('文章失败' + result.msg);
    });

}

