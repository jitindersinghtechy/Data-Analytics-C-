using S2TAnalytics.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.DAL.Interfaces
{
    public interface IUnitOfWork
    {
        IRepository<User> UserRepository { get; }
        IRepository<MT4UserRequest> MT4UserRequestRepository { get; }
        IRepository<OandaAccount> OandaAccountRepository { get; }
        IRepository<AccountDetail> AccountDetailRepository { get; }
        IRepository<AccountStats> AccountStatsRepository { get; }
        IRepository<AccountTransactionHistory> AccountTransactionHistoryRepository { get; }
        IRepository<InstrumentStats> InstrumentStatsRepository { get; }
        IRepository<TimeLineMaster> TimeLineMasterRepository { get; }
        IRepository<PinnedUsers> PinnedUsersRepository { get; }
        IRepository<DailyEquity> DailyEquityRepository { get; }
        IRepository<Datasource> DatasourceRepository { get; }
        IRepository<DataSourceQueue> DataSourceQueueRepository { get; }
        IRepository<ContactUs> ContactUsRepository { get; }
        IRepository<SubscriptionPlan> SubscriptionPlanRepository { get; }
        IRepository<UserSubscriptionDeductionQueue> UserSubscriptionDeductionQueueRepository { get; }
        IRepository<CouponDetail> CouponDetailRepository { get; }
    }
}
