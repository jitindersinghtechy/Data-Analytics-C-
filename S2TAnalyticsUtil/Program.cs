using P23.MetaTrader4.Manager.Contracts;
using S2TAnalytics.Common.Enums;
using S2TAnalytics.DAL.Models;
using S2TAnalytics.DAL.UnitOfWork;
using S2TAnalytics.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using S2TAnalytics.Infrastructure.Models;
using MongoDB.Driver;

namespace S2TAnalyticsUtil
{
    public class Program
    {
        private static UnitOfWork unitOfWork = new UnitOfWork(ConfigurationManager.AppSettings["ELTConnectionString"], ConfigurationManager.AppSettings["DBName"]);
        private static ELTService _eLTService = new ELTService(unitOfWork);
        static void Main(string[] args)
        {
            var runType = "MT4";
            if (runType == "MT4")
            {
                RunMT4UserRequests();
            }
            else
            {
                RunOandaAccounts();
            }
        }
        #region MT4
        private static void RunMT4UserRequests()
        {
            var dataSources = unitOfWork.DatasourceRepository.GetAll().Where(x => x.DatasourceId == (int)DataSourcesEnum.MT4).ToList();

            foreach (var dataSource in dataSources)
            {
                //var userRequests = _eLTService.GetUsers();
                var userRequests = _eLTService.GetUsersByOrganizationId(dataSource.OrganizationId);
                CalculateDailyNav(userRequests);
                foreach (var userRequest in userRequests)
                {
                    var existingAccountDetail = _eLTService.getAccountDetailByAccountNumber(userRequest.Login.ToString());
                    if (existingAccountDetail == null)
                    {
                        AccountDetail account = new AccountDetail();
                        account.OrganizationId = dataSource.OrganizationId;
                        account.DataSourceId = (int)DataSourcesEnum.MT4;
                        account.AccountNumber = userRequest.Login.ToString();
                        account.Name = userRequest.Name;
                        account.Balance = Math.Round(userRequest.Balance, 2);
                        account.Leverage = userRequest.Leverage;
                        account.Country = userRequest.Country;
                        account.City = userRequest.City;
                        account.Address = userRequest.Address;
                        account.Email = userRequest.Email;
                        account.Phone = userRequest.Phone;
                        account.Comment = userRequest.Comment;
                        account.UserGroup = userRequest.Group;
                        account.ActiveSince = userRequest.TradesHistories.Count > 0 ? SecondsToDate(userRequest.TradesHistories[0].OpenTime).ToString() : "";
                        account.LastActiveOn = SecondsToDate(userRequest.LastDate).ToString();
                        //account.CreatedBy = 
                        account.CreatedOn = DateTime.UtcNow.ToString();
                        account.Status = userRequest.Status;

                        account.AccountStats = AddAcountStats(userRequest.TradesHistories, userRequest.Login, userRequest.Leverage);
                        account.InstrumentStats = AddInstrumentStats(userRequest.Name);
                        //account.AccountTransactionHistories = AddAccountTransactionHistoryStats();
                        unitOfWork.AccountDetailRepository.Add(account);
                    }
                    else
                    {
                        var accountStats = AddAcountStats(userRequest.TradesHistories, userRequest.Login, userRequest.Leverage);
                        //account.InstrumentStats = AddInstrumentStats(userRequest.Name);
                        var update = Builders<AccountDetail>.Update.Set("AccountStats", existingAccountDetail.AccountStats);
                        var result = _eLTService.UpdateAccountDetail(existingAccountDetail.Id.ToString(), update);
                    }
                }
            }
        }

