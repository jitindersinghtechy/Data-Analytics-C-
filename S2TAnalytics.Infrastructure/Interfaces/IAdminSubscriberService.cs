using MongoDB.Bson;
using S2TAnalytics.Common.Enums;
using S2TAnalytics.Common.Helper;
using S2TAnalytics.DAL.Models;
using S2TAnalytics.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.Infrastructure.Interfaces
{
    public interface IAdminSubscriberService
    {
        ServiceResponse GetSubscribers(PageRecordModel pageRecordModel);
        ServiceResponse GetSubscriberList(string userID);
        ServiceResponse UpdateSubscriberContactInfo(UserModel user);
        ServiceResponse updateUserActivation(UserModel user);
        ServiceResponse UpdatePlans(string userId, int planId);
        ServiceResponse UpdateUserActivation(List<string> userIds, bool isActive = false);
        List<InvoiceModel> DownloadInvoice(string  userId, List<Guid> invoiceIds);

        bool ExtendUserTrial(List<string> userIds, int ExtendDays);
        ServiceResponse GetInvoiceHistory(string userID, PageRecordModel pageRecordModel);
        ServiceResponse GetUserRequest(string userId, PageRecordModel pageRecordModel);
        ServiceResponse UpdateSubscriberSummary(int selectedPlanId, UserModel user);
        ServiceResponse GetUserNotifications(PageRecordModel pageRecordModel);
        bool SendReminders(List<string> userIds, int reminderId);
        ServiceResponse sendOverdueReminder(UserModel user);
    }
}
