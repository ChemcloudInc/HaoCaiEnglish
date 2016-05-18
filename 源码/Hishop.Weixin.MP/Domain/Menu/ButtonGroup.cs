using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Hishop.Weixin.MP.Domain.Menu
{
	public class ButtonGroup
	{
		public List<BaseButton> button
		{
			get;
			set;
		}

		public ButtonGroup()
		{
            button = new List<BaseButton>();
		}
	}
}