using Himall.Core;
using Himall.IServices;
using Himall.Model;
using Himall.Web.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Himall.Web.Models
{
	public class ShopModel
	{
		public string Account
		{
			get;
			set;
		}

		public string BankAccountName
		{
			get;
			set;
		}

		public string BankAccountNumber
		{
			get;
			set;
		}

		public string BankCode
		{
			get;
			set;
		}

		public string BankName
		{
			get;
			set;
		}

		public string BankPhoto
		{
			get;
			set;
		}

		public string BankRegionId
		{
			get;
			set;
		}

		public List<CategoryKeyVal> BusinessCategory
		{
			get;
			set;
		}

		public DateTime? BusinessLicenceEnd
		{
			get;
			set;
		}

		public string BusinessLicenceNumber
		{
			get;
			set;
		}

		public string BusinessLicenceNumberPhoto
		{
			get;
			set;
		}

		public string BusinessLicenceRegionId
		{
			get;
			set;
		}

		public DateTime? BusinessLicenceStart
		{
			get;
			set;
		}

		public string BusinessLicenseCert
		{
			get;
			set;
		}

		public string BusinessSphere
		{
			get;
			set;
		}

		[Required(ErrorMessage="必须填写公司详细地址")]
		public string CompanyAddress
		{
			get;
			set;
		}

		[DisplayName("员工总数")]
		[Range(1, 2147483647, ErrorMessage="人数必须为大于0的整数")]
		[Required(ErrorMessage="必须填写员工总数")]
		public Himall.Model.CompanyEmployeeCount CompanyEmployeeCount
		{
			get;
			set;
		}

		public DateTime? CompanyFoundingDate
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

		[Required(ErrorMessage="必须填写公司电话")]
		public string CompanyPhone
		{
			get;
			set;
		}

		public string CompanyRegion
		{
			get;
			set;
		}

		[DataType(DataType.Currency, ErrorMessage="必须为货币值")]
		[Range(typeof(decimal), "0.00", "10000.00", ErrorMessage="输入不大于10000")]
		[Required(ErrorMessage="必须填写注册资金")]
		public decimal CompanyRegisteredCapital
		{
			get;
			set;
		}

		[EmailAddress(ErrorMessage="电子邮箱格式不正确")]
		[Required(ErrorMessage="必须填写电子邮箱")]
		public string ContactsEmail
		{
			get;
			set;
		}

		[Required(ErrorMessage="必须填写联系人姓名")]
		public string ContactsName
		{
			get;
			set;
		}

		[Phone(ErrorMessage="电话号码不正确")]
		[Required(ErrorMessage="必须填写联系人电话")]
		public string ContactsPhone
		{
			get;
			set;
		}

		[Required(ErrorMessage="有效期为必填项")]
		public string EndDate
		{
			get;
			set;
		}

		public string GeneralTaxpayerPhot
		{
			get;
			set;
		}

		public long Id
		{
			get;
			set;
		}

		public bool IsSelf
		{
			get;
			set;
		}

		public string legalPerson
		{
			get;
			set;
		}

		[MaxLength(20, ErrorMessage="店铺名称最多20个字符")]
		[Required(ErrorMessage="店铺名称为必填项")]
		public string Name
		{
			get;
			set;
		}

		public int NewBankRegionId
		{
			get;
			set;
		}

		[Required(ErrorMessage="必须选择公司所在地")]
		public int NewCompanyRegionId
		{
			get;
			set;
		}

		public string OrganizationCode
		{
			get;
			set;
		}

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

		public string PayPhoto
		{
			get;
			set;
		}

		public string PayRemark
		{
			get;
			set;
		}

		public string ProductCert
		{
			get;
			set;
		}

		[Required(ErrorMessage="店铺套餐为必填项")]
		public string ShopGrade
		{
			get;
			set;
		}

		public string Status
		{
			get;
			set;
		}

		public string TaxpayerId
		{
			get;
			set;
		}

		public string TaxRegistrationCertificate
		{
			get;
			set;
		}

		public string TaxRegistrationCertificatePhoto
		{
			get;
			set;
		}

		public ShopModel()
		{
		}

		public ShopModel(ShopInfo m) : this()
		{
            Id = m.Id;
            Account = m.ShopAccount;
            Name = m.ShopName;
			ShopGradeInfo shopGrade = ServiceHelper.Create<IShopService>().GetShopGrade(m.GradeId);
            ShopGrade = (shopGrade == null ? "" : shopGrade.Name);
            Status = m.ShopStatus.ToDescription();
            EndDate = (m.EndDate.HasValue ? m.EndDate.Value.ToString("yyyy-MM-dd") : "");
            IsSelf = m.IsSelf;
            CompanyName = m.CompanyName;
            NewCompanyRegionId = m.CompanyRegionId;
            CompanyRegion = ServiceHelper.Create<IRegionService>().GetRegionFullName(m.CompanyRegionId, " ");
            CompanyAddress = m.CompanyAddress;
            CompanyPhone = m.CompanyPhone;
            CompanyEmployeeCount = m.CompanyEmployeeCount;
            CompanyRegisteredCapital = m.CompanyRegisteredCapital;
            ContactsName = m.ContactsName;
            ContactsPhone = m.ContactsPhone;
            ContactsEmail = m.ContactsEmail;
            BusinessLicenseCert = m.BusinessLicenseCert;
            ProductCert = m.ProductCert;
            OtherCert = m.OtherCert;
            BusinessLicenceNumber = m.BusinessLicenceNumber;
            BusinessLicenceNumberPhoto = m.BusinessLicenceNumberPhoto;
            BusinessLicenceRegionId = ServiceHelper.Create<IRegionService>().GetRegionFullName(m.BusinessLicenceRegionId, " ");
            BusinessLicenceStart = m.BusinessLicenceStart;
            BusinessLicenceEnd = m.BusinessLicenceEnd;
            BusinessSphere = m.BusinessSphere;
            OrganizationCode = m.OrganizationCode;
            OrganizationCodePhoto = m.OrganizationCodePhoto;
            GeneralTaxpayerPhot = m.GeneralTaxpayerPhot;
            BankAccountName = m.BankAccountName;
            BankAccountNumber = m.BankAccountNumber;
            BankName = m.BankName;
            BankCode = m.BankCode;
            BankRegionId = ServiceHelper.Create<IRegionService>().GetRegionFullName(m.BankRegionId, " ");
            NewBankRegionId = m.BankRegionId;
            BankPhoto = m.BankPhoto;
            TaxRegistrationCertificate = m.TaxRegistrationCertificate;
            TaxpayerId = m.TaxpayerId;
            TaxRegistrationCertificatePhoto = m.TaxRegistrationCertificatePhoto;
            PayPhoto = m.PayPhoto;
            PayRemark = m.PayRemark;
            legalPerson = m.legalPerson;
            CompanyFoundingDate = new DateTime?((m.CompanyFoundingDate.HasValue ? m.CompanyFoundingDate.Value : DateTime.Now));
		}

		public static implicit operator ShopInfo(ShopModel m)
		{
			ShopInfo shopInfo = new ShopInfo()
			{
				Id = m.Id,
				ShopName = m.Name,
				GradeId = int.Parse(m.ShopGrade),
				EndDate = new DateTime?(DateTime.Parse(m.EndDate)),
				ShopStatus = (ShopInfo.ShopAuditStatus)int.Parse(m.Status),
				BankRegionId = m.NewBankRegionId,
				CompanyRegionId = m.NewCompanyRegionId
			};
			return shopInfo;
		}
	}
}