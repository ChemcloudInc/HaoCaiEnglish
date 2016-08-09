using Himall.Model;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.SellerAdmin.Models
{
	public class HomeModel
	{
		public IQueryable<ArticleInfo> Articles
		{
			get;
			set;
		}

		public string OrderCounts
		{
			get;
			set;
		}

		public string OrderHandlingComplaints
		{
			get;
			set;
		}

		public string OrderProductConsultation
		{
			get;
			set;
		}

		public string OrderReplyComments
		{
			get;
			set;
		}

		public string OrderWaitDelivery
		{
			get;
			set;
		}

		public string OrderWaitPay
		{
			get;
			set;
		}

		public string OrderWithRefund
		{
			get;
			set;
		}

		public string OrderWithRefundAndRGoods
		{
			get;
			set;
		}

		public string ProductAndDescription
		{
			get;
			set;
		}

		public string ProductAndDescriptionPercentage
		{
			get
			{
				int num = (int)(Convert.ToDouble(ProductAndDescription) / 5 * 100);
				return string.Concat(num.ToString(), "%");
			}
		}

		public string ProductsAuditFailed
		{
			get;
			set;
		}

		public string ProductsBrands
		{
			get;
			set;
		}

		public string ProductsEvaluation
		{
			get;
			set;
		}

		public string ProductsInDraft
		{
			get;
			set;
		}

		public string ProductsInfractionSaleOff
		{
			get;
			set;
		}

		public string ProductsInStock
		{
			get;
			set;
		}

		public string ProductsNumber
		{
			get;
			set;
		}

		public string ProductsNumberIng
		{
			get;
			set;
		}

		public string ProductsOnSale
		{
			get;
			set;
		}

		public string ProductsWaitForAuditing
		{
			get;
			set;
		}

		public Himall.Model.SellerConsoleModel SellerConsoleModel
		{
			get;
			set;
		}

		public string SellerDeliverySpeed
		{
			get;
			set;
		}

		public string SellerDeliverySpeedPercentage
		{
			get
			{
				int num = (int)(Convert.ToDouble(SellerDeliverySpeed) / 5 * 100);
				return string.Concat(num.ToString(), "%");
			}
		}

		public string SellerServiceAttitude
		{
			get;
			set;
		}

		public string SellerServiceAttitudePercentage
		{
			get
			{
				int num = (int)(Convert.ToDouble(SellerServiceAttitude) / 5 * 100);
				return string.Concat(num.ToString(), "%");
			}
		}

		public string ShopEndDate
		{
			get;
			set;
		}

		public string ShopGradeName
		{
			get;
			set;
		}

		public long ShopId
		{
			get;
			set;
		}

		public string ShopLogo
		{
			get;
			set;
		}

		public string ShopName
		{
			get;
			set;
		}

		public string UseSpace
		{
			get;
			set;
		}

		public string UseSpaceing
		{
			get;
			set;
		}
        /// <summary>
        /// 待结款金额
        /// </summary>
        public decimal WaitAccount
        {
            get;
            set;
        }
        /// <summary>
        /// 未结款金额
        /// </summary>
        public decimal NoAccount
        {
            get;
            set;
        }
        /// <summary>
        /// 最近结款金额
        /// </summary>
        public decimal LastestAccount
        {
            get;
            set;
        }
        public string LastestFinishData
        {
            get;
            set;
        }

		public HomeModel()
		{
		}
	}
}