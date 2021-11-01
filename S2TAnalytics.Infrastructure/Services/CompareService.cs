using MongoDB.Bson;
using S2TAnalytics.Common.Enums;
using S2TAnalytics.Common.Helper;
using S2TAnalytics.DAL.Interfaces;
using S2TAnalytics.DAL.Models;
using S2TAnalytics.Infrastructure.Interfaces;
using S2TAnalytics.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static S2TAnalytics.Common.Helper.Helper;

namespace S2TAnalytics.Infrastructure.Services
{
    public class CompareService : ICompareService
    {
        public readonly IUnitOfWork _unitOfWork;
        public CompareService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ServiceResponse GetCompareData(Guid OrganationId, ObjectId USerID)
        {
            var timeLines = new List<EnumHelper>();
            foreach (var tline in Timelines.MasterTimelines)
            {
                var timeline = new EnumHelper
                {
                    stringValue = tline.Encrypt(),
                    DisplayName = ((TimeLineEnum)Enum.ToObject(typeof(TimeLineEnum), tline)).GetEnumDisplayName()
                };
                timeLines.Add(timeline);
            }
            var performersModel_top5 = new List<PerformersModel>();
            int timelineId = timeLines[0].stringValue.Decrypt();
            performersModel_top5 = _unitOfWork.AccountDetailRepository.GetAll().Where(p => p.OrganizationId == OrganationId).ToList()
               .Select(p => new PerformersModel()
               {
                   AccountDetailId = p.Id.ToString(),
                   AccountNumber = p.AccountNumber,
                   AvgTrade = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().AvgTrade : 0,
                   ROI = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().ROI : 0,
                   DD = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().DD : 0,
                   BestPL = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().BestPL : "",
                   Leverage = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().Leverage : 0,
                   NAV = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().NAV : 0,
                   PerformerName = p.Name,
                   Balance = p.Balance,
                   WINRate = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().WINRate : 0,
                   SharpRatio = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().SharpRatio : 0,
               }).OrderByDescending(o => o.ROI).Take(5).ToList();

            var performersModel_pined = new List<PerformersModel>();
            var pinnedUsers = _unitOfWork.PinnedUsersRepository.GetAll().Where(x => x.UserId == USerID).SingleOrDefault();
            if (pinnedUsers != null)
            {
                performersModel_pined = _unitOfWork.AccountDetailRepository.GetAll().Where(p => p.OrganizationId == OrganationId
                        && pinnedUsers.AccountIds.Contains(p.Id)).ToList()
                        .Select(p => new PerformersModel()
                        {
                            AccountDetailId = p.Id.ToString(),
                            AccountNumber = p.AccountNumber,
                            AvgTrade = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().AvgTrade : 0,
                            ROI = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().ROI : 0,
                            DD = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().DD : 0,
                            BestPL = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().BestPL : "",
                            Leverage = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().Leverage : 0,
                            NAV = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().NAV : 0,
                            PerformerName = p.Name,
                            WINRate = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().WINRate : 0,
                            SharpRatio = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().SharpRatio : 0,
                        }).OrderByDescending(o => o.ROI).ToList();
            }
            var result = new ServiceResponse
            {
                MultipleData = new Dictionary<string, object>(){
                        { "top5performers", performersModel_top5 },
                        { "pinedusers", performersModel_pined },
                        { "timeLines", timeLines }
                    },
                Success = true
            };
            return result;
        }

