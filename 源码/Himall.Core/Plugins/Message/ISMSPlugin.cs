using Himall.Core.Plugins;
using System;

namespace Himall.Core.Plugins.Message
{
	public interface ISMSPlugin : IMessagePlugin, IPlugin
	{
		string GetBuyLink();

		string GetLoginLink();

		string GetSMSAmount();
	}
}