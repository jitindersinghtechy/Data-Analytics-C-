using S2TAnalytics.Common.Helper;
using S2TAnalytics.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.Infrastructure.Models
{
    public class UserModel
    {
        public string UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Name { get; set; }

        public string WebSite { get; set; }
        public string EmailID { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public int PlanID { get; set; }
        public string RoleID { get; set; }
        public string PhoneNumber { get; set; }

        public string PhoneCountryCode { get; set; }
        public int CountryCode { get; set; }
        public Guid OrganizationID { get; set; }
        public bool IsActive { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public string EmailToken { get; set; }
        public string ISO { get; set; }
        public string Country { get; set; }
        public string DOB { get; set; }
        public DateTime? EmailTokenExpiration { get; set; }
        public List<string> DatasourceIds { get; set; }
        public List<string> UserGroups { get; set; }
        public List<string> Datasources { get; set; }
        public string RoleName { get; set; }
        public string CommaSeperatedDatasourceIds { get; set; }
        public string CommaSeperatedPlanPermissionIds { get; set; }
        public List<RiskFreeRatesForSharpRatio> RiskFreeRates { get; set; }
        public string OTP { get; set; }
        public double MonthlyRatePerAccount { get; set; }

        public UserModel ToUserModel(User user)
        {

            if (user == null)
                return null;

            return new UserModel
            {

                OrganizationID = user.OrganizationID,
                WebSite=user.WebSite,
                EmailID = user.EmailID,
                EmailToken = user.EmailToken,
                EmailTokenExpiration = user.EmailTokenExpiration,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Name = user.Name,
                IsActive = user.IsActive,
                IsEmailConfirmed = user.IsEmailConfirmed,
                Password = user.Password,
                ConfirmPassword = user.ConfirmPassword,
                // PlanID = user.PlanID,
                RoleID = user.RoleID.Encrypt(),
                UserID = user.Id.ToString(),
                PhoneNumber = user.PhoneNumber,
                PhoneCountryCode=user.PhoneCountryCode,
                CountryCode = user.CountryCode,
                ISO = user.ISO,
                Country = user.Country,
                // DOB = user.DOB.ToString("dd/MM/yyyy")
                DOB = user.DOB.ToString("MM/dd/yyyy"),
                OTP = user.OTP,
                MonthlyRatePerAccount = user.MonthlyRatePerAccount
            };
        }
        public User ToUser(UserModel userModel)
        {

            if (userModel == null)
                return null;

            return new User
            {

                OrganizationID = userModel.OrganizationID,
                WebSite=userModel.WebSite,
                EmailID = userModel.EmailID,
                EmailToken = userModel.EmailToken,
                EmailTokenExpiration = userModel.EmailTokenExpiration,
                FirstName = userModel.FirstName,
                LastName = userModel.LastName,
                Name = userModel.Name,
                IsActive = userModel.IsActive,
                IsEmailConfirmed = userModel.IsEmailConfirmed,
                Password = userModel.Password,
                ConfirmPassword = userModel.ConfirmPassword,
                //PlanID = userModel.PlanID,
                RoleID = userModel.RoleID.Decrypt(),
                PhoneNumber = userModel.PhoneNumber,
                PhoneCountryCode = userModel.PhoneCountryCode,
                CountryCode = userModel.CountryCode,
                ISO = userModel.ISO,
                Country = userModel.Country,
                DOB = Convert.ToDateTime(userModel.DOB),
                OTP = userModel.OTP,
                MonthlyRatePerAccount = userModel.MonthlyRatePerAccount
            };

        }
        public List<UserModel> ToUserModel(List<User> users)
        {

            if (users.Count <= 0)
                return new List<UserModel>();

            return users.Select(m => new UserModel
            {

                OrganizationID = m.OrganizationID,
                WebSite=m.WebSite,
                EmailID = m.EmailID,
                EmailToken = m.EmailToken,
                EmailTokenExpiration = m.EmailTokenExpiration,
                FirstName = m.FirstName,
                LastName = m.LastName,
                Name = m.Name,
                IsActive = m.IsActive,
                IsEmailConfirmed = m.IsEmailConfirmed,
                Password = m.Password,
                ConfirmPassword = m.ConfirmPassword,
                // PlanID = m.PlanID,
                RoleID = m.RoleID.Encrypt(),
                UserID = m.Id.ToString(),
                PhoneNumber = m.PhoneNumber,
                CountryCode = m.CountryCode,
                ISO = m.ISO,
                Country = m.Country,
                DOB = m.DOB.ToString("dd/MM/yyyy"),
                OTP = m.OTP,
                MonthlyRatePerAccount = m.MonthlyRatePerAccount
            }).ToList();
        }
    }
}
