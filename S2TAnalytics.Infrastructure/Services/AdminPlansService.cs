using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using S2TAnalytics.Common.Enums;
using S2TAnalytics.Common.Helper;
using S2TAnalytics.DAL.Interfaces;
using S2TAnalytics.Infrastructure.Interfaces;
using S2TAnalytics.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.Infrastructure.Services
{
    public class AdminPlansService : IAdminPlansService
    {
        public readonly IUnitOfWork _unitOfWork;
        public AdminPlansService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ServiceResponse GetPlans()
        {
            var plansIds = Enum.GetValues(typeof(PlansEnum)).Cast<int>().ToList();
            var widgets = new List<EnumHelper>();
            foreach (var w in plansIds)
            {
                var myWidget = new EnumHelper
                {
                    intValue = w,
                    stringValue = Enum.GetName(typeof(PlansEnum),w),
                    DisplayName = ((PlansEnum)Enum.ToObject(typeof(PlansEnum), w)).GetEnumDisplayName()
                };
                widgets.Add(myWidget);
            }
            var plans = _unitOfWork.SubscriptionPlanRepository.GetAll().ToList();
            var result = new ServiceResponse
            {
                MultipleData = new Dictionary<string, object>(){
                        { "widgets", widgets },
                        { "plans", plans },
                    },
                Success = true
            };
            return result;
        }


        public bool UpdatePlans(AdminPlansWidgetModel[] planWidget)
        {

            foreach(var plan in planWidget)
            {
                var PlanID = plan.PlanID;
                var MyPlan = _unitOfWork.SubscriptionPlanRepository.GetAll().Where(x => x.PlanID == PlanID).SingleOrDefault();
                MyPlan.InfrastructureCost = plan.InfrastructureCost;
                 MyPlan.WidgetsAccess = plan.WidgetsAccess;
                _unitOfWork.SubscriptionPlanRepository.Update(MyPlan);
            }
            var result = true;        
            return result;
        }


        public ServiceResponse GetAllNotifications(ObjectId UserID)
        {
            var notifications = _unitOfWork.UserRepository.GetAll().Where(x => x.Id == UserID).Select(x => x.Notifications).FirstOrDefault();
            var allNotifications = notifications != null ? notifications.Where(x => x.IsRead == false).ToList() : null;
            var result = new ServiceResponse
            {
                Data = allNotifications,
                Success = true
            };
            return result;
        }


        public ServiceResponse ReadNotifications(ObjectId userId)
        {
            try
            {
                var user = _unitOfWork.UserRepository.GetAll().Where(u => u.Id == userId).SingleOrDefault();
                user.Notifications.ForEach(x => x.IsRead = true);
                _unitOfWork.UserRepository.Update(user);
                return new ServiceResponse { Success = true };
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    
    
        public ServiceResponse removeSingleNotification(ObjectId userId, Guid id)
        {
            var user = _unitOfWork.UserRepository.GetAll().Where(u => u.Id == userId).SingleOrDefault();
            user.Notifications.RemoveAll(x => x.id == id);
            _unitOfWork.UserRepository.Update(user);
            return new ServiceResponse { Success = true};
        }
    }
}
