using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.Infrastructure.Models
{
    public class UserInvoiceHistoryModel
    {
        public Guid Id { get; set; }
        public int PlanId { get; set; }
        public string PlanName { get; set; }
        public DateTime TransactionDate { get; set; }
        public double Price { get; set; }
        public string Status { get; set; }
        public bool Paid { get; set; }
        public double UsedUserCreditAmount { get; set; }
    }
}
