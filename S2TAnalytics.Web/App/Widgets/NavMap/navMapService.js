'use strict';
appNavMap.service('navMapService', ['$http', function ($http) {

    return {
        getNavMapData: getNavMapData
    };

    function getNavMapData(timelineId, userGroup, country, city) {
        
        userGroup = userGroup == undefined ? "" : userGroup;
        country = country == undefined ? "" : country;
        city = city == undefined ? "" : city;
        return $http.get(serviceBase + "api/Dashboard/GetNavMapData?timelineId=" + timelineId + "&userGroup=" + userGroup + "&country=" + country + "&city=" + city).success(getNavMapDataComplete).error(getNavMapDataFailed);

        function getNavMapDataComplete(response) {
            return response;
        }

        function getNavMapDataFailed(err, status) {

        }

    }

}]);
