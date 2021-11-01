'use strict';
app.service('dashboardService', ['$http', '$q',

function ($http, $q) {
    var deferred;
    this.GetAccounts = function () {
        deferred = $q.defer();
        $.ajax({
            type : "GET",
            url: "/api/Dashboard/GetAccounts",
            success: function (response) {
                deferred.resolve(response);
            },
            error: function (err, status) {
                deferred.reject(err);
            }
        })

        //$http.get("/api/Dashboard/GetAccounts").success(function (response) {
        //    deferred.resolve(response);
        //}).error(function (err, status) {

        //    deferred.resolve(err);
        //});
        return deferred.promise;
    }
}]);
