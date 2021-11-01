using S2TAnalytics.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.Infrastructure.Models
{
    public class SubscriptionPlanModel
    {
        public int PlanId { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }

        public double InfrastructureCost { get; set; }
        public List<string> Widgets { get; set; }

        public List<PlanTermLength> PlanTermLength { get; set; }
    }
}
