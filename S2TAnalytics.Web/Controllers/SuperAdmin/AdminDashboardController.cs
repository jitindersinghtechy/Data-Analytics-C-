using S2TAnalytics.Common.Helper;
using S2TAnalytics.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.UI.HtmlControls;

namespace S2TAnalytics.Web.Controllers
{
    [Authorize]
    [RoutePrefix("api/AdminDashboard")]
 
    public class AdminDashboardController : BaseController
    {
        private IAdminAuthenticateService _accountDetailService;

        public AdminDashboardController(IAdminAuthenticateService accountDetailService, IUserService userService) : base(userService)
        {
            _accountDetailService = accountDetailService;

        }
    }
}
