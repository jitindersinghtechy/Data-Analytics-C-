using MongoDB.Bson;
using S2TAnalytics.Common.Helper;
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
    [RoutePrefix("api/AdminCoupon")]
    public class AdminCouponController : BaseController
    {
        private IAdminCouponService _adminCouponService;

        public AdminCouponController(IAdminCouponService adminCouponService, IUserService userService) : base(userService)
        {
            _adminCouponService = adminCouponService;
        }
        // GET: AdminCoupon

        [Route("GetCoupons")]
        public IHttpActionResult GetSubscribers()
        {
                var response = _adminCouponService.GetCoupons();
                return Ok(response);
        }
        [HttpPost]
        [Route("AddNewCoupon")]
        public IHttpActionResult AddNewCoupon(CouponDetailModel couponDetailModel)
        {
            try
            {
                var response = _adminCouponService.AddCouponDetail(couponDetailModel);

                return Ok(response);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [HttpPost]
        [Route("DeleteCoupon/{couponId}")]
        public IHttpActionResult DeleteCoupon(string couponId)
        {
            var isDelete=_adminCouponService.DeleteCoupon(couponId);


            return Ok(isDelete);
        }

    }
}