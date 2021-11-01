using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.Common.Helper
{
    public static class CommonMessage
    {
        public const string Updated = "Updated Successfully";
        public const string Saved = "Saved Successfully";
        public const string Deleted = "Deleted Successfully";
        public const string SentEmail = "An email has been sent to your email Id to reset the password";
        public const string Set_Password = "Password has been set successfully.";
        public const string Succesfull_LoggedIn = "Succesfull LoggedIn";
        public const string Role_Exist = "Role Name already exist.";
        public const string Email_Exist = "Email already exist.";
        public const string Success = "Success";
        public const string Something_Wrong = "Something went wrong,Please contact administrator";

        public const string SentOTP = "An OTP has been sent to your email Id to Login";
    }
    public static class ErrorMessage
    {
        public const string Invalid_Credentials = "Invalid Credentials.";
        public const string Invalid_Username_Password = "Invalid Username or Password.";
        
        public const string User_Is_Inactive = "User is inactive";
        public const string Failed_Attempt_Message = "Invalid credentials.You have {count} more attempt(s) before your account gets locked out.";
        public const string Reached_Max_Login_Attempts = "Your account has been locked out for {count} minutes due to multiple failed login attempts.";
        public const string Try_Again = "Try Again";
        public const string Link_Expired = "This link has been expired or does not exists.";
        public const string User_Not_Exist = "Username or email does not exist";
        public const string Username_Already_Exist = "Username already Exists";
        public const string Email_Already_Exist = "Email already Exists";
        public const string User_Cannot_be_Saved = "Some Error Occur While Saving  User.";
        public const string Invalid_Username = "Invalid e-mail address";
        public const string Invalid_Token = "Invalid token";


    }
    public static class UserMessage
    {

        public const string EmailToken_Exipred = "Link has been expired";
        public const string Email_NotConfirmed = "Email has not confirmed";
        public const string Email_Confirmed = "Email has confirmed";
        public const string Email_Already_Confirmed = "Email already has confirmed";
        public const string Set_Password = "Password set successfully.";
        public const string Password_Error = "Error setting password.";
        public const string Forgot_Mail_Send = "Mail has been sent to your email ID. Please check your email for further instructions.";
    }

    public static class AccountDetailMessage {
        public const string UserPinnedSuccessfully = "User Pinned Successfully.";
        public const string SomethingWentWrong = "Something Went Wrong.";
        public const string UserUnpinnedSuccessfully = "User Unpinned Successfully.";
        public const string NoPinnedUserFound = "No Pinned User Found.";
        public const string UserExcludeSuccessfully = "User Excluded Successfully.";
    }
    public static class SuperAdminMessage
    {
        public const string UserActivatedSuccessfully = "User Activated Successfully.";
        public const string UserDeactivatedSuccessfully = "User Deactivated Successfully.";
        public const string ReminderMailSuccessfully = "Mail Sent Successfully.";
        public const string SomethingWentWrong= "Something Went Wrong";
    }

}
