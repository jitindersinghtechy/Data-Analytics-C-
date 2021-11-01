using MongoDB.Bson;
using S2TAnalytics.Common.Helper;
using S2TAnalytics.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.Infrastructure.Interfaces
{
    public interface IUserAccountService
    {
        ServiceResponse SaveCard(ObjectId userId, CardModel cardModel);
        ServiceResponse GetDefaultData(ObjectId userId,int RoleID);
        ServiceResponse DeleteCards(List<Guid> CardIds,ObjectId userId);
        ServiceResponse GetPlanDetails(ObjectId userId);
        ServiceResponse RequestPlan(ObjectId userId, int PlanId, int TermLengthId,string promocode);
        ServiceResponse ActivateCard(string cardId, ObjectId userId, bool isActive);
        ServiceResponse SaveBillingAddress(ObjectId userId, UserBillingInfoModel userBillingInfoModel);
        ServiceResponse GetUserBillingAddresses(ObjectId userId);
        ServiceResponse ActivateBillingAddress(string addressId, ObjectId userId, bool isActive);
        ServiceResponse GetUserBillingAddressById(ObjectId userId, string addressId);
        ServiceResponse ChangeSettings(int emailNotificationSettingsId, ObjectId userId, bool hasAccess);
        ServiceResponse ChangeUserDetails(UserModel userModel, ObjectId userId);
        ServiceResponse ChangePassword(string email, string oldPassword, string newPassword);
        List<InvoiceModel> GenerateInvoice(ObjectId userId, string invoiceMonth,string invoiceYear);
        ServiceResponse EmailInvoice(List<string> files, ObjectId userId, string invoiceMonth, string invoiceYear);

        ServiceResponse GetSubscriptionPlans(ObjectId userId);

        ServiceResponse GetPromocodeDiscount(string promoCode);

        ServiceResponse PreRequestPlan(ObjectId userId,int PlanId);
    }
}
