using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class InviteRuleModel
	{
		public long ID
		{
			get;
			set;
		}

		[Range(0, 2147483647, ErrorMessage="积分必须大于等于0")]
		[RegularExpression("\\d+", ErrorMessage="积分必须是数字")]
		[Required(ErrorMessage="邀请积分不能为空")]
		public int? InviteIntegral
		{
			get;
			set;
		}

		[Range(0, 2147483647, ErrorMessage="积分必须大于等于0")]
		[RegularExpression("\\d+", ErrorMessage="积分必须是数字")]
		public int? RegIntegral
		{
			get;
			set;
		}

		[MaxLength(70, ErrorMessage="最多70字")]
		[Required(ErrorMessage="分享描述不能为空")]
		public string ShareDesc
		{
			get;
			set;
		}

		public string ShareIcon
		{
			get;
			set;
		}

		[MaxLength(70, ErrorMessage="最多70字")]
		public string ShareRule
		{
			get;
			set;
		}

		[Required(ErrorMessage="分享标题不能为空")]
		public string ShareTitle
		{
			get;
			set;
		}

		public InviteRuleModel()
		{
		}
	}
}