using Himall.IServices.QueryModel;
using Himall.Model;
using System;
using System.Linq;

namespace Himall.IServices
{
	public interface IMemberService : IService, IDisposable
	{
		void BatchDeleteMember(long[] ids);

		void BatchLock(long[] ids);

		void BindMember(long userId, string serviceProvider, string openId, string headImage = null, string unionid = null, string unionopenid = null);

		void BindMember(long userId, string serviceProvider, string openId, MemberOpenIdInfo.AppIdTypeEnum AppidType, string headImage = null, string unionid = null);

		void ChangePassWord(long id, string password);

		void CheckContactInfoHasBeenUsed(string serviceProvider, string contact, MemberContactsInfo.UserTypes userType = 0);

		bool CheckMemberExist(string username);

		bool CheckMobileExist(string mobile);

		void DeleteMember(long id);

		void DeleteMemberOpenId(long userid, string openid);

		string GetLogo();

		UserMemberInfo GetMember(long id);

		UserMemberInfo GetMemberByContactInfo(string contact);

		UserMemberInfo GetMemberByName(string userName);

		UserMemberInfo GetMemberByOpenId(string serviceProvider, string openId);

		UserMemberInfo GetMemberByUnionId(string serviceProvider, string UnionId);

		PageModel<UserMemberInfo> GetMembers(MemberQuery query);

		IQueryable<UserMemberInfo> GetMembers(bool? status, string keyWords);

		UserMemberInfo GetUserByCache(long id);

		UserCenterModel GetUserCenterModel(long id);

		void LockMember(long id);

		UserMemberInfo Login(string username, string password);

		UserMemberInfo QuickRegister(string username, string realName, string nickName, string serviceProvider, string openId, string headImage = null, MemberOpenIdInfo.AppIdTypeEnum appidtype = MemberOpenIdInfo.AppIdTypeEnum.Normal, string unionid = null, string unionopenid = null);

		UserMemberInfo Register(string username, string password, string mobile = "", long introducer = 0L);

		UserMemberInfo Register(string username, string password, string serviceProvider, string openId, string headImage = null, long introducer = 0L, string nickname = null, string unionid = null);

		void UnLockMember(long id);

		void UpdateMember(UserMemberInfo memeber);
	}
}