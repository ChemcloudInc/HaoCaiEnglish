using Himall.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Himall.IServices
{
	public interface IFloorService : IService, IDisposable
	{
		HomeFloorInfo AddHomeFloorBasicInfo(string name, IEnumerable<long> topLevelCategoryIds);

		void DeleteHomeFloor(long id);

		void EnableHomeFloor(long homeFloorId, bool enable);

		HomeFloorInfo GetHomeFloor(long id);

		IQueryable<HomeFloorInfo> GetHomeFloors();

		void UpdateFloorBasicInfo(long homeFloorId, string name, IEnumerable<long> topLevelCategoryIds);

		void UpdateHomeFloorDetail(HomeFloorInfo homeFloor);

		void UpdateHomeFloorSequence(long sourceSequence, long destiSequence);
	}
}