using MongoDB.Bson;
using S2TAnalytics.Common.Helper;
using S2TAnalytics.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.Infrastructure.Models
{
    public class AccountDetailModel
    {
        public AccountDetailModel()
        {
            this.AccountStats = new List<AccountStatsModel>();
            this.AccountTransactionHistories = new List<AccountTransactionHistoryModel>();
            this.InstrumentStatsModel = new List<InstrumentStatsModel>();
        }
        public string AccountDetailId { get; set; }
        public Guid OrganizationId { get; set; }
        public string DataSourceId { get; set; }
        public string AccountNumber { get; set; }
        public string Name { get; set; }
        public double Balance { get; set; }
        public int Leverage { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public string City { get; set; }
        public double CityLong { get; set; }
        public double CityLat { get; set; }
        public string UserGroup { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Comment { get; set; }
        public string ActiveSince { get; set; }
        public string LastActiveOn { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public string UpdatedOn { get; set; }
        public string Status { get; set; }
        public double Nav { get; set; }
        public bool IsExclude { get; set; }
        public List<AccountStatsModel> AccountStats { get; set; }
        public List<AccountTransactionHistoryModel> AccountTransactionHistories { get; set; }
        public List<InstrumentStatsModel> InstrumentStatsModel { get; set; }


        #region AccountStats Mapping
        public List<AccountDetail> ToAccountDetail(List<AccountDetailModel> accountDetailModels)
        {
            if (accountDetailModels == null && accountDetailModels.Count <= 0)
                return new List<AccountDetail>();
            return accountDetailModels.Select(accountDetailModel => new AccountDetail
            {
                OrganizationId = accountDetailModel.OrganizationId,
                DataSourceId = ObjectId.Parse(accountDetailModel.DataSourceId),
                AccountNumber = accountDetailModel.AccountNumber,
                Name = accountDetailModel.Name,
                Balance = accountDetailModel.Balance,
                Leverage = accountDetailModel.Leverage,
                Country = accountDetailModel.Country,
                CountryCode = accountDetailModel.CountryCode,
                City = accountDetailModel.City,
                CityLong= accountDetailModel.CityLong,
                CityLat= accountDetailModel.CityLat,
                Address = accountDetailModel.Address,
                Email = accountDetailModel.Email,
                Phone = accountDetailModel.Phone,
                Comment = accountDetailModel.Comment,
                UserGroup = accountDetailModel.UserGroup,
                ActiveSince = accountDetailModel.ActiveSince.ToString(),
                LastActiveOn = accountDetailModel.LastActiveOn.ToString(),
                CreatedBy = accountDetailModel.CreatedBy,
                CreatedOn = accountDetailModel.CreatedOn.ToString(),
                UpdatedBy = accountDetailModel.UpdatedBy,
                UpdatedOn = accountDetailModel.UpdatedOn.ToString(),
                Status = accountDetailModel.Status,
                AccountStats = new AccountStatsModel().ToAccountDailyStats(accountDetailModel.AccountStats),
                //  AccountTransactionHistories = new AccountTransactionHistoryModel().ToAccountTransactionHistory(accountDetailModel.AccountTransactionHistories)
            }).ToList();
        }

        public AccountDetail ToAccountDetail(AccountDetailModel accountDetailModel)
        {
            if (accountDetailModel == null)
                return new AccountDetail();
            return new AccountDetail
            {
                OrganizationId = accountDetailModel.OrganizationId,
                DataSourceId = ObjectId.Parse(accountDetailModel.DataSourceId),
                AccountNumber = accountDetailModel.AccountNumber,
                Name = accountDetailModel.Name,
                Balance = accountDetailModel.Balance,
                Leverage = accountDetailModel.Leverage,
                Country = accountDetailModel.Country,
                CountryCode = accountDetailModel.CountryCode,
                City = accountDetailModel.City,
                CityLong = accountDetailModel.CityLong,
                CityLat = accountDetailModel.CityLat,
                Address = accountDetailModel.Address,
                Email = accountDetailModel.Email,
                Phone = accountDetailModel.Phone,
                Comment = accountDetailModel.Comment,
                UserGroup = accountDetailModel.UserGroup,
                ActiveSince = accountDetailModel.ActiveSince.ToString(),
                LastActiveOn = accountDetailModel.LastActiveOn.ToString(),               
                CreatedBy = accountDetailModel.CreatedBy,
                CreatedOn = accountDetailModel.CreatedOn.ToString(),
                UpdatedBy = accountDetailModel.UpdatedBy,
                UpdatedOn = accountDetailModel.UpdatedOn.ToString(),
                Status = accountDetailModel.Status,
                AccountStats = new AccountStatsModel().ToAccountDailyStats(accountDetailModel.AccountStats),
                //  AccountTransactionHistories = new AccountTransactionHistoryModel().ToAccountTransactionHistory(accountDetailModel.AccountTransactionHistories)
            };
        }

        public List<AccountDetailModel> ToAccountDetailModel(List<AccountDetail> accountDetails)
        {
            if (accountDetails == null && accountDetails.Count <= 0)
                return new List<AccountDetailModel>();
            return accountDetails.Select(accountDetail => new AccountDetailModel
            {
                AccountDetailId = accountDetail.Id.ToString(),
                OrganizationId = accountDetail.OrganizationId,
                DataSourceId = accountDetail.DataSourceId.ToString(),
                AccountNumber = accountDetail.AccountNumber,
                Name = accountDetail.Name,
                Balance = accountDetail.Balance,
                Leverage = accountDetail.Leverage,
                Country = accountDetail.Country,
                CountryCode = accountDetail.CountryCode,
                City = accountDetail.City,
                CityLong = accountDetail.CityLong,
                CityLat = accountDetail.CityLat,
                Address = accountDetail.Address,
                Email = accountDetail.Email,
                Phone = accountDetail.Phone,
                Comment = accountDetail.Comment,
                UserGroup = accountDetail.UserGroup,
                ActiveSince = accountDetail.ActiveSince,
                LastActiveOn =accountDetail.LastActiveOn,
                CreatedBy = accountDetail.CreatedBy,
                // CreatedOn = Convert.ToDateTime(accountDetail.CreatedOn),
                UpdatedBy = accountDetail.UpdatedBy,
                // UpdatedOn = Convert.ToDateTime(accountDetail.UpdatedOn),
                Status = accountDetail.Status,
                AccountStats = new AccountStatsModel().ToAccountDailyStatsModel(accountDetail.AccountStats),
                // AccountTransactionHistories = new AccountTransactionHistoryModel().ToAccountTransactionHistoryModel(accountDetail.AccountTransactionHistories)
            }).ToList();
        }

        public AccountDetailModel ToAccountDetailModel(AccountDetail accountDetail)
        {
            if (accountDetail == null)
                return new AccountDetailModel();
            return new AccountDetailModel
            {
                AccountDetailId = accountDetail.Id.ToString(),
                OrganizationId = accountDetail.OrganizationId,
                DataSourceId = accountDetail.DataSourceId.ToString(),
                AccountNumber = accountDetail.AccountNumber,
                Name = accountDetail.Name,
                Balance = accountDetail.Balance,
                Leverage = accountDetail.Leverage,
                Country = accountDetail.Country,
                CountryCode = accountDetail.CountryCode,
                City = accountDetail.City,
                CityLong = accountDetail.CityLong,
                CityLat = accountDetail.CityLat,
                Address = accountDetail.Address,
                Email = accountDetail.Email,
                Phone = accountDetail.Phone,
                Comment = accountDetail.Comment,
                UserGroup = accountDetail.UserGroup,
                ActiveSince = accountDetail.ActiveSince,
                LastActiveOn = accountDetail.LastActiveOn,
                CreatedBy = accountDetail.CreatedBy,
                ////   CreatedOn = Convert.ToDateTime(accountDetail.CreatedOn),
                UpdatedBy = accountDetail.UpdatedBy,
                //UpdatedOn = Convert.ToDateTime(accountDetail.UpdatedOn),
                Status = accountDetail.Status,
                AccountStats = new AccountStatsModel().ToAccountDailyStatsModel(accountDetail.AccountStats),
                //  AccountTransactionHistories = new AccountTransactionHistoryModel().ToAccountTransactionHistoryModel(accountDetail.AccountTransactionHistories)
            };
        }
        #endregion

        public List<AccountDetailModel> ToAccountDetailWithInstrumentStatsModel(List<AccountDetail> accountDetails)
        {
            if (accountDetails == null && accountDetails.Count <= 0)
                return new List<AccountDetailModel>();
            return accountDetails.Select(accountDetail => new AccountDetailModel
            {
                AccountDetailId = accountDetail.Id.ToString(),
                OrganizationId = accountDetail.OrganizationId,
                DataSourceId = accountDetail.DataSourceId.ToString(),
                AccountNumber = accountDetail.AccountNumber,
                Name = accountDetail.Name,
                Balance = accountDetail.Balance,
                Leverage = accountDetail.Leverage,
                Country = accountDetail.Country,
                CountryCode = accountDetail.CountryCode,
                City = accountDetail.City,
                CityLong = accountDetail.CityLong,
                CityLat = accountDetail.CityLat,
                Address = accountDetail.Address,
                Email = accountDetail.Email,
                Phone = accountDetail.Phone,
                Comment = accountDetail.Comment,
                ActiveSince = accountDetail.ActiveSince,
                LastActiveOn = accountDetail.LastActiveOn,
                UserGroup = accountDetail.UserGroup,
                CreatedBy = accountDetail.CreatedBy,
                UpdatedBy = accountDetail.UpdatedBy,
                Status = accountDetail.Status,
                InstrumentStatsModel = new InstrumentStatsModel().ToInstrumentStatsModel(accountDetail.InstrumentStats.ToList()),
                //AccountDailyStats = new AccountDailyStatsModel().ToAccountDailyStatsModel(accountDetail.AccountDailyStats),
                // AccountTransactionHistories = new AccountTransactionHistoryModel().ToAccountTransactionHistoryModel(accountDetail.AccountTransactionHistories)
            }).ToList();
        }

        public AccountDetailModel ToAccountDetailWithInstrumentStatsModel(AccountDetail accountDetails)
        {
            if (accountDetails == null)
                return new AccountDetailModel();
            return new AccountDetailModel
            {
                AccountDetailId = accountDetails.Id.ToString(),
                OrganizationId = accountDetails.OrganizationId,
                DataSourceId = accountDetails.DataSourceId.ToString(),
                AccountNumber = accountDetails.AccountNumber,
                Name = accountDetails.Name,
                Balance = accountDetails.Balance,
                Leverage = accountDetails.Leverage,
                Country = accountDetails.Country,
                CountryCode = accountDetails.CountryCode,
                City = accountDetails.City,
                CityLong = accountDetails.CityLong,
                CityLat = accountDetails.CityLat,
                Address = accountDetails.Address,
                ActiveSince = accountDetails.ActiveSince,
                LastActiveOn = accountDetails.LastActiveOn,
                Email = accountDetails.Email,
                Phone = accountDetails.Phone,
                Comment = accountDetails.Comment,
                UserGroup = accountDetails.UserGroup,
                CreatedBy = accountDetails.CreatedBy,
                UpdatedBy = accountDetails.UpdatedBy,
                Status = accountDetails.Status,
                InstrumentStatsModel = new InstrumentStatsModel().ToInstrumentStatsModel(accountDetails.InstrumentStats.ToList()),
                //AccountDailyStats = new AccountDailyStatsModel().ToAccountDailyStatsModel(accountDetail.AccountDailyStats),
                // AccountTransactionHistories = new AccountTransactionHistoryModel().ToAccountTransactionHistoryModel(accountDetail.AccountTransactionHistories)
            };
        }


        public List<AccountDetailModel> ToComparisonData(List<AccountDetail> accountDetails)
        {
            if (accountDetails == null && accountDetails.Count == 0)
                return new List<AccountDetailModel>();
            return accountDetails.Select(accountDetail => new AccountDetailModel
            {
                AccountDetailId = accountDetail.Id.ToString(),
                OrganizationId = accountDetail.OrganizationId,
                DataSourceId = accountDetail.DataSourceId.ToString(),
                AccountNumber = accountDetail.AccountNumber,
                Name = accountDetail.Name,
                Balance = accountDetail.Balance,
                Leverage = accountDetail.Leverage,
                Country = accountDetail.Country,
                CountryCode = accountDetail.CountryCode,
                City = accountDetail.City,
                CityLong = accountDetail.CityLong,
                CityLat = accountDetail.CityLat,
                ActiveSince = accountDetail.ActiveSince,
                LastActiveOn = accountDetail.LastActiveOn,
                Address = accountDetail.Address,
                Email = accountDetail.Email,
                Phone = accountDetail.Phone,
                Comment = accountDetail.Comment,
                UserGroup = accountDetail.UserGroup,
                CreatedBy = accountDetail.CreatedBy,
                UpdatedBy = accountDetail.UpdatedBy,
                Status = accountDetail.Status,
                AccountStats = new AccountStatsModel().ToAccountDailyStatsModel(accountDetail.AccountStats)
            }).ToList();
        }
    }
}
