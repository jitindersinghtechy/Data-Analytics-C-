using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using S2TAnalytics.DAL.Models;
using S2TAnalytics.Infrastructure.Models;
using S2TAnalytics.Common.Helper;
using MongoDB.Bson;
using S2TAnalytics.Common.Utilities;

namespace S2TAnalytics.Infrastructure.Interfaces
{
    public interface IUserService
    {
        List<UserModel> GetAll();
        User GetUserByEmail(string email);
        ServiceResponse AddNewUser(UserModel userModel);
        ServiceResponse SetEmailConfirmed(string email, string token);
        ServiceResponse SetPassword(string email, string password);
        ServiceResponse ForgotPassword(string email);
        ServiceResponse ContactUs(ContactModel contactModel);
        ServiceResponse GetData();
        void SetSelectedDatasources(List<string> Ids, ObjectId Id);
        void UpdateSelectedDataSources(string datasourceId, ObjectId userID);
                List<ObjectId> GetSelectedDatasourceIds(ObjectId Id);
        AuthenticationResponse GenerateOTP(string email, string password);
        bool SendOTPEmail(User model);
        bool SaveEmbedWidgetPermission(ObjectId userId, int widgetId, string[] domains);
        ServiceResponse GetWidgetPermission(ObjectId userId, int widgetId);
        bool IsAuthenticatedDomain(ObjectId userId, int widgetId, string domain);
        string[] GetUserGroups(Guid organizationID);
        // User AddNewUser(User userModel);
    }
}
