using System;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.SellerAdmin.Controllers
{
	public class PoiEditModel
	{
		public string address
		{
			get;
			set;
		}

		public int avg_price
		{
			get;
			set;
		}

		public string branch_name
		{
			get;
			set;
		}

		public string business_name
		{
			get;
			set;
		}

		public string categoryOne
		{
			get;
			set;
		}

		public string categoryTwo
		{
			get;
			set;
		}

		public string city
		{
			get;
			set;
		}

		public string district
		{
			get;
			set;
		}

		public int id
		{
			get;
			set;
		}

		public string introduction
		{
			get;
			set;
		}

		public string open_time
		{
			get;
			set;
		}

		public string photo_list
		{
			get;
			set;
		}

		public string poi_id
		{
			get;
			set;
		}

		public string province
		{
			get;
			set;
		}

		public string recommend
		{
			get;
			set;
		}

		public string special
		{
			get;
			set;
		}

		public string telephone
		{
			get;
			set;
		}

		public PoiEditModel()
		{
            province = "";
            city = "";
            district = "";
            address = "";
            business_name = "";
            branch_name = "";
            categoryOne = "";
            categoryTwo = "";
            photo_list = "";
            telephone = "";
            open_time = "";
            recommend = "";
            special = "";
            introduction = "";
		}
	}
}