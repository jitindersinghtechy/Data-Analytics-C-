using MongoDB.Bson;
using S2TAnalytics.Common.Helper;
using S2TAnalytics.Infrastructure.Interfaces;
using System;
using System.Web.Http;
using System.Web.Http.Cors;

namespace S2TAnalytics.Web.Controllers
{
    [Authorize]
    [RoutePrefix("api/Compare")]
    //[EnableCors(origins: "http://103.231.77.115", headers: "*", methods: "*")]
    public class CompareController : BaseController
    {
        public readonly ICompareService _compareService;
        public CompareController(ICompareService compareService, IUserService userService) :base(userService)
        {
            _compareService = compareService;
        }

        [HttpPost]
        [Route("GetCompareData")]
        public IHttpActionResult GetCompareData()
        {
            try
            {
                var response = _compareService.GetCompareData(OrganizationID, UserID);
                return Ok(new { response = response });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("GetSingleAccount/{accountId}/{TimeLineId}")]
        public IHttpActionResult GetSingleAccount(string accountId, string TimeLineId)
        {
            try
            {
                var response = _compareService.GetSingleAccount(accountId, TimeLineId.Decrypt(), OrganizationID);
                return Ok(new { response = response });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("GetSingleAccountByAccountID/{accountId}/{TimeLineId}")]
        public IHttpActionResult GetSingleAccountByAccountID(string accountId, string TimeLineId)
        {
            try
            {
                var response = _compareService.GetSingleAccountByAccountID(accountId, TimeLineId.Decrypt(), OrganizationID);
                return Ok(new { response = response });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}