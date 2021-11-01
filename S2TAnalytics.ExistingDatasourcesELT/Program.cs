using MongoDB.Bson;
using MongoDB.Driver;
using P23.MetaTrader4.Manager.Contracts;
using S2TAnalytics.ExistingDatasourcesELT.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using S2TAnalytics.DAL.Models;
using S2TAnalytics.Infrastructure.Services;
using S2TAnalytics.DAL.UnitOfWork;
using S2TAnalytics.Common.Enums;
using S2TAnalytics.Common.Helper;

namespace S2TAnalytics.ExistingDatasourcesELT
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
                GetMT4UserRequests();
            }
            else
            {
                //RunOandaAccounts();
            }
        }
        #region MT4
        private static void GetMT4UserRequests()
        {
            var dataSources = unitOfWork.DatasourceRepository.GetAll().Where(x => x.DatasourceId == (int)DataSourcesEnum.MT4 && x.IsConnected == true).ToList();
            foreach (var dataSource in dataSources)
            {
                try
                {
                    //MT4ELTHelper.CreateWrapper(dataSource.LoginId, dataSource.Password, dataSource.Server, @"D:\Analytics\S2TAnalytics\S2TAnalytics\S2TAnalyticsELT\Connectors\MT4\mtmanapi.dll");
                    //MT4ELTHelper.CreateWrapper(dataSource.LoginId, dataSource.Password, dataSource.Server, @"D:\Gaurav Shukla\Projects\Trading\S2TAnalytics\S2TAnalyticsELT\Connectors\MT4\mtmanapi.dll");
                    //MT4ELTHelper.CreateWrapper(dataSource.LoginId, dataSource.Password, dataSource.Server, @"D:\Rohit\Trading\S2TAnalyticsELT\Connectors\MT4\mtmanapi.dll");
                    //MT4ELTHelper.CreateWrapper(dataSource.LoginId, dataSource.Password, dataSource.Server, @"F:\Projects\Simple2Trade\S2TAnalytics\S2TAnalyticsELT\Connectors\MT4\mtmanapi.dll");
                    MT4ELTHelper.CreateWrapper(dataSource.LoginId, dataSource.Password, dataSource.Server, @"C:\connector\mtmanapi.dll");
                    var MT4Users = MT4ELTHelper.GetAccountSummaryByAccount();
                    var insertMT4Userrequests = new List<MT4UserRequest>();
                    foreach (var user in MT4Users)
                    {
                        var existingUserRequest = _eLTService.GetUsersByLogin(user.Login, dataSource.OrganizationId);
                        if (existingUserRequest == null)
                        {
                            var userRequest = new MT4UserRequest
                            {
                                Login = user.Login,
                                OrganizationId = dataSource.OrganizationId,
                                Address = user.Address,
                                AgentAccount = user.AgentAccount,
                                ApiData = user.ApiData,
                                Balance = user.Balance,
                                City = string.IsNullOrEmpty(user.City) ? "N/A" : Char.ToUpperInvariant(user.City[0]) + user.City.Substring(1).ToLower(),
                                Comment = user.Comment,
                                Country = (string.IsNullOrEmpty(user.Country) || user.Country == "null") ? "N/A" : Char.ToUpperInvariant(user.Country[0]) + user.Country.Substring(1).ToLower(),
                                Credit = user.Credit,
                                Email = user.Email,
                                Enable = user.Enable,
                                EnableChangePassword = user.EnableChangePassword,
                                EnableOTP = user.EnableOTP,
                                EnableReadOnly = user.EnableReadOnly,
                                Group = user.Group,
                                UserId = user.Id,
                                InterestRate = user.InterestRate,
                                LastDate = user.LastDate,
                                LastIp = user.LastIp,
                                LeadSource = user.LeadSource,
                                Leverage = user.Leverage,
                                Mqid = user.Mqid,
                                Name = user.Name == "" ? user.Login.ToString() : user.Name,
                                OTPSecret = user.OTPSecret,
                                Password = user.Password,
                                PasswordInvestor = user.PasswordInvestor,
                                PasswordPhone = user.PasswordPhone,
                                Phone = user.Phone,
                                PrevBalance = user.PrevBalance,
                                PrevEquity = user.PrevEquity,
                                PrevMonthBalance = user.PrevMonthBalance,
                                PrevMonthEquity = user.PrevMonthEquity,
                                PublicKey = user.PublicKey,
                                Regdate = user.Regdate,
                                SendReports = user.SendReports,
                                State = user.State,
                                Status = user.Status,
                                Taxes = user.Taxes,
                                Timestamp = user.Timestamp,
                                UserColor = Convert.ToInt64(user.UserColor),
                                ZipCode = user.ZipCode,

                            };
                            var tradesUserHistory = MT4ELTHelper.GetUserHistoryByAccount(user.Login, new DateTime(1970, 1, 1), DateTime.UtcNow);
                            foreach (var tradesHistory in tradesUserHistory)
                            {
                                userRequest.TradesHistories.Add(tradesHistory);
                            }
                            insertMT4Userrequests.Add(userRequest);
                            //var result = _eLTService.InsertUserRequest(userRequest);
                        }
                        else
                        {
                            if (existingUserRequest.Login == 437007)
                            {

                            }
                            var tradesUserHistory = MT4ELTHelper.GetUserHistoryByAccount(user.Login, new DateTime(1970, 1, 1), DateTime.UtcNow);
                            foreach (var tradesHistory in tradesUserHistory)
                            {
                                if (!existingUserRequest.TradesHistories.Any(x => x.OpenTime == tradesHistory.OpenTime && x.CloseTime == tradesHistory.CloseTime))
                                {
                                    existingUserRequest.TradesHistories.Add(tradesHistory);
                                }
                            }
                            //var update = Builders<MT4UserRequest>.Update.Set("TradesHistories", existingUserRequest.TradesHistories);
                            _eLTService.UpdateUserRequest(existingUserRequest);
                        }
                    }
                    if (insertMT4Userrequests.Any())
                    {
                        _eLTService.InsertUserRequest(insertMT4Userrequests);
                    }
                    MT4UserRequestsToAccountDetails(dataSource);
                }
                catch (Exception ex)
                {
                    AddNotification(dataSource.OrganizationId, "Failed to configure MT4 datasource");
                }
            }
        }
        private static void MT4UserRequestsToAccountDetails(Datasource dataSource)
        {
            try
            {
                var userRequests = _eLTService.GetUsersByOrganizationId(dataSource.OrganizationId);
                CalculateDailyNav(userRequests);
                List<AccountDetail> insertAccountDetail = new List<AccountDetail>();
                foreach (var userRequest in userRequests)
                {
                    var existingAccountDetail = _eLTService.getAccountDetailByAccountNumber(userRequest.Login.ToString(), dataSource.OrganizationId);
                    if (existingAccountDetail == null)
                    {
                        if (userRequest.Login == 440012)
                        {
                            //var a = "";
                        }
                        AccountDetail account = new AccountDetail();
                        account.OrganizationId = dataSource.OrganizationId;
                        //account.DataSourceId = (int)DataSourcesEnum.MT4;
                        account.DataSourceId = dataSource.Id;
                        account.AccountNumber = userRequest.Login.ToString();
                        account.Name = userRequest.Name;
                        account.Balance = Math.Round(userRequest.Balance, 2);
                        account.Leverage = userRequest.Leverage;
                        account.Country = userRequest.Country;
                        account.CountryCode = Location.CountryCodeByCountryName(userRequest.Country);
                        account.City = userRequest.City;
                        var latLong = Location.GetCityLongLat(userRequest.City, userRequest.Country);
                        account.CityLat = latLong.Where(f => f.Key == "Lat").Single().Value;
                        account.CityLong = latLong.Where(f => f.Key == "Long").Single().Value;
                        //account.State = userRequest.State;
                        //account.StateCode = GetStateNameByLatLong(account.CityLat, account.CityLong);
                        account.Address = userRequest.Address;
                        account.Email = userRequest.Email;
                        account.Phone = userRequest.Phone;
                        account.Comment = userRequest.Comment;
                        account.UserGroup = userRequest.Group;
                        account.ActiveSince = userRequest.TradesHistories.Count > 0 ? Dates.SecondsToDate(userRequest.TradesHistories[0].OpenTime).ToString() : "";
                        account.LastActiveOn = Dates.SecondsToDate(userRequest.LastDate).ToString();
                        //account.CreatedBy = 
                        account.CreatedOn = DateTime.UtcNow.ToString();
                        account.Status = userRequest.Status;
                        account.AccountStats = AddAcountStats(userRequest.TradesHistories, userRequest.Login, userRequest.Leverage, dataSource.Id);
                        account.InstrumentStats = AddInstrumentStats(userRequest.Name, userRequest.TradesHistories);
                        //account.AccountTransactionHistories = AddAccountTransactionHistoryStats();
                        //unitOfWork.AccountDetailRepository.Add(account);
                        insertAccountDetail.Add(account);
                    }
                    else
                    {
                        if (userRequest.Login == 456403)
                        {
                        }
                        existingAccountDetail.AccountStats = AddAcountStatsForExistingUsers(userRequest.TradesHistories, userRequest.Login, userRequest.Leverage, dataSource.Id, existingAccountDetail.AccountStats);
                        existingAccountDetail.InstrumentStats = AddInstrumentStatsForExistingUsers(userRequest.Name, userRequest.TradesHistories, existingAccountDetail.InstrumentStats);
                        var result = _eLTService.UpdateAccountDetail(existingAccountDetail);
                    }
                }
                if (insertAccountDetail.Any())
                {
                    unitOfWork.AccountDetailRepository.AddMultiple(insertAccountDetail);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private static void CalculateDailyNav(List<MT4UserRequest> userRequests)
        {
            try
            {
                List<DailyEquity> insertDailyEquityOfUser = new List<DailyEquity>();
                foreach (var userRequest in userRequests)
                {
                    if (userRequest.Login == 442842)
                    {
                    }
                    var existingtDailyEquities = _eLTService.getDailyEquitiesByAccountNumber(userRequest.Login, userRequest.OrganizationId);
                    if (existingtDailyEquities == null)
                    {
                        if (userRequest.TradesHistories.Count() > 0)
                        {
                            var equityByDates = new List<EquityByDate>();
                            var startDate = Dates.SecondsToDate(userRequest.TradesHistories[0].OpenTime).Date;
                            var beginingDate = startDate;
                            var endDate = startDate.AddDays(1);
                            var balance = new double();
                            var balanaceWithoutWithdrawn = new double();
                            var id = 1;
                            var previousDateNav = new double();
                            do
                            {
                                if (userRequest.Login == 441327)
                                {
                                    var b = "";
                                }
                                var startSeconds = (uint)(Int32)(startDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                                var endSeconds = (uint)(Int32)(endDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                                var buyTrades = userRequest.TradesHistories.Where(x => (x.OpenTime >= startSeconds && x.OpenTime < endSeconds) && x.Cmd == TradeCommand.Buy)
                                                                .Sum(x => x.Profit);
                                var sellTrades = userRequest.TradesHistories.Where(x => (x.OpenTime >= startSeconds && x.OpenTime < endSeconds) && x.Cmd == TradeCommand.Sell)
                                                                .Sum(x => x.Profit);
                                var balanaceTypeWithWithdrawn = userRequest.TradesHistories.Where(x => (x.OpenTime >= startSeconds && x.OpenTime < endSeconds)
                                                                && (x.Cmd == TradeCommand.Balance || x.Cmd == TradeCommand.Credit)).Sum(x => x.Profit);
                                var balanaceTypesWithoutWithdrawn = userRequest.TradesHistories.Where(x => (x.OpenTime >= startSeconds && x.OpenTime < endSeconds)
                                                                && (x.Cmd == TradeCommand.Balance || x.Cmd == TradeCommand.Credit) && x.Profit > 0).Sum(x => x.Profit);
                                balance = balance + balanaceTypeWithWithdrawn + buyTrades + sellTrades;
                                balanaceWithoutWithdrawn = balanaceWithoutWithdrawn + balanaceTypesWithoutWithdrawn + buyTrades + sellTrades;
                                if (balance == 0 && startDate == Dates.SecondsToDate(userRequest.TradesHistories[0].OpenTime).Date)
                                {
                                }
                                else
                                {
                                    var sharpeRatioReturn = new double();
                                    if (startDate != Dates.SecondsToDate(userRequest.TradesHistories[0].OpenTime).Date)
                                    {
                                        sharpeRatioReturn = previousDateNav == 0 ? 0 : (balance - previousDateNav) / previousDateNav;
                                        previousDateNav = balance;
                                    }
                                    else
                                    {
                                        sharpeRatioReturn = 0;
                                        previousDateNav = balance;
                                    }
                                    var equityByDate = new EquityByDate();
                                    equityByDate.date = startDate.ToString();
                                    equityByDate.Equity = Math.Round(balance, 2);
                                    equityByDate.EquityWithoutWithdrawn = Math.Round(balanaceWithoutWithdrawn, 2);
                                    equityByDate.id = id;
                                    equityByDate.SharpeRatioReturn = sharpeRatioReturn;
                                    equityByDates.Add(equityByDate);
                                }
                                startDate = startDate.AddDays(1);
                                endDate = endDate.AddDays(1);
                                id++;
                            } while (startDate < DateTime.Today);
                            DailyEquity dailyEquityOfUser = new DailyEquity();
                            dailyEquityOfUser.AccountNumber = userRequest.Login;
                            dailyEquityOfUser.EquityByDate = equityByDates;
                            dailyEquityOfUser.OrganizationId = userRequest.OrganizationId;
                            insertDailyEquityOfUser.Add(dailyEquityOfUser);
                        }
                    }
                    else
                    {
                        if (userRequest.Login == 456403 || userRequest.Login == 447927 || userRequest.Login == 440010)
                        {

                        }
                        if (userRequest.TradesHistories.Count() > 0)
                        {
                            var startDate = Convert.ToDateTime(existingtDailyEquities.EquityByDate[existingtDailyEquities.EquityByDate.Count - 1].date).AddDays(1);
                            var endDate = startDate.AddDays(1);
                            var beginingDate = startDate;
                            var balance = existingtDailyEquities.EquityByDate[existingtDailyEquities.EquityByDate.Count - 1].Equity;
                            var balanceWithoutWithdrawn = existingtDailyEquities.EquityByDate[existingtDailyEquities.EquityByDate.Count - 1].EquityWithoutWithdrawn;
                            var id = 1;
                            var previousDateNav = new double();
                            if (startDate < DateTime.Today)
                            {
                                do
                                {
                                    var startSeconds = (uint)(Int32)(startDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                                    var endSeconds = (uint)(Int32)(endDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                                    var buyTrades = userRequest.TradesHistories.Where(x => (x.OpenTime >= startSeconds && x.OpenTime < endSeconds) && x.Cmd == TradeCommand.Buy).Sum(x => x.Profit);
                                    var sellTrades = userRequest.TradesHistories.Where(x => (x.OpenTime >= startSeconds && x.OpenTime < endSeconds) && x.Cmd == TradeCommand.Sell).Sum(x => x.Profit);
                                    var balanaceTypeWithWithdrawn = userRequest.TradesHistories.Where(x => (x.OpenTime >= startSeconds && x.OpenTime < endSeconds)
                                                                && (x.Cmd == TradeCommand.Balance || x.Cmd == TradeCommand.Credit)).Sum(x => x.Profit);
                                    var balanaceTypesWithoutWithdrawn = userRequest.TradesHistories.Where(x => (x.OpenTime >= startSeconds && x.OpenTime < endSeconds)
                                                                && (x.Cmd == TradeCommand.Balance || x.Cmd == TradeCommand.Credit) && x.Profit > 0).Sum(x => x.Profit);
                                    balance = balance + balanaceTypeWithWithdrawn + buyTrades + sellTrades;
                                    balanceWithoutWithdrawn = balanceWithoutWithdrawn + balanaceTypesWithoutWithdrawn + buyTrades + sellTrades;
                                    var sharpeRatioReturn = new double();
                                    if (startDate != Dates.SecondsToDate(userRequest.TradesHistories[0].OpenTime).Date)
                                    {
                                        previousDateNav = existingtDailyEquities.EquityByDate[existingtDailyEquities.EquityByDate.Count - 1].Equity;
                                        sharpeRatioReturn = (balance - previousDateNav) / previousDateNav;
                                        previousDateNav = balance;
                                    }
                                    else
                                    {
                                        sharpeRatioReturn = 0;
                                        previousDateNav = balance;
                                    }
                                    var equityByDate = new EquityByDate();
                                    equityByDate.date = startDate.ToString();
                                    equityByDate.Equity = Math.Round(balance, 2);
                                    equityByDate.EquityWithoutWithdrawn = Math.Round(balanceWithoutWithdrawn, 2);
                                    equityByDate.id = id;
                                    equityByDate.SharpeRatioReturn = sharpeRatioReturn;
                                    existingtDailyEquities.EquityByDate.Add(equityByDate);
                                    startDate = startDate.AddDays(1);
                                    endDate = endDate.AddDays(1);
                                    id++;
                                } while (startDate < DateTime.Today);
                                var result = _eLTService.UpdateDailyEquity(existingtDailyEquities);
                            }
                        }
                    }
                }
                if (insertDailyEquityOfUser.Any())
                {
                    unitOfWork.DailyEquityRepository.AddMultiple(insertDailyEquityOfUser);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public static List<InstrumentStats> AddInstrumentStats(string name, List<TradeRecord> trades)
        {
            var instrumentName = trades.Where(x => x.Cmd == TradeCommand.Buy || x.Cmd == TradeCommand.Sell).Select(x => x.Symbol).Distinct().ToList();
            var AccountStatsList = new List<InstrumentStats>();
            foreach (TimeLineEnum timeline in Enum.GetValues(typeof(TimeLineEnum)))
            {
                foreach (var item in instrumentName.Select((Value, Index) => new { Value, Index }))
                {
                    InstrumentStats instrumentStats = new InstrumentStats();
                    instrumentStats.TimeLineId = (int)timeline;
                    instrumentStats.AccountStatsId = timeline;
                    instrumentStats.BuyRate = MathCalculations.GenerateRandomNo(3);
                    instrumentStats.CreatedBy = name;
                    //accountStats.CreatedOn = GenerateRandomDate();
                    instrumentStats.InstrumentId = item.Index + 1;
                    instrumentStats.InstrumentName = item.Value;
                    instrumentStats.Volume = InstrumentsCalculations.GetVolumeOfInstrumentByTimelineId((int)timeline, trades, item.Value);
                    instrumentStats.WINRate = MathCalculations.GenerateRandomNo(2);
                    instrumentStats.NAV = MathCalculations.GenerateRandomNo(2);
                    instrumentStats.ROI = MathCalculations.GenerateRandomNo(2);
                    instrumentStats.Profit = InstrumentsCalculations.GetProfitOfInstrumentByTimelineId((int)timeline, trades, item.Value);
                    instrumentStats.Loss = InstrumentsCalculations.GetLossOfInstrumentByTimelineId((int)timeline, trades, item.Value);
                    instrumentStats.Status = true;
                    if (instrumentStats.Volume > 0)
                    {
                        AccountStatsList.Add(instrumentStats);
                    }
                }
            }

            return AccountStatsList;
        }
        private static List<InstrumentStats> AddInstrumentStatsForExistingUsers(string name, List<TradeRecord> trades, List<InstrumentStats> Instruments)
        {

            try
            {
                var instrumentName = trades.Where(x => x.Cmd == TradeCommand.Buy || x.Cmd == TradeCommand.Sell).Select(x => x.Symbol).Distinct().ToList();
                var existingInstruments = Instruments.Select(x => x.InstrumentName).Distinct().ToList();
                var newInstruments = trades.Where(x => (x.Cmd == TradeCommand.Buy || x.Cmd == TradeCommand.Sell) && !existingInstruments.Contains(x.Symbol)).Select(x => x.Symbol).Distinct().ToList();
                var timelineIds = GetTimelinesForExistingUser();
                foreach (var timeline in timelineIds)
                {
                    foreach (var item in existingInstruments.Select((Value, Index) => new { Value, Index }))
                    {
                        InstrumentStats existinInstrument = Instruments.Where(x => x.InstrumentName == item.Value && x.TimeLineId == (int)timeline).FirstOrDefault();
                        if (existinInstrument != null)
                        {
                            existinInstrument.Volume = InstrumentsCalculations.GetVolumeOfInstrumentByTimelineId((int)timeline, trades, item.Value);
                            existinInstrument.Profit = InstrumentsCalculations.GetProfitOfInstrumentByTimelineId((int)timeline, trades, item.Value);
                            existinInstrument.Loss = InstrumentsCalculations.GetLossOfInstrumentByTimelineId((int)timeline, trades, item.Value);
                        }
                    }
                }
                if (newInstruments.Count > 0)
                {
                    foreach (TimeLineEnum timeline in Enum.GetValues(typeof(TimeLineEnum)))
                    {
                        foreach (var item in newInstruments.Select((Value, Index) => new { Value, Index }))
                        {
                            InstrumentStats instrumentStats = new InstrumentStats();
                            instrumentStats.TimeLineId = (int)timeline;
                            instrumentStats.AccountStatsId = timeline;
                            instrumentStats.BuyRate = MathCalculations.GenerateRandomNo(3);
                            instrumentStats.CreatedBy = name;
                            //accountStats.CreatedOn = GenerateRandomDate();
                            instrumentStats.InstrumentId = item.Index + 1;
                            instrumentStats.InstrumentName = item.Value;
                            instrumentStats.Volume = InstrumentsCalculations.GetVolumeOfInstrumentByTimelineId((int)timeline, trades, item.Value);
                            instrumentStats.WINRate = MathCalculations.GenerateRandomNo(2);
                            instrumentStats.NAV = MathCalculations.GenerateRandomNo(2);
                            instrumentStats.ROI = MathCalculations.GenerateRandomNo(2);
                            instrumentStats.Profit = InstrumentsCalculations.GetProfitOfInstrumentByTimelineId((int)timeline, trades, item.Value);
                            instrumentStats.Loss = InstrumentsCalculations.GetLossOfInstrumentByTimelineId((int)timeline, trades, item.Value);
                            instrumentStats.Status = true;
                            if (instrumentStats.Volume > 0)
                            {
                                Instruments.Add(instrumentStats);
                            }
                        }
                    }
                }
                return Instruments;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public static List<AccountStats> AddAcountStats(List<TradeRecord> trades, int accountNumber, int leverage, ObjectId datasourceId)
        {
            //try
            //{
            var dailyNavs = unitOfWork.DailyEquityRepository.GetAll().Where(x => x.AccountNumber == accountNumber).FirstOrDefault();
            var AccountStatsList = new List<AccountStats>();
            var datasource = _eLTService.getDatasourceById(datasourceId);
            foreach (TimeLineEnum timeline in Enum.GetValues(typeof(TimeLineEnum)))
            {
                if ((int)timeline == 45)
                {

                }
                AccountStats accountStats = new AccountStats();
                accountStats.AccountId = timeline;
                accountStats.StatringBalance = AccountStatsCalculations.GetStartingBalanceByTimeline((int)timeline, dailyNavs, trades);
                accountStats.Deposit = AccountStatsCalculations.GetDepositsByTimeline((int)timeline, dailyNavs, trades);
                accountStats.Withdrawn = AccountStatsCalculations.GetWithdrawnByTimeline((int)timeline, dailyNavs, trades);
                accountStats.ProfitLoss = AccountStatsCalculations.GetProfitLossByTimeline((int)timeline, dailyNavs, trades);
                accountStats.BestPL = AccountStatsCalculations.CalculateBestPLForTimeline((int)timeline, trades);
                accountStats.CreatedBy = accountNumber;
                accountStats.Leverage = leverage;
                accountStats.CreatedOn = DateTime.UtcNow.ToString();
                accountStats.DD = dailyNavs == null ? 0 : AccountStatsCalculations.CalculateDDForTimeline((int)timeline, dailyNavs, trades);
                accountStats.NAV = dailyNavs == null ? 0 : AccountStatsCalculations.CalculateNavForTimeline((int)timeline, dailyNavs); //Math.Round(dailyNavs.NAVByDate[dailyNavs.NAVByDate.Count - 1].NAV, 2);
                accountStats.ROI = dailyNavs == null ? 0 : AccountStatsCalculations.CalculateROIForTimeline((int)timeline, dailyNavs, trades);
                accountStats.SharpRatio = dailyNavs == null ? 0 : AccountStatsCalculations.CalculateSharpRatioForTimeline((int)timeline, dailyNavs, datasourceId, datasource, trades);
                accountStats.AvgTrade = dailyNavs == null ? 0 : AccountStatsCalculations.CalculateAvgTradesForTimeline((int)timeline, trades);
                accountStats.Status = true;
                accountStats.TimeLineId = (int)timeline;
                //accountStats.UpdatedBy = MathCalculations.GenerateRandomNo(2);
                //accountStats.UpdatedOn = GenerateRandomDate();
                accountStats.WINRate = AccountStatsCalculations.CalculateWINForTimeline((int)timeline, trades);
                accountStats.WorstPL = AccountStatsCalculations.CalculateWorstPLForTimeline((int)timeline, trades);
                //_unitOfWork.AccountStatsRepository.Add(accountStats);
                AccountStatsList.Add(accountStats);
            }

            return AccountStatsList;
            //}
            //catch (Exception ex)
            //{
            //    throw;
            //}
        }
        private static List<AccountStats> AddAcountStatsForExistingUsers(List<TradeRecord> trades, int accountNumber, int leverage, ObjectId datasourceId, List<AccountStats> accountStats)
        {
            //try
            //{
            var dailyNavs = unitOfWork.DailyEquityRepository.GetAll().Where(x => x.AccountNumber == accountNumber).FirstOrDefault();
            var datasource = _eLTService.getDatasourceById(datasourceId);
            var timelineIds = GetTimelinesForExistingUser();
            foreach (int timeline in timelineIds)
            {
                var existingAccountStats = accountStats.Where(x => x.TimeLineId == timeline).FirstOrDefault();
                existingAccountStats.StatringBalance = AccountStatsCalculations.GetStartingBalanceByTimeline((int)timeline, dailyNavs, trades);
                existingAccountStats.Deposit = AccountStatsCalculations.GetDepositsByTimeline((int)timeline, dailyNavs, trades);
                existingAccountStats.Withdrawn = AccountStatsCalculations.GetWithdrawnByTimeline((int)timeline, dailyNavs, trades);
                existingAccountStats.ProfitLoss = AccountStatsCalculations.GetProfitLossByTimeline((int)timeline, dailyNavs, trades);
                existingAccountStats.BestPL = AccountStatsCalculations.CalculateBestPLForTimeline((int)timeline, trades);
                existingAccountStats.CreatedOn = DateTime.UtcNow.ToString();
                existingAccountStats.DD = dailyNavs == null ? 0 : AccountStatsCalculations.CalculateDDForTimeline((int)timeline, dailyNavs, trades);
                existingAccountStats.NAV = dailyNavs == null ? 0 : AccountStatsCalculations.CalculateNavForTimeline((int)timeline, dailyNavs); //Math.Round(dailyNavs.NAVByDate[dailyNavs.NAVByDate.Count - 1].NAV, 2);
                existingAccountStats.ROI = dailyNavs == null ? 0 : AccountStatsCalculations.CalculateROIForTimeline((int)timeline, dailyNavs, trades);
                existingAccountStats.SharpRatio = dailyNavs == null ? 0 : AccountStatsCalculations.CalculateSharpRatioForTimeline((int)timeline, dailyNavs, datasourceId, datasource, trades);
                existingAccountStats.AvgTrade = dailyNavs == null ? 0 : AccountStatsCalculations.CalculateAvgTradesForTimeline((int)timeline, trades);
                existingAccountStats.WINRate = AccountStatsCalculations.CalculateWINForTimeline((int)timeline, trades);
                existingAccountStats.WorstPL = AccountStatsCalculations.CalculateWorstPLForTimeline((int)timeline, trades);
            }
            return accountStats;
            //}
            //catch (Exception ex)
            //{
            //    throw;
            //}
        }
        private static List<int> GetTimelinesForExistingUser()
        {
            var timelineIds = new List<int>();
            var today = DateTime.Now;
            var DaysInCurrentYear = (int)(today - new DateTime(today.Year, 1, 1)).TotalDays;
            var MonthsInCurrentYear = new DateTime(today.Year, 1, 1).Month;
            var WeeksInCurrentYear = (int)DaysInCurrentYear / 7;
            foreach (var timeline in Timelines.timelinesTillToday)
            {
                timelineIds.Add(timeline);
            }
            //foreach (var timeline in Timelines.MonthlyTimelines.Take(MonthsInCurrentYear))
            //{
            //    timelineIds.Add(timeline);
            //}
            if(MonthsInCurrentYear >= 1)
            {
                timelineIds.Add(Timelines.MonthlyTimelines[0]);
            }
            //foreach (var timeline in Timelines.WeeklyTimelines.Take(WeeksInCurrentYear))
            //{
            //    timelineIds.Add(timeline);
            //}
            if (WeeksInCurrentYear >=1 )
            {
                timelineIds.Add(Timelines.WeeklyTimelines[0]);
            }
            //foreach (var timeline in Timelines.DailyTimelines.Take(DaysInCurrentYear))
            //{
            //    timelineIds.Add(timeline);
            //}
            if (DaysInCurrentYear >= 1)
            {
                timelineIds.Add(Timelines.DailyTimelines[0]);
            }
            if ((int)(today - new DateTime(today.Year, 1, 1)).TotalDays == 0)
            {
                timelineIds.Add(1);
            }
            if (today.Month == 1 || today.Month == 7)
            {
                timelineIds.Add(2);
            }
            if (today.Month == 1 || today.Month == 4 || today.Month == 7 || today.Month == 10)
            {
                timelineIds.Add(3);
            }
            if ((int)(today - new DateTime(today.Year, today.Month, 1)).TotalDays == 0)
            {
                timelineIds.Add(4);
            }
            if (today.DayOfWeek == DayOfWeek.Monday)
            {
                timelineIds.Add(6);
            }
            return timelineIds;
        }
        #endregion
        private static void AddNotification(Guid organizationID, string notification)
        {
            var user = unitOfWork.UserRepository.GetAll().Where(u => u.OrganizationID == organizationID && u.RoleID == 2).Single();
            user.Notifications.Add(new Notification()
            {
                id = Guid.NewGuid(),
                Text = notification,
                Date = DateTime.Now,
                IsRead = false
            });
            unitOfWork.UserRepository.Update(user);
        }
    }
}
