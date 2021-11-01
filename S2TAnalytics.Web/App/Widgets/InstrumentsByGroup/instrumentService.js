'use strict';
appInstrumentGroup.service('instrumentService', ['$http', function ($http) {

    return {
        getInstrumentsByLocation: getInstrumentsByLocation,
        getAllInstruments: getAllInstruments,
        getInstrumentsByGroup: getInstrumentsByGroup

    };

    function getInstrumentsByLocation() {

        return $http.get(serviceBase + "/api/Dashboard/GetAccounts").success(getInstrumentsByLocationComplete).error(getInstrumentsByLocationFailed);

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

}]);
