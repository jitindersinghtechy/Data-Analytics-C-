'use strict';
app.service('compareService', ['$http', function ($http) {
     this.getCompareData = function() {
        return $http.post(serviceBase + "api/Compare/GetCompareData").success(GetDataComplete).error(GetDataFailed);
        function GetDataComplete(response) {
            return response;
        }
        function GetDataFailed(err, status) {
            return err;
        }
     }

     this.getSingleAccount = function (AccountNo,TimelineId) {
         var detail = { 'accountId': AccountNo, 'TimeLineId': TimelineId };
         return $http.get(serviceBase + "api/Compare/GetSingleAccount/"+AccountNo+"/"+TimelineId).success(GetDataComplete).error(GetDataFailed);
         function GetDataComplete(response) {
             return response;
         }
         function GetDataFailed(err, status) {
             return err;
         }
     }

     this.getSingleAccountByAccountID = function (AccountId, TimelineId) {
         return $http.get(serviceBase + "api/Compare/GetSingleAccountByAccountID/" + AccountId + "/" + TimelineId).success(GetDataComplete).error(GetDataFailed);
         function GetDataComplete(response) {
             return response;
         }
         function GetDataFailed(err, status) {
             return err;
         }
     }
}]);
