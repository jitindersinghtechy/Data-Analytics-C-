using MongoDB.Bson;
using S2TAnalytics.Common.Enums;
using S2TAnalytics.Common.Helper;
using S2TAnalytics.DAL.Interfaces;
using S2TAnalytics.DAL.Models;
using S2TAnalytics.Infrastructure.Interfaces;
using S2TAnalytics.Infrastructure.Models;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;


namespace S2TAnalytics.Infrastructure.Services
{
    public class UserAccountService : IUserAccountService
    {
        public readonly IUnitOfWork _unitOfWork;
        public UserAccountService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public ServiceResponse SaveCard(ObjectId userId, CardModel cardModel)
        {
            var user = _unitOfWork.UserRepository.GetAll().Where(u => u.Id == userId).SingleOrDefault();
            if (user != null)
            {
                var isCardExist = user.UserCards != null ? user.UserCards.Any(x => x.CardNumber == cardModel.CardNumber) : false;
                if (!isCardExist)
                {
                    var response = GetCustomerId(user.EmailID, cardModel);
                    if (response.Success)
                    {
                        if (user.UserCards == null)
                        {
                            user.UserCards = new List<UserCard>();
                        }
                        user.UserCards.Add(new UserCard()
                        {
                            Id = Guid.NewGuid(),
                            PaymentType = cardModel.PaymentType.Decrypt(),
                            CardName = cardModel.CardName,
                            CardNumber = cardModel.CardNumber,
                            ExpirationMonth = cardModel.ExpirationMonth,
                            ExpirationYear = cardModel.ExpirationYear,
                            IsActive = user.UserCards.Count == 0 ? true : false,
                            CreatedDate = DateTime.UtcNow,
                            CustomerId = response.Data.ToString()
                        });

                        _unitOfWork.UserRepository.Update(user);
                        user = _unitOfWork.UserRepository.GetAll().Where(u => u.Id == userId).Single();
                        return new ServiceResponse { Data = new CardModel().ToCardModel(user.UserCards), Success = true, Message = "Card details saved successfully." };
                    }
                    else
                    {
                        return new ServiceResponse { Success = false, Message = response.Message };
                    }
                }
                else
                {
                    return new ServiceResponse { Success = false, Message = "Card Number already exists." };
                }
            }
            else
            {
                return new ServiceResponse { Success = false, Message = "Error saving card details." };
            }
        }

        public ServiceResponse GetDefaultData(ObjectId userId, int RoleID)
        {
            var response = new ServiceResponse();
            if (RoleID != (int)UserRolesEnum.ReportUser)
            {
                var userCards = GetUserCards(userId);
                var paymentTypes = GetPaymentTypeEnum();
                var userPlan = GetUserSubscriptionPlan(userId);
                var userInvoiceHistory = GetUserInvoiceHistory(userId);
                var user = GetUser(userId);
                var emailNotificationSettingsEnum = GetEmailNotificationSettingsEnum();
                var emailNotificationSettings = GetEmailNotificationSettings(userId);
                response = new ServiceResponse()
                {
                    Success = true,
                    MultipleData = new Dictionary<string, object>(){

                    { "userCards", userCards },
                    { "paymentTypes", paymentTypes },
                    { "userPlan", userPlan},
                    { "user", user},
                    {"userInvoiceHistory",userInvoiceHistory },
                    {"emailNotificationSettingsEnum",emailNotificationSettingsEnum },
                    {"emailNotificationSettings",emailNotificationSettings }
                }
                };
            }
            else
            {
                var user = GetUser(userId);
                var emailNotificationSettingsEnum = GetEmailNotificationSettingsEnum();
                var emailNotificationSettings = GetEmailNotificationSettings(userId);
                response = new ServiceResponse()
                {
                    Success = true,
                    MultipleData = new Dictionary<string, object>(){
                    { "user", user},
                    {"emailNotificationSettingsEnum",emailNotificationSettingsEnum },
                    {"emailNotificationSettings",emailNotificationSettings }
                }
                };
            }
            return response;
        }

