var opid = '';
function checkScanState() {
    $.getJSON('/ScanState/GetState', { sceneid: sceneid }, function (result) {
        if (result.success) {
            opid = result.data.OpenId;
            $('#nikename').text(result.data.NickName);
            $.dialog.succeedTips('Sweep code success!', function () {
                JumpStep(3);
            });
        }
        else {
            setTimeout(checkScanState, 0);
        }
    });
}
function JumpStep(step)
{
    $('div[name="stepname"]').hide();
    switch(step)
    {
        case 1:
            $('#stepnav').hide();
            $('#step' + step).show();
            break;
        case 2:
            $('#stepnav').show();
            $('#step' + step).show();
            break;
        case 3:
            $('#stepnav').show();
            $('div[name="step3"]').removeClass('todo').addClass('active');
            $('div[name="step3"]').prev().removeClass('active').addClass('done');
            $('#step' + step).show();
            break;
        case 4:
            $('#stepnav').show();
            $('div[name="step4"]').removeClass('todo').addClass('active');
            $('div[name="step4"]').prev().removeClass('active').addClass('done');
            $('#step' + step).show();
            break;
    }
}
