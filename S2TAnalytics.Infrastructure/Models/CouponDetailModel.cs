using S2TAnalytics.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.Infrastructure.Models
{
    public class CouponDetailModel
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; }

        public CouponDetail ToCouponDetail(CouponDetailModel model)
        {
            if (model == null)
                return null;
            return new CouponDetail
            { 
                Code=model.Code,
                Amount = model.Amount,
                Description =model.Description,
            };
        }
        public CouponDetailModel ToCouponDetailModel(CouponDetail couponDetail)
        {
            if (couponDetail == null)
                return null;
            return new CouponDetailModel
            {
                Id=couponDetail.Id.ToString(),
                Code = couponDetail.Code,
                Amount = couponDetail.Amount,
                Description = couponDetail.Description,
            };
        }
        public List<CouponDetailModel> ToCouponDetailModel(List<CouponDetail> couponDetail)
        {
            if (couponDetail.Count<=0)
                return new List<CouponDetailModel>();

            return couponDetail.Select(m => new CouponDetailModel
            {
                Id = m.Id.ToString(),
                Code = m.Code,
                Amount = m.Amount,
                Description = m.Description,
            }).ToList();
        }
    }
}
