app.component("pinnedUsers", {
    templateUrl: serviceBase + "App/Views/Components/PinnedUsers.html",
    bindings: { name: '@' },
    controller: ['$scope', 'pinnedUsersService', '$location', 'preselectedTimelineID', function ($scope, pinnedUsersService, $location, preselectedTimelineID) {
        var vm = this;
        vm.Countries = [];
        vm.Country = "";
        vm.TimelineId = "yDwjeVGRQ8E=";
        vm.loadComplete = false; //Loader
        vm.NoPinnedUserFoundAfterFilter = false;


        var permissions = $scope.$parent.mainVM;
        vm.isPinnedUsers = $scope.$parent.mainVM.isPinnedUsers;
        vm.isExcludeUser = $scope.$parent.mainVM.isExcludeUser;
        vm.isViewDetail = $scope.$parent.mainVM.isViewDetail;
        vm.isCompare = $scope.$parent.mainVM.isCompare;
        vm.isExportData = $scope.$parent.mainVM.isExportData;
        vm.isEmbedWidget = $scope.$parent.mainVM.isEmbedWidget;
        vm.isAddReport = $scope.$parent.mainVM.isAddReport;
        //vm.NoPinnedUserFound = true;
        var pageRecordModel = {
            Filters: {
                TimeLineId: preselectedTimelineID
            },
        };
        var getPinnedUsers = function () {
            vm.loadComplete = false;
            pinnedUsersService.getPinnedUsers(pageRecordModel).then(function (response) {
                if (response.data.Success) {
                    var data = response.data.MultipleData;
                    vm.PinnedUsers = data.performers;
                    vm.Countries = data.countries;
                    vm.NoPinnedUserFound = false;
                }
                else {
                    vm.NoPinnedUserFound = true;
                    //toastr.warning(response.data.Message);
                }
                vm.loadComplete = true;
            });
        }
        var init = function () {
            getPinnedUsers();
        }
        init();
        vm.getAccountStatsValueByTimeLine = function (column, accountDetail) {
            var accountStat = [];
            angular.forEach(accountDetail.AccountStats, function (value, index) {
                if (value.TimeLineId == vm.TimelineId) {
                    accountStat = value;
                }
            });
            return accountStat[column];
        }
        vm.getPinnedUsersByFilters = function () {
            if (vm.Country != undefined) {
                pageRecordModel.Filters.Country = vm.Country;
            }
            else if (pageRecordModel.Filters.hasOwnProperty('Country')) {
                delete pageRecordModel.Filters['Country'];
            }

            vm.loadComplete = false;
            pinnedUsersService.getPinnedUsers(pageRecordModel).then(function (response) {
                if (response.data.MultipleData != null) {
                    vm.PinnedUsers = response.data.MultipleData.performers;
                    vm.NoPinnedUserFound = false;
                } else {
                    vm.NoPinnedUserFound = true;
                }
                vm.loadComplete = true;
            });
        }

        vm.updateCompareUsers = function (accountDetailId) {  // Storing Data in local storgae...
            var selectedAccountDetailsIds = [accountDetailId];
            var CompareDataFromLocalStorage = [];
            //  CompareDataFromLocalStorage = angular.fromJson(CompareDataFromLocalStorage);
            var accountId = accountDetailId;
            var value = true;
            CompareDataFromLocalStorage.push({ accountId, value });
            localStorage.setItem('CompareData', JSON.stringify(CompareDataFromLocalStorage));
            localStorage.removeItem('selectedTimelineId');
            toastr.success("Successfully Added to compare");
            localStorage.setItem('previousLocation', 'PinnedUsers');
            $location.path("/Compare");
        }
        vm.unpinUser = function (pinnedUser) {
            pinnedUsersService.unpinUser(pinnedUser.AccountDetailId).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    toastr.success(response.data.Message);
                    var index = vm.PinnedUsers.indexOf(pinnedUser);
                    vm.PinnedUsers.splice(index, 1);
                    if (vm.PinnedUsers.length == 0)
                        vm.NoPinnedUserFound = true;
                }
                else {
                    toastr.warning(response.data.Message);
                }
            });
        }

        vm.viewUserDetails = function (accountDetailId) {
            $location.path("/UserDetails/" + accountDetailId);
            localStorage.setItem('previousLocation', 'PinnedUsers');
            //localStorage.setItem('selectedTimelineId', null)
            localStorage.removeItem('selectedTimelineId');
        }

        vm.updateExcludeUsers = function (accountId) {
            var selectedAccountDetailsIds = [accountId]
            BootstrapDialog.confirm({
                title: 'DELETE PERFORMER(S)',
                message: 'Are you sure you want to delete selected performer?',
                callback: function (result) {
                    if (result) {
                        pinnedUsersService.updateExcludeUsers(selectedAccountDetailsIds).then(function (response) {
                            if (response.data.Success) {
                                toastr.success(response.data.Message);
                                getPinnedUsers();
                            }
                            else {
                                toastr.warning(response.data.Message);
                            }
                        });
                    }
                }
            });

        }
    }],
controllerAs: 'vm'
});