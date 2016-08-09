using System;

namespace Himall.Web.App_Code.UEditor
{
	public class ConfigHandler : IUEditorHandle
	{
		public ConfigHandler()
		{
		}

		public object Process()
		{
			return Config.Items;
		}
	}
}