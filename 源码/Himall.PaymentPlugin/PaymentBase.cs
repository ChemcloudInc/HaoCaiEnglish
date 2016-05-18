using Himall.Core;
using Himall.Core.Plugins;
using Himall.Core.Plugins.Payment;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Himall.PaymentPlugin
{
	public class PaymentBase<T>
	where T : ConfigBase, new()
	{
		private static Dictionary<string, string> workDirectories;

		public IEnumerable<PlatformType> SupportPlatforms
		{
			get
			{
				T config = Utility<T>.GetConfig(WorkDirectory);
				PlatformType[] supportPlatfoms = config.SupportPlatfoms;
				if ((supportPlatfoms == null ? true : supportPlatfoms.Count() == 0))
				{
					config.SupportPlatfoms = new PlatformType[1];
					Utility<T>.SaveConfig(config, WorkDirectory);
				}
				return config.SupportPlatfoms;
			}
		}

		public string WorkDirectory
		{
			get
			{
				string item;
				string fullName = GetType().FullName;
				Log.Info(string.Concat("get WorkDirectory:", fullName));
				if ((string.IsNullOrWhiteSpace(fullName) ? true : !PaymentBase<T>.workDirectories.ContainsKey(fullName)))
				{
					item = null;
				}
				else
				{
					item = PaymentBase<T>.workDirectories[fullName];
				}
				return item;
			}
			set
			{
				string fullName = GetType().FullName;
				Log.Info(string.Concat("set WorkDirectory:", fullName));
				if (!PaymentBase<T>.workDirectories.ContainsKey(fullName))
				{
					PaymentBase<T>.workDirectories.Add(fullName, value);
				}
			}
		}

		static PaymentBase()
		{
			PaymentBase<T>.workDirectories = new Dictionary<string, string>();
		}

		public PaymentBase()
		{
		}

		public void Disable(PlatformType platform)
		{
            EnableOrDisable(platform, false);
		}

		public void Enable(PlatformType platform)
		{
            EnableOrDisable(platform, true);
		}

		private void EnableOrDisable(PlatformType platform, bool enable)
		{
			T config = Utility<T>.GetConfig(WorkDirectory);
			if (!config.SupportPlatfoms.Contains<PlatformType>(platform))
			{
				throw new Himall.Core.PlatformNotSupportedException(platform);
			}
			if (!config.OpenStatus.ContainsKey(platform))
			{
				config.OpenStatus.Add(platform, enable);
			}
			else
			{
				config.OpenStatus[platform] = enable;
			}
			Utility<T>.SaveConfig(config, WorkDirectory);
		}

		public virtual PaymentInfo EnterprisePay(EnterprisePayPara para)
		{
			throw new PluginException("未实现此方法");
		}

		public bool IsEnable(PlatformType platform)
		{
			T config = Utility<T>.GetConfig(WorkDirectory);
			return (!config.OpenStatus.ContainsKey(platform) ? false : config.OpenStatus[platform]);
		}

		public virtual PaymentInfo ProcessRefundFee(PaymentPara para)
		{
			throw new PluginException("未实现此方法");
		}
	}
}