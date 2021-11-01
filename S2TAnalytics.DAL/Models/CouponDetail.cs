using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.DAL.Models
{
    public class CouponDetail : BaseEntity
    {
        public string Code { get; set; }

        public double Amount { get; set; }
        public string Description { get; set; }
    }
}
