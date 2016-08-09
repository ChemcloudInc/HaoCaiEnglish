$(function () {
    //初始化页面
    $('#inputDefFirstUnit').val(initDefFirst);
    $('#inputDefFirstUnitMonry').val(initDefFirstMoney);
    $('#inputDefAccumulationUnit').val(initDefAccumulationUnit);
    $('#inputDefAccumulationUnitMoney').val(initDefAccumulationUnitMoney);
    //初始化商家地址
    setProvince();
    if (sourceAdd != '') {
        var address = sourceAdd.split(',');
        if (address.length == 1) {//省
            $('#provinceDiv').val(address[0]);
        } else {
            if (address.length == 2) {//省市
                $('#provinceDiv').val(address[0]);
                $('#provinceDiv').trigger('change');
                $('#cityDiv').val(address[1]);
            } else {
                if (address.length == 3) {//省市区
                    $('#provinceDiv').val(address[0]);
                    $('#provinceDiv').trigger('change');
                    $('#cityDiv').val(address[1]);
                    $('#cityDiv').trigger('change');
                    $('#countyDiv').val(address[2]);
                }
            }
        }
    }
    $('#radioSelfDef,#radioSellerDef').click(function () {
        SetFreeStatus();
    });
    $('#radioPiece,#radioWeight,#radioBulk').click(function () {
        setUnit();
    });
    //根据计价方式设置单位
    setUnit();
    //根据是否包邮隐藏地区运费
    SetFreeStatus();

    $('#btnSave').click(function () {
        SaveData();
    });
    //新增行
    $('#addCityFreight').click(function () {
        var str = '<tr><td><span class="chooseArea">未添加地区</span><a class="editArea">编辑</a></td><td><input type="text" class="form-control input-xs" value="1"/></td><td><input type="text" class="form-control input-xs" /></td><td><input type="text" class="form-control input-xs" value="1"/></td><td><input type="text" class="form-control input-xs" /></td><td><span class=\"btn-a\"><a name=\"delContent\">删除</a></span></td></tr>';
        $(this).siblings().find('tbody').append(str);
        $('a[name="delContent"]').click(function () {
            $(this).parent().parent().parent().remove();
        });
    });

    $('a[name="delContent"]').click(function () {
        $(this).parent().parent().parent().remove();
    });

    //弹框显示市级
    $(document).on('click', '.operate', function () {
        var cityDiv = $(this).siblings('div');
        if (cityDiv.is(':hidden')) {
            $('.city-box').hide();
            cityDiv.show();
        } else {
            cityDiv.hide();
        }

    });
    if (IsUsed == 1)
    {
        $('input[name="valuationMethod"]').attr('disabled', 'disabled');
        $('#valuationMethodTip').text('已使用，不能修改');
    }
    //弹框关闭市级
    $(document).on('click', '.city .colse', function () {
        $(this).parents('.city-box').hide();
    });

    //指定行编辑操作
    $('.table-area-freight').on('click', '.editArea', function () {
        var areaContent = null;
        if ($("#ulArea").size() == 0) {
            //初始化弹窗内容
            $('.table-area-freight').after("<ul id='ulArea' class='province clearfix'></ul>");
            areaContent = $("#ulArea");
            if (province) {
                var html = "";
                var city = "";
                $(province).each(function (i) {
                    var provinceid = this.id;
                    var provincename = this.name;
                    var operate = this.city != null ? "<b class='operate glyphicon glyphicon-menu-down'></b>" : "";
                    city = "";
                    if (this.city != null) {
                        //加载城市
                        city += "<div class='city-box' id='dvCity_" + i + "'><ul class='city clearfix'><i class='colse'>×</i>";
                        $(this.city).each(function (j) {
                            city += "<li><label><input type='checkbox' id='city_" + this.id + "' />" + this.name + "</label></li>";
                        });
                        city += "</ul></div>";
                    }

                    html += "<li><label><input type='checkbox' class='pro-check' id='province_" + provinceid + "' />" + provincename + "</label><span class='spCount'></span>" + operate + city + "</li>";

                });

                areaContent.html(html);

                $("#ulArea input[type='checkbox']").change(function () {
                    var isprovince = $(this).attr("id").indexOf("province_") > -1;
                    var count = $(this).parent().siblings(".spCount");
                    var cityCheck = $(this).parents('li').find("input:checkbox[id^='city_']");
                    var thisCity = $(this).parents('.city-box');
                    if (isprovince) {
                        //省份
                        if (this.checked)
                            cityCheck.not('.hide').attr("checked", "checked");
                        else
                            cityCheck.removeAttr("checked");
                        count.text("(" + cityCheck.filter('input:checked').length + ")");
                    } else {
                        thisCity.siblings(".spCount").text("(" + cityCheck.filter('input:checked').length + ")");
                    }
                });
            }
        }

        var _this = $(this);

        //弹窗
        $.dialog({
            title: '选择区域',
            lock: true,
            width: 310,
            id: 'logoArea',
            content: $("#ulArea")[0],
            init: function () {
                clearData(_this);
                bindData(_this);
            },
            padding: '20px 10px',
            okVal: '保存',
            ok: function () {
                var data = "", text = "";
                $("#ulArea :checkbox[id^='city_']:checked").each(function () {
                    data += $(this).attr("id").replace("city_", "") + ',';
                    text += $(this).parent().text() + ',';
                });
                _this.siblings('span').html(text.substring(0, text.length - 1)).data('id', data.substring(0, data.length - 1));
            }
        });
    });
});


	

