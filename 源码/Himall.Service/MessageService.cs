using Himall.Core;
using Himall.Core.Plugins;
using Himall.Core.Plugins.Message;
using Himall.Entity;
using Himall.IServices;
using Himall.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Himall.Service
{
	public class MessageService : ServiceBase, IMessageService, IService, IDisposable
	{
		public MessageService()
		{
		}

		public string GetDestination(long userId, string pluginId, MemberContactsInfo.UserTypes type)
		{
			MemberContactsInfo memberContactsInfo = (
				from a in context.MemberContactsInfo
				where a.UserId == userId && (a.ServiceProvider == pluginId) && (int)a.UserType == (int)type
				select a).FirstOrDefault();
			if (memberContactsInfo == null)
			{
				return "";
			}
			return memberContactsInfo.Contact;
		}

		public MemberContactsInfo GetMemberContactsInfo(string pluginId, string contact, MemberContactsInfo.UserTypes type)
		{
			return (
				from a in context.MemberContactsInfo
				where (a.ServiceProvider == pluginId) && (int)a.UserType == (int)type && (a.Contact == contact)
				select a).FirstOrDefault();
		}

		public void SendMessageCode(string destination, string pluginId, MessageUserInfo info)
		{
			Plugin<IMessagePlugin> plugin = PluginsManagement.GetPlugin<IMessagePlugin>(pluginId);
			if (string.IsNullOrEmpty(destination) || !plugin.Biz.CheckDestination(destination))
			{
				throw new HimallException(string.Concat(plugin.Biz.ShortName, "错误"));
			}
			string str = plugin.Biz.SendMessageCode(destination, info);
			if (plugin.Biz.EnableLog)
			{
				DbSet<MessageLog> messageLog = context.MessageLog;
				MessageLog messageLog1 = new MessageLog()
				{
					SendTime = new DateTime?(DateTime.Now),
					ShopId = new long?(0),
					MessageContent = str,
					TypeId = "短信"
				};
				messageLog.Add(messageLog1);
                context.SaveChanges();
			}
		}

		public void SendMessageOnFindPassWord(long userId, MessageUserInfo info)
		{
			foreach (Plugin<IMessagePlugin> list in PluginsManagement.GetPlugins<IMessagePlugin>().ToList<Plugin<IMessagePlugin>>())
			{
				if (list.Biz.GetStatus(MessageTypeEnum.FindPassWord) != StatusEnum.Open)
				{
					continue;
				}
				string destination = GetDestination(userId, list.PluginInfo.PluginId, MemberContactsInfo.UserTypes.General);
				if (!list.Biz.CheckDestination(destination))
				{
					throw new HimallException(string.Concat(list.Biz.ShortName, "错误"));
				}
				string str = list.Biz.SendMessageOnFindPassWord(destination, info);
				if (!list.Biz.EnableLog)
				{
					continue;
				}
				DbSet<MessageLog> messageLog = context.MessageLog;
				MessageLog messageLog1 = new MessageLog()
				{
					SendTime = new DateTime?(DateTime.Now),
					ShopId = new long?(0),
					MessageContent = str,
					TypeId = "短信"
				};
				messageLog.Add(messageLog1);
                context.SaveChanges();
			}
		}

		public void SendMessageOnOrderCreate(long userId, MessageOrderInfo info)
		{
			foreach (Plugin<IMessagePlugin> list in PluginsManagement.GetPlugins<IMessagePlugin>().ToList<Plugin<IMessagePlugin>>())
			{
				if (list.Biz.GetStatus(MessageTypeEnum.OrderCreated) != StatusEnum.Open)
				{
					continue;
				}
				string destination = GetDestination(userId, list.PluginInfo.PluginId, MemberContactsInfo.UserTypes.General);
				if (!list.Biz.CheckDestination(destination))
				{
					throw new HimallException(string.Concat(list.Biz.ShortName, "错误"));
				}
				string str = list.Biz.SendMessageOnOrderCreate(destination, info);
				if (!list.Biz.EnableLog)
				{
					continue;
				}
				DbSet<MessageLog> messageLog = context.MessageLog;
				MessageLog messageLog1 = new MessageLog()
				{
					SendTime = new DateTime?(DateTime.Now),
					ShopId = new long?(info.ShopId),
					MessageContent = str,
					TypeId = "短信"
				};
				messageLog.Add(messageLog1);
                context.SaveChanges();
			}
		}

		public void SendMessageOnOrderPay(long userId, MessageOrderInfo info)
		{
			foreach (Plugin<IMessagePlugin> list in PluginsManagement.GetPlugins<IMessagePlugin>().ToList<Plugin<IMessagePlugin>>())
			{
				if (list.Biz.GetStatus(MessageTypeEnum.OrderPay) != StatusEnum.Open)
				{
					continue;
				}
				string destination = GetDestination(userId, list.PluginInfo.PluginId, MemberContactsInfo.UserTypes.General);
				if (!list.Biz.CheckDestination(destination))
				{
					throw new HimallException(string.Concat(list.Biz.ShortName, "错误"));
				}
				string str = list.Biz.SendMessageOnOrderPay(destination, info);
				if (!list.Biz.EnableLog)
				{
					continue;
				}
				DbSet<MessageLog> messageLog = context.MessageLog;
				MessageLog messageLog1 = new MessageLog()
				{
					SendTime = new DateTime?(DateTime.Now),
					ShopId = new long?(info.ShopId),
					MessageContent = str,
					TypeId = "短信"
				};
				messageLog.Add(messageLog1);
                context.SaveChanges();
			}
		}

		public void SendMessageOnOrderRefund(long userId, MessageOrderInfo info)
		{
			foreach (Plugin<IMessagePlugin> list in PluginsManagement.GetPlugins<IMessagePlugin>().ToList<Plugin<IMessagePlugin>>())
			{
				if (list.Biz.GetStatus(MessageTypeEnum.OrderRefund) != StatusEnum.Open)
				{
					continue;
				}
				string destination = GetDestination(userId, list.PluginInfo.PluginId, MemberContactsInfo.UserTypes.General);
				if (!list.Biz.CheckDestination(destination))
				{
					throw new HimallException(string.Concat(list.Biz.ShortName, "错误"));
				}
				string str = list.Biz.SendMessageOnOrderRefund(destination, info);
				if (!list.Biz.EnableLog)
				{
					continue;
				}
				DbSet<MessageLog> messageLog = context.MessageLog;
				MessageLog messageLog1 = new MessageLog()
				{
					SendTime = new DateTime?(DateTime.Now),
					ShopId = new long?(info.ShopId),
					MessageContent = str,
					TypeId = "短信"
				};
				messageLog.Add(messageLog1);
                context.SaveChanges();
			}
		}

		public void SendMessageOnOrderShipping(long userId, MessageOrderInfo info)
		{
			foreach (Plugin<IMessagePlugin> list in PluginsManagement.GetPlugins<IMessagePlugin>().ToList<Plugin<IMessagePlugin>>())
			{
				if (list.Biz.GetStatus(MessageTypeEnum.OrderShipping) != StatusEnum.Open)
				{
					continue;
				}
				string destination = GetDestination(userId, list.PluginInfo.PluginId, MemberContactsInfo.UserTypes.General);
				if (!list.Biz.CheckDestination(destination))
				{
					throw new HimallException(string.Concat(list.Biz.ShortName, "错误"));
				}
				string str = list.Biz.SendMessageOnOrderShipping(destination, info);
				if (!list.Biz.EnableLog)
				{
					continue;
				}
				DbSet<MessageLog> messageLog = context.MessageLog;
				MessageLog messageLog1 = new MessageLog()
				{
					SendTime = new DateTime?(DateTime.Now),
					ShopId = new long?(info.ShopId),
					MessageContent = str,
					TypeId = "短信"
				};
				messageLog.Add(messageLog1);
                context.SaveChanges();
			}
		}

		public void SendMessageOnShopAudited(long userId, MessageShopInfo info)
		{
			foreach (Plugin<IMessagePlugin> list in PluginsManagement.GetPlugins<IMessagePlugin>().ToList<Plugin<IMessagePlugin>>())
			{
				if (list.Biz.GetStatus(MessageTypeEnum.ShopAudited) != StatusEnum.Open)
				{
					continue;
				}
				string destination = GetDestination(userId, list.PluginInfo.PluginId, MemberContactsInfo.UserTypes.ShopManager);
				if (!list.Biz.CheckDestination(destination))
				{
					throw new HimallException(string.Concat(list.Biz.ShortName, "错误"));
				}
				string str = list.Biz.SendMessageOnShopAudited(destination, info);
				if (!list.Biz.EnableLog)
				{
					continue;
				}
				DbSet<MessageLog> messageLog = context.MessageLog;
				MessageLog messageLog1 = new MessageLog()
				{
					SendTime = new DateTime?(DateTime.Now),
					ShopId = new long?(0),
					MessageContent = str,
					TypeId = "短信"
				};
				messageLog.Add(messageLog1);
                context.SaveChanges();
			}
		}

		public void SendMessageOnShopSuccess(long userId, MessageShopInfo info)
		{
			foreach (Plugin<IMessagePlugin> list in PluginsManagement.GetPlugins<IMessagePlugin>().ToList<Plugin<IMessagePlugin>>())
			{
				if (list.Biz.GetStatus(MessageTypeEnum.ShopSuccess) != StatusEnum.Open)
				{
					continue;
				}
				string destination = GetDestination(userId, list.PluginInfo.PluginId, MemberContactsInfo.UserTypes.ShopManager);
				if (!list.Biz.CheckDestination(destination))
				{
					throw new HimallException(string.Concat(list.Biz.ShortName, "错误"));
				}
				string str = list.Biz.SendMessageOnShopSuccess(destination, info);
				if (!list.Biz.EnableLog)
				{
					continue;
				}
				DbSet<MessageLog> messageLog = context.MessageLog;
				MessageLog messageLog1 = new MessageLog()
				{
					SendTime = new DateTime?(DateTime.Now),
					ShopId = new long?(0),
					MessageContent = str,
					TypeId = "短信"
				};
				messageLog.Add(messageLog1);
                context.SaveChanges();
			}
		}

		public void UpdateMemberContacts(MemberContactsInfo info)
		{
			MemberContactsInfo contact = context.MemberContactsInfo.FirstOrDefault((MemberContactsInfo a) => (a.ServiceProvider == info.ServiceProvider) && a.UserId == info.UserId && (int)a.UserType == (int)info.UserType);
			if (contact == null)
			{
                context.MemberContactsInfo.Add(info);
			}
			else
			{
				contact.Contact = info.Contact;
			}
            context.SaveChanges();
		}
	}
}