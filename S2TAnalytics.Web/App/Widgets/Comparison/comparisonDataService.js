'use strict';
app.service('comparisonDataService', ['$http', function ($http) {

    return {
        getComparisonData: getComparisonData
    };

    function getComparisonData(timelineId, country, city) {
        
        country = country == undefined ? "" : country;
        city = city == undefined ? "" : city;
        return $http.get(serviceBase + "api/Dashboard/GetComparison?timelineId=" + timelineId + "&country=" + country + "&city=" + city).success(getComparisonDataComplete).error(getComparisonDataFailed);

        function getComparisonDataComplete(response) {
            return response;
        }

        function getComparisonDataFailed(err, status) {

        }

    }

}]);
