using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.Infrastructure.Models
{
    public class UserPlanModel
    {
        public int PlanID { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int Days { get; set; }
        public int TermLengthId { get; set; }

    }
}
