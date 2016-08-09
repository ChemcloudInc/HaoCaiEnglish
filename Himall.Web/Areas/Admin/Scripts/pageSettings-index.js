$(function () {
    InitUpload();
    SetLogo();
    SetKeyWords();
    SetRecommand();
});

function InitUpload() {
    $("#uploadImg").himallUpload(
   {
       displayImgSrc: logo,
       imgFieldName: "Logo",
       title: 'LOGO:',
       imageDescript: '270*60',
       dataWidth: 8
   });
}

//设置LOGO
function SetLogo() {
    $('.logo-area').click(function () {
        $.dialog({
            title: 'LOGO设置',
            lock: true,
            width: 350,
            id: 'logoArea',
            content: document.getElementById("logosetting"),
            padding: '20px 10px',
            okVal: '保存',
            ok: function () {
                var logosrc = $("input[name='Logo']").val();
                if (logosrc == "") {
                    $.dialog.tips("请上传一张LOGO图片！");
                    return false;
                }
                var loading = showLoading();
                $.post(setlogoUrl, { logo: logosrc },
                    function (data) {
                        loading.close();
                        if (data.success) {
                            $.dialog.succeedTips("LOGO修改成功！");
                            $("input[name='Logo']").val(data.logo);
                            logo = data.logo;
                        }
                        else { $.dialog.errorTips("LOGO修改失败！") }
                    });
            }
        });
    });
}


//热门关键字设置
function SetKeyWords() {
    $('.search-area').click(function () {
        $("#txtkeyword").val(keyword);
        $("#txthotkeywords").val(hotkeywords);
        $.dialog({
            title: '热门关键字设置',
            lock: true,
            id: 'searchArea',
            content: document.getElementById("keywordsSettting"),
            okVal: '保存',
            ok: function () {
                var word = $("#txtkeyword").val();
                var words = $("#txthotkeywords").val();
                if (word == "" || words == "") {
                    $.dialog.tips("请填写关键字！");
                    return false;
                }
                var loading = showLoading();
                $.post(setkeyWords, { keyword: word, hotkeywords: words },
                     function (data) {
                         loading.close();
                         if (data.success) {
                             $.dialog.succeedTips("关键字设置成功！");
                             logo = data.logo;
                             keyword = word;
                             hotkeywords = words;
                         }
                         else { $.dialog.succeedTips(data.msg); }
                     });
            }
        });
    });
}

//LOGO设置

function SetRecommand() {

    //橱窗1编辑
    $('.imageAdRecommend').click(function () {
        var that = this;
        var thisPic = $(this).attr('pic');
        var thisUrl = $(this).attr('url');
        var value = $(this).attr('value');
        var imageDescript = "190*240";

        switch (parseInt(value)) {
            case 1:
            case 8:
                imageDescript = "464*288";
                break;
            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
            case 7:
                imageDescript = "226*288";
                break;
            case 13:
                imageDescript = "464*288";
                break;

        }
        $.dialog({
            title: '推荐商品编辑',
            lock: true,
            width: 360,
            id: 'goodsArea',
            content: ['<div class="dialog-form">',
                '<div id="HandSlidePic" class="form-group upload-img clearfix">',
                '</div>',
                '<div class="form-group clearfix">',
                    '<label class="label-inline" for="">跳转链接</label>',
                    '<input class="form-control input-sm" type="text" id="url">',
                '</div>',
            '</div>'].join(''),
            okVal: '保存',
            init: function () {
                $("#HandSlidePic").himallUpload(
                {
                    title: '显示图片',
                    imageDescript: imageDescript,
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
                if (url.length === 0) { $("#url").focus(); $.dialog.errorTips('链接地址不能为空.'); return valida; }
                if (pic.length === 0) { $.dialog.errorTips('图片不能为空.'); return valida; }
                var loading = showLoading();
                ajaxRequest({
                    type: 'POST',
                    url: './SlideAd/UpdateImageAd',
                    cache: false,
                    param: { url: url, pic: pic, id: id },
                    dataType: 'json',
                    success: function (data) {
                        loading.close();
                        if (data.successful == true) {
                            $.dialog.succeedTips("推荐商品修改成功！", function () { location.reload(); });
                        }
                    },
                    error: function (data) {
                        loading.close();
                        $.dialog.errorTips('操作失败,请稍候尝试.');
                    }
                });
            }
        });
    });
}