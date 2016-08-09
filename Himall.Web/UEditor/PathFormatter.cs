using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Himall.Web.App_Code.UEditor
{
	public static class PathFormatter
	{
		public static string Format(string originFileName, string pathFormat)
		{
			if (string.IsNullOrWhiteSpace(pathFormat))
			{
				pathFormat = "{filename}{rand:6}";
			}
			originFileName = (new Regex("[\\\\\\/\\:\\*\\?\\042\\<\\>\\|]")).Replace(originFileName, "");
			string extension = Path.GetExtension(originFileName);
			pathFormat = pathFormat.Replace("{filename}", Path.GetFileNameWithoutExtension(originFileName));
			pathFormat = (new Regex("\\{rand(\\:?)(\\d+)\\}", RegexOptions.Compiled)).Replace(pathFormat, (Match match) => {
				int num = 6;
				if (match.Groups.Count > 2)
				{
					num = Convert.ToInt32(match.Groups[2].Value);
				}
				return (new Random()).Next((int)Math.Pow(10, num), (int)Math.Pow(10, num + 1)).ToString();
			});
			long ticks = DateTime.Now.Ticks;
			pathFormat = pathFormat.Replace("{time}", ticks.ToString());
			int year = DateTime.Now.Year;
			pathFormat = pathFormat.Replace("{yyyy}", year.ToString());
			int year1 = DateTime.Now.Year % 100;
			pathFormat = pathFormat.Replace("{yy}", year1.ToString("D2"));
			int month = DateTime.Now.Month;
			pathFormat = pathFormat.Replace("{mm}", month.ToString("D2"));
			int day = DateTime.Now.Day;
			pathFormat = pathFormat.Replace("{dd}", day.ToString("D2"));
			int hour = DateTime.Now.Hour;
			pathFormat = pathFormat.Replace("{hh}", hour.ToString("D2"));
			int minute = DateTime.Now.Minute;
			pathFormat = pathFormat.Replace("{ii}", minute.ToString("D2"));
			int second = DateTime.Now.Second;
			pathFormat = pathFormat.Replace("{ss}", second.ToString("D2"));
			return string.Concat(pathFormat, extension);
		}
	}
}