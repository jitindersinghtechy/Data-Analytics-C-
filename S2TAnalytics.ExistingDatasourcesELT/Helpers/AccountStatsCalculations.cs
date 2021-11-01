using MongoDB.Bson;
using P23.MetaTrader4.Manager.Contracts;
using S2TAnalytics.Common.Enums;
using S2TAnalytics.Common.Helper;
using S2TAnalytics.DAL.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.ExistingDatasourcesELT.Helpers
{
    public static class AccountStatsCalculations
    {
        public  static double GetStartingBalanceByTimeline(int timelineId, DailyEquity dailyEquity, List<TradeRecord> trades)
        {
            if (dailyEquity != null && dailyEquity.EquityByDate.Count > 0)
            {
                var startDate = new DateTime();
                var endDate = new DateTime();
                if (timelineId == (int)TimeLineEnum.Overall) //timelineid= 45
                    startDate = Dates.SecondsToDate(trades[0].OpenTime).Date;
                else
                    startDate = Dates.GetStartDateByTimeLineID(timelineId, DateTime.Today.AddDays(-1).Date);


                var startingBalance = dailyEquity.EquityByDate.Where(x => x.date == startDate.ToString()).Select(x => x.Equity).FirstOrDefault();
                return startingBalance;
            }
            else
            {
                return 0;
            }
        }
        public static double GetDepositsByTimeline(int timelineId, DailyEquity dailyEquity, List<TradeRecord> trades)
        {
            if (trades.Count > 0)
            {
                var startDate = new DateTime();
                var endDate = new DateTime();
                if (timelineId == (int)TimeLineEnum.Overall) //timelineid= 45
                    startDate = Dates.SecondsToDate(trades[0].OpenTime).Date;
                else
                    startDate = Dates.GetStartDateByTimeLineID(timelineId, DateTime.Today.AddDays(-1).Date);

                if (Timelines.timelinesTillToday.Contains(timelineId))
                    endDate = DateTime.Today.AddDays(-1).Date;
                else
                    endDate = Dates.GetEndDateByTimeLineID(timelineId, startDate);
                var startSeconds = (uint)(Int32)(startDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                var endSeconds = (uint)(Int32)(endDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                var deposits = trades.Where(x => (x.OpenTime >= startSeconds && x.OpenTime < endSeconds) && (x.Cmd == TradeCommand.Balance || x.Cmd == TradeCommand.Credit) && x.Profit > 0).Sum(x => x.Profit);
                return deposits;
            }
            else
            {
                return 0;
            }
        }
        public static double GetWithdrawnByTimeline(int timelineId, DailyEquity dailyEquity, List<TradeRecord> trades)
        {
            if (trades.Count > 0)
            {
                var startDate = new DateTime();
                var endDate = new DateTime();
                if (timelineId == (int)TimeLineEnum.Overall) //timelineid= 45
                    startDate = Dates.SecondsToDate(trades[0].OpenTime).Date;
                else
                    startDate = Dates.GetStartDateByTimeLineID(timelineId, DateTime.Today.AddDays(-1).Date);

                if (Timelines.timelinesTillToday.Contains(timelineId))
                    endDate = DateTime.Today.AddDays(-1).Date;
                else
                    endDate = Dates.GetEndDateByTimeLineID(timelineId, startDate);
                var startSeconds = (uint)(Int32)(startDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                var endSeconds = (uint)(Int32)(endDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                var withdrawn = trades.Where(x => (x.OpenTime >= startSeconds && x.OpenTime < endSeconds) && (x.Cmd == TradeCommand.Balance || x.Cmd == TradeCommand.Credit) && x.Profit < 0).Sum(x => x.Profit);
                return withdrawn;
            }
            else
            {
                return 0;
            }
        }
        public static double GetProfitLossByTimeline(int timelineId, DailyEquity dailyEquity, List<TradeRecord> trades)
        {
            if (trades.Count > 0)
            {
                var startDate = new DateTime();
                var endDate = new DateTime();
                if (timelineId == (int)TimeLineEnum.Overall) //timelineid= 45
                    startDate = Dates.SecondsToDate(trades[0].OpenTime).Date;
                else
                    startDate = Dates.GetStartDateByTimeLineID(timelineId, DateTime.Today.AddDays(-1).Date);

                if (Timelines.timelinesTillToday.Contains(timelineId))
                    endDate = DateTime.Today.AddDays(-1).Date;
                else
                    endDate = Dates.GetEndDateByTimeLineID(timelineId, startDate);
                var startSeconds = (uint)(Int32)(startDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                var endSeconds = (uint)(Int32)(endDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                var buyTrades = trades.Where(x => (x.OpenTime >= startSeconds && x.OpenTime < endSeconds) && x.Cmd == TradeCommand.Buy).Sum(x => x.Profit);
                var sellTrades = trades.Where(x => (x.OpenTime >= startSeconds && x.OpenTime < endSeconds) && x.Cmd == TradeCommand.Sell).Sum(x => x.Profit);
                var profitLoss = buyTrades + sellTrades;
                return profitLoss;
            }
            else
            {
                return 0;
            }
        }
        public static double CalculateAvgTradesForTimeline(int timelineId, List<TradeRecord> trades)
        {
            if (trades.Count > 0)
            {
                var startDate = new DateTime();
                var endDate = new DateTime();
                if (timelineId == (int)TimeLineEnum.Overall) //timelineid= 45
                    startDate = Dates.SecondsToDate(trades[0].OpenTime).Date;
                else
                    startDate = Dates.GetStartDateByTimeLineID(timelineId, DateTime.Today.AddDays(-1).Date);

                if (Timelines.timelinesTillToday.Contains(timelineId))
                    endDate = DateTime.Today.AddDays(-1).Date;
                else
                    endDate = Dates.GetEndDateByTimeLineID(timelineId, startDate);
                var startSeconds = (uint)(Int32)(startDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                var endSeconds = (uint)(Int32)(endDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                var buyTrades = trades.Where(x => (x.OpenTime >= startSeconds && x.OpenTime < endSeconds) && x.Cmd == TradeCommand.Buy).Count();
                var sellTrades = trades.Where(x => (x.OpenTime >= startSeconds && x.OpenTime < endSeconds) && x.Cmd == TradeCommand.Sell).Count();
                var numberOfDays = (endDate - startDate).TotalDays;
                var avgTrades = Math.Round(((buyTrades + sellTrades) / numberOfDays), 2);
                return avgTrades;
            }
            else
            {
                return 0;
            }
        }
        public static double CalculateSharpRatioForTimeline(int timelineId, DailyEquity dailyEquity, ObjectId datasourceId, Datasource datasource, List<TradeRecord> trades)
        {
            if (dailyEquity.EquityByDate.Count > 0)
            {
                var startDate = new DateTime();
                var endDate = new DateTime();
                if (timelineId == (int)TimeLineEnum.Overall) //timelineid= 45
                    startDate = Convert.ToDateTime(dailyEquity.EquityByDate[0].date);
                else
                    startDate = Dates.GetStartDateByTimeLineID(timelineId, DateTime.Today.AddDays(-1).Date);

                if (Timelines.timelinesTillToday.Contains(timelineId))
                    endDate = DateTime.Today.AddDays(-1).Date;
                else
                    endDate = Dates.GetEndDateByTimeLineID(timelineId, startDate).AddDays(-1);

                var firstTradeTime = trades.Where(x => x.Cmd == TradeCommand.Buy || x.Cmd == TradeCommand.Sell).OrderBy(x => x.OpenTime).Select(x => x.OpenTime).FirstOrDefault();
                var firstTradeDate = new DateTime(1970, 1, 1, 0, 0, 1, 0).AddSeconds(firstTradeTime).Date;
                if (timelineId == (int)TimeLineEnum.Overall) //timelineid= 45
                {
                    if (startDate != firstTradeDate)
                    {
                        startDate = firstTradeDate;
                    }
                }
                else
                {
                    if (startDate < firstTradeDate && firstTradeDate < endDate)
                    {
                        startDate = firstTradeDate;
                    }
                }

                var riskFreeRate = Timelines.MasterTimelines.Any(x => x == timelineId) ? datasource.RateOfReturns.Where(x => x.TimelineId == timelineId).FirstOrDefault().RateOfReturn
                                        : Convert.ToDouble(ConfigurationManager.AppSettings["RiskFreeRateForSharpeRatio"]);
                var NavsForStatndardDeviaition = dailyEquity.EquityByDate.Where(x => Convert.ToDateTime(x.date) >= startDate && Convert.ToDateTime(x.date) <= endDate)
                                                .Select(x => x.SharpeRatioReturn).ToList();
                var startingNav = dailyEquity.EquityByDate.Where(x => x.date == startDate.ToString()).Select(x => x.EquityWithoutWithdrawn).FirstOrDefault() == 0 ?
                                  dailyEquity.EquityByDate.Where(x => x.date == endDate.ToString()).Select(x => x.EquityWithoutWithdrawn).FirstOrDefault() == 0 ? 0 : dailyEquity.EquityByDate[0].EquityWithoutWithdrawn :
                                  dailyEquity.EquityByDate.Where(x => x.date == startDate.ToString()).Select(x => x.EquityWithoutWithdrawn).FirstOrDefault();

                var endingNav = dailyEquity.EquityByDate.Where(x => x.date == endDate.ToString()).Select(x => x.EquityWithoutWithdrawn).FirstOrDefault() == 0 ? 0 :
                                                          dailyEquity.EquityByDate.Where(x => x.date == endDate.ToString()).Select(x => x.EquityWithoutWithdrawn).FirstOrDefault();
                if (startingNav == 0 || startingNav == endingNav || NavsForStatndardDeviaition.Count == 0)
                    return 0;
                else
                {
                    var portfolioReturn = (endingNav - startingNav) / startingNav;
                    //var standardDev = calculateStandardDeviation(new List<double> { startingNav, endingNav });
                    var standardDev = calculateStandardDeviation(NavsForStatndardDeviaition);

                    var sharpeRatio = Math.Round((portfolioReturn - (Convert.ToDouble(riskFreeRate) / 100)) / standardDev, 2);
                    return sharpeRatio;
                }
            }
            else
            {
                return 0;
            }
        }
        public static double calculateStandardDeviation(List<double> navsList)
        {
            double average = navsList.Average();
            double sumOfSquaresOfDifferences = navsList.Select(val => (val - average) * (val - average)).Sum();
            double sd = Math.Sqrt(sumOfSquaresOfDifferences / navsList.Count);
            return sd;
        }
        public static double CalculateNavForTimeline(int timelineId, DailyEquity dailyEquity)
        {
            if (dailyEquity.EquityByDate.Count > 0)
            {
                var startDate = new DateTime();
                var endDate = new DateTime();

                if (timelineId == (int)TimeLineEnum.Overall) //timelineid= 45
                    startDate = Convert.ToDateTime(dailyEquity.EquityByDate[0].date);
                else
                    startDate = Dates.GetStartDateByTimeLineID(timelineId, DateTime.Today.AddDays(-1).Date);

                if (Timelines.timelinesTillToday.Contains(timelineId))
                    endDate = DateTime.Today.AddDays(-1).Date;
                else
                    endDate = Dates.GetEndDateByTimeLineID(timelineId, startDate);

                var endingNav = dailyEquity.EquityByDate.Where(x => x.date == endDate.ToString()).Select(x => x.Equity).FirstOrDefault() == 0 ? 0 :
                                                          dailyEquity.EquityByDate.Where(x => x.date == endDate.ToString()).Select(x => x.Equity).FirstOrDefault();
                return endingNav;
            }
            else
            {
                return 0;
            }
        }
        public static string CalculateBestPLForTimeline(int timelineId, List<TradeRecord> trades)
        {
            if (trades.Count > 0)
            {
                var startDate = new DateTime();
                var endDate = new DateTime();
                if (timelineId == (int)TimeLineEnum.Overall) //timelineid= 45
                    startDate = Dates.SecondsToDate(trades[0].OpenTime).Date;
                else
                    startDate = Dates.GetStartDateByTimeLineID(timelineId, DateTime.Today.AddDays(-1).Date);

                if (Timelines.timelinesTillToday.Contains(timelineId))
                    endDate = DateTime.Today.AddDays(-1).Date;
                else
                    endDate = Dates.GetEndDateByTimeLineID(timelineId, startDate);

                var startSeconds = (uint)(Int32)(startDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                var endSeconds = (uint)(Int32)(endDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                var bestBuyTrades = trades.Where(x => (x.OpenTime >= startSeconds && x.OpenTime < endSeconds) && x.Cmd == TradeCommand.Buy).OrderByDescending(x => x.Profit).FirstOrDefault();
                var bestSellTrades = trades.Where(x => (x.OpenTime >= startSeconds && x.OpenTime < endSeconds) && x.Cmd == TradeCommand.Sell).OrderByDescending(x => x.Profit).FirstOrDefault();

                if (bestBuyTrades == null && bestSellTrades == null)
                    return "0";
                else if (bestBuyTrades == null && bestSellTrades != null)
                    return Math.Round(bestSellTrades.Profit, 2).ToString();
                else if (bestBuyTrades != null && bestSellTrades == null)
                    return Math.Round(bestBuyTrades.Profit, 2).ToString();
                else
                    return bestBuyTrades.Profit > bestSellTrades.Profit ? Math.Round(bestBuyTrades.Profit, 2).ToString() : Math.Round(bestSellTrades.Profit, 2).ToString();
            }
            else
            {
                return "0";
            }
        }
        public static string CalculateWorstPLForTimeline(int timelineId, List<TradeRecord> trades)
        {
            if (trades.Count > 0)
            {
                var startDate = new DateTime();
                var endDate = new DateTime();
                if (timelineId == (int)TimeLineEnum.Overall) //timelineid= 45
                    startDate = Dates.SecondsToDate(trades[0].OpenTime).Date;
                else
                    startDate = Dates.GetStartDateByTimeLineID(timelineId, DateTime.Today.AddDays(-1).Date);

                if (Timelines.timelinesTillToday.Contains(timelineId))
                    endDate = DateTime.Today.AddDays(-1).Date;
                else
                    endDate = Dates.GetEndDateByTimeLineID(timelineId, startDate);

                var startSeconds = (uint)(Int32)(startDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                var endSeconds = (uint)(Int32)(endDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                var worstBuyTrades = trades.Where(x => (x.OpenTime >= startSeconds && x.OpenTime < endSeconds) && x.Cmd == TradeCommand.Buy).OrderBy(x => x.Profit).FirstOrDefault();
                var worstSellTrades = trades.Where(x => (x.OpenTime >= startSeconds && x.OpenTime < endSeconds) && x.Cmd == TradeCommand.Sell).OrderBy(x => x.Profit).FirstOrDefault();

                if (worstBuyTrades == null && worstSellTrades == null)
                    return "0";
                else if (worstBuyTrades == null && worstSellTrades != null)
                    return Math.Round(worstSellTrades.Profit, 2).ToString();
                else if (worstBuyTrades != null && worstSellTrades == null)
                    return Math.Round(worstBuyTrades.Profit, 2).ToString();
                else
                    return worstBuyTrades.Profit > worstSellTrades.Profit ? Math.Round(worstSellTrades.Profit, 2).ToString() : Math.Round(worstBuyTrades.Profit, 2).ToString();
            }
            else
            {
                return "0";
            }
        }
        public static double CalculateWINForTimeline(int timelineId, List<TradeRecord> trades)
        {
            if (trades.Count > 0)
            {
                var startDate = new DateTime();
                var endDate = new DateTime();
                if (timelineId == (int)TimeLineEnum.Overall) //timelineid= 45
                    startDate = Dates.SecondsToDate(trades[0].OpenTime).Date;
                else
                    startDate = Dates.GetStartDateByTimeLineID(timelineId, DateTime.Today.AddDays(-1).Date);

                if (Timelines.timelinesTillToday.Contains(timelineId))
                    endDate = DateTime.Today.AddDays(-1).Date;
                else
                    endDate = Dates.GetEndDateByTimeLineID(timelineId, startDate);

                var startSeconds = (uint)(Int32)(startDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                var endSeconds = (uint)(Int32)(endDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                var buyTrades = trades.Where(x => (x.OpenTime >= startSeconds && x.OpenTime < endSeconds) && x.Cmd == TradeCommand.Buy).Count();
                var sellTrades = trades.Where(x => (x.OpenTime >= startSeconds && x.OpenTime < endSeconds) && x.Cmd == TradeCommand.Sell).Count();

                var positiveBuyTrades = trades.Where(x => (x.OpenTime >= startSeconds && x.OpenTime < endSeconds) && x.Cmd == TradeCommand.Buy && x.Profit > 0).Count();
                var positiveSellTrades = trades.Where(x => (x.OpenTime >= startSeconds && x.OpenTime < endSeconds) && x.Cmd == TradeCommand.Sell && x.Profit > 0).Count();
                var winRate = new double();
                if (buyTrades == 0 && sellTrades == 0)
                    winRate = 0;
                else
                    winRate = Math.Round((Convert.ToDouble(positiveSellTrades + positiveBuyTrades) / Convert.ToDouble(buyTrades + sellTrades)) * 100, 2);

                return winRate;
            }
            else
            {
                return 0;
            }
        }
        public static double CalculateDDForTimeline(int timelineId, DailyEquity dailyEquity, List<TradeRecord> trades)
        {
            if (dailyEquity.EquityByDate.Count > 0)
            {
                if (dailyEquity.AccountNumber == 442842)
                {
                    var a = "";
                }
                var startDate = new DateTime();
                var endDate = new DateTime();

                if (timelineId == (int)TimeLineEnum.Overall) //timelineid= 45
                    startDate = Convert.ToDateTime(dailyEquity.EquityByDate[0].date);
                else
                    startDate = Dates.GetStartDateByTimeLineID(timelineId, DateTime.Today.AddDays(-1).Date);

                if (Timelines.timelinesTillToday.Contains(timelineId))
                    endDate = DateTime.Today.AddDays(-1).Date;
                else
                    endDate = Dates.GetEndDateByTimeLineID(timelineId, startDate);

                var firstTradeTime = trades.Where(x => x.Cmd == TradeCommand.Buy || x.Cmd == TradeCommand.Sell).OrderBy(x => x.OpenTime).Select(x => x.OpenTime).FirstOrDefault();
                var firstTradeDate = new DateTime(1970, 1, 1, 0, 0, 1, 0).AddSeconds(firstTradeTime).Date;
                if (timelineId == (int)TimeLineEnum.Overall) //timelineid= 45
                {
                    if (startDate != firstTradeDate)
                    {
                        startDate = firstTradeDate;
                    }
                }
                else
                {
                    if (startDate < firstTradeDate && firstTradeDate < endDate)
                    {
                        startDate = firstTradeDate;
                    }
                }

                var startIndex = dailyEquity.EquityByDate.Where(x => x.date == startDate.AddDays(-1).ToString()).FirstOrDefault() == null ? 0 :
                                                             dailyEquity.EquityByDate.IndexOf(dailyEquity.EquityByDate.Where(x => x.date == startDate.ToString()).FirstOrDefault());

                var endIndex = dailyEquity.EquityByDate.IndexOf(dailyEquity.EquityByDate.Where(x => x.date == endDate.ToString()).FirstOrDefault()) < 0 ? 0 :
                                                           dailyEquity.EquityByDate.IndexOf(dailyEquity.EquityByDate.Where(x => x.date == endDate.ToString()).FirstOrDefault());

                var NavsByTimeline = dailyEquity.EquityByDate.Skip(dailyEquity.EquityByDate.Where(x => x.date == startDate.AddDays(-1).ToString()).FirstOrDefault() == null ? 0 : startIndex + 1).Take(endIndex - startIndex).ToList();

                var lowestNav = NavsByTimeline.OrderBy(x => x.EquityWithoutWithdrawn).FirstOrDefault();
                var lowestIndex = NavsByTimeline.IndexOf(lowestNav);
                var dd = new double();
                if (lowestIndex > 0)
                {
                    var NAVBeforeLowestIndex = NavsByTimeline.Take(lowestIndex + 1);
                    var highestNAV = NAVBeforeLowestIndex.OrderByDescending(x => x.EquityWithoutWithdrawn).FirstOrDefault();
                    if (highestNAV.EquityWithoutWithdrawn != 0)
                    {
                        dd = Math.Round(((lowestNav.EquityWithoutWithdrawn - highestNAV.EquityWithoutWithdrawn) / highestNAV.EquityWithoutWithdrawn) * 100, 2);
                    }
                    else
                    {
                        dd = 0;
                    }
                }
                else
                    dd = 0;
                return dd;
            }
            else
            {
                return 0;
            }
        }
        public static double CalculateROIForTimeline(int timelineId, DailyEquity dailyEquity, List<TradeRecord> trades)
        {
            if (trades.Count > 0 && dailyEquity.EquityByDate.Count > 0)
            {
                var startDate = new DateTime();
                var endDate = new DateTime();
                if (timelineId == (int)TimeLineEnum.Overall) //timelineid= 45
                    startDate = Convert.ToDateTime(dailyEquity.EquityByDate[0].date);
                else
                    startDate = Dates.GetStartDateByTimeLineID(timelineId, DateTime.Today.AddDays(-1).Date);

                if (Timelines.timelinesTillToday.Contains(timelineId))
                    endDate = DateTime.Today.AddDays(-1).Date;
                else
                    endDate = Dates.GetEndDateByTimeLineID(timelineId, startDate);

                var startingBalance = new double();
                if (dailyEquity.EquityByDate.Where(x => x.date == startDate.AddDays(-1).ToString()).FirstOrDefault() == null)
                {
                    startingBalance = dailyEquity.EquityByDate[0].EquityWithoutWithdrawn;
                    startDate = Convert.ToDateTime(dailyEquity.EquityByDate[0].date).AddDays(1).Date;
                }
                else
                {
                    startingBalance = dailyEquity.EquityByDate.Where(x => x.date == startDate.AddDays(-1).ToString()).Select(x => x.EquityWithoutWithdrawn).FirstOrDefault();
                }

                var startSeconds = (uint)(Int32)(startDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                var endSeconds = (uint)(Int32)(endDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                var buyTrades = trades.Where(x => (x.OpenTime >= startSeconds && x.OpenTime < endSeconds) && x.Cmd == TradeCommand.Buy).Sum(x => x.Profit);
                var sellTrades = trades.Where(x => (x.OpenTime >= startSeconds && x.OpenTime < endSeconds) && x.Cmd == TradeCommand.Sell).Sum(x => x.Profit);
                var Deposits = trades.Where(x => (x.OpenTime >= startSeconds && x.OpenTime < endSeconds) && (x.Cmd == TradeCommand.Balance || x.Cmd == TradeCommand.Credit)
                            && x.Profit > 0).Sum(x => x.Profit);


                var tradesAndDeposits = Deposits + buyTrades + sellTrades;
                var endingBalance = startingBalance + tradesAndDeposits;

                var roi = Math.Round(((endingBalance - (startingBalance + Deposits)) / (startingBalance + Deposits)) * 100, 2);

                if (startingBalance == 0)
                    return 0;
                else
                    return roi;
            }
            else
            {
                return 0;
            }
        }
    }
}
