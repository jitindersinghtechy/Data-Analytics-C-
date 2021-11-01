using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using S2TAnalytics.Common.Enums;
using S2TAnalytics.Common.Helper;
using S2TAnalytics.DAL.Interfaces;
using S2TAnalytics.DAL.Models;
using S2TAnalytics.Infrastructure.Interfaces;
using S2TAnalytics.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using static S2TAnalytics.Common.Helper.Helper;

namespace S2TAnalytics.Infrastructure.Services
{
    public class PerformersService : IPerformersService
    {
        public readonly IUnitOfWork _unitOfWork;
        public PerformersService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ServiceResponse GetAccountDetails(PageRecordModel pageRecordModel, string[] assignedUserGroups)
        {
            if (assignedUserGroups.Length == 0)
                return new ServiceResponse() { Success = false };
            double count = 0;
            double minWin = 0; double minROI = 0; double minDD = 0; double maxWin = 0; double maxROI = 0; double maxDD = 0;
            //var accountDetails = new List<AccountDetail>();
            var performersModel = new List<PerformersModel>();
            if (string.IsNullOrEmpty(pageRecordModel.SortBy))
                pageRecordModel.SortBy = "Name";
            var accountDetailList = _unitOfWork.AccountDetailRepository.GetAll().Where(x => x.OrganizationId == pageRecordModel.OrganizationID && pageRecordModel.DatasourceIDs.Contains(x.DataSourceId) && assignedUserGroups.Contains(x.UserGroup)).ToList();
            int timelineId = pageRecordModel.Filters.Single(x => x.Key == "TimeLineId").Value.Decrypt();



            if (accountDetailList.Count != 0)
            {
                minWin = _unitOfWork.AccountDetailRepository.GetAll().Where(x => x.OrganizationId == pageRecordModel.OrganizationID && pageRecordModel.DatasourceIDs.Contains(x.DataSourceId) && assignedUserGroups.Contains(x.UserGroup)).Select(p => new PerformersModel()
                {
                    WINRate = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().WINRate : 0,

                }).Min(m => m.WINRate);

                minROI = _unitOfWork.AccountDetailRepository.GetAll().Where(x => x.OrganizationId == pageRecordModel.OrganizationID && pageRecordModel.DatasourceIDs.Contains(x.DataSourceId) && assignedUserGroups.Contains(x.UserGroup)).Select(p => new PerformersModel()
                {
                    ROI = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().ROI : 0,

                }).Min(m => m.ROI);

                minDD = _unitOfWork.AccountDetailRepository.GetAll().Where(x => x.OrganizationId == pageRecordModel.OrganizationID && pageRecordModel.DatasourceIDs.Contains(x.DataSourceId) && assignedUserGroups.Contains(x.UserGroup)).Select(p => new PerformersModel()
                {
                    DD = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().DD : 0,

                }).Min(m => m.DD);

                maxWin = _unitOfWork.AccountDetailRepository.GetAll().Where(x => x.OrganizationId == pageRecordModel.OrganizationID && pageRecordModel.DatasourceIDs.Contains(x.DataSourceId) && assignedUserGroups.Contains(x.UserGroup)).Select(p => new PerformersModel()
                {
                    WINRate = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().WINRate : 0,

                }).Max(m => m.WINRate);

                maxROI = _unitOfWork.AccountDetailRepository.GetAll().Where(x => x.OrganizationId == pageRecordModel.OrganizationID && pageRecordModel.DatasourceIDs.Contains(x.DataSourceId) && assignedUserGroups.Contains(x.UserGroup)).Select(p => new PerformersModel()
                {
                    ROI = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().ROI : 0,

                }).Max(m => m.ROI);

                maxDD = _unitOfWork.AccountDetailRepository.GetAll().Where(x => x.OrganizationId == pageRecordModel.OrganizationID && pageRecordModel.DatasourceIDs.Contains(x.DataSourceId) && assignedUserGroups.Contains(x.UserGroup)).Select(p => new PerformersModel()
                {
                    DD = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().DD : 0,

                }).Max(m => m.DD);
            }
            //else
            //{

            //}
            if (pageRecordModel.Filters.Count > 0 && !string.IsNullOrEmpty(pageRecordModel.SortBy))
            {
                //List<FilterDefinition<AccountDetail>> filtersList = GetFilterDefinationFromFilters(pageRecordModel);
                //var filters = Builders<AccountDetail>.Filter.And(filtersList);
                //var Sort = Builders<AccountDetail>.Sort;
                //if (pageRecordModel.SortOrder == "asc")
                //    accountDetails = _unitOfWork.AccountDetailRepository.GetPagedRecords(pageRecordModel, Sort.Ascending(pageRecordModel.SortBy), filters).ToList();
                //else
                //    accountDetails = _unitOfWork.AccountDetailRepository.GetPagedRecords(pageRecordModel, Sort.Descending(pageRecordModel.SortBy), filters).ToList();
                //var sort = accountDetails.OrderBy(x => x.AccountStats.OrderBy(a => a.ROI)).ToList();


                //var country = "";
                var country = new List<string>();
                //var usergroup = "";
                var usergroup = new List<string>();
                var search = "";
                foreach (var filter in pageRecordModel.Filters)
                {
                    //if (filter.Key == "Country")
                    //    country = pageRecordModel.Filters.Single(x => x.Key == "Country").Value;
                    //if (filter.Key == "UserGroup")
                    //    usergroup = pageRecordModel.Filters.Single(x => x.Key == "UserGroup").Value;
                    if (filter.Key == "WINRateMin")
                        minWin = Convert.ToDouble(pageRecordModel.Filters.Single(x => x.Key == "WINRateMin").Value);
                    if (filter.Key == "WINRateMax")
                        maxWin = Convert.ToDouble(pageRecordModel.Filters.Single(x => x.Key == "WINRateMax").Value);
                    if (filter.Key == "ROIMax")
                        maxROI = Convert.ToDouble(pageRecordModel.Filters.Single(x => x.Key == "ROIMax").Value);
                    if (filter.Key == "ROIMin")
                        minROI = Convert.ToDouble(pageRecordModel.Filters.Single(x => x.Key == "ROIMin").Value);
                    if (filter.Key == "DDMax")
                        maxDD = Convert.ToDouble(pageRecordModel.Filters.Single(x => x.Key == "DDMax").Value);
                    if (filter.Key == "DDMin")
                        minDD = Convert.ToDouble(pageRecordModel.Filters.Single(x => x.Key == "DDMin").Value);
                    if (filter.Key == "Search")
                        search = pageRecordModel.Filters.Single(x => x.Key == "Search").Value;
                }
                foreach (var filter in pageRecordModel.MultipleListFilter)
                {
                    if (filter.Key == "Countries")
                        country = pageRecordModel.MultipleListFilter.Single(x => x.Key == "Countries").Value.ToList();
                    if (filter.Key == "UserGroups")
                        usergroup = pageRecordModel.MultipleListFilter.Single(x => x.Key == "UserGroups").Value.ToList();
                }
                var expression = GetExpression(pageRecordModel.SortBy);
                IQueryable<PerformersModel> accountsModel;
                //if (!string.IsNullOrEmpty(country) && !string.IsNullOrEmpty(usergroup))
                if (country.Any() && usergroup.Any())
                {
                    //accountsModel = _unitOfWork.AccountDetailRepository.GetAll().Where(x => x.OrganizationId == pageRecordModel.OrganizationID &&  x.IsExclude==false  && x.Country == country && x.UserGroup == usergroup && (x.Name.Contains(search) || x.AccountNumber.Contains(search)))
                    accountsModel = _unitOfWork.AccountDetailRepository.GetAll().Where(x => x.OrganizationId == pageRecordModel.OrganizationID && assignedUserGroups.Contains(x.UserGroup) && x.IsExclude == false && country.Contains(x.Country) && usergroup.Contains(x.UserGroup)
                        && (x.Name.Contains(search) || x.AccountNumber.Contains(search)) && pageRecordModel.DatasourceIDs.Contains(x.DataSourceId))
                        .Select(p => new PerformersModel()
                        {
                            AccountDetailIdObjectId = p.Id,
                            AccountNumber = p.AccountNumber,
                            ROI = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().ROI : 0,
                            DD = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().DD : 0,
                            BestPL = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().BestPL : "",
                            Leverage = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().Leverage : 0,
                            NAV = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().NAV : 0,
                            PerformerName = p.Name,
                            Balance = p.Balance,
                            Location = p.Country,
                            UserGroup = p.UserGroup,
                            WINRate = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().WINRate : 0,
                            SharpRatio = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().SharpRatio : 0,
                        }).Where(w => w.WINRate <= maxWin && w.WINRate >= minWin && w.DD <= maxDD && w.DD >= minDD && w.ROI <= maxROI && w.ROI >= minROI);
                }

                //else if (!string.IsNullOrEmpty(usergroup))
                else if (usergroup.Any())
                {
                    //accountsModel = _unitOfWork.AccountDetailRepository.GetAll().Where(x => x.OrganizationId == pageRecordModel.OrganizationID && x.IsExclude == false &&  x.UserGroup == usergroup && (x.Name.Contains(search) || x.AccountNumber.Contains(search))).Select(p => new PerformersModel()
                    accountsModel = _unitOfWork.AccountDetailRepository.GetAll().Where(x => x.OrganizationId == pageRecordModel.OrganizationID && assignedUserGroups.Contains(x.UserGroup) && x.IsExclude == false &&
                        usergroup.Contains(x.UserGroup) && (x.Name.Contains(search) || x.AccountNumber.Contains(search)) && pageRecordModel.DatasourceIDs.Contains(x.DataSourceId))
                        .Select(p => new PerformersModel()
                        {
                            AccountDetailIdObjectId = p.Id,
                            AccountNumber = p.AccountNumber,
                            ROI = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().ROI : 0,
                            DD = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().DD : 0,
                            BestPL = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().BestPL : "",
                            Leverage = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().Leverage : 0,
                            NAV = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().NAV : 0,
                            PerformerName = p.Name,
                            Balance = p.Balance,
                            Location = p.Country,
                            UserGroup = p.UserGroup,
                            WINRate = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().WINRate : 0,
                            SharpRatio = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().SharpRatio : 0,
                        }).Where(w => w.WINRate <= maxWin && w.WINRate >= minWin && w.DD <= maxDD && w.DD >= minDD && w.ROI <= maxROI && w.ROI >= minROI);
                }

                //else if (!string.IsNullOrEmpty(country))
                else if (country.Any())
                {
                    //accountsModel = _unitOfWork.AccountDetailRepository.GetAll().Where(x => x.OrganizationId == pageRecordModel.OrganizationID && x.IsExclude == false  && x.Country == country && (x.Name.Contains(search) || x.AccountNumber.Contains(search))).Select(p => new PerformersModel()
                    accountsModel = _unitOfWork.AccountDetailRepository.GetAll().Where(x => x.OrganizationId == pageRecordModel.OrganizationID && assignedUserGroups.Contains(x.UserGroup) && x.IsExclude == false
                        && country.Contains(x.Country) && (x.Name.Contains(search) || x.AccountNumber.Contains(search))
                        && pageRecordModel.DatasourceIDs.Contains(x.DataSourceId)).Select(p => new PerformersModel()
                        {
                            AccountDetailIdObjectId = p.Id,

                            AccountNumber = p.AccountNumber,
                            ROI = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().ROI : 0,
                            DD = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().DD : 0,
                            BestPL = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().BestPL : "",
                            Leverage = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().Leverage : 0,
                            NAV = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().NAV : 0,
                            PerformerName = p.Name,
                            Balance = p.Balance,
                            Location = p.Country,
                            UserGroup = p.UserGroup,
                            WINRate = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().WINRate : 0,
                            SharpRatio = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().SharpRatio : 0,
                        }).Where(w => w.WINRate <= maxWin && w.WINRate >= minWin && w.DD <= maxDD && w.DD >= minDD && w.ROI <= maxROI && w.ROI >= minROI);
                }

                else if (!string.IsNullOrEmpty(search))
                {
                    accountsModel = _unitOfWork.AccountDetailRepository.GetAll().Where(x => x.OrganizationId == pageRecordModel.OrganizationID && assignedUserGroups.Contains(x.UserGroup) && x.IsExclude == false
                            && (x.Name.ToLower().Contains(search) || x.AccountNumber.ToLower().Contains(search)) && pageRecordModel.DatasourceIDs.Contains(x.DataSourceId)).Select(p => new PerformersModel()
                            {
                                AccountDetailIdObjectId = p.Id,
                                AccountNumber = p.AccountNumber,
                                ROI = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().ROI : 0,
                                DD = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().DD : 0,
                                BestPL = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().BestPL : "",
                                Leverage = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().Leverage : 0,
                                NAV = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().NAV : 0,
                                PerformerName = p.Name,
                                Balance = p.Balance,
                                Location = p.Country,
                                UserGroup = p.UserGroup,
                                WINRate = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().WINRate : 0,
                                SharpRatio = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().SharpRatio : 0,
                            }).Where(w => w.WINRate <= maxWin && w.WINRate >= minWin && w.DD <= maxDD && w.DD >= minDD && w.ROI <= maxROI && w.ROI >= minROI);
                }
                else
                {
                    accountsModel = _unitOfWork.AccountDetailRepository.GetAll().Where(x => x.OrganizationId == pageRecordModel.OrganizationID && assignedUserGroups.Contains(x.UserGroup) && x.IsExclude == false
                        && pageRecordModel.DatasourceIDs.Contains(x.DataSourceId)).Select(p => new PerformersModel()
                        {
                            AccountDetailIdObjectId = p.Id,

                            AccountNumber = p.AccountNumber,
                            ROI = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().ROI : 0,
                            DD = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().DD : 0,
                            BestPL = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().BestPL : "",
                            Leverage = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().Leverage : 0,
                            NAV = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().NAV : 0,
                            PerformerName = p.Name,
                            Balance = p.Balance,
                            Location = p.Country,
                            UserGroup = p.UserGroup,
                            WINRate = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().WINRate : 0,
                            SharpRatio = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().SharpRatio : 0,
                        }).Where(w => w.WINRate <= maxWin && w.WINRate >= minWin && w.DD <= maxDD && w.DD >= minDD && w.ROI <= maxROI && w.ROI >= minROI);
                }
                count = accountsModel.Count();
                if (pageRecordModel.SortOrder == "asc")
                    performersModel = accountsModel.OrderBy(expression).Skip((pageRecordModel.PageNumber - 1) * pageRecordModel.PageSize).Take(pageRecordModel.PageSize).ToList();
                else
                    performersModel = accountsModel.OrderByDescending(expression).Skip((pageRecordModel.PageNumber - 1) * pageRecordModel.PageSize).Take(pageRecordModel.PageSize).ToList();
                performersModel.ForEach(f => f.AccountDetailId = f.AccountDetailIdObjectId.ToString());
            }
            //else if (pageRecordModel.Filters.Count > 0 && string.IsNullOrEmpty(pageRecordModel.SortBy))
            //{
            //    List<FilterDefinition<AccountDetail>> filtersList = GetFilterDefinationFromFilters(pageRecordModel);
            //    var filters = Builders<AccountDetail>.Filter.And(filtersList);
            //    accountDetails = _unitOfWork.AccountDetailRepository.GetPagedRecords(pageRecordModel, null, filters).ToList();
            //}
            //else if (pageRecordModel.Filters.Count == 0 && !string.IsNullOrEmpty(pageRecordModel.SortBy))
            //{
            //    var Sort = Builders<AccountDetail>.Sort;
            //    if (pageRecordModel.SortOrder == "asc")
            //        accountDetails = _unitOfWork.AccountDetailRepository.GetPagedRecords(pageRecordModel, Sort.Ascending(pageRecordModel.SortBy), null).ToList();
            //    else
            //        accountDetails = _unitOfWork.AccountDetailRepository.GetPagedRecords(pageRecordModel, Sort.Descending(pageRecordModel.SortBy), null).ToList();
            //    var sort = accountDetails.OrderBy(x => x.AccountStats.OrderBy(a => a.ROI)).ToList();
            //}

            var accountDetails = _unitOfWork.AccountDetailRepository.GetAll().Where(x => x.OrganizationId == pageRecordModel.OrganizationID && assignedUserGroups.Contains(x.UserGroup) && pageRecordModel.DatasourceIDs.Contains(x.DataSourceId)).ToList();
            var countries = accountDetails.Select(x => x.Country).Distinct().ToList();
            var userGroups = accountDetails.Select(x => x.UserGroup).Distinct().ToList();
            var timeLines = GetTimeLines(accountDetails);

            var result = new ServiceResponse
            {
                MultipleData = new Dictionary<string, object>(){
                        { "accountDetails", performersModel },
                        { "countries", countries },
                        { "userGroups" , userGroups },
                        { "timeLines", timeLines },
                        { "minROI", minROI },
                        { "maxROI", maxROI },
                        { "minWIN", minWin },
                        { "maxWIN", maxWin },
                        { "minDD", minDD },
                        { "maxDD", maxDD },
                        { "count", count },
                    },

                Success = true
            };
            return result;
        }