        public static List<InstrumentStats> AddInstrumentStats(string name)
        {
            var instrumentName = new List<string> { "EUR/USD", "GBP/SGD", "SGD/AUD" }.ToList();
            var timeLine = new List<int> { 1, 2, 3 }.ToList();

            var AccountStatsList = new List<InstrumentStats>();

            int count = 11;
            for (int i = 1; i <= 45; i++)
            {
                foreach (var item in instrumentName.Select((Value, Index) => new { Value, Index }))
                {
                    InstrumentStats accountStats = new InstrumentStats();

                    accountStats.AccountStatsId = i.ToString();
                    accountStats.BuyRate = MathCalculations.GenerateRandomNo(3);
                    accountStats.CreatedBy = name;
                    //accountStats.CreatedOn = GenerateRandomDate();
                    accountStats.InstrumentId = item.Index + 1;
                    //InstrumentMasterEnum.EURUSD.ToString();
                    accountStats.Volume = MathCalculations.GenerateRandomNo(2);
                    accountStats.WINRate = MathCalculations.GenerateRandomNo(2);
                    accountStats.NAV = MathCalculations.GenerateRandomNo(2);
                    accountStats.ROI = MathCalculations.GenerateRandomNo(2);
                    accountStats.Status = false;

                    accountStats.TimeLineId = i;
                    //accountStats.UpdatedBy = name;
                    //accountStats.UpdatedOn = GenerateRandomDate();
                    AccountStatsList.Add(accountStats);

                    ++count;
                }
            }

            return AccountStatsList;
        }

