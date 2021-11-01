using S2TAnalytics.Common.Helper;
using S2TAnalytics.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.Infrastructure.Models
{
    public class AccountTransactionHistoryModel
    {
        public string AccountTransactionHistoryId { get; set; }
        public int TicketId { get; set; }
        public string AccountId { get; set; }
        public string TransactionType { get; set; }
        public string TransactionReason { get; set; }
        public Boolean TakeProfit { get; set; }
        public Boolean StopLoss { get; set; }
        public Boolean TrailingStop { get; set; }
        public Boolean UpperBound { get; set; }
        public Boolean LowerBound { get; set; }
        public string Side { get; set; }
        public string Instrument { get; set; }
        public double Volume { get; set; }
        public double OpenPrice { get; set; }
        public double ClosePrice { get; set; }
        public DateTime TransactionCreatedOn { get; set; }
        public DateTime TransactionClosedOn { get; set; }
        public double ProfitLoss { get; set; }
        public double Amount { get; set; }
        public double Interest { get; set; }
        public DateTime Expiry { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public Boolean Status { get; set; }


        public List<AccountTransactionHistory> ToAccountTransactionHistory(List<AccountTransactionHistoryModel> accountTransactionHistoryModels)
        {
          //  if (accountTransactionHistoryModels == null && accountTransactionHistoryModels.Count <= 0)
                return new List<AccountTransactionHistory>();
            //return accountTransactionHistoryModels.Select(accountTransactionHistoryModel => new AccountTransactionHistory
            //{
            //    AccountTransactionHistoryId = accountTransactionHistoryModel.AccountTransactionHistoryId.Decrypt(),
            //    TicketId = accountTransactionHistoryModel.TicketId,
            //    AccountId = accountTransactionHistoryModel.AccountId,
            //    TransactionType = accountTransactionHistoryModel.TransactionType,
            //    TransactionReason = accountTransactionHistoryModel.TransactionReason,
            //    TakeProfit = accountTransactionHistoryModel.TakeProfit,
            //    StopLoss = accountTransactionHistoryModel.StopLoss,
            //    TrailingStop = accountTransactionHistoryModel.TrailingStop,
            //    UpperBound = accountTransactionHistoryModel.UpperBound,
            //    LowerBound = accountTransactionHistoryModel.LowerBound,
            //    Side = accountTransactionHistoryModel.Side,
            //    Instrument = accountTransactionHistoryModel.Instrument,
            //    Volume = accountTransactionHistoryModel.Volume,
            //    OpenPrice = accountTransactionHistoryModel.OpenPrice,
            //    ClosePrice = accountTransactionHistoryModel.ClosePrice,
            //    TransactionCreatedOn = accountTransactionHistoryModel.TransactionCreatedOn.ToString(),
            //    TransactionClosedOn = accountTransactionHistoryModel.TransactionClosedOn.ToString(),
            //    ProfitLoss = accountTransactionHistoryModel.ProfitLoss,
            //    Amount = accountTransactionHistoryModel.Amount,
            //    Interest = accountTransactionHistoryModel.Interest,
            //    Expiry = accountTransactionHistoryModel.Expiry.ToString(),
            //    CreatedBy = accountTransactionHistoryModel.CreatedBy,
            //    CreatedOn = accountTransactionHistoryModel.CreatedOn.ToString(),
            //    UpdatedBy = accountTransactionHistoryModel.UpdatedBy,
            //    UpdatedOn = accountTransactionHistoryModel.UpdatedOn.ToString(),
            //    Status = accountTransactionHistoryModel.Status
            //}).ToList();
        }


        public AccountTransactionHistory ToAccountTransactionHistory(AccountTransactionHistoryModel accountTransactionHistoryModel)
        {
            //if (accountTransactionHistoryModel == null)
                return new AccountTransactionHistory();
            //return new AccountTransactionHistory
            //{
            //    AccountTransactionHistoryId = accountTransactionHistoryModel.AccountTransactionHistoryId.Decrypt(),
            //    TicketId = accountTransactionHistoryModel.TicketId,
            //    AccountId = accountTransactionHistoryModel.AccountId,
            //    TransactionType = accountTransactionHistoryModel.TransactionType,
            //    TransactionReason = accountTransactionHistoryModel.TransactionReason,
            //    TakeProfit = accountTransactionHistoryModel.TakeProfit,
            //    StopLoss = accountTransactionHistoryModel.StopLoss,
            //    TrailingStop = accountTransactionHistoryModel.TrailingStop,
            //    UpperBound = accountTransactionHistoryModel.UpperBound,
            //    LowerBound = accountTransactionHistoryModel.LowerBound,
            //    Side = accountTransactionHistoryModel.Side,
            //    Instrument = accountTransactionHistoryModel.Instrument,
            //    Volume = accountTransactionHistoryModel.Volume,
            //    OpenPrice = accountTransactionHistoryModel.OpenPrice,
            //    ClosePrice = accountTransactionHistoryModel.ClosePrice,
            //    TransactionCreatedOn = accountTransactionHistoryModel.TransactionCreatedOn.ToString(),
            //    TransactionClosedOn = accountTransactionHistoryModel.TransactionClosedOn.ToString(),
            //    ProfitLoss = accountTransactionHistoryModel.ProfitLoss,
            //    Amount = accountTransactionHistoryModel.Amount,
            //    Interest = accountTransactionHistoryModel.Interest,
            //    Expiry = accountTransactionHistoryModel.Expiry.ToString(),
            //    CreatedBy = accountTransactionHistoryModel.CreatedBy,
            //    CreatedOn = accountTransactionHistoryModel.CreatedOn.ToString(),
            //    UpdatedBy = accountTransactionHistoryModel.UpdatedBy,
            //    UpdatedOn = accountTransactionHistoryModel.UpdatedOn.ToString(),
            //    Status = accountTransactionHistoryModel.Status
            //};
        }


        public List<AccountTransactionHistoryModel> ToAccountTransactionHistoryModel(List<AccountTransactionHistory> accountTransactionHistories)
        {
            if (accountTransactionHistories == null && accountTransactionHistories.Count <= 0)
                return new List<AccountTransactionHistoryModel>();
                return new List<AccountTransactionHistoryModel>();
            //return accountTransactionHistories.Select(accountTransactionHistory => new AccountTransactionHistoryModel
            //{
            //    AccountTransactionHistoryId = accountTransactionHistory.AccountTransactionHistoryId.Encrypt(),
            //    TicketId = accountTransactionHistory.TicketId,
            //    AccountId = accountTransactionHistory.AccountId,
            //    TransactionType = accountTransactionHistory.TransactionType,
            //    TransactionReason = accountTransactionHistory.TransactionReason,
            //    TakeProfit = accountTransactionHistory.TakeProfit,
            //    StopLoss = accountTransactionHistory.StopLoss,
            //    TrailingStop = accountTransactionHistory.TrailingStop,
            //    UpperBound = accountTransactionHistory.UpperBound,
            //    LowerBound = accountTransactionHistory.LowerBound,
            //    Side = accountTransactionHistory.Side,
            //    Instrument = accountTransactionHistory.Instrument,
            //    Volume = accountTransactionHistory.Volume,
            //    OpenPrice = accountTransactionHistory.OpenPrice,
            //    ClosePrice = accountTransactionHistory.ClosePrice,
            //    TransactionCreatedOn = Convert.ToDateTime(accountTransactionHistory.TransactionCreatedOn),
            //    TransactionClosedOn = Convert.ToDateTime(accountTransactionHistory.TransactionClosedOn),
            //    ProfitLoss = accountTransactionHistory.ProfitLoss,
            //    Amount = accountTransactionHistory.Amount,
            //    Interest = accountTransactionHistory.Interest,
            //    Expiry = Convert.ToDateTime(accountTransactionHistory.Expiry),
            //    CreatedBy = accountTransactionHistory.CreatedBy,
            //    CreatedOn = Convert.ToDateTime(accountTransactionHistory.CreatedOn),
            //    UpdatedBy = accountTransactionHistory.UpdatedBy,
            //    UpdatedOn = Convert.ToDateTime(accountTransactionHistory.UpdatedOn),
            //    Status = accountTransactionHistory.Status
            //}).ToList();
        }


        public AccountTransactionHistoryModel ToAccountTransactionHistoryModel(AccountTransactionHistory accountTransactionHistory)
        {
          //  if (accountTransactionHistory == null)
                return new AccountTransactionHistoryModel();
            //return new AccountTransactionHistoryModel
            //{
            //    AccountTransactionHistoryId = accountTransactionHistory.AccountTransactionHistoryId.Encrypt(),
            //    TicketId = accountTransactionHistory.TicketId,
            //    AccountId = accountTransactionHistory.AccountId,
            //    TransactionType = accountTransactionHistory.TransactionType,
            //    TransactionReason = accountTransactionHistory.TransactionReason,
            //    TakeProfit = accountTransactionHistory.TakeProfit,
            //    StopLoss = accountTransactionHistory.StopLoss,
            //    TrailingStop = accountTransactionHistory.TrailingStop,
            //    UpperBound = accountTransactionHistory.UpperBound,
            //    LowerBound = accountTransactionHistory.LowerBound,
            //    Side = accountTransactionHistory.Side,
            //    Instrument = accountTransactionHistory.Instrument,
            //    Volume = accountTransactionHistory.Volume,
            //    OpenPrice = accountTransactionHistory.OpenPrice,
            //    ClosePrice = accountTransactionHistory.ClosePrice,
            //    TransactionCreatedOn = Convert.ToDateTime(accountTransactionHistory.TransactionCreatedOn),
            //    TransactionClosedOn = Convert.ToDateTime(accountTransactionHistory.TransactionClosedOn),
            //    ProfitLoss = accountTransactionHistory.ProfitLoss,
            //    Amount = accountTransactionHistory.Amount,
            //    Interest = accountTransactionHistory.Interest,
            //    Expiry = Convert.ToDateTime(accountTransactionHistory.Expiry),
            //    CreatedBy = accountTransactionHistory.CreatedBy,
            //    CreatedOn = Convert.ToDateTime(accountTransactionHistory.CreatedOn),
            //    UpdatedBy = accountTransactionHistory.UpdatedBy,
            //    UpdatedOn = Convert.ToDateTime(accountTransactionHistory.UpdatedOn),
            //    Status = accountTransactionHistory.Status
            //};
        }

    }
}
