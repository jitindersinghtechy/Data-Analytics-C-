using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.DAL.Models
{
    public class AccountTransactionHistory:BaseEntity
    {
        
        public int TicketId { get; set; }
        public Object AccountId { get; set; }
        public string TransactionType { get; set; }
        public string TransactionReason { get; set; }
        public int TakeProfit { get; set; }
        public string StopLoss { get; set; }
        public string TrailingStop { get; set; }
        public string UpperBound { get; set; }
        public string LowerBound { get; set; }
        public string Side { get; set; }
        public string Instrument { get; set; }
        public int Volume { get; set; }
        public int  OpenPrice { get; set; }
        public int ClosePrice { get; set; }
        public string TransactionCreatedOn { get; set; }
        public string TransactionClosedOn { get; set; }
        public int ProfitLoss { get; set; }
        public int Amount { get; set; }
        public int Interest { get; set; }
        public string Expiry { get; set; }
        public Object CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public Object UpdatedBy { get; set; }
        public string UpdatedOn { get; set; }
        public bool Status { get; set; }




    }
}
