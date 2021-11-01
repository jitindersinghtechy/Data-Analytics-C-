using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.Infrastructure.Models
{
    public class RequestPlanModel
    {
        public int PlanId { get; set; }
        public int TermLengthId { get; set; }
        public string Promocode { get; set; }
    }
}
