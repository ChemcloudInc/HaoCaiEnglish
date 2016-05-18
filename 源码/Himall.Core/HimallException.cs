using System;

namespace Himall.Core
{
	public class HimallException : ApplicationException
	{
		public HimallException()
		{
			Log.Info(Message, this);
		}

		public HimallException(string message) : base(message)
		{
			Log.Info(message, this);
		}

		public HimallException(string message, Exception inner) : base(message, inner)
		{
			Log.Info(message, inner);
		}
	}
}