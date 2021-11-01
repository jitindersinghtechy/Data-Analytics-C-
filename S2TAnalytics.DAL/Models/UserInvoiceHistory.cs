using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.DAL.Models
{
    public class UserInvoiceHistory
    {
        public Guid Id { get; set; }
        public int PlanId { get; set; }
        public string PlanName { get; set; }
        public DateTime TransactionDate { get; set; }
        public double Price { get; set; }
        public double ApplicationFee { get; set; }
        public double ServiceFee { get; set; }
        public double InfrastructureCost { get; set; }
        public double UsedUserCreditAmount { get; set; }

        public double Discount { get; set; }
        public string AppliedCoupon { get; set; }

        public string Status { get; set; }
        public string FailureCode { get; set; }
        public string FailureMessage { get; set; }
        public bool Paid { get; set; }  
    }
}
