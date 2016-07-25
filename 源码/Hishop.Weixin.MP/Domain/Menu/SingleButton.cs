using System;
using System.Runtime.CompilerServices;

namespace Hishop.Weixin.MP.Domain.Menu
{
	public abstract class SingleButton : BaseButton
	{
		public string type
		{
			get;
			set;
		}

		public SingleButton(string theType)
		{
            type = theType;
		}
	}
}