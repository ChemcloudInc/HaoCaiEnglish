using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class MemberGrade : BaseModel
	{
		private long _id;

		public string GradeName
		{
			get;
			set;
		}

		public new long Id
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

		public int Integral
		{
			get;
			set;
		}

		public bool IsNoDelete
		{
			get;
			set;
		}

		public string Remark
		{
			get;
			set;
		}

		public MemberGrade()
		{
		}
	}
}