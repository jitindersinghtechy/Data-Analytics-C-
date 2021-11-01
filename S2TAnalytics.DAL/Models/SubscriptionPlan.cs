using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.DAL.Models
{
    public class SubscriptionPlan : BaseEntity
    {
        public SubscriptionPlan()
        {
            this.WidgetsAccess = new List<WidgetAccess>();
            this.PlanTermLength = new List<PlanTermLength>();
        }
        public int PlanID { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }

        public double InfrastructureCost { get; set; }
        public int Days { get; set; }
        public List<WidgetAccess> WidgetsAccess { get; set; }
        public List<PlanTermLength> PlanTermLength { get; set; }
    }
}
