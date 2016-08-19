using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Himall.Model.Models
{
  public  class SpecificationsInfo : BaseModel
    {
     private long _id;

		public virtual ICollection<Himall.Model.SpecificationValueInfo> SpecificationValueInfo
		{
			get;
			set;
		}

		[NotMapped]
		public string AttrValue
		{
			get;
			set;
		}

		public long DisplaySequence
		{
			get;
			set;
		}

		public new long Id
		{
			get
			{
				return _id;
			}
			set
			{
                _id = value;
				base.Id = value;
			}
		}

		public bool IsMulti
		{
			get;
			set;
		}

		public bool IsMust
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}


		public long TypeId
		{
			get;
			set;
		}

		public virtual ProductTypeInfo TypesInfo
		{
			get;
			set;
		}

        public SpecificationsInfo()
		{
            SpecificationValueInfo = new HashSet<Himall.Model.SpecificationValueInfo>();
             
		}
	}
}