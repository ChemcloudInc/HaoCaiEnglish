using Himall.Core;
using Himall.IServices;
using Himall.Model.Models;
using Senparc.Weixin;
using Senparc.Weixin.Entities;
using Senparc.Weixin.MP.AdvancedAPIs.Poi;
using Senparc.Weixin.MP.CommonAPIs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Himall.Service
{
	public class PoiService : ServiceBase, IPoiService, IService, IDisposable
	{
		private string _accessToken = string.Empty;

		public PoiService()
		{
		}

		public bool AddPoi(CreateStoreData createStoreData)
		{
			WxJsonResult wxJsonResult = PoiApi.AddPoi(_accessToken, createStoreData, 10000);
			if (wxJsonResult.errcode != ReturnCode.请求成功)
			{
				throw new Exception(wxJsonResult.errmsg);
			}
			return true;
		}

		public bool DeletePoi(string poiId)
		{
			WxJsonResult wxJsonResult = PoiApi.DeletePoi(_accessToken, poiId, 10000);
			if (wxJsonResult.errcode == (ReturnCode.获取access_token时AppSecret错误或者access_token无效 | ReturnCode.不合法的凭证类型 | ReturnCode.不合法的OpenID | ReturnCode.不合法的按钮个数1 | ReturnCode.不合法的按钮个数2 | ReturnCode.不合法的按钮名字长度 | ReturnCode.不合法的按钮KEY长度 | ReturnCode.access_token超时 | ReturnCode.refresh_token超时 | ReturnCode.oauth_code超时 | ReturnCode.客服帐号个数超过限制 | ReturnCode.无效头像文件类型invalid_file_type) || wxJsonResult.errcode == ReturnCode.系统繁忙此时请开发者稍候再试)
			{
				throw new HimallException("系统繁忙，请稍后尝试！");
			}
			if (wxJsonResult.errcode != ReturnCode.请求成功)
			{
				throw new Exception(wxJsonResult.errmsg);
			}
			return true;
		}

		public new void Dispose()
		{
		}

		public List<WXCategory> GetCategory()
		{
			PoiCategory category = PoiApi.GetCategory(_accessToken, 10000);
			if (category.errcode != ReturnCode.请求成功)
			{
				throw new Exception(category.errmsg);
			}
			List<WXCategory> wXCategories = new List<WXCategory>();
			foreach (string categoryList in category.category_list)
			{
				string[] strArrays = categoryList.Split(new char[] { ',' });
				if (strArrays.Count() == 1)
				{
					WXCategory wXCategory = new WXCategory()
					{
						Id = Guid.NewGuid().ToString(),
						Name = strArrays[0]
					};
					wXCategories.Add(wXCategory);
				}
				else if (!wXCategories.Exists((WXCategory p) => p.Name == strArrays[0]))
				{
					WXCategory wXCategory1 = new WXCategory()
					{
						Id = Guid.NewGuid().ToString(),
						Name = strArrays[0]
					};
					WXCategory wXCategory2 = new WXCategory()
					{
						Id = wXCategory1.Id,
						Name = strArrays[1]
					};
					wXCategory1.Child.Add(wXCategory2);
					wXCategories.Add(wXCategory1);
				}
				else
				{
					WXCategory wXCategory3 = wXCategories.FirstOrDefault((WXCategory p) => p.Name == strArrays[0]);
					if (wXCategory3.Child.Exists((WXCategory p) => p.Name == strArrays[1]))
					{
						continue;
					}
					WXCategory wXCategory4 = new WXCategory()
					{
						Id = wXCategory3.Id,
						Name = strArrays[1]
					};
					wXCategory3.Child.Add(wXCategory4);
				}
			}
			return wXCategories;
		}

		public GetStoreBaseInfo GetPoi(string poiId)
		{
			GetStoreResultJson poi = PoiApi.GetPoi(_accessToken, poiId, 10000);
			if (poi.errcode != ReturnCode.请求成功)
			{
				throw new Exception(poi.errmsg);
			}
			return poi.business.base_info;
		}

		public GetStoreListResultJson GetPoiList(int page, int rows)
		{
			int num = 0;
			if (page > 1)
			{
				num = (page - 1) * rows;
			}
			GetStoreListResultJson poiList = PoiApi.GetPoiList(_accessToken, num, rows, 10000);
			if (poiList.errcode != ReturnCode.请求成功 && poiList.errcode != ReturnCode.api功能未授权)
			{
				throw new Exception(poiList.errmsg);
			}
			return poiList;
		}

		public List<GetStoreList_BaseInfo> GetPoiList()
		{
			GetStoreListResultJson poiList = PoiApi.GetPoiList(_accessToken, 0, 50, 10000);
			if (poiList.errcode != ReturnCode.请求成功)
			{
				throw new Exception(poiList.errmsg);
			}
			return (
				from l in poiList.business_list
				select l.base_info).ToList();
		}

		public void init(string appid, string secret)
		{
            _accessToken = AccessTokenContainer.TryGetToken(appid, secret, false);
		}

		public bool UpdatePoi(UpdateStoreData updateStoreData)
		{
			WxJsonResult wxJsonResult = PoiApi.UpdatePoi(_accessToken, updateStoreData, 10000);
			if (wxJsonResult.errcode != ReturnCode.请求成功)
			{
				throw new Exception(wxJsonResult.errmsg);
			}
			if (wxJsonResult.errcode == (ReturnCode.获取access_token时AppSecret错误或者access_token无效 | ReturnCode.不合法的凭证类型 | ReturnCode.不合法的OpenID | ReturnCode.不合法的按钮个数1 | ReturnCode.不合法的按钮个数2 | ReturnCode.不合法的按钮名字长度 | ReturnCode.不合法的按钮KEY长度 | ReturnCode.access_token超时 | ReturnCode.refresh_token超时 | ReturnCode.oauth_code超时 | ReturnCode.客服帐号个数超过限制 | ReturnCode.无效头像文件类型invalid_file_type))
			{
				throw new HimallException("暂时不允许修改");
			}
			return true;
		}

		public string UploadImage(string filePath)
		{
			UploadImageResultJson uploadImageResultJson = PoiApi.UploadImage(_accessToken, filePath, 10000);
			if (uploadImageResultJson.errcode != ReturnCode.请求成功)
			{
				throw new Exception(uploadImageResultJson.errmsg);
			}
			return uploadImageResultJson.url;
		}
	}
}