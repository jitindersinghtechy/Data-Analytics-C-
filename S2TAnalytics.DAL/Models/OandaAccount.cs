using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using OANDARestLibrary.TradeLibrary.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.DAL.Models
{
  public  class OandaAccount : BaseEntity
    {
        public OandaAccount()
        {
            TransactionHistories = new List<Transaction>();
        }
        public bool HasAccountCurrency;
        public bool HasAccountId;
        public bool HasAccountName;
        [IsOptional]
        public bool HasBalance;
        [IsOptional]
        public bool HasMarginAvail;
        public bool HasMarginRate;
        [IsOptional]
        public bool HasMarginUsed;
        [IsOptional]
        public bool HasOpenOrders;
        [IsOptional]
        public bool HasOpenTrades;
        [IsOptional]
        public bool HasRealizedPl;
        [IsOptional]
        public bool HasUnrealizedPl;
        public string accountCurrency { get; set; }
        public string accountId { get; set; }
        public string accountName { get; set; }
        public string balance { get; set; }
        public string marginAvail { get; set; }
        public string marginRate { get; set; }
        public string marginUsed { get; set; }
        public string openOrders { get; set; }
        public string openTrades { get; set; }
        public string realizedPl { get; set; }
        public string unrealizedPl { get; set; }
        public List<Transaction> TransactionHistories { get; set; }
    }
}
