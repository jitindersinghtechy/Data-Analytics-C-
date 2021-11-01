using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.DAL.Models
{
    public class User : BaseEntity
    {

        public User()
        {
            this.DatasourceIds = new List<ObjectId>();
            this.Notifications = new List<Notification>();
            this.UserPlans = new List<UserPlan>();
            this.UserCards = new List<UserCard>();
            this.UserGroups = new List<string>();
            this.UserEmailNotificationSettings = new List<Models.UserEmailNotificationSettings>();
            this.EmbedWidgetPermissions = new List<UserEmbedWidgetPermission>();
            this.UserRequests = new List<UserRequest>();
        }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Name
        {
            get
            {
                return FirstName + " " + LastName;
            }
            set { }
        }

        public string WebSite { get; set; }

        public string EmailID { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        //public int PlanID { get; set; }
        public int RoleID { get; set; }
        public Guid OrganizationID { get; set; }
        public bool IsActive { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public string EmailToken { get; set; }
        public string ISO { get; set; }
        public string Country { get; set; }
        public DateTime DOB { get; set; }
        public DateTime? EmailTokenExpiration { get; set; }
        public List<ObjectId> DatasourceIds { get; set; }
        public List<ObjectId> SelectedDatasourceIds { get; set; }
        public List<string> UserGroups { get; set; }
        public List<Notification> Notifications { get; set; }
        public List<UserCard> UserCards { get; set; }
        public List<UserPlan> UserPlans { get; set; }
        public List<UserCredit> UserCredits { get; set; }
        public List<UserEmailNotificationSettings> UserEmailNotificationSettings { get; set; }
        public List<UserBillingInfo> UserBillingInfos { get; set; }
        public List<UserInvoiceHistory> UserInvoiceHistory { get; set; }
        public string PhoneNumber { get; set; }
        public string PhoneCountryCode { get; set; }
        public int CountryCode { get; set; }
        public List<RiskFreeRatesForSharpRatio> RiskFreeRates { get; set; }
        public List<UserEmbedWidgetPermission> EmbedWidgetPermissions { get; set; }
        public List<UserRequest> UserRequests { get; set; }
        public string OTP { get; set; }
        public double MonthlyRatePerAccount { get; set; }
    }
}
