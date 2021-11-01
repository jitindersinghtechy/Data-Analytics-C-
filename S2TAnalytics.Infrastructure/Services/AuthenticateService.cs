using S2TAnalytics.DAL.Interfaces;
using S2TAnalytics.DAL.Models;
using S2TAnalytics.Common.Helper;
using S2TAnalytics.Infrastructure.Interfaces;
using S2TAnalytics.Infrastructure.Models;
using System;
using System.Linq;
using S2TAnalytics.Common.Utilities;
using S2TAnalytics.Common.Constants;
using S2TAnalytics.Common.Enums;

namespace S2TAnalytics.Infrastructure.Services
{
    public class AuthenticateService : IAuthenticateService
    {
        private readonly IUnitOfWork _unitOfWork;
        public AuthenticateService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            //InsertPlans();
        }
        public ServiceResponse CheckEmailToken(string email, string token)
        {
            //var response = new ServiceResponse();
            var IsEmailTokenas = _unitOfWork.UserRepository.GetAll().ToList();
            var IsEmailToken = _unitOfWork.UserRepository.GetAll().Any(m => m.EmailID == email && m.EmailToken != null && m.EmailTokenExpiration >= DateTime.UtcNow && m.EmailToken.Equals(token));
            if (!IsEmailToken)
            {
                return new ServiceResponse { Message = UserMessage.EmailToken_Exipred, Success = false };

            }
            return new ServiceResponse { Message = UserMessage.Email_Confirmed, Success = true };
        }
        public AuthenticationResponse AuthenticateUser(string email, string password,ref bool isExpired)
        {
            //var hasher = new PasswordHasher();
            //hasher.SaltSize = 16;
            //var hashedPasswordToStoreInDB = hasher.Encrypt(password);

            //var hasher1 = new PasswordHasher();
            //hasher1.SaltSize = 16;
            //bool areEqual = hasher1.CompareStringToHash(password, hashedPasswordToStoreInDB);

            try
            {
                var user = _unitOfWork.UserRepository.GetAll().Where(m => m.EmailID.ToLower() == email.ToLower().Trim() && m.IsEmailConfirmed).FirstOrDefault();
                if (user != null)
                {
                    var isLogin = CommonFunction.ComparePassword(password, user.Password);
                    int planId=0;

                    if (isLogin)
                    {

                        UserModel userModel = new UserModel();
                        userModel.OrganizationID = user.OrganizationID;
                        userModel.UserID = user.Id.ToString();
                        userModel.RoleID = user.RoleID.ToString();
                        userModel.FirstName = user.FirstName;
                        userModel.LastName = user.LastName;
                        userModel.CommaSeperatedDatasourceIds = "";

                        var IsActive = user.IsActive;
                        if (!IsActive)
                            return new AuthenticationResponse { Message = "Your Account is Blocked.", Data = userModel, Success = false };


                        if (user.RoleID == 2)
                        {
                            var datasourceIds = _unitOfWork.DatasourceRepository.GetAll().Where(u => u.OrganizationId == user.OrganizationID).Select(x => x.Id).ToList();
                            foreach (var d in datasourceIds)
                            {
                                userModel.CommaSeperatedDatasourceIds += d.ToString() + ",";
                            }
                            if (datasourceIds.Count > 0)
                                userModel.CommaSeperatedDatasourceIds = userModel.CommaSeperatedDatasourceIds.Substring(0, userModel.CommaSeperatedDatasourceIds.Length - 1);

                            userModel.UserGroups = _unitOfWork.AccountDetailRepository.GetAll().Where(x => x.OrganizationId == user.OrganizationID).Select(x => x.UserGroup).Distinct().ToList();
                        }
                        else
                        {
                            foreach (var d in user.DatasourceIds)
                            {
                                userModel.CommaSeperatedDatasourceIds += d.ToString() + ",";
                            }
                            if (user.DatasourceIds.Count > 0)
                                userModel.CommaSeperatedDatasourceIds = userModel.CommaSeperatedDatasourceIds.Substring(0, userModel.CommaSeperatedDatasourceIds.Length - 1);
                            userModel.UserGroups = user.UserGroups;
                        }





                        if (user.RoleID == 3)
                        {
                            var adminPlan = _unitOfWork.UserRepository.GetAll().Where(u => u.OrganizationID == user.OrganizationID && u.RoleID == 2).Single().UserPlans.Where(up => up.IsActive).Single();
                            planId = adminPlan.PlanID;
                            if (adminPlan.Days != 0)
                            {
                                var DaysLeft = adminPlan.Days - ((DateTime.Now.Date) - adminPlan.StartedDate.Date).TotalDays;
                                if (DaysLeft == 0)
                                    return new AuthenticationResponse { Message = "Your Account is InActive. Please contact your administrator.", Data = userModel, Success = false };
                            }
                        }
                        
                        var Plan = _unitOfWork.UserRepository.GetAll().Where(u => u.OrganizationID == user.OrganizationID && u.RoleID == 2).Single().UserPlans.Where(up => up.IsActive).Single();
                        if (Plan.Days != 0)
                        {
                            var DaysLeft = Plan.Days - ((DateTime.Now.Date) - Plan.StartedDate.Date).TotalDays;
                            if (DaysLeft == 0)
                            {
                                isExpired = true;
                                return new AuthenticationResponse { Message = CommonMessage.Succesfull_LoggedIn, Data = userModel, Success = true };
                            }
                        }
                        planId= planId<=0 ? Plan.PlanID : planId;
                        //Getting Plan Permissions Ids by PlanID
                        var plansPermissions = _unitOfWork.SubscriptionPlanRepository.GetAll().Where(r => r.PlanID == planId).Single().WidgetsAccess.Where(wa=>wa.HasAccess).Select(x => x.WidgetID).ToArray();
                       // var plansPermissions1 = _unitOfWork.SubscriptionPlanRepository.GetAll().Where(r => r.PlanID == Plan.PlanID && r.WidgetsAccess.Where(sa=>sa.HasAccess).Count()>0).Single().WidgetsAccess.Select(x => x.WidgetID).ToArray();
                        userModel.CommaSeperatedPlanPermissionIds = string.Join(",", plansPermissions);



                        return new AuthenticationResponse { Message = CommonMessage.Succesfull_LoggedIn, Data = userModel, Success = true };
                    }
                    return new AuthenticationResponse { Message = ErrorMessage.Invalid_Username_Password, Success = false };
                }

                return new AuthenticationResponse { Message = ErrorMessage.Invalid_Credentials, Success = false };
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public AuthenticationResponse AuthenticateSuperAdmin(string email, string password, string OTP)
        {
            try
            {
                var user = _unitOfWork.UserRepository.GetAll().Where(m => m.EmailID == email.Trim()).FirstOrDefault();
                if (user != null)
                {
                    UserModel userModel = new UserModel();
                    userModel.OrganizationID = user.OrganizationID;
                    userModel.UserID = user.Id.ToString();
                    userModel.RoleID = user.RoleID.ToString();
                    userModel.FirstName = user.FirstName;
                    userModel.LastName = user.LastName;

                    //userModel.CommaSeperatedDatasourceIds = "";
                    //if (user.RoleID == 1)
                    //{
                    //    var datasourceIds = _unitOfWork.DatasourceRepository.GetAll().Where(u => u.OrganizationId == user.OrganizationID).Select(x => x.Id).ToList();
                    //    foreach (var d in datasourceIds)
                    //    {
                    //        userModel.CommaSeperatedDatasourceIds += d.ToString() + ",";
                    //    }
                    //    if (datasourceIds.Count > 0)
                    //        userModel.CommaSeperatedDatasourceIds = userModel.CommaSeperatedDatasourceIds.Substring(0, userModel.CommaSeperatedDatasourceIds.Length - 1);
                    //}
                    //else
                    //{
                    //    foreach (var d in user.DatasourceIds)
                    //    {
                    //        userModel.CommaSeperatedDatasourceIds += d.ToString() + ",";
                    //    }
                    //    if (user.DatasourceIds.Count > 0)
                    //        userModel.CommaSeperatedDatasourceIds = userModel.CommaSeperatedDatasourceIds.Substring(0, userModel.CommaSeperatedDatasourceIds.Length - 1);
                    //}
                    var isLogin = CommonFunction.ComparePassword(password, user.Password);
                    var otp = _unitOfWork.UserRepository.GetAll().Where(m => m.EmailID == email.Trim()).Select(x => x.OTP).FirstOrDefault();

                    if (isLogin && string.Compare(otp, OTP) == 0)
                        return new AuthenticationResponse { Message = CommonMessage.Succesfull_LoggedIn, Data = userModel, Success = true };

                    return new AuthenticationResponse { Message = ErrorMessage.Invalid_Username_Password, Success = false };
                }

                return new AuthenticationResponse { Message = ErrorMessage.Invalid_Credentials, Success = false };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void EmptyOTP(string email, string password)
        {
            try
            {
                var user = _unitOfWork.UserRepository.GetAll().Where(m => m.EmailID == email.Trim()).FirstOrDefault();
                if (user != null)
                {
                    user.OTP = null;
                    _unitOfWork.UserRepository.Update(user);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void InsertPlans()
        {
            var plan = new SubscriptionPlan();
            plan.PlanID = 1;
            plan.Name = "SILVER";
            plan.Price = 4;
            plan.Days = 0;
            plan.InfrastructureCost = 200;


            var PlanList = Enum.GetValues(typeof(PlansEnum)).Cast<int>().ToArray();

            foreach (var p in PlanList)
            {
                plan.WidgetsAccess.Add(new WidgetAccess()
                {
                    HasAccess = true,
                    WidgetID = Convert.ToInt32(p)
                });
            }
            PlanTermLength termLength = new PlanTermLength();
            termLength.PlanTermLengthId = 1;
            termLength.Percent = 100;
            termLength.Month = 1;
            plan.PlanTermLength.Add(termLength);

            termLength = new PlanTermLength();
            termLength.PlanTermLengthId = 2;
            termLength.Percent = 75;
            termLength.Month = 12;
            plan.PlanTermLength.Add(termLength);

           


            _unitOfWork.SubscriptionPlanRepository.Add(plan);


            plan = new SubscriptionPlan();
            plan.PlanID = 2;
            plan.Name = "GOLD";
            plan.Price = 6;
            plan.Days = 15;
            plan.InfrastructureCost = 300;

            PlanList = Enum.GetValues(typeof(PlansEnum)).Cast<int>().ToArray();
            foreach (var p in PlanList)
            {
                plan.WidgetsAccess.Add(new WidgetAccess()
                {
                    HasAccess = true,
                    WidgetID = Convert.ToInt32(p)
                });
            }

            termLength = new PlanTermLength();
            termLength.PlanTermLengthId = 1;
            termLength.Percent = 100;
            termLength.Month = 1;
            plan.PlanTermLength.Add(termLength);

            termLength = new PlanTermLength();
            termLength.PlanTermLengthId = 2;
            termLength.Percent = 75;
            termLength.Month = 12;
            plan.PlanTermLength.Add(termLength);

           

            _unitOfWork.SubscriptionPlanRepository.Add(plan);


            plan = new SubscriptionPlan();
            plan.PlanID = 3;
            plan.Name = "ULTIMATE";
            plan.Price = 8;
            plan.Days = 0;
            plan.InfrastructureCost = 500;

            PlanList = Enum.GetValues(typeof(PlansEnum)).Cast<int>().ToArray();
            foreach (var p in PlanList)
            {
                plan.WidgetsAccess.Add(new WidgetAccess()
                {
                    HasAccess = true,
                    WidgetID = Convert.ToInt32(p)
                });
            }
            termLength = new PlanTermLength();
            termLength.PlanTermLengthId = 1;
            termLength.Percent = 100;
            termLength.Month = 1;
            plan.PlanTermLength.Add(termLength);

            termLength = new PlanTermLength();
            termLength.PlanTermLengthId = 2;
            termLength.Percent = 75;
            termLength.Month = 12;
            plan.PlanTermLength.Add(termLength);

           

            _unitOfWork.SubscriptionPlanRepository.Add(plan);

            


        }
        //public bool IsEmailConfirmed(UserModel userModel)
        //{
        //    var user = _unitOfWork.UserRepository.GetAll().Where(m => m.EmailID == userModel.EmailID).Select(m=>m.IsEmailConfirmed).Any();
        //   // user.IsEmailConfirmed = true;
        //    // _unitOfWork.UserRepository.Update(user);
        //    return user;
        //}
    }
}
