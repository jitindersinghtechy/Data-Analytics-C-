using S2TAnalytics.DAL.Interfaces;
using S2TAnalytics.DAL.Models;
using S2TAnalytics.DAL.Repository;
using MongoDB.Driver;
using System.Configuration;
using System.Collections.Generic;

namespace S2TAnalytics.DAL.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoClient _client;

        public UnitOfWork(string connectionString, string dbName)
        {
            _client = new MongoClient(connectionString + "/" + dbName);
            _database = _client.GetDatabase(dbName);
            //CreateIndexes();
        }


        private void CreateIndexes()
        {
            List<CreateIndexModel<User>> userIndexModels = new List<CreateIndexModel<User>>();
            userIndexModels.Add(new CreateIndexModel<User>(Builders<User>.IndexKeys.Ascending(field => field.EmailID), new CreateIndexOptions<User>() { Name = "User_Email", Unique = true }));
            userIndexModels.Add(new CreateIndexModel<User>(Builders<User>.IndexKeys.Ascending(field => field.OrganizationID), new CreateIndexOptions<User>() { Name = "User_OrganizationID" }));
            userIndexModels.Add(new CreateIndexModel<User>(Builders<User>.IndexKeys.Ascending(field => field.RoleID), new CreateIndexOptions<User>() { Name = "User_RoleID" }));
            userIndexModels.Add(new CreateIndexModel<User>(Builders<User>.IndexKeys.Ascending(field => field.DatasourceIds), new CreateIndexOptions<User>() { Name = "User_DatasourceIDs" }));
            userIndexModels.Add(new CreateIndexModel<User>(Builders<User>.IndexKeys.Ascending(field => field.SelectedDatasourceIds), new CreateIndexOptions<User>() { Name = "User_SelectedDatasourceIDs" }));
            _database.GetCollection<User>("User").Indexes.CreateMany(userIndexModels);

            List<CreateIndexModel<AccountDetail>> accountIndexModels = new List<CreateIndexModel<AccountDetail>>();
            accountIndexModels.Add(new CreateIndexModel<AccountDetail>(Builders<AccountDetail>.IndexKeys.Ascending(field => field.OrganizationId), new CreateIndexOptions<AccountDetail>() { Name = "Account_OrganizationID" }));
            accountIndexModels.Add(new CreateIndexModel<AccountDetail>(Builders<AccountDetail>.IndexKeys.Ascending(field => field.AccountNumber), new CreateIndexOptions<AccountDetail>() { Name = "Account_AccountNumber" }));
            accountIndexModels.Add(new CreateIndexModel<AccountDetail>(Builders<AccountDetail>.IndexKeys.Ascending(field => field.Country), new CreateIndexOptions<AccountDetail>() { Name = "Account_Country" }));
            accountIndexModels.Add(new CreateIndexModel<AccountDetail>(Builders<AccountDetail>.IndexKeys.Ascending(field => field.City), new CreateIndexOptions<AccountDetail>() { Name = "Account_City" }));
            accountIndexModels.Add(new CreateIndexModel<AccountDetail>(Builders<AccountDetail>.IndexKeys.Ascending(field => field.IsExclude), new CreateIndexOptions<AccountDetail>() { Name = "Account_IsExclude" }));
            accountIndexModels.Add(new CreateIndexModel<AccountDetail>(Builders<AccountDetail>.IndexKeys.Ascending(field => field.UserGroup), new CreateIndexOptions<AccountDetail>() { Name = "Account_UserGroup" }));
            accountIndexModels.Add(new CreateIndexModel<AccountDetail>(Builders<AccountDetail>.IndexKeys.Ascending(field => field.AccountStats), new CreateIndexOptions<AccountDetail>() { Name = "Account_Stats" }));
            accountIndexModels.Add(new CreateIndexModel<AccountDetail>(Builders<AccountDetail>.IndexKeys.Ascending(field => field.InstrumentStats), new CreateIndexOptions<AccountDetail>() { Name = "Account_InstrumentStats" }));
            accountIndexModels.Add(new CreateIndexModel<AccountDetail>(Builders<AccountDetail>.IndexKeys.Ascending(field => field.AccountTransactionHistories), new CreateIndexOptions<AccountDetail>() { Name = "Account_AccountTransactionHistories" }));

            _database.GetCollection<AccountDetail>("AccountDetail").Indexes.CreateMany(accountIndexModels);

            List<CreateIndexModel<DailyEquity>> dailyEquityIndexModels = new List<CreateIndexModel<DailyEquity>>();
            dailyEquityIndexModels.Add(new CreateIndexModel<DailyEquity>(Builders<DailyEquity>.IndexKeys.Ascending(field => field.AccountNumber), new CreateIndexOptions<DailyEquity>() { Name = "DailyEquity_AccountNumber" }));
            dailyEquityIndexModels.Add(new CreateIndexModel<DailyEquity>(Builders<DailyEquity>.IndexKeys.Ascending(field => field.OrganizationId), new CreateIndexOptions<DailyEquity>() { Name = "DailyEquity_OrganizationID" }));
            _database.GetCollection<DailyEquity>("DailyEquity").Indexes.CreateMany(dailyEquityIndexModels);

            List<CreateIndexModel<MT4UserRequest>> mt4RequestsIndexModels = new List<CreateIndexModel<MT4UserRequest>>();
            mt4RequestsIndexModels.Add(new CreateIndexModel<MT4UserRequest>(Builders<MT4UserRequest>.IndexKeys.Ascending(field => field.OrganizationId), new CreateIndexOptions<MT4UserRequest>() { Name = "MT4Requests_OrganizationID" }));
            mt4RequestsIndexModels.Add(new CreateIndexModel<MT4UserRequest>(Builders<MT4UserRequest>.IndexKeys.Ascending(field => field.Login), new CreateIndexOptions<MT4UserRequest>() { Name = "MT4Requests_Login" }));
            _database.GetCollection<MT4UserRequest>("MT4UserRequest").Indexes.CreateMany(mt4RequestsIndexModels);

            List<CreateIndexModel<Datasource>> datasourceIndexModels = new List<CreateIndexModel<Datasource>>();
            datasourceIndexModels.Add(new CreateIndexModel<Datasource>(Builders<Datasource>.IndexKeys.Ascending(field => field.OrganizationId), new CreateIndexOptions<Datasource>() { Name = "Datasource_OrganizationID" }));
            _database.GetCollection<Datasource>("Datasource").Indexes.CreateMany(datasourceIndexModels);

            List<CreateIndexModel<DataSourceQueue>> datasourceQueueIndexModels = new List<CreateIndexModel<DataSourceQueue>>();
            datasourceQueueIndexModels.Add(new CreateIndexModel<DataSourceQueue>(Builders<DataSourceQueue>.IndexKeys.Ascending(field => field.OrganizationId), new CreateIndexOptions<DataSourceQueue>() { Name = "Datasource_OrganizationID" }));
            _database.GetCollection<DataSourceQueue>("DataSourceQueue").Indexes.CreateMany(datasourceQueueIndexModels);

        }
        public IRepository<User> UserRepository
        {
            get
            {
                return new BaseRepository<User>(_database);
            }
        }

        public IRepository<MT4UserRequest> MT4UserRequestRepository
        {
            get
            {
                return new BaseRepository<MT4UserRequest>(_database);
            }
        }

        public IRepository<OandaAccount> OandaAccountRepository
        {
            get
            {
                return new BaseRepository<OandaAccount>(_database);
            }
        }
        public IRepository<AccountDetail> AccountDetailRepository
        {
            get
            {
                return new BaseRepository<AccountDetail>(_database);
            }
        }
        public IRepository<AccountStats> AccountStatsRepository
        {
            get
            {
                return new BaseRepository<AccountStats>(_database);
            }
        }
        public IRepository<AccountTransactionHistory> AccountTransactionHistoryRepository
        {
            get
            {
                return new BaseRepository<AccountTransactionHistory>(_database);
            }
        }
        public IRepository<InstrumentStats> InstrumentStatsRepository
        {
            get
            {
                return new BaseRepository<InstrumentStats>(_database);
            }
        }

        public IRepository<TimeLineMaster> TimeLineMasterRepository
        {
            get
            {
                return new BaseRepository<TimeLineMaster>(_database);
            }
        }

        public IRepository<PinnedUsers> PinnedUsersRepository
        {
            get
            {
                return new BaseRepository<PinnedUsers>(_database);
            }
        }
        public IRepository<DailyEquity> DailyEquityRepository
        {
            get
            {
                return new BaseRepository<DailyEquity>(_database);
            }
        }

        public IRepository<Datasource> DatasourceRepository
        {
            get
            {
                return new BaseRepository<Datasource>(_database);
            }
        }

        public IRepository<DataSourceQueue> DataSourceQueueRepository
        {
            get
            {
                return new BaseRepository<DataSourceQueue>(_database);
            }
        }
        public IRepository<ContactUs> ContactUsRepository
        {
            get
            {
                return new BaseRepository<ContactUs>(_database);
            }
        }

        public IRepository<SubscriptionPlan> SubscriptionPlanRepository
        {
            get
            {
                return new BaseRepository<SubscriptionPlan>(_database);
            }
        }

        public IRepository<UserSubscriptionDeductionQueue> UserSubscriptionDeductionQueueRepository
        {
            get
            {
                return new BaseRepository<UserSubscriptionDeductionQueue>(_database);
            }
        }
        public IRepository<CouponDetail> CouponDetailRepository
        {
            get
            {
                return new BaseRepository<CouponDetail>(_database);
            }
        }
    }

}
