using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
namespace Himall.Web.App_Code.UEditor
{
	public static class Config
	{
		private static bool noCache;

		private static JObject _Items;

		public static JObject Items
		{
			get
			{
				if (Config.noCache || Config._Items == null)
				{
					Config._Items = Config.BuildItems();
				}
				return Config._Items;
			}
		}

		static Config()
		{
			Config.noCache = true;
		}

		private static JObject BuildItems()
		{
			string str = File.ReadAllText(HttpContext.Current.Server.MapPath("/Scripts/ueditor/config.json"));
			return JObject.Parse(str);
		}

		public static int GetInt(string key)
		{
			return Config.GetValue<int>(key);
		}

		public static string GetString(string key)
		{
			return Config.GetValue<string>(key);
		}

		public static string[] GetStringList(string key)
		{
			return (
				from x in Config.Items[key]
				select x.Value<string>()).ToArray();
		}

		public static T GetValue<T>(string key)
		{
			return Config.Items[key].Value<T>();
		}
	}
}