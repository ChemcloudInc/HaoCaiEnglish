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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Himall.Service
{
	public class VShopService : ServiceBase, IVShopService, IService, IDisposable
	{
		public VShopService()
		{
		}

		public void AddBuyNumber(long vshopId)
		{
			VShopInfo vShopInfo = this.context.VShopInfo.FirstOrDefault((VShopInfo item) => item.Id == vshopId);
			if (vShopInfo != null)
			{
				VShopInfo vShopInfo1 = vShopInfo;
				vShopInfo1.buyNum = vShopInfo1.buyNum + 1;
				this.context.SaveChanges();
			}
		}

		public void AddVisitNumber(long vshopId)
		{
			VShopInfo vShopInfo = this.context.VShopInfo.FirstOrDefault((VShopInfo item) => item.Id == vshopId);
			if (vShopInfo != null)
			{
				VShopInfo visitNum = vShopInfo;
				visitNum.VisitNum = visitNum.VisitNum + 1;
				this.context.SaveChanges();
			}
		}

		private void AddVShopSetting(WXShopInfo vshopSetting)
		{
			if (string.IsNullOrEmpty(vshopSetting.AppId))
			{
				throw new HimallException("微信AppId不能为空！");
			}
			if (string.IsNullOrEmpty(vshopSetting.AppSecret))
			{
				throw new HimallException("微信AppSecret不能为空！");
			}
			this.context.WXShopInfo.Add(vshopSetting);
			this.context.SaveChanges();
		}

		public void AuditRefused(long vshopId)
		{
			this.GetVshopById(vshopId).State = VShopInfo.VshopStates.Refused;
			this.context.SaveChanges();
		}

		public void AuditThrough(long vshopId)
		{
			DbSet<VShopExtendInfo> vShopExtendInfo = this.context.VShopExtendInfo;
			VShopExtendInfo vShopExtendInfo1 = new VShopExtendInfo()
			{
				Sequence = new int?(1),
				VShopId = vshopId,
				AddTime = DateTime.Now,
				Type = VShopExtendInfo.VShopExtendType.HotVShop
			};
			vShopExtendInfo.Add(vShopExtendInfo1);
			this.context.SaveChanges();
		}

		public void CloseShop(long vshopId)
		{
			this.GetVshopById(vshopId).State = VShopInfo.VshopStates.Close;
			this.context.VShopExtendInfo.OrderBy((VShopExtendInfo item) => item.VShopId == vshopId);
			this.context.SaveChanges();
		}

		private void CopyImages(long shopId, ref string logo, ref string backgroundImage, ref string wxlogo)
		{
			string str = string.Format("/Storage/Shop/{0}/VShop", shopId);
			string mapPath = IOHelper.GetMapPath(str);
			if (!Directory.Exists(mapPath))
			{
				Directory.CreateDirectory(mapPath);
			}
			if (!logo.Contains("/Storage"))
			{
				string str1 = string.Concat(str, "/Logo.", logo.Substring(logo.LastIndexOf('.') + 1));
				string mapPath1 = IOHelper.GetMapPath(logo);
				string mapPath2 = IOHelper.GetMapPath(str1);
				if (File.Exists(mapPath2))
				{
					File.Delete(mapPath2);
				}
				File.Copy(mapPath1, mapPath2);
				logo = str1;
			}
			if (!backgroundImage.Contains("/Storage"))
			{
				string str2 = string.Concat(str, "/BackgroundImage.", backgroundImage.Substring(backgroundImage.LastIndexOf('.') + 1));
				string mapPath3 = IOHelper.GetMapPath(backgroundImage);
				string str3 = IOHelper.GetMapPath(str2);
				if (File.Exists(str3))
				{
					File.Delete(str3);
				}
				File.Copy(mapPath3, str3, true);
				backgroundImage = str2;
			}
			if (!wxlogo.Contains("/Storage"))
			{
				string str4 = string.Concat(str, "/WXLogo.png");
				string mapPath4 = IOHelper.GetMapPath(wxlogo);
				string mapPath5 = IOHelper.GetMapPath(str4);
				using (Image image = Image.FromFile(mapPath4))
				{
					image.Save(string.Concat(mapPath4, ".png"), ImageFormat.Png);
					if (File.Exists(mapPath5))
					{
						File.Delete(mapPath5);
					}
					ImageHelper.CreateThumbnail(string.Concat(mapPath4, ".png"), mapPath5, 100, 100);
				}
				wxlogo = str4;
			}
		}

		public void CreateVshop(VShopInfo vshopInfo)
		{
			if (vshopInfo.ShopId <= 0)
			{
				throw new InvalidPropertyException("请传入合法的店铺Id，店铺Id必须大于0");
			}
			if (string.IsNullOrWhiteSpace(vshopInfo.Logo))
			{
				throw new InvalidPropertyException("微店Logo不能为空");
			}
			if (string.IsNullOrWhiteSpace(vshopInfo.WXLogo))
			{
				throw new InvalidPropertyException("微信Logo不能为空");
			}
			if (string.IsNullOrWhiteSpace(vshopInfo.BackgroundImage))
			{
				throw new InvalidPropertyException("微店背景图片不能为空");
			}
			if (string.IsNullOrWhiteSpace(vshopInfo.Description))
			{
				throw new InvalidPropertyException("微店描述不能为空");
			}
			if (!string.IsNullOrWhiteSpace(vshopInfo.Tags))
			{
				string tags = vshopInfo.Tags;
				char[] chrArray = new char[] { ',' };
				if (tags.Split(chrArray).Length > 4)
				{
					throw new InvalidPropertyException("最多只能有4个标签");
				}
			}
			if (this.context.VShopInfo.Any((VShopInfo item) => item.ShopId == vshopInfo.ShopId))
			{
				throw new InvalidPropertyException(string.Format("店铺{0}已经创建过微店", vshopInfo.ShopId));
			}
			ShopInfo shop = Instance<IShopService>.Create.GetShop(vshopInfo.ShopId, false);
			vshopInfo.Name = shop.ShopName;
			vshopInfo.CreateTime = DateTime.Now;
			vshopInfo.State = VShopInfo.VshopStates.Normal;
			string logo = vshopInfo.Logo;
			string backgroundImage = vshopInfo.BackgroundImage;
			string wXLogo = vshopInfo.WXLogo;
			this.CopyImages(vshopInfo.ShopId, ref logo, ref backgroundImage, ref wXLogo);
			vshopInfo.Logo = logo;
			vshopInfo.BackgroundImage = backgroundImage;
			vshopInfo.WXLogo = wXLogo;
			this.context.VShopInfo.Add(vshopInfo);
			this.context.SaveChanges();
		}

		public void DeleteHotShop(long vshopId)
		{
			this.context.VShopExtendInfo.OrderBy((VShopExtendInfo item) => item.VShopId == vshopId && (int)item.Type == 2);
			this.context.SaveChanges();
		}

		public void DeleteTopShop(long vshopId)
		{
			this.context.VShopExtendInfo.OrderBy((VShopExtendInfo item) => item.VShopId == vshopId && (int)item.Type == 1);
			this.context.SaveChanges();
		}

		private IEnumerable<VShopInfo> GetAllVshop()
		{
			return 
				from a in this.context.VShopInfo
				where (int)a.State != 99
				select a;
		}

		public IEnumerable<VShopInfo> GetHotShop(VshopQuery vshopQuery, DateTime? startTime, DateTime? endTime, out int total)
		{
			IQueryable<VShopInfo> vShopInfo = 
				from v in this.context.VShopInfo
				where v.VShopExtendInfo.Count > 0 && v.VShopExtendInfo.Any((VShopExtendInfo p) => (int)p.Type == 2)
				select v;
			if (!string.IsNullOrEmpty(vshopQuery.Name))
			{
				vShopInfo = 
					from a in vShopInfo
					where a.Name.Contains(vshopQuery.Name)
					select a;
			}
			if (startTime.HasValue && endTime.HasValue)
			{
				vShopInfo = 
					from a in vShopInfo
					where a.VShopExtendInfo.Any((VShopExtendInfo item) => item.AddTime >= startTime && item.AddTime <= endTime)
					select a;
			}
			total = vShopInfo.Count();
			return (
				from a in vShopInfo
				orderby a.CreateTime
				select a).Skip((vshopQuery.PageNo - 1) * vshopQuery.PageSize).Take(vshopQuery.PageSize);
		}

		public IQueryable<VShopInfo> GetHotShops(int page, int pageSize, out int total)
		{
			IQueryable<VShopInfo> vShopInfo = 
				from a in this.context.VShopInfo
				where this.context.VShopExtendInfo.Where((VShopExtendInfo b) => (int)b.Type == 2).Select<VShopExtendInfo, long>((VShopExtendInfo b) => b.VShopId).Contains<long>(a.Id)
				select a;
			total = vShopInfo.Count();
			return (
				from item in vShopInfo
				orderby item.CreateTime descending
				select item).Skip((page - 1) * pageSize).Take(pageSize);
		}

		public VShopInfo GetTopShop()
		{
			return (
				from a in this.context.VShopInfo
				where a.VShopExtendInfo.Any((VShopExtendInfo item) => (int)item.Type == 1)
				select a).FirstOrDefault();
		}

		public IQueryable<VShopInfo> GetUserConcernVShops(long userId, int pageNo, int pageSize)
		{
			long[] array = (
				from item in context.FavoriteShopInfo
				where item.UserId == userId
				select item.ShopId).ToArray();
			IQueryable<VShopInfo> vShopInfo = 
				from item in context.VShopInfo
				where (int)item.State != 99
				select item;
			IQueryable<VShopInfo> vShopInfos = (
				from item in vShopInfo
				where array.Contains(item.ShopId)
				orderby item.Id descending
				select item).Skip((pageNo - 1) * pageSize).Take(pageSize);
			return vShopInfos;
		}

		public VShopInfo GetVShop(long id)
		{
			return this.context.VShopInfo.FirstOrDefault((VShopInfo item) => item.Id == id);
		}

		private VShopInfo GetVshopById(long vshopId)
		{
			return (
				from a in this.context.VShopInfo
				where a.Id == vshopId
				select a).FirstOrDefault();
		}

		public IEnumerable<VShopInfo> GetVShopByParamete(VshopQuery vshopQuery, out int total)
		{
			IEnumerable<VShopInfo> allVshop = this.GetAllVshop();
			if (vshopQuery.VshopType.HasValue)
			{
				VShopExtendInfo.VShopExtendType? vshopType = vshopQuery.VshopType;
				allVshop = ((vshopType.GetValueOrDefault() != 0 ? false : vshopType.HasValue) ?
                    from a in this.context.VShopInfo
                    where a.VShopExtendInfo.Count() == 0
                    select a :
                    from a in this.context.VShopInfo
                    where a.VShopExtendInfo.Where((VShopExtendInfo b) => (int?)b.Type == (int?)vshopQuery.VshopType).Select<VShopExtendInfo, long>((VShopExtendInfo b) => b.VShopId).Contains<long>(a.Id)
                    select a);
			}
			if (!string.IsNullOrEmpty(vshopQuery.Name))
			{
				allVshop = 
					from a in allVshop
					where a.Name.Contains(vshopQuery.Name)
					select a;
			}
			allVshop = 
				from e in allVshop
				where e.State == VShopInfo.VshopStates.Normal
				select e;
			total = allVshop.Count();
			return (
				from a in allVshop
				orderby a.CreateTime
				select a).Skip((vshopQuery.PageNo - 1) * vshopQuery.PageSize).Take(vshopQuery.PageSize);
		}

		public VShopInfo GetVShopByShopId(long shopId)
		{
			return this.context.VShopInfo.FirstOrDefault((VShopInfo item) => item.ShopId == shopId);
		}

		public IQueryable<CouponSettingInfo> GetVShopCouponSetting(long shopid)
		{
			IQueryable<long> couponInfo = 
				from item in this.context.CouponInfo
				where item.ShopId == shopid
				select item.Id;
			return 
				from item in this.context.CouponSettingInfo
				where couponInfo.Contains<long>(item.CouponID) && (item.Himall_Coupon.EndTime > DateTime.Now) && (item.Display.HasValue ? item.Display == 1 : true)
				select item;
		}

		public IQueryable<VShopInfo> GetVShops()
		{
			return this.context.VShopInfo.FindAll<VShopInfo>();
		}

		public IQueryable<VShopInfo> GetVShops(int page, int pageSize, out int total)
		{
			total = this.context.VShopInfo.Count();
			return (
				from item in this.context.VShopInfo
				orderby item.Id descending
				select item).Skip((page - 1) * pageSize).Take(pageSize);
		}

		public IQueryable<VShopInfo> GetVShops(int page, int pageSize, out int total, VShopInfo.VshopStates state)
		{
			total = this.context.VShopInfo.Count();
			return (
				from item in this.context.VShopInfo
				where (int)item.State == (int)state
				orderby item.Id descending
				select item).Skip((page - 1) * pageSize).Take(pageSize);
		}

		public WXShopInfo GetVShopSetting(long shopId)
		{
			return this.context.WXShopInfo.FirstOrDefault((WXShopInfo item) => item.ShopId == shopId);
		}

		public int LogVisit(long id)
		{
			VShopInfo vshopById = this.GetVshopById(id);
			VShopInfo visitNum = vshopById;
			visitNum.VisitNum = visitNum.VisitNum + 1;
			this.context.SaveChanges();
			return vshopById.VisitNum;
		}

		public void ReplaceHotShop(long oldVShopId, long newHotVShopId)
		{
			VShopExtendInfo now = this.context.VShopExtendInfo.FindBy((VShopExtendInfo item) => item.VShopId == oldVShopId && (int)item.Type == 2).FirstOrDefault();
			now.VShopId = newHotVShopId;
			now.AddTime = DateTime.Now;
			this.context.SaveChanges();
		}

		public void ReplaceTopShop(long oldVShopId, long newTopVShopId)
		{
			VShopExtendInfo now = this.context.VShopExtendInfo.FindBy((VShopExtendInfo item) => item.VShopId == oldVShopId && (int)item.Type == 1).FirstOrDefault();
			now.VShopId = newTopVShopId;
			now.AddTime = DateTime.Now;
			this.context.SaveChanges();
		}

		public void SaveVShopCouponSetting(IEnumerable<CouponSettingInfo> infolist)
		{
			if (infolist == null)
			{
				throw new HimallException("没有可更新的数据！");
			}
			foreach (CouponSettingInfo couponSettingInfo in infolist)
			{
				CouponSettingInfo display = this.context.CouponSettingInfo.FirstOrDefault((CouponSettingInfo item) => item.CouponID == couponSettingInfo.CouponID);
				if (display == null)
				{
					if (!couponSettingInfo.Display.HasValue)
					{
						continue;
					}
					int? nullable = couponSettingInfo.Display;
					if ((nullable.GetValueOrDefault() != 1 ? true : !nullable.HasValue))
					{
						continue;
					}
					DbSet<CouponSettingInfo> couponSettingInfos = this.context.CouponSettingInfo;
					CouponSettingInfo couponSettingInfo1 = new CouponSettingInfo()
					{
						CouponID = couponSettingInfo.CouponID,
						Display = couponSettingInfo.Display,
						PlatForm = PlatformType.Mobile
					};
					couponSettingInfos.Add(couponSettingInfo1);
				}
				else
				{
					display.Display = couponSettingInfo.Display;
				}
			}
			this.context.SaveChanges();
		}

		public void SaveVShopSetting(WXShopInfo wxShop)
		{
			if (this.GetVShopSetting(wxShop.ShopId) == null)
			{
				this.AddVShopSetting(wxShop);
				return;
			}
			this.UpdateVShopSetting(wxShop);
		}

		public void SetHotShop(long vshopId)
		{
			if (this.GetAllVshop().ToList().Where((VShopInfo a) => {
				if (a.VShopExtendInfo == null)
				{
					return false;
				}
				return a.VShopExtendInfo.Any((VShopExtendInfo item) => item.Type == VShopExtendInfo.VShopExtendType.HotVShop);
			}).Count() >= 60)
			{
				throw new HimallException("热门微店最多为60个");
			}
			if ((
				from a in this.GetVshopById(vshopId).VShopExtendInfo
				where a.Type == VShopExtendInfo.VShopExtendType.HotVShop
				select a).Count() >= 1)
			{
				throw new HimallException("该微店已经是热门微店");
			}
			if ((
				from a in this.context.VShopExtendInfo
				where a.VShopId == vshopId
				select a).Count() >= 1)
			{
				this.context.VShopExtendInfo.OrderBy((VShopExtendInfo item) => item.VShopId == vshopId);
			}
			DbSet<VShopExtendInfo> vShopExtendInfo = this.context.VShopExtendInfo;
			VShopExtendInfo vShopExtendInfo1 = new VShopExtendInfo()
			{
				VShopId = vshopId,
				AddTime = DateTime.Now,
				Type = VShopExtendInfo.VShopExtendType.HotVShop,
				Sequence = new int?(1)
			};
			vShopExtendInfo.Add(vShopExtendInfo1);
			this.context.SaveChanges();
		}

		public void SetTopShop(long vshopId)
		{
			this.GetVshopById(vshopId);
			if ((
				from a in this.context.VShopExtendInfo
				where (int)a.Type == 1
				select a).Count() == 1)
			{
				VShopExtendInfo vShopExtendInfo = (
					from a in this.context.VShopExtendInfo
					where (int)a.Type == 1
					select a).FirstOrDefault();
				this.context.VShopExtendInfo.Remove(vShopExtendInfo);
			}
			if ((
				from a in this.context.VShopExtendInfo
				where a.VShopId == vshopId
				select a).Count() >= 1)
			{
				this.context.VShopExtendInfo.OrderBy((VShopExtendInfo item) => item.VShopId == vshopId);
			}
			DbSet<VShopExtendInfo> vShopExtendInfos = this.context.VShopExtendInfo;
			VShopExtendInfo vShopExtendInfo1 = new VShopExtendInfo()
			{
				VShopId = vshopId,
				Type = VShopExtendInfo.VShopExtendType.TopShow,
				AddTime = DateTime.Now
			};
			vShopExtendInfos.Add(vShopExtendInfo1);
			this.context.SaveChanges();
		}

		public void UpdateSequence(long vshopId, int? sequence)
		{
			VShopExtendInfo vShopExtendInfo = this.context.VShopExtendInfo.FindBy((VShopExtendInfo item) => item.VShopId == vshopId && (int)item.Type == 2).FirstOrDefault();
			vShopExtendInfo.Sequence = sequence;
			this.context.SaveChanges();
		}

		public void UpdateVShop(VShopInfo vshopInfo)
		{
			if (vshopInfo.Id <= 0)
			{
				throw new InvalidPropertyException("请传入合法的微店Id，微店Id必须大于0");
			}
			if (string.IsNullOrWhiteSpace(vshopInfo.Logo))
			{
				throw new InvalidPropertyException("微店Logo不能为空");
			}
			if (string.IsNullOrWhiteSpace(vshopInfo.WXLogo))
			{
				throw new InvalidPropertyException("微信Logo不能为空");
			}
			if (string.IsNullOrWhiteSpace(vshopInfo.BackgroundImage))
			{
				throw new InvalidPropertyException("微店背景图片不能为空");
			}
			if (string.IsNullOrWhiteSpace(vshopInfo.Description))
			{
				throw new InvalidPropertyException("微店描述不能为空");
			}
			if (!string.IsNullOrWhiteSpace(vshopInfo.Tags))
			{
				string tags = vshopInfo.Tags;
				char[] chrArray = new char[] { ',' };
				if (tags.Split(chrArray).Length > 4)
				{
					throw new InvalidPropertyException("最多只能有4个标签");
				}
			}
			VShopInfo vshopById = this.GetVshopById(vshopInfo.Id);
			if (vshopById.ShopId != vshopInfo.ShopId)
			{
				throw new InvalidPropertyException("修改微店信息时，不能变更所属店铺");
			}
			vshopById.HomePageTitle = vshopInfo.HomePageTitle;
			vshopById.Tags = vshopInfo.Tags;
			vshopById.Description = vshopInfo.Description;
			string logo = vshopInfo.Logo;
			string backgroundImage = vshopInfo.BackgroundImage;
			string wXLogo = vshopInfo.WXLogo;
			this.CopyImages(vshopInfo.ShopId, ref logo, ref backgroundImage, ref wXLogo);
			vshopById.Logo = logo;
			vshopById.BackgroundImage = backgroundImage;
			vshopById.WXLogo = wXLogo;
			this.context.SaveChanges();
		}

		private void UpdateVShopSetting(WXShopInfo vshopSetting)
		{
			WXShopInfo vShopSetting = this.GetVShopSetting(vshopSetting.ShopId);
			if (string.IsNullOrEmpty(vshopSetting.AppId))
			{
				throw new HimallException("微信AppId不能为空！");
			}
			if (string.IsNullOrEmpty(vshopSetting.AppSecret))
			{
				throw new HimallException("微信AppSecret不能为空！");
			}
			vShopSetting.ShopId = vshopSetting.ShopId;
			vShopSetting.AppId = vshopSetting.AppId;
			vShopSetting.AppSecret = vshopSetting.AppSecret;
			vShopSetting.FollowUrl = vshopSetting.FollowUrl;
			this.context.SaveChanges();
		}
	}
}