        private Expression<Func<PerformersModel, object>> GetExpression(string sortBy)
        {
            switch (sortBy)
            {
                case "Name":
                    return x => x.PerformerName;
                case "AccountNumber":
                    return x => x.AccountNumber;
                case "Balance":
                    return x => x.Balance;
                case "ROI":
                    return x => x.ROI;
                case "WIN":
                    return x => x.WINRate;
                case "DD":
                    return x => x.DD;
                case "Leverage":
                    return x => x.Leverage;
                case "Country":
                    return x => x.Location;
                case "UserGroup":
                    return x => x.Location;
                case "NAV":
                    return x => x.NAV;

                default:
                    return x => x.WINRate;
            }
        }

        public int GetAccountDetailsCount(PageRecordModel pageRecordModel, string[] assignedUserGroups)
        {
            List<FilterDefinition<AccountDetail>> filtersList = GetFilterDefinationFromFilters(pageRecordModel, assignedUserGroups);
            var filters = Builders<AccountDetail>.Filter.And(filtersList);
            var count = _unitOfWork.AccountDetailRepository.GetTotalRecordsCount(filters);
            return count;
        }

        public ServiceResponse GetTop5Performers(PageRecordModel pageRecordModel, string[] assignedUserGroups)
        {
            //var accountDetails = new List<AccountDetail>();
            //List<FilterDefinition<AccountDetail>> filtersList = GetFilterDefinationFromFilters(pageRecordModel);
            //var filters = Builders<AccountDetail>.Filter.And(filtersList);
            //  accountDetails = _unitOfWork.AccountDetailRepository.GetByFilters(filters).Take(5).ToList();

            int timelineId = pageRecordModel.Filters.Where(f => f.Key == "TimeLineId").Single().Value.Decrypt();
            var sortBy = pageRecordModel.Filters.Where(f => f.Key == "sortPerformersBy").Single().Value;
            var performersModel = new List<PerformersModel>();
            IEnumerable<PerformersModel> EnumerableperformersModel;
            if (pageRecordModel.ListFilter != null && pageRecordModel.ListFilter.Count > 0)
            {
                if (pageRecordModel.Filters.ContainsKey("Country") && !string.IsNullOrEmpty(pageRecordModel.Filters.Where(f => f.Key == "Country").Single().Value))
                {
                    var country = pageRecordModel.Filters.Where(f => f.Key == "Country").Single().Value;
                    //EnumerableperformersModel = _unitOfWork.AccountDetailRepository.GetAll().Where(whereExpression).ToList();
                    EnumerableperformersModel = _unitOfWork.AccountDetailRepository.GetAll().Where(p => p.OrganizationId == pageRecordModel.OrganizationID && assignedUserGroups.Contains(p.UserGroup)
                        && p.Country == country && pageRecordModel.ListFilter.Contains(p.UserGroup) && p.IsExclude == false
                        && pageRecordModel.DatasourceIDs.Contains(p.DataSourceId)).ToList().Select(p => new PerformersModel()
                        {
                            AccountDetailId = p.Id.ToString(),
                            AccountNumber = p.AccountNumber,
                            ROI = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().ROI : 0,
                            DD = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().DD : 0,
                            BestPL = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().BestPL : "",
                            Leverage = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().Leverage : 0,
                            NAV = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().NAV : 0,
                            PerformerName = p.Name,
                            WINRate = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().WINRate : 0,
                            SharpRatio = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().SharpRatio : 0,
                            UserGroup = p.UserGroup
                        });//.OrderByDescending(o => o.ROI).Take(5).ToList();
                }
                else
                    EnumerableperformersModel = _unitOfWork.AccountDetailRepository.GetAll().Where(p => p.OrganizationId == pageRecordModel.OrganizationID && assignedUserGroups.Contains(p.UserGroup)
                        && pageRecordModel.ListFilter.Contains(p.UserGroup) && p.IsExclude == false && pageRecordModel.DatasourceIDs.Contains(p.DataSourceId)).ToList()
                       .Select(p => new PerformersModel()
                       {
                           AccountDetailId = p.Id.ToString(),
                           AccountNumber = p.AccountNumber,
                           ROI = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().ROI : 0,
                           DD = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().DD : 0,
                           BestPL = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().BestPL : "",
                           Leverage = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().Leverage : 0,
                           NAV = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().NAV : 0,
                           PerformerName = p.Name,
                           WINRate = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().WINRate : 0,
                           SharpRatio = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().SharpRatio : 0,
                           UserGroup = p.UserGroup
                       });//.OrderByDescending(o => o.ROI).Take(5).ToList();
            }
            else
            {


                if (pageRecordModel.Filters.ContainsKey("Country") && !string.IsNullOrEmpty(pageRecordModel.Filters.Where(f => f.Key == "Country").Single().Value))
                {
                    var country = pageRecordModel.Filters.Where(f => f.Key == "Country").Single().Value;
                    //EnumerableperformersModel = _unitOfWork.AccountDetailRepository.GetAll().Where(whereExpression).ToList();
                    EnumerableperformersModel = _unitOfWork.AccountDetailRepository.GetAll().Where(p => p.OrganizationId == pageRecordModel.OrganizationID && assignedUserGroups.Contains(p.UserGroup)
                    && p.Country == country && p.IsExclude == false && pageRecordModel.DatasourceIDs.Contains(p.DataSourceId)).ToList()
                       .Select(p => new PerformersModel()
                       {
                           AccountDetailId = p.Id.ToString(),
                           AccountNumber = p.AccountNumber,
                           ROI = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().ROI : 0,
                           DD = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().DD : 0,
                           BestPL = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().BestPL : "",
                           Leverage = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().Leverage : 0,
                           NAV = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().NAV : 0,
                           PerformerName = p.Name,
                           WINRate = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().WINRate : 0,
                           SharpRatio = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().SharpRatio : 0,
                           UserGroup = p.UserGroup
                       });//.OrderByDescending(o => o.ROI).Take(5).ToList();
                }
                else
                    EnumerableperformersModel = _unitOfWork.AccountDetailRepository.GetAll().Where(p => p.OrganizationId == pageRecordModel.OrganizationID && assignedUserGroups.Contains(p.UserGroup)
                    && p.IsExclude == false && pageRecordModel.DatasourceIDs.Contains(p.DataSourceId)).ToList()
                       .Select(p => new PerformersModel()
                       {
                           AccountDetailId = p.Id.ToString(),
                           AccountNumber = p.AccountNumber,
                           ROI = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().ROI : 0,
                           DD = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().DD : 0,
                           BestPL = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().BestPL : "",
                           Leverage = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().Leverage : 0,
                           NAV = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().NAV : 0,
                           PerformerName = p.Name,
                           WINRate = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().WINRate : 0,
                           SharpRatio = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().SharpRatio : 0,
                           UserGroup = p.UserGroup
                       });//.OrderByDescending(o => o.ROI).Take(5).ToList();
            }

            if (sortBy == "maxDD")
            {
                performersModel = EnumerableperformersModel.OrderByDescending(o => o.DD).Take(5).ToList();
            }
            else if (sortBy == "winRate")
            {
                performersModel = EnumerableperformersModel.OrderByDescending(o => o.WINRate).Take(5).ToList();
            }
            else if (sortBy == "sharpRatio")
            {
                performersModel = EnumerableperformersModel.OrderByDescending(o => o.SharpRatio).Take(5).ToList();
            }
            else
            {
                performersModel = EnumerableperformersModel.OrderByDescending(o => o.ROI).Take(5).ToList();
            }



            //var Sort = Builders<AccountDetail>.Sort;
            //accountDetails = _unitOfWork.AccountDetailRepository.GetPagedRecords(pageRecordModel, Sort.Ascending("AccountStats.ROI"), filters);

            //var accountDetailsModel = new AccountDetailModel().ToAccountDetailModel(accountDetails);
            var countries = _unitOfWork.AccountDetailRepository.GetAll().Where(p => p.OrganizationId == pageRecordModel.OrganizationID && assignedUserGroups.Contains(p.UserGroup) && p.IsExclude == false && pageRecordModel.DatasourceIDs.Contains(p.DataSourceId))
                .Select(x => x.Country).Distinct().ToList();

            var timeLineIds = _unitOfWork.AccountDetailRepository.GetAll().Where(p => p.OrganizationId == pageRecordModel.OrganizationID && assignedUserGroups.Contains(p.UserGroup) && p.IsExclude == false && pageRecordModel.DatasourceIDs.Contains(p.DataSourceId))
                .SelectMany(x => x.AccountStats).Select(x => x.TimeLineId).Distinct().ToList();
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
            var pinnedUsers = _unitOfWork.PinnedUsersRepository.GetAll().Where(x => x.UserId == pageRecordModel.UserID).FirstOrDefault();
            var userGroups = performersModel.Select(x => x.UserGroup).Distinct().ToList();
            var riskFreeRate = "";
            if (pageRecordModel.DatasourceIDs.Count > 1)
            {
                riskFreeRate = "N/A";
                performersModel.ForEach(x => x.SharpRatio = null);
            }
            else if (pageRecordModel.DatasourceIDs.Count != 0)
            {
                var datasourceId = pageRecordModel.DatasourceIDs.FirstOrDefault();
                var datasource = _unitOfWork.DatasourceRepository.GetAll().Where(x => x.Id == datasourceId).FirstOrDefault();
                riskFreeRate = datasource.RateOfReturns.Where(x => x.TimelineId == timelineId).FirstOrDefault().RateOfReturn.ToString();
            }

            var result = new ServiceResponse
            {
                MultipleData = new Dictionary<string, object>(){
                        { "performers", performersModel },
                        { "countries", countries },
                        { "timeLines", timeLines },
                        { "pinnedUsers", pinnedUsers },
                        { "userGroups", userGroups },
                        { "WidgetId", Convert.ToInt32(EmbedWidgetEnum.TopPerformersAndPinnedUsers).Encrypt()},
                        { "riskFreeRate", riskFreeRate}
                },
                Success = true
            };
            return result;
        }

