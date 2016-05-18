using Himall.Model;
using System;

namespace Himall.IServices
{
	public interface ISiteSettingService : IService, IDisposable
	{
		SiteSettingsInfo GetSiteSettings();

		void SaveSetting(string key, object value);

		void SetSiteSettings(SiteSettingsInfo siteSettingsInfo);
	}
}