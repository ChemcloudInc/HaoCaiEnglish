using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class CollocationPoruductModel
	{
		public List<Himall.Model.CollocationSkus> CollocationSkus
		{
			get;
			set;
		}

		public long ColloId
		{
			get;
			set;
		}

		public int DisplaySequence
		{
			get;
			set;
		}

		public long Id
		{
			get;
			set;
		}

		public string ImagePath
		{
			get;
			set;
		}

		public bool IsMain
		{
			get;
			set;
		}

		public long ProductId
		{
			get;
			set;
		}

		public string ProductName
		{
			get;
			set;
		}

		public CollocationPoruductModel()
		{
		}
	}
}