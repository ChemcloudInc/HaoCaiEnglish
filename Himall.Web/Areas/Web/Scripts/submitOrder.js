$(function () {
    bindRecieverEdit();
    bindSubmit();
    initAddress();
    bindAddressRadioClick();
    InvoiceInit();
    InvoiceOperationInit();
})


function InvoiceInit()
{
    $( "#invoceMsg" ).hide();
    $( "input[name='isInvoce']" ).click( function ()
    {
        var id = $( this ).attr( "id" );
        if ( id == "isInvoce1" )
        {
            $( "#invoceMsg" ).hide();
        }
        else if ( id == "isInvoce2" )
        {
            $( "#invoceMsg" ).show();

        }
    } )

    $( "#dvInvoice .invoice-list div:eq(0)" ).addClass( "invoice-item-selected" );
    $( "#dvInvoice .invoice-list div" ).click( function ()
    {
        $( "#dvInvoice .invoice-list div" ).removeClass( "invoice-item-selected" );
        $( this ).addClass( "invoice-item-selected" );
    } )

    $( "#dvInvoice .invoice-tit-list div" ).click( function ()
    {
        $( "#dvInvoice .invoice-tit-list div" ).removeClass( "invoice-item-selected" );
        $( this ).addClass( "invoice-item-selected" );
    } )

    $( "#btnAddInvoice" ).click( function ()
    {
        var html = '<div class="invoice-item invoice-item-selected">';
        html += '<input type="text" value="">';
        html += '<div class="item-btns">';
        html += '<a href="javascript:void(0);" class="ml10 update-tit">保存</a>';
        html += '<a href="javascript:void(0);" class="ml10 del-tit hide">删除</a>';
        html += '</div>';
        html += '</div>';

        $( "#dvInvoice .invoice-tit-list .invoice-item" ).removeClass( "invoice-item-selected" );
        $( "#dvInvoice .invoice-tit-list" ).prepend( html );
        $( "#dvInvoice .invoice-tit-list .invoice-item-selected input" )[0].focus();

        InvoiceOperationInit();
    } )

    $( "#btnOk" ).click( function ()
    {
        var title = $( "#dvInvoice .invoice-tit-list .invoice-item-selected input" ).val();
        var context = $( "#dvInvoice .invoice-list .invoice-item-selected span" ).html();
        if ( title == null || context == null )
        {
        
        }
        else if ( title.length > 0 && context.length > 0 )
        {
            $( "#invoiceTitle" ).html( title );
            $( "#invoiceContext" ).html( context );
            $( '.thickbox,.thickdiv' ).hide();
        }
        else
        {
            $.dialog.tips( "请选择发票信息" );
        }
        
    } )
}

function InvoiceOperationInit()
{
    $( "#dvInvoice .invoice-tit-list .del-tit" ).click( function ()
    {
        var self = this;
        var id = $( self ).attr( "key" );
        $.dialog.confirm( "确定删除该发票抬头吗？", function ()
        {
            var loading = showLoading();
            $.post( "./DeleteInvoiceTitle", { id: id }, function ( result )
            {
                loading.close();
                if ( result ==true)
                {
                    $( self ).parents( ".invoice-item" ).remove();
                    $( ".invoice-tit-list .invoice-item:eq(0)" ).addClass( "invoice-item-selected" );
                    $.dialog.tips( '删除成功！' );
                }
                else {
                    $.dialog.tips('删除失败！');
                }
            } )
        } );
    } )

    $( "#dvInvoice .invoice-tit-list .update-tit" ).click( function ()
    {
        var self = this;
        var name = $( this ).parents( ".invoice-item" ).find( "input" ).val();
        if ( $.trim( name ) == "" )
        {
            $.dialog.tips( '不能为空！' );
            return;
        }
        var loading = showLoading();
        $.post( "./SaveInvoiceTitle", { name: name }, function ( result )
        {
            loading.close();
            if ( result != undefined && result != null && result > 0)
            {
                $( self ).parents( ".invoice-item" ).find( ".del-tit" ).removeClass( "hide" ).attr( "key", result );
                $( self ).addClass( "hide" );
                $( self ).parents( ".invoice-item" ).find( "input" ).attr( "disabled", true );
                $( self ).parents( ".invoice-item" ).addClass( "invoice-item-selected" );

                $( "#dvInvoice .invoice-tit-list div" ).click( function ()
                {
                    $( "#dvInvoice .invoice-tit-list div" ).removeClass( "invoice-item-selected" );
                    $( this ).addClass( "invoice-item-selected" );
                } )
                InvoiceOperationInit();
                $.dialog.tips( '保存成功！' );
            }
            else
            {
                $.dialog.tips('保存失败！');
            }
        } )
    } )
}


