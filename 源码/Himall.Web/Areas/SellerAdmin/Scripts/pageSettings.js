$(function () {

    imageAdsClickEventBind();
    InitUpload();
    SetLogo();
});

function InitUpload() {
    $("#uploadImg").himallUpload(
   {
       displayImgSrc: logo,
       imgFieldName: "Logo",
       title: 'LOGO:',
       imageDescript: '150*120',
       dataWidth: 8
   });
}

//设置LOGO
function SetLogo() {
    $('.logo-area').click(function () {
        $.dialog({
            title: 'LOGO设置',
            lock: true,
            width: 620,
            id: 'logoArea',
            content: document.getElementById("logosetting"),
            padding: '20px 10px',
            okVal: 'Save',
            ok: function () {
                var logosrc = $("input[name='Logo']").val();
                if (logosrc == "") {
                    $.dialog.tips("Please upload a LOGO picture!");
                    return false;
                }
                var loading = showLoading();
                $.post(setlogoUrl, { logo: logosrc },
                    function (data) {
                        loading.close();
                        if (data.success) {
                            $.dialog.succeedTips("LOGO modified successfully!");
                            $("input[name='Logo']").val(data.logo);
                            logo = data.logo;
                        }
                        else { $.dialog.errorTips("LOGO modified failed!") }
                    });
            }
        });
    });
}

function imageAdsClickEventBind() {

    $('a[type="imageAd"]').click(function () {
        var that = this;
        var thisPic = $(this).attr('pic');
        var thisUrl = $(this).attr('url');
        $.dialog({
            title: '',
            lock: true,
            width: 620,
            id: 'goodsArea',
            content: ['<div class="dialog-form">',
                '<div id="HandSlidePic" class="form-group upload-img clearfix">',
                '</div>',
                '<div class="form-group">',
                    '<label class="label-inline" for="">Link</label>',
                    '<input class="form-control input-sm" type="text" id="url">',
                '</div>',
            '</div>'].join(''),
            okVal: 'Save',
            init: function () {
                $("#HandSlidePic").himallUpload(
                {
                    title: 'Please upload photos',
                    imageDescript: $(that).attr("imageDescript"),
                    displayImgSrc: thisPic,
                    imgFieldName: "HandSlidePic",
					dataWidth: 8
                });
                $("#url").val(thisUrl);
            },
            ok: function () {
                var valida = false;
                var id = parseInt($(that).attr('value'));
                var url = $("#url").val();
                var pic = $("#HandSlidePic").himallUpload('getImgSrc');
                if (url.length === 0) { $("#url").focus(); $.dialog.errorTips('Link address can not be empty.'); return valida; }
                if (pic.length === 0) { $.dialog.errorTips('Pictures can not be empty.'); return valida; }
                var loading = showLoading();
                $.ajax({
                    type: "POST",
                    url: "UpdateImageAd",
                    data: { url: url, pic: pic, id: id },
                    dataType: "json",
                    async:false,
                    success: function (data) {
                        loading.close();
                        if (data.success) {
                            $(that).attr('pic', data.imageUrl);
                            $(that).attr('url', url);
                            $.dialog.tips('Save successfully!');
                        }
                        else {
                            $.dialog.errorTips('Save failed!' + data.msg);
                            return false;
                        }
                    },
                    error: function (data) {
                        loading.close(); $.dialog.errorTips('Operation failed, please wait to try.');
                    }
                });
            }
        });
    });
}

