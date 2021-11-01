using MongoDB.Driver;
using S2TAnalytics.Common.Enums;
using S2TAnalytics.Common.Helper;
using S2TAnalytics.Common.Utilities;
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
    public class AccountDetailService : IAccountDetailService
    {
        public readonly IUnitOfWork _unitOfWork;
        public AccountDetailService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public AnalyticsResponse GetInstrumentStats(Guid orgId, int timelineId, string[] userGroups, string instrument = "", string country = "", string city = "")
        {
            var response = new AnalyticsResponse();
            List<AccountDetail> accountDetail = new List<AccountDetail>();
            var topInstruments = _unitOfWork.AccountDetailRepository.GetAll().Where(m => m.OrganizationId == orgId && userGroups.Contains(m.UserGroup)).SelectMany(x => x.InstrumentStats).GroupBy(x => x.InstrumentName)
                                .Select(g => new
                                {
                                    Key = g.Key,
                                    Value = g.Sum(s => s.Volume),
                                }).OrderByDescending(x => x.Value).Select(x => x.Key).Take(5).ToList();
            if (string.IsNullOrEmpty(country) && string.IsNullOrEmpty(city))
            {

                //var aaccountDetail = _unitOfWork.AccountDetailRepository.GetAll().Where(m => m.OrganizationId == orgId && m.InstrumentStats.Any(x => x.TimeLineId == timelineId))
                //                   .SelectMany(x => x.InstrumentStats).OrderByDescending(x => x.Volume).ToList();
                accountDetail = _unitOfWork.AccountDetailRepository.GetAll().Where(m => m.OrganizationId == orgId && userGroups.Contains(m.UserGroup)).ToList();

                accountDetail.ForEach(x => x.InstrumentStats = x.InstrumentStats.Where(t => t.TimeLineId == timelineId && topInstruments.Contains(t.InstrumentName)).ToList());
            }
            else if (!string.IsNullOrEmpty(country) && string.IsNullOrEmpty(city))
            {
                accountDetail = _unitOfWork.AccountDetailRepository.GetAll().Where(m => m.OrganizationId == orgId && userGroups.Contains(m.UserGroup) && m.Country == country).ToList();
                accountDetail.ForEach(x => x.InstrumentStats = x.InstrumentStats.Where(t => t.TimeLineId == timelineId && t.InstrumentName == instrument && topInstruments.Contains(t.InstrumentName)).ToList());
            }
            if (!string.IsNullOrEmpty(country) && !string.IsNullOrEmpty(city))
            {
                accountDetail = _unitOfWork.AccountDetailRepository.GetAll().Where(m => m.OrganizationId == orgId && userGroups.Contains(m.UserGroup) && m.Country == country && m.City == city).ToList();
                accountDetail.ForEach(x => x.InstrumentStats = x.InstrumentStats.Where(t => t.TimeLineId == timelineId && t.InstrumentName == instrument && topInstruments.Contains(t.InstrumentName)).ToList());
            }


            var accountsData = new AccountDetailModel().ToAccountDetailWithInstrumentStatsModel(accountDetail);
            response.Data = accountsData.OrderByDescending(x => x.InstrumentStatsModel.Sum(y => y.Volume)).ToList();
            //response.Cities = accountDetail.Select(x => x.City).Distinct().ToList();

            //  response.Accounts = accountsData.Select(x => x.Name).Distinct();
            //response.Instruments = accountsData.SelectMany(x => x.InstrumentStatsModel).Select(x => x.InstrumentName).Distinct();
            response.Instruments = topInstruments;

            List<AccountDetailModel> aD = new List<AccountDetailModel>();
            foreach (string item in accountsData.Select(x => x.City).Distinct())
            {
                aD.Add(new AccountDetailModel() { City = item, Nav = accountsData.Where(z => z.City == item).SelectMany(n => n.InstrumentStatsModel).Sum(x => x.Volume) });
            }

            response.Cities = aD.OrderByDescending(n => n.Nav).Take(5).Select(x => x.City).Distinct();

            aD = new List<AccountDetailModel>();
            foreach (string item in accountsData.Select(x => x.Name).Distinct())
            {
                aD.Add(new AccountDetailModel() { Name = item, Nav = accountsData.Where(z => z.Name == item).SelectMany(n => n.InstrumentStatsModel).Sum(x => x.Volume) });
            }

            response.Accounts = aD.OrderByDescending(n => n.Nav).Take(5).Select(x => x.Name).Distinct();
            aD = new List<AccountDetailModel>();
            foreach (string item in accountDetail.Select(x => x.Country).Distinct())
            {
                aD.Add(new AccountDetailModel() { Country = item, Nav = accountsData.Where(z => z.Country == item).SelectMany(n => n.InstrumentStatsModel).Sum(x => x.Volume) });
            }

            response.Countries = aD.OrderByDescending(n => n.Nav).Take(5).Select(x => x.Country).Distinct();
            var timeLineIds = accountDetail.SelectMany(x => x.AccountStats).Select(x => x.TimeLineId).Distinct().ToList();
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
            //foreach (var timeLineId in timeLineIds.OrderBy(x => x).ToList())
            //{
            //    var timeline = new EnumHelper
            //    {
            //        stringValue = timeLineId.Encrypt(),
            //        DisplayName = ((TimeLineEnum)Enum.ToObject(typeof(TimeLineEnum), Convert.ToInt32(timeLineId))).GetEnumDisplayName()
            //    };
            //    timeLines.Add(timeline);
            //}
            response.Timelines = timeLines;
            response.WidgetId = Convert.ToInt32(EmbedWidgetEnum.InstrumentLocation).Encrypt();
            return response;
        }

        public AnalyticsResponse GetInstrumentStatsByGroup(Guid orgId, int timeLineId, string[] userGroups, string instrument = "", string userGroup = "")
        {
            var response = new AnalyticsResponse();
            // int timeLineID = 7;
            //int instrumentId = 0;
            //if (!string.IsNullOrEmpty(instrument))
            //{
            //    instrumentId = Convert.ToInt32(EnumHelper<InstrumentMasterEnum>.GetValueFromName(instrument));
            //}
            List<AccountDetail> accountDetail = new List<AccountDetail>();
            var topInstruments = _unitOfWork.AccountDetailRepository.GetAll().Where(m => m.OrganizationId == orgId && userGroups.Contains(m.UserGroup)).SelectMany(x => x.InstrumentStats).GroupBy(x => x.InstrumentName)
                               .Select(g => new
                               {
                                   Key = g.Key,
                                   Value = g.Sum(s => s.Volume),
                               }).OrderByDescending(x => x.Value).Select(x => x.Key).Take(5).ToList();
            if (string.IsNullOrEmpty(userGroup))
            {
                accountDetail = _unitOfWork.AccountDetailRepository.GetAll().Where(m => m.OrganizationId == orgId && userGroups.Contains(m.UserGroup)).ToList();
                accountDetail.ForEach(x => x.InstrumentStats = x.InstrumentStats.Where(t => t.TimeLineId == timeLineId && topInstruments.Contains(t.InstrumentName)).ToList());
            }
            else if (!string.IsNullOrEmpty(userGroup))
            {
                accountDetail = _unitOfWork.AccountDetailRepository.GetAll().Where(m => m.OrganizationId == orgId && userGroups.Contains(m.UserGroup) && m.UserGroup == userGroup).ToList();
                accountDetail.ForEach(x => x.InstrumentStats = x.InstrumentStats.Where(t => t.TimeLineId == timeLineId && t.InstrumentName == instrument && topInstruments.Contains(t.InstrumentName)).ToList());
            }


            var accountsData = new AccountDetailModel().ToAccountDetailWithInstrumentStatsModel(accountDetail);
            response.Data = accountsData;

            //response.Accounts = accountsData.Select(x => x.Name).Distinct();
            //response.Instruments = accountsData.SelectMany(x => x.InstrumentStatsModel).Select(x => x.InstrumentName).Distinct();
            response.Instruments = topInstruments;

            List<AccountDetailModel> aD = new List<AccountDetailModel>();
            foreach (string item in accountsData.Select(x => x.Name).Distinct())
            {
                aD.Add(new AccountDetailModel() { Name = item, Nav = accountsData.Where(z => z.Name == item).SelectMany(n => n.InstrumentStatsModel).Sum(x => x.Volume) });
            }

            response.Accounts = aD.OrderByDescending(n => n.Nav).Take(5).Select(x => x.Name).Distinct();
            aD = new List<AccountDetailModel>();
            foreach (string item in accountDetail.Select(x => x.UserGroup).Distinct())
            {
                aD.Add(new AccountDetailModel() { UserGroup = item, Nav = accountsData.Where(z => z.UserGroup == item).SelectMany(n => n.InstrumentStatsModel).Sum(x => x.Volume) });
            }

            response.Groups = aD.OrderByDescending(n => n.Nav).Take(5).Select(x => x.UserGroup).Distinct();
            var timeLineIds = accountDetail.SelectMany(x => x.AccountStats).Select(x => x.TimeLineId).Distinct().ToList();
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
            //foreach (var item in timeLineIds)
            //{
            //    var timeline = new EnumHelper
            //    {
            //        stringValue = item.Encrypt(),
            //        DisplayName = ((TimeLineEnum)Enum.ToObject(typeof(TimeLineEnum), Convert.ToInt32(item))).GetEnumDisplayName()
            //    };
            //    timeLines.Add(timeline);
            //}
            response.Timelines = timeLines;
            response.WidgetId = Convert.ToInt32(EmbedWidgetEnum.InstrumentGroup).Encrypt();
            return response;
        }

        public AnalyticsResponse GetComparisonData(Guid orgId, int timelineId, string[] userGroups, string country = "", string city = "", string selectedSeries = "")
        {
             var response = new AnalyticsResponse();
            List<AccountDetail> accountDetail = new List<AccountDetail>();
            if (string.IsNullOrEmpty(country))
            {
                accountDetail = _unitOfWork.AccountDetailRepository.GetAll().Where(m => m.OrganizationId == orgId && userGroups.Contains(m.UserGroup)).ToList();
                //accountDetail.ForEach(x => x.AccountStats = x.AccountStats.Where(t => t.TimeLineId == timelineId && topAccountStats.Contains(t.ROI)).ToList());
                //accountDetail = accountDetail.Where(x => x.AccountStats.Count > 0).Take(5).ToList();
            }
            else if (!string.IsNullOrEmpty(country) && string.IsNullOrEmpty(city))
            {
                if (selectedSeries == "ROI%")
                {
                    var topAccountStats = _unitOfWork.AccountDetailRepository.GetAll().Where(m => m.OrganizationId == orgId && userGroups.Contains(m.UserGroup) && m.Country == country)
                    .SelectMany(x => x.AccountStats).Where(x => x.TimeLineId == timelineId).OrderByDescending(x => x.ROI).Select(x => x.ROI).Take(5).ToList();
                    accountDetail = _unitOfWork.AccountDetailRepository.GetAll().Where(m => m.OrganizationId == orgId && userGroups.Contains(m.UserGroup) && m.Country == country).ToList();
                    accountDetail.ForEach(x => x.AccountStats = x.AccountStats.Where(t => t.TimeLineId == timelineId && topAccountStats.Contains(t.ROI)).ToList());
                    accountDetail = accountDetail.Where(x => x.AccountStats.Count > 0).Take(5).ToList();
                }
                if (selectedSeries == "MAX DD%")
                {
                    var topAccountStats = _unitOfWork.AccountDetailRepository.GetAll().Where(m => m.OrganizationId == orgId && userGroups.Contains(m.UserGroup) && m.Country == country)
                    .SelectMany(x => x.AccountStats).Where(x => x.TimeLineId == timelineId).OrderByDescending(x => x.DD).Select(x => x.DD).Take(5).ToList();
                    accountDetail = _unitOfWork.AccountDetailRepository.GetAll().Where(m => m.OrganizationId == orgId && userGroups.Contains(m.UserGroup) && m.Country == country).ToList();
                    accountDetail.ForEach(x => x.AccountStats = x.AccountStats.Where(t => t.TimeLineId == timelineId && topAccountStats.Contains(t.DD)).ToList());
                    accountDetail = accountDetail.Where(x => x.AccountStats.Count > 0).Take(5).ToList();
                }
                if (selectedSeries == "WIN%")
                {
                    var topAccountStats = _unitOfWork.AccountDetailRepository.GetAll().Where(m => m.OrganizationId == orgId && userGroups.Contains(m.UserGroup) && m.Country == country)
                    .SelectMany(x => x.AccountStats).Where(x => x.TimeLineId == timelineId).OrderByDescending(x => x.WINRate).Select(x => x.WINRate).Take(5).ToList();
                    accountDetail = _unitOfWork.AccountDetailRepository.GetAll().Where(m => m.OrganizationId == orgId && userGroups.Contains(m.UserGroup) && m.Country == country).ToList();
                    accountDetail.ForEach(x => x.AccountStats = x.AccountStats.Where(t => t.TimeLineId == timelineId && topAccountStats.Contains(t.WINRate)).ToList());
                    accountDetail = accountDetail.Where(x => x.AccountStats.Count > 0).Take(5).ToList();
                }

            }
            else if (!string.IsNullOrEmpty(country) && !string.IsNullOrEmpty(city))
            {
                if (selectedSeries == "ROI%")
                {
                    var topAccountStats = _unitOfWork.AccountDetailRepository.GetAll().Where(m => m.OrganizationId == orgId && userGroups.Contains(m.UserGroup) && m.Country == country && m.City == city)
                    .SelectMany(x => x.AccountStats).Where(x => x.TimeLineId == timelineId).OrderByDescending(x => x.ROI).Select(x => x.ROI).Take(5).ToList();
                    accountDetail = _unitOfWork.AccountDetailRepository.GetAll().Where(m => m.OrganizationId == orgId && userGroups.Contains(m.UserGroup) && m.Country == country && m.City == city).ToList();
                    accountDetail.ForEach(x => x.AccountStats = x.AccountStats.Where(t => t.TimeLineId == timelineId && topAccountStats.Contains(t.ROI)).ToList());
                    accountDetail = accountDetail.Where(x => x.AccountStats.Count > 0).Take(5).ToList();
                }
                if (selectedSeries == "MAX DD%")
                {
                    var topAccountStats = _unitOfWork.AccountDetailRepository.GetAll().Where(m => m.OrganizationId == orgId && userGroups.Contains(m.UserGroup) && m.Country == country && m.City == city)
                    .SelectMany(x => x.AccountStats).Where(x => x.TimeLineId == timelineId).OrderByDescending(x => x.DD).Select(x => x.DD).Take(5).ToList();
                    accountDetail = _unitOfWork.AccountDetailRepository.GetAll().Where(m => m.OrganizationId == orgId && userGroups.Contains(m.UserGroup) && m.Country == country && m.City == city).ToList();
                    accountDetail.ForEach(x => x.AccountStats = x.AccountStats.Where(t => t.TimeLineId == timelineId && topAccountStats.Contains(t.DD)).ToList());
                    accountDetail = accountDetail.Where(x => x.AccountStats.Count > 0).Take(5).ToList();
                }
                if (selectedSeries == "WIN%")
                {
                    var topAccountStats = _unitOfWork.AccountDetailRepository.GetAll().Where(m => m.OrganizationId == orgId && userGroups.Contains(m.UserGroup) && m.Country == country && m.City == city)
                    .SelectMany(x => x.AccountStats).Where(x => x.TimeLineId == timelineId).OrderByDescending(x => x.WINRate).Select(x => x.    WINRate).Take(5).ToList();
                    accountDetail = _unitOfWork.AccountDetailRepository.GetAll().Where(m => m.OrganizationId == orgId && userGroups.Contains(m.UserGroup) && m.Country == country && m.City == city).ToList();
                    accountDetail.ForEach(x => x.AccountStats = x.AccountStats.Where(t => t.TimeLineId == timelineId && topAccountStats.Contains(t.WINRate)).ToList());
                    accountDetail = accountDetail.Where(x => x.AccountStats.Count > 0).Take(5).ToList();
                }

            }

            var timeLineIds = accountDetail.SelectMany(x => x.AccountStats).Select(x => x.TimeLineId).Distinct().ToList();
            accountDetail.ForEach(x => x.AccountStats = x.AccountStats.Where(t => t.TimeLineId == timelineId).ToList());

            var accountsData = new AccountDetailModel().ToComparisonData(accountDetail);

            response.Data = accountsData;
            //response.Cities = accountDetail.Select(x => x.City).Distinct();

            // response.Accounts = accountDetail.Select(x => x.Name).Distinct();
            List<AccountDetailModel> aD = new List<AccountDetailModel>();
            foreach (string item in accountsData.Select(x => x.City).Distinct())
            {
                aD.Add(new AccountDetailModel() { City = item, Nav = Math.Round(accountsData.Where(z => z.City == item).SelectMany(n => n.AccountStats).Sum(x => x.NAV), 2) });
            }

            response.Cities = aD.OrderByDescending(n => n.Nav).Take(5).Select(x => x.City).Distinct();

            aD = new List<AccountDetailModel>();
            foreach (string item in accountsData.Select(x => x.Name).Distinct())
            {
                aD.Add(new AccountDetailModel() { Name = item, Nav = Math.Round(accountsData.Where(z => z.Name == item).SelectMany(n => n.AccountStats).Sum(x => x.NAV), 2) });
            }

            response.Accounts = accountDetail.OrderByDescending(n => n.Name).Take(5).Select(x => x.Name).Distinct();
            aD = new List<AccountDetailModel>();
            foreach (string item in accountDetail.Select(x => x.Country).Distinct())
            {
                aD.Add(new AccountDetailModel() { Country = item, Nav = Math.Round(accountsData.Where(z => z.Country == item).SelectMany(n => n.AccountStats).Sum(x => x.NAV), 2) });
            }

            response.Countries = aD.OrderByDescending(n => n.Nav).Take(5).Select(x => x.Country).Distinct();

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
            //foreach (var timeLineId in timeLineIds.OrderBy(x => x).ToList())
            //{
            //    var timeline = new EnumHelper
            //    {
            //        stringValue = timeLineId.Encrypt(),
            //        DisplayName = ((TimeLineEnum)Enum.ToObject(typeof(TimeLineEnum), Convert.ToInt32(timeLineId))).GetEnumDisplayName()
            //    };
            //    timeLines.Add(timeline);
            //}
            response.Timelines = timeLines;
            response.WidgetId = Convert.ToInt32(EmbedWidgetEnum.Comparison).Encrypt();
            return response;
        }

        public AnalyticsResponse GetNavMapData(Guid orgId, int timelineId, string[] userGroups, string userGroup = "", string country = "", string city = "")
        {
            var response = new AnalyticsResponse();
            // int timeLineID = 7;

            List<AccountDetail> accountDetail = new List<AccountDetail>();
            if (string.IsNullOrEmpty(country))
            {
                accountDetail = _unitOfWork.AccountDetailRepository.GetAll().Where(m => m.OrganizationId == orgId && userGroups.Contains(m.UserGroup)).ToList();
            }
            else if (!string.IsNullOrEmpty(country) && string.IsNullOrEmpty(city) && string.IsNullOrEmpty(userGroup))
            {
                accountDetail = _unitOfWork.AccountDetailRepository.GetAll().Where(m => m.OrganizationId == orgId && userGroups.Contains(m.UserGroup) && m.Country == country).ToList();
            }
            else if (!string.IsNullOrEmpty(country) && !string.IsNullOrEmpty(city) && string.IsNullOrEmpty(userGroup))
            {
                accountDetail = _unitOfWork.AccountDetailRepository.GetAll().Where(m => m.OrganizationId == orgId && userGroups.Contains(m.UserGroup) && m.Country == country && m.City == city).ToList();
            }
            else if (!string.IsNullOrEmpty(userGroup) && string.IsNullOrEmpty(country) && string.IsNullOrEmpty(city))
            {
                accountDetail = _unitOfWork.AccountDetailRepository.GetAll().Where(m => m.OrganizationId == orgId && userGroups.Contains(m.UserGroup) && m.UserGroup == userGroup).ToList();
            }
            else if (!string.IsNullOrEmpty(userGroup) && !string.IsNullOrEmpty(country) && string.IsNullOrEmpty(city))
            {
                accountDetail = _unitOfWork.AccountDetailRepository.GetAll().Where(m => m.OrganizationId == orgId && userGroups.Contains(m.UserGroup) && m.UserGroup == userGroup && m.Country == country).ToList();
            }
            else if (!string.IsNullOrEmpty(userGroup) && !string.IsNullOrEmpty(country) && !string.IsNullOrEmpty(city))
            {
                accountDetail = _unitOfWork.AccountDetailRepository.GetAll().Where(m => m.OrganizationId == orgId && userGroups.Contains(m.UserGroup) && m.UserGroup == userGroup && m.Country == country && m.City == city).Take(5).ToList();
            }
            var timeLineIds = accountDetail.SelectMany(x => x.AccountStats).Select(x => x.TimeLineId).Distinct().ToList();
            accountDetail.ForEach(x => x.AccountStats = x.AccountStats.Where(t => t.TimeLineId == timelineId).ToList());

            var accountsData = new AccountDetailModel().ToComparisonData(accountDetail);
            response.Data = accountsData;
            //response.Cities = accountDetail.Select(x => x.City).Distinct();
            // response.Countries = accountDetail.Select(x => x.Country).Distinct();
            // response.Accounts = accountDetail.Select(x => x.Name).Distinct();
            List<AccountDetailModel> aD = new List<AccountDetailModel>();
            foreach (string item in accountsData.Select(x => x.City).Distinct())
            {
                aD.Add(new AccountDetailModel() { City = item, Nav = accountsData.Where(z => z.City == item).SelectMany(n => n.AccountStats).Sum(x => x.NAV) });
            }

            response.Cities = aD.OrderByDescending(n => n.Nav).Take(5).Select(x => x.City).Distinct();

            aD = new List<AccountDetailModel>();
            foreach (string item in accountDetail.Select(x => x.Country).Distinct())
            {
                aD.Add(new AccountDetailModel() { Country = item, Nav = accountsData.Where(z => z.Country == item).SelectMany(n => n.AccountStats).Sum(x => x.NAV) });
            }

            response.Countries = aD.OrderByDescending(n => n.Nav).Take(5).Select(x => x.Country).Distinct();

            aD = new List<AccountDetailModel>();
            foreach (string item in accountsData.Select(x => x.Name).Distinct())
            {
                aD.Add(new AccountDetailModel() { Name = item, Nav = accountsData.Where(z => z.Name == item).SelectMany(n => n.AccountStats).Sum(x => x.NAV) });
            }

            response.Accounts = accountDetail.OrderByDescending(n => n.Name).Take(5).Select(x => x.Name).Distinct();
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
            //foreach (var timeLineId in timeLineIds.OrderBy(x => x).ToList())
            //{
            //    var timeline = new EnumHelper
            //    {
            //        stringValue = timeLineId.Encrypt(),
            //        DisplayName = ((TimeLineEnum)Enum.ToObject(typeof(TimeLineEnum), Convert.ToInt32(timeLineId))).GetEnumDisplayName()
            //    };
            //    timeLines.Add(timeline);
            //}
            response.Timelines = timeLines;
            response.WidgetId = Convert.ToInt32(EmbedWidgetEnum.Nav).Encrypt();
            return response;

            //var response = new AnalyticsResponse();
            //// int timeLineID = 7;

            //List<AccountDetail> accountDetail = new List<AccountDetail>();
            //if (string.IsNullOrEmpty(country))
            //{
            //    accountDetail = _unitOfWork.AccountDetailRepository.GetAll().Where(m => m.OrganizationId == orgId).ToList();
            //}
            //else if (!string.IsNullOrEmpty(country) && string.IsNullOrEmpty(city) && string.IsNullOrEmpty(userGroup))
            //{
            //    accountDetail = _unitOfWork.AccountDetailRepository.GetAll().Where(m => m.OrganizationId == orgId && m.Country == country).ToList();
            //}
            //else if (!string.IsNullOrEmpty(country) && !string.IsNullOrEmpty(city) && string.IsNullOrEmpty(userGroup))
            //{
            //    accountDetail = _unitOfWork.AccountDetailRepository.GetAll().Where(m => m.OrganizationId == orgId && m.Country == country && m.City == city).ToList();
            //}
            //else if (!string.IsNullOrEmpty(userGroup) && string.IsNullOrEmpty(country) && string.IsNullOrEmpty(city))
            //{
            //    accountDetail = _unitOfWork.AccountDetailRepository.GetAll().Where(m => m.OrganizationId == orgId && m.UserGroup == userGroup).ToList();
            //}
            //else if (!string.IsNullOrEmpty(userGroup) && !string.IsNullOrEmpty(country) && string.IsNullOrEmpty(city))
            //{
            //    accountDetail = _unitOfWork.AccountDetailRepository.GetAll().Where(m => m.OrganizationId == orgId && m.UserGroup == userGroup && m.Country == country).ToList();
            //}
            //else if (!string.IsNullOrEmpty(userGroup) && !string.IsNullOrEmpty(country) && !string.IsNullOrEmpty(city))
            //{
            //    accountDetail = _unitOfWork.AccountDetailRepository.GetAll().Where(m => m.OrganizationId == orgId && m.UserGroup == userGroup && m.Country == country && m.City == city).Take(5).ToList();
            //}
            //var timeLineIds = accountDetail.SelectMany(x => x.AccountStats).Select(x => x.TimeLineId).Distinct().ToList();
            //accountDetail.ForEach(x => x.AccountStats = x.AccountStats.Where(t => t.TimeLineId == timelineId).ToList());

            //var accountsData = new AccountDetailModel().ToComparisonData(accountDetail);
            //response.Data = accountsData;
            ////response.Cities = accountDetail.Select(x => x.City).Distinct();
            //// response.Countries = accountDetail.Select(x => x.Country).Distinct();
            //// response.Accounts = accountDetail.Select(x => x.Name).Distinct();
            //List<AccountDetailModel> aD = new List<AccountDetailModel>();
            //foreach (string item in accountsData.Select(x => x.City).Distinct())
            //{
            //    aD.Add(new AccountDetailModel() { City = item, Nav = accountsData.Where(z => z.City == item).SelectMany(n => n.AccountStats).Sum(x => x.NAV) });
            //}

            //response.Cities = aD.OrderByDescending(n => n.Nav).Select(x => x.City).Distinct();

            //aD = new List<AccountDetailModel>();
            //foreach (string item in accountDetail.Select(x => x.Country).Distinct())
            //{
            //    aD.Add(new AccountDetailModel() { Country = item, Nav = accountsData.Where(z => z.Country == item).SelectMany(n => n.AccountStats).Sum(x => x.NAV) });
            //}

            //response.Countries = aD.OrderByDescending(n => n.Nav).Select(x => x.Country).Distinct();

            //aD = new List<AccountDetailModel>();
            //foreach (string item in accountsData.Select(x => x.Name).Distinct())
            //{
            //    aD.Add(new AccountDetailModel() { Name = item, Nav = accountsData.Where(z => z.Name == item).SelectMany(n => n.AccountStats).Sum(x => x.NAV) });
            //}

            //response.Accounts = accountDetail.OrderByDescending(n => n.Name).Select(x => x.Name).Distinct();
            //var timeLines = new List<EnumHelper>();
            //foreach (var timeLineId in timeLineIds.OrderBy(x => x).ToList())
            //{
            //    var timeline = new EnumHelper
            //    {
            //        stringValue = timeLineId.Encrypt(),
            //        DisplayName = ((TimeLineEnum)Enum.ToObject(typeof(TimeLineEnum), Convert.ToInt32(timeLineId))).GetEnumDisplayName()
            //    };
            //    timeLines.Add(timeline);
            //}
            //response.Timelines = timeLines;
            //return response;
        }
    }
}






