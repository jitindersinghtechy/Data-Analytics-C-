'use strict';
app.service('adminMainService', ['$http', function ($http) {
    return {
        getNotifications: getNotifications,
        readNotifications: readNotifications,
        removeSingleNotifications: removeSingleNotifications,
    };
    function getNotifications() {
        return $http.get(serviceBase + "api/AdminPlans/GetAllNotifications").success(contactComplete).error(contactFailed);
        function contactComplete(response) {
            return response;
        }
        function contactFailed(err, status) {
        }
    }


    function readNotifications() {
        return $http.post(serviceBase + "api/AdminPlans/ReadNotifications").success(contactComplete).error(contactFailed);
        function contactComplete(response) {
            return response;
        }
        function contactFailed(err, status) {
        }
    }

    function removeSingleNotifications(id) {
        return $http.get(serviceBase + "api/AdminPlans/RemoveSingleNotification/"+id).success(contactComplete).error(contactFailed);
        function contactComplete(response) {
            return response;
        }
        function contactFailed(err, status) {
        }
    }
}]);
