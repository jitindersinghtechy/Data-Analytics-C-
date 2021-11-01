using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using S2TAnalytics.Common.Enums;
using S2TAnalytics.Common.Helper;
using S2TAnalytics.DAL.Interfaces;
using S2TAnalytics.DAL.Models;
using S2TAnalytics.Infrastructure.Interfaces;
using S2TAnalytics.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.Infrastructure.Services
{
    public class AdminCouponService:IAdminCouponService
    {
        public readonly IUnitOfWork _unitOfWork;
        public AdminCouponService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<CouponDetailModel> GetCoupons()
        {
            var coupons = _unitOfWork.CouponDetailRepository.GetAll().ToList();
            var result = new CouponDetailModel().ToCouponDetailModel(coupons);
            return result;
        }

        public ServiceResponse AddCouponDetail(CouponDetailModel couponDetailModel)
        {

            var couponDetail = new CouponDetailModel().ToCouponDetail(couponDetailModel);

            if (couponDetail==null)
                  return new ServiceResponse { Data = couponDetailModel,Message="No data found", Success = false, };

            if (_unitOfWork.CouponDetailRepository.GetAll().Any(x=>x.Code==couponDetail.Code))
                return new ServiceResponse { Data = couponDetailModel, Message = "Coupon Code Already Exist", Success = false, };

            _unitOfWork.CouponDetailRepository.Add(couponDetail);

            couponDetailModel = new CouponDetailModel().ToCouponDetailModel(couponDetail);

            return new ServiceResponse { Data = couponDetailModel, Success = true, };
        }
        public bool DeleteCoupon(string couponId)
        {
            var isDelete = false;
            if (!string.IsNullOrEmpty(couponId))
            {
                try
                {
                    _unitOfWork.CouponDetailRepository.Delete(e => e.Id == ObjectId.Parse(couponId));
                    isDelete = true;
                }
                catch (Exception)
                {
                    isDelete = false;
                    throw;
                }
               
            }
            return isDelete;
        }
        



    }
}
