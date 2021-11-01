using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.Infrastructure.Models
{
    public class PerformersModel
    {
        public ObjectId AccountDetailIdObjectId { get; set; }
        public string AccountDetailId { get; set; }
        public string PerformerName { get; set; }
        public string AccountNumber { get; set; }
        public string TimeLineId { get; set; }
        public double AvgTrade { get; set; }
        public double ROI { get; set; }
        public double WINRate { get; set; }
        public double DD { get; set; }
        public int Leverage { get; set; }
        public double? SharpRatio { get; set; }
        public double Balance { get; set; }
        public double NAV { get; set; }
        public string BestPL { get; set; }
        public string WorstPL { get; set; }
        public string Location { get; set; }
        public string UserGroup { get; set; }
        public double StatringBalance { get; set; }
        public double Deposit { get; set; }
        public double ProfitLoss { get; set; }
        public double Withdrawn { get; set; }
    }
}
