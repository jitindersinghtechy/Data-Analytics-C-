using MongoDB.Bson;
using S2TAnalytics.Common.Helper;
using S2TAnalytics.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.Infrastructure.Interfaces
{
  public  interface IAdminCouponService
    {
        List<CouponDetailModel> GetCoupons();
        ServiceResponse AddCouponDetail(CouponDetailModel couponDetailModel);
        bool DeleteCoupon(string couponId);
    }
}
