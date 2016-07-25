using System;
using System.Runtime.CompilerServices;

namespace Himall.Service
{
	public class ReceiveModel
	{
		public decimal Price
		{
			get;
			set;
		}

		public ReceiveStatus State
		{
			get;
			set;
		}

		public ReceiveModel()
		{
		}
	}
}