        public ServiceResponse GetSingleAccount(string AccountNumber, int TimeLineId, Guid OrganationId)
        {
            var compareModel = _unitOfWork.AccountDetailRepository.GetAll().Where(p => p.AccountNumber == AccountNumber && p.OrganizationId == OrganationId).ToList()
                  .Select(p => new CompareModel()
                  {
                      Name = p.Name,
                      AccountDetailId = p.Id.ToString(),
                      AccountNumber = p.AccountNumber,
                      LastTradedDate = p.LastActiveOn,
                      ActiveSince = p.ActiveSince,
                      AvgTrade = p.AccountStats.Count(ast => ast.TimeLineId == TimeLineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == TimeLineId).First().AvgTrade : 0,
                      ROI = p.AccountStats.Count(ast => ast.TimeLineId == TimeLineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == TimeLineId).First().ROI : 0,
                      DD = p.AccountStats.Count(ast => ast.TimeLineId == TimeLineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == TimeLineId).First().DD : 0,
                      BestPL = p.AccountStats.Count(ast => ast.TimeLineId == TimeLineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == TimeLineId).First().BestPL : "",
                      WorstPL = p.AccountStats.Count(ast => ast.TimeLineId == TimeLineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == TimeLineId).First().WorstPL : "",
                      NAV = p.AccountStats.Count(ast => ast.TimeLineId == TimeLineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == TimeLineId).First().NAV : 0,
                      WINRate = p.AccountStats.Count(ast => ast.TimeLineId == TimeLineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == TimeLineId).First().WINRate : 0,
                      SharpRatio = p.AccountStats.Count(ast => ast.TimeLineId == TimeLineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == TimeLineId).First().SharpRatio : 0
                  }).FirstOrDefault();
            var accountDetail = _unitOfWork.AccountDetailRepository.GetAll().Where(x => x.AccountNumber == AccountNumber).FirstOrDefault();
            var performaceData = getAccountStatsFromAccountDetailAccordingToTimeLine(accountDetail, TimeLineId);
            var result = new ServiceResponse
            {
                MultipleData = new Dictionary<string, object>(){
                        { "CompareModel", compareModel },
                        { "accountDetail", accountDetail },
                        { "performaceData", performaceData },
                    },
                Success = true
            };
            return result;
        }
        public ServiceResponse GetSingleAccountByAccountID(string AccountId, int TimeLineId, Guid OrganationId)
        {
            var compareModel = _unitOfWork.AccountDetailRepository.GetAll().Where(p => p.Id == ObjectId.Parse(AccountId) && p.OrganizationId == OrganationId).ToList()
                  .Select(p => new CompareModel()
                  {
                      Name = p.Name,
                      AccountDetailId = p.Id.ToString(),
                      AccountNumber = p.AccountNumber,
                      LastTradedDate = p.LastActiveOn,
                      ActiveSince = p.ActiveSince,
                      AvgTrade = p.AccountStats.Count(ast => ast.TimeLineId == TimeLineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == TimeLineId).First().AvgTrade : 0,
                      ROI = p.AccountStats.Count(ast => ast.TimeLineId == TimeLineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == TimeLineId).First().ROI : 0,
                      DD = p.AccountStats.Count(ast => ast.TimeLineId == TimeLineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == TimeLineId).First().DD : 0,
                      BestPL = p.AccountStats.Count(ast => ast.TimeLineId == TimeLineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == TimeLineId).First().BestPL : "",
                      WorstPL = p.AccountStats.Count(ast => ast.TimeLineId == TimeLineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == TimeLineId).First().WorstPL : "",
                      NAV = p.AccountStats.Count(ast => ast.TimeLineId == TimeLineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == TimeLineId).First().NAV : 0,
                      WINRate = p.AccountStats.Count(ast => ast.TimeLineId == TimeLineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == TimeLineId).First().WINRate : 0,
                      SharpRatio = p.AccountStats.Count(ast => ast.TimeLineId == TimeLineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == TimeLineId).First().SharpRatio : 0
                  }).FirstOrDefault();
            var accountDetail = _unitOfWork.AccountDetailRepository.GetById(AccountId);
            var performaceData = getAccountStatsFromAccountDetailAccordingToTimeLine(accountDetail, TimeLineId);
            var result = new ServiceResponse
            {
                MultipleData = new Dictionary<string, object>(){
                        { "CompareModel", compareModel },
                        { "accountDetail", accountDetail },
                        { "performaceData", performaceData },
                    },
                Success = true
            };
            return result;
        }


