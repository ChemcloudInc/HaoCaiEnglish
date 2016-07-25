using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;

namespace Himall.Web.App_Code.UEditor
{
	public class UploadHandle : IUEditorHandle
	{
		private UploadResult Result
		{
			get;
			set;
		}

		private Himall.Web.App_Code.UEditor.UploadConfig UploadConfig
		{
			get;
			set;
		}

		public UploadHandle(Himall.Web.App_Code.UEditor.UploadConfig config)
		{
            UploadConfig = config;
            Result = new UploadResult()
			{
				State = UploadState.Unknown
			};
		}

		private bool CheckFileSize(int size)
		{
			return size < UploadConfig.SizeLimit;
		}

		private bool CheckFileType(string filename)
		{
			string lower = Path.GetExtension(filename).ToLower();
			return (
				from x in UploadConfig.AllowExtensions
				select x.ToLower()).Contains<string>(lower);
		}

		private string GetStateMessage(UploadState state)
		{
			switch (state)
			{
				case UploadState.NetworkError:
				{
					return "网络错误";
				}
				case UploadState.FileAccessError:
				{
					return "文件访问出错，请检查写入权限";
				}
				case UploadState.TypeNotAllow:
				{
					return "不允许的文件格式";
				}
				case UploadState.SizeLimitExceed:
				{
					return "文件大小超出服务器限制";
				}
				case UploadState.Success:
				{
					return "SUCCESS";
				}
			}
			return "未知错误";
		}

		public object Process()
		{
			byte[] numArray = null;
			string fileName = null;
			if (!UploadConfig.Base64)
			{
				HttpPostedFile item = HttpContext.Current.Request.Files[UploadConfig.UploadFieldName];
				fileName = item.FileName;
				if (!CheckFileType(fileName))
				{
                    Result.State = UploadState.TypeNotAllow;
					return WriteResult();
				}
				if (!CheckFileSize(item.ContentLength))
				{
                    Result.State = UploadState.SizeLimitExceed;
					return WriteResult();
				}
				numArray = new byte[item.ContentLength];
				try
				{
					item.InputStream.Read(numArray, 0, item.ContentLength);
				}
				catch (Exception exception)
				{
                    Result.State = UploadState.NetworkError;
                    WriteResult();
				}
			}
			else
			{
				fileName = UploadConfig.Base64Filename;
				numArray = Convert.FromBase64String(HttpContext.Current.Request[UploadConfig.UploadFieldName]);
			}
            Result.OriginFileName = fileName;
			string str = PathFormatter.Format(fileName, UploadConfig.PathFormat);
			string str1 = HttpContext.Current.Server.MapPath(str);
			try
			{
				if (!Directory.Exists(Path.GetDirectoryName(str1)))
				{
					Directory.CreateDirectory(Path.GetDirectoryName(str1));
				}
				File.WriteAllBytes(str1, numArray);
                Result.Url = str;
                Result.State = UploadState.Success;
			}
			catch (Exception exception2)
			{
				Exception exception1 = exception2;
                Result.State = UploadState.FileAccessError;
                Result.ErrorMessage = exception1.Message;
			}
			return WriteResult();
		}

		private object WriteResult()
		{
			return new { state = GetStateMessage(Result.State), url = Result.Url, title = Result.OriginFileName, original = Result.OriginFileName, error = Result.ErrorMessage };
		}
	}
}