'use strict';
app.service('signUpService', ['$http', '$q',function ($http, $q, authData) {
    var userInfo;
    var loginServiceURL = serviceBase + 'api/Account/SignUp';
    var deferred;

    this.createNewUser = function (user) {
        deferred = $q.defer();
        var data = user;
        //"grant_type=password&username=" + userName + "&password=" + password;
        //$.ajax({
        //    method: "POST",
        //    url: loginServiceURL,
        //    data: data,
        //    success: function (response) {  },
        //    error: function (e) { }

        //})
        $http({
            url: loginServiceURL,
            data: data,
            method: "POST"
        }).then(function (response) {
            deferred.resolve(response.data);
        }, function (err, status) {
            deferred.reject(err);
        });
        return deferred.promise;
    }
}]);
