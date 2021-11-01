'use strict'
app.service("userDetailsService", ["$http", function ($http) {
    return {
        getUserDetails: getUserDetails,
        getUserInstruments:getUserInstruments,
        updatePinnedUsers: updatePinnedUsers,
        unpinUser: unpinUser,
        getWinLossData: getWinLossData
    };

    function getUserDetails(pageRecord) {
        return $http.post(serviceBase + "api/Performers/GetUserDetails", pageRecord).success(getUserDetailsComplete).error(getUserDetailsFailed);
        function getUserDetailsComplete(response) {
            return response;
        }
        function getUserDetailsFailed(e) {
        }
    }

    function getUserInstruments(records, pageRecord) {
        return $http.post(serviceBase + "api/Performers/GetUserInstrumentalDetails/" + records, pageRecord).success(getUserDetailsComplete).error(getUserDetailsFailed);
        function getUserDetailsComplete(response) {
            return response;
        }
        function getUserDetailsFailed(e) {
        }
    }

    function getWinLossData(instrumentName,timlinId,accountId) {
        return $http.post(serviceBase + "api/Performers/GetWinLossData/" + instrumentName + "/" + timlinId + "/" + accountId).success(getUserDetailsComplete).error(getUserDetailsFailed);
        function getUserDetailsComplete(response) {
            return response;
        }
        function getUserDetailsFailed(e) {
        }
    }

    function updatePinnedUsers(selectedAccountDetailsIds) {

        return $http.post(serviceBase + "api/Performers/UpdatePinnedUsers", selectedAccountDetailsIds).success(UpdatePinnedUsersComplete).error(UpdatePinnedUsersFailed);
        function UpdatePinnedUsersComplete(response) {
            return response;
        }
        function UpdatePinnedUsersFailed(err, status) {
        }
    }
    function unpinUser(accountDetailId) {
        return $http.post(serviceBase + "api/Performers/UnpinUser/" + accountDetailId).success(unpinUserComplete).error(unpinUserFailed);
        function unpinUserComplete(response) {
            return response;
        }
        function unpinUserFailed() {
        }
    }
}])