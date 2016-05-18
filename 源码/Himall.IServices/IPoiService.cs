using Himall.Model.Models;
using Senparc.Weixin.MP.AdvancedAPIs.Poi;
using System;
using System.Collections.Generic;

namespace Himall.IServices
{
	public interface IPoiService : IService, IDisposable
	{
		bool AddPoi(CreateStoreData createStoreData);

		bool DeletePoi(string poiId);

		List<WXCategory> GetCategory();

		GetStoreBaseInfo GetPoi(string poiId);

		GetStoreListResultJson GetPoiList(int page, int rows);

		List<GetStoreList_BaseInfo> GetPoiList();

		void init(string appid, string secret);

		bool UpdatePoi(UpdateStoreData updateStoreData);

		string UploadImage(string filePath);
	}
}