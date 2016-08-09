﻿var platFormCategoryId;
var categoryId;
var brandid;
var paraSaleStatus = -1;
var MaxFileSize = 15728640;//15M
var MaxImportCount = 20;
var RefreshInterval = 2;//Second
var _refreshProcess;//定时器
var loading;
var sellercate1 ;
var sellercate2 ;
$(function () {
    $("#btnFile").bind("change", function () {
        if ($("#btnFile").val() != '') {
            var dom_btnFile = document.getElementById('btnFile');
            if (typeof (dom_btnFile.files) == 'undefined') {
                try {
                    var fso = new ActiveXObject('Scripting.FileSystemObject');
                    var f = fso.GetFile($("#btnFile").val());
                    if (f.Size > MaxFileSize) {
                        $.dialog.tips('选择的文件太大');
                        return;
                    }
                }
                catch (e) {
                    errorTips(e);
                }
            }
            else {
                if (dom_btnFile.files.length > 0 && dom_btnFile.files[0].size > MaxFileSize) {
                    $.dialog.tips('选择的文件太大');
                    return;
                }
            }
            var filename = $("#btnFile").val().substring($("#btnFile").val().lastIndexOf('\\') + 1);
            $('#inputFile').val(filename);
        }
        else {
            $('#inputFile').val('请选择文件');
        }
    });
    $('#btnUpload').bind('click', function () {
        
        sellercate1 = $('#sellercategory1').val() || '';
        sellercate2 = $('#sellercategory2').val() || '';

        if (sellercate1 == '' && sellercate2 == '') {
            $.dialog.tips('请选择一个商品分类');
            return;
        }
        
        
        var filename = $('#inputFile').val();
        if (filename == '请选择文件') {
            $.dialog.tips('请选择文件');
            return;
        }
        else {
            GetImportOpCount();
        }
    });

   

    $('#sellercategory1,#sellercategory2').himallLinkage({
        url: '../Category/getCategory',
        enableDefaultItem: true,
        defaultItemsText: '',
        onChange: function (level, value, text) {
            categoryId = value;
        }
    });

});
function fnUploadFileCallBack(filename) {
    var shopCategory = parseFloat(sellercate1);
    if (sellercate2 != '')
        shopCategory = parseFloat(sellercate2);
    var loading = showLoading();
    $.ajax({
        type: 'post'
        , url: 'ImportProductsFromExcel'
        , data: { shopCategoyId: shopCategory, filename: filename }
        , datatype: 'json'
        , success: function (data) {
            var returnMess = "";
            if (data.message == 1) {
                returnMess = "已经成功导入" + data.SuccessCount + "条数据," + "有" + data.ErrorCount + "条数据有误。";
            }
            else if (data.message == 6) {
                returnMess = "没有找到文件,请重新上传";
            }
            else {
                returnMess = "系统内部异常";
            }
            $('.ajax-loading').remove();
            //art.dialog.alert(returnMess, function () { location.reload(); });
            $.dialog.alert(returnMess);
            window.clearInterval(_refreshProcess);
        }
    });
    
}


function GetImportOpCount() {
    $.ajax({
        type: 'get'
       , url: 'GetImportOpCount'
       , datatype: 'json'
       , success: function (data) {
           if (data.Count >= MaxImportCount) {
               $.dialog.tips('上传人数较多，请稍等。。。');
               return;
           }
           var dom_iframe = document.getElementById('iframe');
           //非IE、IE
           dom_iframe.onload = function () {
               var filename = this.contentDocument.body.innerHTML;
               if (filename != 'NoFile' && filename != 'Error') {
                   fnUploadFileCallBack(filename);//上传文件后，继续导入商品操作
                   $('#inputFile').val('请选择文件');
               }
               else {
                   $.dialog.tips('上传文件异常');
               }
               this.onload = null;
               this.onreadystatechange = null;
           };
           //IE
           dom_iframe.onreadystatechange = function () {
               if (this.readyState == 'complete' || this.readyState == 'loaded') {
                   var filename = this.contentDocument.body.innerHTML;
                   if (filename != 'NoFile' && filename != 'Error') {
                       fnUploadFileCallBack(filename);//上传文件后，继续导入商品操作
                   }
                   else {
                       loading.close();
                       $.dialog.tips('上传文件异常');
                   }
                   this.onload = null;
                   this.onreadystatechange = null;
               }
           };
           loading = showLoading('正在上传');
           $('#formUpload').submit();
       }
    });
}

