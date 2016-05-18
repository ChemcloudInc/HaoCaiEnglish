using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public abstract class BaseModel
	{
		protected string ImageServerUrl = "";

		public object Id
		{
			get;
			set;
		}

		protected BaseModel()
		{
		}
	}
}