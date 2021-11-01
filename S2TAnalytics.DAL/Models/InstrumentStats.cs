using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.DAL.Models
{
    public class InstrumentStats:BaseEntity
    {
        public Object AccountStatsId { get; set; }
        public int InstrumentId { get; set; }
        public string InstrumentName { get; set; }
        public int TimeLineId { get; set; }
        public double NAV { get; set; }
        public double ROI { get; set; }
        public double WINRate { get; set; }
        public double BuyRate { get; set; }
        public double Volume { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatedOn { get; set; }
        public bool Status { get; set; }
        public double Profit { get; set; }
        public double Loss { get; set; }
    }
}
