'use strict'
app.controller('adminCouponController', ['$scope', 'adminCouponService', '$location', '$timeout', function ($scope, adminCouponService, $location, $timeout) {
    var vm = this;
    vm.Coupons = [];
    vm.Coupon = {};

    var init = function () {
        getCoupons();
    }


   

    vm.AddCoupon = function () {
        if (parseInt(vm.Coupon.Amount) > 0) {
            adminCouponService.addNewCoupon(vm.Coupon).then(function (response) {
                if (response.data.Success) {
                    $("#couponPopUp").modal('hide');
                    vm.Coupon = {};
                    getCoupons();
                    toastr.success("Coupon code has been added successfully .")
                } else {

                    toastr.warning(response.data.Message);
                }
            });
        }
        else
            toastr.warning("Amount should be grater then 0.")
    }
    vm.DeleteCoupon = function (couponId) {

        BootstrapDialog.confirm({
            title: 'Delete Coupon(S)',
            message: 'Are you sure you want to delete selected Coupon(s)?',

            callback: function (result) {
                // result will be true if button was click, while it will be false if users close the dialog directly.
                if (result) {
                    adminCouponService.deleteCoupon(couponId).then(function (response) {
                        if (response.data) {
                            toastr.success("Coupon code has been successfully deleted.")
                        } else {
                            toastr.warning("Something went wrong.")
                        }
                        getCoupons();
                    });
                }
            }
        });


    }

    var getCoupons = function () {
        adminCouponService.getCoupons().then(function (response) {
            // if (response.data.response.Success) {
            vm.Coupons = response.data;
            //}
        });
    }

    vm.ResetValidation= function(){
        $("#reset1").click();
    }

    init();
}])