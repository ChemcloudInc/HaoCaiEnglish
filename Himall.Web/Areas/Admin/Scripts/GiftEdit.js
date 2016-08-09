var eidtor;
var dtips;

$(function () {
    initRichTextEditor();
    $("#GiftName").focus();
    dtips = $("#destips");

    //提交前检测
    $("#gifteditform").bind("submit", function (e) {
        var isdataok = true;
        eidtor.sync();
        var des = $("[name='Description']").val();
        if (des.length < 1) {
            dtips.show();
            isdataok= false;
        }
        var pic1 = $("#PicUrl1").val();
        if (pic1.length < 1) {
            $('#up_pic1').css({ border: '1px solid #f60' });
            $('#PicUrl1').focus();
            isdataok= false;
        }
        return isdataok;
    });
});
function initRichTextEditor() {
    eidtor = UE.getEditor('Description');
    eidtor.addListener('contentChange', function () {
        //    editor.sync();
        dtips.hide();
    });
    //同步内容
    eidtor.addListener("blur", function () {
        eidtor.sync();
        var content = eidtor.getContent();
        if (content.length < 1) {
            dtips.show();
        } else {
            dtips.hide();
        }
    })

}