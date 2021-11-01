'use strict';
app.controller('signUpController', ['$scope', 'signUpService', '$location', function ($scope, signUpService, $location) {
    var userVM =this;
    userVM.Roles = {
        SuperAdmin: 1,
        Admin:2,
    };
    userVM.Plans = {
        Demo:1
    };

    userVM.User = {
        FirstName: "",
        LastName: "",
        RoleID: "",
        PlanID: "",
        Password: "",
        ConfirmPassword: "",
        EmailID:""
    };

    userVM.createNewUser = function () {
        
        $('.sign-form').append("<div class='preloader'></div>")
        signUpService.createNewUser(userVM.User).then(function (response) {
            if (response.Success) {
                window.location.href = "/";
            } else {
                toastr.warning(response.Message);
            }
            $(".preloader").hide();
        });
    }
}]);
