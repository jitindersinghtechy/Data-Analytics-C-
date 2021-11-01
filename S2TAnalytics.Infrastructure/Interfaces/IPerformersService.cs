using S2TAnalytics.Common.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using S2TAnalytics.Infrastructure.Models;

namespace S2TAnalytics.Infrastructure.Interfaces
{
    public interface IPerformersService
    {
        ServiceResponse GetAccountDetails(PageRecordModel pageRecordModel, string[] assignedUserGroups);
        int GetAccountDetailsCount(PageRecordModel pageRecordModel, string[] assignedUserGroups);
        ServiceResponse GetTop5Performers(PageRecordModel pageRecordModel, string[] assignedUserGroups);
        ServiceResponse UpdatePinnedUsers(List<string> selectedAccountDetailsIds, Guid organizationID, ObjectId userID);
        ServiceResponse GetPinnedUsers(PageRecordModel pageRecordModel);
        ServiceResponse GetTopFivePinnedUsers(PageRecordModel pageRecordModel);
        ServiceResponse UnpinUser(string accountDetailId, ObjectId userID);
        //ServiceResponse GetUserDetails(string accountId);
        ServiceResponse GetUserDetails(PageRecordModel pageRecordModel, ObjectId userID);

        //--instrumental data user detail
        ServiceResponse GetUserDetailInstrumental(int records, PageRecordModel pageRecordModel, ObjectId userID);
        ServiceResponse UpdateExcludeUsers(List<string> selectedAccountDetailsIds, Guid organizationID, ObjectId userID);
        List<PerformersModel> GetAccountsForExport(PageRecordModel pageRecordModel, string[] assignedUserGroups);

        InstrumentStatsModel GetProfitLossData(string instrumentalName, string timeLineId, string UserID, string[] assignedUserGroups);
    }
}
