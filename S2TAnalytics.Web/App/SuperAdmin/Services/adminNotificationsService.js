'use strict';
app.service('adminNotificationsService', ['$http', function ($http) {
      return {
          getUserNotifications: getUserNotifications,

      };
      function getUserNotifications(pageRecord) {
          return $http.post(serviceBase + "api/AdminSubscriber/GetUserNotifications", pageRecord).success(contactComplete).error(contactFailed);
          function contactComplete(response) {
              return response;
          }
          function contactFailed(err, status) {
          }
      }

}]);