        private static void CalculateDailyNav(List<MT4UserRequestModel> userRequests)
        {
            try
            {
                foreach (var userRequest in userRequests)
                {
                    var existingtDailyNavs = _eLTService.getDailyNavsByAccountNumber(userRequest.Login);
                    if (existingtDailyNavs == null)
                    {
                        if (userRequest.TradesHistories.Count() > 0)
                        {
                            var NAVByDates = new List<NAVByDate>();
                            var startDate = SecondsToDate(userRequest.TradesHistories[0].OpenTime).Date;
                            var beginingDate = startDate;
                            var endDate = startDate.AddDays(1);
                            var balance = userRequest.TradesHistories[0].Profit;
                            var id = 1;
                            var previousDateNav = new double();
                            do
                            {
                                var startSeconds = (uint)(Int32)(startDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                                var endSeconds = (uint)(Int32)(endDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                                var buyTrades = userRequest.TradesHistories.Where(x => (x.OpenTime >= startSeconds && x.OpenTime < endSeconds) && x.Cmd == TradeCommand.Buy).Sum(x => x.Profit);
                                var sellTrades = userRequest.TradesHistories.Where(x => (x.CloseTime >= startSeconds && x.CloseTime < endSeconds) && x.Cmd == TradeCommand.Sell).Sum(x => x.Profit);
                                var balanaceWithdrawn = new double();
                                if (startDate != beginingDate)
                                {
                                    balanaceWithdrawn = userRequest.TradesHistories.Where(x => (x.CloseTime >= startSeconds && x.CloseTime < endSeconds) && x.Cmd == TradeCommand.Balance).Sum(x => x.Profit);
                                }
                                balance = balance + balanaceWithdrawn + buyTrades + sellTrades;

                                var sharpeRatioReturn = new double();
                                if (startDate != SecondsToDate(userRequest.TradesHistories[0].OpenTime).Date)
                                {
                                    sharpeRatioReturn = (balance - previousDateNav) / previousDateNav;
                                    previousDateNav = balance;
                                }
                                else
                                {
                                    sharpeRatioReturn = 0;
                                    previousDateNav = balance;
                                }

                                var nAVByDate = new NAVByDate();
                                nAVByDate.date = startDate.ToString();
                                nAVByDate.NAV = Math.Round(balance, 2);
                                nAVByDate.id = id;
                                nAVByDate.SharpeRatioReturn = Math.Round(sharpeRatioReturn, 2);
                                NAVByDates.Add(nAVByDate);

                                startDate = startDate.AddDays(1);
                                endDate = endDate.AddDays(1);
                                id++;
                            } while (startDate < DateTime.Today);

                            DailyNAV dailyNAVOfUser = new DailyNAV();
                            dailyNAVOfUser.AccountNumber = userRequest.Login;
                            dailyNAVOfUser.NAVByDate = NAVByDates;
                            unitOfWork.DailyNAVRepository.Add(dailyNAVOfUser);
                        }
                    }
                    else
                    {
                        if (userRequest.TradesHistories.Count() > 0)
                        {
                            var startDate = Convert.ToDateTime(existingtDailyNavs.NAVByDate[existingtDailyNavs.NAVByDate.Count - 1].date).AddDays(1);
                            var endDate = startDate.AddDays(1);
                            var beginingDate = startDate;
                            var balance = existingtDailyNavs.NAVByDate[existingtDailyNavs.NAVByDate.Count - 1].NAV;
                            var id = 1;
                            var previousDateNav = new double();
                            if (startDate < DateTime.Today)
                            {
                                do
                                {
                                    var startSeconds = (uint)(Int32)(startDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                                    var endSeconds = (uint)(Int32)(endDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                                    var buyTrades = userRequest.TradesHistories.Where(x => (x.OpenTime >= startSeconds && x.OpenTime < endSeconds) && x.Cmd == TradeCommand.Buy).Sum(x => x.Profit);
                                    var sellTrades = userRequest.TradesHistories.Where(x => (x.CloseTime >= startSeconds && x.CloseTime < endSeconds) && x.Cmd == TradeCommand.Sell).Sum(x => x.Profit);
                                    var balanaceWithdrawn = new double();
                                    if (startDate != beginingDate)
                                    {
                                        balanaceWithdrawn = userRequest.TradesHistories.Where(x => (x.CloseTime >= startSeconds && x.CloseTime < endSeconds) && x.Cmd == TradeCommand.Balance).Sum(x => x.Profit);
                                    }
                                    balance = balance + balanaceWithdrawn + buyTrades + sellTrades;

                                    var sharpeRatioReturn = new double();
                                    if (startDate != SecondsToDate(userRequest.TradesHistories[0].OpenTime).Date)
                                    {
                                        sharpeRatioReturn = (balance - previousDateNav) / previousDateNav;
                                        previousDateNav = balance;
                                    }
                                    else
                                    {
                                        sharpeRatioReturn = 0;
                                        previousDateNav = balance;
                                    }

                                    var nAVByDate = new NAVByDate();
                                    nAVByDate.date = startDate.ToString();
                                    nAVByDate.NAV = Math.Round(balance, 2);
                                    nAVByDate.id = id;
                                    nAVByDate.SharpeRatioReturn = Math.Round(sharpeRatioReturn, 2);
                                    existingtDailyNavs.NAVByDate.Add(nAVByDate);

                                    startDate = startDate.AddDays(1);
                                    endDate = endDate.AddDays(1);
                                    id++;

                                } while (startDate < DateTime.Today);

                                var update = Builders<DailyNAV>.Update.Set("NAVByDate", existingtDailyNavs.NAVByDate);
                                var result = _eLTService.UpdateDailyNAV(existingtDailyNavs.Id.ToString(), update);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public static List<AccountStats> AddAcountStats(List<TradeRecord> trades, int accountNumber, int leverage)
        {
            var dailyNavs = unitOfWork.DailyNAVRepository.GetAll().Where(x => x.AccountNumber == accountNumber).FirstOrDefault();
            var AccountStatsList = new List<AccountStats>();

            for (int i = 1; i <= 44; i++)
            {
                AccountStats accountStats = new AccountStats();
                accountStats.AccountId = i.ToString();
                accountStats.Balance = MathCalculations.GenerateRandomNo(6);
                accountStats.BestPL = CalculateBestPLForTimeline(i, trades);
                accountStats.CreatedBy = MathCalculations.GenerateRandomNo(2);
                accountStats.Leverage = leverage;
                accountStats.CreatedOn = DateTime.UtcNow.ToString();
                accountStats.DD = dailyNavs == null ? 0 : CalculateDDForTimeline(i, dailyNavs);
                accountStats.NAV = dailyNavs == null ? 0 : CalculateNavForTimeline(i, dailyNavs); //Math.Round(dailyNavs.NAVByDate[dailyNavs.NAVByDate.Count - 1].NAV, 2);
                accountStats.ROI = dailyNavs == null ? 0 : CalculateROIForTimeline(i, dailyNavs, trades);
                accountStats.SharpRatio = dailyNavs == null ? 0 : CalculateSharpRatioForTimeline(i, dailyNavs);
                accountStats.Status = true;
                accountStats.TimeLineId = i;
                //accountStats.UpdatedBy = MathCalculations.GenerateRandomNo(2);
                //accountStats.UpdatedOn = GenerateRandomDate();
                accountStats.WINRate = CalculateWINForTimeline(i, trades);
                accountStats.WorstPL = CalculateWorstPLForTimeline(i, trades);

                //_unitOfWork.AccountStatsRepository.Add(accountStats);
                AccountStatsList.Add(accountStats);
            }

            return AccountStatsList;
        }

        private static double CalculateSharpRatioForTimeline(int timelineId, DailyNAV dailyNavs)
        {
            var startDate = GetStartDateByTimeLineID(timelineId, DateTime.Today.AddDays(-1).Date);
            var endDate = GetEndDateByTimeLineID(timelineId, startDate);
            var riskFreeRate = ConfigurationManager.AppSettings["RiskFreeRateForSharpeRatio"];
            var startingNav = dailyNavs.NAVByDate.Where(x => x.date == startDate.ToString()).Select(x => x.NAV).FirstOrDefault() == 0 ? 0 :
                                                       dailyNavs.NAVByDate.Where(x => x.date == startDate.ToString()).Select(x => x.NAV).FirstOrDefault();

            var endingNav = dailyNavs.NAVByDate.Where(x => x.date == endDate.ToString()).Select(x => x.NAV).FirstOrDefault() == 0 ? 0 :
                                                      dailyNavs.NAVByDate.Where(x => x.date == endDate.ToString()).Select(x => x.NAV).FirstOrDefault();
            if (startingNav == 0 || startingNav == endingNav)
                return 0;
            else
            {
                var portfolioReturn = (endingNav - startingNav) / startingNav;
                var standardDev = calculateStandardDeviation(new List<double> { startingNav, endingNav });

                var sharpeRatio = Math.Round((portfolioReturn - Convert.ToDouble(riskFreeRate)) / standardDev, 2);
                return sharpeRatio;
            }
        }

        private static double calculateStandardDeviation(List<double> navsList)
        {
            double average = navsList.Average();
            double sumOfSquaresOfDifferences = navsList.Select(val => (val - average) * (val - average)).Sum();
            double sd = Math.Sqrt(sumOfSquaresOfDifferences / navsList.Count);
            return sd;
        }

        private static double CalculateNavForTimeline(int timelineId, DailyNAV dailyNavs)
        {
            var startDate = GetStartDateByTimeLineID(timelineId, DateTime.Today.AddDays(-1).Date);
            var endDate = GetEndDateByTimeLineID(timelineId, startDate);

            var endingNav = dailyNavs.NAVByDate.Where(x => x.date == endDate.ToString()).Select(x => x.NAV).FirstOrDefault() == 0 ? 0 :
                                                      dailyNavs.NAVByDate.Where(x => x.date == endDate.ToString()).Select(x => x.NAV).FirstOrDefault();
            return endingNav;
        }

        private static string CalculateBestPLForTimeline(int timelineId, List<TradeRecord> trades)
        {
            var startDate = GetStartDateByTimeLineID(timelineId, DateTime.Today.AddDays(-1).Date);
            var startSeconds = (uint)(Int32)(startDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            var endDate = GetEndDateByTimeLineID(timelineId, startDate);
            var endSeconds = (uint)(Int32)(endDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            var bestBuyTrades = trades.Where(x => (x.OpenTime >= startSeconds && x.OpenTime < endSeconds) && x.Cmd == TradeCommand.Buy).OrderByDescending(x => x.Profit).FirstOrDefault();
            var bestSellTrades = trades.Where(x => (x.CloseTime >= startSeconds && x.CloseTime < endSeconds) && x.Cmd == TradeCommand.Sell).OrderByDescending(x => x.Profit).FirstOrDefault();

            if (bestBuyTrades == null && bestSellTrades == null)
                return "0";
            else if (bestBuyTrades == null && bestSellTrades != null)
                return Math.Round(bestSellTrades.Profit, 2).ToString();
            else if (bestBuyTrades != null && bestSellTrades == null)
                return Math.Round(bestBuyTrades.Profit, 2).ToString();
            else
                return bestBuyTrades.Profit > bestSellTrades.Profit ? Math.Round(bestBuyTrades.Profit, 2).ToString() : Math.Round(bestSellTrades.Profit, 2).ToString();
        }

        private static string CalculateWorstPLForTimeline(int timelineId, List<TradeRecord> trades)
        {
            var startDate = GetStartDateByTimeLineID(timelineId, DateTime.Today.AddDays(-1).Date);
            var startSeconds = (uint)(Int32)(startDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            var endDate = GetEndDateByTimeLineID(timelineId, startDate);
            var endSeconds = (uint)(Int32)(endDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            var worstBuyTrades = trades.Where(x => (x.OpenTime >= startSeconds && x.OpenTime < endSeconds) && x.Cmd == TradeCommand.Buy).OrderBy(x => x.Profit).FirstOrDefault();
            var worstSellTrades = trades.Where(x => (x.CloseTime >= startSeconds && x.CloseTime < endSeconds) && x.Cmd == TradeCommand.Sell).OrderBy(x => x.Profit).FirstOrDefault();

            if (worstBuyTrades == null && worstSellTrades == null)
                return "0";
            else if (worstBuyTrades == null && worstSellTrades != null)
                return Math.Round(worstSellTrades.Profit, 2).ToString();
            else if (worstBuyTrades != null && worstSellTrades == null)
                return Math.Round(worstBuyTrades.Profit, 2).ToString();
            else
                return worstBuyTrades.Profit > worstSellTrades.Profit ? Math.Round(worstSellTrades.Profit, 2).ToString() : Math.Round(worstBuyTrades.Profit, 2).ToString();
        }

        private static double CalculateWINForTimeline(int timelineId, List<TradeRecord> trades)
        {
            var startDate = GetStartDateByTimeLineID(timelineId, DateTime.Today.AddDays(-1).Date);
            var startSeconds = (uint)(Int32)(startDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            var endDate = GetEndDateByTimeLineID(timelineId, startDate);
            var endSeconds = (uint)(Int32)(endDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            var buyTrades = trades.Where(x => (x.OpenTime >= startSeconds && x.OpenTime < endSeconds) && x.Cmd == TradeCommand.Buy).Count();
            var sellTrades = trades.Where(x => (x.CloseTime >= startSeconds && x.CloseTime < endSeconds) && x.Cmd == TradeCommand.Sell).Count();

            var positiveBuyTrades = trades.Where(x => (x.OpenTime >= startSeconds && x.OpenTime < endSeconds) && x.Cmd == TradeCommand.Buy && x.Profit > 0).Count();
            var positiveSellTrades = trades.Where(x => (x.CloseTime >= startSeconds && x.CloseTime < endSeconds) && x.Cmd == TradeCommand.Sell && x.Profit > 0).Count();
            var winRate = new double();
            if (buyTrades == 0 && sellTrades == 0)
                winRate = 0;
            else
                winRate = Math.Round((Convert.ToDouble(positiveSellTrades + positiveBuyTrades) / Convert.ToDouble(buyTrades + sellTrades)) * 100, 2);

            return winRate;
        }

        private static double CalculateDDForTimeline(int timelineId, DailyNAV dailyNavs)
        {
            var startDate = GetStartDateByTimeLineID(timelineId, DateTime.Today.AddDays(-1).Date);
            var startIndex = dailyNavs.NAVByDate.IndexOf(dailyNavs.NAVByDate.Where(x => x.date == startDate.ToString()).FirstOrDefault()) < 0 ? 0 :
                                                         dailyNavs.NAVByDate.IndexOf(dailyNavs.NAVByDate.Where(x => x.date == startDate.ToString()).FirstOrDefault());

            var endDate = GetEndDateByTimeLineID(timelineId, startDate);
            var endIndex = dailyNavs.NAVByDate.IndexOf(dailyNavs.NAVByDate.Where(x => x.date == endDate.ToString()).FirstOrDefault()) < 0 ? 0 :
                                                       dailyNavs.NAVByDate.IndexOf(dailyNavs.NAVByDate.Where(x => x.date == endDate.ToString()).FirstOrDefault());

            var NavsByTimeline = dailyNavs.NAVByDate.Skip(startIndex + 1).Take(endIndex - startIndex).ToList();

            var lowestNav = NavsByTimeline.OrderBy(x => x.NAV).FirstOrDefault();
            var lowestIndex = NavsByTimeline.IndexOf(lowestNav);
            var dd = new double();
            if (lowestIndex > 0)
            {
                var NAVBeforeLowestIndex = NavsByTimeline.Take(lowestIndex);
                var highestNAV = NAVBeforeLowestIndex.OrderByDescending(x => x.NAV).FirstOrDefault();

                dd = Math.Round(((lowestNav.NAV - highestNAV.NAV) / highestNAV.NAV) * 100, 2);
            }
            else
                dd = 0;
            return dd;
        }

        private static double CalculateROIForTimeline(int timelineId, DailyNAV dailyNavs, List<TradeRecord> trades)
        {
            //if (dailyNavs.AccountNumber == 439465)
            //{
            //    var a = "";
            //}
            var startDate = GetStartDateByTimeLineID(timelineId, DateTime.Today.AddDays(-1).Date);
            var endDate = GetEndDateByTimeLineID(timelineId, startDate);
            var startingBalance = dailyNavs.NAVByDate.Where(x => x.date == startDate.AddDays(-1).ToString()).Select(x => x.NAV).FirstOrDefault() == 0 ? 0 :
                                                        dailyNavs.NAVByDate.Where(x => x.date == startDate.AddDays(-1).ToString()).Select(x => x.NAV).FirstOrDefault();

            var startSeconds = (uint)(Int32)(startDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            var endSeconds = (uint)(Int32)(endDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            var buyTrades = trades.Where(x => (x.OpenTime >= startSeconds && x.OpenTime < endSeconds) && x.Cmd == TradeCommand.Buy).Sum(x => x.Profit);
            var sellTrades = trades.Where(x => (x.CloseTime >= startSeconds && x.CloseTime < endSeconds) && x.Cmd == TradeCommand.Sell).Sum(x => x.Profit);
            var Deposits = trades.Where(x => (x.CloseTime >= startSeconds && x.CloseTime < endSeconds) && x.Cmd == TradeCommand.Balance
                        && x.Profit > 0).Sum(x => x.Profit);


            var tradesAndDeposits = Deposits + buyTrades + sellTrades;
            var endingBalance = startingBalance + tradesAndDeposits;

            var roi = Math.Round(((endingBalance - startingBalance) / startingBalance) * 100, 2);

            if (startingBalance == 0)
                return 0;
            else
                return roi;
        }

        private static DateTime GetStartDateByTimeLineID(int timelineId, DateTime startDate)
        {
            switch (timelineId)
            {
                case 1:
                    var year = new DateTime(startDate.Year, 1, 1);
                    startDate = year.AddYears(-1).Date;
                    break;
                case 2:
                    var month = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = month.AddMonths(-6).Date;
                    break;
                case 3:
                    month = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = month.AddMonths(-3).Date;
                    break;
                case 4:
                    month = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = month.AddMonths(-1).Date;
                    break;
                case 5:
                    var daysOfWeek = DayOfWeek.Monday - startDate.DayOfWeek;
                    startDate = startDate.AddDays(daysOfWeek).AddDays(-14).Date;
                    break;
                case 6:
                    daysOfWeek = DayOfWeek.Monday - startDate.DayOfWeek;
                    startDate = startDate.AddDays(daysOfWeek).AddDays(-7).Date;
                    break;
                case 7:
                    year = new DateTime(startDate.Year, 1, 1);
                    startDate = year.AddMonths(-12).Date;
                    break;
                case 8:
                    year = new DateTime(startDate.Year, 1, 1);
                    startDate = year.AddMonths(-11).Date;
                    break;
                case 9:
                    year = new DateTime(startDate.Year, 1, 1);
                    startDate = year.AddMonths(-10).Date;
                    break;
                case 10:
                    year = new DateTime(startDate.Year, 1, 1);
                    startDate = year.AddMonths(-9).Date;
                    break;
                case 11:
                    year = new DateTime(startDate.Year, 1, 1);
                    startDate = year.AddMonths(-8).Date;
                    break;
                case 12:
                    year = new DateTime(startDate.Year, 1, 1);
                    startDate = year.AddMonths(-7).Date;
                    break;
                case 13:
                    year = new DateTime(startDate.Year, 1, 1);
                    startDate = year.AddMonths(-6).Date;
                    break;
                case 14:
                    year = new DateTime(startDate.Year, 1, 1);
                    startDate = year.AddMonths(-5).Date;
                    break;
                case 15:
                    year = new DateTime(startDate.Year, 1, 1);
                    startDate = year.AddMonths(-4).Date;
                    break;
                case 16:
                    year = new DateTime(startDate.Year, 1, 1);
                    startDate = year.AddMonths(-3).Date;
                    break;
                case 17:
                    year = new DateTime(startDate.Year, 1, 1);
                    startDate = year.AddMonths(-2).Date;
                    break;
                case 18:
                    year = new DateTime(startDate.Year, 1, 1);
                    startDate = year.AddMonths(-1).Date;
                    break;
                case 19:
                    month = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = month.AddDays(-84).Date;
                    break;
                case 20:
                    month = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = month.AddDays(-77).Date;
                    break;
                case 21:
                    month = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = month.AddDays(-70).Date;
                    break;
                case 22:
                    month = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = month.AddDays(-63).Date;
                    break;
                case 23:
                    month = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = month.AddDays(-56).Date;
                    break;
                case 24:
                    month = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = month.AddDays(-49).Date;
                    break;
                case 25:
                    month = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = month.AddDays(-42).Date;
                    break;
                case 26:
                    month = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = month.AddDays(-35).Date;
                    break;
                case 27:
                    month = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = month.AddDays(-28).Date;
                    break;
                case 28:
                    month = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = month.AddDays(-21).Date;
                    break;
                case 29:
                    month = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = month.AddDays(-14).Date;
                    break;
                case 30:
                    month = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = month.AddDays(-7).Date;
                    break;
                case 31:
                    daysOfWeek = DayOfWeek.Monday - startDate.DayOfWeek;
                    startDate = startDate.AddDays(daysOfWeek).AddDays(-14).Date;
                    break;
                case 32:
                    daysOfWeek = DayOfWeek.Monday - startDate.DayOfWeek;
                    startDate = startDate.AddDays(daysOfWeek).AddDays(-13).Date;

                    break;
                case 33:
                    daysOfWeek = DayOfWeek.Monday - startDate.DayOfWeek;
                    startDate = startDate.AddDays(daysOfWeek).AddDays(-12).Date;

                    break;
                case 34:
                    daysOfWeek = DayOfWeek.Monday - startDate.DayOfWeek;
                    startDate = startDate.AddDays(daysOfWeek).AddDays(-11).Date;
                    break;
                case 35:
                    daysOfWeek = DayOfWeek.Monday - startDate.DayOfWeek;
                    startDate = startDate.AddDays(daysOfWeek).AddDays(-10).Date;

                    break;
                case 36:
                    daysOfWeek = DayOfWeek.Monday - startDate.DayOfWeek;
                    startDate = startDate.AddDays(daysOfWeek).AddDays(-9).Date;
                    break;
                case 37:
                    daysOfWeek = DayOfWeek.Monday - startDate.DayOfWeek;
                    startDate = startDate.AddDays(daysOfWeek).AddDays(-8).Date;
                    break;
                case 38:
                    daysOfWeek = DayOfWeek.Monday - startDate.DayOfWeek;
                    startDate = startDate.AddDays(daysOfWeek).AddDays(-7).Date;
                    break;
                case 39:
                    daysOfWeek = DayOfWeek.Monday - startDate.DayOfWeek;
                    startDate = startDate.AddDays(daysOfWeek).AddDays(-6).Date;
                    break;
                case 40:
                    daysOfWeek = DayOfWeek.Monday - startDate.DayOfWeek;
                    startDate = startDate.AddDays(daysOfWeek).AddDays(-5).Date;
                    break;
                case 41:
                    daysOfWeek = DayOfWeek.Monday - startDate.DayOfWeek;
                    startDate = startDate.AddDays(daysOfWeek).AddDays(-4).Date;
                    break;
                case 42:
                    daysOfWeek = DayOfWeek.Monday - startDate.DayOfWeek;
                    startDate = startDate.AddDays(daysOfWeek).AddDays(-3).Date;
                    break;
                case 43:
                    daysOfWeek = DayOfWeek.Monday - startDate.DayOfWeek;
                    startDate = startDate.AddDays(daysOfWeek).AddDays(-2).Date;
                    break;
                case 44:
                    daysOfWeek = DayOfWeek.Monday - startDate.DayOfWeek;
                    startDate = startDate.AddDays(daysOfWeek).AddDays(-1).Date;
                    break;
                case 45:
                    startDate = startDate.Date;
                    break;

                default:
                    break;
            }
            return startDate;
        }

        private static DateTime GetEndDateByTimeLineID(int timelineId, DateTime startDate)
        {
            var endDate = new DateTime();
            switch (timelineId)
            {
                case 1:
                    endDate = startDate.AddYears(1).Date;
                    break;

                case 2:
                    endDate = startDate.AddMonths(6).Date;
                    break;

                case 3:
                    endDate = startDate.AddMonths(3).Date;
                    break;

                case 4:
                    endDate = startDate.AddMonths(1).Date;
                    break;

                case 5:
                    endDate = startDate.AddDays(14).Date;
                    break;

                case 6:
                    endDate = startDate.AddDays(7).Date;
                    break;

                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                case 16:
                case 17:
                case 18:
                    endDate = startDate.AddMonths(1).Date;
                    break;

                case 19:
                case 20:
                case 21:
                case 22:
                case 23:
                case 24:
                case 25:
                case 26:
                case 27:
                case 28:
                case 29:
                case 30:
                    endDate = startDate.AddDays(7).Date;
                    break;

                case 31:
                case 32:
                case 33:
                case 34:
                case 35:
                case 36:
                case 37:
                case 38:
                case 39:
                case 40:
                case 41:
                case 42:
                case 43:
                case 44:
                    endDate = startDate.AddDays(2).Date;
                    break;

                case 45:
                    endDate = startDate.Date;
                    break;

                default:
                    break;
            }
            return endDate;
        }

        public static DateTime SecondsToDate(Int64 seconds)
        {
            return new DateTime(1970, 1, 1).AddSeconds(seconds).ToLocalTime();
        }
        #endregion


        private static void RunOandaAccounts()
        {
            //string AccessToken = "c5ad79f200e1176f93eb79d189308f42-d5b241034d259888f586184af29b47dc";
            //bool isDemo = false;
            //string User = "";
            //var OandaAccounts = Rest.GetAccountListSync(AccessToken, isDemo, User);
            //foreach (var account in OandaAccounts)
            //{
            //    var existingOandaAccounts = _eLTService.GetOandaAccountByAccountId(account.accountId);
            //    if (existingOandaAccounts == null)
            //    {
            //        var acc = new OandaAccountModel
            //        {
            //            HasAccountCurrency = account.HasAccountCurrency,
            //            HasAccountId = account.HasAccountId,
            //            HasAccountName = account.HasAccountName,
            //            HasBalance = account.HasBalance,
            //            HasMarginAvail = account.HasMarginAvail,
            //            HasMarginRate = account.HasMarginRate,
            //            HasMarginUsed = account.HasMarginUsed,
            //            HasOpenOrders = account.HasOpenOrders,
            //            HasOpenTrades = account.HasOpenTrades,
            //            HasRealizedPl = account.HasRealizedPl,
            //            HasUnrealizedPl = account.HasUnrealizedPl,
            //            accountCurrency = account.accountCurrency,
            //            accountId = account.accountId,
            //            accountName = account.accountName,
            //            balance = account.balance,
            //            marginAvail = account.marginAvail,
            //            marginRate = account.marginRate,
            //            marginUsed = account.marginUsed,
            //            openOrders = account.openOrders,
            //            openTrades = account.openTrades,
            //            realizedPl = account.realizedPl,
            //            unrealizedPl = account.unrealizedPl,
            //        };
            //        var TransactionHistories = Rest.GetFullTransactionHistoryAsync(account.accountId, AccessToken, isDemo).Result;
            //        foreach (var transactionHistory in TransactionHistories)
            //        {
            //            acc.TransactionHistories.Add(transactionHistory);
            //        }
            //        var result = _eLTService.InsertOandaAccount(acc);
            //    }
            //    else
            //    {
            //        int startFrom = 1;
            //        var TransactionHistories = Rest.GetTransactionListAsync(account.accountId, AccessToken, isDemo, startFrom);
            //        foreach (var transactionHistory in TransactionHistories)
            //        {
            //            if (!existingOandaAccounts.TransactionHistories.Any(x => x.id == transactionHistory.id))
            //            {
            //                existingOandaAccounts.TransactionHistories.Add(transactionHistory);
            //            }
            //        }
            //        var update = Builders<OandaAccount>.Update.Set("TransactionHistories", existingOandaAccounts.TransactionHistories);
            //        var result = _eLTService.UpdateOandaAccount(existingOandaAccounts.OandaAccountId, update);
            //    }
            //}
        }
    }
}
