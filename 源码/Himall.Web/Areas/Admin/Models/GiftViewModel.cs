using Himall.Model;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.Admin.Models
{
	public class GiftViewModel
	{
		public DateTime? AddDate
		{
			get;
			set;
		}

		[Required(ErrorMessage="请填写礼品描述")]
		public string Description
		{
			get;
			set;
		}

		[Required(ErrorMessage="请填写兑换截止时间")]
		public DateTime EndDate
		{
			get;
			set;
		}

		[MaxLength(100, ErrorMessage="超过100字符")]
		[Required(ErrorMessage="请填写礼品名称")]
		public string GiftName
		{
			get;
			set;
		}

		[Range(0.01, 99999.99, ErrorMessage="礼品价值应在0.01-99999之间")]
		[Required(ErrorMessage="请填写礼品价值")]
		public decimal? GiftValue
		{
			get;
			set;
		}

		[Required]
		public long Id
		{
			get;
			set;
		}

		public string ImagePath
		{
			get;
			set;
		}

		[Range(0, 2147483647, ErrorMessage="限兑数量有误,0表示不限")]
		[Required(ErrorMessage="请填写限兑数量,0表不限")]
		public int LimtQuantity
		{
			get;
			set;
		}

		[Required(ErrorMessage="请选择会员等级要求")]
		public int NeedGrade
		{
			get;
			set;
		}

		[Range(10, 2147483647, ErrorMessage="积分必须大于10")]
		[Required(ErrorMessage="请填写所需积分")]
		public int NeedIntegral
		{
			get;
			set;
		}

		public string PicUrl1
		{
			get;
			set;
		}

		public string PicUrl2
		{
			get;
			set;
		}

		public string PicUrl3
		{
			get;
			set;
		}

		public string PicUrl4
		{
			get;
			set;
		}

		public string PicUrl5
		{
			get;
			set;
		}

		public int RealSales
		{
			get;
			set;
		}

		public GiftInfo.GiftSalesStatus SalesStatus
		{
			get;
			set;
		}

		public int Sequence
		{
			get;
			set;
		}

		[Range(0, 2147483647, ErrorMessage="库存必须大于0")]
		[Required(ErrorMessage="请填写库存数量")]
		public int StockQuantity
		{
			get;
			set;
		}

		[Required(ErrorMessage="请填写虚拟销量")]
		public int VirtualSales
		{
			get;
			set;
		}

		public GiftViewModel()
		{
		}
	}
}