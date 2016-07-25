using System;

namespace Himall.Web.App_Code.UEditor
{
	public enum UploadState
	{
		NetworkError = -4,
		FileAccessError = -3,
		TypeNotAllow = -2,
		SizeLimitExceed = -1,
		Success = 0,
		Unknown = 1
	}
}