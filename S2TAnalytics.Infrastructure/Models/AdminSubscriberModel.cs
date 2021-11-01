using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.Infrastructure.Models
{
    public class AdminSubscriberModel
    {
        public AdminSubscriberModel()
        {

        }
        public ObjectId UserID { get; set; }
        public string Name { get; set; }
        public string PlanName { get; set; }
        public string EmailID { get; set; }
        public string PhoneNumber { get; set; }
        public int ReportUser { get; set; }
        public double MonthlyRatePerAccount { get; set; }
        public double BillAmount { get; set; }
        public string  PaymentStatus { get; set; }
        public string OpenRequest { get; set; }

        public bool IsTrial { get; set; }
    }
}
