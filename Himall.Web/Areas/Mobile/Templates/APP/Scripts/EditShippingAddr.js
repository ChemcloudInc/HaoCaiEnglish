

$(function () {
    var id = QueryString("addressId");
    if (id == 0) {
        $('#consignee_province,#consignee_city,#consignee_county').cityLink(0, 0, 0);
    }
    else {
        $.post('/' + areaName + '/order/GetUserShippingAddresses', { addressId: id }, function (addresses) {
            var regionPath = addresses.fullRegionIdPath.split(',');
            $('#consignee_province,#consignee_city,#consignee_county').cityLink(regionPath[0], regionPath[1], regionPath[2]);
        });
    }
})



function saveAddress(id, regionId, shipTo, address, phone, callBack) {
    id = (parseInt(id) == 0) ? '' : parseInt(id);

    var url = '';
    if (id) {
        url = '/' + areaName + '/Member/EditShippingAddress';
        $.post(url, { id: id, RegionId: regionId, Address: address, Phone: phone, ShipTo: shipTo }, function (result) {
            if (result.success) {
                $.dialog.tips('保存成功');
                location.href = decodeURIComponent(QueryString('returnURL'));
            }
            else
                $.dialog.alert('保存失败!' + result.msg);
        });
    }
    else {
        url = '/' + areaName + '/Member/AddShippingAddress';

        $.post(url, { RegionId: regionId, Address: address, Phone: phone, ShipTo: shipTo }, function (result) {
            if (result.success) {
                $.dialog.tips('保存成功');
                location.href = decodeURIComponent(QueryString('returnURL'));
            }
            else
                $.dialog.alert('保存失败!' + result.msg);
        });
    }
}

; (function ($, data) {
    $.fn.cityLink = function (a, b, c) {
        var that = this,
            province = '',//省
            city = '',//市
            area = '',//区域
            i, e,
            createElem = function (data, elem, select, id) {// 创建元素
                if (!data) { return; }
                if (select) {
                    elem.append('<option value="0">请选择</option>');
                } else {
                    elem.append('<option value="0" selected="true">请选择</option>');
                }
                for (var i = 0, e; e = data[i++];) {
                    if (select == e.id) {
                        elem.append('<option value="' + e.id + '" selected="true" ' + (id ? 'data-id="' + id + '"' : '') + '>' + e.name + '</option>');
                    } else {
                        elem.append('<option value="' + e.id + '"' + (id ? 'data-id="' + id + '"' : '') + '>' + e.name + '</option>');
                    }
                }
            },
            fnSelect = function (data, val, tag) {
                if (!data) { return; }
                for (var i = 0, e; e = data[i++];) {
                    if (e.id == val) {
                        return e[tag];
                    }
                }
            },
            fnChange = function (dom, target, tag, bool) {
                dom.change(function (e) {
                    var t = e.target,
                        val = $(this).val(),
                        dataTag = '',
                        temp = '',
                        cityData;
                    if (val != 0) {
                        if (bool) {
                            dataTag = $(this).find("option:selected").attr('data-id');
                            temp = fnSelect(data, dataTag, bool);
                            cityData = fnSelect(temp, val, tag);
                        } else {
                            cityData = fnSelect(data, val, tag);
                        }
                        target.html('');
                        if (tag == 'city') {
                            $('.selected-address i').html($(this).find("option:selected").html() + ' ');
                            $('.selected-address em').html(' ');
                            $('.selected-address s').html(' ');
                        } else {
                            $('.selected-address em').html($(this).find("option:selected").html() + ' ');
                        }
                        createElem(cityData, target, val, val);
                    }
                    return false;
                });
            },
            init = function (a, b, c) {
                $(that[0]).html('');
                if (b == 0) {
                    $(that[1]).html('<option value="0">请选择</option>');
                }
                if (c == 0) {
                    $(that[2]).html('<option value="0">请选择</option>');
                }
                createElem(data, province, a);
                var cityData = fnSelect(data, a, 'city'),
                    areaData = fnSelect(cityData, b, 'county');

                if (province && city) {
                    createElem(cityData, city, b);
                    fnChange(province, city, 'city');//@@province dom //当前点击事件的dom @@city市级dom @@市级字符串用来读取json数据
                }
                if (province && city && area) {
                    createElem(areaData, area, c);
                    fnChange(city, area, 'county', 'city');
                }
                $('#consignee_county').change(function (e) {
                    var id = $(this).val();
                    if (id != 0) {
                        $('.selected-address s').html($(this).find("option:selected").html());
                    }
                });
            };
        that.each(function (i, e) {
            switch (i) {
                case 0: province = $(e); break;
                case 1: city = $(e); break;
                case 2: area = $(e); break;
                default: break;
            }
        });
        init(a, b, c);// 初始化
    };
}(jQuery, province)
);


(function ($, data) {
    var getOption = function (elem, bool) {
        var s, t;
        if (bool) {
            elem.children().each(function (i, e) {
                s = e.selected;
                if (s == true) {
                    t = $(e).html();
                    return;
                }
            });
        } else {
            elem.children().each(function (i, e) {
                s = e.selected;
                if (s == true) {
                    t = $(e).val();
                    return;
                }
            });
        }
        return t;
    };
    $('#saveConsigneeTitleDiv').bind('click', function () {
        var a = getOption($('#consignee_province'), 0),
            b = getOption($('#consignee_city'), 0),
            c = getOption($('#consignee_county'), 0),
            province = getOption($('#consignee_province'), 1),
            city = getOption($('#consignee_city'), 1),
            county = getOption($('#consignee_county'), 1),
            value = a + ',' + b + ',' + c,
            str = province + ' ' + city + ' ' + county,
            indexId = QueryString("addressId");


        var shipTo = $('#shipName').val();
        var regword = /[\w\u4E00-\u9FA5\uF900-\uFA2D]+/;
        var regTel = /([\d]{11}$)|(^0[\d]{2,3}-?[\d]{7,8}$)/;
        var phone = $('#phone').val();
        var address = $('#address').val();
        var regionId = $('#consignee_county').val() || '0';

        if (!regword.test(shipTo)) {
            $.dialog.alert('请填写收货人');
            return false;
        }
        else if (!phone) {
            $.dialog.tips('请填写电话');
            return false;
        }
        else if (!regTel.test(phone)) {
            $.dialog.tips('请填写正确的电话');
            return false;
        }
        else if (!isSelectAddr($('#consignee_province'), $('#consignee_city'), $('#consignee_county'))) {
            $.dialog.tips('请填选择所在地区');
            return false;
        }
        else if (!address)
            $.dialog.alert('请填写详细地址');
        else {
            regionId = regionId == '0' || isNaN(regionId) ? b : regionId;
            saveAddress(indexId, regionId, shipTo, address, phone, function (result) { })

        }
    });
}(jQuery, province));