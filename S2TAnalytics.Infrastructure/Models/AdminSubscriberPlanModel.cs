using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.Infrastructure.Models
{
    public class AdminSubscriberPlanModel
    {
        public int PlanID { get; set; }
        public String Name { get; set; }
        public bool IsActive { get; set; }
        public DateTime ExpireDate { get; set; }
        public int Days { get; set; }

        public int TermLengthId { get; set; }

        public double Price { get; set; }
    }
}
