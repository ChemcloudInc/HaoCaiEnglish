using Himall.Core.Plugins.Express;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Himall.ExpressPlugin
{
	public abstract class ExpressPluginBase
	{
		private ExpressInfo expressInfo;

		private static Dictionary<string, string> workDirectories;

		public string BackGroundImage
		{
			get
			{
				return expressInfo.BackGroundImage;
			}
			set
			{
                expressInfo.BackGroundImage = value;
			}
		}

		public string DisplayName
		{
			get
			{
				return expressInfo.DisplayName;
			}
		}

		public IEnumerable<ExpressPrintElement> Elements
		{
			get
			{
				return expressInfo.Elements;
			}
		}

		public int Height
		{
			get
			{
				return expressInfo.Height;
			}
		}

		public string Kuaidi100Code
		{
			get
			{
				return expressInfo.Kuaidi100Code;
			}
		}

		public string Logo
		{
			get
			{
				return expressInfo.Logo;
			}
			set
			{
                expressInfo.Logo = value;
			}
		}

		public string Name
		{
			get
			{
				return expressInfo.Name;
			}
		}

		public string TaobaoCode
		{
			get
			{
				return expressInfo.TaobaoCode;
			}
		}

		public int Width
		{
			get
			{
				return expressInfo.Width;
			}
		}

		public string WorkDirectory
		{
			get
			{
				string item;
				string fullName = GetType().FullName;
				if ((string.IsNullOrWhiteSpace(fullName) ? true : !ExpressPluginBase.workDirectories.ContainsKey(fullName)))
				{
					item = null;
				}
				else
				{
					item = ExpressPluginBase.workDirectories[fullName];
				}
				return item;
			}
			set
			{
				string fullName = GetType().FullName;
				if (!ExpressPluginBase.workDirectories.ContainsKey(fullName))
				{
					ExpressPluginBase.workDirectories.Add(fullName, value);
				}
			}
		}

		static ExpressPluginBase()
		{
			ExpressPluginBase.workDirectories = new Dictionary<string, string>();
		}

		public ExpressPluginBase()
		{
			if (WorkDirectory != null)
			{
                RefreshExpressInfo();
			}
		}

		public void CheckCanEnable()
		{
			throw new NotImplementedException();
		}

		public virtual bool CheckExpressCodeIsValid(string expressCode)
		{
			long num;
			return long.TryParse(expressCode, out num);
		}

		public virtual string NextExpressCode(string currentExpressCode)
		{
			long num;
			if (!long.TryParse(currentExpressCode, out num))
			{
				throw new FormatException("快递单号格式不正确,正确的格式为数字");
			}
			return (num + 1).ToString();
		}

		private void RefreshExpressInfo()
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(WorkDirectory);
			FileInfo fileInfo = directoryInfo.GetFiles("config.xml").FirstOrDefault();
			if (fileInfo != null)
			{
				FileStream fileStream = new FileStream(fileInfo.FullName, FileMode.Open);
				try
				{
					XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExpressInfo));
                    expressInfo = (ExpressInfo)xmlSerializer.Deserialize(fileStream);
				}
				finally
				{
					if (fileStream != null)
					{
						((IDisposable)fileStream).Dispose();
					}
				}
			}
		}

		public void UpdatePrintElement(IEnumerable<ExpressPrintElement> printElements)
		{
            expressInfo.Elements = printElements.ToArray();
			FileStream fileStream = new FileStream(string.Concat(WorkDirectory, "/config.xml"), FileMode.Create);
			try
			{
				(new XmlSerializer(typeof(ExpressInfo))).Serialize(fileStream, expressInfo);
			}
			finally
			{
				if (fileStream != null)
				{
					((IDisposable)fileStream).Dispose();
				}
			}
		}
	}
}