function initAddress() {
    if (!$('#selectedAddress').html()) {
        $('#editReciever').click();
    }
}

var shippingAddress = [];

function bindRecieverEdit() {
    $('#editReciever').click(function () {
        $.post('/order/GetUserShippingAddresses', {}, function (addresses) {
            var html = '';
            var currentSelectedId = parseInt($('#shippingAddressId').val());
            $.each(addresses, function (i, address) {
                shippingAddress[address.id] = address;
                html += '<div class="item" name="li-' + address.id + '">\
                          <label>\
                             <input type="radio" class="hookbox" name="address" '+ (address.id == currentSelectedId ? 'checked="checked"' : '') + ' value="' + address.id + '" />\
                             <b>' + address.shipTo + '</b>&nbsp; ' + address.fullRegionName + ' &nbsp; ' + address.address + ' &nbsp; ' + address.phone + ' &nbsp\
                          </label>\
                          <span class="item-action">\
                              <a href="javascript:;" onclick="showEditArea(\'' + address.id + '\')">编辑</a> &nbsp;\
                              <a href="javascript:;" onclick="deleteAddress(\'' + address.id + '\')">删除</a>&nbsp;\
                          </span>\
                      </div>';
            });
            $('#consignee-list').html(html).show();
            $('#step-1').addClass('step-current');
            $('#addressListArea').show();

            $('input[name="address"]').change(function () {
                var shippingAddressId = $(this).val();
                $('#shippingAddressId').val(shippingAddressId);
            });
        });
    });
}


function bindAddressRadioClick() {
    $('#consignee-list').on('click', 'input[type="radio"]', function () {
        $('#addressEditArea').hide();

    });
}


function deleteAddress(id) {
    $.dialog.confirm('您确定要删除该收货地址吗？', function () {
        var loading = showLoading();
        $.post('/UserAddress/DeleteShippingAddress', { id: id }, function (result) {
            loading.close();
            if (result.success) {
                var current = $('div[name="li-' + id + '"]');
                if ($('input[type="radio"][name="address"]:checked').val() == id) {
                    $('input[type="radio"][name="address"]').first().click();
                    $('#selectedAddress').html('');
                    $('#shippingAddressId').val('');
                }
                current.remove();
            }
            else
                $.dialog.errorTips(result.msg);
        });
    });
}


function saveAddress(id, regionId, shipTo, address, phone, callBack) {
    id = isNaN(parseInt(id)) ? '' : parseInt(id);

    var url = '';
    if (id)
        url = '/UserAddress/EditShippingAddress';
    else
        url = '/UserAddress/AddShippingAddress';

    var data = {};
    if (id)
        data.id = id;
    data.regionId = regionId;
    data.shipTo = shipTo;
    data.address = address;
    data.phone = phone;


    var loading = showLoading();
    $.post(url, data, function (result) {
        loading.close();
        if (result.success)
            callBack(result);
        else
            $.dialog.errorTips('保存失败!' + result.msg);
    });
}


