using Newtonsoft.Json.Linq;
using S2TAnalytics.Common.Enums;
using S2TAnalytics.Common.Helper;
using S2TAnalytics.Infrastructure.Interfaces;
using S2TAnalytics.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.UI.HtmlControls;

namespace S2TAnalytics.Web.Controllers
{
    [Authorize]
    [RoutePrefix("api/Dashboard")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class DashboardController : BaseController
    {
        private IAccountDetailService _accountDetailService;
        private IUserService _userService;

        public DashboardController(IAccountDetailService accountDetailService, IUserService userService) : base(userService)
        {
            _accountDetailService = accountDetailService;
            _userService = userService;
        }

        [HttpGet]
        [Route("GetInstrumentStats")]
        public IHttpActionResult GetInstrumentStats(string timelineId = "", string instrument = "", string country = "", string city = "")
        {
            //for (int i = 0; i < 5; i++)
            //{
            //    _accountDetailService.AddDummyAcountDetails();
            //}

            var response = _accountDetailService.GetInstrumentStats(OrganizationID, timelineId.Decrypt(), UserGroups, instrument, country, city);

            return Ok(response);
        }

        [HttpGet]
        [Route("GetInstrumentStatsByGroup")]
        public IHttpActionResult GetInstrumentStatsByGroup(string timelineId, string instrument = "", string userGroup = "")
        {
            //  Guid orgId = Guid.Parse("d5cc177f-5293-6247-bed7-838438c810a3");
            var response = _accountDetailService.GetInstrumentStatsByGroup(OrganizationID, timelineId.Decrypt(), UserGroups, instrument, userGroup);

            return Ok(response);
        }

        [HttpGet]
        [Route("GetComparison")]
        public IHttpActionResult GetComparison(string timelineId, string selectedSeries = "", string country = "", string city = "")
        {
            //Guid orgId = Guid.Parse("d5cc177f-5293-6247-bed7-838438c810a3");
            // timelineId = 7;
            var response = _accountDetailService.GetComparisonData(OrganizationID, timelineId.Decrypt(), UserGroups, country, city, selectedSeries);

            return Ok(response);
        }

        [HttpGet]
        [Route("GetNavMapData")]
        public IHttpActionResult GetNavMapData(string timelineId, string userGroup = "", string country = "", string city = "")
        {
            var response = _accountDetailService.GetNavMapData(OrganizationID, timelineId.Decrypt(), UserGroups, userGroup, country, city);

            return Ok(response);
        }


        [HttpGet]
        [Route("ComparisonView")]
        public HttpResponseMessage ComparisonView()
        {
            bool hasAccess = _userService.IsAuthenticatedDomain(UserID, Convert.ToInt32(EmbedWidgetEnum.Comparison), Request.RequestUri.Host);
            var path = HttpContext.Current.Server.MapPath("/App/Widgets/Comparison/ComparisonData.html");

            string html = File.ReadAllText(@path);
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            resp.Content = new StringContent(html, System.Text.Encoding.UTF8, "text/plain");
            return resp;
        }

        [HttpGet]
        [Route("InstrumentsGroupView")]
        public HttpResponseMessage InstrumentsGroupView()
        {
            bool hasAccess = _userService.IsAuthenticatedDomain(UserID, Convert.ToInt32(EmbedWidgetEnum.InstrumentGroup), Request.RequestUri.Host);
            string html = "<b style='color: red;'>Access Denied.</b>";
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            if (hasAccess)
            {
                var path = HttpContext.Current.Server.MapPath("/App/Widgets/InstrumentsByGroup/InstrumentsGroup.html");
                html = File.ReadAllText(@path);
            }
            resp.Content = new StringContent(html, System.Text.Encoding.UTF8, "text/plain");
            return resp;
        }

        [HttpGet]
        [Route("InstrumentsLocationView")]
        public HttpResponseMessage InstrumentsLocationView()
        {
            bool hasAccess = _userService.IsAuthenticatedDomain(UserID, Convert.ToInt32(EmbedWidgetEnum.InstrumentLocation), Request.RequestUri.Host);
            string html = "<b style='color: red;'>Access Denied.</b>";
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            if (hasAccess)
            {
                var path = HttpContext.Current.Server.MapPath("/App/Widgets/InstrumentsByLocation/InstrumentsLocation.html");
                html = File.ReadAllText(@path);
            }

            resp.Content = new StringContent(html, System.Text.Encoding.UTF8, "text/plain");
            return resp;
        }

        [HttpGet]
        [Route("NavMapView")]
        public HttpResponseMessage NavMapView()
        {
            bool hasAccess = _userService.IsAuthenticatedDomain(UserID, Convert.ToInt32(EmbedWidgetEnum.Nav), Request.RequestUri.Host);
            string html = "<b style='color: red;'>Access Denied.</b>";
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            if (hasAccess)
            {
                var path = HttpContext.Current.Server.MapPath("/App/Widgets/NavMap/NavMapData.html");
                html = File.ReadAllText(@path);
            }
            resp.Content = new StringContent(html, System.Text.Encoding.UTF8, "text/plain");
            return resp;
        }

        [HttpGet]
        [Route("PerformersAndPinnedUsersView")]
        public HttpResponseMessage PerformersAndPinnedUsersView()
        {
            bool hasAccess = _userService.IsAuthenticatedDomain(UserID, Convert.ToInt32(EmbedWidgetEnum.TopPerformersAndPinnedUsers), Request.RequestUri.Host);
            string html = "<b style='color: red;'>Access Denied.</b>";
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            if (hasAccess)
            {
                var path = HttpContext.Current.Server.MapPath("/App/Widgets/TopPerformersAndPinnedUsers/TopPerformersAndPinnedUsers.html");
                html = File.ReadAllText(@path);
            }
            resp.Content = new StringContent(html, System.Text.Encoding.UTF8, "text/plain");
            return resp;
        }

        [HttpGet]
        [Route("TrendingPerformersView")]
        public HttpResponseMessage TrendingPerformersView()
        {
            bool hasAccess = _userService.IsAuthenticatedDomain(UserID, Convert.ToInt32(EmbedWidgetEnum.TrendingPerformers), Request.RequestUri.Host);
            string html = "<b style='color: red;'>Access Denied.</b>";
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            if (hasAccess)
            {
                var path = HttpContext.Current.Server.MapPath("/App/Widgets/TrendingPerformers/TopPerformers.html");

                html = File.ReadAllText(@path);
            }
            resp.Content = new StringContent(html, System.Text.Encoding.UTF8, "text/plain");
            return resp;
        }

        [HttpPost]
        [Route("SaveEmbedWidgetPermission")]
        public IHttpActionResult SaveEmbedWidgetPermission(UserEmbedWidgetPermissionModel userWidgets)
        {
            var response = _userService.SaveEmbedWidgetPermission(UserID, userWidgets.WidgetId.Decrypt(), userWidgets.Domain);

            return Ok(response);
        }

        [HttpGet]
        [Route("GetWidgetPermission")]
        public IHttpActionResult GetWidgetPermission(string widgetId)
        {
            var response = _userService.GetWidgetPermission(UserID, widgetId.Decrypt());

            return Ok(response);
        }
      
    }
}
