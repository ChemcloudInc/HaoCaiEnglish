using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class UserInviteModel
	{
		public int InviteIntergralCount
		{
			get;
			set;
		}

		public string InviteLink
		{
			get;
			set;
		}

		public int InvitePersonCount
		{
			get;
			set;
		}

		public string QR
		{
			get;
			set;
		}

		public UserInviteModel()
		{
		}
	}
}