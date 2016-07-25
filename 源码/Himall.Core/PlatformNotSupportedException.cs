using System;

namespace Himall.Core
{
	public class PlatformNotSupportedException : HimallException
	{
		public PlatformNotSupportedException(PlatformType platformType) : base(string.Concat("不支持", platformType.ToDescription(), "平台"))
		{
			Log.Info(Message, this);
		}
	}
}