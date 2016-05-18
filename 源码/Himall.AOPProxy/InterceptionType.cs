using System;

namespace Himall.AOPProxy
{
	public enum InterceptionType
	{
		OnEntry,
		OnExit,
		OnSuccess,
		OnException,
		OnLogException
	}
}