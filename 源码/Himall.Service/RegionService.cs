using Himall.Core;
using Himall.Core.Helper;
using Himall.IServices;
using Himall.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Himall.Service
{
	public class RegionService : ServiceBase, IRegionService, IService, IDisposable
	{
		public RegionService()
		{
		}

		public int GetCityId(string regionIdPath)
		{
			int num = 0;
			if (!string.IsNullOrEmpty(regionIdPath))
			{
				string[] strArrays = new string[] { "," };
				string[] strArrays1 = regionIdPath.Split(strArrays, StringSplitOptions.RemoveEmptyEntries);
				if (strArrays1.Length > 1)
				{
					num = int.Parse(strArrays1[1]);
				}
			}
			return num;
		}

		public long GetCityIdByName(string CityName, long RegionId, bool NullGetFirst = false)
		{
			long id = 0;
			IEnumerable<ProvinceMode> regions = GetRegions();
			ProvinceMode provinceMode = regions.FirstOrDefault((ProvinceMode d) => d.Id == RegionId);
			if (provinceMode != null)
			{
				CityMode cityMode = provinceMode.City.FirstOrDefault((CityMode d) => d.Name.Contains(CityName));
				if (cityMode == null && NullGetFirst)
				{
					cityMode = provinceMode.City.FirstOrDefault();
				}
				if (cityMode != null)
				{
					id = cityMode.Id;
				}
			}
			return id;
		}

		public IEnumerable<KeyValuePair<long, string>> GetRegion(long parentId)
		{
			IEnumerable<KeyValuePair<long, string>> county;
			IEnumerable<ProvinceMode> regions = GetRegions();
			if (parentId <= 0)
			{
				return 
					from item in regions
					select new KeyValuePair<long, string>(item.Id, item.Name);
			}
			ProvinceMode provinceMode = (
				from item in regions
				where item.Id == parentId
				select item).FirstOrDefault();
			if (provinceMode != null)
			{
				return 
					from item in provinceMode.City
					select new KeyValuePair<long, string>(item.Id, item.Name);
			}
			List<ProvinceMode>.Enumerator enumerator = regions.ToList().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ProvinceMode current = enumerator.Current;
					CityMode cityMode = (
						from item in current.City
						where item.Id == parentId
						select item).FirstOrDefault();
					if (cityMode == null || cityMode.County == null || cityMode.County.Count() <= 0)
					{
						continue;
					}
					county = 
						from item in cityMode.County
						select new KeyValuePair<long, string>(item.Id, item.Name);
					return county;
				}
				return null;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return county;
		}

		public long GetRegionByIPInTaobao(string ip)
		{
			string str = "http://ip.taobao.com/service/getIpInfo.php?ip={0}";
			long num = 0;
			str = string.Format(str, ip);
			try
			{
				TaobaoIpDataModel taobaoIpDataModel = JsonConvert.DeserializeObject<TaobaoIpDataModel>(WebHelper.GetRequestData(str, ""));
				if (taobaoIpDataModel != null && taobaoIpDataModel.code == 0 && !string.IsNullOrWhiteSpace(taobaoIpDataModel.data.region))
				{
					long regionIdByName = GetRegionIdByName(taobaoIpDataModel.data.region);
					if (regionIdByName > 0)
					{
						long cityIdByName = 0;
						if (!string.IsNullOrWhiteSpace(taobaoIpDataModel.data.city))
						{
							cityIdByName = GetCityIdByName(taobaoIpDataModel.data.city, regionIdByName, true);
						}
						num = cityIdByName;
					}
				}
			}
			catch (Exception exception)
			{
			}
			return num;
		}

		public string GetRegionFullName(long regionId, string seperator = " ")
		{
			string name;
			List<ProvinceMode>.Enumerator enumerator = GetRegions().ToList().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ProvinceMode current = enumerator.Current;
					if (current.Id != regionId)
					{
						List<CityMode>.Enumerator enumerator1 = current.City.ToList().GetEnumerator();
						try
						{
							while (enumerator1.MoveNext())
							{
								CityMode cityMode = enumerator1.Current;
								if (cityMode.Id != regionId)
								{
									if (cityMode.County == null)
									{
										continue;
									}
									List<CountyMode>.Enumerator enumerator2 = cityMode.County.ToList().GetEnumerator();
									try
									{
										while (enumerator2.MoveNext())
										{
											CountyMode countyMode = enumerator2.Current;
											if (countyMode.Id != regionId)
											{
												continue;
											}
											string[] strArrays = new string[] { current.Name, seperator, cityMode.Name, seperator, countyMode.Name };
											name = string.Concat(strArrays);
											return name;
										}
									}
									finally
									{
										((IDisposable)enumerator2).Dispose();
									}
								}
								else
								{
									name = string.Concat(current.Name, seperator, cityMode.Name);
									return name;
								}
							}
						}
						finally
						{
							((IDisposable)enumerator1).Dispose();
						}
					}
					else
					{
						name = current.Name;
						return name;
					}
				}
				return string.Empty;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return name;
		}

		public long GetRegionIdByName(string RegionName)
		{
			long id = 0;
			string shortAddressName = GetShortAddressName(RegionName);
			IEnumerable<ProvinceMode> regions = GetRegions();
			ProvinceMode provinceMode = regions.FirstOrDefault((ProvinceMode d) => d.Name.Contains(shortAddressName));
			if (provinceMode != null)
			{
				id = provinceMode.Id;
			}
			return id;
		}

		public string GetRegionIdPath(long regionId)
		{
			string str;
			List<ProvinceMode>.Enumerator enumerator = GetRegions().ToList().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ProvinceMode current = enumerator.Current;
					if (current.Id != regionId)
					{
						List<CityMode>.Enumerator enumerator1 = current.City.ToList().GetEnumerator();
						try
						{
							while (enumerator1.MoveNext())
							{
								CityMode cityMode = enumerator1.Current;
								if (cityMode.Id != regionId)
								{
									if (cityMode.County == null)
									{
										continue;
									}
									List<CountyMode>.Enumerator enumerator2 = cityMode.County.ToList().GetEnumerator();
									try
									{
										while (enumerator2.MoveNext())
										{
											CountyMode countyMode = enumerator2.Current;
											if (countyMode.Id != regionId)
											{
												continue;
											}
											string[] strArrays = new string[] { current.Id.ToString(), ",", cityMode.Id.ToString(), ",", countyMode.Id.ToString() };
											str = string.Concat(strArrays);
											return str;
										}
									}
									finally
									{
										((IDisposable)enumerator2).Dispose();
									}
								}
								else
								{
									string str1 = current.Id.ToString();
									long id = cityMode.Id;
									str = string.Concat(str1, ",", id.ToString());
									return str;
								}
							}
						}
						finally
						{
							((IDisposable)enumerator1).Dispose();
						}
					}
					else
					{
						str = current.Id.ToString();
						return str;
					}
				}
				return string.Empty;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return str;
		}

		public string GetRegionName(string regionIds, string seperator)
		{
			string[] strArrays = new string[] { seperator };
			string[] strArrays1 = regionIds.Split(strArrays, StringSplitOptions.RemoveEmptyEntries);
			string empty = string.Empty;
			for (int i = 0; i < strArrays1.Length; i++)
			{
				long num = long.Parse(strArrays1[i]);
				empty = string.Concat(empty, GetRegionName(num), seperator);
			}
			if (empty.EndsWith(seperator))
			{
				empty = empty.Substring(0, empty.Length - seperator.Length);
			}
			return empty;
		}

		private string GetRegionName(long regionId)
		{
			string name;
			List<ProvinceMode>.Enumerator enumerator = GetRegions().ToList().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ProvinceMode current = enumerator.Current;
					if (current.Id != regionId)
					{
						List<CityMode>.Enumerator enumerator1 = current.City.ToList().GetEnumerator();
						try
						{
							while (enumerator1.MoveNext())
							{
								CityMode cityMode = enumerator1.Current;
								if (cityMode.Id != regionId)
								{
									if (cityMode.County == null)
									{
										continue;
									}
									List<CountyMode>.Enumerator enumerator2 = cityMode.County.ToList().GetEnumerator();
									try
									{
										while (enumerator2.MoveNext())
										{
											CountyMode countyMode = enumerator2.Current;
											if (countyMode.Id != regionId)
											{
												continue;
											}
											name = countyMode.Name;
											return name;
										}
									}
									finally
									{
										((IDisposable)enumerator2).Dispose();
									}
								}
								else
								{
									name = cityMode.Name;
									return name;
								}
							}
						}
						finally
						{
							((IDisposable)enumerator1).Dispose();
						}
					}
					else
					{
						name = current.Name;
						return name;
					}
				}
				return string.Empty;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return name;
		}

		private IEnumerable<ProvinceMode> GetRegions()
		{
			IEnumerable<ProvinceMode> provinceModes;
			if (Cache.Get("Cache-Regions") == null)
			{
				string empty = string.Empty;
				using (FileStream fileStream = new FileStream(IOHelper.GetMapPath("/Scripts/Region.js"), FileMode.Open))
				{
					using (StreamReader streamReader = new StreamReader(fileStream))
					{
						empty = streamReader.ReadToEnd();
					}
				}
				empty = empty.Replace("var province=", "");
				provinceModes = JsonConvert.DeserializeObject<IEnumerable<ProvinceMode>>(empty);
				Cache.Insert("Cache-Regions", provinceModes);
			}
			else
			{
				provinceModes = (IEnumerable<ProvinceMode>)Cache.Get("Cache-Regions");
			}
			return provinceModes;
		}

		public string GetRegionShortName(long regionId)
		{
			string empty = string.Empty;
			foreach (ProvinceMode list in GetRegions().ToList())
			{
				if (list.Id == regionId)
				{
					empty = list.Name;
				}
				foreach (CityMode cityMode in list.City.ToList())
				{
					if (cityMode.Id == regionId)
					{
						empty = string.Concat(list.Name, " ", cityMode.Name);
					}
					if (cityMode.County == null)
					{
						continue;
					}
					foreach (CountyMode countyMode in cityMode.County.ToList())
					{
						if (countyMode.Id != regionId)
						{
							continue;
						}
						string[] name = new string[] { list.Name, " ", cityMode.Name, " ", countyMode.Name };
						empty = string.Concat(name);
					}
				}
			}
			return GetShortAddress(empty);
		}

		public string GetShortAddress(string regionFullName)
		{
			string empty = string.Empty;
			string[] strArrays = regionFullName.Split(new char[] { ' ' });
			if (strArrays[0] == "北京" || strArrays[0] == "上海" || strArrays[0] == "天津" || strArrays[0] == "重庆")
			{
				empty = strArrays[0];
			}
			else if (!strArrays[0].Contains("特别行政区"))
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(strArrays[0]);
				stringBuilder = stringBuilder.Replace("省", "");
				stringBuilder = stringBuilder.Replace("维吾尔", "");
				stringBuilder = stringBuilder.Replace("回族", "");
				stringBuilder = stringBuilder.Replace("壮族", "");
				stringBuilder = stringBuilder.Replace("自治区", "");
				StringBuilder stringBuilder1 = new StringBuilder();
                if (strArrays.Length != 1)
                {
                    stringBuilder1.Append(strArrays[1]);
                    stringBuilder1 = stringBuilder1.Replace("市", "");
                    stringBuilder1 = stringBuilder1.Replace("盟", "");
                    stringBuilder1 = stringBuilder1.Replace("林区", "");
                    stringBuilder1 = stringBuilder1.Replace("地区", "");
                    stringBuilder1 = stringBuilder1.Replace("土家族", "");
                    stringBuilder1 = stringBuilder1.Replace("苗族", "");
                    stringBuilder1 = stringBuilder1.Replace("回族", "");
                    stringBuilder1 = stringBuilder1.Replace("黎族", "");
                    stringBuilder1 = stringBuilder1.Replace("藏族", "");
                    stringBuilder1 = stringBuilder1.Replace("傣族", "");
                    stringBuilder1 = stringBuilder1.Replace("彝族", "");
                    stringBuilder1 = stringBuilder1.Replace("哈尼族", "");
                    stringBuilder1 = stringBuilder1.Replace("壮族", "");
                    stringBuilder1 = stringBuilder1.Replace("白族", "");
                    stringBuilder1 = stringBuilder1.Replace("景颇族", "");
                    stringBuilder1 = stringBuilder1.Replace("傈僳族", "");
                    stringBuilder1 = stringBuilder1.Replace("朝鲜族", "");
                    stringBuilder1 = stringBuilder1.Replace("蒙古", "");
                    stringBuilder1 = stringBuilder1.Replace("哈萨克", "");
                    stringBuilder1 = stringBuilder1.Replace("柯尔克孜", "");
                    stringBuilder1 = stringBuilder1.Replace("自治州", "");
                    stringBuilder1 = stringBuilder1.Replace("自治县", "");
                    stringBuilder1 = stringBuilder1.Replace("县", "");
                    empty = string.Concat(stringBuilder.ToString(), stringBuilder1.ToString());
                }
                else
                    empty = stringBuilder.ToString();
			}
			else
			{
				empty = strArrays[0].Replace("特别行政区", "");
			}
			return empty;
		}

		private string GetShortAddressName(string str)
		{
			string str1 = str.Replace("特别行政区", "");
			str1 = str1.Replace("省", "");
			str1 = str1.Replace("维吾尔", "");
			str1 = str1.Replace("回族", "");
			str1 = str1.Replace("壮族", "");
			str1 = str1.Replace("自治区", "");
			str1 = str1.Replace("市", "");
			str1 = str1.Replace("盟", "");
			str1 = str1.Replace("林区", "");
			str1 = str1.Replace("地区", "");
			str1 = str1.Replace("土家族", "");
			str1 = str1.Replace("苗族", "");
			str1 = str1.Replace("回族", "");
			str1 = str1.Replace("黎族", "");
			str1 = str1.Replace("藏族", "");
			str1 = str1.Replace("傣族", "");
			str1 = str1.Replace("彝族", "");
			str1 = str1.Replace("哈尼族", "");
			str1 = str1.Replace("壮族", "");
			str1 = str1.Replace("白族", "");
			str1 = str1.Replace("景颇族", "");
			str1 = str1.Replace("傈僳族", "");
			str1 = str1.Replace("朝鲜族", "");
			str1 = str1.Replace("蒙古", "");
			str1 = str1.Replace("哈萨克", "");
			str1 = str1.Replace("柯尔克孜", "");
			str1 = str1.Replace("自治州", "");
			return str1.Replace("自治县", "").Replace("县", "");
		}
	}
}