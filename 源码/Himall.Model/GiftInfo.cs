using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class GiftInfo : BaseModel
	{
		private long _id;

		public DateTime? AddDate
		{
			get;
			set;
		}

		public string Description
		{
			get;
			set;
		}

		public DateTime EndDate
		{
			get;
			set;
		}

		[NotMapped]
		public GiftInfo.GiftSalesStatus GetSalesStatus
		{
			get
			{
				GiftInfo.GiftSalesStatus salesStatus = SalesStatus;
				if (salesStatus == GiftInfo.GiftSalesStatus.Normal && EndDate < DateTime.Now)
				{
					salesStatus = GiftInfo.GiftSalesStatus.HasExpired;
				}
				return salesStatus;
			}
		}

		public string GiftName
		{
			get;
			set;
		}

		public decimal? GiftValue
		{
			get;
			set;
		}

		public int GradeIntegral
		{
			get;
			set;
		}

		public new long Id
		{
			get
			{
				return _id;
			}
			set
			{
                _id = value;
				base.Id = value;
			}
		}

		public string ImagePath
		{
			get;
			set;
		}

		public int LimtQuantity
		{
			get;
			set;
		}

		public int NeedGrade
		{
			get;
			set;
		}

		[NotMapped]
		public string NeedGradeName
		{
			get;
			set;
		}

		public int NeedIntegral
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

		[NotMapped]
		public string ShowImagePath
		{
			get
			{
				return string.Concat(ImageServerUrl, ImagePath);
			}
			set
			{
				if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(ImageServerUrl))
				{
                    ImagePath = value;
					return;
				}
                ImagePath = value.Replace(ImageServerUrl, "");
			}
		}

		[NotMapped]
		public string ShowLimtQuantity
		{
			get
			{
				string str = "";
				str = (LimtQuantity != 0 ? LimtQuantity.ToString() : "不限");
				return str;
			}
		}

		public int StockQuantity
		{
			get;
			set;
		}

		public long SumSales
		{
			get
			{
				return VirtualSales + RealSales;
			}
		}

		public int VirtualSales
		{
			get;
			set;
		}

		public GiftInfo()
		{
		}

		public string GetImage(GiftInfo.ImageSize imageSize, int imageIndex = 1)
		{
			return string.Format(string.Concat(ShowImagePath, "/{0}_{1}.png"), imageIndex, (int)imageSize);
		}

		public enum GiftSalesStatus
		{
			[Description("删除")]
			IsDelete = -1,
			[Description("下架")]
			OffShelves = 0,
			[Description("正常")]
			Normal = 1,
			[Description("过期")]
			HasExpired = 2
		}

		public enum ImageSize
		{
			[Description("50×50")]
			Size_50 = 50,
			[Description("100×100")]
			Size_100 = 100,
			[Description("150×150")]
			Size_150 = 150,
			[Description("220×220")]
			Size_220 = 220,
			[Description("350×350")]
			Size_350 = 350,
			[Description("400×400")]
			Size_400 = 400,
			[Description("500×500")]
			Size_500 = 500
		}
	}
}