        public List<CardModel> GetUserCards(ObjectId userId)
        {
            try
            {
                var user = _unitOfWork.UserRepository.GetAll().Where(u => u.Id == userId).SingleOrDefault();
                if (user != null)
                {
                    var cards = new CardModel().ToCardModel(user.UserCards);
                    return cards;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public UserModel GetUser(ObjectId userId)
        {
            var user = _unitOfWork.UserRepository.GetAll().Where(u => u.Id == userId).Single();
            return new UserModel().ToUserModel(user);
        }

        public UserPlanModel GetUserSubscriptionPlan(ObjectId userId)
        {
            var plan = _unitOfWork.UserRepository.GetAll().Where(u => u.Id == userId).Single().UserPlans.Where(up => up.IsActive).Select(n => new UserPlanModel()
            {
                Days = n.Days,
                PlanID = n.PlanID,
                Price = n.Price,
                TermLengthId = n.TermLengthId
            }).SingleOrDefault();
            plan.Name = _unitOfWork.SubscriptionPlanRepository.GetAll().Where(s => s.PlanID == plan.PlanID).SingleOrDefault().Name;
            return plan;
        }

        private List<UserInvoiceHistoryModel> GetUserInvoiceHistory(ObjectId userId)
        {
            var user = _unitOfWork.UserRepository.GetAll().Where(u => u.Id == userId).SingleOrDefault();
            if (user.UserInvoiceHistory != null)
            {
                var userInvoiceHistoryModels = user.UserInvoiceHistory.Select(n => new UserInvoiceHistoryModel()
                {
                    Id = n.Id,
                    PlanName = n.PlanName,
                    PlanId = n.PlanId,
                    TransactionDate = n.TransactionDate,
                    Status = n.Status,
                    Price = n.Price,
                    Paid = n.Paid
                }).ToList();
                //userInvoiceHistoryModel.PlanName = _unitOfWork.SubscriptionPlanRepository.GetAll().Where(s => s.PlanID == userInvoiceHistoryModel.PlanId).Single().Name;
                return userInvoiceHistoryModels;
            }
            else
            {
                return null;
            }
        }
        private List<EmailNotificationSettingsModel> GetEmailNotificationSettings(ObjectId userId)
        {
            var user = _unitOfWork.UserRepository.GetAll().Where(u => u.Id == userId).SingleOrDefault();
            if (user.UserEmailNotificationSettings != null)
            {
                var userEmailNotificationSettings = user.UserEmailNotificationSettings.Where(x => x.HasAccess).Select(n => new EmailNotificationSettingsModel()
                {
                    NotificationSettingsId = n.NotificationSettingsId

                }).ToList();
                return userEmailNotificationSettings;
            }
            else
            {
                return null;
            }
        }


        private static List<EnumHelper> GetPaymentTypeEnum()
        {
            var paymentTypes = new List<EnumHelper>();
            foreach (PaymentTypeEnum paymentType in Enum.GetValues(typeof(PaymentTypeEnum)))
            {
                var paymentTypeEnum = new EnumHelper
                {
                    stringValue = ((int)paymentType).Encrypt(),
                    DisplayName = ((PaymentTypeEnum)Enum.ToObject(typeof(PaymentTypeEnum), (int)paymentType)).GetEnumDisplayName()
                };
                //timeLines.Add(timeline);
                paymentTypes.Add(paymentTypeEnum);
            }

            return paymentTypes;
        }

        private static List<EnumHelper> GetEmailNotificationSettingsEnum()
        {
            var emailNotificationSettings = new List<EnumHelper>();
            foreach (EmailNotificationSettingsEnum emailNotificationSetting in Enum.GetValues(typeof(EmailNotificationSettingsEnum)))
            {
                var emailNotificationSettingEnum = new EnumHelper
                {
                    stringValue = ((int)emailNotificationSetting).Encrypt(),
                    DisplayName = ((EmailNotificationSettingsEnum)Enum.ToObject(typeof(EmailNotificationSettingsEnum), (int)emailNotificationSetting)).GetEnumDisplayName(),
                    intValue = (int)emailNotificationSetting
                };
                emailNotificationSettings.Add(emailNotificationSettingEnum);
            }

            return emailNotificationSettings;
        }

        public ServiceResponse GetCustomerId(string userEmail, CardModel card)
        {
            try
            {

                var myCustomer = new StripeCustomerCreateOptions();
                myCustomer.Email = userEmail;

                // setting up the card
                myCustomer.SourceCard = new SourceCard()
                {
                    Number = card.CardNumber,
                    ExpirationYear = card.ExpirationYear.ToString(),
                    ExpirationMonth = card.ExpirationMonth.ToString(),
                    Name = card.CardName,
                    Cvc = card.CVV
                };

                var customerService = new StripeCustomerService();
                StripeCustomer stripeCustomer = customerService.Create(myCustomer);
                return new ServiceResponse { Data = stripeCustomer.Id, Success = true };

            }
            catch (Exception ex)
            {
                return new ServiceResponse { Success = false, Message = ex.Message };
            }
        }

        public ServiceResponse DeleteCards(List<Guid> CardIds, ObjectId userId)
        {
            var user = _unitOfWork.UserRepository.GetAll().Where(u => u.Id == userId).SingleOrDefault();
            if (user != null)
            {
                user.UserCards.RemoveAll(x => CardIds.Contains(x.Id));
                _unitOfWork.UserRepository.Update(user);

                return new ServiceResponse { Data = new CardModel().ToCardModel(user.UserCards), Success = true, Message = "Card(s) deleted successfully." };
            }
            else
            {
                return new ServiceResponse { Success = false, Message = CommonMessage.Something_Wrong };
            }
        }

        public ServiceResponse GetPlanDetails(ObjectId userId)
        {
            try
            {
                var user = _unitOfWork.UserRepository.GetAll().Where(u => u.Id == userId).SingleOrDefault();
                if (user != null)
                {
                    var plan = user.UserPlans.Where(x => x.IsActive).SingleOrDefault();
                    if (plan == null && user.RoleID == 3)
                    {
                        plan = _unitOfWork.UserRepository.GetAll().Where(u => u.OrganizationID == user.OrganizationID && u.RoleID == 2).Single().UserPlans.Where(up => up.IsActive).Single();
                    }
                    var accounts = _unitOfWork.AccountDetailRepository.GetAll().Where(x => x.OrganizationId == user.OrganizationID).Count();
                    double DaysLeft = 0;
                    if (plan.Days != 0)
                    {
                        DaysLeft = plan.Days - ((DateTime.Now.Date) - plan.StartedDate.Date).TotalDays;
                    }
                    var response = new ServiceResponse()
                    {
                        Success = true,
                        MultipleData = new Dictionary<string, object>(){
                            { "PlanId", plan.PlanID },
                             { "TotalDays", plan.Days },
                            { "DaysLeft", DaysLeft },
                            { "accounts", accounts }
                        }
                    };
                    return response;
                }
                else
                {
                    return new ServiceResponse { Success = false, Message = CommonMessage.Something_Wrong };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public ServiceResponse GetSubscriptionPlans(ObjectId userId)
        {
            var subscriptionPlans = _unitOfWork.SubscriptionPlanRepository.GetAll().ToList();
            var plans = subscriptionPlans.Select(s => new SubscriptionPlanModel()
            {
                PlanId = s.PlanID,
                Name = s.Name,
                Price = s.Price,
                InfrastructureCost = s.InfrastructureCost,
                Widgets = ((s.WidgetsAccess.Where(x => x.HasAccess == true).Select(g => g.WidgetID.ToString())).ToList()).Select(wid => ((PlansEnum)Enum.ToObject(typeof(PlansEnum), Convert.ToInt32(wid))).GetEnumDisplayName()).ToList(),
                PlanTermLength = s.PlanTermLength
            }).ToList();

            var termLength = new List<EnumHelper>();
            var planTermLength = Enum.GetValues(typeof(PlanTermLengthEnum)).Cast<int>().ToList();
            foreach (var p in planTermLength)
            {
                var termL = new EnumHelper
                {
                    intValue = p,
                    stringValue = Enum.GetName(typeof(PlanTermLengthEnum), p),
                    DisplayName = ((PlanTermLengthEnum)Enum.ToObject(typeof(PlanTermLengthEnum), p)).GetEnumDisplayName()
                };
                termLength.Add(termL);
            }

            //var userCreditAmount = 0.0;
            //var userCredit = _unitOfWork.UserRepository.GetAll().Where(x => x.Id == userId).Select(x=>x.UserCredits).FirstOrDefault();
            //if (userCredit != null)
            //    userCreditAmount = userCredit.OrderByDescending(x => x.AddedDate).Select(x => x.Amount).FirstOrDefault();

            var user = _unitOfWork.UserRepository.GetAll().Where(u => u.Id == userId).SingleOrDefault();


            int accounts = _unitOfWork.AccountDetailRepository.GetAll().Where(x => x.OrganizationId == user.OrganizationID && !x.IsExclude).Count();

            var activePlan = user.UserPlans.Where(x => x.IsActive).SingleOrDefault();

            var plan = _unitOfWork.SubscriptionPlanRepository.GetAll().Where(x => x.PlanID == activePlan.PlanID).SingleOrDefault();

            int ActivePlanPrice = activePlan.PlanID == 2 && activePlan.TermLengthId == 0 ? 0 : Convert.ToInt32(activePlan.Price == 0 ? (accounts * plan.Price) : activePlan.Price);

            int usedDays = Convert.ToInt32((DateTime.Now.Date - activePlan.StartedDate.Date).TotalDays);
            int usedAmount = 0;
            if (activePlan.TermLengthId == Convert.ToInt32(PlanTermLengthEnum.OneMonth))
                usedAmount = (usedDays == 0 ? 1 : usedDays) * (ActivePlanPrice / (DateTime.DaysInMonth((activePlan.StartedDate).Year, (activePlan.StartedDate).Month)));
            else if (activePlan.TermLengthId == Convert.ToInt32(PlanTermLengthEnum.OneYear))
                usedAmount = (usedDays == 0 ? 1 : usedDays) * (ActivePlanPrice / 365);
            //else if (activePlan.TermLengthId == Convert.ToInt32(PlanTermLengthEnum.TwoYear))
            //    usedAmount = (usedDays == 0 ? 1 : usedDays) * (ActivePlanPrice / 365 * 2);
            //else if (activePlan.TermLengthId == Convert.ToInt32(PlanTermLengthEnum.ThreeYear))
            //    usedAmount = (usedDays == 0 ? 1 : usedDays) * (ActivePlanPrice / 365 * 3);
            else
                usedAmount = 0;

            var unusedAmount = ActivePlanPrice - usedAmount;
            int totalCredits = (user.UserCredits != null && user.UserCredits.Count() > 0) ? Convert.ToInt32(user.UserCredits.OrderByDescending(x => x.AddedDate).First().Amount) : 0;
            totalCredits += unusedAmount;

            var response = new ServiceResponse()
            {
                Success = true,
                Data = plans,
                MultipleData = new Dictionary<string, object>(){
                    { "Plans", plans },
                    { "TermLength", termLength },
                    { "UserCreditAmount", totalCredits },
                }
            };
            return response;
        }


        public ServiceResponse GetPromocodeDiscount(string promoCode)
        {
            var check = false;
            var amount = 0.0;

            var coupon = _unitOfWork.CouponDetailRepository.GetAll().Where(x => x.Code == promoCode).FirstOrDefault();
            if (coupon != null)
            {
                check = true;
                amount = coupon.Amount;
            }
            var response = new ServiceResponse()
            {
                Success = check,
                Data = amount,
            };
            return response;
        }

        public ServiceResponse PreRequestPlan(ObjectId userId, int PlanId)
        {
            var user = _unitOfWork.UserRepository.GetAll().Where(u => u.Id == userId).SingleOrDefault();
            if (user != null)
            {
                int totalCredits = (user.UserCredits != null && user.UserCredits.Count() > 0) ? Convert.ToInt32(user.UserCredits.OrderByDescending(x => x.AddedDate).First().Amount) : 0;

                var activePlan = user.UserPlans.Where(x => x.IsActive).SingleOrDefault();

                //var plan = _unitOfWork.SubscriptionPlanRepository.GetAll().Where(x => x.PlanID == PlanId).SingleOrDefault();
                int accounts = _unitOfWork.AccountDetailRepository.GetAll().Where(x => x.OrganizationId == user.OrganizationID && !x.IsExclude).Count();
                // int ActivePlanPrice = activePlan.PlanID == 2 && activePlan.TermLengthId == 0 ? 0 : Convert.ToInt32(activePlan.Price == 0 ? (accounts * plan.Price) : activePlan.Price);
                int ActivePlanPrice = activePlan.PlanID == 2 && activePlan.TermLengthId == 0 ? 0 : Convert.ToInt32(activePlan.Price);
                int usedDays = Convert.ToInt32((DateTime.Now.Date - activePlan.StartedDate.Date).TotalDays);
                int usedAmount = 0;
                if (activePlan.TermLengthId == Convert.ToInt32(PlanTermLengthEnum.OneMonth))
                    usedAmount = (usedDays == 0 ? 1 : usedDays) * (ActivePlanPrice / (DateTime.DaysInMonth((activePlan.StartedDate).Year, (activePlan.StartedDate).Month)));
                else if (activePlan.TermLengthId == Convert.ToInt32(PlanTermLengthEnum.OneYear))
                    usedAmount = (usedDays == 0 ? 1 : usedDays) * (ActivePlanPrice / 365);
                //else if (activePlan.TermLengthId == Convert.ToInt32(PlanTermLengthEnum.TwoYear))
                //    usedAmount = (usedDays == 0 ? 1 : usedDays) * (ActivePlanPrice / 365 * 2);
                //else if (activePlan.TermLengthId == Convert.ToInt32(PlanTermLengthEnum.ThreeYear))
                //usedAmount = (usedDays == 0 ? 1 : usedDays) * (ActivePlanPrice / 365 * 3);
                else
                    usedAmount = 0;

                var unusedAmount = ActivePlanPrice - usedAmount;

                totalCredits += unusedAmount;
                var response = new ServiceResponse()
                {
                    Success = true,
                    Data = totalCredits
                };
                return response;
            }
            else
            {
                return new ServiceResponse { Success = false, Message = CommonMessage.Something_Wrong };
            }
        }

        public ServiceResponse RequestPlan(ObjectId userId, int PlanId, int TermLengthId, string promocode)
        {
            try
            {
                //if (promocode == "")
                //    promocode = "";
                var user = _unitOfWork.UserRepository.GetAll().Where(u => u.Id == userId).SingleOrDefault();
                if (user != null)
                {
                    if (user.UserCards != null && user.UserCards.Count != 0)
                    {
                        var activeCard = user.UserCards.Where(x => x.IsActive).SingleOrDefault();
                        if (activeCard == null) { return new ServiceResponse { Success = false, Message = "Please add one payment method and make it active. After that request can be initiated." }; }

                        var activePlan = user.UserPlans.Where(x => x.IsActive).SingleOrDefault();

                        if (PlanId == 3)
                        {
                            AddNotification(userId, "Request has been sent to Super Admin for subscription to Ultimate Plan.");
                            SendNotificationToSuperAdmin(userId, user.Name, " Requested for subscription to Ultimate Plan.");
                            AddUserRequest(userId, user.Name + " Requested for subscription to Ultimate Plan.");
                            return new ServiceResponse { Success = true, Message = "Your Request has been sent to Super Admin for subscription of this plan." };
                        }
                        //if (PlanId == 5)
                        //{
                        //    AddNotification(userId, "Request has been sent to Super Admin for subscription to Customized Plan.");
                        //    return new ServiceResponse { Success = true, Message = "Your Request has been sent to Super Admin for subscription of this plan." };
                        //}

                        var plan = _unitOfWork.SubscriptionPlanRepository.GetAll().Where(x => x.PlanID == PlanId).SingleOrDefault();
                        int accounts = _unitOfWork.AccountDetailRepository.GetAll().Where(x => x.OrganizationId == user.OrganizationID && !x.IsExclude).Count();
                        int Price = 0;
                        // int ActivePlanPrice = activePlan.PlanID == 1 ? 0 : Convert.ToInt32(activePlan.Price == 0 ? (accounts * plan.Price) : activePlan.Price);

                        //int ActivePlanPrice = activePlan.PlanID == 2 && activePlan.TermLengthId == 0 ? 0 : Convert.ToInt32(activePlan.Price == 0 ? (accounts * plan.Price) : activePlan.Price * accounts);
                        int ActivePlanPrice = activePlan.PlanID == 2 && activePlan.TermLengthId == 0 ? 0 : Convert.ToInt32(activePlan.Price);

                        int usedDays = Convert.ToInt32((DateTime.Now.Date - activePlan.StartedDate.Date).TotalDays);
                        int usedAmount = 0;
                        if (activePlan.TermLengthId == Convert.ToInt32(PlanTermLengthEnum.OneMonth))
                            usedAmount = (usedDays == 0 ? 1 : usedDays) * (ActivePlanPrice / (DateTime.DaysInMonth((activePlan.StartedDate).Year, (activePlan.StartedDate).Month)));
                        else if (activePlan.TermLengthId == Convert.ToInt32(PlanTermLengthEnum.OneYear))
                            usedAmount = (usedDays == 0 ? 1 : usedDays) * (ActivePlanPrice / 365);
                        //else if (activePlan.TermLengthId == Convert.ToInt32(PlanTermLengthEnum.TwoYear))
                        //    usedAmount = (usedDays == 0 ? 1 : usedDays) * (ActivePlanPrice / 365 * 2);
                        //else if (activePlan.TermLengthId == Convert.ToInt32(PlanTermLengthEnum.ThreeYear))
                        //    usedAmount = (usedDays == 0 ? 1 : usedDays) * (ActivePlanPrice / 365 * 3);
                        else
                            usedAmount = 0;

                        var unusedAmount = ActivePlanPrice - usedAmount;
                        int totalCredits = (user.UserCredits != null && user.UserCredits.Count() > 0) ? Convert.ToInt32(user.UserCredits.OrderByDescending(x => x.AddedDate).First().Amount) : 0;
                        totalCredits += unusedAmount;
                        int UsedUserCreditAmount = 0;
                        int remainingCredits = 0;
                        //int newPlanPrice = Convert.ToInt32(accounts * plan.Price);

                        var promoDiscount = 0.0;

                        var planTermLength = plan.PlanTermLength.Where(x => x.PlanTermLengthId == TermLengthId).Single();
                        int newPlanPrice = Convert.ToInt32(accounts) * Convert.ToInt32(plan.Price * planTermLength.Percent * planTermLength.Month) / 100;
                        if (!string.IsNullOrEmpty(promocode))
                        {
                            var coupon = _unitOfWork.CouponDetailRepository.GetAll().Where(x => x.Code == promocode).FirstOrDefault();
                            if (coupon != null)
                                promoDiscount = coupon.Amount;
                        }
                        if (promoDiscount > newPlanPrice)
                            promoDiscount = newPlanPrice;

                        newPlanPrice = newPlanPrice - Convert.ToInt32(promoDiscount);

                        newPlanPrice = newPlanPrice + Convert.ToInt32(plan.InfrastructureCost + Math.Round((Convert.ToDouble(newPlanPrice * 20 / 100)), 2));
                        if (totalCredits == 0)
                        {
                            Price = newPlanPrice;
                        }
                        else if (totalCredits > newPlanPrice)
                        {
                            remainingCredits = totalCredits - newPlanPrice;
                            Price = 0;
                            UsedUserCreditAmount = newPlanPrice;
                        }
                        else if (totalCredits != 0 && newPlanPrice >= totalCredits)
                        {
                            Price = newPlanPrice - totalCredits;
                            remainingCredits = 0;
                            UsedUserCreditAmount = totalCredits;
                        }


                        var serviceFees = Math.Round((((Price) * 0.029) + 0.30), 2);

                        var charge = new StripeCharge();

                        if ((Convert.ToInt32(Price)) != 0 && accounts != 0)
                        {
                            charge = ChargeCustomer(activeCard.CustomerId, Convert.ToInt32(Price + serviceFees));
                        }

                        //if (charge.Id != null && charge.FailureCode==null)
                        //{
                        user.UserPlans.ForEach(x => x.IsActive = false);
                        user.UserPlans.Add(new UserPlan()
                        {
                            PlanID = plan.PlanID,
                            Price = newPlanPrice,
                            // Days = plan.Days,
                            Days = 0,
                            IsActive = true,
                            StartedDate = DateTime.Now,
                            TermLengthId = TermLengthId
                        });

                        if (user.UserInvoiceHistory == null)
                        {
                            user.UserInvoiceHistory = new List<UserInvoiceHistory>();
                        }
                        if ((Convert.ToInt32(Price)) != 0 && accounts != 0)
                        {
                            user.UserInvoiceHistory.Add(new UserInvoiceHistory()
                            {
                                Id = Guid.NewGuid(),
                                PlanId = plan.PlanID,
                                PlanName = plan.Name,
                                Status = charge.Status,
                                Price = Price,
                                Paid = charge.Paid,
                                ApplicationFee = Math.Round((Convert.ToDouble(newPlanPrice * 20 / 100)), 2),
                                ServiceFee = serviceFees,
                                InfrastructureCost = plan.InfrastructureCost,
                                FailureCode = charge.FailureCode,
                                FailureMessage = charge.FailureMessage,
                                TransactionDate = DateTime.Now,
                                UsedUserCreditAmount = UsedUserCreditAmount,
                                AppliedCoupon = promocode,
                                Discount = promoDiscount
                            });
                        }
                        else
                        {
                            user.UserInvoiceHistory.Add(new UserInvoiceHistory()
                            {
                                Id = Guid.NewGuid(),
                                PlanId = plan.PlanID,
                                PlanName = plan.Name,
                                // Status = charge.Status,
                                Price = Price,
                                Paid = true,
                                ApplicationFee = Math.Round((Convert.ToDouble(newPlanPrice * 20 / 100)), 2),
                                ServiceFee = serviceFees,
                                InfrastructureCost = plan.InfrastructureCost,
                                // FailureCode = charge.FailureCode,
                                //  FailureMessage = charge.FailureMessage,
                                TransactionDate = DateTime.Now,
                                UsedUserCreditAmount = UsedUserCreditAmount
                            });
                        }


                        if (user.UserCredits == null)
                        {
                            user.UserCredits = new List<UserCredit>();
                        }
                        user.UserCredits.Add(new UserCredit
                        {
                            Amount = remainingCredits,
                            AddedDate = DateTime.Now
                        });
                        _unitOfWork.UserRepository.Update(user);


                        UserSubscriptionDeductionQueue userDeduction = _unitOfWork.UserSubscriptionDeductionQueueRepository.GetAll().Where(x => x.UserId == userId).SingleOrDefault();
                        if (userDeduction == null)
                        {
                            userDeduction = new UserSubscriptionDeductionQueue();
                            userDeduction.LastDeductionDate = DateTime.Now;
                            userDeduction.PlanId = PlanId;
                            userDeduction.StartedDate = DateTime.Now;
                            userDeduction.UserId = user.Id;
                            userDeduction.TermLengthId = TermLengthId;
                            _unitOfWork.UserSubscriptionDeductionQueueRepository.Add(userDeduction);
                        }
                        else
                        {
                            userDeduction.LastDeductionDate = DateTime.Now;
                            userDeduction.PlanId = PlanId;
                            userDeduction.StartedDate = DateTime.Now;
                            userDeduction.TermLengthId = TermLengthId;
                            _unitOfWork.UserSubscriptionDeductionQueueRepository.Update(userDeduction);
                        }

                        _unitOfWork.UserRepository.Update(user);

                        var plansPermissions = _unitOfWork.SubscriptionPlanRepository.GetAll().Where(r => r.PlanID == PlanId).Single().WidgetsAccess.Where(wa => wa.HasAccess).Select(x => x.WidgetID).ToArray();
                        var CommaSeperatedPlanPermissionIds = string.Join(",", plansPermissions);

                        var response = new ServiceResponse()
                        {
                            Success = true,
                            //Data = user,
                            Data = CommaSeperatedPlanPermissionIds,
                            MultipleData = new Dictionary<string, object>()
                            {{ "userPlan", GetUserSubscriptionPlan(userId) }
                            },

                            Message = "Plan subscribed successfully."
                        };
                        return response;
                        //}
                        //else
                        //{
                        //    return new ServiceResponse { Success = false, Message = CommonMessage.Something_Wrong };
                        //}
                    }
                    else
                    {
                        return new ServiceResponse { Success = false, Message = "Please add one payment method and make it active. After that request has been initiated." };
                    }
                }
                else
                {
                    return new ServiceResponse { Success = false, Message = CommonMessage.Something_Wrong };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void AddUserRequest(ObjectId userId, string message)
        {
            var user = _unitOfWork.UserRepository.GetAll().Where(u => u.Id == userId).Single();
            user.UserRequests.Add(new UserRequest()
            {
                RequestType = 1,
                Details = message,
                StatusID = 1,
            });
            _unitOfWork.UserRepository.Update(user);
        }

        private static StripeCharge ChargeCustomer(string customerId, int amount)
        {
            var myCharge = new StripeChargeCreateOptions
            {
                Amount = amount * 100,
                Currency = "usd",
                CustomerId = customerId
            };

            var chargeService = new StripeChargeService();
            var stripeCharge = chargeService.Create(myCharge);
            return stripeCharge;
        }

        public void AddNotification(ObjectId userId, string notification)
        {
            var user = _unitOfWork.UserRepository.GetAll().Where(u => u.Id == userId).Single();
            user.Notifications.Add(new Notification()
            {
                id = Guid.NewGuid(),
                Text = notification,
                Date = DateTime.Now,
                IsRead = false
            });
            _unitOfWork.UserRepository.Update(user);
        }

        public void SendNotificationToSuperAdmin(ObjectId userId, string name, string notification)
        {
            var admins = _unitOfWork.UserRepository.GetAll().Where(u => u.RoleID == 1).ToList();

            foreach (var user in admins)
            {
                user.Notifications.Add(new Notification()
                {
                    id = Guid.NewGuid(),
                    From = userId,
                    FromName = name,
                    Text = notification,
                    Date = DateTime.Now,
                    IsRead = false
                });
                _unitOfWork.UserRepository.Update(user);
            }
        }

        public ServiceResponse ActivateCard(string cardId, ObjectId userId, bool isActive)
        {
            var user = _unitOfWork.UserRepository.GetAll().Where(u => u.Id == userId).Single();
            foreach (var card in user.UserCards)
            {
                if (cardId == card.Id.ToString())
                {
                    card.IsActive = isActive;
                }
                else
                {
                    card.IsActive = false;
                }
            }

            _unitOfWork.UserRepository.Update(user);

            var response = new ServiceResponse()
            {
                Success = true,
                Data = new CardModel().ToCardModel(user.UserCards)
            };
            return response;
        }

        public ServiceResponse SaveBillingAddress(ObjectId userId, UserBillingInfoModel userBillingInfoModel)
        {
            var user = _unitOfWork.UserRepository.GetAll().Where(u => u.Id == userId).SingleOrDefault();
            if (user != null)
            {
                var billingAddress = user.UserBillingInfos != null ? user.UserBillingInfos.Where(x => x.Id == userBillingInfoModel.Id).SingleOrDefault() : null;
                if (billingAddress == null)
                {

                    if (user.UserBillingInfos == null)
                    {
                        user.UserBillingInfos = new List<UserBillingInfo>();
                    }
                    user.UserBillingInfos.Add(new UserBillingInfo()
                    {
                        Id = Guid.NewGuid(),
                        Address = userBillingInfoModel.Address,
                        Country = userBillingInfoModel.Country,
                        ISO = userBillingInfoModel.ISO,
                        State = userBillingInfoModel.State,
                        City = userBillingInfoModel.City,
                        ZipCode = userBillingInfoModel.ZipCode,
                        CountryCode = userBillingInfoModel.CountryCode,
                        PhoneNumber = userBillingInfoModel.PhoneNumber
                    });

                    _unitOfWork.UserRepository.Update(user);
                    return new ServiceResponse { Success = true, Message = "Billing address details saved successfully." };

                }
                else
                {
                    foreach (var billingInfo in user.UserBillingInfos)
                    {
                        if (userBillingInfoModel.Id == billingInfo.Id)
                        {
                            billingInfo.Address = userBillingInfoModel.Address;
                            billingInfo.Country = userBillingInfoModel.Country;
                            billingInfo.ISO = userBillingInfoModel.ISO;
                            billingInfo.State = userBillingInfoModel.State;
                            billingInfo.City = userBillingInfoModel.City;
                            billingInfo.ZipCode = userBillingInfoModel.ZipCode;
                            billingInfo.CountryCode = userBillingInfoModel.CountryCode;
                            billingInfo.PhoneNumber = userBillingInfoModel.PhoneNumber;
                        }
                    }

                    _unitOfWork.UserRepository.Update(user);

                    return new ServiceResponse { Success = true, Message = "Billing address details updated successfully." };
                }
            }
            else
            {
                return new ServiceResponse { Success = false, Message = "Error saving billing address details." };
            }
        }

        public ServiceResponse GetUserBillingAddresses(ObjectId userId)
        {
            var user = _unitOfWork.UserRepository.GetAll().Where(u => u.Id == userId).SingleOrDefault();
            if (user.UserBillingInfos != null)
            {
                var userBillingInfoModels = user.UserBillingInfos.Select(n => new UserBillingInfoModel()
                {
                    Id = n.Id,
                    Address = n.Address,
                    State = n.State,
                    City = n.City,
                    Country = n.Country,
                    ISO = n.ISO,
                    ZipCode = n.ZipCode,
                    CountryCode = n.CountryCode,
                    PhoneNumber = n.PhoneNumber,
                    IsActive = n.IsActive
                }).ToList();

                return new ServiceResponse { Data = userBillingInfoModels, Success = true };

            }
            else
            {
                return new ServiceResponse { Success = false };
            }
        }

        public ServiceResponse ActivateBillingAddress(string addressId, ObjectId userId, bool isActive)
        {
            var user = _unitOfWork.UserRepository.GetAll().Where(u => u.Id == userId).Single();
            foreach (var billingInfo in user.UserBillingInfos)
            {
                if (addressId == billingInfo.Id.ToString())
                {
                    billingInfo.IsActive = isActive;
                }
                else
                {
                    billingInfo.IsActive = false;
                }
            }

            _unitOfWork.UserRepository.Update(user);

            var response = new ServiceResponse()
            {
                Success = true,
                Data = new UserBillingInfoModel().ToUserBillingModel(user.UserBillingInfos),
                Message = "Updated Successfully"
            };
            return response;
        }

        public ServiceResponse GetUserBillingAddressById(ObjectId userId, string addressId)
        {
            var user = _unitOfWork.UserRepository.GetAll().Where(u => u.Id == userId).SingleOrDefault();
            if (user.UserBillingInfos != null)
            {
                var userBillingInfoModels = user.UserBillingInfos.Where(x => x.Id.ToString() == addressId).Select(n => new UserBillingInfoModel()
                {
                    Id = n.Id,
                    Address = n.Address,
                    State = n.State,
                    City = n.City,
                    Country = n.Country,
                    ISO = n.ISO,
                    ZipCode = n.ZipCode,
                    CountryCode = n.CountryCode,
                    PhoneNumber = n.PhoneNumber,
                    IsActive = n.IsActive
                }).SingleOrDefault();

                return new ServiceResponse { Data = userBillingInfoModels, Success = true };

            }
            else
            {
                return new ServiceResponse { Success = false };
            }
        }

        public ServiceResponse ChangeSettings(int emailNotificationSettingsId, ObjectId userId, bool hasAccess)
        {
            var user = _unitOfWork.UserRepository.GetAll().Where(u => u.Id == userId).Single();
            if (user != null)
            {
                var emailNotification = user.UserEmailNotificationSettings != null ? user.UserEmailNotificationSettings.Where(x => x.NotificationSettingsId == emailNotificationSettingsId).SingleOrDefault() : null;
                if (emailNotification == null)
                {
                    if (user.UserEmailNotificationSettings == null)
                    {
                        user.UserEmailNotificationSettings = new List<UserEmailNotificationSettings>();
                    }
                    user.UserEmailNotificationSettings.Add(new UserEmailNotificationSettings
                    {
                        NotificationSettingsId = emailNotificationSettingsId,
                        HasAccess = hasAccess
                    });
                }
                else
                {
                    foreach (var emailNotificationSetting in user.UserEmailNotificationSettings)
                    {
                        if (emailNotificationSettingsId == emailNotificationSetting.NotificationSettingsId)
                        {
                            emailNotificationSetting.HasAccess = hasAccess;
                        }
                    }
                }
                _unitOfWork.UserRepository.Update(user);

                var response = new ServiceResponse()
                {
                    Success = true,
                    Data = new UserBillingInfoModel().ToUserBillingModel(user.UserBillingInfos)
                };
                return response;
            }
            else
            {
                return new ServiceResponse { Success = false, Message = "Error saving billing address details." };
            }
        }
        public bool IsEmailExist(string email, ObjectId userId)
        {
            return _unitOfWork.UserRepository.GetAll().Any(m => m.EmailID == email && m.Id != userId);
            //var IsUser= _unitOfWork.UserRepository.GetAll().Any(m => m.EmailID == email);
            //return IsUser;
        }

        public ServiceResponse ChangeUserDetails(UserModel userModel, ObjectId userId)
        {
            try
            {
                var user = _unitOfWork.UserRepository.GetAll().Where(u => u.Id == userId).Single();
                if (user != null)
                {
                    if (IsEmailExist(userModel.EmailID, userId)) { return new ServiceResponse { Data = userModel, Success = false, Message = ErrorMessage.Email_Already_Exist }; }
                    user.EmailID = userModel.EmailID;
                    user.CountryCode = userModel.CountryCode;
                    user.PhoneNumber = userModel.PhoneNumber;
                    user.PhoneCountryCode = userModel.PhoneCountryCode;
                    user.Country = userModel.Country;
                    user.ISO = userModel.ISO;
                    user.FirstName = userModel.FirstName;
                    user.LastName = userModel.LastName;
                    user.Name = userModel.Name;
                    user.DOB = Convert.ToDateTime(userModel.DOB).AddHours(6);
                    user.DOB = user.DOB.AddHours(6);
                    _unitOfWork.UserRepository.Update(user);
                    return new ServiceResponse { Data = new UserModel().ToUserModel(user), Success = true, Message = CommonMessage.Updated };
                }
                else
                {
                    return new ServiceResponse { Success = false, Message = "Error saving user details." };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ServiceResponse ChangePassword(string email, string oldPassword, string newPassword)
        {
            var user = _unitOfWork.UserRepository.GetAll().Where(u => u.EmailID == email).Single();
            if (user != null)
            {
                if (CommonFunction.ComparePassword(oldPassword, user.Password))
                {
                    user.Password = newPassword.PasswordEncrypt();
                    _unitOfWork.UserRepository.Update(user);
                    return new ServiceResponse { Success = true, Message = CommonMessage.Updated };
                }
                else
                {
                    return new ServiceResponse { Success = false, Message = "Old password is incorrect." };
                }
            }
            else
            {
                return new ServiceResponse { Success = false, Message = "Error saving password." };
            }
        }

        public List<InvoiceModel> GenerateInvoice(ObjectId userId, string invoiceMonth, string invoiceYear)
        {
            var user = _unitOfWork.UserRepository.GetAll().Where(u => u.Id == userId).SingleOrDefault();
            if (user != null)
            {
                if (user.UserInvoiceHistory == null) { return null; }

                var invoiceHistoryForInvoice = user.UserInvoiceHistory.Where(x => x.TransactionDate.Month.ToString() == invoiceMonth.TrimStart('0') && x.TransactionDate.Year.ToString() == invoiceYear).ToList();
                if (invoiceHistoryForInvoice == null || invoiceHistoryForInvoice.Count == 0) { return null; }
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
                    ServiceFee = x.ServiceFee.ToString(),
                    InfrastructureCost = x.InfrastructureCost.ToString(),
                    SubTotal = Math.Round((x.Price)).ToString(),
                    Total = (Math.Round((x.Price)) + x.ApplicationFee).ToString(),
                    CreditedAmount = x.UsedUserCreditAmount.ToString(),
                    Discount=x.Discount.ToString()
                }).ToList();


                return invoiceModelList;
            }
            else
            {
                return null;
            }
        }



        public static class MathCalculations
        {
            private static Random rnd = new Random();

            public static Int32 GenerateRandomNo(int toPlaces)
            {
                var min = "";
                var max = "";

                for (int i = 0; i < toPlaces; i++)
                {
                    if (i == 0)
                    {
                        min += "1";
                    }
                    else if (i < toPlaces - 1)
                    {
                        min += "0";
                    }
                    max += "9";
                }
                return rnd.Next(Convert.ToInt32(min), Convert.ToInt32(max));
            }
        }

        public ServiceResponse EmailInvoice(List<string> files, ObjectId userId, string invoiceMonth, string invoiceYear)
        {
            var user = _unitOfWork.UserRepository.GetAll().Where(u => u.Id == userId).SingleOrDefault();
            if (user != null)
            {
                var isSent = this.SendEmail(user, files, invoiceMonth, invoiceYear);
                return new ServiceResponse { Success = true, Message = "Mail send successfully." };
            }
            else
            {
                return new ServiceResponse { Success = false, Message = CommonMessage.Something_Wrong };
            }
        }

        public bool SendEmail(User model, List<string> files, string invoiceMonth, string invoiceYear)
        {
            try
            {
                MailHelper mailHelper = new MailHelper();
                mailHelper.ToEmail = model.EmailID;

                mailHelper.Subject = "Invoice";
                mailHelper.Month = invoiceMonth;
                mailHelper.Year = invoiceYear;
                mailHelper.Body = CommonFunction.InvoiceMailBody(model.FirstName + " " + model.LastName);
                mailHelper.AttachmentPath = files;
                mailHelper.SendEmail();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
