using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.Infrastructure.Models
{
    public class InvoiceModel
    {
        public string InvoiceNumber { get; set; }
        public string EmailId { get; set; }
        public string Country { get; set; }
        public string ClientName { get; set; }
        public string InvoiceDate { get; set; }
        public string PlanName { get; set; }
        public string PlanLogo { get; set; }
        public string TotalAccounts { get; set; }
        public string Price { get; set; }
        public string ApplicationFee { get; set; }
        public string ServiceFee { get; set; }
        public string Discount { get; set; }
        public string InfrastructureCost { get; set; }
        public string SubTotal { get; set; }
        public string Total { get; set; }
        public string CreditedAmount { get; set; }
    }
}
