using Himall.Plugin.Payment.PayPal.PayPalSvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Himall.Plugin.Payment.PayPal
{
    public class PaypalHelper
    {
        public static CurrencyCodeType GetPaypalCurrency( )
        {
            CurrencyCodeType currencyCodeType = CurrencyCodeType.USD;
            return currencyCodeType;
        }
        public static bool CheckSuccess(AbstractResponseType abstractResponse, out string errorMsg)
        {
            bool success = false;
            StringBuilder sb = new StringBuilder();
            switch (abstractResponse.Ack)
            {
                case AckCodeType.Success:
                case AckCodeType.SuccessWithWarning:
                    success = true;
                    break;
                default:
                    break;
            }
            if (null != abstractResponse.Errors)
            {
                foreach (ErrorType errorType in abstractResponse.Errors)
                {
                    if (sb.Length <= 0)
                    {
                        sb.Append(Environment.NewLine);
                    }
                    sb.Append("LongMessage: ").Append(errorType.LongMessage).Append(Environment.NewLine);
                    sb.Append("ShortMessage: ").Append(errorType.ShortMessage).Append(Environment.NewLine);
                    sb.Append("ErrorCode: ").Append(errorType.ErrorCode).Append(Environment.NewLine);
                }
            }
            errorMsg = sb.ToString();
            return success;
        }
    }
}
