'use strict';
app.service('contactService', ['$http', function ($http) {
    return {
        ContactUs: ContactUs,
        getData: getData
    };
    function ContactUs(contactPerson) {
        return $http.post(serviceBase + "api/Account/ContactUs", contactPerson).success(contactComplete).error(contactFailed);
        function contactComplete(response) {
            return response;
        }
        function contactFailed(err, status) {
        }
    }

    function getData() {
        return $http.get(serviceBase + "api/Account/GetData").success(getDataComplete).error(getDataFailed);
        function getDataComplete(response) {
            return response;
        }
        function getDataFailed(err, status) {
        }
    }
}]);
