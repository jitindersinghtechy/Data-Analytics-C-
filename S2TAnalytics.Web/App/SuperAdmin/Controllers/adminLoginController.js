'use strict'
app.controller('adminLoginController', ['$scope', 'adminLoginService', '$location', '$timeout', function ($scope, adminLoginService, $location, $timeout) {
    var vm = this;

    vm.User = {
        UserName: "",
        Password: "",
        OTP: ""
    };

    vm.generateOTP = function () {
        adminLoginService.generateOTP(vm.User.UserName, vm.User.Password).then(function (response) {
            if (response.Success) {
                toastr.success(response.Message);
            }
            else {
                toastr.warning(response.Message);
            }
        });
    }
    vm.login = function () {
        vm.isLoginDisabled = true;
        $('.login').append("<div class='preloader show-loder'><img src='../Images/loading-circle.svg' class='img' /></div>");
        adminLoginService.login(vm.User.UserName, vm.User.Password, vm.User.OTP).then(function (response) {
            vm.isLoginDisabled = false;
            if (response.error != 'invalid_grant') {
           
                localStorage.setItem("fullName", response.fullName)
                var arr = [];
                localStorage.setItem('CompareData', JSON.stringify(arr));
                $location.path("/AdminSubscribers");
                //location.href = "#/AdminSubscribers"
            } else {
                toastr.warning(response.error_description);
            }
            $(".preloader").hide();
        });
    }
}])