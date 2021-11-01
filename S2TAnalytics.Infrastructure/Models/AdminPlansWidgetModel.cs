using S2TAnalytics.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.Infrastructure.Models
{
    public class AdminPlansWidgetModel
    {
        public List<WidgetAccess> WidgetsAccess { get; set; }
        public int PlanID { get; set; }

        public double InfrastructureCost { get; set; }

        
    }
}
