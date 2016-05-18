using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.SellerAdmin.Models
{
	public class ShopProfileStep2
	{
		[Required(ErrorMessage="必须填写银行开户名")]
		public string BankAccountName
		{
			get;
			set;
		}

		[Required(ErrorMessage="必须填写公司银行账号")]
		public string BankAccountNumber
		{
			get;
			set;
		}

		[Required(ErrorMessage="必须填写支行联行号")]
		public string BankCode
		{
			get;
			set;
		}

		[Required(ErrorMessage="必须填写开户银行支行名称")]
		public string BankName
		{
			get;
			set;
		}

		[Required(ErrorMessage="必须上传开户银行许可证电子版")]
		public string BankPhoto
		{
			get;
			set;
		}

		[Required(ErrorMessage="必须填写开户银行所在地")]
		public int BankRegionId
		{
			get;
			set;
		}

		[Required(ErrorMessage="必须填写纳税人识别号")]
		public string TaxpayerId
		{
			get;
			set;
		}

		[Required(ErrorMessage="必须填写税务登记证号")]
		public string TaxRegistrationCertificate
		{
			get;
			set;
		}

		[Required(ErrorMessage="必须填写税务登记证号电子版")]
		public string TaxRegistrationCertificatePhoto
		{
			get;
			set;
		}

		public ShopProfileStep2()
		{
		}
	}
}