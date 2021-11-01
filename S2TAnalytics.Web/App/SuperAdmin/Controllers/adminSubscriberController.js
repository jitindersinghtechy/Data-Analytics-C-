'use strict'
app.controller('adminSubscriberController', ['$scope', 'adminSubscriberService', '$location', '$timeout', function ($scope, adminSubscriberService, $location, $timeout) {
    var vm = this;

    vm.Subscribers = [];
    vm.Plans = [];
    vm.Reminders = [];
    vm.ExtendDays = "10";
    vm.ReminderId="";
    var init= function()
    {
        getSubscribers();
    }
   
    var pageRecordModel = {
        PageNumber: 1,
        PageSize: 10,
        Filters: {},
        SortOrder: "asc",
        SortBy: "",
    };

    vm.TotalRecords = 0;
    vm.StartingRecordNumber = 1;
    vm.ToRecordNumber = 0;
    vm.PageSizes = [10, 50, 100, 500];
    vm.selectedPageSize = vm.PageSizes[0];


    vm.SortSubscribers = function (sortColumn) {
        vm.sortColumn = sortColumn;
        if (vm.lastSort != sortColumn) {
            vm.orderBy = true;
            vm.lastSort = sortColumn;
        }
        else {
            vm.orderBy = !vm.orderBy;
        }
        vm.sortOrder = vm.orderBy == true ? 'asc' : 'desc';
        pageRecordModel.PageNumber = 1;
        pageRecordModel.SortBy = sortColumn;
        pageRecordModel.SortOrder = vm.sortOrder;
        adminSubscriberService.getSubscribers(pageRecordModel).then(function (response) {
            vm.Subscribers = response.data.response.MultipleData.Subscribers;
            vm.TotalRecords = vm.Subscribers.length;
            getPageData();
        });
    }


    vm.changePageSize = function () {
        pageRecordModel.PageSize = vm.selectedPageSize;
        pageRecordModel.PageNumber = 1;
        adminSubscriberService.getSubscribers(pageRecordModel).then(function (response) {
            vm.Subscribers = response.data.response.MultipleData.Subscribers;     
            getPageData();
        });
    }

    vm.changePage = function (value) {
        if (value == -1 && pageRecordModel.PageNumber > 1 || value == 1 && vm.ToRecordNumber < vm.TotalRecords) {
            pageRecordModel.PageNumber = pageRecordModel.PageNumber + (value);
            adminSubscriberService.getSubscribers(pageRecordModel).then(function (response) {
                vm.Subscribers = response.data.response.MultipleData.Subscribers;
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
        if (vm.PlanFilter != undefined || vm.PlanFilter != "") {
            pageRecordModel.Filters.PlanFilter = vm.PlanFilter;
        }
        else if (pageRecordModel.Filters.hasOwnProperty('PlanFilter')) {
            delete pageRecordModel.Filters['PlanFilter'];
        }

        if (vm.PaymentFilter != undefined || vm.PaymentFilter != "") {
            pageRecordModel.Filters.PaymentFilter = vm.PaymentFilter;
        }
        else if (pageRecordModel.Filters.hasOwnProperty('PaymentFilter')) {
            delete pageRecordModel.Filters['PaymentFilter'];
        }

        adminSubscriberService.getSubscribers(pageRecordModel).then(function (response) {
            vm.Subscribers = response.data.response.MultipleData.Subscribers;
            vm.TotalRecords = vm.Subscribers.length;
        });
    }

    vm.viewSubscribersList = function () {
        var Count = 0;
        var selectedSubscriber = "";
        angular.forEach(vm.selectedSubscriber.UserId, function (value, accountId) {
            if (value == true) {
                selectedSubscriber = accountId;
                Count++;
            }
        })
        if (Count > 1 || Count == 0) {
        }
        else {
            $location.path("/AdminViewDetails/" + selectedSubscriber);
            //localStorage.setItem('selectedTimelineId', vm.TimeLine.stringValue)
        }
    }


   


    vm.updateUserActivation = function (isActive) {
        var Count = 0;
        var selectedSubscriber = "";
        //var userIds = Object.getOwnPropertyNames(vm.selectedSubscriber.UserId);

        if (vm.selectedSubscriber != undefined) {
            var userIds = $.map(vm.selectedSubscriber.UserId, function (item, key) {
                if (item)
                    return key;
            });
            if (userIds.length>0)
            adminSubscriberService.updateUserActivation(userIds, isActive).then(function (response) {
                //getSubscribers();
                (isActive ? toastr.success("Enable Subscriber") : toastr.error("Disable Subscriber"));
            });
        }
    }

    vm.ExtendUserTrial = function () {
        var Count = 0;
        var selectedSubscriber = "";
        if (vm.selectedSubscriber != undefined) {
            BootstrapDialog.confirm({
                title: 'EXTEND TRIAL',
                message: 'Are you sure you want to extend trial of selected subscriber(s)?',
                callback: function (result) {

                    angular.forEach(vm.selectedSubscriber.UserId, function (data, Id) {
                            angular.forEach(vm.Subscribers, function (val, index) {                   
                            if (val.UserID == Id && val.IsTrial==false)
                                toastr.warning("User " + val.Name + " dont have any trial");
                        });
                    });


                    var userIds = $.map(vm.selectedSubscriber.UserId, function (item, key) {
                        if (item)
                            return key;
                    });
                    if (userIds.length > 0)
                        adminSubscriberService.extendUserTrial(userIds, parseInt(vm.ExtendDays)).then(function (response) {
                            if (response)
                                toastr.success("Seccessfully Updated");
                        });
                }
            });
        }
    }



    vm.checkAll = function () {
       // vm.selectedSubscriber = {};        
        angular.forEach(vm.Subscribers,function(val,ind){
            val.checked = vm.checkedAll ? true : false;
        })
    }
    vm.singleCheck = function () {
        vm.selectedSubscriber.UserId;
        vm.checkedAll =vm.checkedAll?false:undefined;
    }



    $scope.$on('searchOrganizationName', function (event, args) {
        if (args.data != undefined || args.data != "") {
            pageRecordModel.Filters.Name = args.data;
        }
        else if (pageRecordModel.Filters.hasOwnProperty('Name')) {
            delete pageRecordModel.Filters['Name'];
        }

        getSubscribers();
    });



    var getSubscribers = function () {
        adminSubscriberService.getSubscribers(pageRecordModel).then(function (response) {
            if (response.data.response.Success) {
                vm.Subscribers = response.data.response.MultipleData.Subscribers;
                vm.TotalRecords = vm.Subscribers.length;
                vm.Plans = response.data.response.MultipleData.Plans;
                vm.Reminders = response.data.response.MultipleData.SubscriberReminders;
                getPageData();
            }
        });
    }


      vm.SendReminders = function () {
        var Count = 0;
        var selectedSubscriber = "";

        if (vm.ReminderId == "") {
            toastr.warning("Please select a reminder type.");
        }
        else {
             if (vm.selectedSubscriber != undefined) {
                BootstrapDialog.confirm({
                    title: 'SEND REMINDERS(S)',
                    message: 'Are you sure you want to send reminder to  selected subscribers(s)?',
                    callback: function (result) {
                        var userIds = $.map(vm.selectedSubscriber.UserId, function (item, key) {
                            if (item)
                                return key;
                        });
                        if (userIds.length > 0)
                            adminSubscriberService.sendReminders(userIds, parseInt(vm.ReminderId)).then(function (response) {
                                if (response)
                                    toastr.success("Successfully Sent.");
                                vm.ReminderId = "";
                            });
                    }
                });
            }
        }
    }

    init();
}])