'use strict';
app.service('configureService', ['$http', function ($http) {
    return {
        getUsers: getUsers,
        getDatasources: getDatasources,
        addUser: addUser,
        saveDatasource: saveDatasource,
        updateDataTransformation: updateDataTransformation,
        getUsersList: getUsersList,
        updateUsersAccess: updateUsersAccess,
        deleteUsers: deleteUsers,
        disconnectDatasource: disconnectDatasource,
        getNotifications: getNotifications,
        changeUserGroup: changeUserGroup,
        changeDataSource: changeDataSource,
        checkForSynronizatedDataSource: checkForSynronizatedDataSource,
        updateSelectedDataSources: updateSelectedDataSources
    };
    function getUsers() {
        return $http.get(serviceBase + "api/Configure/GetUsers").success(getUsersComplete).error(getUsersFailed);
        function getUsersComplete(response) {
            return response;
        }
        function getUsersFailed(err, status) {

        }
    }
    function getUsersList(pageRecordModel) {
        return $http.post(serviceBase + "api/Configure/GetUsersList", pageRecordModel).success(getUsersListComplete).error(getUsersListFailed);
        function getUsersListComplete(response) {
            return response;
        }
        function getUsersListFailed(err, status) {

        }
    }

    function deleteUsers(emailIds) {
        return $http.post(serviceBase + "api/Configure/DeleteUsers", emailIds).success(deleteUsersComplete).error(deleteUsersFailed);
        function deleteUsersComplete(response) {
            return response;
        }
        function deleteUsersFailed(err, status) {
        }
    }

    function updateUsersAccess(IsBlock, emailIds) {
        var data = { IsBlock: IsBlock, emailIds: emailIds }
        return $http.post(serviceBase + "api/Configure/UpdateUserAccess", data).success(updateUsersAccessComplete).error(updateUsersAccessFailed);
        function updateUsersAccessComplete(response) {
            return response;
        }
        function updateUsersAccessFailed(err, status) {
        }
    }

    function getDatasources() {
        return $http.get(serviceBase + "api/Configure/GetDatasources").success(getDatasourcesComplete).error(getDatasourcesFailed);
        function getDatasourcesComplete(response) {
            return response;
        }
        function getDatasourcesFailed(err, status) {

        }
    }
    function addUser(userDetails) {
        return $http.post(serviceBase + "api/Configure/AddUser", userDetails).success(AddUserComplete).error(AddUserFailed);
        function AddUserComplete(response) {
            return response;
        }
        function AddUserFailed(err, status) {
        }
    }
    function saveDatasource(datasource) {
        return $http.post(serviceBase + "api/Configure/SaveDatasource", datasource).success(SaveDatasourceComplete).error(SaveDatasourceFailed);
        function SaveDatasourceComplete(response) {
            return response;
        }
        function SaveDatasourceFailed(err, status) {
        }
    }

    function updateDataTransformation(datasource) {
        return $http.post(serviceBase + "api/Configure/UpdateDataTransformation", datasource).success(SaveDatasourceComplete).error(SaveDatasourceFailed);
        function SaveDatasourceComplete(response) {
            return response;
        }
        function SaveDatasourceFailed(err, status) {
        }
    }

    

    function saveRORByDatasource(datasource) {
        return $http.post(serviceBase + "api/Configure/SaveRORByDatasource", datasource).success(SaveDatasourceComplete).error(SaveDatasourceFailed);
        function SaveDatasourceComplete(response) {
            return response;
        }
        function SaveDatasourceFailed(err, status) {
        }
    }


    function disconnectDatasource(datasource) {
        return $http.post(serviceBase + "api/Configure/DisconnectDatasource", datasource).success(DisconnectDatasourceComplete).error(DisconnectDatasourceFailed);
        function DisconnectDatasourceComplete(response) {
            return response;
        }
        function DisconnectDatasourceFailed(err, status) {
        }
    }

    function getNotifications() {

        return $http.get(serviceBase + "api/Configure/GetNotifications").success(GetNotificationsComplete).error(GetNotificationsFailed);
        function GetNotificationsComplete(response) {
            return response;
        }
        function GetNotificationsFailed(err, status) {
        }
    }

    //Remove Notification
    function removeSingleNotification(id) {
        return $http.get(serviceBase + "api/Configure/RemoveSingleNotification/" + id).success(GetNotificationsComplete).error(GetNotificationsFailed);
        function GetNotificationsComplete(response) {
            return response;
        }
        function GetNotificationsFailed(err, status) {
        }
    }
    function removeAllNotification() {
        return $http.get(serviceBase + "api/Configure/RemoveAllNotification").success(GetNotificationsComplete).error(GetNotificationsFailed);
        function GetNotificationsComplete(response) {
            return response;
        }
        function GetNotificationsFailed(err, status) {
        }
    }

    function changeUserGroup(emailIds, userGroups) {
        var data = { emailIds: emailIds, userGroups: userGroups }
        return $http.post(serviceBase + "api/Configure/ChangeUserGroup", data).success(changeUserGroupComplete).error(changeUserGroupFailed);
        function changeUserGroupComplete(response) {
            return response;
        }
        function changeUserGroupFailed(err, status) {
        }
    }

    function changeDataSource(emailIds, dataSourceIds) {
        var data = { emailIds: emailIds, dataSourceIds: dataSourceIds }
        return $http.post(serviceBase + "api/Configure/ChangeDataSource", data).success(changeDataSourceComplete).error(changeDataSourceFailed);
        function changeDataSourceComplete(response) {
            return response;
        }
        function changeDataSourceFailed(err, status) {
        }
    }

    function checkForSynronizatedDataSource(id) {
        return $http.get(serviceBase + "api/Configure/CheckForSynronizatedDataSource/" + id).success(checkForSyncronizedDataSourceComplete).error(checkForSyncronizedDataSourceFailed);
        function checkForSyncronizedDataSourceComplete(response) {
            return response;
        }
        function checkForSyncronizedDataSourceFailed(err, status) {
        }
    }

    function updateSelectedDataSources(id) {
        return $http.post(serviceBase + "api/Configure/UpdateSelectedDataSources/"+ id).success(changeDatasourcesComplete).error(changeDatasourcesFailed);
        function changeDatasourcesComplete(response) {
            if (response.Success) {
            }
        }
        function changeDatasourcesFailed(err, status) {
        }
    }
}]);
