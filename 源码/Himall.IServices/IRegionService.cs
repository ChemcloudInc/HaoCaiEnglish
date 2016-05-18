using System;
using System.Collections.Generic;

namespace Himall.IServices
{
	public interface IRegionService : IService, IDisposable
	{
		int GetCityId(string regionIdPath);

		long GetCityIdByName(string CityName, long RegionId, bool NullGetFirst = false);

		IEnumerable<KeyValuePair<long, string>> GetRegion(long parentId);

		long GetRegionByIPInTaobao(string ip);

		string GetRegionFullName(long regionId, string seperator = " ");

		long GetRegionIdByName(string RegionName);

		string GetRegionIdPath(long regionId);

		string GetRegionName(string regionIds, string seperator = ",");

		string GetRegionShortName(long regionId);
	}
}