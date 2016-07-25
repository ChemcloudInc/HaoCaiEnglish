using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class SensitiveWordsInfo : BaseModel
	{
		private int _id;

		public string CategoryName
		{
			get;
			set;
		}

		public new int Id
		{
			get
			{
				return _id;
			}
			set
			{
                _id = value;
				base.Id = value;
			}
		}

		public string SensitiveWord
		{
			get;
			set;
		}

		public SensitiveWordsInfo()
		{
		}
	}
}