function bindSubmit() {
    $('#submit').click(function () {
        var fn = function () {
            var arr = [];
            $('.shopb').each(function (i, e) {
                $(e).children().each(function (l, k) {
                    if ($(k).attr('selected')) {
                        var b = $( k ).val();
                        var s = b + "_" + $( k ).attr( "data-type" );
                        arr.push( s );
                    }
                });
            });
            return arr.join(',');
        };
        
        //var skuIds = QueryString('skuIds');改为用隐藏域来获取 zesion修改为
        //var counts = QueryString('counts');
       // ViewBag.collIds = collIds;//组合购商品ID
       // ViewBag.skuIds = skuIds;//sku集合
       // ViewBag.counts = counts;//数量集合

        var skuIds = $("#skuIds").val();
        var collIds = $("#collIds").val();
        var counts = $("#counts").val();


        var action = "SubmitOrder";
        var couponIds = fn();

        var cartItemIds = QueryString('cartItemIds');
        var recieveAddressId = $('#shippingAddressId').val();

        var integral = parseInt($("#integral").val());
        integral = isNaN(integral) ? 0 : integral;

        recieveAddressId = parseInt(recieveAddressId);
        recieveAddressId = isNaN(recieveAddressId) ? null : recieveAddressId;

        $('input:radio[name="sex"]').is(":checked")

        var invoiceType = $( "input[name='isInvoce']:checked" ).val();
        //if ($("input[name='invoiceType']").is(":checked"))
        //    invoiceType = $("input[name='invoiceType']:checked").val();

        var invoiceTitle = $("#invoiceTitle").html();
        if ( invoiceTitle == null || invoiceTitle == '' )
        {
            invoiceTitle = "";
            //$.dialog.tips( '请选择发票抬头' );
            //return false;
        }
        var invoiceContext = $( "#invoiceContext" ).html();
        if ( invoiceContext == null || invoiceContext == '' )
        {
            invoiceContext = "";
            //$.dialog.tips( '请选择发票内容' );
            //return false;
        }

        if ( invoiceType == "2" )
        {
            if ( invoiceTitle == null || invoiceTitle == '' )
            {
                $.dialog.tips( '请选择发票抬头' );
                return false;
            }

            if ( invoiceContext == null || invoiceContext == '' )
            {
                $.dialog.tips( '请选择发票内容' );
                return false;
            }
        }
        //if ($("#isInvoice").attr('checked')) {
        //    if (!$("input[name='invoiceType']").is(":checked")) {
        //        $.dialog.tips('请选择发票类型');
        //        return false;
        //    }
        //    if ( invoiceTitle == null || invoiceTitle == '' )
        //    {
        //        $.dialog.tips('请输入发票抬头');
        //        return false;
        //    }
        //}

        if (!recieveAddressId)
            $.dialog.tips('请选择或新建收货地址');
        else {
            if (skuIds) {
                action = "SubmitOrderByProductId";
            }
            var loading = showLoading();
            $.post( '/order/' + action, { integral: integral, couponIds: couponIds, skuIds: skuIds, counts: counts,collIds:collIds,cartItemIds: cartItemIds, recieveAddressId: recieveAddressId, invoiceType: invoiceType, invoiceTitle: invoiceTitle, invoiceContext: invoiceContext }, function ( result )
            {
                if (result.success) {
                    location.href = '/order/pay?orderIds=' + result.orderIds.toString();
                    loading.close();
                }
                else {
                    loading.close();
                    $.dialog.errorTips(result.msg);
                }
            });
        }
    });
}

function showEditArea(id) {
    $('input[name="address"][value="' + id + '"]').click();

    var address = shippingAddress[id];
    var shipTo = address == null ? '' : address.shipTo;
    var addressName = address == null ? '' : address.address;
    var phone = address == null ? '' : address.phone;
    if (address) {
        var arr = (address.fullRegionName).split(' ');
    }
    //arr[2] = arr[2] || '';
    var fullRegionName = address == null ? '<i></i><em></em><s></s>' : '<i>' + arr[0] + ' </i><em>' + arr[1] + ' </em><s>' + arr[2] + '</s>';

    $('input[name="shipTo"]').val(shipTo);
    $('input[type="text"][name="address"]').val(addressName);
    $('input[name="phone"]').val(phone);
    $('span[name="regionFullName"]').html('');
    $('span[name="regionFullName"]').html(fullRegionName);

    $('#addressEditArea').show();
    if (id === 0) {
        $('#consignee_province').val(0);
        $('#consignee_city').val(0);
        $('#consignee_county').val(0);
        return;
    }
    var regionPath = address.fullRegionIdPath.split(',');
    $('#consignee_province').val(regionPath[0]);
    $('#consignee_province').trigger('change');
    $('#consignee_city').val(regionPath[1]);
    $('#consignee_city').trigger('change');
    if (regionPath.length == 3) {
        $('#consignee_county').val(regionPath[2]);
        $('#consignee_county').trigger('change');
    }
}



