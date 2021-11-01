'use strict';
app.service('adminPlansService', ['$http', function ($http) {
      return {
          getAllPlans: getAllPlans,
          updateAllPlans: updateAllPlans
    };
      function getAllPlans() {
          return $http.post(serviceBase + "api/AdminPlans/GetAllPlans").success(contactComplete).error(contactFailed);
        function contactComplete(response) {
            return response;
        }
        function contactFailed(err, status) {
        }
    }

      function updateAllPlans(allWidget) {
          return $http.post(serviceBase +"api/AdminPlans/UpdateAllPlans", allWidget).success(contactComplete).error(contactFailed);
          function contactComplete(response) {
              return response;
          }
          function contactFailed(err, status) {
          }
      }

}]);
