//using MongoDB.Bson;
using OANDARestLibrary.TradeLibrary.DataTypes;
using S2TAnalytics.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.Infrastructure.Models
{
    public class OandaAccountModel
    {
        public OandaAccountModel()
        {
            TransactionHistories = new List<Transaction>();
        }
        public string OandaAccountId { get; set; }
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


        public List<OandaAccount> ToOandaAccount(List<OandaAccountModel> oandaAccountModels)
        {
            if (oandaAccountModels == null && oandaAccountModels.Count <= 0)
                return new List<OandaAccount>();
            return oandaAccountModels.Select(oandaAccountModel => new OandaAccount
            {
                HasAccountCurrency = oandaAccountModel.HasAccountCurrency,
                HasAccountId = oandaAccountModel.HasAccountId,
                HasAccountName = oandaAccountModel.HasAccountName,
                HasBalance = oandaAccountModel.HasBalance,
                HasMarginAvail = oandaAccountModel.HasMarginAvail,
                HasMarginRate = oandaAccountModel.HasMarginRate,
                HasMarginUsed = oandaAccountModel.HasMarginUsed,
                HasOpenOrders = oandaAccountModel.HasOpenOrders,
                HasOpenTrades = oandaAccountModel.HasOpenTrades,
                HasRealizedPl = oandaAccountModel.HasRealizedPl,
                HasUnrealizedPl = oandaAccountModel.HasUnrealizedPl,
                accountCurrency = oandaAccountModel.accountCurrency,
                accountId = oandaAccountModel.accountId,
                accountName = oandaAccountModel.accountName,
                balance = oandaAccountModel.balance,
                marginAvail = oandaAccountModel.marginAvail,
                marginRate = oandaAccountModel.marginRate,
                marginUsed = oandaAccountModel.marginUsed,
                openOrders = oandaAccountModel.openOrders,
                openTrades = oandaAccountModel.openTrades,
                realizedPl = oandaAccountModel.realizedPl,
                unrealizedPl = oandaAccountModel.unrealizedPl,
                TransactionHistories = oandaAccountModel.TransactionHistories
            }).ToList();
        }


        public OandaAccount ToOandaAccount(OandaAccountModel oandaAccountModel)
        {
            if (oandaAccountModel == null )
                return new OandaAccount();
            return new OandaAccount
            {
                HasAccountCurrency = oandaAccountModel.HasAccountCurrency,
                HasAccountId = oandaAccountModel.HasAccountId,
                HasAccountName = oandaAccountModel.HasAccountName,
                HasBalance = oandaAccountModel.HasBalance,
                HasMarginAvail = oandaAccountModel.HasMarginAvail,
                HasMarginRate = oandaAccountModel.HasMarginRate,
                HasMarginUsed = oandaAccountModel.HasMarginUsed,
                HasOpenOrders = oandaAccountModel.HasOpenOrders,
                HasOpenTrades = oandaAccountModel.HasOpenTrades,
                HasRealizedPl = oandaAccountModel.HasRealizedPl,
                HasUnrealizedPl = oandaAccountModel.HasUnrealizedPl,
                accountCurrency = oandaAccountModel.accountCurrency,
                accountId = oandaAccountModel.accountId,
                accountName = oandaAccountModel.accountName,
                balance = oandaAccountModel.balance,
                marginAvail = oandaAccountModel.marginAvail,
                marginRate = oandaAccountModel.marginRate,
                marginUsed = oandaAccountModel.marginUsed,
                openOrders = oandaAccountModel.openOrders,
                openTrades = oandaAccountModel.openTrades,
                realizedPl = oandaAccountModel.realizedPl,
                unrealizedPl = oandaAccountModel.unrealizedPl,
                TransactionHistories = oandaAccountModel.TransactionHistories
            };
        }


        public List<OandaAccountModel> ToOandaAccountModel(List<OandaAccount> oandaAccounts)
        {
            if (oandaAccounts == null && oandaAccounts.Count <= 0)
                return new List<OandaAccountModel>();
            return oandaAccounts.Select(oandaAccount => new OandaAccountModel
            {
                OandaAccountId = oandaAccount.Id.ToString(),
                HasAccountCurrency = oandaAccount.HasAccountCurrency,
                HasAccountId = oandaAccount.HasAccountId,
                HasAccountName = oandaAccount.HasAccountName,
                HasBalance = oandaAccount.HasBalance,
                HasMarginAvail = oandaAccount.HasMarginAvail,
                HasMarginRate = oandaAccount.HasMarginRate,
                HasMarginUsed = oandaAccount.HasMarginUsed,
                HasOpenOrders = oandaAccount.HasOpenOrders,
                HasOpenTrades = oandaAccount.HasOpenTrades,
                HasRealizedPl = oandaAccount.HasRealizedPl,
                HasUnrealizedPl = oandaAccount.HasUnrealizedPl,
                accountCurrency = oandaAccount.accountCurrency,
                accountId = oandaAccount.accountId,
                accountName = oandaAccount.accountName,
                balance = oandaAccount.balance,
                marginAvail = oandaAccount.marginAvail,
                marginRate = oandaAccount.marginRate,
                marginUsed = oandaAccount.marginUsed,
                openOrders = oandaAccount.openOrders,
                openTrades = oandaAccount.openTrades,
                realizedPl = oandaAccount.realizedPl,
                unrealizedPl = oandaAccount.unrealizedPl,
                TransactionHistories = oandaAccount.TransactionHistories
            }).ToList();
        }


        public OandaAccountModel ToOandaAccountModel(OandaAccount oandaAccount)
        {
            if (oandaAccount == null)
                return new OandaAccountModel();
            return new OandaAccountModel
            {
                OandaAccountId = oandaAccount.Id.ToString(),
                HasAccountCurrency = oandaAccount.HasAccountCurrency,
                HasAccountId = oandaAccount.HasAccountId,
                HasAccountName = oandaAccount.HasAccountName,
                HasBalance = oandaAccount.HasBalance,
                HasMarginAvail = oandaAccount.HasMarginAvail,
                HasMarginRate = oandaAccount.HasMarginRate,
                HasMarginUsed = oandaAccount.HasMarginUsed,
                HasOpenOrders = oandaAccount.HasOpenOrders,
                HasOpenTrades = oandaAccount.HasOpenTrades,
                HasRealizedPl = oandaAccount.HasRealizedPl,
                HasUnrealizedPl = oandaAccount.HasUnrealizedPl,
                accountCurrency = oandaAccount.accountCurrency,
                accountId = oandaAccount.accountId,
                accountName = oandaAccount.accountName,
                balance = oandaAccount.balance,
                marginAvail = oandaAccount.marginAvail,
                marginRate = oandaAccount.marginRate,
                marginUsed = oandaAccount.marginUsed,
                openOrders = oandaAccount.openOrders,
                openTrades = oandaAccount.openTrades,
                realizedPl = oandaAccount.realizedPl,
                unrealizedPl = oandaAccount.unrealizedPl,
                TransactionHistories = oandaAccount.TransactionHistories
            };
        }
    }
}