        public Dictionary<string, Object> getAccountStatsFromAccountDetailAccordingToTimeLine(AccountDetail accountDetail, int timelineId)
        {

            var accountStats = new List<AccountStats>();
            var categories = new List<string>();
            if (timelineId == 1)
            {
                var ids = new List<int>() { 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18 };
                accountStats = accountDetail.AccountStats.Where(x => ids.Contains(x.TimeLineId)).ToList();
                categories = Enumerable.Range(0, 12).Select(i => DateTime.Now.AddMonths(i - 12)).Select(date => date.ToString("MMM-yy")).ToList();
            }
            else if (timelineId == 2)
            {
                var ids = new List<int>() { 13, 14, 15, 16, 17, 18 };
                accountStats = accountDetail.AccountStats.Where(x => ids.Contains(x.TimeLineId)).ToList();
                categories = Enumerable.Range(0, 6).Select(i => DateTime.Now.AddMonths(i - 6)).Select(date => date.ToString("MMM-yy")).ToList();

            }
            else if (timelineId == 3)
            {
                var ids = new List<int>() { 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30 };
                accountStats = accountDetail.AccountStats.Where(x => ids.Contains(x.TimeLineId)).ToList();
                var dayOfMonth = (new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).Date - DateTime.Now.Date).TotalDays - 1;
                categories = Enumerable.Range(0, 12).Select(i => DateTime.Now.AddDays(dayOfMonth).AddDays((i * 7) - (12 * 7))).Select(date => date.ToString("dd-MMM")).ToList();
            }
            else if (timelineId == 4)
            {
                var ids = new List<int>() { 27, 28, 29, 30 };
                accountStats = accountDetail.AccountStats.Where(x => ids.Contains(x.TimeLineId)).ToList();
                var dayOfMonth = (new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).Date - DateTime.Now.Date).TotalDays - 1;
                categories = Enumerable.Range(0, 4).Select(i => DateTime.Now.AddDays(dayOfMonth).AddDays((i * 7) - (4 * 7))).Select(date => date.ToString("dd-MMM")).ToList();

            }
            else if (timelineId == 5)
            {
                var ids = new List<int>() { 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45 };
                accountStats = accountDetail.AccountStats.Where(x => ids.Contains(x.TimeLineId)).ToList();
                var daysOfWeek = DayOfWeek.Monday - DateTime.Now.DayOfWeek;
                categories = Enumerable.Range(0, 14).Select(i => DateTime.Now.AddDays(daysOfWeek).AddDays(i - 14)).Select(date => date.ToString("dd-MMM")).ToList();
            }
            else if (timelineId == 6)
            {
                var ids = new List<int>() { 38, 39, 40, 41, 42, 43, 44, 45 };
                accountStats = accountDetail.AccountStats.Where(x => ids.Contains(x.TimeLineId)).ToList();
                var daysOfWeek = DayOfWeek.Monday - DateTime.Now.DayOfWeek;
                categories = Enumerable.Range(0, 7).Select(i => DateTime.Now.AddDays(daysOfWeek).AddDays(i - 7)).Select(date => date.ToString("dd-MMM")).ToList();
            }
            else if (timelineId == 45)
            {
                var ids = Timelines.MonthlyTimelines;
                accountStats = accountDetail.AccountStats.Where(x => ids.Contains(x.TimeLineId)).Zip(ids, (o, i) => new { o, i })
                               .OrderBy(x => x.i).Select(x => x.o).Reverse().ToList();
                categories = Enumerable.Range(0, Timelines.MonthlyTimelines.Count).Select(i => DateTime.Now.AddMonths(i - Timelines.MonthlyTimelines.Count))
                            .Select(date => date.ToString("MMM-yy")).ToList();
            }
            else if (timelineId == 46)
            {
                var day = DateTime.Now.Day;
                var DayOfWeek = DateTime.Now.DayOfWeek - System.DayOfWeek.Monday;
                var weeks = Convert.ToInt32((day - DayOfWeek) / 7);
                if (weeks != 0)
                {
                    var ids = Timelines.WeeklyTimelines.Take(weeks).ToList();
                    var a = accountDetail.AccountStats.Where(x => ids.Contains(x.TimeLineId)).ToList();
                    accountStats = accountDetail.AccountStats.Where(x => ids.Contains(x.TimeLineId)).Zip(ids, (o, i) => new { o, i })
                                   .OrderBy(x => x.i).Select(x => x.o).Reverse().ToList();
                    var dayOfMonth = (new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).Date - DateTime.Now.Date).TotalDays - 1;
                    categories = Enumerable.Range(0, weeks).Select(i => DateTime.Now.AddDays((i * 7) - (weeks * 7))).Select(date => date.ToString("dd-MMM")).ToList();
                }
                else
                {
                    var DayOfMonth = DateTime.Now.Day - new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).Day;
                    var ids = Timelines.DailyTimelines.Take(DayOfMonth).ToList();
                    accountStats = accountDetail.AccountStats.Where(x => ids.Contains(x.TimeLineId)).Zip(ids, (o, i) => new { o, i })
                                   .OrderBy(x => x.i).Select(x => x.o).Reverse().ToList();
                    categories = Enumerable.Range(0, DayOfMonth).Select(i => DateTime.Now.AddDays(i - DayOfMonth)).Select(date => date.ToString("dd-MMM")).ToList();
                }
            }
            else if (timelineId == 47)
            {
                var DayOfWeek = DateTime.Now.DayOfWeek - System.DayOfWeek.Monday;
                var ids = Timelines.DailyTimelines.Take(DayOfWeek).ToList();
                accountStats = accountDetail.AccountStats.Where(x => ids.Contains(x.TimeLineId)).Zip(ids, (o, i) => new { o, i })
                               .OrderBy(x => x.i).Select(x => x.o).Reverse().ToList();
                categories = Enumerable.Range(0, DayOfWeek).Select(i => DateTime.Now.AddDays(i - DayOfWeek)).Select(date => date.ToString("dd-MMM")).ToList();
            }
            else if (timelineId == 48)
            {
                var day = DateTime.Now.Day;
                var DayOfWeek = DateTime.Now.DayOfWeek - System.DayOfWeek.Monday;
                var monthFromQuater = DateTime.Now.Month % 3 == 0 ? 3 : DateTime.Now.Month % 3;
                var weeksInCurrentMonth = Convert.ToInt32((day - DayOfWeek) / 7);
                var weeks = ((monthFromQuater - 1) * 4) + weeksInCurrentMonth;
                var ids = Timelines.WeeklyTimelines.Take(weeks).ToList();
                accountStats = accountDetail.AccountStats.Where(x => ids.Contains(x.TimeLineId)).Zip(ids, (o, i) => new { o, i })
                               .OrderBy(x => x.i).Select(x => x.o).Reverse().ToList();
                var dayOfMonth = (new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).Date - DateTime.Now.Date).TotalDays - 1;
                categories = Enumerable.Range(0, weeks).Select(i => DateTime.Now.AddDays((i * 7) - (weeks * 7))).Select(date => date.ToString("dd-MMM")).ToList();
            }
            else if (timelineId == 49)
            {
                var month = DateTime.Now.Month;
                var ids = Timelines.MonthlyTimelines.Take(month).ToList();
                accountStats = accountDetail.AccountStats.Where(x => ids.Contains(x.TimeLineId)).Zip(ids, (o, i) => new { o, i })
                               .OrderBy(x => x.i).Select(x => x.o).Reverse().ToList();
                categories = Enumerable.Range(0, month).Select(i => new DateTime(DateTime.Now.Year, 1, 1).AddMonths(i))
                            .Select(date => date.ToString("MMM-yy")).ToList();
            }

            var result = new Dictionary<string, object>() {
                { "accountStats",  new AccountStatsModel().ToAccountDailyStatsModel(accountStats)},
                { "categories",  categories}
            };
            return result;
        }
    }
}
