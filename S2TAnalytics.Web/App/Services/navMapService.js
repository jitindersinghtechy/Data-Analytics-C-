'use strict';
app.service('navMapService', ['$http', function ($http) {

    return {
        getNavMapData: getNavMapData,
        SaveEmbedWidgetPermission: SaveEmbedWidgetPermission,
        getWidgetPermission: getWidgetPermission
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

    function SaveEmbedWidgetPermission(widgetId, Domain) {
        var data = { WidgetId: widgetId, Domain: Domain }
        return $http.post(serviceBase + "api/Dashboard/SaveEmbedWidgetPermission", data).success(getSaveEmbedWidgetPermissionComplete).error(getSaveEmbedWidgetPermissionFailed);

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
