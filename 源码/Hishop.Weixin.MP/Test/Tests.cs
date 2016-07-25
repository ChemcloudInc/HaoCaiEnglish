using Hishop.Weixin.MP.Api;
using Hishop.Weixin.MP.Domain;
using Hishop.Weixin.MP.Domain.Menu;
using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace Hishop.Weixin.MP.Test
{
	public class Tests
	{
		private const string AppID = "wxe7322013e6e964b8";

		private const string AppSecret = "9e4e5617c1b543e3164befd1952716b0";

		public Tests()
		{
		}

		public string CreateMenu()
		{
			string token = GetToken();
			return MenuApi.CreateMenus(token, GetMenuJson());
		}

		public string DeleteMenu()
		{
			return MenuApi.DeleteMenus(GetToken());
		}

		public string GetMenu()
		{
			return MenuApi.GetMenus(GetToken());
		}

		public string GetMenuJson()
		{
			Menu menu = new Menu();
			SingleClickButton singleClickButton = new SingleClickButton()
			{
				name = "热卖商品",
				key = "123"
			};
			SingleClickButton singleClickButton1 = singleClickButton;
			SingleClickButton singleClickButton2 = new SingleClickButton()
			{
				name = "推荐商品",
				key = "SINGER"
			};
			SingleClickButton singleClickButton3 = singleClickButton2;
			SingleViewButton singleViewButton = new SingleViewButton()
			{
				name = "会员卡",
				url = "www.baidu.com"
			};
			SingleViewButton singleViewButton1 = singleViewButton;
			SingleViewButton singleViewButton2 = new SingleViewButton()
			{
				name = "积分商城",
				url = "www.baidu.com"
			};
			SingleViewButton singleViewButton3 = singleViewButton2;
			SubMenu subMenu = new SubMenu()
			{
				name = "个人中心"
			};
			subMenu.sub_button.Add(singleViewButton1);
			subMenu.sub_button.Add(singleViewButton3);
			menu.menu.button.Add(singleClickButton1);
			menu.menu.button.Add(singleClickButton3);
			menu.menu.button.Add(subMenu);
			return (new JavaScriptSerializer()).Serialize(menu.menu);
		}

		public string GetToken()
		{
			string token = TokenApi.GetToken("wxe7322013e6e964b8", "9e4e5617c1b543e3164befd1952716b0");
			return (new JavaScriptSerializer()).Deserialize<Token>(token).access_token;
		}
	}
}