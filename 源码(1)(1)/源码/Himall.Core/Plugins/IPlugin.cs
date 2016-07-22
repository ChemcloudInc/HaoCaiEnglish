using System;

namespace Himall.Core.Plugins
{
	public interface IPlugin
	{
		string WorkDirectory
		{
			set;
		}

		void CheckCanEnable();
	}
}