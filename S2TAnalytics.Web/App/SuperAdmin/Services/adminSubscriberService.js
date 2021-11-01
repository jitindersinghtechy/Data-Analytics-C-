'use strict';
app.service('adminSubscriberService', ['$http', function ($http) {
      return {
          getSubscribers: getSubscribers,
          updateUserActivation: updateUserActivation,
          extendUserTrial: extendUserTrial,
          sendReminders: sendReminders
      };
      function getSubscribers(pageRecord) {
          return $http.post(serviceBase + "api/AdminSubscriber/GetSubscribers", pageRecord).success(contactComplete).error(contactFailed);
          function contactComplete(response) {
              return response;
          }
          function contactFailed(err, status) {
          }
      }
       function updateUserActivation(userIds,isActive) {
           return $http.post(serviceBase + "api/AdminSubscriber/UpdateUserActivation/" + isActive, userIds).success(contactComplete).error(contactFailed);
          function contactComplete(response) {
              return response;
          }
          function contactFailed(err, status) {
          }
       }

       function extendUserTrial(userIds, extendDays) {
           return $http.post(serviceBase + "api/AdminSubscriber/ExtendUserTrial/" + extendDays, userIds).success(contactComplete).error(contactFailed);
           function contactComplete(response) {
               return response;
           }
           function contactFailed(err, status) {
           }
       }

       function sendReminders(userIds, reminderId) {
           return $http.post(serviceBase + "api/AdminSubscriber/SendReminders/" + reminderId, userIds).success(contactComplete).error(contactFailed);
           function contactComplete(response) {
               return response;
           }
           function contactFailed(err, status) {
           }
       }
}]);
