using S2TAnalytics.Infrastructure.Interfaces;
using S2TAnalytics.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace S2TAnalytics.Web.Controllers.SuperAdmin
{
   
    [RoutePrefix("api/AdminAccount")]
    public class AdminAccountController : ApiController
    {
        private IUserService _userService;
        private IAdminAuthenticateService _adminAuthenticateService;
        public AdminAccountController(IUserService userService, IAdminAuthenticateService adminAuthenticateService)
        {
            _userService = userService;
            _adminAuthenticateService = adminAuthenticateService;
        }

        [HttpPost]
        [Route("GenerateOTP")]
        public IHttpActionResult GenerateOTP(UserViewModel userVM)
        {

            //var userModel = new UserViewModel().ToUserModel(userVM);
            var response = _userService.GenerateOTP(userVM.EmailID, userVM.Password);
            return Ok(response);
        }
    }
}