        public ServiceResponse UpdatePinnedUsers(List<string> selectedAccountDetailsIds, Guid organizationID, ObjectId userID)
        {
            //var userid = "1";
            var pinnedUsers = _unitOfWork.PinnedUsersRepository.GetAll().Where(x => x.UserId == userID).SingleOrDefault();
            //var pinnedUsers = _unitOfWork.PinnedUsersRepository.GetAll().Where(x => x.UserId == ObjectId.Parse(userid)).SingleOrDefault();
            var accountIds = pinnedUsers == null ? new List<ObjectId>() : pinnedUsers.AccountIds;
            var isAnyNewEntry = false;
            foreach (var accountDetailsId in selectedAccountDetailsIds)
            {
                if (pinnedUsers == null)
                {
                    accountIds.Add(ObjectId.Parse(accountDetailsId));
                    pinnedUsers = new PinnedUsers()
                    {
                        UserId = userID,
                        //UserId = ObjectId.Parse(userid),
                        AccountIds = accountIds
                    };
                    _unitOfWork.PinnedUsersRepository.Add(pinnedUsers);
                }
                else
                {
                    if (!pinnedUsers.AccountIds.Any(x => x == ObjectId.Parse(accountDetailsId)))
                    {
                        accountIds.Add(ObjectId.Parse(accountDetailsId));
                        isAnyNewEntry = true;
                    }
                }
            }
            if (isAnyNewEntry == true)
                _unitOfWork.PinnedUsersRepository.Update(pinnedUsers);
            var result = new ServiceResponse { Success = true, Message = AccountDetailMessage.UserPinnedSuccessfully };
            return result;
        }

        public ServiceResponse UpdateExcludeUsers(List<string> selectedAccountDetailsIds, Guid organizationID, ObjectId userID)
        {
            foreach (var accountDetailsId in selectedAccountDetailsIds)
            {
                var account = _unitOfWork.AccountDetailRepository.GetAll().Where(x => x.Id == ObjectId.Parse(accountDetailsId)).SingleOrDefault();
                account.IsExclude = true;
                _unitOfWork.AccountDetailRepository.Update(account);
            }
            var result = new ServiceResponse { Success = true, Message = AccountDetailMessage.UserExcludeSuccessfully };
            return result;
        }

