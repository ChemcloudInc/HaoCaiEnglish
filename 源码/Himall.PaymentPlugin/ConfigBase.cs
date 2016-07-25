using Himall.Core;
using System;
using System.Runtime.CompilerServices;

namespace Himall.PaymentPlugin
{
	public abstract class ConfigBase
	{
		public SerializableDictionary<PlatformType, bool> OpenStatus
		{
			get;
			set;
		}

		public PlatformType[] SupportPlatfoms
		{
			get;
			set;
		}

		protected ConfigBase()
		{
		}
	}
}