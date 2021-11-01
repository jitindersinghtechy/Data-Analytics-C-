using MongoDB.Bson;
using S2TAnalytics.Common.Enums;
using S2TAnalytics.Common.Helper;
using S2TAnalytics.DAL.Interfaces;
using S2TAnalytics.DAL.Models;
using S2TAnalytics.Infrastructure.Interfaces;
using S2TAnalytics.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.Infrastructure.Services
{
    public class AdminSubscriberService : IAdminSubscriberService
    {
        public readonly IUnitOfWork _unitOfWork;
        public AdminSubscriberService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public ServiceResponse GetSubscribers(PageRecordModel pageRecordModel)
        {
            var planFilter = "";
            var paymentFilter = "";
            var nameFilter = "";
            foreach (var filter in pageRecordModel.Filters)
            {
                if (filter.Key == "PlanFilter")
                    planFilter = pageRecordModel.Filters.Single(x => x.Key == "PlanFilter").Value;
                if (filter.Key == "PaymentFilter")
                    paymentFilter = pageRecordModel.Filters.Single(x => x.Key == "PaymentFilter").Value;
                if (filter.Key == "Name")
                    nameFilter = pageRecordModel.Filters.Single(x => x.Key == "Name").Value;
            }

            var plans = _unitOfWork.SubscriptionPlanRepository.GetAll().ToList();
            var subscribersList = new List<AdminSubscriberModel>();
            IEnumerable<AdminSubscriberModel> subscribers;
            if (!string.IsNullOrEmpty(planFilter))
            {
                subscribers = _unitOfWork.UserRepository.GetAll().Where(x => x.RoleID == 2 && x.Name.ToLower().Contains(nameFilter.ToLower()) && x.UserPlans.Any(y => y.IsActive == true && y.PlanID == Convert.ToInt32(planFilter))).ToList()
                   .Select(a => new AdminSubscriberModel()
                   {
                       UserID = a.Id,
                       Name = a.Name,
                       EmailID = a.EmailID,
                       PlanName = plans.Where(x => x.PlanID == a.UserPlans.Where(up => up.IsActive == true).Select(up => up.PlanID).FirstOrDefault()).Select(x => x.Name).FirstOrDefault(),
                       IsTrial = a.UserPlans.Where(up => up.IsActive == true).Select(up => up.Days).First() > 0 ? true : false,
                       PhoneNumber = a.PhoneNumber,
                       ReportUser = _unitOfWork.UserRepository.GetAll().Where(x => x.RoleID == 3 && x.OrganizationID == a.OrganizationID).Count(),
                       MonthlyRatePerAccount = a.MonthlyRatePerAccount != 0 ? a.MonthlyRatePerAccount : 0,
                       BillAmount = _unitOfWork.UserRepository.GetAll().Any(X => X.RoleID == 3) ? _unitOfWork.UserRepository.GetAll().Where(x => x.RoleID == 3 && x.OrganizationID == a.OrganizationID).Count() * a.MonthlyRatePerAccount != 0 ? a.MonthlyRatePerAccount : 0 : 0,
                       PaymentStatus = GetUserPaymentStatusByuser(a),
                       OpenRequest = a.UserRequests.Where(x => x.StatusID == (int)RequestStatusEnum.Open).FirstOrDefault() != null ? S2TAnalytics.Common.Helper.Helper.GetEnumDisplayName(RequestTypeEnum.SubscritionPlan) : ""
                   });
            }
            else
            {
                subscribers = _unitOfWork.UserRepository.GetAll().Where(x => x.RoleID == 2 && x.Name.ToLower().Contains(nameFilter.ToLower()) && x.UserPlans.Any(y => y.IsActive == true)).ToList()
                    .Select(a => new AdminSubscriberModel()
                    {
                        UserID = a.Id,
                        Name = a.Name,
                        EmailID = a.EmailID,
                        PlanName = plans.Where(x => x.PlanID == a.UserPlans.Where(up => up.IsActive == true).Select(up => up.PlanID).FirstOrDefault()).Select(x => x.Name).FirstOrDefault(),
                        IsTrial = a.UserPlans.Where(up => up.IsActive == true).Select(up => up.Days).First() > 0 ? true : false,
                        PhoneNumber = a.PhoneNumber,
                        ReportUser = _unitOfWork.UserRepository.GetAll().Where(x => x.RoleID == 3 && x.OrganizationID == a.OrganizationID).Count(),
                        MonthlyRatePerAccount = a.MonthlyRatePerAccount != 0 ? a.MonthlyRatePerAccount : 0,
                        BillAmount = _unitOfWork.UserRepository.GetAll().Any(X => X.RoleID == 3) ? _unitOfWork.UserRepository.GetAll().Where(x => x.RoleID == 3 && x.OrganizationID == a.OrganizationID).Count() * a.MonthlyRatePerAccount != 0 ? a.MonthlyRatePerAccount : 0 : 0,
                        PaymentStatus = GetUserPaymentStatusByuser(a),
                        OpenRequest = a.UserRequests.Where(x => x.StatusID == (int)RequestStatusEnum.Open).FirstOrDefault() != null ? S2TAnalytics.Common.Helper.Helper.GetEnumDisplayName(RequestTypeEnum.SubscritionPlan) : ""
                    });
            }
            //  var expression = GetExpression(pageRecordModel.SortBy);
            if (pageRecordModel.SortOrder == "asc")
            {
                if (pageRecordModel.SortBy == "Name" || pageRecordModel.SortBy == "")
                    subscribersList = subscribers.OrderBy(x => x.Name).ToList();
                else if (pageRecordModel.SortBy == "PlanName")
                    subscribersList = subscribers.OrderBy(x => x.PlanName).ToList();
                else if (pageRecordModel.SortBy == "Email")
                    subscribersList = subscribers.OrderBy(x => x.EmailID).ToList();
                else if (pageRecordModel.SortBy == "Phone")
                    subscribersList = subscribers.OrderBy(x => x.PhoneNumber).ToList();
                else if (pageRecordModel.SortBy == "ReportUser")
                    subscribersList = subscribers.OrderBy(x => x.ReportUser).ToList();
                else if (pageRecordModel.SortBy == "MonthlyRates")
                    subscribersList = subscribers.OrderBy(x => x.MonthlyRatePerAccount).ToList();
                else if (pageRecordModel.SortBy == "Bill")
                    subscribersList = subscribers.OrderBy(x => x.BillAmount).ToList();
                else if (pageRecordModel.SortBy == "PaymentStatus")
                    subscribersList = subscribers.OrderBy(x => x.PaymentStatus).ToList();
                else if (pageRecordModel.SortBy == "Request")
                    subscribersList = subscribers.OrderBy(x => x.OpenRequest).ToList();
            }
            else
            {
                if (pageRecordModel.SortBy == "Name" || pageRecordModel.SortBy == "")
                    subscribersList = subscribers.OrderByDescending(x => x.Name).ToList();
                else if (pageRecordModel.SortBy == "PlanName")
                    subscribersList = subscribers.OrderByDescending(x => x.PlanName).ToList();
                else if (pageRecordModel.SortBy == "Email")
                    subscribersList = subscribers.OrderByDescending(x => x.EmailID).ToList();
                else if (pageRecordModel.SortBy == "Phone")
                    subscribersList = subscribers.OrderByDescending(x => x.PhoneNumber).ToList();
                else if (pageRecordModel.SortBy == "ReportUser")
                    subscribersList = subscribers.OrderByDescending(x => x.ReportUser).ToList();
                else if (pageRecordModel.SortBy == "MonthlyRates")
                    subscribersList = subscribers.OrderByDescending(x => x.MonthlyRatePerAccount).ToList();
                else if (pageRecordModel.SortBy == "Bill")
                    subscribersList = subscribers.OrderByDescending(x => x.BillAmount).ToList();
                else if (pageRecordModel.SortBy == "PaymentStatus")
                    subscribersList = subscribers.OrderByDescending(x => x.PaymentStatus).ToList();
                else if (pageRecordModel.SortBy == "Request")
                    subscribersList = subscribers.OrderByDescending(x => x.OpenRequest).ToList();
            }

            var newSubscribersList = subscribersList.Skip((pageRecordModel.PageNumber - 1) * pageRecordModel.PageSize).Take(pageRecordModel.PageSize).ToList();
            //if (pageRecordModel.SortOrder == "asc")
            //    subscribersList = subscribers.OrderBy(expression).ToList();
            //else
            //    subscribersList = subscribers.OrderByDescending(expression).ToList();
            var subscriberPlan = _unitOfWork.SubscriptionPlanRepository.GetAll().Select(p => new EnumHelper()
            {
                stringValue = p.Name,
                intValue = p.PlanID
            }).ToList();

            var plansIds = Enum.GetValues(typeof(SubscriberReminderEnum)).Cast<int>().ToList();
            var subscriberReminders = new List<EnumHelper>();
            foreach (var w in plansIds)
            {
                var r = new EnumHelper
                {
                    intValue = w,
                    stringValue = Enum.GetName(typeof(SubscriberReminderEnum), w),
                    DisplayName = ((SubscriberReminderEnum)Enum.ToObject(typeof(SubscriberReminderEnum), w)).GetEnumDisplayName()
                };
                subscriberReminders.Add(r);
            }

            var result = new ServiceResponse
            {
                MultipleData = new Dictionary<string, object>(){
                        { "Subscribers", newSubscribersList },
                         { "Plans", subscriberPlan },
                           { "SubscriberReminders", subscriberReminders },
                    },
                Success = true
            };
            return result;
        }
        private string GetUserPaymentStatusByuser(User user)
        {

            var ActivePlan = user.UserPlans.Where(x => x.IsActive == true).FirstOrDefault();
            var Termlength = (int)(PlanTermLengthEnum)ActivePlan.TermLengthId;
            var startDate = ActivePlan.StartedDate;
            var endDate = new DateTime();
            if (ActivePlan.TermLengthId == 0)
            {
                return S2TAnalytics.Common.Helper.Helper.GetEnumDisplayName(PaymentStatusEnum.TrailPeriod);
            }
            else if (ActivePlan.TermLengthId == 1)
            {
                endDate = startDate.AddMonths(1);
            }
            else if (ActivePlan.TermLengthId == 2)
            {
                endDate = startDate.AddYears(1);
            }
            else if (ActivePlan.TermLengthId == 3)
            {
                endDate = startDate.AddYears(2);
            }
            else if (ActivePlan.TermLengthId == 4)
            {
                endDate = startDate.AddYears(3);
            }

            if (startDate < endDate)
            {
                return S2TAnalytics.Common.Helper.Helper.GetEnumDisplayName(PaymentStatusEnum.Paid);
            }
            else
            {
                return S2TAnalytics.Common.Helper.Helper.GetEnumDisplayName(PaymentStatusEnum.Overdue);
            }
        }
        private Expression<Func<AdminSubscriberModel, object>> GetExpression(string sortBy)
        {
            switch (sortBy)
            {
                case "Name":
                    return x => x.Name;
                case "PlanName":
                    return x => x.PlanName;
                case "Email":
                    return x => x.EmailID;
                case "Phone":
                    return x => x.PhoneNumber;
                case "ReportUser":
                    return x => x.ReportUser;
                case "MonthlyRates":
                    return x => x.MonthlyRatePerAccount;
                case "Bill":
                    return x => x.BillAmount;
                case "PaymentStatus":
                    return x => x.PaymentStatus;
                case "Request":
                    return x => x.OpenRequest;
                default:
                    return x => x.Name;
            }
        }
        public ServiceResponse GetSubscriberList(string userID)
        {
            ObjectId UserID = ObjectId.Parse(userID);
            int CountPlans = 0;
            if (_unitOfWork.UserRepository.GetAll().Where(x => x.Id == UserID && x.UserPlans.Any(y => y.IsActive == true)).FirstOrDefault() != null)
                CountPlans = (_unitOfWork.UserRepository.GetAll().Where(x => x.Id == UserID && x.UserPlans.Any(y => y.IsActive == true)).FirstOrDefault()).UserPlans.ToList().Count;

            var us = _unitOfWork.UserRepository.GetAll().Where(x => x.Id == UserID).FirstOrDefault();
            UserModel user = new UserModel().ToUserModel(us);

            AdminSubscriberPlanModel plans = new AdminSubscriberPlanModel();
            if (CountPlans > 0)
            {
                plans = us.UserPlans.Select(p => new AdminSubscriberPlanModel()
                {
                    PlanID = p.PlanID,
                    IsActive = p.IsActive,
                    Days = p.Days,
                    Price = p.Price,
                    TermLengthId = p.TermLengthId,
                    ExpireDate = p.StartedDate,
                }).Where(x => x.IsActive == true).FirstOrDefault();

                var sub_plan = _unitOfWork.SubscriptionPlanRepository.GetAll().Where(p => p.PlanID == plans.PlanID).FirstOrDefault();

                var planPeriod = sub_plan.PlanTermLength.Where(x => x.PlanTermLengthId == plans.TermLengthId).FirstOrDefault();
                var month = planPeriod != null ? planPeriod.Month : 0;

                plans.Name = sub_plan.Name;

                if (plans.Days > 0)
                    plans.ExpireDate = plans.ExpireDate.AddDays(plans.Days);
                else
                    plans.ExpireDate = plans.ExpireDate.AddMonths(month);
            }
            List<UserInvoiceHistory> PaymentDetail = new List<UserInvoiceHistory>();
            List<UserRequest> userRequest = new List<UserRequest>();

            if (us.UserInvoiceHistory != null)
                PaymentDetail = us.UserInvoiceHistory.ToList();
            if (us.UserRequests != null)
                userRequest = us.UserRequests.ToList();

            var SubscribersPlans = _unitOfWork.SubscriptionPlanRepository.GetAll().ToList();

            var reportUser = _unitOfWork.UserRepository.GetAll().Where(x => x.RoleID == 3 && x.OrganizationID == user.OrganizationID).Count();
            var accounts = _unitOfWork.AccountDetailRepository.GetAll().Where(x => x.OrganizationId == user.OrganizationID).Count();
            var dataSource = 1;
            var result = new ServiceResponse
            {
                MultipleData = new Dictionary<string, object>(){
                    { "ContactInfo", user },
                    { "Plans", plans },
                    { "SubscribersPlans", SubscribersPlans },
                    { "PaymentDetail", PaymentDetail },
                    { "UserRequest", userRequest },
                    { "ReposrtUsers", reportUser },
                    { "Accounts", accounts },
                    { "DataSources", dataSource },
                },
                Success = true
            };
            return result;
        }
        public ServiceResponse GetInvoiceHistory(string userID, PageRecordModel pageRecordModel)
        {
            ObjectId UserID = ObjectId.Parse(userID);
            var us = _unitOfWork.UserRepository.GetAll().Where(x => x.Id == UserID).FirstOrDefault();
            List<UserInvoiceHistoryModel> userInvoiceHistoryModels = null;
            if (us.UserInvoiceHistory != null)
            {
                userInvoiceHistoryModels = us.UserInvoiceHistory.Select(n => new UserInvoiceHistoryModel()
                {
                    Id = n.Id,
                    PlanName = n.PlanName,
                    PlanId = n.PlanId,
                    TransactionDate = n.TransactionDate,
                    Status = n.Status,
                    Price = n.Price,
                    Paid = n.Paid
                }).ToList();
            }
            var userInvoiceHistoryList = userInvoiceHistoryModels != null ? userInvoiceHistoryModels.Skip((pageRecordModel.PageNumber - 1) * pageRecordModel.PageSize).Take(pageRecordModel.PageSize).ToList() : null;
            var result = new ServiceResponse
            {
                MultipleData = new Dictionary<string, object>(){
                         { "Total", userInvoiceHistoryModels!=null?userInvoiceHistoryModels.Count:0 },
                          { "PaymentDetail", userInvoiceHistoryList }
                    },
                Success = true
            };
            return result;
        }
        public ServiceResponse GetUserRequest(string userID, PageRecordModel pageRecordModel)
        {
            ObjectId UserID = ObjectId.Parse(userID);
            var us = _unitOfWork.UserRepository.GetAll().Where(x => x.Id == UserID).FirstOrDefault();
            //List<UserRequest> userRequest = null;
            //if (us.UserRequests != null)
            //{
            //    userRequest = us.UserRequests.Select(n => new UserRequest()
            //    {
            //       Details = n.Details,
            //    }).ToList();
            //}

            var requestType = new List<EnumHelper>();
            foreach (RequestTypeEnum RequestType in Enum.GetValues(typeof(RequestTypeEnum)))
            {
                var r = new EnumHelper
                {
                    intValue = (int)RequestType,
                    stringValue = Enum.GetName(typeof(RequestTypeEnum), RequestType),
                    DisplayName = ((RequestTypeEnum)Enum.ToObject(typeof(RequestTypeEnum), RequestType)).GetEnumDisplayName()
                };
                requestType.Add(r);
            }

            var requestStatus = new List<EnumHelper>();
            foreach (RequestStatusEnum RequestStatus in Enum.GetValues(typeof(RequestStatusEnum)))
            {
                var r = new EnumHelper
                {
                    intValue = (int)RequestStatus,
                    stringValue = Enum.GetName(typeof(RequestStatusEnum), RequestStatus),
                    DisplayName = ((RequestStatusEnum)Enum.ToObject(typeof(RequestStatusEnum), RequestStatus)).GetEnumDisplayName()
                };
                requestStatus.Add(r);
            }

            var userRequests = us.UserRequests.Skip((pageRecordModel.PageNumber - 1) * pageRecordModel.PageSize).Take(pageRecordModel.PageSize).ToList();
            var result = new ServiceResponse
            {
                MultipleData = new Dictionary<string, object>(){
                   { "Total", us.UserRequests.Count },
                   { "UserRequests", userRequests },
                   { "RequestType", requestType },
                   { "RequestStatus", requestStatus },

                },
                Success = true
            };
            return result;
        }
        public ServiceResponse UpdateSubscriberContactInfo(UserModel user)
        {
            var us = new UserModel().ToUser(user);
            var myuser = _unitOfWork.UserRepository.GetAll().Where(x => x.Id == ObjectId.Parse(user.UserID)).FirstOrDefault();
            if (myuser != null)
            {
                myuser.WebSite = us.WebSite; ;
                myuser.PhoneNumber = us.PhoneNumber;
                myuser.PhoneCountryCode = us.PhoneCountryCode;
                myuser.CountryCode = us.CountryCode;
                myuser.EmailID = us.EmailID;
                myuser.Country = us.Country;

            }
            _unitOfWork.UserRepository.Update(myuser);
            UserModel res = new UserModel().ToUserModel(us);
            res.UserID = user.UserID;
            var result = new ServiceResponse
            {
                MultipleData = new Dictionary<string, object>(){
                        { "ContactInfo", res },
                    },
                Success = true
            };
            return result;
        }
        public ServiceResponse updateUserActivation(UserModel user)
        {
            var us = new UserModel().ToUser(user);
            var myuser = _unitOfWork.UserRepository.GetAll().Where(x => x.Id == ObjectId.Parse(user.UserID)).FirstOrDefault();
            if (myuser != null)
            {
                myuser.IsActive = us.IsActive;
            }
            _unitOfWork.UserRepository.Update(myuser);
            var result = new ServiceResponse
            {
                Success = true,
                Message = us.IsActive ? SuperAdminMessage.UserActivatedSuccessfully : SuperAdminMessage.UserDeactivatedSuccessfully
            };
            return result;
        }
        public ServiceResponse UpdateSubscriberSummary(int selectedPlanId, UserModel user)
        {
            var subscriptionPlan = _unitOfWork.SubscriptionPlanRepository.GetAll().Where(x => x.PlanID == selectedPlanId).FirstOrDefault();

            var result = new ServiceResponse
            {
                MultipleData = new Dictionary<string, object>(){
                    { "ContactInfo", user },

                },
                Success = true,
                Message = ""
            };
            return result;
        }
        public bool ExtendUserTrial(List<string> userIds, int ExtendDays)
        {

            var myUser = _unitOfWork.UserRepository.GetAll().ToList();

            var userList = myUser.Where(m => userIds.Where(z => ObjectId.Parse(z) == m.Id).Count() > 0).ToList();

            foreach (var user in userList)
            {
                var activePlan = user.UserPlans.Where(x => x.IsActive == true).FirstOrDefault();
                if (activePlan != null)
                {
                    if (activePlan.Days > 0)
                        activePlan.Days = activePlan.Days + ExtendDays;
                }
                _unitOfWork.UserRepository.Update(user);
            }

            var result = true;
            return result;
        }
        public ServiceResponse UpdatePlans(string userId, int planId)
        {

            var myuser = _unitOfWork.UserRepository.GetAll().Where(x => x.Id == ObjectId.Parse(userId)).FirstOrDefault();
            if (myuser != null)
            {
                (myuser.UserPlans.Where(x => x.IsActive).First()).IsActive = false;
                (myuser.UserPlans.Where(x => x.PlanID == planId).First()).IsActive = true;

            }
            _unitOfWork.UserRepository.Update(myuser);
            var result = new ServiceResponse
            {
                MultipleData = new Dictionary<string, object>()
                {

                },
                Success = true
            };
            return result;
        }
        public ServiceResponse UpdateUserActivation(List<string> userIds, bool isActive = false)
        {
            var myuser = _unitOfWork.UserRepository.GetAll().ToList();
            //var myuser1 = _unitOfWork.UserRepository.GetAll().Where(m => userIds.Where(z => ObjectId.Parse(z) == m.Id).Count() > 0).ToList();
            var userList = myuser.Where(m => userIds.Where(z => ObjectId.Parse(z) == m.Id).Count() > 0).ToList();
            foreach (var user in userList)
            {
                user.IsActive = isActive;
                _unitOfWork.UserRepository.Update(user);
            }
            return new ServiceResponse();
        }

        public List<InvoiceModel> DownloadInvoice(string userId, List<Guid> invoiceIds)
        {
            var user = _unitOfWork.UserRepository.GetAll().Where(x=> x.Id == ObjectId.Parse(userId)).FirstOrDefault();
            if (user != null)
            {
                if (user.UserInvoiceHistory == null) { return null; }

                var invoiceHistoryForInvoice = user.UserInvoiceHistory.Where(x => invoiceIds.Contains(x.Id)).ToList();
                //if (invoiceHistoryForInvoice == null || invoiceHistoryForInvoice.Count == 0) { return null; }
                var accounts = _unitOfWork.AccountDetailRepository.GetAll().Where(x => x.OrganizationId == user.OrganizationID).Count();

                List<InvoiceModel> invoiceModelList;
                invoiceModelList = invoiceHistoryForInvoice.Select(x => new InvoiceModel
                {
                    InvoiceNumber = MathCalculations.GenerateRandomNo(8).ToString(),
                    Country = user.Country,
                    EmailId = user.EmailID,
                    InvoiceDate = (DateTime.UtcNow).ToString("dd/MM/yyyy"),
                    ClientName = user.Name,
                    PlanName = x.PlanName,
                    PlanLogo = x.PlanId == 2 ? "basic.png" : x.PlanId == 3 ? "advance.png" : "ultimate.png",
                    Price = (x.Price + x.UsedUserCreditAmount).ToString(),
                    TotalAccounts = accounts.ToString(),
                    ApplicationFee = x.ApplicationFee.ToString(),
                    SubTotal = Math.Round((x.Price)).ToString(),
                    Total = (Math.Round((x.Price)) + x.ApplicationFee).ToString(),
                    CreditedAmount = x.UsedUserCreditAmount.ToString()
                }).ToList();


                return invoiceModelList;
            }
            else
            {
                return null;
            }
        }

        public ServiceResponse GetUserNotifications(PageRecordModel pageRecordModel)
        {
            var Name = "";
            foreach (var filter in pageRecordModel.Filters)
            {
                if (filter.Key == "Name")
                    Name = pageRecordModel.Filters.Single(x => x.Key == "Name").Value;
            }
            var allUsers = _unitOfWork.UserRepository.GetAll().Where(x => x.RoleID == 2 && x.Name.ToLower().Contains(Name.ToLower())).ToList();
            var allNotifications = new List<AdminNotificationModel>();

            foreach (var user in allUsers)
            {

                foreach (var n in user.Notifications)
                {
                    AdminNotificationModel notification = new AdminNotificationModel();
                    notification.userId = user.Id.ToString();
                    notification.Name = user.Name;
                    notification.Email = user.EmailID;
                    notification.NotificationId = n.id;
                    notification.Notification = n.Text;
                    notification.Date = n.Date;
                    allNotifications.Add(notification);
                }
            }
            var notifications = allNotifications.Skip((pageRecordModel.PageNumber - 1) * pageRecordModel.PageSize).Take(pageRecordModel.PageSize).ToList();



            var result = new ServiceResponse
            {
                Data = notifications,

                MultipleData = new Dictionary<string, object>(){
                        { "Notifications", notifications },
                         { "Total", allNotifications.Count },
                    },
                Success = true
            };
            return result;
        }
        public bool SendReminders(List<string> userIds, int reminderId)
        {
            var myUser = _unitOfWork.UserRepository.GetAll().ToList();
            var userList = myUser.Where(m => userIds.Where(z => ObjectId.Parse(z) == m.Id).Count() > 0).ToList();
            foreach (var user in userList)
            {
                var email = user.EmailID;
                MailHelper mailHelper = new MailHelper();
                var type = ((SubscriberReminderEnum)Enum.ToObject(typeof(SubscriberReminderEnum), reminderId)).GetEnumDisplayName();
                try
                {
                    mailHelper.ToEmail = email;
                    mailHelper.Subject = "Reminder Mail";

                    if (reminderId == Convert.ToInt32(SubscriberReminderEnum.PaymentOverdue))
                    {
                        mailHelper.Body = CommonFunction.PaymentOverDueReminderMailBody(user.Name, type);
                    }
                    else if (reminderId == Convert.ToInt32(SubscriberReminderEnum.TrailPeriod))
                    {
                        mailHelper.Body = CommonFunction.TrailPeriodReminderMailBody(user.Name, type);
                    }
                    else if (reminderId == Convert.ToInt32(SubscriberReminderEnum.AfterTrailPeriod))
                    {
                        mailHelper.Body = CommonFunction.AfterTrailPeriodReminderMailBody(user.Name, type);
                    }
                    mailHelper.SendEmail();
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }
      public  ServiceResponse sendOverdueReminder(UserModel user)
        {
            try
            {
                MailHelper mailHelper = new MailHelper();
                mailHelper.ToEmail = user.EmailID;
                mailHelper.Subject = "Reminder Mail";
                mailHelper.Body = CommonFunction.PaymentOverDueReminderMailBody(user.Name, SubscriberReminderEnum.PaymentOverdue.ToString());
                mailHelper.SendEmail();
                return new ServiceResponse
                {
                    Success = true,
                    Message = SuperAdminMessage.ReminderMailSuccessfully
                };
            }
            catch
            {
                return new ServiceResponse
                {
                    Success = true,
                    Message = SuperAdminMessage.SomethingWentWrong
                };
            }
        }
    }
}
