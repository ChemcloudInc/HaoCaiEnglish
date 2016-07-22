using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class CollocationDataModel
	{
		public List<CollocationPoruductModel> CollocationPoruducts
		{
			get;
			set;
		}

		public DateTime CreateTime
		{
			get;
			set;
		}

		public DateTime EndTime
		{
			get;
			set;
		}

		public long Id
		{
			get;
			set;
		}

		public long ShopId
		{
			get;
			set;
		}

		public string ShortDesc
		{
			get;
			set;
		}

		public DateTime StartTime
		{
			get;
			set;
		}

		public string Title
		{
			get;
			set;
		}

		public CollocationDataModel()
		{
		}
	}
}