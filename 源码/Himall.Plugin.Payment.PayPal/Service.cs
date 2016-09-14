using Himall.Core;
using Himall.Core.Plugins;
using Himall.Core.Plugins.Payment;
using Himall.PaymentPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Himall.Plugin.Payment.PayPal
{
    public class Service : PaymentBase<Config>, IPaymentPlugin, IPlugin
    {
        private string _logo;

        public string HelpImage
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Logo
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_logo))
                {
                    _logo = Utility<Config>.GetConfig(base.WorkDirectory).Logo;
                }
                return _logo;
            }
            set
            {
                _logo = value;
            }
        }

        public string PluginListUrl
        {
            set
            {
                Config.PluginListUrl = value;
            }
        }

        public UrlType RequestUrlType
        {
            get
            {
                return 0;
            }
        }

        public Service()
        {
        }

        public void CheckCanEnable()
        {
            Config config = Utility<Config>.GetConfig(base.WorkDirectory);
            if (string.IsNullOrWhiteSpace(config.BusinessEmail))
            {
                throw new PluginConfigException("Paypal Standard business Email is empty");
            }
            if (string.IsNullOrWhiteSpace(config.PDTId))
            {
                throw new PluginConfigException("Paypal Standard PTD Id is empty");
            }
            if (string.IsNullOrWhiteSpace(config.UseSandBox))
            {
                throw new PluginConfigException("UseSandBox is require");
            }
        }
        private string GetPaypalUrl()
        {
            Config config = Utility<Config>.GetConfig(base.WorkDirectory);
            return  config.UseSandBox=="1"? "https://www.sandbox.paypal.com/us/cgi-bin/webscr" :"https://www.paypal.com/us/cgi-bin/webscr";
        }
        public string ConfirmPayResult()
        {
            return "success";
        }
        public FormData GetFormData()
        {
            Config config = Utility<Config>.GetConfig(base.WorkDirectory);
            FormData formDatum = new FormData();
            FormData.FormItem[] formItemArray = new FormData.FormItem[2];

            FormData.FormItem formItem = new FormData.FormItem();
            formItem.DisplayName = "BusinessEmail";
            formItem.Name = "BusinessEmail";
            formItem.IsRequired = true;
            formItem.Type = (FormData.FormItemType)1;
            formItem.Value = (config.BusinessEmail);
            formItemArray[0] = formItem;

            FormData.FormItem formItem1 = new FormData.FormItem();
            formItem1.DisplayName = ("PDTId");
            formItem1.Name = ("PDTId");
            formItem1.IsRequired = (true);
            formItem1.Type = (FormData.FormItemType)(1);
            formItem1.Value = (config.PDTId);
            formItemArray[1] = formItem1;

            formDatum.Items = (formItemArray);
            return formDatum;
        }

         public void SetFormValues(IEnumerable<KeyValuePair<string, string>> values)
        {
            KeyValuePair<string, string> keyValuePair = values.FirstOrDefault<KeyValuePair<string, string>>((KeyValuePair<string, string> item) => item.Key == "BusinessEmail");
            if (string.IsNullOrWhiteSpace(keyValuePair.Value))
            {
                throw new PluginConfigException("Paypal Standard business Email is empty");
            }
            KeyValuePair<string, string> keyValuePair1 = values.FirstOrDefault<KeyValuePair<string, string>>((KeyValuePair<string, string> item) => item.Key == "PDTId");
            if (string.IsNullOrWhiteSpace(keyValuePair1.Value))
            {
                throw new PluginConfigException("Paypal Standard PTD Id is empty");
            }
            Config config = Utility<Config>.GetConfig(base.WorkDirectory);
            config.PDTId = keyValuePair1.Value;
            config.BusinessEmail = keyValuePair.Value;
            Utility<Config>.SaveConfig(config, base.WorkDirectory);
        }
         public string GetRequestUrl(string returnUrl, string notifyUrl, string orderId, decimal totalFee, string productInfo, string openId = null)
         {
             string empty = string.Empty;
             StringBuilder builder = new StringBuilder();
             Config config = Utility<Config>.GetConfig(base.WorkDirectory);
             builder.Append(GetPaypalUrl());
             builder.AppendFormat("?cmd=_xclick&business={0}", HttpUtility.UrlEncode(config.BusinessEmail));
             builder.AppendFormat("&item_name=Order Number {0}", orderId);
             builder.AppendFormat("&custom={0}", orderId);
             builder.AppendFormat("&amount={0}", totalFee.ToString("N"));
             builder.Append(string.Format("&no_note=1&currency_code={0}", HttpUtility.UrlEncode("USD")));
             builder.AppendFormat("&invoice={0}", orderId);
             builder.AppendFormat("&rm=2", new object[0]);
             
             if (order.ShippingStatus != ShippingStatusEnum.ShippingNotRequired)
                 builder.AppendFormat("&no_shipping=2", new object[0]);
             else
                 builder.AppendFormat("&no_shipping=1", new object[0]);
             builder.AppendFormat("&return={0}&cancel_return={1}", HttpUtility.UrlEncode(returnUrl), HttpUtility.UrlEncode(notifyUrl));
             builder.AppendFormat("&first_name={0}", HttpUtility.UrlEncode(order.BillingFirstName));
             builder.AppendFormat("&last_name={0}", HttpUtility.UrlEncode(order.BillingLastName));
             builder.AppendFormat("&address1={0}", HttpUtility.UrlEncode(order.BillingAddress1));
             builder.AppendFormat("&address2={0}", HttpUtility.UrlEncode(order.BillingAddress2));
             builder.AppendFormat("&city={0}", HttpUtility.UrlEncode(order.BillingCity));
             StateProvince billingStateProvince = StateProvinceManager.GetStateProvinceById(order.BillingStateProvinceId);
             if (billingStateProvince != null)
                 builder.AppendFormat("&state={0}", HttpUtility.UrlEncode(billingStateProvince.Abbreviation));
             else
                 builder.AppendFormat("&state={0}", HttpUtility.UrlEncode(order.BillingStateProvince));
             Country billingCountry = CountryManager.GetCountryById(order.BillingCountryId);
             if (billingCountry != null)
                 builder.AppendFormat("&country={0}", HttpUtility.UrlEncode(billingCountry.TwoLetterIsoCode));
             else
                 builder.AppendFormat("&country={0}", HttpUtility.UrlEncode(order.BillingCountry));
             builder.AppendFormat("&Email={0}", HttpUtility.UrlEncode(order.BillingEmail));
             HttpContext.Current.Response.Redirect(builder.ToString());
             return empty;
         }
    }
}
