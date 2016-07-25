using System;
using System.ComponentModel;

namespace Himall.Model
{
	public enum InvoiceType
	{
		[Description("不需要发票")]
		None,
		[Description("增值税发票")]
		VATInvoice,
		[Description("普通发票")]
		OrdinaryInvoices
	}
}