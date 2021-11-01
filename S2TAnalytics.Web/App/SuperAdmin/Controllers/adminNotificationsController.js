
'use strict'
app.controller('adminNotificationsController', ['$scope', 'adminNotificationsService', '$location', '$timeout', function ($scope, adminNotificationsService, $location, $timeout) {
    var vm = this;

    vm.Notifications = [];

    var init = function () {
        getNotifications();
    }

    var pageRecordModel = {
        PageNumber: 1,
        PageSize: 1,
        Filters: {},
    };

    vm.UserName = "";
    vm.TotalRecords = 0;
    vm.StartingRecordNumber = 1;
    vm.ToRecordNumber = 0;
    vm.PageSizes = [1, 10, 50, 100, 500];
    vm.selectedPageSize = vm.PageSizes[0];

    vm.changePageSize = function () {
        pageRecordModel.PageSize = vm.selectedPageSize;
        pageRecordModel.PageNumber = 1;
        adminNotificationsService.getUserNotifications(pageRecordModel).then(function (response) {
            vm.Notifications = response.data.response.MultipleData.Notifications;
            getPageData();
        });
    }

    vm.changePage = function (value) {
        if (value == -1 && pageRecordModel.PageNumber > 1 || value == 1 && vm.ToRecordNumber < vm.TotalRecords) {
            pageRecordModel.PageNumber = pageRecordModel.PageNumber + (value);
            adminNotificationsService.getUserNotifications(pageRecordModel).then(function (response) {
                vm.Notifications = response.data.response.MultipleData.Notifications;
                getPageData();
            });
        }
    }

    function getPageData() {
        vm.StartingRecordNumber = ((pageRecordModel.PageNumber - 1) * pageRecordModel.PageSize) + 1;
        var toRecordNumber = pageRecordModel.PageNumber * pageRecordModel.PageSize;
        if (toRecordNumber < vm.TotalRecords) {
            vm.ToRecordNumber = toRecordNumber;
        }
        else {
            vm.ToRecordNumber = vm.TotalRecords;
        }
    }



    vm.changeGridByFilters = function () {
        if (vm.UserName != undefined || vm.UserName != "") {
            pageRecordModel.Filters.Name = vm.UserName;
        }
        else if (pageRecordModel.Filters.hasOwnProperty('Name')) {
            delete pageRecordModel.Filters['Name'];
        }
        adminNotificationsService.getUserNotifications(pageRecordModel).then(function (response) {
            vm.Notifications = response.data.response.MultipleData.Notifications;
            vm.TotalRecords = response.data.response.MultipleData.Total;
        });
    }

    var getNotifications = function () {
        adminNotificationsService.getUserNotifications(pageRecordModel).then(function (response) {
            if (response.data.response.Success) {
                vm.Notifications = response.data.response.MultipleData.Notifications;
                vm.TotalRecords = response.data.response.MultipleData.Total;
                getPageData();
            }
        });
    }

    init();

}]);