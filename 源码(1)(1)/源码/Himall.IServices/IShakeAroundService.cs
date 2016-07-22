using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs.ShakeAround;
using System;
using System.Collections.Generic;

namespace Himall.IServices
{
	public interface IShakeAroundService : IService, IDisposable
	{
		bool AddDevice(int quantity, string applyReason, string comment = null, long? poiId = null);

		bool AddPage(string title, string description, string pageUrl, string iconUrl, string comment = null);

		bool DeletePage(List<long> ids);

		bool DeviceBindLocatoin(long deviceId, string uuId, long major, long minor, long poiId);

		List<DeviceModel> GetDeviceAll();

		DeviceList2ResultJson GetDeviceAll(int page, int rows);

		DeviceModel GetDeviceById(long id);

		SearchPages_Data GetPageAll();

		SearchPages_Data GetPageAll(int page, int rows);

		List<SearchPages_Data_Page> GetPageById(long[] ids);

		List<long> GetPageids(DeviceModel model);

		void init(string appid, string secret);

		bool SetRelationship(DeviceApply_Data_Device_Identifiers deviceIdentifier, long[] pageIds, ShakeAroundBindType type);

		DeviceListResultJson UnauthorizedTest();

		bool UpdateDevice(long deviceId, string uuId, long major, long minor, string comment, long poiId = 0L);

		bool UpdatePage(long pageId, string title, string description, string pageUrl, string iconUrl, string comment = null);

		string UploadImage(string file);
	}
}