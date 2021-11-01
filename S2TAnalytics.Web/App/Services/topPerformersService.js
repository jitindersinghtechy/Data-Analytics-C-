'use strict';
app.service('topPerformersService', ['$http', function ($http) {
    return {
        getTop5Performers: getTop5Performers,
        updatePinnedUsers: updatePinnedUsers,
        unpinUser: unpinUser,
        updateExcludeUsers: updateExcludeUsers,
        SaveEmbedWidgetPermission: SaveEmbedWidgetPermission,
        getWidgetPermission: getWidgetPermission
    };
    function getTop5Performers(pageRecord) {
        return $http.post(serviceBase + "api/Performers/GetTop5Performers", pageRecord).success(getAccountsComplete).error(getAccountsFailed);
        function getAccountsComplete(response) {
            return response;
        }
        function getAccountsFailed(err, status) {
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
    function updateExcludeUsers(selectedAccountDetailsIds) {
        return $http.post(serviceBase + "api/Performers/UpdateExcludeUsers", selectedAccountDetailsIds).success(UpdatePinnedUsersComplete).error(UpdatePinnedUsersFailed);
        function UpdatePinnedUsersComplete(response) {
            return response;
        }
        function UpdatePinnedUsersFailed(err, status) {
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
