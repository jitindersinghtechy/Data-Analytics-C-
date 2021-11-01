'use strict';
app.service('adminLoginService', ['$http', '$q', '$location', function ($http,$q, $location) {
    var userInfo;
    var loginServiceURL = serviceBase + 'token';
    var deferred;

    this.login = function (userName, password, OTP) {
    
        deferred = $q.defer();
        var data = "grant_type=password&username=" + userName + "&password=" + password + "&OTP=" + OTP + "&loginType='superAdmin'";
        $http.post(loginServiceURL, data, {
            headers:
               { 'Content-Type': 'application/x-www-form-urlencoded' }
        }).success(function (response) {
            var o = response;
            userInfo = {
                accessToken: response.access_token,
                userName: response.userName
            };
            localStorage.setItem('roleId', response.RoleID)
            localStorage.setItem('accessToken', response.access_token)
            deferred.resolve(response);
        })
        .error(function (err, status) {
            deferred.resolve(err);
        });
        return deferred.promise;
    }

    this.generateOTP = function (userName, password, OTP) {
        deferred = $q.defer();
        var data = { EmailID: userName, Password: password }
        $http.post(serviceBase + "api/AdminAccount/GenerateOTP", data).success(function (response) {
            var o = response;
            userInfo = {
                accessToken: response.access_token,
                userName: response.userName
            };
            localStorage.setItem('roleId', response.RoleID)
            localStorage.setItem('accessToken', response.access_token)
            deferred.resolve(response);
        })
        .error(function (err, status) {
            deferred.resolve(err);
        });
        return deferred.promise;
    }

    this.logOut = function () {
        localStorage.removeItem('accessToken');
        $location.path("/login");
    }

    this.forgotPassword = function (email) {
        var data = { EmailID: email }
        var deferred1 = $q.defer();
        $http.post(serviceBase + "api/Account/ForgotPassword", data).success(function (response) {
            deferred1.resolve(response);
        })
        .error(function (err, status) {
            deferred1.resolve(err);
        });
        return deferred1.promise;
    }

    
}]);
