using S2TAnalytics.Infrastructure.Interfaces;
using S2TAnalytics.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace S2TAnalytics.Web.Controllers.SuperAdmin
{
    [Authorize]
    [RoutePrefix("api/AdminPlans")]
    public class AdminPlansController : BaseController
    {
        private IAdminPlansService _adminPlansService;

        public AdminPlansController(IAdminPlansService adminPlansService, IUserService userService) : base(userService)
        {
            _adminPlansService = adminPlansService;
        }

        [HttpPost]
        [Route("GetAllPlans")]
        public IHttpActionResult GetAllPlans()
        {
            try
            {
                var response = _adminPlansService.GetPlans();
                return Ok(new { response = response });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("UpdateAllPlans")]
        public IHttpActionResult UpdateAllPlans(AdminPlansWidgetModel[] planWidget)
        {
            try
            {
                var response = _adminPlansService.UpdatePlans(planWidget);
                return Ok(new { response = response });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("GetAllNotifications")]
        public IHttpActionResult GetAllNotifications()
        {
            try
            {
                var response = _adminPlansService.GetAllNotifications(UserID);
                return Ok(new { response = response });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("ReadNotifications")]
        public IHttpActionResult ReadNotifications()
        {
            return Ok(_adminPlansService.ReadNotifications(UserID));
        }

        [HttpGet]
        [Route("RemoveSingleNotification/{id}")]
        public IHttpActionResult RemoveSingleNotification(Guid id)
        {
            return Ok(_adminPlansService.removeSingleNotification(UserID, id));
        }
    }
}