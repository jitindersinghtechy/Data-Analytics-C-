using ClosedXML.Excel;
using MongoDB.Bson;
using S2TAnalytics.Common.Enums;
using S2TAnalytics.Common.Helper;
using S2TAnalytics.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Cors;

namespace S2TAnalytics.Web.Controllers
{
    [Authorize]
    [RoutePrefix("api/Performers")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PerformersController : BaseController
    {
        public readonly IPerformersService _performersService;
        public PerformersController(IPerformersService performersService, IUserService userService) : base(userService)
        {
            _performersService = performersService;
        }
        [HttpPost]
        [Route("GetAccounts")]
        public IHttpActionResult GetAccounts(PageRecordModel pageRecordModel)
        {
            //_accountDetailService.AddDummyAcountDetails();
            try
            {
                pageRecordModel.PageSize = pageRecordModel.PageSize == 0 ? ReadConfiguration.PageSize : pageRecordModel.PageSize;
                pageRecordModel.OrganizationID = OrganizationID;
                pageRecordModel.UserID = UserID;
                pageRecordModel.DatasourceIDs = DatasourceIDs;
                var response = _performersService.GetAccountDetails(pageRecordModel, UserGroups);
                var totalRecords = _performersService.GetAccountDetailsCount(pageRecordModel, UserGroups);
                return Ok(new { response = response, count = totalRecords });
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [HttpPost]
        [Route("GetFilteringData")]
        public IHttpActionResult GetFilteringData(PageRecordModel pageRecordModel)
        {
            try
            {
                //var timeLines = _performersService.getTimeLines();
                pageRecordModel.OrganizationID = OrganizationID;
                pageRecordModel.UserID = UserID;
                pageRecordModel.DatasourceIDs = DatasourceIDs;
                var response = _performersService.GetAccountDetails(pageRecordModel, UserGroups);
                var totalRecords = _performersService.GetAccountDetailsCount(pageRecordModel, UserGroups);
                return Ok(new { records = response, count = totalRecords });
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [HttpPost]
        [Route("GetTop5Performers")]
        public IHttpActionResult GetTop5Performers(PageRecordModel pageRecordModel)
        {
            try
            {
                pageRecordModel.OrganizationID = OrganizationID;
                pageRecordModel.UserID = UserID;
                pageRecordModel.DatasourceIDs = DatasourceIDs;
                var response = _performersService.GetTop5Performers(pageRecordModel, UserGroups);
                return Ok(new { response = response });
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [HttpPost]
        [Route("UpdatePinnedUsers")]
        public IHttpActionResult UpdatePinnedUsers(List<string> selectedAccountDetailsIds)
        {
            var result = _performersService.UpdatePinnedUsers(selectedAccountDetailsIds, OrganizationID, UserID);
            return Ok(result);
        }
        [HttpPost]
        [Route("UpdateExcludeUsers")]
        public IHttpActionResult UpdateExcludeUsers(List<string> selectedAccountDetailsIds)
        {
            var result = _performersService.UpdateExcludeUsers(selectedAccountDetailsIds, OrganizationID, UserID);
            return Ok(result);
        }
        [HttpPost]
        [Route("GetPinnedUsers")]
        public IHttpActionResult GetPinnedUsers(PageRecordModel pageRecordModel)
        {
            pageRecordModel.OrganizationID = OrganizationID;
            pageRecordModel.UserID = UserID;
                pageRecordModel.DatasourceIDs = DatasourceIDs;
            var response = _performersService.GetPinnedUsers(pageRecordModel);
            return Ok(response);
        }
        [HttpPost]
        [Route("GetTopFivePinnedUsers")]
        public IHttpActionResult GetTopFivePinnedUsers(PageRecordModel pageRecordModel)
        {
            pageRecordModel.OrganizationID = OrganizationID;
            pageRecordModel.UserID = UserID;
            pageRecordModel.DatasourceIDs = DatasourceIDs;
            var response = _performersService.GetTopFivePinnedUsers(pageRecordModel);
            return Ok(response);
        }
        [HttpPost]
        [Route("UnpinUser/{accountDetailId}")]
        public IHttpActionResult UnpinUser(string accountDetailId)
        {
            var response = _performersService.UnpinUser(accountDetailId, UserID);
            return Ok(response);
        }
        [HttpPost]
        [Route("GetUserDetails")]
        public IHttpActionResult GetUserDetails(PageRecordModel pageRecordModel)
        {
            pageRecordModel.OrganizationID = OrganizationID;
            pageRecordModel.UserID = UserID;
            pageRecordModel.DatasourceIDs = DatasourceIDs;
            var result = _performersService.GetUserDetails(pageRecordModel, UserID);
            return Ok(result);
        }
        [HttpPost]
        [Route("GetUserInstrumentalDetails/{records}")]
        public IHttpActionResult GetUserInstrumentalDetails(int records, PageRecordModel pageRecordModel)
        {
            pageRecordModel.OrganizationID = OrganizationID;
            pageRecordModel.UserID = UserID;
                pageRecordModel.DatasourceIDs = DatasourceIDs;
            var result = _performersService.GetUserDetailInstrumental(records, pageRecordModel, UserID);
            return Ok(result);
        }
        [HttpPost]
        [Route("GetWinLossData/{instrumentalName}/{timeLineId}/{accountId}")]
        public IHttpActionResult GetWinLossData(string instrumentalName, string timeLineId, string accountId)
        {
            var result = _performersService.GetProfitLossData(instrumentalName, timeLineId, accountId, UserGroups);
            return Ok(result);
        }
        [HttpPost]
        [Route("DownloadExcel")]
        public HttpResponseMessage DownloadExcel(PageRecordModel pageRecordModel)
        {
            pageRecordModel.OrganizationID = OrganizationID;
            pageRecordModel.UserID = UserID;

                pageRecordModel.DatasourceIDs = DatasourceIDs;
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Performers");

            // From a query
            //var list = new List<Person>();
            //list.Add(new Person() { Name = "John", Age = 30, House = "On Elm St." });
            //list.Add(new Person() { Name = "Mary", Age = 15, House = "On Main St." });
            //list.Add(new Person() { Name = "Luis", Age = 21, House = "On 23rd St." });
            //list.Add(new Person() { Name = "Henry", Age = 45, House = "On 5th Ave." });

            var performers = _performersService.GetAccountsForExport(pageRecordModel, UserGroups);
            var performersToExport = from p in performers
                                     select new { p.PerformerName, p.AccountNumber, p.NAV, p.ROI, p.WINRate, p.DD, p.Leverage, p.Location, p.UserGroup };
            //ws.Cell(6, 6).Value = "From Query";
            //ws.Range(6, 6, 6, 8).Merge().AddToNamed("Titles");
            string[] columns = { "Name", "Account Number", "NAV", "ROI", "WIN Rate", "DD", "Leverage", "Location", "UserGroup" };
            foreach (var i in Enumerable.Range(1, 9))
            {
                ws.Cell(1, i).Value = columns[i - 1];

                ws.Cell(1, i).Style.Fill.BackgroundColor = XLColor.BallBlue;
                ws.Cell(1, i).Style.Font.Bold = true;
                ws.Cell(1, i).Style.Font.SetFontColor(XLColor.White);
            }

            ws.Cell(2, 1).InsertData(performersToExport);

            //// Prepare the style for the titles
            //var titlesStyle = wb.Style;
            //titlesStyle.Font.Bold = true;
            //titlesStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            //titlesStyle.Fill.BackgroundColor = XLColor.Cyan;

            //// Format all titles in one shot
            //wb.NamedRanges.NamedRange("Titles").Ranges.Style = titlesStyle;

            ws.Columns().AdjustToContents();

            using (var ms = new MemoryStream())
            {
                wb.SaveAs(ms);

                var result = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(ms.ToArray())
                };
                result.Content.Headers.Add("x-filename", "Performers.xls");
                result.Content.Headers.ContentDisposition =
                    new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                result.Content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/octet-stream");

                return result;
            }
        }
    }

    public class Person
    {
        public String House { get; set; }
        public String Name { get; set; }
        public Int32 Age { get; set; }
    }
}
