using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.DAL.Models
{
    public class AccountStats:BaseEntity
    {
        public Object AccountId { get; set; }
        public int TimeLineId { get; set; }
        public double AvgTrade { get; set; }
        public double ROI { get; set; }
        public double WINRate { get; set; }
        public double DD { get; set; }
        public int Leverage { get; set; }
        public double SharpRatio { get; set; }
        public double StatringBalance { get; set; }
        public double Deposit { get; set; }
        public double ProfitLoss { get; set; }
        public double Withdrawn { get; set; }
        public double NAV { get; set; }
        public string BestPL { get; set; }
        public string WorstPL { get; set; }
        public Object CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public Object UpdatedBy { get; set; }
        public string UpdatedOn { get; set; }
        public bool Status { get; set; }
    }
}
