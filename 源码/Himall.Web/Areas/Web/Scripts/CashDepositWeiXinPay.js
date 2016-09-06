$(function () {
    checkPayDone();
});

function checkPayDone() {
    var balance = QueryString('balance');
    
    $.getJSON('/SellerAdmin/CashDeposit/WeiXinPayReturn', { balance: balance }, function (result) {
        if (result.success) {
            $.dialog.succeedTips('Pay Success!', function () {

                location.href = "/selleradmin?url=/selleradmin/CashDeposit/Management&tar=CashDeposit";
            });
        }
        else {
            setTimeout(checkPayDone, 0);
        }
    });
    
}