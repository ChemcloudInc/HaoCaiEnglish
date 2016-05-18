using System;
using System.IO;

namespace Himall.WeixinPaymentBase
{
	public class FileHelper
	{
		public FileHelper()
		{
		}

		public static FileStream GetFileStream(string fileName)
		{
			FileStream fileStream = null;
			if ((string.IsNullOrEmpty(fileName) ? false : File.Exists(fileName)))
			{
				fileStream = new FileStream(fileName, FileMode.Open);
			}
			return fileStream;
		}
	}
}