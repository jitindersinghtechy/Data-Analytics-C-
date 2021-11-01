'use strict';
app.service('pinnedUsersService', ['$http', function ($http) {
    return {
        getPinnedUsers: getPinnedUsers,
        unpinUser: unpinUser,
        updateExcludeUsers: updateExcludeUsers
    };
    function getPinnedUsers(pageRecord) {
        return $http.post(serviceBase + "api/Performers/GetPinnedUsers", pageRecord).success(getAccountsComplete).error(getAccountsFailed);
        function getAccountsComplete(response) {
            return response;
        }
        function getAccountsFailed(err, status) {
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
    function updateExcludeUsers(selectedAccountDetailsIds) {
        return $http.post(serviceBase + "api/Performers/UpdateExcludeUsers", selectedAccountDetailsIds).success(UpdatePinnedUsersComplete).error(UpdatePinnedUsersFailed);
        function UpdatePinnedUsersComplete(response) {
            return response;
        }
        function UpdatePinnedUsersFailed(err, status) {
        }
    }
}]);
