using S2TAnalytics.Common.Enums;
using S2TAnalytics.DAL.Models;
using S2TAnalytics.DAL.UnitOfWork;
using Stripe;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.StripeRecurring
{
    class Program
    {
        private static UnitOfWork unitOfWork = new UnitOfWork(ConfigurationManager.AppSettings["ConnectionString"], ConfigurationManager.AppSettings["DBName"]);
        static void Main(string[] args)
        {
            var usersForPaymentDeduction = unitOfWork.UserSubscriptionDeductionQueueRepository.GetAll().ToList();
            foreach (var paymentUser in usersForPaymentDeduction)
            {
                DateTime newDeductionDate;
                if (paymentUser.TermLengthId == Convert.ToInt32(PlanTermLengthEnum.OneMonth))
                {
                    int days = (DateTime.DaysInMonth((paymentUser.LastDeductionDate).Year, ((paymentUser.LastDeductionDate).Month+1)));
                    newDeductionDate = paymentUser.LastDeductionDate.AddDays(days);
                }
                else if(paymentUser.TermLengthId == Convert.ToInt32(PlanTermLengthEnum.OneYear))
                {
                    newDeductionDate = paymentUser.LastDeductionDate.AddDays(365);
                }
                else if (paymentUser.TermLengthId == Convert.ToInt32(PlanTermLengthEnum.OneYear))
                {
                    newDeductionDate = paymentUser.LastDeductionDate.AddDays(365*2);
                }
                else 
                {
                    newDeductionDate = paymentUser.LastDeductionDate.AddDays(365*3);
                }
                
                // DateTime newDeductionDate = paymentUser.LastDeductionDate.AddDays();
                if (DateTime.Now.Date == newDeductionDate.Date)
                {
                    var user = unitOfWork.UserRepository.GetAll().Where(u => u.Id == paymentUser.UserId).SingleOrDefault();
                    if (user != null && user.UserCards != null && user.UserCards.Count() > 0)
                    {
                        var activeCard = user.UserCards.Where(c => c.IsActive).SingleOrDefault();
                        var currentUserPlan = user.UserPlans.Where(up => up.IsActive).SingleOrDefault();
                        if (activeCard != null)
                        {
                            if (currentUserPlan != null)
                            {
                                StripeCharge charge = null;
                               // int newPlanPrice = Convert.ToInt32(accounts) * Convert.ToInt32(plan.Price * planTermLength.Percent * planTermLength.Month) / 100;
                                int price = Convert.ToInt32(currentUserPlan.Price);
                                int userCredits = (user.UserCredits != null && user.UserCredits.Count() > 0) ? Convert.ToInt32(user.UserCredits.OrderByDescending(c => c.AddedDate).First().Amount) : 0;

                                try
                                {
                                    int usedUserCredits = 0;
                                    if (userCredits != 0 && userCredits < price)
                                    {
                                        price = price - userCredits;
                                        usedUserCredits = userCredits;
                                    }
                                    else if (userCredits > price)
                                    {
                                        price = 0;
                                        userCredits = userCredits - price;
                                        usedUserCredits = price;
                                    }

                                    string PlanName = unitOfWork.SubscriptionPlanRepository.GetAll().Where(p => p.PlanID == currentUserPlan.PlanID).Single().Name;
                                    if (price > 0)
                                    {
                                        var ApplicationFees = Math.Round((((price) * 0.029) + 0.30), 2);
                                        charge = ChargeCustomer(activeCard.CustomerId, Convert.ToInt32(price + ApplicationFees));
                                        user.UserInvoiceHistory.Add(new UserInvoiceHistory()
                                        {
                                            Id = Guid.NewGuid(),
                                            PlanId = currentUserPlan.PlanID,
                                            PlanName = PlanName,
                                            Status = charge.Status,
                                            Price = price,
                                            Paid = charge.Paid,
                                            FailureCode = charge.FailureCode,
                                            FailureMessage = charge.FailureMessage,
                                            TransactionDate = DateTime.Now,
                                            UsedUserCreditAmount = usedUserCredits
                                        });
                                    }
                                    else
                                    {
                                        user.UserInvoiceHistory.Add(new UserInvoiceHistory()
                                        {
                                            Id = Guid.NewGuid(),
                                            PlanId = currentUserPlan.PlanID,
                                            PlanName = PlanName,
                                            Status = "succeeded",
                                            Price = price,
                                            Paid = true,
                                            TransactionDate = DateTime.Now,
                                            UsedUserCreditAmount = usedUserCredits
                                        });
                                    }

                                    if (userCredits > 0)
                                        user.UserCredits.Add(new UserCredit() { AddedDate = DateTime.Now, Amount = userCredits });

                                    paymentUser.LastDeductionDate = DateTime.Now;
                                    unitOfWork.UserSubscriptionDeductionQueueRepository.Update(paymentUser);
                                    unitOfWork.UserRepository.Update(user);

                                }
                                catch (Exception ex)
                                {
                                    user.UserPlans.Remove(currentUserPlan);
                                    currentUserPlan.IsCardDeductionError = true;
                                    currentUserPlan.IsActive = false;
                                    user.UserPlans.Add(currentUserPlan);
                                    unitOfWork.UserRepository.Update(user);
                                }
                            }
                        }
                        else
                        {
                            user.UserPlans.Remove(currentUserPlan);
                            currentUserPlan.IsActive = false;
                            user.UserPlans.Add(currentUserPlan);
                            unitOfWork.UserRepository.Update(user);
                        }
                    }
                }
            }
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
    }
}
