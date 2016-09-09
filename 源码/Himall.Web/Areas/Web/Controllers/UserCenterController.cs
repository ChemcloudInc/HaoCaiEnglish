using Himall.Core;
using Himall.Core.Plugins;
using Himall.Core.Plugins.Message;
using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using Himall.ServiceProvider;
using Himall.Web.Areas.Web;
using Himall.Web.Areas.Web.Models;
using Himall.Web.Framework;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Himall.Web.Areas.Web.Controllers
{
	public class UserCenterController : BaseMemberController
	{
		public UserCenterController()
		{
		}

		public ActionResult AccountSafety()
		{
			MemberAccountSafety memberAccountSafety = new MemberAccountSafety()
			{
				AccountSafetyLevel = 1
			};
			if (base.CurrentUser.PayPwd != null)
			{
				memberAccountSafety.PayPassword = true;
				MemberAccountSafety accountSafetyLevel = memberAccountSafety;
				accountSafetyLevel.AccountSafetyLevel = accountSafetyLevel.AccountSafetyLevel + 1;
			}
			IEnumerable<Plugin<IMessagePlugin>> plugins = PluginsManagement.GetPlugins<IMessagePlugin>();
			IMessageService create = Instance<IMessageService>.Create;
			IEnumerable<PluginsInfo> pluginsInfo = 
				from item in plugins
				select new PluginsInfo()
				{
					ShortName = item.Biz.ShortName,
					PluginId = item.PluginInfo.PluginId,
					Enable = item.PluginInfo.Enable,
					IsSettingsValid = item.Biz.IsSettingsValid,
					IsBind = !string.IsNullOrEmpty(create.GetDestination(CurrentUser.Id, item.PluginInfo.PluginId, MemberContactsInfo.UserTypes.General))
				};
			foreach (PluginsInfo pluginsInfo1 in pluginsInfo)
			{
				if (pluginsInfo1.PluginId.IndexOf("SMS") <= 0)
				{
					if (!pluginsInfo1.IsBind)
					{
						continue;
					}
					memberAccountSafety.BindEmail = true;
					MemberAccountSafety accountSafetyLevel1 = memberAccountSafety;
					accountSafetyLevel1.AccountSafetyLevel = accountSafetyLevel1.AccountSafetyLevel + 1;
				}
				else
				{
					if (!pluginsInfo1.IsBind)
					{
						continue;
					}
					memberAccountSafety.BindPhone = true;
					MemberAccountSafety memberAccountSafety1 = memberAccountSafety;
					memberAccountSafety1.AccountSafetyLevel = memberAccountSafety1.AccountSafetyLevel + 1;
				}
			}
			return View(memberAccountSafety);
		}

		public ActionResult Bind(string pluginId)
		{
			Plugin<IMessagePlugin> plugin = PluginsManagement.GetPlugin<IMessagePlugin>(pluginId);
			ViewBag.ShortName = plugin.Biz.ShortName;
			ViewBag.id = pluginId;
			return View();
		}

		[HttpPost]
		public ActionResult CheckCode(string pluginId, string code, string destination)
		{
			string str = CacheKeyCollection.MemberPluginCheck(base.CurrentUser.UserName, pluginId);
			object obj = Cache.Get(str);
			UserMemberInfo currentUser = base.CurrentUser;
			string str1 = "";
			if (obj == null || !(obj.ToString() == code))
			{
				Result result = new Result()
				{
					success = false,
					msg = "Verification code is error or timeout"
				};
				return Json(result);
			}
			IMessageService messageService = ServiceHelper.Create<IMessageService>();
			if (messageService.GetMemberContactsInfo(pluginId, destination, MemberContactsInfo.UserTypes.General) != null)
			{
				Result result1 = new Result()
				{
					success = false,
					msg = string.Concat(destination, "Already Binded！")
				};
				return Json(result1);
			}
			if (pluginId.ToLower().Contains("email"))
			{
				currentUser.Email = destination;
				str1 = "Email";
			}
			else if (pluginId.ToLower().Contains("sms"))
			{
				currentUser.CellPhone = destination;
				str1 = "Mobile";
			}
			ServiceHelper.Create<IMemberService>().UpdateMember(currentUser);
			MemberContactsInfo memberContactsInfo = new MemberContactsInfo()
			{
				Contact = destination,
				ServiceProvider = pluginId,
				UserId = base.CurrentUser.Id,
				UserType = MemberContactsInfo.UserTypes.General
			};
			messageService.UpdateMemberContacts(memberContactsInfo);
			Cache.Remove(CacheKeyCollection.MemberPluginCheck(base.CurrentUser.UserName, pluginId));
			Cache.Remove(CacheKeyCollection.Member(base.CurrentUser.Id));
			Cache.Remove(string.Concat("Rebind", base.CurrentUser.Id));
			UserMemberInfo member = null;
			if (currentUser.InviteUserId.HasValue)
			{
				member = ServiceHelper.Create<IMemberService>().GetMember(currentUser.InviteUserId.Value);
			}
			Task.Factory.StartNew(() => {
				MemberIntegralRecord memberIntegralRecord = new MemberIntegralRecord()
				{
					UserName = currentUser.UserName,
					MemberId = currentUser.Id,
					RecordDate = new DateTime?(DateTime.Now),
					TypeId = MemberIntegral.IntegralType.Reg,
					ReMark = string.Concat("Bind ", str1)
				};
				IConversionMemberIntegralBase conversionMemberIntegralBase = ServiceHelper.Create<IMemberIntegralConversionFactoryService>().Create(MemberIntegral.IntegralType.Reg, 0);
				ServiceHelper.Create<IMemberIntegralService>().AddMemberIntegral(memberIntegralRecord, conversionMemberIntegralBase);
				if (member != null)
				{
					ServiceHelper.Create<IMemberInviteService>().AddInviteIntegel(currentUser, member);
				}
			});
			Result result2 = new Result()
			{
				success = true,
				msg = "Verify Success"
			};
			return Json(result2);
		}

		public ActionResult Finished()
		{
			return View();
		}

		public ActionResult Home()
		{
			string str;
			long num;
			UserCenterModel userCenterModel = ServiceHelper.Create<IMemberService>().GetUserCenterModel(base.CurrentUser.Id);
			dynamic viewBag = base.ViewBag;
			str = (base.CurrentUser.Nick == "" ? base.CurrentUser.UserName : base.CurrentUser.Nick);
			viewBag.UserName = str;
			ViewBag.Logo = base.CurrentUser.Photo;
			long[] array = (
				from a in ServiceHelper.Create<ICartService>().GetCart(base.CurrentUser.Id).Items
				orderby a.AddTime descending
				select a into p
				select p.ProductId).Take(3).ToArray();
			ViewBag.ShoppingCartItems = ServiceHelper.Create<IProductService>().GetProductByIds(array).ToArray();
			OrderItemInfo[] orderItemInfoArray = ServiceHelper.Create<ICommentService>().GetUnEvaluatProducts(base.CurrentUser.Id).ToArray();
			ViewBag.UnEvaluatProductsNum = orderItemInfoArray.Count();
			ViewBag.Top3UnEvaluatProducts = orderItemInfoArray.Take(3).ToArray();
			ViewBag.Top3RecommendProducts = ServiceHelper.Create<IProductService>().GetPlatHotSaleProductByNearShop(8, base.CurrentUser.Id).ToArray();
			dynamic browsingProducts = base.ViewBag;
			num = (base.CurrentUser == null ? 0 : base.CurrentUser.Id);
			browsingProducts.BrowsingProducts = BrowseHistrory.GetBrowsingProducts(4, num);
			IEnumerable<Plugin<IMessagePlugin>> plugins = PluginsManagement.GetPlugins<IMessagePlugin>();
			IEnumerable<PluginsInfo> pluginsInfo = 
				from item in plugins
				select new PluginsInfo()
				{
					ShortName = item.Biz.ShortName,
					PluginId = item.PluginInfo.PluginId,
					Enable = item.PluginInfo.Enable,
					IsSettingsValid = item.Biz.IsSettingsValid,
					IsBind = !string.IsNullOrEmpty(ServiceHelper.Create<IMessageService>().GetDestination(base.CurrentUser.Id, item.PluginInfo.PluginId, MemberContactsInfo.UserTypes.General))
				};
			ViewBag.BindContactInfo = pluginsInfo;
			IOrderService orderService = ServiceHelper.Create<IOrderService>();
			OrderQuery orderQuery = new OrderQuery()
			{
				PageNo = 1,
				PageSize = 2147483647,
				UserId = new long?(base.CurrentUser.Id)
			};
			PageModel<OrderInfo> orders = orderService.GetOrders<OrderInfo>(orderQuery, null);
			ViewBag.OrderCount = orders.Total;
			dynamic obj = base.ViewBag;
			IQueryable<OrderInfo> models = orders.Models;
			obj.OrderWaitReceiving = (
				from c in models
				where (int)c.OrderStatus == 3
				select c).Count();
			dynamic viewBag1 = base.ViewBag;
			IQueryable<OrderInfo> orderInfos = orders.Models;
			viewBag1.OrderWaitPay = (
				from c in orderInfos
				where (int)c.OrderStatus == 1
				select c).Count();
			ICommentService commentService = ServiceHelper.Create<ICommentService>();
			CommentQuery commentQuery = new CommentQuery()
			{
				UserID = base.CurrentUser.Id,
				PageSize = 2147483647,
				PageNo = 1,
				Sort = "PComment"
			};
			IQueryable<long> nums = (
				from item in commentService.GetProductEvaluation(commentQuery).Models
				where !item.EvaluationStatus
				select item.OrderId).Distinct<long>();
			ViewBag.OrderEvaluationStatus = nums.Count();
			CapitalInfo capitalInfo = ServiceHelper.Create<IMemberCapitalService>().GetCapitalInfo(base.CurrentUser.Id);
			decimal value = new decimal(0);
			if (capitalInfo != null && capitalInfo.Balance.HasValue)
			{
				value = capitalInfo.Balance.Value;
			}
			ViewBag.Balance = value;
			MemberAccountSafety memberAccountSafety = new MemberAccountSafety()
			{
				AccountSafetyLevel = 1
			};
			if (base.CurrentUser.PayPwd != null)
			{
				memberAccountSafety.PayPassword = true;
				MemberAccountSafety accountSafetyLevel = memberAccountSafety;
				accountSafetyLevel.AccountSafetyLevel = accountSafetyLevel.AccountSafetyLevel + 1;
			}
			IMessageService create = Instance<IMessageService>.Create;
			foreach (PluginsInfo pluginsInfo1 in pluginsInfo)
			{
				if (pluginsInfo1.PluginId.IndexOf("SMS") <= 0)
				{
					if (!pluginsInfo1.IsBind)
					{
						continue;
					}
					memberAccountSafety.BindEmail = true;
					MemberAccountSafety accountSafetyLevel1 = memberAccountSafety;
					accountSafetyLevel1.AccountSafetyLevel = accountSafetyLevel1.AccountSafetyLevel + 1;
				}
				else
				{
					if (!pluginsInfo1.IsBind)
					{
						continue;
					}
					memberAccountSafety.BindPhone = true;
					MemberAccountSafety memberAccountSafety1 = memberAccountSafety;
					memberAccountSafety1.AccountSafetyLevel = memberAccountSafety1.AccountSafetyLevel + 1;
				}
			}
			userCenterModel.memberAccountSafety = memberAccountSafety;
			return View(userCenterModel);
		}

		public ActionResult Index()
		{
			ViewBag.Logo = base.CurrentSiteSetting.Logo;
			ViewBag.Keyword = base.CurrentSiteSetting.Keyword;
			return View();
		}

		[HttpPost]
		public ActionResult SendCode(string pluginId, string destination)
		{
			ServiceHelper.Create<IMemberService>().CheckContactInfoHasBeenUsed(pluginId, destination, MemberContactsInfo.UserTypes.General);
			if (Cache.Get(CacheKeyCollection.MemberPluginCheckTime(base.CurrentUser.UserName, pluginId)) != null)
			{
				Result result = new Result()
				{
					success = false,
                    msg = "Only allowed to request once in 60 seconds，Please wait and try it again!"
				};
				return Json(result);
			}
            
			int num = (new Random()).Next(10000, 99999);
			DateTime dateTime = DateTime.Now.AddMinutes(15);
			if (pluginId.ToLower().Contains("email"))
			{
				dateTime = DateTime.Now.AddHours(24);
            }
			Cache.Insert(CacheKeyCollection.MemberPluginCheck(base.CurrentUser.UserName, pluginId), num, dateTime);
			MessageUserInfo messageUserInfo = new MessageUserInfo()
			{
				UserName = base.CurrentUser.UserName,
				SiteName = base.CurrentSiteSetting.SiteName,
				CheckCode = num.ToString()
			};
			ServiceHelper.Create<IMessageService>().SendMessageCode(destination, pluginId, messageUserInfo);
			string str = CacheKeyCollection.MemberPluginCheckTime(base.CurrentUser.UserName, pluginId);
			DateTime now = DateTime.Now;
			Cache.Insert(str, "0", now.AddSeconds(110));
			Result result1 = new Result()
			{
				success = true,
				msg = "Send Success"
			};
			return Json(result1);
		}
	}
}