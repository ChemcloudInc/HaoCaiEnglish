using Himall.Model;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Himall.Web.Areas.SellerAdmin.Models
{
	public class ShopProfileStep1
	{
		[Required(ErrorMessage="必须填写公司详细地址")]
		public string Address
		{
			get;
			set;
		}

		[Required(ErrorMessage="必须填写营业执照所在地")]
		public int BusinessLicenceArea
		{
			get;
			set;
		}

		[Required(ErrorMessage="必须填写营业执照号")]
		public string BusinessLicenceNumber
		{
			get;
			set;
		}

		[Required(ErrorMessage="必须上传营业执照号电子版")]
		public string BusinessLicenceNumberPhoto
		{
			get;
			set;
		}

		[Required(ErrorMessage="必须填写营业执照截止有效期")]
		public DateTime BusinessLicenceValidEnd
		{
			get;
			set;
		}

		[Required(ErrorMessage="必须填写营业执照起始有效期")]
		public DateTime BusinessLicenceValidStart
		{
			get;
			set;
		}

		public string BusinessLicenseCert
		{
			get;
			set;
		}

		public string BusinessLicenseCert1
		{
			get;
			set;
		}

		[Required(ErrorMessage="必须填写法定经营范围")]
		public string BusinessSphere
		{
			get;
			set;
		}

		[Required(ErrorMessage="必须选择公司所在地")]
		public int CityRegionId
		{
			get;
			set;
		}

		[Required(ErrorMessage="必须填写公司成立日期")]
		public DateTime CompanyFoundingDate
		{
			get;
			set;
		}

		[MinLength(5, ErrorMessage="公司名称不能小于5个字符")]
		[Remote("CheckCompanyName", "ShopProfile", "SellerAdmin", ErrorMessage="该公司名已存在")]
		[Required(ErrorMessage="必须填写公司名称")]
		[StringLength(60, ErrorMessage="最大长度不能超过60")]
		public string CompanyName
		{
			get;
			set;
		}

		[Required(ErrorMessage="必须填写联系人姓名")]
		public string ContactName
		{
			get;
			set;
		}

		[Phone(ErrorMessage="电话号码不正确")]
		[Required(ErrorMessage="必须填写联系人电话")]
		public string ContactPhone
		{
			get;
			set;
		}

		[EmailAddress(ErrorMessage="电子邮箱格式不正确")]
		[Required(ErrorMessage="必须填写电子邮箱")]
		public string Email
		{
			get;
			set;
		}

		[DisplayName("员工总数")]
		[Range(1, 2147483647, ErrorMessage="请选择公司人数")]
		[Required(ErrorMessage="必须填写员工总数")]
		public CompanyEmployeeCount EmployeeCount
		{
			get;
			set;
		}

		[Required(ErrorMessage="必须填写一般纳税人证明")]
		public string GeneralTaxpayerPhoto
		{
			get;
			set;
		}

		[Required(ErrorMessage="必须填写公司法定代表人")]
		public string legalPerson
		{
			get;
			set;
		}

		[Required(ErrorMessage="必须填写组织机构代码")]
		public string OrganizationCode
		{
			get;
			set;
		}

		[Required(ErrorMessage="必须上传组织机构代码证电子版")]
		public string OrganizationCodePhoto
		{
			get;
			set;
		}

		public string OtherCert
		{
			get;
			set;
		}

		public string OtherCert1
		{
			get;
			set;
		}

		[Required(ErrorMessage="必须填写公司电话")]
		public string Phone
		{
			get;
			set;
		}

		public string ProductCert
		{
			get;
			set;
		}

		public string ProductCert1
		{
			get;
			set;
		}

		[DataType(DataType.Currency, ErrorMessage="必须为货币值")]
		[Range(typeof(decimal), "0.00", "10000.00", ErrorMessage="输入不大于10000")]
		[Required(ErrorMessage="必须填写注册资金")]
		public decimal RegisterMoney
		{
			get;
			set;
		}

		public string taxRegistrationCert
		{
			get;
			set;
		}

		public ShopProfileStep1()
		{
		}
	}
}