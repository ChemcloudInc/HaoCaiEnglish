using Himall.PaymentPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;


namespace Himall.Plugin.Payment.PayPal
{
    public class Config : ConfigBase
    {
        /// <summary>
        /// PaymentMethod.PaypalStandard.PTIIdentityToken
        /// </summary>
        public string PDTId
        {
            get;
            set;
        }
        /// <summary>
        /// PaymentMethod.Paypalstandard.BusinessEmail
        /// </summary>
        public string BusinessEmail
        {
            get;
            set;
        }
        public string Logo
        {
            get;
            set;
        }
        public string UseSandBox
        {
            get;
            set;
        }
        [XmlIgnore]
        public static string PluginListUrl
        {
            get;
            set;
        }
         
        public Config()
        {
        }
    }
}