//清空弹框内容并绑定数据
function clearData(cur) {
    var area = $("#ulArea");
    area.find('.spCount').text('');
    area.find('.city-box').hide();
    area.find('input:checked').removeAttr("checked");
	area.find('input.hide').removeClass();
	area.find('li').removeAttr('style');
	
	var strs = new Array();
	var chooseId;
	cur.parents('tr').siblings().find('.chooseArea').each(function(){
		chooseId=$(this).data('id').toString();
		if(chooseId.indexOf(',')>0){
			strs = chooseId.split(",");
			for (i = 0; i < strs.length; i++) {
				$('#city_' + strs[i]).addClass('hide').parent().parent().hide();
			}
		}else{
			$('#city_' + chooseId).addClass('hide').parent().parent().hide();
		}
		
	});
	area.find('.city-box').each(function() {
		if($(this).find('.hide').length==$(this).find('input').length){
			$(this).parent().hide();
		}
	});

}
function bindData(cur) {
    var area = $("#ulArea");
    var dataId = cur.siblings('span').data('id');
    if (dataId != null) {
		if(dataId.toString().indexOf(',')>0){
			var strs = new Array();
			strs = dataId.split(",");
			for (i = 0; i < strs.length; i++) {
				$('#city_' + strs[i]).attr('checked', true);
			}
		}else{
			$('#city_' + dataId).attr('checked', true);
		}
        
    }
    area.find('li').each(function () {
        var len = $(this).find('input:checked').length;
        var AllLen = $(this).find('.city').find('input').length;
        if (len > 0)
            $(this).find('.spCount').text('(' + len + ')');
        if (len == AllLen)
            $(this).find('.pro-check').attr('checked', true);
    });
}
function setUnit() {
    if ($('#radioPiece').attr('checked') == 'checked') {
        $('span[name="ValuationUnitDesc"]').text('件');
        $('span[name="ValuationUnit"]').html('件');
    }
    if ($('#radioWeight').attr('checked') == 'checked') {
        $('span[name="ValuationUnitDesc"]').text('重');
        $('span[name="ValuationUnit"]').html('kg');
    }
    if ($('#radioBulk').attr('checked') == 'checked') {
        $('span[name="ValuationUnitDesc"]').text('体积');
        $('span[name="ValuationUnit"]').html('m<sup>3</sup>');
    }
}
function SetFreeStatus()
{
    if ($('#radioSelfDef').attr('checked') == 'checked') {
        $('#divContent').show();
    }
    else {
        $('#divContent').hide();
    }
}
var freightTempContent = '';
function checkData() {
    if ($('#inputTempName').val()=='')
    {
        $('#inputTempName').focus();
        $.dialog.errorTips('请输入运费模板名称');
        return false;
    }
    if ($('#provinceDiv').val() == '0' && $('#cityDiv').val() == '0' && $('#countyDiv').val() == '0')
    {
        $.dialog.errorTips('请选择商品地址');
        return false;
    }
    freightTempContent = '';
    if ($('#radioSelfDef').attr('checked') == 'checked') {
        //默认运费检查
        var reg = /^[0-9]+([.]{1}[0-9]{1,3})?$/;
        var defFirstUnit = $('#inputDefFirstUnit').val(),
            defFirstUnitMonry = $('#inputDefFirstUnitMonry').val(),
            defAccumulationUnit = $('#inputDefAccumulationUnit').val(),
            defAccumulationUnitMoney = $('#inputDefAccumulationUnitMoney').val();
        if (!reg.test($('#inputDefFirstUnit').val()) || !reg.test($('#inputDefFirstUnitMonry').val()) || !reg.test($('#inputDefAccumulationUnit').val()) || !reg.test($('#inputDefAccumulationUnitMoney').val())) {
            $.dialog.errorTips('默认运费为空或不为数字，请检查');
            return false;
        }
        else {
            if (parseFloat(defFirstUnit) <= 0 || parseFloat(defFirstUnitMonry) <= 0 || parseFloat(defAccumulationUnit) <= 0 || parseFloat(defAccumulationUnitMoney) < 0) {
                $.dialog.errorTips('默认运费不能小于零，请检查');
                return false;
            }
            else {
                freightTempContent = [freightTempContent, '{AreaContent:"",FirstUnit:', defFirstUnit, ',FirstUnitMonry:', defFirstUnitMonry, ',AccumulationUnit:', defAccumulationUnit, ',AccumulationUnitMoney:', defAccumulationUnitMoney, ',IsDefault:1,FreightTemplateId:', tempid, '},'].join('');
            }
        }
        //地区模板运费检查
        var areaContent = '';
        var firstUnit = 0, firstUnitMonry = 0, accumulationUnit = 0, accumulationUnitMoney = 0;
        var errorMessage = '';
        $('.table-area-freight tr').each(function (idx, el) {
            if (idx > 0 && errorMessage == '') {
                areaContent = $('td', el).eq(0).find('span').data('id') || '';
                firstUnit = $('td', el).eq(1).find('input').val() || 0;
                firstUnitMonry = $('td', el).eq(2).find('input').val() || 0;
                accumulationUnit = $('td', el).eq(3).find('input').val() || 0;
                accumulationUnitMoney = $('td', el).eq(4).find('input').val() || 0;
                if (areaContent == '') {
                    errorMessage = '运送地区不能为空，请检查';
                }
                if (!reg.test($('td', el).eq(1).find('input').val()) || !reg.test($('td', el).eq(2).find('input').val()) || !reg.test($('td', el).eq(3).find('input').val()) || !reg.test($('td', el).eq(4).find('input').val())) {
                    //是否非数字
                    errorMessage = '运费只能为数字，请检查';
                }
                else {//是否小于0
                    if (parseFloat(firstUnit) <= 0 || parseFloat(firstUnitMonry) <= 0 || parseFloat(accumulationUnit) <= 0 || parseFloat(accumulationUnitMoney) < 0) {
                        errorMessage = '运费不能小于零，请检查';
                    }
                    else {
                        freightTempContent = [freightTempContent, '{AreaContent:"', areaContent, '",FirstUnit:', firstUnit, ',FirstUnitMonry:', firstUnitMonry, ',AccumulationUnit:', accumulationUnit, ',AccumulationUnitMoney:', accumulationUnitMoney, ',IsDefault:0,FreightTemplateId:', tempid, '},'].join('');
                    }
                }

            }
        });
        if (errorMessage != '') {
            $.dialog.errorTips(errorMessage);
            return false;
        }
        if (freightTempContent.length > 0) {
            freightTempContent = freightTempContent.substr(0, freightTempContent.length - 1);
        }
        freightTempContent = 'Himall_FreightAreaContent:[' + freightTempContent + ']';

    }
    return true;
}
function SaveData() {
    if (checkData())
    {
        var freightTemplate = '';
        freightTemplate = freightTemplate + 'Id:' + tempid;
        freightTemplate = freightTemplate + ',Name:"' + $('#inputTempName').val()+'"';
        var sourceAdd= $('#countyDiv').val() != '0' ? $('#countyDiv').val() : ($('#cityDiv').val() != '0' ? $('#cityDiv').val() : $('#provinceDiv').val());
        freightTemplate = freightTemplate + ',SourceAddress:' + sourceAdd;
        $('input[name="isfree"]').each(function (idx, el) {
            if ($(el).attr('checked') == 'checked')
            {
                freightTemplate = freightTemplate + ',IsFree:' + $(el).attr('status')
            }
        });
        $('input[name="valuationMethod"]').each(function (idx, el) {
            if ($(el).attr('checked') == 'checked') {
                freightTemplate = freightTemplate + ',ValuationMethod:' + $(el).attr('status')
            }
        });
        freightTemplate = freightTemplate + ',SendTime:"' + $("#selsendtime").val() + '"';
        freightTemplate = freightTemplate + ',ShopID:' + shopid;

        freightTemplate = '{' + freightTemplate + ',' + freightTempContent + '}';
        var loading = showLoading();
        $.post('SaveTemplate', { templateinfo: freightTemplate }, function (result) {
            loading.close();
            if (result.successful)
            {
                $.dialog.succeedTips('保存成功！');
                if (window.top.location.href.toLowerCase().indexOf('tar=freighttemplate')>0)
                {
                    if (window.top.opener && window.top.opener.BindfreightTemplate)
                    {
                        window.top.opener.BindfreightTemplate();
                    }
                    window.top.close();
                }
            }
        });
    }
}