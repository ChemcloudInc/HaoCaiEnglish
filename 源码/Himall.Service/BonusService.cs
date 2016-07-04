using Himall.Core;
using Himall.Core.Helper;
using Himall.Entity;
using Himall.IServices;
using Himall.Model;
using Himall.Service.Market.Business;
using Himall.ServiceProvider;
using Senparc.Weixin;
using Senparc.Weixin.Entities;
using Senparc.Weixin.MP.AdvancedAPIs.User;
using Senparc.Weixin.MP.CommonAPIs;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Himall.Service
{
	public class BonusService : ServiceBase, IBonusService, IService, IDisposable
	{
		private string _accessToken = string.Empty;

		public BonusService()
		{
			SiteSettingsInfo siteSettings = Instance<ISiteSettingService>.Create.GetSiteSettings();
			if (!string.IsNullOrEmpty(siteSettings.WeixinAppId) || !string.IsNullOrEmpty(siteSettings.WeixinAppSecret))
			{
                init(siteSettings.WeixinAppId, siteSettings.WeixinAppSecret);
			}
		}

		public void Add(BonusInfo model, string receiveurl)
		{
			BonusInfo bonusInfo = model;
			BonusInfo bonusInfo1 = bonusInfo;
			DateTime endTime = bonusInfo.EndTime;
			DateTime dateTime = endTime.AddHours(23).AddMinutes(59);
			bonusInfo1.EndTime = dateTime.AddSeconds(59);
			bonusInfo.IsInvalid = false;
			bonusInfo.ReceiveCount = 0;
			bonusInfo.QRPath = "";
			bonusInfo.ReceiveHref = "";
			bonusInfo = context.BonusInfo.Add(bonusInfo);
            context.Configuration.ValidateOnSaveEnabled = false;
            context.SaveChanges();
			bonusInfo.ReceiveHref = string.Concat(receiveurl, bonusInfo.Id);
			bonusInfo.QRPath = GenerateQR(bonusInfo.ReceiveHref);
            context.SaveChanges();
            context.Configuration.ValidateOnSaveEnabled = true;
			Task.Factory.StartNew(() => GenerateBonusDetail(bonusInfo));
		}

		public bool CanAddBonus()
		{
			if ((
				from p in context.BonusInfo
				where (int)p.Type == 2 && !p.IsInvalid && (p.EndTime > DateTime.Now)
				select p).Count() == 1)
			{
				return false;
			}
			return true;
		}

		private void DepositShopBonus(MemberOpenIdInfo openInfo, Entities efContext)
		{
			List<ShopBonusReceiveInfo> list = (
				from p in efContext.ShopBonusReceiveInfo
				where (p.OpenId == openInfo.OpenId) && p.UserId == null
				select p).ToList();
			if (list.Count() <= 0)
			{
				return;
			}
			DateTime now = DateTime.Now;
			foreach (ShopBonusReceiveInfo nullable in list)
			{
				nullable.UserId = new long?(openInfo.UserId);
				nullable.ReceiveTime = new DateTime?(now);
			}
			try
			{
				efContext.SaveChanges();
			}
			catch (Exception exception)
			{
				Log.Info("商家红包存储出错：", exception);
			}
		}

		private void DepositToMember(string openId, decimal price)
		{
			Entities entity = new Entities();
			MemberOpenIdInfo memberOpenIdInfo = (
				from p in entity.MemberOpenIdInfo
				where p.OpenId == openId
				select p).FirstOrDefault();
			if (memberOpenIdInfo != null)
			{
				BonusReceiveInfo bonusReceiveInfo = (
					from p in entity.BonusReceiveInfo
					where (p.OpenId == openId) && !p.IsTransformedDeposit
					select p).FirstOrDefault();
				bonusReceiveInfo.IsTransformedDeposit = true;
				bonusReceiveInfo.Himall_Members = entity.UserMemberInfo.FirstOrDefault((UserMemberInfo p) => p.Id == memberOpenIdInfo.UserId);
				entity.SaveChanges();
				IMemberCapitalService create = Instance<IMemberCapitalService>.Create;
				CapitalDetailModel capitalDetailModel = new CapitalDetailModel()
				{
					UserId = memberOpenIdInfo.UserId,
					SourceType = CapitalDetailInfo.CapitalDetailType.RedPacket,
					Amount = price,
					CreateTime = bonusReceiveInfo.ReceiveTime.Value.ToString("yyyy-MM-dd HH:mm:ss")
				};
				create.AddCapital(capitalDetailModel);
			}
			entity.Dispose();
		}

		public void DepositToRegister(long userid)
		{
			Entities entity = new Entities();
			List<MemberOpenIdInfo> list = (
				from p in entity.MemberOpenIdInfo
				where p.UserId == userid && !string.IsNullOrEmpty(p.OpenId)
				select p).ToList();
			if (list == null || list.Count == 0)
			{
				return;
			}
			foreach (MemberOpenIdInfo memberOpenIdInfo in list)
			{
                DepositToRegister(memberOpenIdInfo, entity);
			}
			foreach (MemberOpenIdInfo memberOpenIdInfo1 in list)
			{
                DepositShopBonus(memberOpenIdInfo1, entity);
			}
			entity.Dispose();
		}

		private void DepositToRegister(MemberOpenIdInfo openInfo, Entities efContext)
		{
			IQueryable<BonusReceiveInfo> bonusReceiveInfo = 
				from p in efContext.BonusReceiveInfo
				where (p.OpenId == openInfo.OpenId) && !p.IsTransformedDeposit
				select p;
			List<BonusReceiveInfo> list = bonusReceiveInfo.ToList();
			List<CapitalDetailModel> capitalDetailModels = new List<CapitalDetailModel>();
			if (list.Count > 0)
			{
				foreach (BonusReceiveInfo bonusReceiveInfo1 in list)
				{
					bonusReceiveInfo1.IsTransformedDeposit = true;
					bonusReceiveInfo1.Himall_Members = efContext.UserMemberInfo.FirstOrDefault((UserMemberInfo p) => p.Id == openInfo.UserId);
					CapitalDetailModel capitalDetailModel = new CapitalDetailModel()
					{
						UserId = openInfo.UserId,
						SourceType = CapitalDetailInfo.CapitalDetailType.RedPacket,
						Amount = bonusReceiveInfo1.Price,
						CreateTime = bonusReceiveInfo1.ReceiveTime.Value.ToString("yyyy-MM-dd HH:mm:ss")
					};
					capitalDetailModels.Add(capitalDetailModel);
				}
				efContext.SaveChanges();
			}
			IMemberCapitalService create = Instance<IMemberCapitalService>.Create;
			foreach (CapitalDetailModel capitalDetailModel1 in capitalDetailModels)
			{
				create.AddCapital(capitalDetailModel1);
			}
		}

		private void GenerateBonusDetail(BonusInfo model)
		{
			(new GenerateDetailContext(model)).Generate();
		}

		private string GenerateQR(string path)
		{
			Bitmap bitmap = QRCodeHelper.Create(path);
			Guid guid = Guid.NewGuid();
			string str = string.Concat(guid.ToString(), ".jpg");
			string str1 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Storage", "Plat", "Bonus");
			string str2 = Path.Combine(str1, str);
			if (!Directory.Exists(str1))
			{
				Directory.CreateDirectory(str1);
			}
			bitmap.Save(str2);
			return string.Concat("/Storage/Plat/Bonus/", str);
		}

		public PageModel<BonusInfo> Get(int type, int state = 1, string name = "", int pageIndex = 1, int pageSize = 20)
		{
			IQueryable<BonusInfo> bonusInfo = context.BonusInfo;
			if (type > 0)
			{
				bonusInfo = 
					from p in context.BonusInfo
					where (int)p.Type == type
					select p;
			}
			if (!string.IsNullOrEmpty(name))
			{
				bonusInfo = 
					from p in bonusInfo
					where p.Name.Contains(name)
					select p;
			}
			if (pageIndex <= 0)
			{
				pageIndex = 1;
			}
			if (state == 1)
			{
				bonusInfo = 
					from p in bonusInfo
					where (p.EndTime > DateTime.Now) && !p.IsInvalid
					select p;
			}
			else if (state == 2)
			{
				bonusInfo = 
					from p in bonusInfo
					where p.IsInvalid || (p.EndTime < DateTime.Now)
					select p;
			}
			int num = 0;
			IQueryable<BonusInfo> page = bonusInfo.GetPage(out num, (IQueryable<BonusInfo> p) => 
				from o in p
				orderby o.StartTime descending
				select o, pageIndex, pageSize);
			foreach (BonusInfo description in page)
			{
				description.TypeStr = description.Type.ToDescription();
				description.StartTimeStr = description.StartTime.ToString("yyyy-MM-dd");
				description.EndTimeStr = description.EndTime.ToString("yyyy-MM-dd");
			}
			return new PageModel<BonusInfo>()
			{
				Models = page,
				Total = num
			};
		}

		public BonusInfo Get(long id)
		{
			return context.BonusInfo.FindById<BonusInfo>(id);
		}

		public PageModel<BonusReceiveInfo> GetDetail(long bonusId, int pageIndex = 1, int pageSize = 15)
		{
			if (pageIndex <= 0)
			{
				pageIndex = 1;
			}
			int num = 0;
			IQueryable<BonusReceiveInfo> bonusReceiveInfo = 
				from p in context.BonusReceiveInfo
				where p.Himall_Bonus.Id == bonusId
				select p;
			IQueryable<BonusReceiveInfo> page = bonusReceiveInfo.GetPage(out num, (IQueryable<BonusReceiveInfo> p) => 
				from o in p
				orderby o.ReceiveTime descending
				select o, pageIndex, pageSize);
			return new PageModel<BonusReceiveInfo>()
			{
				Models = page,
				Total = num
			};
		}

		public decimal GetReceivePriceByOpendId(long id, string openId)
		{
			return (
				from p in context.BonusReceiveInfo
				where (p.OpenId == openId) && p.BonusId == id
				select p).FirstOrDefault().Price;
		}

		public void GetSuccessSendWXMessage(BonusReceiveInfo data, string openId, string url)
		{
			WX_MSGGetCouponModel wXMSGGetCouponModel = new WX_MSGGetCouponModel();
			wXMSGGetCouponModel.first.@value = "领取红包成功！";
			wXMSGGetCouponModel.first.color = "#000000";
			wXMSGGetCouponModel.keyword1.@value = "现金红包(1个)";
			wXMSGGetCouponModel.keyword1.color = "#000000";
			WX_MSGItemBaseModel wXMSGItemBaseModel = wXMSGGetCouponModel.keyword2;
			decimal price = data.Price;
			wXMSGItemBaseModel.@value = string.Concat(price.ToString(), "元");
			wXMSGGetCouponModel.keyword2.color = "#FF0000";
			wXMSGGetCouponModel.remark.@value = "红包领取成功会直接计入预存款，可消费与提现。";
			wXMSGGetCouponModel.remark.color = "#000000";
			SiteSettingsInfo siteSettings = Instance<ISiteSettingService>.Create.GetSiteSettings();
			Instance<IWXApiService>.Create.SendMessageByTemplate(siteSettings.WeixinAppId, siteSettings.WeixinAppSecret, openId, siteSettings.WX_MSGGetCouponTemplateId, "#000000", url, wXMSGGetCouponModel);
		}

		private void init(string appid, string secret)
		{
            //_accessToken = AccessTokenContainer.TryGetToken(appid, secret, false);
		}

		public void Invalid(long id)
		{
			BonusInfo bonusInfo = context.BonusInfo.FirstOrDefault((BonusInfo p) => p.Id == id);
			bonusInfo.IsInvalid = true;
            context.SaveChanges();
		}

		public bool IsAttention(string openId)
		{
			OpenIdsInfo openIdsInfo = context.OpenIdsInfo.FirstOrDefault((OpenIdsInfo p) => p.OpenId == openId);
			if (openIdsInfo != null)
			{
				return openIdsInfo.IsSubscribe;
			}
			return IsAttentionByRPC(openId);
		}

		private bool IsAttentionByRPC(string openId)
		{
			UserInfoJson userInfoJson = UserApi.Info(_accessToken, openId, Language.zh_CN);
			if (userInfoJson.errcode == ReturnCode.不合法的OpenID || userInfoJson.subscribe == 0)
			{
				return false;
			}
			if (userInfoJson.errcode != ReturnCode.请求成功)
			{
				throw new Exception(userInfoJson.errmsg);
			}
			return userInfoJson.subscribe == 1;
		}

		public object Receive(long id, string openId)
		{
			if (!IsAttention(openId))
			{
				ReceiveModel receiveModel = new ReceiveModel()
				{
					State = ReceiveStatus.NotAttention,
					Price = new decimal(0)
				};
				return receiveModel;
			}
			IQueryable<BonusReceiveInfo> bonusReceiveInfo = 
				from p in context.BonusReceiveInfo
				where (p.OpenId == openId) && p.BonusId == id
				select p;
			int num = bonusReceiveInfo.Count();
			bool flag = (
				from p in bonusReceiveInfo
				where p.IsShare == true
                select p).Count() != 0;
			if (num > 0 && (!flag && num == 1 || num == 2))
			{
				ReceiveModel receiveModel1 = new ReceiveModel()
				{
					State = ReceiveStatus.Receive,
					Price = new decimal(0)
				};
				return receiveModel1;
			}
			IQueryable<BonusReceiveInfo> bonusReceiveInfos = 
				from p in context.BonusReceiveInfo
				where p.BonusId == id && string.IsNullOrEmpty(p.OpenId)
				select p;
			BonusReceiveInfo nullable = bonusReceiveInfos.FirstOrDefault();
			if (nullable == null)
			{
				ReceiveModel receiveModel2 = new ReceiveModel()
				{
					State = ReceiveStatus.HaveNot,
					Price = new decimal(0)
				};
				return receiveModel2;
			}
			if (nullable.Himall_Bonus.IsInvalid)
			{
				ReceiveModel receiveModel3 = new ReceiveModel()
				{
					State = ReceiveStatus.Invalid,
					Price = new decimal(0)
				};
				return receiveModel3;
			}
			BonusInfo himallBonus = nullable.Himall_Bonus;
			himallBonus.ReceiveCount = himallBonus.ReceiveCount + 1;
			BonusInfo receivePrice = nullable.Himall_Bonus;
			receivePrice.ReceivePrice = receivePrice.ReceivePrice + nullable.Price;
			nullable.OpenId = openId;
			nullable.ReceiveTime = new DateTime?(DateTime.Now);
			nullable.IsTransformedDeposit = false;
            context.Configuration.ValidateOnSaveEnabled = false;
            context.SaveChanges();
            context.Configuration.ValidateOnSaveEnabled = true;
			string str = string.Concat("http://", WebHelper.GetHost(), "/userCenter");
			Task.Factory.StartNew(() => {
				Instance<IWXApiService>.Create.Subscribe(openId);
                DepositToMember(openId, nullable.Price);
                GetSuccessSendWXMessage(nullable, openId, str);
			});
			ReceiveModel receiveModel4 = new ReceiveModel()
			{
				State = ReceiveStatus.CanReceive,
				Price = nullable.Price
			};
			return receiveModel4;
		}

		public string Receive(string openId)
		{
			if ((
				from p in context.OpenIdsInfo
				where p.OpenId == openId
				select p).Count() > 0)
			{
				return null;
			}
			IQueryable<BonusReceiveInfo> bonusReceiveInfo = 
				from p in context.BonusReceiveInfo
				where (int)p.Himall_Bonus.Type == 2 && !p.Himall_Bonus.IsInvalid && (p.Himall_Bonus.EndTime >= DateTime.Now) && (p.Himall_Bonus.StartTime <= DateTime.Now) && string.IsNullOrEmpty(p.OpenId) && !p.IsTransformedDeposit
				select p;
			BonusReceiveInfo nullable = bonusReceiveInfo.FirstOrDefault();
			if (nullable == null)
			{
				return null;
			}
			BonusInfo himallBonus = nullable.Himall_Bonus;
			himallBonus.ReceiveCount = himallBonus.ReceiveCount + 1;
			BonusInfo receivePrice = nullable.Himall_Bonus;
			receivePrice.ReceivePrice = receivePrice.ReceivePrice + nullable.Price;
			nullable.OpenId = openId;
			nullable.ReceiveTime = new DateTime?(DateTime.Now);
			nullable.IsTransformedDeposit = false;
            context.Configuration.ValidateOnSaveEnabled = false;
            context.SaveChanges();
            context.Configuration.ValidateOnSaveEnabled = true;
			string str = string.Concat("http://", WebHelper.GetHost(), "/userCenter");
			Task.Factory.StartNew(() => {
                DepositToMember(openId, nullable.Price);
                GetSuccessSendWXMessage(nullable, openId, str);
			});
			return "";
		}

		public void SetShare(long id, string openId)
		{
			BonusReceiveInfo nullable = (
				from p in context.BonusReceiveInfo
				where p.BonusId == id && (p.OpenId == openId)
				select p).FirstOrDefault();
			nullable.IsShare = new bool?(true);
            context.Configuration.ValidateOnSaveEnabled = false;
            context.SaveChanges();
            context.Configuration.ValidateOnSaveEnabled = true;
		}

		public void Update(BonusInfo model)
		{
			BonusInfo style = context.BonusInfo.FindById<BonusInfo>(model.Id);
			style.Style = model.Style;
			style.Name = model.Name;
			style.MerchantsName = model.MerchantsName;
			style.Remark = model.Remark;
			style.Blessing = model.Blessing;
			style.StartTime = model.StartTime;
			style.EndTime = model.EndTime;
			style.ImagePath = model.ImagePath;
			style.Description = model.Description;
            context.Configuration.ValidateOnSaveEnabled = false;
            context.SaveChanges();
            context.Configuration.ValidateOnSaveEnabled = true;
		}
	}
}