'use strict';
app.service('instrumentService', ['$http', function ($http) {

    return {
        getInstrumentsByLocation: getInstrumentsByLocation,
        getAllInstruments: getAllInstruments,
        getInstrumentsByGroup: getInstrumentsByGroup,
        SaveEmbedWidgetPermission: SaveEmbedWidgetPermission,
        getWidgetPermission: getWidgetPermission

    };

    function getInstrumentsByLocation() {

        return $http.get(serviceBase + "api/Dashboard/GetAccounts").success(getInstrumentsByLocationComplete).error(getInstrumentsByLocationFailed);

        function getInstrumentsByLocationComplete(response) {
            return response;
        }

        function getInstrumentsByLocationFailed(err, status) {

        }

    }

    function getAllInstruments(timelineId, instrument, country, city) {

        instrument = instrument == undefined ? "" : instrument;
        country = country == undefined ? "" : country;
        city = city == undefined ? "" : city;
        return $http.get(serviceBase + "api/Dashboard/GetInstrumentStats?timelineId=" + timelineId + "&instrument=" + instrument + "&country=" + country + "&city=" + city).success(getInstrumentsByLocationComplete).error(getInstrumentsByLocationFailed);

        function getInstrumentsByLocationComplete(response) {
            return response;
        }

        function getInstrumentsByLocationFailed(err, status) {

        }

    }

    function getInstrumentsByGroup(timelineId, instrument, userGroup) {

        instrument = instrument == undefined ? "" : instrument;
        userGroup = userGroup == undefined ? "" : userGroup;
        return $http.get(serviceBase + "api/Dashboard/GetInstrumentStatsByGroup?timelineId=" + timelineId + "&instrument=" + instrument + "&userGroup=" + userGroup).success(getInstrumentsByGroupComplete).error(getInstrumentsByGroupFailed);

        function getInstrumentsByGroupComplete(response) {
            return response;
        }

        function getInstrumentsByGroupFailed(err, status) {

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