        public ServiceResponse GetPinnedUsers(PageRecordModel pageRecordModel)
        {
            var pinnedUsers = _unitOfWork.PinnedUsersRepository.GetAll().Where(x => x.UserId == pageRecordModel.UserID).SingleOrDefault();
            if (pinnedUsers != null)
            {
                int timelineId = pageRecordModel.Filters.Where(f => f.Key == "TimeLineId").Single().Value.Decrypt();
                var performersModel = new List<PerformersModel>();

                if (pageRecordModel.Filters.ContainsKey("Country") && !string.IsNullOrEmpty(pageRecordModel.Filters.Where(f => f.Key == "Country").Single().Value))
                {
                    var country = pageRecordModel.Filters.Where(f => f.Key == "Country").Single().Value;
                    performersModel = _unitOfWork.AccountDetailRepository.GetAll().Where(p => p.OrganizationId == pageRecordModel.OrganizationID
                        && pinnedUsers.AccountIds.Contains(p.Id) && p.Country == country && p.IsExclude == false && pageRecordModel.DatasourceIDs.Contains(p.DataSourceId)).ToList()
                        .Select(p => new PerformersModel()
                        {
                            AccountDetailId = p.Id.ToString(),
                            AccountNumber = p.AccountNumber,
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
                else
                    performersModel = _unitOfWork.AccountDetailRepository.GetAll().Where(p => p.OrganizationId == pageRecordModel.OrganizationID
                        && pinnedUsers.AccountIds.Contains(p.Id) && p.IsExclude == false && pageRecordModel.DatasourceIDs.Contains(p.DataSourceId)).ToList()
                        .Select(p => new PerformersModel()
                        {
                            AccountDetailId = p.Id.ToString(),
                            AccountNumber = p.AccountNumber,
                            ROI = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().ROI : 0,
                            DD = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().DD : 0,
                            BestPL = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().BestPL : "",
                            Leverage = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().Leverage : 0,
                            NAV = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().NAV : 0,
                            PerformerName = p.Name,
                            WINRate = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().WINRate : 0,
                            SharpRatio = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().SharpRatio : 0,
                        }).OrderByDescending(o => o.ROI).ToList();

                //var Sort = Builders<AccountDetail>.Sort;
                //accountDetails = _unitOfWork.AccountDetailRepository.GetPagedRecords(pageRecordModel, Sort.Ascending("AccountStats.ROI"), filters);

                //var accountDetailsModel = new AccountDetailModel().ToAccountDetailModel(accountDetails);
                var countries = _unitOfWork.AccountDetailRepository.GetAll().Where(p => p.OrganizationId == pageRecordModel.OrganizationID && p.IsExclude == false && pageRecordModel.DatasourceIDs.Contains(p.DataSourceId))
                    .Select(x => x.Country).Distinct().ToList();

                if (performersModel.Count > 0)
                {
                    var result = new ServiceResponse
                    {
                        MultipleData = new Dictionary<string, object>(){
                        { "performers", performersModel },
                        { "countries", countries }
                    },
                        Success = true
                    };
                    return result;
                }
                else
                {
                    var result = new ServiceResponse
                    {
                        Success = false,
                        Message = AccountDetailMessage.NoPinnedUserFound
                    };
                    return result;
                }
            }
            else
            {
                var result = new ServiceResponse
                {
                    Success = false,
                    Message = AccountDetailMessage.NoPinnedUserFound
                };
                return result;
            }

            //var country = "";
            //var timelineId = 0;
            ////var userid = "1";
            //foreach (var filter in pageRecordModel.Filters)
            //{
            //    if (filter.Key == "TimeLineId")
            //        timelineId = filter.Value.Decrypt();
            //    else if (filter.Key == "Country")
            //        country = filter.Value;
            //}
            //var pinnedUsers=  _unitOfWork.PinnedUsersRepository.GetAll().Where(x => x.UserId == ObjectId.Parse(userid)).SingleOrDefault()

            //if (string.IsNullOrEmpty(country))
            //    accountDetails = _unitOfWork.AccountDetailRepository.GetAll().Where(x => pinnedUsers.AccountIds.Contains(x.Id)).ToList();
            //else
            //    accountDetails = _unitOfWork.AccountDetailRepository.GetAll().Where(x => pinnedUsers.AccountIds.Contains(x.Id) && x.Country == country).ToList();
        }

        public ServiceResponse GetTopFivePinnedUsers(PageRecordModel pageRecordModel)
        {
            var g = _unitOfWork.PinnedUsersRepository.GetAll().ToList();

            var pinnedUsers = _unitOfWork.PinnedUsersRepository.GetAll().Where(x => x.UserId == pageRecordModel.UserID).SingleOrDefault();

            if (pinnedUsers != null && pinnedUsers.AccountIds.Count > 0)
            {
                int timelineId = pageRecordModel.Filters.Where(f => f.Key == "TimeLineId").Single().Value.Decrypt();
                var sortBy = pageRecordModel.Filters.Where(f => f.Key == "sortPerformersBy").Single().Value;
                var performersModel = new List<PerformersModel>();
                IEnumerable<PerformersModel> enumerablePerformersModel;
                if (pageRecordModel.ListFilter != null && pageRecordModel.ListFilter.Count > 0)
                {
                    if (pageRecordModel.Filters.ContainsKey("Country") && !string.IsNullOrEmpty(pageRecordModel.Filters.Where(f => f.Key == "Country").Single().Value))
                    {
                        var country = pageRecordModel.Filters.Where(f => f.Key == "Country").Single().Value;
                        enumerablePerformersModel = _unitOfWork.AccountDetailRepository.GetAll().Where(p => p.OrganizationId == pageRecordModel.OrganizationID
                        && pinnedUsers.AccountIds.Contains(p.Id) && p.Country == country && pageRecordModel.ListFilter.Contains(p.UserGroup) && p.IsExclude == false && pageRecordModel.DatasourceIDs.Contains(p.DataSourceId)).ToList()
                        .Select(p => new PerformersModel()
                        {
                            AccountDetailId = p.Id.ToString(),
                            AccountNumber = p.AccountNumber,
                            ROI = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().ROI : 0,
                            DD = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().DD : 0,
                            BestPL = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().BestPL : "",
                            Leverage = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().Leverage : 0,
                            NAV = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().NAV : 0,
                            PerformerName = p.Name,
                            WINRate = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().WINRate : 0,
                            SharpRatio = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().SharpRatio : 0,
                            UserGroup = p.UserGroup
                        });//.OrderByDescending(o => o.ROI).Take(5).ToList();
                    }
                    else
                        enumerablePerformersModel = _unitOfWork.AccountDetailRepository.GetAll().Where(p => p.OrganizationId == pageRecordModel.OrganizationID
                        && pinnedUsers.AccountIds.Contains(p.Id) && pageRecordModel.ListFilter.Contains(p.UserGroup) && p.IsExclude == false && pageRecordModel.DatasourceIDs.Contains(p.DataSourceId)).ToList()
                        .Select(p => new PerformersModel()
                        {
                            AccountDetailId = p.Id.ToString(),
                            AccountNumber = p.AccountNumber,
                            ROI = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().ROI : 0,
                            DD = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().DD : 0,
                            BestPL = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().BestPL : "",
                            Leverage = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().Leverage : 0,
                            NAV = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().NAV : 0,
                            PerformerName = p.Name,
                            WINRate = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().WINRate : 0,
                            SharpRatio = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().SharpRatio : 0,
                            UserGroup = p.UserGroup
                        });//.OrderByDescending(o => o.ROI).Take(5).ToList();
                }
                else
                {
                    if (pageRecordModel.Filters.ContainsKey("Country") && !string.IsNullOrEmpty(pageRecordModel.Filters.Where(f => f.Key == "Country").Single().Value))
                    {
                        var country = pageRecordModel.Filters.Where(f => f.Key == "Country").Single().Value;
                        enumerablePerformersModel = _unitOfWork.AccountDetailRepository.GetAll().Where(p => p.OrganizationId == pageRecordModel.OrganizationID
                        && pinnedUsers.AccountIds.Contains(p.Id) && p.Country == country && p.IsExclude == false && pageRecordModel.DatasourceIDs.Contains(p.DataSourceId)).ToList()
                        .Select(p => new PerformersModel()
                        {
                            AccountDetailId = p.Id.ToString(),
                            AccountNumber = p.AccountNumber,
                            ROI = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().ROI : 0,
                            DD = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().DD : 0,
                            BestPL = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().BestPL : "",
                            Leverage = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().Leverage : 0,
                            NAV = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().NAV : 0,
                            PerformerName = p.Name,
                            WINRate = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().WINRate : 0,
                            SharpRatio = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().SharpRatio : 0,
                            UserGroup = p.UserGroup
                        });//.OrderByDescending(o => o.ROI).Take(5).ToList();
                    }
                    else
                        enumerablePerformersModel = _unitOfWork.AccountDetailRepository.GetAll().Where(p => p.OrganizationId == pageRecordModel.OrganizationID
                        && pinnedUsers.AccountIds.Contains(p.Id) && p.IsExclude == false && pageRecordModel.DatasourceIDs.Contains(p.DataSourceId)).ToList()
                        .Select(p => new PerformersModel()
                        {
                            AccountDetailId = p.Id.ToString(),
                            AccountNumber = p.AccountNumber,
                            ROI = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().ROI : 0,
                            DD = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().DD : 0,
                            BestPL = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().BestPL : "",
                            Leverage = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().Leverage : 0,
                            NAV = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().NAV : 0,
                            PerformerName = p.Name,
                            WINRate = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().WINRate : 0,
                            SharpRatio = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().SharpRatio : 0,
                            UserGroup = p.UserGroup
                        });//.OrderByDescending(o => o.ROI).Take(5).ToList();
                }
                if (sortBy == "maxDD")
                {
                    performersModel = enumerablePerformersModel.OrderByDescending(o => o.DD).Take(5).ToList();
                }
                else if (sortBy == "winRate")
                {
                    performersModel = enumerablePerformersModel.OrderByDescending(o => o.WINRate).Take(5).ToList();
                }
                else if (sortBy == "sharpRatio")
                {
                    performersModel = enumerablePerformersModel.OrderByDescending(o => o.SharpRatio).Take(5).ToList();
                }
                else
                {
                    performersModel = enumerablePerformersModel.OrderByDescending(o => o.ROI).Take(5).ToList();
                }

                //var Sort = Builders<AccountDetail>.Sort;
                //accountDetails = _unitOfWork.AccountDetailRepository.GetPagedRecords(pageRecordModel, Sort.Ascending("AccountStats.ROI"), filters);

                //var accountDetailsModel = new AccountDetailModel().ToAccountDetailModel(accountDetails);
                var countries = _unitOfWork.AccountDetailRepository.GetAll().Where(p => p.OrganizationId == pageRecordModel.OrganizationID && p.IsExclude == false && pageRecordModel.DatasourceIDs.Contains(p.DataSourceId))
                    .Select(x => x.Country).Distinct().ToList();

                var timeLineIds = _unitOfWork.AccountDetailRepository.GetAll().Where(p => p.OrganizationId == pageRecordModel.OrganizationID && p.IsExclude == false && pageRecordModel.DatasourceIDs.Contains(p.DataSourceId))
                    .SelectMany(x => x.AccountStats).Select(x => x.TimeLineId).Distinct().ToList();
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

                var userGroups = performersModel.Select(x => x.UserGroup).Distinct().ToList();
                var riskFreeRate = "";
                if (pageRecordModel.DatasourceIDs.Count > 1)
                {
                    riskFreeRate = "N/A";
                    performersModel.ForEach(x => x.SharpRatio = null);
                }
                else
                {
                    var datasourceId = pageRecordModel.DatasourceIDs.FirstOrDefault();
                    var datasource = _unitOfWork.DatasourceRepository.GetAll().Where(x => x.Id == datasourceId).FirstOrDefault();
                    riskFreeRate = datasource==null?"":datasource.RateOfReturns.Where(x => x.TimelineId == timelineId).FirstOrDefault().RateOfReturn.ToString();
                }
                var result = new ServiceResponse
                {
                    MultipleData = new Dictionary<string, object>(){
                        { "performers", performersModel },
                        { "countries", countries },
                        { "timeLines", timeLines },
                        { "userGroups", userGroups },
                        { "riskFreeRate", riskFreeRate}
                    },
                    Success = true
                };
                return result;
            }
            else
            {
                var result = new ServiceResponse
                {
                    Success = false,
                    Message = AccountDetailMessage.NoPinnedUserFound
                };
                return result;
            }

            //var country = "";
            //var timelineId = 0;
            ////var userid = "1";
            //foreach (var filter in pageRecordModel.Filters)
            //{
            //    if (filter.Key == "TimeLineId")
            //        timelineId = filter.Value.Decrypt();
            //    else if (filter.Key == "Country")
            //        country = filter.Value;
            //}
            //if (string.IsNullOrEmpty(country))
            //    accountDetails = _unitOfWork.AccountDetailRepository.GetAll().Where(x => pinnedUsers.AccountIds.Contains(x.Id) && x.AccountStats.Any(a => a.TimeLineId == timelineId)).Take(5).ToList();
            //else
            //    accountDetails = _unitOfWork.AccountDetailRepository.GetAll().Where(x => pinnedUsers.AccountIds.Contains(x.Id) && x.Country == country && x.AccountStats.Any(a => a.TimeLineId == timelineId)).Take(5).ToList();

        }

        public ServiceResponse UnpinUser(string accountDetailId, ObjectId userID)
        {
            try
            {
                //var userId = "1";
                var pinnedUsers = _unitOfWork.PinnedUsersRepository.GetAll().Where(x => x.UserId == userID).SingleOrDefault();
                pinnedUsers.AccountIds.RemoveAll(x => x == ObjectId.Parse(accountDetailId));
                _unitOfWork.PinnedUsersRepository.Update(pinnedUsers);
                return new ServiceResponse() { Success = true, Message = AccountDetailMessage.UserUnpinnedSuccessfully };
            }
            catch (Exception)
            {
                return new ServiceResponse() { Success = true, Message = AccountDetailMessage.SomethingWentWrong };
            }

        }

        public ServiceResponse GetUserDetails(PageRecordModel pageRecordModel, ObjectId userID)
        {
            var accountId = pageRecordModel.Filters.FirstOrDefault(x => x.Key == "accountId").Value;
            var timelineId = pageRecordModel.Filters.FirstOrDefault(x => x.Key == "TimeLineId").Value.Decrypt();
            var accountDetail = _unitOfWork.AccountDetailRepository.GetById(accountId);
            var accountDetailModel = new AccountDetailModel().ToAccountDetailModel(accountDetail);
            var accountDetailModelWithInstuments = new AccountDetailModel().ToAccountDetailWithInstrumentStatsModel(accountDetail);

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
            var performaceData = getAccountStatsFromAccountDetailAccordingToTimeLine(accountDetail, timelineId);

            //var listOfTimeLines = new List<int>(timelineId);
            //var highestNavAccountId = GetHighestNavAccountId(timelineId);
            //var highestNavUser = _unitOfWork.AccountDetailRepository.GetById(highestNavAccountId.ToString());
            //var highestNavs = getAccountStatsFromAccountDetailAccordingToTimeLine(highestNavUser, timelineId);

            var instrumentalTradeByTimelineId = accountDetailModelWithInstuments.InstrumentStatsModel.Where(x => x.TimeLineId == timelineId).OrderByDescending(x => x.Volume).Take(5).ToList();
            var instrumentalTradeChartData = GetInstrumentalTradeChartData(instrumentalTradeByTimelineId);

            var test = instrumentalTradeChartData.OrderBy(x => x.Key);

            var maxInstrumentalTrade = instrumentalTradeByTimelineId.OrderByDescending(item => item.Volume).FirstOrDefault();



            var pinnedUsers = _unitOfWork.PinnedUsersRepository.GetAll().Where(x => x.UserId == userID).FirstOrDefault();
            var isUserPinned = pinnedUsers == null ? false : pinnedUsers.AccountIds.Any(x => x == accountDetail.Id);
            //var maxInstrumentalTradeValue = new Dictionary<string, double>{ "",instrumentalTradeByTimelineId.Max(x => x.Volume) };
            return new ServiceResponse()
            {
                MultipleData = new Dictionary<string, object>(){
                    { "accountDetails", accountDetailModel },
                    { "timeLines", timeLines },
                    { "performaceData", performaceData },
                    //{ "highestNavs", highestNavs},
                    { "instrumentalTradeChartData", instrumentalTradeChartData },
                    { "maxInstrumentalTrade", maxInstrumentalTrade },
                    { "isUserPinned", isUserPinned }
                },
                Success = true
            };
        }


        public ServiceResponse GetUserDetailInstrumental(int records, PageRecordModel pageRecordModel, ObjectId userID)
        {

            var accountId = pageRecordModel.Filters.FirstOrDefault(x => x.Key == "accountId").Value;
            var timelineId = pageRecordModel.Filters.FirstOrDefault(x => x.Key == "TimeLineId").Value.Decrypt();
            var accountDetail = _unitOfWork.AccountDetailRepository.GetById(accountId);

            var accountDetailModelWithInstuments = new AccountDetailModel().ToAccountDetailWithInstrumentStatsModel(accountDetail);

            var instrumentalTradeByTimelineId = accountDetailModelWithInstuments.InstrumentStatsModel.Where(x => x.TimeLineId == timelineId).OrderByDescending(x => x.Volume).ToList();

            records = records == 100 ? instrumentalTradeByTimelineId.Count : records;

            instrumentalTradeByTimelineId = accountDetailModelWithInstuments.InstrumentStatsModel.Where(x => x.TimeLineId == timelineId).OrderByDescending(x => x.Volume).Take(records).ToList();

            var instrumentalTradeChartData = GetInstrumentalTradeChartData(instrumentalTradeByTimelineId);


            var maxInstrumentalTrade = instrumentalTradeByTimelineId.OrderByDescending(item => item.Volume).FirstOrDefault();

            return new ServiceResponse()
            {
                MultipleData = new Dictionary<string, object>(){
                    { "instrumentalTradeChartData", instrumentalTradeChartData },
                    { "maxInstrumentalTrade", maxInstrumentalTrade },
                },
                Success = true
            };
        }

        public ObjectId GetHighestNavAccountId(int timelineId)
        {
            var accountId = new ObjectId();
            if (timelineId == 1)
            {
                var month = DateTime.Now.Month;
                var ids = Timelines.MonthlyTimelines.Skip(month).Take(12).ToList();
                var AccountDetailWithHighestValue = _unitOfWork.AccountDetailRepository.GetAll().Where(x => x.AccountStats.Any(y => ids.Contains(y.TimeLineId))).Select(x => new AccountDetailWithHighestValue()
                {
                    AccountId = x.Id,
                    AverageNav = x.AccountStats.AsQueryable().Where(y => ids.Contains(y.TimeLineId)).Sum(z => z.NAV)
                }).OrderByDescending(x => x.AverageNav).FirstOrDefault();
                accountId = AccountDetailWithHighestValue.AccountId;
            }
            else if (timelineId == 2)
            {
                var month = DateTime.Now.Month;
                if (month > 6)
                    month = month - 6;

                var ids = Timelines.MonthlyTimelines.Skip(month).Take(6).ToList();
                accountId = _unitOfWork.AccountDetailRepository.GetAll().Where(x => x.AccountStats.Any(y => ids.Contains(y.TimeLineId))).Select(x => new AccountDetailWithHighestValue()
                {
                    AccountId = x.Id,
                    AverageNav = x.AccountStats.AsQueryable().Where(y => ids.Contains(y.TimeLineId)).Sum(z => z.NAV)
                }).OrderByDescending(x => x.AverageNav).FirstOrDefault().AccountId;

            }
            else if (timelineId == 3)
            {
                var day = DateTime.Now.Day;
                var DayOfWeek = DateTime.Now.DayOfWeek - System.DayOfWeek.Monday;
                var monthFromQuater = DateTime.Now.Month % 3 == 0 ? 3 : DateTime.Now.Month % 3;
                var weeksInCurrentMonth = Convert.ToInt32((day - DayOfWeek) / 7);
                var weeks = ((monthFromQuater - 1) * 4) + weeksInCurrentMonth;
                var ids = Timelines.WeeklyTimelines.Skip(weeks).Take(12).ToList();
                accountId = _unitOfWork.AccountDetailRepository.GetAll().Where(x => x.AccountStats.Any(y => ids.Contains(y.TimeLineId))).Select(x => new AccountDetailWithHighestValue()
                {
                    AccountId = x.Id,
                    AverageNav = x.AccountStats.AsQueryable().Where(y => ids.Contains(y.TimeLineId)).Sum(z => z.NAV)
                }).OrderByDescending(x => x.AverageNav).FirstOrDefault().AccountId;
            }
            else if (timelineId == 4)
            {
                var day = DateTime.Now.Day;
                var DayOfWeek = DateTime.Now.DayOfWeek - System.DayOfWeek.Monday;
                var weeks = Convert.ToInt32((day - DayOfWeek) / 7);
                var ids = Timelines.WeeklyTimelines.Skip(weeks).Take(4).ToList();
                accountId = _unitOfWork.AccountDetailRepository.GetAll().Where(x => x.AccountStats.Any(y => ids.Contains(y.TimeLineId))).Select(x => new AccountDetailWithHighestValue()
                {
                    AccountId = x.Id,
                    AverageNav = x.AccountStats.AsQueryable().Where(y => ids.Contains(y.TimeLineId)).Sum(z => z.NAV)
                }).OrderByDescending(x => x.AverageNav).FirstOrDefault().AccountId;

            }
            else if (timelineId == 6)
            {
                var DayOfWeek = DateTime.Now.DayOfWeek - System.DayOfWeek.Monday;
                var ids = Timelines.DailyTimelines.Skip(DayOfWeek).Take(7).ToList();
                accountId = _unitOfWork.AccountDetailRepository.GetAll().Where(x => x.AccountStats.Any(y => ids.Contains(y.TimeLineId))).Select(x => new AccountDetailWithHighestValue()
                {
                    AccountId = x.Id,
                    AverageNav = x.AccountStats.AsQueryable().Where(y => ids.Contains(y.TimeLineId)).Sum(z => z.NAV)
                }).OrderByDescending(x => x.AverageNav).FirstOrDefault().AccountId;
            }
            else if (timelineId == 45)
            {
                var ids = Timelines.MonthlyTimelines;
                accountId = _unitOfWork.AccountDetailRepository.GetAll().Where(x => x.AccountStats.Any(y => ids.Contains(y.TimeLineId))).Select(x => new AccountDetailWithHighestValue()
                {
                    AccountId = x.Id,
                    AverageNav = x.AccountStats.AsQueryable().Where(y => ids.Contains(y.TimeLineId)).Sum(z => z.NAV)
                }).OrderByDescending(x => x.AverageNav).FirstOrDefault().AccountId;
            }
            else if (timelineId == 46)
            {
                var day = DateTime.Now.Day;
                var DayOfWeek = DateTime.Now.DayOfWeek - System.DayOfWeek.Monday;
                var weeks = Convert.ToInt32((day - DayOfWeek) / 7);
                var ids = Timelines.WeeklyTimelines.Take(weeks).ToList();
                accountId = _unitOfWork.AccountDetailRepository.GetAll().Where(x => x.AccountStats.Any(y => ids.Contains(y.TimeLineId))).Select(x => new AccountDetailWithHighestValue()
                {
                    AccountId = x.Id,
                    AverageNav = x.AccountStats.AsQueryable().Where(y => ids.Contains(y.TimeLineId)).Sum(z => z.NAV)
                }).OrderByDescending(x => x.AverageNav).FirstOrDefault().AccountId;
            }
            else if (timelineId == 47)
            {
                var DayOfWeek = DateTime.Now.DayOfWeek - System.DayOfWeek.Monday;
                var ids = Timelines.DailyTimelines.Take(DayOfWeek).ToList();
                accountId = _unitOfWork.AccountDetailRepository.GetAll().Where(x => x.AccountStats.Any(y => ids.Contains(y.TimeLineId))).Select(x => new AccountDetailWithHighestValue()
                {
                    AccountId = x.Id,
                    AverageNav = x.AccountStats.AsQueryable().Where(y => ids.Contains(y.TimeLineId)).Sum(z => z.NAV)
                }).OrderByDescending(x => x.AverageNav).FirstOrDefault().AccountId;
            }
            else if (timelineId == 48)
            {
                var day = DateTime.Now.Day;
                var DayOfWeek = DateTime.Now.DayOfWeek - System.DayOfWeek.Monday;
                var monthFromQuater = DateTime.Now.Month % 3 == 0 ? 3 : DateTime.Now.Month % 3;
                var weeksInCurrentMonth = Convert.ToInt32((day - DayOfWeek) / 7);
                var weeks = ((monthFromQuater - 1) * 4) + weeksInCurrentMonth;
                var ids = Timelines.WeeklyTimelines.Take(weeks).ToList();
                accountId = _unitOfWork.AccountDetailRepository.GetAll().Where(x => x.AccountStats.Any(y => ids.Contains(y.TimeLineId))).Select(x => new AccountDetailWithHighestValue()
                {
                    AccountId = x.Id,
                    AverageNav = x.AccountStats.AsQueryable().Where(y => ids.Contains(y.TimeLineId)).Sum(z => z.NAV)
                }).OrderByDescending(x => x.AverageNav).FirstOrDefault().AccountId;
            }
            else if (timelineId == 49)
            {
                var month = DateTime.Now.Month;
                var ids = Timelines.MonthlyTimelines.Take(month).ToList();
                accountId = _unitOfWork.AccountDetailRepository.GetAll().Where(x => x.AccountStats.Any(y => ids.Contains(y.TimeLineId))).Select(x => new AccountDetailWithHighestValue()
                {
                    AccountId = x.Id,
                    AverageNav = x.AccountStats.AsQueryable().Where(y => ids.Contains(y.TimeLineId)).Sum(z => z.NAV)
                }).OrderByDescending(x => x.AverageNav).FirstOrDefault().AccountId;
            }

            return accountId;
        }

        public Dictionary<string, double> GetInstrumentalTradeChartData(List<InstrumentStatsModel> instrumentStatsModels)
        {
            var result = new Dictionary<string, double>();
            var volume = instrumentStatsModels.Sum(x => x.Volume);
            foreach (var instrumentStatsModel in instrumentStatsModels)
            {
                var percent = Math.Round(((instrumentStatsModel.Volume / volume) * 100), 2);
                result.Add(instrumentStatsModel.InstrumentName, percent);
            }
            return result;
        }

        public Dictionary<string, Object> getAccountStatsFromAccountDetailAccordingToTimeLine(AccountDetail accountDetail, int timelineId)
        {
            var accountStats = new List<AccountStats>();
            var categories = new List<string>();
            if (timelineId == 1)
            {
                var month = DateTime.Now.Month;
                var ids = Timelines.MonthlyTimelines.Skip(month).Take(12).ToList();
                accountStats = accountDetail.AccountStats.Where(x => ids.Contains(x.TimeLineId)).Zip(ids, (o, i) => new { o, i })
                               .OrderBy(x => x.i).Select(x => x.o).Reverse().ToList();
                categories = Enumerable.Range(0, 12).Select(i => new DateTime(DateTime.Now.Year, 1, 1).AddMonths(i - 12))
                            .Select(date => date.ToString("MMM-yy")).ToList();
            }
            else if (timelineId == 2)
            {
                var month = DateTime.Now.Month;
                if (month > 6)
                    month = month - 6;

                var ids = Timelines.MonthlyTimelines.Skip(month).Take(6).ToList();
                accountStats = accountDetail.AccountStats.Where(x => ids.Contains(x.TimeLineId)).Zip(ids, (o, i) => new { o, i })
                               .OrderBy(x => x.i).Select(x => x.o).Reverse().ToList();
                categories = Enumerable.Range(0, 6).Select(i => DateTime.Now.AddMonths(-month + 1).AddMonths(i - 6))
                             .Select(date => date.ToString("MMM-yy")).ToList();
            }
            else if (timelineId == 3)
            {
                var day = DateTime.Now.Day;
                var DayOfWeek = DateTime.Now.DayOfWeek - System.DayOfWeek.Monday;
                var monthFromQuater = DateTime.Now.Month % 3 == 0 ? 3 : DateTime.Now.Month % 3;
                var weeksInCurrentMonth = Convert.ToInt32((day - DayOfWeek) / 7);
                var weeks = ((monthFromQuater - 1) * 4) + weeksInCurrentMonth;
                var ids = Timelines.WeeklyTimelines.Skip(weeks).Take(12).ToList();
                accountStats = accountDetail.AccountStats.Where(x => ids.Contains(x.TimeLineId)).Zip(ids, (o, i) => new { o, i })
                               .OrderBy(x => x.i).Select(x => x.o).Reverse().ToList();
                categories = Enumerable.Range(0, 12).Select(i => DateTime.Now.AddMonths(-monthFromQuater).AddDays((i * 7) - (12 * 7))).Select(date => date.ToString("dd-MMM")).ToList();
            }
            else if (timelineId == 4)
            {
                var day = DateTime.Now.Day;
                var DayOfWeek = DateTime.Now.DayOfWeek - System.DayOfWeek.Monday;
                var weeks = Convert.ToInt32((day - DayOfWeek) / 7);
                var ids = Timelines.WeeklyTimelines.Skip(weeks).Take(4).ToList();
                accountStats = accountDetail.AccountStats.Where(x => ids.Contains(x.TimeLineId)).Zip(ids, (o, i) => new { o, i })
                               .OrderBy(x => x.i).Select(x => x.o).Reverse().ToList();
                var dayOfMonth = (new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).Date - DateTime.Now.Date).TotalDays - 1;
                categories = Enumerable.Range(0, 4).Select(i => DateTime.Now.AddDays(dayOfMonth).AddDays((i * 7) - (4 * 7))).Select(date => date.ToString("dd-MMM")).ToList();
            }

            else if (timelineId == 6)
            {
                var DayOfWeek = DateTime.Now.DayOfWeek - System.DayOfWeek.Monday;
                var ids = Timelines.DailyTimelines.Skip(DayOfWeek).Take(7).ToList();
                accountStats = accountDetail.AccountStats.Where(x => ids.Contains(x.TimeLineId)).Zip(ids, (o, i) => new { o, i })
                               .OrderBy(x => x.i).Select(x => x.o).Reverse().ToList();
                categories = Enumerable.Range(0, 7).Select(i => DateTime.Now.AddDays(-DayOfWeek).AddDays(i - 7)).Select(date => date.ToString("dd-MMM")).ToList();
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

        private static List<EnumHelper> GetTimeLines(List<AccountDetail> accountDetails)
        {
            var timeLineIds = accountDetails.SelectMany(x => x.AccountStats).Select(x => x.TimeLineId).Distinct().ToList();
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
            return timeLines;
        }

        private static List<FilterDefinition<AccountDetail>> GetFilterDefinationFromFilters(PageRecordModel pageRecordModel, string[] assignedUserGroups)
        {
            var filtersList = new List<FilterDefinition<AccountDetail>>();
            var timeLineId = pageRecordModel.Filters.FirstOrDefault(x => x.Key == "TimeLineId");
            foreach (var filter in pageRecordModel.Filters)
            {
                if (filter.Key == "WINRate")
                    filtersList.Add(Builders<AccountDetail>.Filter.ElemMatch(x => x.AccountStats, y => y.WINRate <= Convert.ToDouble(filter.Value) && y.TimeLineId == timeLineId.Value.Decrypt()));
                else if (filter.Key == "ROI")
                    filtersList.Add(Builders<AccountDetail>.Filter.ElemMatch(x => x.AccountStats, y => y.ROI <= Convert.ToDouble(filter.Value) && y.TimeLineId == timeLineId.Value.Decrypt()));
                else if (filter.Key == "DD")
                    filtersList.Add(Builders<AccountDetail>.Filter.ElemMatch(x => x.AccountStats, y => y.DD <= Convert.ToDouble(filter.Value) && y.TimeLineId == timeLineId.Value.Decrypt()));
                else if (filter.Key == "TimeLineId")
                    filtersList.Add(Builders<AccountDetail>.Filter.ElemMatch(x => x.AccountStats, y => y.TimeLineId == filter.Value.Decrypt()));
                else if (filter.Key == "Search")
                    filtersList.Add(Builders<AccountDetail>.Filter.Regex(x => x.Name, new BsonRegularExpression(filter.Value, "i")) | Builders<AccountDetail>.Filter.Regex(x => x.AccountNumber, filter.Value));
                else if (filter.Key != "WINRate" && filter.Key != "ROI" && filter.Key != "DD")
                    filtersList.Add(Builders<AccountDetail>.Filter.Eq(filter.Key, filter.Value));
            }
            //filtersList.Add(Builders<AccountDetail>.Filter.Eq("Id", pageRecordModel.UserID));
            filtersList.Add(Builders<AccountDetail>.Filter.Eq("OrganizationId", pageRecordModel.OrganizationID));
            filtersList.Add(Builders<AccountDetail>.Filter.Where(a => assignedUserGroups.Contains(a.UserGroup)));

            return filtersList;
        }

        public List<PerformersModel> GetAccountsForExport(PageRecordModel pageRecordModel, string[] assignedUserGroups)
        {
            double count = 0;
            //var accountDetails = new List<AccountDetail>();
            var performersModel = new List<PerformersModel>();
            if (string.IsNullOrEmpty(pageRecordModel.SortBy))
                pageRecordModel.SortBy = "Name";

            int timelineId = pageRecordModel.Filters.Single(x => x.Key == "TimeLineId").Value.Decrypt();
            double minWin = _unitOfWork.AccountDetailRepository.GetAll().Where(x => x.OrganizationId == pageRecordModel.OrganizationID && assignedUserGroups.Contains(x.UserGroup) && pageRecordModel.DatasourceIDs.Contains(x.DataSourceId)).Select(p => new PerformersModel()
            {
                WINRate = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().WINRate : 0,

            }).Min(m => m.WINRate);

            double minROI = _unitOfWork.AccountDetailRepository.GetAll().Where(x => x.OrganizationId == pageRecordModel.OrganizationID && assignedUserGroups.Contains(x.UserGroup) && pageRecordModel.DatasourceIDs.Contains(x.DataSourceId)).Select(p => new PerformersModel()
            {
                ROI = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().ROI : 0,

            }).Min(m => m.ROI);

            double minDD = _unitOfWork.AccountDetailRepository.GetAll().Where(x => x.OrganizationId == pageRecordModel.OrganizationID && assignedUserGroups.Contains(x.UserGroup) && pageRecordModel.DatasourceIDs.Contains(x.DataSourceId)).Select(p => new PerformersModel()
            {
                DD = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().DD : 0,

            }).Min(m => m.DD);

            double maxWin = _unitOfWork.AccountDetailRepository.GetAll().Where(x => x.OrganizationId == pageRecordModel.OrganizationID && assignedUserGroups.Contains(x.UserGroup) && pageRecordModel.DatasourceIDs.Contains(x.DataSourceId)).Select(p => new PerformersModel()
            {
                WINRate = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().WINRate : 0,

            }).Max(m => m.WINRate);

            double maxROI = _unitOfWork.AccountDetailRepository.GetAll().Where(x => x.OrganizationId == pageRecordModel.OrganizationID && assignedUserGroups.Contains(x.UserGroup) && pageRecordModel.DatasourceIDs.Contains(x.DataSourceId)).Select(p => new PerformersModel()
            {
                ROI = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().ROI : 0,

            }).Max(m => m.ROI);

            double maxDD = _unitOfWork.AccountDetailRepository.GetAll().Where(x => x.OrganizationId == pageRecordModel.OrganizationID && assignedUserGroups.Contains(x.UserGroup) && pageRecordModel.DatasourceIDs.Contains(x.DataSourceId)).Select(p => new PerformersModel()
            {
                DD = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().DD : 0,

            }).Max(m => m.DD);

            if (pageRecordModel.Filters.Count > 0 && !string.IsNullOrEmpty(pageRecordModel.SortBy))
            {
                //List<FilterDefinition<AccountDetail>> filtersList = GetFilterDefinationFromFilters(pageRecordModel);
                //var filters = Builders<AccountDetail>.Filter.And(filtersList);
                //var Sort = Builders<AccountDetail>.Sort;
                //if (pageRecordModel.SortOrder == "asc")
                //    accountDetails = _unitOfWork.AccountDetailRepository.GetPagedRecords(pageRecordModel, Sort.Ascending(pageRecordModel.SortBy), filters).ToList();
                //else
                //    accountDetails = _unitOfWork.AccountDetailRepository.GetPagedRecords(pageRecordModel, Sort.Descending(pageRecordModel.SortBy), filters).ToList();
                //var sort = accountDetails.OrderBy(x => x.AccountStats.OrderBy(a => a.ROI)).ToList();


                //var country = "";
                var country = new List<string>();
                //var usergroup = "";
                var usergroup = new List<string>();
                var search = "";
                foreach (var filter in pageRecordModel.Filters)
                {
                    //if (filter.Key == "Country")
                    //    country = pageRecordModel.Filters.Single(x => x.Key == "Country").Value;
                    //if (filter.Key == "UserGroup")
                    //    usergroup = pageRecordModel.Filters.Single(x => x.Key == "UserGroup").Value;
                    if (filter.Key == "WINRateMin")
                        minWin = Convert.ToDouble(pageRecordModel.Filters.Single(x => x.Key == "WINRateMin").Value);
                    if (filter.Key == "WINRateMax")
                        maxWin = Convert.ToDouble(pageRecordModel.Filters.Single(x => x.Key == "WINRateMax").Value);
                    if (filter.Key == "ROIMax")
                        maxROI = Convert.ToDouble(pageRecordModel.Filters.Single(x => x.Key == "ROIMax").Value);
                    if (filter.Key == "ROIMin")
                        minROI = Convert.ToDouble(pageRecordModel.Filters.Single(x => x.Key == "ROIMin").Value);
                    if (filter.Key == "DDMax")
                        maxDD = Convert.ToDouble(pageRecordModel.Filters.Single(x => x.Key == "DDMax").Value);
                    if (filter.Key == "DDMin")
                        minDD = Convert.ToDouble(pageRecordModel.Filters.Single(x => x.Key == "DDMin").Value);
                    if (filter.Key == "Search")
                        search = pageRecordModel.Filters.Single(x => x.Key == "Search").Value;
                }
                foreach (var filter in pageRecordModel.MultipleListFilter)
                {
                    if (filter.Key == "Countries")
                        country = pageRecordModel.MultipleListFilter.Single(x => x.Key == "Countries").Value.ToList();
                    if (filter.Key == "UserGroups")
                        usergroup = pageRecordModel.MultipleListFilter.Single(x => x.Key == "UserGroups").Value.ToList();
                }
                var expression = GetExpression(pageRecordModel.SortBy);
                IQueryable<PerformersModel> accountsModel;
                //if (!string.IsNullOrEmpty(country) && !string.IsNullOrEmpty(usergroup))
                if (country.Any() && usergroup.Any())
                {
                    //accountsModel = _unitOfWork.AccountDetailRepository.GetAll().Where(x => x.OrganizationId == pageRecordModel.OrganizationID &&  x.IsExclude==false  && x.Country == country && x.UserGroup == usergroup && (x.Name.Contains(search) || x.AccountNumber.Contains(search)))
                    accountsModel = _unitOfWork.AccountDetailRepository.GetAll().Where(x => x.OrganizationId == pageRecordModel.OrganizationID && assignedUserGroups.Contains(x.UserGroup) && x.IsExclude == false &&
                    country.Contains(x.Country) && usergroup.Contains(x.UserGroup) && (x.Name.Contains(search) || x.AccountNumber.Contains(search))
                    && pageRecordModel.DatasourceIDs.Contains(x.DataSourceId)).Select(p => new PerformersModel()
                    {
                        AccountDetailIdObjectId = p.Id,
                        AccountNumber = p.AccountNumber,
                        ROI = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().ROI : 0,
                        DD = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().DD : 0,
                        BestPL = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().BestPL : "",
                        Leverage = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().Leverage : 0,
                        NAV = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().NAV : 0,
                        PerformerName = p.Name,
                        Balance = p.Balance,
                        Location = p.Country,
                        UserGroup = p.UserGroup,
                        WINRate = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().WINRate : 0,
                        SharpRatio = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().SharpRatio : 0,
                    }).Where(w => w.WINRate <= maxWin && w.WINRate >= minWin && w.DD <= maxDD && w.DD >= minDD && w.ROI <= maxROI && w.ROI >= minROI);
                }

                //else if (!string.IsNullOrEmpty(usergroup))
                else if (usergroup.Any())
                {
                    //accountsModel = _unitOfWork.AccountDetailRepository.GetAll().Where(x => x.OrganizationId == pageRecordModel.OrganizationID && x.IsExclude == false &&  x.UserGroup == usergroup && (x.Name.Contains(search) || x.AccountNumber.Contains(search))).Select(p => new PerformersModel()
                    accountsModel = _unitOfWork.AccountDetailRepository.GetAll().Where(x => x.OrganizationId == pageRecordModel.OrganizationID && assignedUserGroups.Contains(x.UserGroup) && x.IsExclude == false &&
                    usergroup.Contains(x.UserGroup) && (x.Name.Contains(search) || x.AccountNumber.Contains(search)) && pageRecordModel.DatasourceIDs.Contains(x.DataSourceId)).Select(p => new PerformersModel()
                    {
                        AccountDetailIdObjectId = p.Id,
                        AccountNumber = p.AccountNumber,
                        ROI = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().ROI : 0,
                        DD = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().DD : 0,
                        BestPL = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().BestPL : "",
                        Leverage = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().Leverage : 0,
                        NAV = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().NAV : 0,
                        PerformerName = p.Name,
                        Balance = p.Balance,
                        Location = p.Country,
                        UserGroup = p.UserGroup,
                        WINRate = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().WINRate : 0,
                        SharpRatio = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().SharpRatio : 0,
                    }).Where(w => w.WINRate <= maxWin && w.WINRate >= minWin && w.DD <= maxDD && w.DD >= minDD && w.ROI <= maxROI && w.ROI >= minROI);
                }

                //else if (!string.IsNullOrEmpty(country))
                else if (country.Any())
                {
                    //accountsModel = _unitOfWork.AccountDetailRepository.GetAll().Where(x => x.OrganizationId == pageRecordModel.OrganizationID && x.IsExclude == false  && x.Country == country && (x.Name.Contains(search) || x.AccountNumber.Contains(search))).Select(p => new PerformersModel()
                    accountsModel = _unitOfWork.AccountDetailRepository.GetAll().Where(x => x.OrganizationId == pageRecordModel.OrganizationID && assignedUserGroups.Contains(x.UserGroup) && x.IsExclude == false &&
                    country.Contains(x.Country) && (x.Name.Contains(search) || x.AccountNumber.Contains(search)) && pageRecordModel.DatasourceIDs.Contains(x.DataSourceId)).Select(p => new PerformersModel()
                    {
                        AccountDetailIdObjectId = p.Id,

                        AccountNumber = p.AccountNumber,
                        ROI = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().ROI : 0,
                        DD = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().DD : 0,
                        BestPL = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().BestPL : "",
                        Leverage = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().Leverage : 0,
                        NAV = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().NAV : 0,
                        PerformerName = p.Name,
                        Balance = p.Balance,
                        Location = p.Country,
                        UserGroup = p.UserGroup,
                        WINRate = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().WINRate : 0,
                        SharpRatio = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().SharpRatio : 0,
                    }).Where(w => w.WINRate <= maxWin && w.WINRate >= minWin && w.DD <= maxDD && w.DD >= minDD && w.ROI <= maxROI && w.ROI >= minROI);
                }

                else if (!string.IsNullOrEmpty(search))
                {
                    accountsModel = _unitOfWork.AccountDetailRepository.GetAll().Where(x => x.OrganizationId == pageRecordModel.OrganizationID && assignedUserGroups.Contains(x.UserGroup) && x.IsExclude == false &&
                    (x.Name.Contains(search) || x.AccountNumber.Contains(search)) && pageRecordModel.DatasourceIDs.Contains(x.DataSourceId)).Select(p => new PerformersModel()
                    {
                        AccountDetailIdObjectId = p.Id,

                        AccountNumber = p.AccountNumber,
                        ROI = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().ROI : 0,
                        DD = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().DD : 0,
                        BestPL = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().BestPL : "",
                        Leverage = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().Leverage : 0,
                        NAV = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().NAV : 0,
                        PerformerName = p.Name,
                        Balance = p.Balance,
                        Location = p.Country,
                        UserGroup = p.UserGroup,
                        WINRate = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().WINRate : 0,
                        SharpRatio = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().SharpRatio : 0,
                    }).Where(w => w.WINRate <= maxWin && w.WINRate >= minWin && w.DD <= maxDD && w.DD >= minDD && w.ROI <= maxROI && w.ROI >= minROI);
                }
                else
                {
                    accountsModel = _unitOfWork.AccountDetailRepository.GetAll().Where(x => x.OrganizationId == pageRecordModel.OrganizationID && assignedUserGroups.Contains(x.UserGroup) && x.IsExclude == false && pageRecordModel.DatasourceIDs.Contains(x.DataSourceId)).Select(p => new PerformersModel()
                    {
                        AccountDetailIdObjectId = p.Id,

                        AccountNumber = p.AccountNumber,
                        ROI = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().ROI : 0,
                        DD = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().DD : 0,
                        BestPL = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().BestPL : "",
                        Leverage = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().Leverage : 0,
                        NAV = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().NAV : 0,
                        PerformerName = p.Name,
                        Balance = p.Balance,
                        Location = p.Country,
                        UserGroup = p.UserGroup,
                        WINRate = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().WINRate : 0,
                        SharpRatio = p.AccountStats.Count(ast => ast.TimeLineId == timelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == timelineId).First().SharpRatio : 0,
                    }).Where(w => w.WINRate <= maxWin && w.WINRate >= minWin && w.DD <= maxDD && w.DD >= minDD && w.ROI <= maxROI && w.ROI >= minROI);
                }
                count = accountsModel.Count();
                if (pageRecordModel.SortOrder == "asc")
                    performersModel = accountsModel.OrderBy(expression).ToList();
                else
                    performersModel = accountsModel.OrderByDescending(expression).ToList();
            }

            return performersModel;
        }


        public InstrumentStatsModel GetProfitLossData(string instrumentalName, string timeLineId, string accountId, string[] assignedUserGroups)
        {
            int TimeLineID = timeLineId.Decrypt();
            var AccountId = ObjectId.Parse(accountId);
            var instrument = _unitOfWork.AccountDetailRepository.GetAll().Where(x => x.Id == AccountId && assignedUserGroups.Contains(x.UserGroup) && x.InstrumentStats.Any(y => y.InstrumentName == instrumentalName)).Select(p => new InstrumentStatsModel()
            {
                Profit = p.InstrumentStats.Count(ast => ast.TimeLineId == TimeLineID && ast.InstrumentName == instrumentalName) > 0 ? p.InstrumentStats.Where(ast => ast.TimeLineId == TimeLineID && ast.InstrumentName == instrumentalName).First().Profit : 0,
                Loss = p.InstrumentStats.Count(ast => ast.TimeLineId == TimeLineID && ast.InstrumentName == instrumentalName) > 0 ? p.InstrumentStats.Where(ast => ast.TimeLineId == TimeLineID && ast.InstrumentName == instrumentalName).First().Loss : 0
            }).FirstOrDefault();

            //instrument.Profit = (instrument.Profit) * 100 / (instrument.Profit + instrument.Loss);
            instrument.Loss = -(instrument.Loss);
            // .Where(x => x.InstrumentName.Contains(instrumentalName))
            return instrument;
        }
    }
}
