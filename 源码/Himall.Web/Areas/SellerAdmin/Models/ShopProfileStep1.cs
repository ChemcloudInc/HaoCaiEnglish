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

        [MinLength(5, ErrorMessage = "Company name can not be less than 5 characters")]
        [Remote("CheckCompanyName", "ShopProfile", "SellerAdmin", ErrorMessage = "The company name already exists")]
        [Required(ErrorMessage = "Company name is required")]
        [StringLength(60, ErrorMessage = "The maximum length can not exceed 60")]
		public string CompanyName
		{
			get;
			set;
		}

		[Required(ErrorMessage="Contact name is required")]
		public string ContactName
		{
			get;
			set;
		}

        [Phone(ErrorMessage = "Phone number is incorrect")]
		[Required(ErrorMessage="Phone number is required")]
		public string ContactPhone
		{
			get;
			set;
		}

        [EmailAddress(ErrorMessage = "E-mail format is incorrect")]
		[Required(ErrorMessage="E-mail is required")]
		public string Email
		{
			get;
			set;
		}

        [DisplayName("Number of employees")]
        [Range(1, 2147483647, ErrorMessage = "Please select number of employees")]
        [Required(ErrorMessage = "Number of employees is required")]
		public CompanyEmployeeCount EmployeeCount
		{
			get;
			set;
		}

        [Required(ErrorMessage = "General tax payer photo is required")]
		public string GeneralTaxpayerPhoto
		{
			get;
			set;
		}

        [Required(ErrorMessage = "Legal person is required")]
		public string legalPerson
		{
			get;
			set;
		}

		[Required(ErrorMessage="Organization code is required")]
		public string OrganizationCode
		{
			get;
			set;
		}

		[Required(ErrorMessage="Organization code photo is required")]
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

		[Required(ErrorMessage="Phone is required")]
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

        [DataType(DataType.Currency, ErrorMessage = "Must be a monetary value")]
        [Range(typeof(decimal), "0.00", "10000.00", ErrorMessage = "Input is not more than 10,000")]
		[Required(ErrorMessage="Register money is required")]
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