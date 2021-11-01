using S2TAnalytics.Common.Helper;
using S2TAnalytics.Infrastructure.Models;
//using S2TAnalytics.Web.Helper;

using System;
using System.ComponentModel.DataAnnotations;

namespace S2TAnalytics.Web.Models
{
    public class UserViewModel
    {
        public string WebSite { get; set; }
        public string UserID { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string Name { get; set; }
        [Required,EmailAddress]
        public string EmailID { get; set; }
        public int PlanID { get; set; }
        public int RoleID { get; set; }
        public Guid OrganizationID { get; set; }
        public bool IsActive { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public string EmailToken { get; set; }
        public string EmailTokenExpiration { get; set; }
        public string ISO { get; set; }
        public string Country { get; set; }
        public string DOB { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }
        public string PhoneNumber { get; set; }

        public string PhoneCountryCode { get; set; }
        public string OTP { get; set; }
        public double MonthlyRatePerAccount { get; set; }
        

        public UserViewModel ToUserViewModel(UserModel userModel)
        {

            if (userModel == null)
                return null;

            return new UserViewModel
            {

                OrganizationID = userModel.OrganizationID,
                EmailID = userModel.EmailID,
                EmailToken = userModel.EmailToken,
                EmailTokenExpiration = userModel.EmailTokenExpiration!=null?userModel.EmailTokenExpiration.Value.ToDateTime():null,
                FirstName = userModel.FirstName,
                LastName = userModel.LastName,
                Name = userModel.Name,
                IsActive = userModel.IsActive,
                IsEmailConfirmed = userModel.IsEmailConfirmed,
                Password = userModel.Password,
                ConfirmPassword = userModel.ConfirmPassword,
                PlanID = userModel.PlanID,
                RoleID = userModel.RoleID.Decrypt(),
                UserID = userModel.UserID,
                PhoneNumber=userModel.PhoneNumber,
                PhoneCountryCode=userModel.PhoneCountryCode,
                ISO = userModel.ISO,
                Country = userModel.Country,
                DOB=userModel.DOB,
                OTP = userModel.OTP,
                MonthlyRatePerAccount = userModel.MonthlyRatePerAccount
            };
        }

        public UserModel ToUserModel(UserViewModel userViewModel)
        {

            if (userViewModel == null)
                return null;

            return new UserModel
            {

                OrganizationID = Guid.NewGuid(),
                EmailID = userViewModel.EmailID,
                EmailToken = userViewModel.EmailToken,
                EmailTokenExpiration = userViewModel.EmailTokenExpiration!=null? userViewModel.EmailTokenExpiration.ToDateTime():(DateTime?)null,
                FirstName = userViewModel.FirstName,
                LastName = userViewModel.LastName,
                Name = userViewModel.Name,
                IsActive = userViewModel.IsActive,
                IsEmailConfirmed = userViewModel.IsEmailConfirmed,
                Password = userViewModel.Password.PasswordEncrypt(),
                ConfirmPassword = Password,
                PlanID = userViewModel.PlanID,
                RoleID = userViewModel.RoleID.Encrypt(),
                UserID = userViewModel.UserID,
                PhoneNumber=userViewModel.PhoneNumber,
                PhoneCountryCode = userViewModel.PhoneCountryCode,
                ISO = userViewModel.ISO,
                Country = userViewModel.Country,
                DOB = userViewModel.DOB,
                OTP = userViewModel.OTP,
                MonthlyRatePerAccount = userViewModel.MonthlyRatePerAccount
            };

        }
    }
}