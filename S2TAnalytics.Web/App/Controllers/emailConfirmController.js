'use strict';
app.controller('emailConfirmController', ['$scope', 'emailConfirmService', '$location', function ($scope, emailConfirmService, $location) {
    var emailConfirmVm = this;
    var params = $location.path().split('/');
    emailConfirmVm.SetEmailConfirmed = function () {
        $('.confirm-email').append("<div class='preloader'></div>")
        emailConfirmService.SetEmailConfirmed(params[2], params[3]).then(function (response) {
            emailConfirmVm.message = response.Message;
            $(".preloader").hide();
            localStorage.setItem("isEmailConfirmed", true);
            $location.path("/login");
        });
    }
    emailConfirmVm.SetEmailConfirmed();
}]);
