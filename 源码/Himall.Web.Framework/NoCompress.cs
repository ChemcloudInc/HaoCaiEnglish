using System;

namespace Himall.Web.Framework
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple=false, Inherited=false)]
	public class NoCompress : Attribute
	{
		public NoCompress()
		{
		}
	}
}