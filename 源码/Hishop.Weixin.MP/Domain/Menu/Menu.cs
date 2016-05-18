using System;
using System.Runtime.CompilerServices;

namespace Hishop.Weixin.MP.Domain.Menu
{
	public class Menu
	{
		public ButtonGroup menu
		{
			get;
			set;
		}

		public Menu()
		{
            menu = new ButtonGroup();
		}
	}
}