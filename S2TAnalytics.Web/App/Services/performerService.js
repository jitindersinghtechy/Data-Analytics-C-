'use strict';
app.service('performerService', ['$http', function ($http) {
    return {
        getAccounts: getAccounts,
        getFilteringData: getFilteringData,
        updatePinnedUsers: updatePinnedUsers,
        updateExcludeUsers: updateExcludeUsers,
        exportExcel: exportExcel
    };
    function getAccounts(pageRecord) {
        return $http.post(serviceBase + "api/Performers/GetAccounts", pageRecord).success(getAccountsComplete).error(getAccountsFailed);
        function getAccountsComplete(response) {
            return response;
        }
        function getAccountsFailed(err, status) {
        }
    }

    function getFilteringData(pageRecord) {
        return $http.post(serviceBase + "api/Performers/GetFilteringData", pageRecord).success(GetFilteringDataComplete).error(GetFilteringDataFailed);
        function GetFilteringDataComplete(response) {
            return response;
        }
        function GetFilteringDataFailed(err, status) {
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

    function updateExcludeUsers(selectedAccountDetailsIds) {
        return $http.post(serviceBase + "api/Performers/UpdateExcludeUsers", selectedAccountDetailsIds).success(UpdatePinnedUsersComplete).error(UpdatePinnedUsersFailed);
        function UpdatePinnedUsersComplete(response) {
            return response;
        }
        function UpdatePinnedUsersFailed(err, status) {
        }
    }

    function exportExcel(pageRecord) {
        $http({
            method: 'POST',
            url: serviceBase + "api/Performers/DownloadExcel",
            data: pageRecord,
            responseType: 'arraybuffer'
        }).success(exportExcelComplete).error(exportExcelFailed);

        function exportExcelComplete(data, status, headers) {
            headers = headers();
            var filename = headers['x-filename'];
            var contentType = headers['content-type'];

            var linkElement = document.createElement('a');
            try {
                var blob = new Blob([data], { type: contentType });
                var url = window.URL.createObjectURL(blob);

                linkElement.setAttribute('href', url);
                linkElement.setAttribute("download", filename);

                var clickEvent = new MouseEvent("click", {
                    "view": window,
                    "bubbles": true,
                    "cancelable": false
                });
                linkElement.dispatchEvent(clickEvent);
            } catch (ex) {
                console.log(ex);
            }
        }
        function exportExcelFailed(err, status) {
        }
    }


}]);
