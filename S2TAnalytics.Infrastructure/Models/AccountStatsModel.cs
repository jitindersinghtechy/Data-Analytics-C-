using S2TAnalytics.Common.Helper;
using S2TAnalytics.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.Infrastructure.Models
{
    public class AccountStatsModel
    {
        public string AccountStatsId { get; set; }
        public string AccountId { get; set; }
        public string TimeLineId { get; set; }
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


        public List<AccountStats> ToAccountDailyStats(List<AccountStatsModel> accountDailyStatsModels)
        {
            if (accountDailyStatsModels == null && accountDailyStatsModels.Count <= 0)
                return new List<AccountStats>();
            return accountDailyStatsModels.Select(accountDailyStatsModel => new AccountStats
            {
                AccountId = accountDailyStatsModel.AccountId,
                TimeLineId = accountDailyStatsModel.TimeLineId.Decrypt(),
                AvgTrade = accountDailyStatsModel.AvgTrade,
                ROI = accountDailyStatsModel.ROI,
                WINRate = accountDailyStatsModel.WINRate,
                DD = accountDailyStatsModel.DD,
                Leverage = accountDailyStatsModel.Leverage,
                SharpRatio = accountDailyStatsModel.SharpRatio,
                StatringBalance = accountDailyStatsModel.StatringBalance,
                Deposit = accountDailyStatsModel.Deposit,
                Withdrawn = accountDailyStatsModel.Withdrawn,
                ProfitLoss = accountDailyStatsModel.ProfitLoss,
                NAV = accountDailyStatsModel.NAV,
                BestPL = accountDailyStatsModel.BestPL,
                WorstPL = accountDailyStatsModel.WorstPL,
                CreatedBy = accountDailyStatsModel.CreatedBy,
                CreatedOn = accountDailyStatsModel.CreatedOn.ToString(),
                UpdatedBy = accountDailyStatsModel.UpdatedBy,
                UpdatedOn = accountDailyStatsModel.UpdatedOn.ToString(),
                Status = accountDailyStatsModel.Status
            }).ToList();
        }

        public AccountStats ToAccountDailyStats(AccountStatsModel accountDailyStatsModel)
        {
            if (accountDailyStatsModel == null)
                return new AccountStats();
            return new AccountStats
            {
                AccountId = accountDailyStatsModel.AccountId,
                TimeLineId = accountDailyStatsModel.TimeLineId.Decrypt(),
                AvgTrade = accountDailyStatsModel.AvgTrade,
                ROI = accountDailyStatsModel.ROI,
                WINRate = accountDailyStatsModel.WINRate,
                DD = accountDailyStatsModel.DD,
                Leverage = accountDailyStatsModel.Leverage,
                SharpRatio = accountDailyStatsModel.SharpRatio,
                StatringBalance = accountDailyStatsModel.StatringBalance,
                Deposit = accountDailyStatsModel.Deposit,
                Withdrawn = accountDailyStatsModel.Withdrawn,
                ProfitLoss = accountDailyStatsModel.ProfitLoss,
                NAV = accountDailyStatsModel.NAV,
                BestPL = accountDailyStatsModel.BestPL,
                WorstPL = accountDailyStatsModel.WorstPL,
                CreatedBy = accountDailyStatsModel.CreatedBy,
                CreatedOn = accountDailyStatsModel.CreatedOn.ToString(),
                UpdatedBy = accountDailyStatsModel.UpdatedBy,
                UpdatedOn = accountDailyStatsModel.UpdatedOn.ToString(),
                Status = accountDailyStatsModel.Status
            };
        }

        public List<AccountStatsModel> ToAccountDailyStatsModel(List<AccountStats> accountDailyStats)
        {
            if (accountDailyStats == null && accountDailyStats.Count <= 0)
                return new List<AccountStatsModel>();
            return accountDailyStats.Select(dailyStats => new AccountStatsModel
            {
                AccountStatsId = dailyStats.Id.ToString(),
                AccountId = dailyStats.AccountId.ToString(),
                TimeLineId = dailyStats.TimeLineId.Encrypt(),
                AvgTrade = dailyStats.AvgTrade,
                ROI = dailyStats.ROI,
                WINRate = dailyStats.WINRate,
                DD = dailyStats.DD,
                Leverage = dailyStats.Leverage,
                SharpRatio = dailyStats.SharpRatio,
                StatringBalance = dailyStats.StatringBalance,
                Deposit = dailyStats.Deposit,
                Withdrawn = dailyStats.Withdrawn,
                ProfitLoss = dailyStats.ProfitLoss,
                NAV = dailyStats.NAV,
                BestPL = dailyStats.BestPL,
                WorstPL = dailyStats.WorstPL,
                CreatedBy = dailyStats.CreatedBy,
                CreatedOn = dailyStats.CreatedOn == null ? "" : dailyStats.CreatedOn.ToString(),
                UpdatedBy = dailyStats.UpdatedBy,
                UpdatedOn = dailyStats.UpdatedOn == null ? "" : dailyStats.UpdatedOn.ToString(),
                Status = dailyStats.Status
            }).ToList();
        }

        public AccountStatsModel ToAccountDailyStatsModel(AccountStats dailyStats)
        {
            if (dailyStats == null)
                return new AccountStatsModel();
            return new AccountStatsModel
            {
                AccountStatsId = dailyStats.Id.ToString(),
                AccountId = dailyStats.AccountId.ToString(),
                TimeLineId = dailyStats.TimeLineId.Encrypt(),
                AvgTrade = dailyStats.AvgTrade,
                ROI = dailyStats.ROI,
                WINRate = dailyStats.WINRate,
                DD = dailyStats.DD,
                Leverage = dailyStats.Leverage,
                SharpRatio = dailyStats.SharpRatio,
                StatringBalance = dailyStats.StatringBalance,
                Deposit = dailyStats.Deposit,
                Withdrawn = dailyStats.Withdrawn,
                ProfitLoss = dailyStats.ProfitLoss,
                NAV = dailyStats.NAV,
                BestPL = dailyStats.BestPL,
                WorstPL = dailyStats.WorstPL,
                CreatedBy = dailyStats.CreatedBy,
                CreatedOn = dailyStats.CreatedOn.ToString(),
                UpdatedBy = dailyStats.UpdatedBy,
                UpdatedOn = dailyStats.UpdatedOn.ToString(),
                Status = dailyStats.Status
            };
        }
    }
}
