using Hishop.Weixin.MP;
using System;
using System.Runtime.CompilerServices;

namespace Hishop.Weixin.MP.Request
{
	public class LocationRequest : AbstractRequest
	{
		public string Label
		{
			get;
			set;
		}

		public float Location_X
		{
			get;
			set;
		}

		public float Location_Y
		{
			get;
			set;
		}

		public int Scale
		{
			get;
			set;
		}

		public LocationRequest()
		{
		}
	}
}