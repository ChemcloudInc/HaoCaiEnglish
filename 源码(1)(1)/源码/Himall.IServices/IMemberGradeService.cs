using Himall.Model;
using System;
using System.Collections.Generic;

namespace Himall.IServices
{
	public interface IMemberGradeService : IService, IDisposable
	{
		void AddMemberGrade(MemberGrade model);

		void DeleteMemberGrade(long id);

		MemberGrade GetMemberGrade(long id);

		IEnumerable<MemberGrade> GetMemberGradeList();

		void UpdateMemberGrade(MemberGrade model);
	}
}