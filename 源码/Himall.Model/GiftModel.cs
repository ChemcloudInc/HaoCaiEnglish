using System;

namespace Himall.Model
{
	public class GiftModel : GiftInfo
	{
		public string ShowSalesStatus
		{
			get
			{
				string str = "有错误";
				if (this != null)
				{
					switch (base.GetSalesStatus)
					{
						case GiftInfo.GiftSalesStatus.IsDelete:
						{
							str = "己删除";
							break;
						}
						case GiftInfo.GiftSalesStatus.OffShelves:
						{
							str = "己下架";
							break;
						}
						case GiftInfo.GiftSalesStatus.Normal:
						{
							str = "可兑换";
							break;
						}
						case GiftInfo.GiftSalesStatus.HasExpired:
						{
							str = "己过期";
							break;
						}
					}
				}
				return str;
			}
		}

		public GiftModel()
		{
		}
	}
}