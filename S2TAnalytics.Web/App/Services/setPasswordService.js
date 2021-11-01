
'use strict';
app.service('setPasswordService', ['$http', '$q',

function ($http, $q) {
    var userInfo;
    var deferred;
    var setPasswordServiceURL = serviceBase + 'api/Account/SetPassword';

    this.SetPassword = function (email,password) {
        deferred = $q.defer();
        var data = { EmailID: email, Password: password }
        setPasswordServiceURL = setPasswordServiceURL;
               
        $http({
            url: setPasswordServiceURL,
            method: "POST",
            data:data
        }).then(function (response) {
            deferred.resolve(response.data);
        }, function (err, status) {
            deferred.reject(err);
        });
        return deferred.promise;
    }

}
]);
