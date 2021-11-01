'use strict';
app.service('comparisonDataService', ['$http', function ($http) {

    return {
        getComparisonData: getComparisonData,
        SaveEmbedWidgetPermission: SaveEmbedWidgetPermission,
        getWidgetPermission: getWidgetPermission
    };

    function getComparisonData(timelineId,selectedSeries, country, city) {
        
        country = country == undefined ? "" : country;
        city = city == undefined ? "" : city;
        return $http.get(serviceBase + "api/Dashboard/GetComparison?timelineId=" + timelineId + "&selectedSeries=" + selectedSeries + "&country=" + country + "&city=" + city).success(getComparisonDataComplete).error(getComparisonDataFailed);

        function getComparisonDataComplete(response) {
            return response;
        }

        function getComparisonDataFailed(err, status) {

        }

    }

    function SaveEmbedWidgetPermission(widgetId, Domain)
    {
        var data = { WidgetId: widgetId, Domain: Domain }
        return $http.post(serviceBase + "api/Dashboard/SaveEmbedWidgetPermission",data).success(getSaveEmbedWidgetPermissionComplete).error(getSaveEmbedWidgetPermissionFailed);

        function getSaveEmbedWidgetPermissionComplete(response) {
            return response;
        }

        function getSaveEmbedWidgetPermissionFailed(err, status) {

        }
    }

    function getWidgetPermission(widgetId) {

        return $http.get(serviceBase + "api/Dashboard/GetWidgetPermission?widgetId=" + widgetId).success(getWidgetPermissionComplete).error(getWidgetPermissionFailed);

        function getWidgetPermissionComplete(response) {
            return response;
        }

        function getWidgetPermissionFailed(err, status) {

        }

    }
}]);