; (function ($, data) {
    var getOption = function (elem, bool) {
        var s, t;
        if (bool) {
            elem.children().each(function (i, e) {
                s = e.selected;
                if (s == true && i>0) {
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
    setProvince($('#consignee_province'), $('#consignee_city'), $('#consignee_county'));
    $('#saveConsigneeTitleDiv').bind('click', function () {
        var a = getOption($('#consignee_province'), 0),
            b = getOption($('#consignee_city'), 0),
            c = getOption($('#consignee_county'), 0),
            province = getOption($('#consignee_province'), 1),
            city = getOption($('#consignee_city'), 1),
            county = getOption($('#consignee_county'), 1)|| '',
            value = a + ',' + b + ',' + c,
            str = province + ' ' + city + ' ' + county,
            indexId = $('input[type="radio"][name="address"]:checked').val();



        if ($('#addressEditArea').css('display') == 'none') {
            var selectedAddress = shippingAddress[indexId];

            var newSelectedText = selectedAddress.shipTo + ' &nbsp;&nbsp;&nbsp; ' + selectedAddress.phone + ' &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<br />' + selectedAddress.fullRegionName + ' &nbsp; &nbsp;' + selectedAddress.address + '&nbsp;';
            $('#shippingAddressId').val(indexId);
            $('#addressEditArea').hide();
            $('#selectedAddress').html(newSelectedText);
            $('#addressListArea').hide();
            $('#consignee-list').hide();
            $('#step-1').removeClass('step-current');

            var regionId = $("#shippingAddressId").val();
            var cartItemIds = $("#cartItemIds").val();
            var cartItemIds = window.location.search.replace(/.*cartItemIds=([^&?]+).*/i, "$1");
            if (/cartItemIds/i.test(window.location.search) == false) cartItemIds = undefined;
            if (cartItemIds) {
                window.location.href = "/order/submit?cartItemIds=" + cartItemIds + "&regionId=" + regionId;
            } else {
                if ($('#collIds').val())
                    window.location.href = "/order/SubmitByProductId?skuIds=" + $('#skuIds').val() + "&counts=" + $('#counts').val() + "&collIds=" + $('#collIds').val() + "&regionId=" + regionId;
                else {
                    window.location.href = "/order/SubmitByProductId?skuIds=" + $('#skuIds').val() + "&counts=" + $('#counts').val() + "&regionId=" + regionId;
                }
                //window.location.reload();
            }
            CalcFright();
        }
        else {
            var shipTo = $('input[name="shipTo"]').val();
            var regTel = /([\d]{11}$)|(^0[\d]{2,3}-?[\d]{7,8}$)/;
            var phone = $('input[name="phone"]').val();
            var address = $('input[type="text"][name="address"]').val();
            var regionId = $('#consignee_county').val();

            if (!shipTo) {
                $.dialog.tips('请填写收货人');
                return false;
            }
            else if ( $.trim( shipTo ).length == 0 )
            {
                $.dialog.tips( '请填写收货人' );
                return false;
            }
            else if (!phone) {
                $.dialog.tips('请填写电话');
                return false;
            }
            else if(!regTel.test(phone))
            {
                $.dialog.tips('请填写正确的电话');
                return false;
            }
            else //RegionBind.js
                if (!isSelectAddr($('#consignee_province'), $('#consignee_city'), $('#consignee_county'))) {
                    $.dialog.tips('请填选择所在地区');
                    return false;
                }
                else if (!address) {
                    $.dialog.tips('请填写详细地址');
                    return false;
                }
                else if ( $.trim( address ).length == 0 )
                {
                    $.dialog.tips( '请填写详细地址' );
                    return false;
                }
                else {
                    regionId = regionId == '0' ? b : regionId;
                    saveAddress(indexId, regionId, shipTo, address, phone, function (result) {

                        var newSelectedText = shipTo + ' &nbsp;&nbsp;&nbsp; ' + phone + ' &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<br />' + str + ' &nbsp; &nbsp;' + address + '&nbsp;';
                        $('#selectedAddress').html(newSelectedText);

                        indexId = isNaN(parseInt(indexId)) ? '' : parseInt(indexId);
                        if (indexId == 0) {
                            indexId = result.id;
                            shippingAddress[indexId] = {};
                        }

                        $('#shippingAddressId').val(indexId);
                        $('#addressEditArea').hide();
                        $('#selectedAddress').html(newSelectedText);
                        $('#addressListArea').hide();
                        $('#consignee-list').hide();
                        $('#step-1').removeClass('step-current');
                        if (shippingAddress) {
                            shippingAddress[indexId].fullRegionIdPath = value;
                            shippingAddress[indexId].fullRegionName = str;
                        }

                        var regionId = $("#shippingAddressId").val();
                        var cartItemIds = window.location.search.replace(/.*cartItemIds=([^&?]+).*/i, "$1");
                        if (/cartItemIds/i.test(window.location.search) == false) cartItemIds = undefined;
                        if (cartItemIds) {
                            window.location.href = "/order/submit?cartItemIds=" + cartItemIds + "&regionId=" + regionId;
                        } else {
                            if ($('#collIds').val())
                                window.location.href = "/order/SubmitByProductId?skuIds=" + $('#skuIds').val() + "&counts=" + $('#counts').val() + "&collIds=" + $('#collIds').val() + "&regionId=" + regionId;
                            else {
                                window.location.href = "/order/SubmitByProductId?skuIds=" + $('#skuIds').val() + "&counts=" + $('#counts').val() + "&regionId=" + regionId;
                            }
                        }
                        //CalcFright();

                    });
                }

        }
    });
}(jQuery, province));