'use strict';
app.service('mainService', ['$http', function ($http) {
    return {
        getSubscriptionPlans: getSubscriptionPlans,
        getPromocodeDiscount: getPromocodeDiscount
    };
    function getSubscriptionPlans() {
        return $http.get(serviceBase + "api/UserAccount/GetSubscriptionPlans").success(contactComplete).error(contactFailed);
        function contactComplete(response) {
            return response;
        }
        function contactFailed(err, status) {
        }
    }

    function getPromocodeDiscount(promoCode) {
        return $http.get(serviceBase + "api/UserAccount/GetPromocodeDiscount/" + promoCode).success(contactComplete).error(contactFailed);
        function contactComplete(response) {
            return response;
        }
        function contactFailed(err, status) {
        }
    }

}]);
