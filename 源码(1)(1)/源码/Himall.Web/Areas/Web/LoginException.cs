using Himall.Core;
using System;

namespace Himall.Web.Areas.Web
{
	internal class LoginException : HimallException
	{
		private LoginException.ErrorTypes _errrorType;

		public LoginException.ErrorTypes ErrorType
		{
			get
			{
				return _errrorType;
			}
		}

		public LoginException(string msg, LoginException.ErrorTypes errorType) : base(msg)
		{
            _errrorType = errorType;
		}

		public enum ErrorTypes
		{
			UsernameError,
			PasswordError,
			CheckCodeError
		}
	}
}