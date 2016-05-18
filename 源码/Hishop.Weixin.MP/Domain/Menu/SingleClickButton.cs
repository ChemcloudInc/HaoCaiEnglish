using Hishop.Weixin.MP;
using System;
using System.Runtime.CompilerServices;

namespace Hishop.Weixin.MP.Domain.Menu
{
	public class SingleClickButton : SingleButton
	{
		public string key
		{
			get;
			set;
		}

		public SingleClickButton() : base(ButtonType.click.ToString())
		{
		}
	}
}