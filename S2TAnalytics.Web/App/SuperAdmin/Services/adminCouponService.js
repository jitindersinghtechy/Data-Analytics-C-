'use strict';
app.service('adminCouponService', ['$http', function ($http) {
    return {
        getCoupons: getCoupons,
        addNewCoupon: addNewCoupon,
            deleteCoupon:deleteCoupon
       
    };
    function getCoupons() {
        return $http.get(serviceBase + "api/AdminCoupon/GetCoupons").success(contactComplete).error(contactFailed);
        function contactComplete(response) {
            return response;
        }
        function contactFailed(err, status) {
        }
    } 
    function addNewCoupon(couponDetailModel) {

        return $http.post(serviceBase + "api/AdminCoupon/AddNewCoupon", couponDetailModel).success(couponComplete).error(couponFailed);
        function couponComplete(response) {
            return response;
        }
        function couponFailed(err, status) {
        }
    }
    function deleteCoupon(couponId) {

        return $http.post(serviceBase + "api/AdminCoupon/DeleteCoupon/" + couponId).success(couponComplete).error(couponFailed);
        function couponComplete(response) {
            return response;
        }
        function couponFailed(err, status) {
        }

    }
}]);
