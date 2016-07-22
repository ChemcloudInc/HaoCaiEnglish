using Himall.Core;
using Himall.Core.Helper;
using Himall.Entity;
using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using Himall.ServiceProvider;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Himall.Service
{
	internal class ShopBonusService : ServiceBase, IShopBonusService, IService, IDisposable
	{
		private WXCardLogInfo.CouponTypeEnum ThisCouponType = WXCardLogInfo.CouponTypeEnum.Bonus;

		private IWXCardService ser_wxcard;

		private Random _random = new Random((int)(DateTime.Now.Ticks & -1) | (int)(DateTime.Now.Ticks >> 32));

		public ShopBonusService()
		{
            ser_wxcard = Instance<IWXCardService>.Create;
		}

		public void Add(ShopBonusInfo model, long shopid)
		{
			ShopBonusInfo shopBonusInfo = (
				from p in context.ShopBonusInfo
				where p.ShopId == shopid
				select p).FirstOrDefault();
			if (shopBonusInfo != null && !shopBonusInfo.IsInvalid && shopBonusInfo.DateEnd > DateTime.Now && shopBonusInfo.DateStart < DateTime.Now)
			{
				throw new HimallException("一个时间段只能新增一个随机红包");
			}
			DateTime dateTime = model.DateEnd.AddHours(23);
			DateTime dateTime1 = dateTime.AddMinutes(59);
			model.DateEnd = dateTime1.AddSeconds(59);
			DateTime dateTime2 = model.BonusDateEnd.AddHours(23);
			DateTime dateTime3 = dateTime2.AddMinutes(59);
			model.BonusDateEnd = dateTime3.AddSeconds(59);
			model.ShopId = shopid;
			model.IsInvalid = false;
			model.ReceiveCount = new int?(0);
			model.QRPath = "";
			model = context.ShopBonusInfo.Add(model);
            context.Configuration.ValidateOnSaveEnabled = false;
            context.SaveChanges();
            context.Configuration.ValidateOnSaveEnabled = false;
            context.SaveChanges();
			if (model.SynchronizeCard)
			{
				WXCardLogInfo wXCardLogInfo = new WXCardLogInfo()
				{
					CardColor = model.CardColor,
					CardTitle = model.CardTitle,
					CardSubTitle = model.CardSubtitle,
					CouponType = new WXCardLogInfo.CouponTypeEnum?(ThisCouponType),
					CouponId = new long?(model.Id),
					ShopId = model.ShopId,
					Quantity = 0
				};
				string str = model.RandomAmountStart.ToString("F2");
				decimal randomAmountEnd = model.RandomAmountEnd;
				wXCardLogInfo.DefaultDetail = string.Concat(str, "元", randomAmountEnd.ToString("F2"), "元随机优惠券1张");
				wXCardLogInfo.LeastCost = (model.UseState == ShopBonusInfo.UseStateType.None ? 0 : (int)(model.UsrStatePrice * new decimal(100)));
				wXCardLogInfo.BeginTime = model.BonusDateStart.Date;
				DateTime dateTime4 = model.BonusDateEnd.AddDays(1);
				wXCardLogInfo.EndTime = dateTime4.AddMinutes(-1);
				Instance<IWXCardService>.Create.Add(wXCardLogInfo);
			}
		}

		public long GenerateBonusDetail(ShopBonusInfo model, long createUserid, long orderid, string receiveurl)
		{
			if (model.DateEnd <= DateTime.Now || model.IsInvalid)
			{
				Log.Info(string.Concat("此活动已过期 , shopid = ", model.ShopId));
				return 0;
			}
			if (model.DateStart > DateTime.Now)
			{
				Log.Info(string.Concat("此活动未开始 , shopid = ", model.ShopId));
				return 0;
			}
			ShopBonusGrantInfo shopBonusGrantInfo = null;
			try
			{
				shopBonusGrantInfo = (
					from p in context.ShopBonusGrantInfo
					where p.OrderId == orderid
					select p).FirstOrDefault();
				if (!context.ShopBonusGrantInfo.Exist<ShopBonusGrantInfo>((ShopBonusGrantInfo p) => p.OrderId == orderid))
				{
					shopBonusGrantInfo = new ShopBonusGrantInfo()
					{
						ShopBonusId = model.Id,
						UserId = createUserid,
						OrderId = orderid,
						BonusQR = ""
					};
					shopBonusGrantInfo = context.ShopBonusGrantInfo.Add(shopBonusGrantInfo);
                    context.SaveChanges();
					long id = shopBonusGrantInfo.Id;
					string str = GenerateQR(Path.Combine(receiveurl, id.ToString()));
					shopBonusGrantInfo.BonusQR = str;
					List<ShopBonusReceiveInfo> shopBonusReceiveInfos = new List<ShopBonusReceiveInfo>();
					for (int i = 0; i < model.Count; i++)
					{
						decimal num = GenerateRandomAmountPrice(model.RandomAmountStart, model.RandomAmountEnd);
						ShopBonusReceiveInfo shopBonusReceiveInfo = new ShopBonusReceiveInfo()
						{
							BonusGrantId = shopBonusGrantInfo.Id,
							Price = new decimal?(num),
							OpenId = null,
							State = ShopBonusReceiveInfo.ReceiveState.NotUse
						};
						shopBonusReceiveInfos.Add(shopBonusReceiveInfo);
					}
                    context.ShopBonusReceiveInfo.AddRange(shopBonusReceiveInfos);
                    context.SaveChanges();
				}
				else
				{
					Log.Info(string.Concat("此活动已存在防止多次添加 , shopid = ", model.ShopId));
					return shopBonusGrantInfo.Id;
				}
			}
			catch (Exception exception)
			{
				Log.Info("错误：", exception);
                context.ShopBonusReceiveInfo.OrderBy((ShopBonusReceiveInfo p) => p.Himall_ShopBonusGrant.ShopBonusId == model.Id);
                context.ShopBonusGrantInfo.OrderBy((ShopBonusGrantInfo p) => p.ShopBonusId == model.Id);
                context.ShopBonusInfo.OrderBy((ShopBonusInfo p) => p.Id == model.Id);
                context.SaveChanges();
			}
			return shopBonusGrantInfo.Id;
		}

		private string GenerateQR(string path)
		{
			Bitmap bitmap = QRCodeHelper.Create(path);
			Guid guid = Guid.NewGuid();
			string str = string.Concat(guid.ToString(), ".jpg");
			string str1 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Storage", "Shop", "Bonus");
			string str2 = Path.Combine(str1, str);
			if (!Directory.Exists(str1))
			{
				Directory.CreateDirectory(str1);
			}
			bitmap.Save(str2);
			return string.Concat("/Storage/Shop/Bonus/", str);
		}

		private decimal GenerateRandomAmountPrice(decimal start, decimal end)
		{
			if (start == end)
			{
				return start;
			}
			decimal num = _random.Next((int)start, (int)end);
			string str = string.Format("{0:N2} ", start);
			string str1 = string.Format("{0:N2} ", end);
			str = str.Substring(str.IndexOf('.') + 1, 2);
			str1 = str1.Substring(str1.IndexOf('.') + 1, 2);
			if (int.Parse(str) == 0)
			{
				str = "1";
			}
			if (int.Parse(str1) < int.Parse(str))
			{
				str1 = "100";
			}
			decimal num1 = _random.Next(int.Parse(str), int.Parse(str1)) / new decimal(100);
			num = num + num1;
			return num;
		}

		public PageModel<ShopBonusInfo> Get(long shopid, string name, int state, int pageIndex, int pageSize)
		{
			IQueryable<ShopBonusInfo> shopBonusInfo = 
				from p in context.ShopBonusInfo
				where p.ShopId == shopid
				select p;
			if (!string.IsNullOrEmpty(name))
			{
				shopBonusInfo = 
					from p in shopBonusInfo
					where p.Name.Contains(name)
					select p;
			}
			shopBonusInfo = (state != 1 ? 
				from p in shopBonusInfo
				where (p.DateEnd < DateTime.Now) || p.IsInvalid
				select p : 
				from p in shopBonusInfo
				where (p.DateEnd > DateTime.Now) && !p.IsInvalid
				select p);
			if (pageIndex <= 0)
			{
				pageIndex = 1;
			}
			int num = 0;
			IQueryable<ShopBonusInfo> page = shopBonusInfo.GetPage(out num, (IQueryable<ShopBonusInfo> p) => 
				from o in p
				orderby o.BonusDateStart descending
				select o, pageIndex, pageSize);
			PageModel<ShopBonusInfo> pageModel = new PageModel<ShopBonusInfo>()
			{
				Models = 
					from p in page
					orderby p.BonusDateStart descending
					select p,
				Total = num
			};
			return pageModel;
		}

		public ShopBonusInfo Get(long id)
		{
			return context.ShopBonusInfo.FindById<ShopBonusInfo>(id);
		}

		public ShopBonusInfo GetByGrantId(long grantid)
		{
			ShopBonusGrantInfo shopBonusGrantInfo = (
				from p in context.ShopBonusGrantInfo
				where p.Id == grantid
				select p).FirstOrDefault();
			if (shopBonusGrantInfo == null)
			{
				return null;
			}
			return shopBonusGrantInfo.Himall_ShopBonus;
		}

		public ShopBonusGrantInfo GetByOrderId(long orderid)
		{
			ShopBonusGrantInfo shopBonusGrantInfo = (
				from p in context.ShopBonusGrantInfo
				where p.OrderId == orderid
				select p).FirstOrDefault();
			return shopBonusGrantInfo;
		}

		public ShopBonusInfo GetByShopId(long shopid)
		{
			return (
				from p in context.ShopBonusInfo
				where p.ShopId == shopid && !p.IsInvalid && (p.DateEnd > DateTime.Now)
				select p).FirstOrDefault();
		}

		public List<ShopBonusReceiveInfo> GetCanUseDetailByUserId(long userid)
		{
			return (
				from p in context.ShopBonusReceiveInfo
				where p.UserId == userid && (int)p.State == 1 && (p.Himall_ShopBonusGrant.Himall_ShopBonus.BonusDateEnd > DateTime.Now)
				select p).ToList();
		}

		public PageModel<ShopBonusReceiveInfo> GetDetail(long bonusid, int pageIndex, int pageSize)
		{
			if (pageIndex <= 0)
			{
				pageIndex = 1;
			}
			int num = 0;
			IQueryable<ShopBonusReceiveInfo> shopBonusReceiveInfo = 
				from p in context.ShopBonusReceiveInfo
				where p.Himall_ShopBonusGrant.ShopBonusId == bonusid
				select p;
			IQueryable<ShopBonusReceiveInfo> page = shopBonusReceiveInfo.GetPage(out num, (IQueryable<ShopBonusReceiveInfo> p) => 
				from o in p
				orderby o.ReceiveTime descending
				select o, pageIndex, pageSize);
			return new PageModel<ShopBonusReceiveInfo>()
			{
				Models = page,
				Total = num
			};
		}

		public List<ShopBonusReceiveInfo> GetDetailByGrantId(long grantid)
		{
			return (
				from p in context.ShopBonusReceiveInfo
				where p.BonusGrantId == grantid && !string.IsNullOrEmpty(p.OpenId)
				select p).ToList();
		}

		public ShopBonusReceiveInfo GetDetailById(long userid, long id)
		{
			return (
				from p in context.ShopBonusReceiveInfo
				where p.Id == id && p.UserId == userid
                select p).FirstOrDefault();
		}

		public PageModel<ShopBonusReceiveInfo> GetDetailByQuery(CouponRecordQuery query)
		{
			if (query.PageNo <= 0)
			{
				query.PageNo = 1;
			}
			int num = 0;
			IQueryable<ShopBonusReceiveInfo> shopBonusReceiveInfo = 
				from p in context.ShopBonusReceiveInfo
				where p.UserId == query.UserId
				select p;
			int? status = query.Status;
			if ((status.GetValueOrDefault() != 0 ? true : !status.HasValue))
			{
				int? nullable = query.Status;
				if ((nullable.GetValueOrDefault() != 1 ? false : nullable.HasValue))
				{
					shopBonusReceiveInfo = 
						from p in shopBonusReceiveInfo
						where (int)p.State == 2
						select p;
				}
			}
			else
			{
				shopBonusReceiveInfo = 
					from p in shopBonusReceiveInfo
					where (p.Himall_ShopBonusGrant.Himall_ShopBonus.BonusDateEnd > DateTime.Now) && (int)p.State == 1
					select p;
			}
			int? status1 = query.Status;
			if ((status1.GetValueOrDefault() != 2 ? false : status1.HasValue))
			{
				shopBonusReceiveInfo = 
					from p in shopBonusReceiveInfo
					where p.Himall_ShopBonusGrant.Himall_ShopBonus.BonusDateEnd < DateTime.Now
					select p;
			}
			IQueryable<ShopBonusReceiveInfo> page = shopBonusReceiveInfo.GetPage(out num, (IQueryable<ShopBonusReceiveInfo> p) => 
				from o in p
				orderby o.ReceiveTime descending
				select o, query.PageNo, query.PageSize);
			return new PageModel<ShopBonusReceiveInfo>()
			{
				Models = page,
				Total = num
			};
		}

		public List<ShopBonusReceiveInfo> GetDetailByUserId(long userid)
		{
			return (
				from p in context.ShopBonusReceiveInfo
				where p.UserId == userid
                select p).ToList();
		}

		public List<ShopBonusReceiveInfo> GetDetailToUse(long shopid, long userid, decimal sumprice)
		{
			List<ShopBonusReceiveInfo> list = (
				from p in context.ShopBonusReceiveInfo
				where p.UserId == userid && (int)p.State == 1 && p.Himall_ShopBonusGrant.Himall_ShopBonus.ShopId == shopid && (p.Himall_ShopBonusGrant.Himall_ShopBonus.BonusDateEnd > DateTime.Now) && (p.Himall_ShopBonusGrant.Himall_ShopBonus.BonusDateStart < DateTime.Now)
				select p).ToList();
			if (list.Count <= 0)
			{
				return list;
			}
			if (list[0].Himall_ShopBonusGrant.Himall_ShopBonus.UseState == ShopBonusInfo.UseStateType.FilledSend)
			{
				list = list.Where((ShopBonusReceiveInfo p) => {
					if (p.Himall_ShopBonusGrant.Himall_ShopBonus.UsrStatePrice >= sumprice)
					{
						return false;
					}
					decimal? price = p.Price;
					decimal num = sumprice;
					if (price.GetValueOrDefault() >= num)
					{
						return false;
					}
					return price.HasValue;
				}).OrderByDescending<ShopBonusReceiveInfo, decimal?>((ShopBonusReceiveInfo p) => p.Price).ToList();
			}
			return list.Where((ShopBonusReceiveInfo p) => {
				decimal? price = p.Price;
				decimal num = sumprice;
				if (price.GetValueOrDefault() >= num)
				{
					return false;
				}
				return price.HasValue;
			}).OrderByDescending<ShopBonusReceiveInfo, decimal?>((ShopBonusReceiveInfo p) => p.Price).ToList();
		}

		public ShopBonusGrantInfo GetGrantByUserOrder(long orderid, long userid)
		{
			ShopBonusGrantInfo shopBonusGrantInfo = (
				from p in context.ShopBonusGrantInfo
				where p.OrderId == orderid && p.UserId == userid && p.Himall_ShopBonusReceive.Any((ShopBonusReceiveInfo o) => o.UserId == null || o.UserId <= (long)0) && (p.Himall_ShopBonus.DateEnd > DateTime.Now)
				select p).FirstOrDefault();
			return shopBonusGrantInfo;
		}

		public long GetGrantIdByOrderId(long orderid)
		{
			ShopBonusGrantInfo shopBonusGrantInfo = (
				from p in context.ShopBonusGrantInfo
				where p.OrderId == orderid
				select p).FirstOrDefault();
			if (shopBonusGrantInfo != null)
			{
				return shopBonusGrantInfo.Id;
			}
			return 0;
		}

		public ActiveMarketServiceInfo GetShopBonusService(long shopId)
		{
			if (shopId <= 0)
			{
				throw new HimallException("ShopId不能识别");
			}
			return context.ActiveMarketServiceInfo.FirstOrDefault((ActiveMarketServiceInfo m) => m.ShopId == shopId && (int)m.TypeId == 4);
		}

		public decimal GetUsedPrice(long orderid, long userid)
		{
			ShopBonusReceiveInfo shopBonusReceiveInfo = (
				from p in context.ShopBonusReceiveInfo
				where p.UsedOrderId == orderid && p.UserId == userid
                select p).FirstOrDefault();
			if (shopBonusReceiveInfo == null)
			{
				return new decimal(0);
			}
			return shopBonusReceiveInfo.Price.Value;
		}

		public void Invalid(long id)
		{
			ShopBonusInfo shopBonusInfo = context.ShopBonusInfo.FirstOrDefault((ShopBonusInfo p) => p.Id == id);
			shopBonusInfo.IsInvalid = true;
            context.Configuration.ValidateOnSaveEnabled = false;
            context.SaveChanges();
            context.Configuration.ValidateOnSaveEnabled = true;
		}

		public bool IsAdd(long shopid)
		{
			if ((
				from p in context.ShopBonusInfo
				where p.ShopId == shopid && !p.IsInvalid && (p.DateEnd > DateTime.Now) && (p.DateStart < DateTime.Now)
				select p).FirstOrDefault() != null)
			{
				return false;
			}
			return true;
		}

		public bool IsOverDate(DateTime bonusDateEnd, DateTime dateEnd, long shopid)
		{
			DateTime dateTime = GetShopBonusService(shopid).MarketServiceRecordInfo.Max<MarketServiceRecordInfo, DateTime>((MarketServiceRecordInfo a) => a.EndTime);
			if (!(bonusDateEnd > dateTime) && !(dateEnd > dateTime))
			{
				return false;
			}
			return true;
		}

		public object Receive(long grantid, string openId, string wxhead, string wxname)
		{
			Log.Info(string.Format("Receive函数 = gid:{0} , oid:{1}", grantid, openId));
			if ((
				from p in context.ShopBonusReceiveInfo
				where (p.OpenId == openId) && p.BonusGrantId == grantid
				select p).Count() > 0)
			{
				ShopReceiveModel shopReceiveModel = new ShopReceiveModel()
				{
					State = ShopReceiveStatus.Receive,
					Price = new decimal(0)
				};
				return shopReceiveModel;
			}
			ShopBonusReceiveInfo nullable = (
				from p in context.ShopBonusReceiveInfo
				where p.BonusGrantId == grantid && string.IsNullOrEmpty(p.OpenId)
				select p).FirstOrDefault();
			if (nullable == null)
			{
				ShopReceiveModel shopReceiveModel1 = new ShopReceiveModel()
				{
					State = ShopReceiveStatus.HaveNot,
					Price = new decimal(0)
				};
				return shopReceiveModel1;
			}
			if (nullable.Himall_ShopBonusGrant.Himall_ShopBonus.IsInvalid)
			{
				ShopReceiveModel shopReceiveModel2 = new ShopReceiveModel()
				{
					State = ShopReceiveStatus.Invalid,
					Price = new decimal(0)
				};
				return shopReceiveModel2;
			}
			MemberOpenIdInfo memberOpenIdInfo = (
				from p in context.MemberOpenIdInfo
				where p.OpenId == openId
				select p).FirstOrDefault();
			MemberOpenIdInfo memberOpenIdInfo1 = memberOpenIdInfo;
			MemberOpenIdInfo memberOpenIdInfo2 = memberOpenIdInfo;
			memberOpenIdInfo2 = memberOpenIdInfo1;
			if (memberOpenIdInfo2 != null)
			{
				nullable.UserId = new long?(memberOpenIdInfo2.UserId);
			}
			nullable.OpenId = openId;
			nullable.ReceiveTime = new DateTime?(DateTime.Now);
			nullable.WXHead = wxhead;
			nullable.WXName = wxname;
            context.SaveChanges();
			if (memberOpenIdInfo2 == null)
			{
				ShopReceiveModel shopReceiveModel3 = new ShopReceiveModel()
				{
					State = ShopReceiveStatus.CanReceiveNotUser,
					Price = nullable.Price.Value,
					Id = nullable.Id
				};
				return shopReceiveModel3;
			}
			string str = (
				from p in context.UserMemberInfo
				where p.Id == memberOpenIdInfo2.UserId
				select p.UserName).FirstOrDefault();
			ShopReceiveModel shopReceiveModel4 = new ShopReceiveModel()
			{
				State = ShopReceiveStatus.CanReceive,
				Price = nullable.Price.Value,
				UserName = str,
				Id = nullable.Id
			};
			return shopReceiveModel4;
		}

		public void SetBonusToUsed(long userid, List<OrderInfo> orders, long rid)
		{
			ShopBonusReceiveInfo nullable = (
				from p in context.ShopBonusReceiveInfo
				where p.UserId == userid && p.Id == rid
				select p).FirstOrDefault();
			nullable.State = ShopBonusReceiveInfo.ReceiveState.Use;
			nullable.UsedTime = new DateTime?(DateTime.Now);
			nullable.UsedOrderId = new long?(orders.FirstOrDefault((OrderInfo p) => p.ShopId == nullable.Himall_ShopBonusGrant.Himall_ShopBonus.ShopId).Id);
            context.SaveChanges();
            ser_wxcard.Consume(nullable.Id, ThisCouponType);
		}

		public void Update(ShopBonusInfo model)
		{
			ShopBonusInfo name = context.ShopBonusInfo.FindById<ShopBonusInfo>(model.Id);
			name.Name = model.Name;
			name.GrantPrice = model.GrantPrice;
			name.DateStart = model.DateStart;
			DateTime dateTime = model.DateEnd.AddHours(23);
			DateTime dateTime1 = dateTime.AddMinutes(59);
			name.DateEnd = dateTime1.AddSeconds(59);
			name.ShareDetail = model.ShareDetail;
			name.ShareImg = model.ShareImg;
			name.ShareTitle = model.ShareTitle;
            context.Configuration.ValidateOnSaveEnabled = false;
            context.SaveChanges();
            context.Configuration.ValidateOnSaveEnabled = true;
		}
	}
}