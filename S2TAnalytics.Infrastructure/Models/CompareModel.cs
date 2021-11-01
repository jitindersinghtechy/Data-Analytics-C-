using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.Infrastructure.Models
{
    class CompareModel
    {
        public string Name { get; set; }
        public string ActiveSince { get; set; }
        public string LastTradedDate { get; set; }
        public double AvgTrade { get; set; }
        public string AccountDetailId { get; set; }
        public string AccountNumber { get; set; }
        public string TimeLineId { get; set; }
        public double ROI { get; set; }
        public double WINRate { get; set; }
        public double DD { get; set; }
        public double SharpRatio { get; set; }
        public double NAV { get; set; }
        public string BestPL { get; set; }
        public string WorstPL { get; set; }
    }
}
