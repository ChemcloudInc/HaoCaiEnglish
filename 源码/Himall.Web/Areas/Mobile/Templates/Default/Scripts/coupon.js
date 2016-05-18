//个人优惠卷管理
$(function () {
    $('#spanEnable').click(function () {
        displayCoupon('enabled');
    });
    $('#spanDisable').click(function () {
        displayCoupon('disabled');
    });
});

function displayCoupon(type) {
    if (type=='disabled')
    {
        $('#spanEnable').removeClass('active');
        $('#spanDisable').addClass('active');
        $('.coupon-bd ul[name="disabled"]').show();
        $('.coupon-bd ul[name="enabled"]').hide();
    }
    else {
        $('#spanEnable').addClass('active');
        $('#spanDisable').removeClass('active');
        $('.coupon-bd ul[name="disabled"]').hide();
        $('.coupon-bd ul[name="enabled"]').show();
    }
};
