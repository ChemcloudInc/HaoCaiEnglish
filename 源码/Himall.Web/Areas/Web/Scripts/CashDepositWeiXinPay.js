$(function () {
    checkPayDone();
});

function checkPayDone() {
    var balance = QueryString('balance');
    
    $.getJSON('/SellerAdmin/CashDeposit/WeiXinPayReturn', { balance: balance }, function (result) {
        if (result.success) {
            $.dialog.succeedTips('支付成功!', function () {

                location.href = "/selleradmin?url=/selleradmin/CashDeposit/Management&tar=CashDeposit";
            });
        }
        else {
            setTimeout(checkPayDone, 0);
        }
    });
    
}