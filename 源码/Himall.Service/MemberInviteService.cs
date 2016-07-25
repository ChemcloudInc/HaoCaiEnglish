using Himall.Core.Helper;
using Himall.Entity;
using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using Himall.ServiceProvider;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Himall.Service
{
	public class MemberInviteService : ServiceBase, IMemberInviteService, IService, IDisposable
	{
		public MemberInviteService()
		{
		}

		public void AddInviteIntegel(UserMemberInfo RegMember, UserMemberInfo InviteMember)
		{
			InviteRuleInfo inviteRule = GetInviteRule();
			if (inviteRule != null)
			{
				int? inviteIntegral = inviteRule.InviteIntegral;
				if ((inviteIntegral.GetValueOrDefault() != 0 ? true : !inviteIntegral.HasValue))
				{
					if (!HasInviteIntegralRecord(RegMember.Id))
					{
						IMemberIntegralConversionFactoryService create = Instance<IMemberIntegralConversionFactoryService>.Create;
						IMemberIntegralService memberIntegralService = Instance<IMemberIntegralService>.Create;
						MemberIntegralRecord memberIntegralRecord = new MemberIntegralRecord()
						{
							UserName = RegMember.UserName,
							MemberId = RegMember.Id,
							RecordDate = new DateTime?(DateTime.Now),
							ReMark = "被邀请注册",
							TypeId = MemberIntegral.IntegralType.Others
						};
						IConversionMemberIntegralBase conversionMemberIntegralBase = create.Create(MemberIntegral.IntegralType.Others, inviteRule.RegIntegral.Value);
						memberIntegralService.AddMemberIntegral(memberIntegralRecord, conversionMemberIntegralBase);
						MemberIntegralRecord memberIntegralRecord1 = new MemberIntegralRecord()
						{
							UserName = InviteMember.UserName,
							MemberId = InviteMember.Id,
							RecordDate = new DateTime?(DateTime.Now),
							ReMark = "邀请会员",
							TypeId = MemberIntegral.IntegralType.InvitationMemberRegiste
						};
						memberIntegralService.AddMemberIntegral(memberIntegralRecord1, create.Create(MemberIntegral.IntegralType.InvitationMemberRegiste, 0));
						InviteRecordInfo inviteRecordInfo = new InviteRecordInfo()
						{
							RegIntegral = new int?(inviteRule.RegIntegral.GetValueOrDefault()),
							InviteIntegral = inviteRule.InviteIntegral.GetValueOrDefault(),
							RecordTime = new DateTime?(DateTime.Now),
							RegTime = new DateTime?(RegMember.CreateDate),
							RegUserId = new long?(RegMember.Id),
							RegName = RegMember.UserName,
							UserId = new long?(InviteMember.Id),
							UserName = InviteMember.UserName
						};
                        AddInviteRecord(inviteRecordInfo);
					}
					return;
				}
			}
		}

		public void AddInviteRecord(InviteRecordInfo info)
		{
            context.InviteRecordInfo.Add(info);
            context.SaveChanges();
		}

		public PageModel<InviteRecordInfo> GetInviteList(InviteRecordQuery query)
		{
			int num = 0;
			PageModel<InviteRecordInfo> pageModel = new PageModel<InviteRecordInfo>();
			IQueryable<InviteRecordInfo> page = context.InviteRecordInfo.AsQueryable<InviteRecordInfo>();
			if (!string.IsNullOrEmpty(query.userName))
			{
				page = 
					from d in page
					where d.UserName.Equals(query.userName)
					select d;
			}
			page = page.GetPage(out num, query.PageNo, query.PageSize, (IQueryable<InviteRecordInfo> d) => 
				from a in d
				orderby a.Id descending
				select a);
			pageModel.Models = page;
			pageModel.Total = num;
			return pageModel;
		}

		public InviteRuleInfo GetInviteRule()
		{
			return context.InviteRuleInfo.FirstOrDefault();
		}

		public UserInviteModel GetMemberInviteInfo(long userId)
		{
			UserInviteModel userInviteModel = new UserInviteModel();
			IQueryable<InviteRecordInfo> inviteRecordInfo = 
				from a in context.InviteRecordInfo
				where a.UserId == userId
                select a;
			int num = inviteRecordInfo.Count();
			int? nullable = inviteRecordInfo.Sum<InviteRecordInfo>((InviteRecordInfo a) => (int?)a.InviteIntegral);
			userInviteModel.InviteIntergralCount = nullable.GetValueOrDefault();
			userInviteModel.InvitePersonCount = num;
			return userInviteModel;
		}

		public bool HasInviteIntegralRecord(long RegId)
		{
			return context.InviteRecordInfo.Any((InviteRecordInfo a) => a.RegUserId == RegId);
		}

		private string MoveImages(string image)
		{
			string mapPath = IOHelper.GetMapPath(image);
			string extension = (new FileInfo(mapPath)).Extension;
			string empty = string.Empty;
			empty = IOHelper.GetMapPath("/Storage/Plat/MemberInvite");
			string str = "/Storage/Plat/MemberInvite/";
			string str1 = string.Concat("Invite_Icon", extension);
			if (!Directory.Exists(empty))
			{
				Directory.CreateDirectory(empty);
			}
			if (!image.Replace("\\", "/").Contains("/temp/"))
			{
				return image;
			}
			IOHelper.CopyFile(mapPath, empty, false, str1);
			return string.Concat(str, str1);
		}

		public void SetInviteRule(InviteRuleInfo model)
		{
			InviteRuleInfo inviteIntegral = context.InviteRuleInfo.FirstOrDefault((InviteRuleInfo a) => a.Id == model.Id);
			if (inviteIntegral != null)
			{
				inviteIntegral.InviteIntegral = model.InviteIntegral;
				inviteIntegral.RegIntegral = model.RegIntegral;
				inviteIntegral.ShareDesc = model.ShareDesc;
				inviteIntegral.ShareIcon = model.ShareIcon;
				inviteIntegral.ShareRule = model.ShareRule;
				inviteIntegral.ShareTitle = model.ShareTitle;
			}
			else
			{
                context.InviteRuleInfo.Add(model);
			}
            context.SaveChanges();
			inviteIntegral.ShareIcon = MoveImages(model.ShareIcon);
            context.SaveChanges();
		}
	}
}