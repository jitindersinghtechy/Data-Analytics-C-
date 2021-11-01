'use strict';
appTrendingPerformers.service('trendingPerformersService', ['$http', function ($http) {
    return {
        getTop5Performers: getTop5Performers,
        updatePinnedUsers: updatePinnedUsers,
        unpinUser: unpinUser,
        updateExcludeUsers: updateExcludeUsers
    };
    function getTop5Performers(pageRecord) {
        return $http.post(serviceBase + "/api/Performers/GetTop5Performers", pageRecord).success(getAccountsComplete).error(getAccountsFailed);
        function getAccountsComplete(response) {
            return response;
        }
        function getAccountsFailed(err, status) {
        }
    }
    function updatePinnedUsers(selectedAccountDetailsIds) {
        return $http.post(serviceBase + "/api/Performers/UpdatePinnedUsers", selectedAccountDetailsIds).success(UpdatePinnedUsersComplete).error(UpdatePinnedUsersFailed);
        function UpdatePinnedUsersComplete(response) {
            return response;
        }
        function UpdatePinnedUsersFailed(err, status) {
        }
    }
    function unpinUser(accountDetailId) {
        return $http.post(serviceBase + "/api/Performers/UnpinUser/" + accountDetailId).success(unpinUserComplete).error(unpinUserFailed);
        function unpinUserComplete(response) {
            return response;
        }
        function unpinUserFailed() {

        }
    }
    function updateExcludeUsers(selectedAccountDetailsIds) {
        return $http.post(serviceBase + "/api/Performers/UpdateExcludeUsers", selectedAccountDetailsIds).success(UpdatePinnedUsersComplete).error(UpdatePinnedUsersFailed);
        function UpdatePinnedUsersComplete(response) {
            return response;
        }
        function UpdatePinnedUsersFailed(err, status) {
        }
    }
}]);
