using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.DAL.Models
{
    public class RiskFreeRatesForSharpRatio
    {
        public Guid OrganizationID { get; set; }
        public int TimeLineId { get; set; }
        public double RiskFreeRate { get; set; }
    }
}
