
'use strict';
app.service('emailConfirmService', ['$http', '$q',

function ($http, $q) {
    var userInfo;
    var loginServiceURL = serviceBase + 'api/Account/Confirm';
    var deferred;

    this.SetEmailConfirmed = function (email, code) {
        deferred = $q.defer();
        //var data = "?email=" + email + "code=" + code;
        loginServiceURL = loginServiceURL +'/'+ email + '/' + code;

        $http.get(loginServiceURL).success(function (response) {
            deferred.resolve(response);
        }).error(function (err, status) {
            deferred.resolve(err);
        });



        //$http.post('/User/CalculatePickupCost', modelData).success(function (response) {
        //    deferred.resolve(response);
        //}).error(function (err, status) {
        //    deferred.reject(err);
        //});



        return deferred.promise;
    }

 
}
]);
