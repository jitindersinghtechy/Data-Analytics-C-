'use strict';
app.controller('setPasswordController', ['$scope', 'emailConfirmService', 'setPasswordService', '$location', function ($scope, emailConfirmService, setPasswordService, $location) {
    var setPwdVm = this;
    var params = $location.path().split('/');
    setPwdVm.SetPassword = function () {
        localStorage.setItem("isSetPassword", true);
        localStorage.setItem("SetPasswordEmail", params[2]);
        $location.path("/login");
    }

    setPwdVm.SetPassword();
}]);