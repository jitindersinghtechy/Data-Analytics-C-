'use strict';
app.service('topPinnedUsersService', ['$http', function ($http) {
    return {
        getTopFivePinnedUsers: getTopFivePinnedUsers,
        unpinUser: unpinUser,
        SaveEmbedWidgetPermission: SaveEmbedWidgetPermission,
        getWidgetPermission: getWidgetPermission
    }

    function getTopFivePinnedUsers(pageRecord) {
        return $http.post(serviceBase + "api/Performers/GetTopFivePinnedUsers", pageRecord).success(getTopFivePinnedUsersComplete).error(getTopFivePinnedUsersFailed);
        function getTopFivePinnedUsersComplete(response) {
            return response;
        }
        function getTopFivePinnedUsersFailed() {

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
}])