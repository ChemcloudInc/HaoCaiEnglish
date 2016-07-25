using Himall.Core;
using Himall.IServices;
using Senparc.Weixin;
using Senparc.Weixin.Entities;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs.ShakeAround;
using Senparc.Weixin.MP.CommonAPIs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Himall.Service
{
	public class ShakeAroundService : ServiceBase, IShakeAroundService, IService, IDisposable
	{
		private string _accessToken = string.Empty;

		public ShakeAroundService()
		{
		}

		public bool AddDevice(int quantity, string applyReason, string comment = null, long? poiId = null)
		{
			DeviceApplyResultJson deviceApplyResultJson = ShakeAroundApi.DeviceApply(_accessToken, quantity, applyReason, comment, poiId, 10000);
			if (deviceApplyResultJson.errcode != ReturnCode.请求成功)
			{
				throw new Exception(deviceApplyResultJson.errmsg);
			}
			return true;
		}

		public bool AddPage(string title, string description, string pageUrl, string iconUrl, string comment = null)
		{
			AddPageResultJson addPageResultJson = ShakeAroundApi.AddPage(_accessToken, title, description, pageUrl, iconUrl, comment, 10000);
			if (addPageResultJson.errcode != ReturnCode.请求成功)
			{
				throw new Exception(addPageResultJson.errmsg);
			}
			return true;
		}

		public bool DeletePage(List<long> ids)
		{
			WxJsonResult wxJsonResult = ShakeAroundApi.DeletePage(_accessToken, ids.ToArray(), 10000);
			if (wxJsonResult.errcode != ReturnCode.请求成功)
			{
				if (!wxJsonResult.errmsg.Equals("the page has been binded to device ID"))
				{
					throw new Exception(wxJsonResult.errmsg);
				}
				throw new HimallException("页面已配置到设备");
			}
			return true;
		}

		public bool DeviceBindLocatoin(long deviceId, string uuId, long major, long minor, long poiId)
		{
			WxJsonResult wxJsonResult = ShakeAroundApi.DeviceBindLocatoin(_accessToken, deviceId, uuId, major, minor, poiId, 10000);
			if (wxJsonResult.errcode != ReturnCode.请求成功)
			{
				throw new Exception(wxJsonResult.errmsg);
			}
			return true;
		}

		public List<DeviceModel> GetDeviceAll()
		{
			DeviceListResultJson deviceListResultJson = ShakeAroundApi.SearchDeviceByRange(_accessToken, 0, 49, 10000);
			if (deviceListResultJson.errcode != ReturnCode.请求成功)
			{
				throw new Exception(deviceListResultJson.errmsg);
			}
			return deviceListResultJson.data.devices;
		}

		public DeviceList2ResultJson GetDeviceAll(int page, int rows)
		{
			int num = 0;
			if (page > 1)
			{
				num = (page - 1) * rows;
			}
			DeviceListResultJson deviceListResultJson = ShakeAroundApi.SearchDeviceByRange(_accessToken, num, rows, 10000);
			if (deviceListResultJson.errcode != ReturnCode.请求成功)
			{
				throw new Exception(deviceListResultJson.errmsg);
			}
			return deviceListResultJson.data;
		}

		public DeviceModel GetDeviceById(long id)
		{
			DeviceApply_Data_Device_Identifiers deviceApplyDataDeviceIdentifier = new DeviceApply_Data_Device_Identifiers()
			{
				device_id = id
			};
			string str = _accessToken;
			List<DeviceApply_Data_Device_Identifiers> deviceApplyDataDeviceIdentifiers = new List<DeviceApply_Data_Device_Identifiers>()
			{
				deviceApplyDataDeviceIdentifier
			};
			DeviceListResultJson deviceListResultJson = ShakeAroundApi.SearchDeviceById(str, deviceApplyDataDeviceIdentifiers, 10000);
			if (deviceListResultJson.errcode != ReturnCode.请求成功)
			{
				throw new Exception(deviceListResultJson.errmsg);
			}
			if (deviceListResultJson.data.devices.Count <= 0)
			{
				return null;
			}
			return deviceListResultJson.data.devices[0];
		}

		public SearchPages_Data GetPageAll()
		{
			SearchPagesResultJson searchPagesResultJson = ShakeAroundApi.SearchPagesByRange(_accessToken, 0, 49, 10000);
			if (searchPagesResultJson.errcode != ReturnCode.请求成功)
			{
				throw new Exception(searchPagesResultJson.errmsg);
			}
			return searchPagesResultJson.data;
		}

		public SearchPages_Data GetPageAll(int page, int rows)
		{
			int num = 0;
			if (page > 1)
			{
				num = (page - 1) * rows;
			}
			SearchPagesResultJson searchPagesResultJson = ShakeAroundApi.SearchPagesByRange(_accessToken, num, rows, 10000);
			if (searchPagesResultJson.errcode != ReturnCode.请求成功)
			{
				throw new Exception(searchPagesResultJson.errmsg);
			}
			return searchPagesResultJson.data;
		}

		public List<SearchPages_Data_Page> GetPageById(long[] ids)
		{
			SearchPagesResultJson searchPagesResultJson = ShakeAroundApi.SearchPagesByPageId(_accessToken, ids, 10000);
			if (searchPagesResultJson.errcode != ReturnCode.请求成功)
			{
				throw new Exception(searchPagesResultJson.errmsg);
			}
			return searchPagesResultJson.data.pages;
		}

		public List<long> GetPageids(DeviceModel model)
		{
			DeviceApply_Data_Device_Identifiers deviceApplyDataDeviceIdentifier = new DeviceApply_Data_Device_Identifiers()
			{
				device_id = long.Parse(model.device_id),
				major = long.Parse(model.major),
				minor = long.Parse(model.minor),
				uuid = model.uuid
			};
			RelationResult relationResult = ShakeAroundApi.SearchPagesByDeviceId(_accessToken, 1, deviceApplyDataDeviceIdentifier, 10000);
			return (
				from p in relationResult.data.relations
				select p.page_id).ToList();
		}

		public void init(string appid, string secret)
		{
            _accessToken = AccessTokenContainer.TryGetToken(appid, secret, false);
		}

		public bool SetRelationship(DeviceApply_Data_Device_Identifiers deviceIdentifier, long[] pageIds, ShakeAroundBindType type)
		{
			WxJsonResult wxJsonResult = ShakeAroundApi.BindPage(_accessToken, deviceIdentifier, pageIds, type, ShakeAroundAppendType.新增, 10000);
			if (wxJsonResult.errcode != ReturnCode.请求成功)
			{
				throw new Exception(wxJsonResult.errmsg);
			}
			return true;
		}

		public DeviceListResultJson UnauthorizedTest()
		{
			DeviceListResultJson deviceListResultJson = ShakeAroundApi.SearchDeviceByRange(_accessToken, 0, 1, 10000);
			if (deviceListResultJson.errcode != ReturnCode.请求成功 && deviceListResultJson.errcode != ReturnCode.api功能未授权)
			{
				throw new Exception(deviceListResultJson.errmsg);
			}
			return deviceListResultJson;
		}

		public bool UpdateDevice(long deviceId, string uuId, long major, long minor, string comment, long poiId = 0L)
		{
			WxJsonResult wxJsonResult = ShakeAroundApi.DeviceUpdate(_accessToken, deviceId, uuId, major, minor, comment, poiId, 10000);
			if (wxJsonResult.errcode != ReturnCode.请求成功)
			{
				throw new Exception(wxJsonResult.errmsg);
			}
			return true;
		}

		public bool UpdatePage(long pageId, string title, string description, string pageUrl, string iconUrl, string comment = null)
		{
			UpdatePageResultJson updatePageResultJson = ShakeAroundApi.UpdatePage(_accessToken, pageId, title, description, pageUrl, iconUrl, comment, 10000);
			if (updatePageResultJson.errcode != ReturnCode.请求成功)
			{
				throw new Exception(updatePageResultJson.errmsg);
			}
			return true;
		}

		public string UploadImage(string file)
		{
			UploadImageResultJson uploadImageResultJson = ShakeAroundApi.UploadImage(_accessToken, file, 10000);
			if (uploadImageResultJson.errcode != ReturnCode.请求成功)
			{
				throw new Exception(uploadImageResultJson.errmsg);
			}
			return uploadImageResultJson.data.pic_url;
		}
	}
}