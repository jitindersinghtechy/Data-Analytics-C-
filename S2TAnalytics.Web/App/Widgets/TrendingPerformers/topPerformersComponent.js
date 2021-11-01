
appTrendingPerformers.component("topPerformers", {
    templateUrl: serviceBase + "api/Dashboard/TrendingPerformersView",
    bindings: { name: '@' },
    controller: ['$scope', 'trendingPerformersService', '$location', '$timeout', function ($scope, topPerformersService, $location, $timeout) {
        var vm = this;
        //var orderOfDisplay = [{ value: 1, column: 'ROI' }, { value: 2, column: 'DD' }, { value: 3, column: 'WINRate' }, { value: 4, column: 'SharpRatio' }]
        var orderOfDisplay = ['ROI', 'DD', 'WINRate', 'SharpRatio'];
        vm.Performers = [];
        vm.Countries = [];
        vm.TimeLines = [];
        vm.PerformerTimeLines = [];
        vm.pinnedUsersList = [];
        vm.UserGroups = [];
        vm.SelectedUserGroups = [];
        //vm.Country = "";
        vm.showTopFerformers = true;
        vm.showPerformerData = false;
        vm.PerformerData = [];
        vm.sortPerformersBy = "roi";
        vm.preselectedTimelineID = "yDwjeVGRQ8E=";
        vm.loadComplete = false; //Loader
        var pageRecordModel = {
            Filters: {
                TimeLineId: vm.preselectedTimelineID,
                sortPerformersBy: vm.sortPerformersBy
            },
            ListFilter: {},
        };
        vm.UserGroupsSettings = {
            showCheckAll: true,
            showUncheckAll: false,
            smartButtonMaxItems: 1,
            smartButtonTextConverter: function (itemText, originalItem) {
                return itemText;
            }
        }
        vm.onUserGroupsSelect = function (item) {
            //if (item.id == "all") {
            //    vm.SelectedUserGroups = [];
            //    angular.forEach(vm.UserGroups, function (value, index) {
            //        vm.SelectedUserGroups.push({ id: value.id, label: value.label });
            //    });
            //}
            //else {
            //    vm.SelectedUserGroups.filter(function (value, index) {
            //        if (value.id == "all") {
            //            var index = vm.SelectedUserGroups.indexOf(value);
            //            vm.SelectedUserGroups.splice(index, 1);
            //        }
            //    })
            //}
            vm.getTopPerformersByFilters();
        }
        vm.onUserGroupsDeselect = function (item) {
            //if (item.id == "all") {
            //    vm.SelectedUserGroups = [];
            //}
            //else {
            //    vm.SelectedUserGroups.filter(function (value, index) {
            //        if (value.id == "all") {
            //            var index = vm.SelectedUserGroups.indexOf(value);
            //            vm.SelectedUserGroups.splice(index, 1);
            //        }
            //    })
            //}
            vm.getTopPerformersByFilters();
        }

        vm.getClass = function (timeLine) {
            return (vm.TimeLine == timeLine) ? 'active-menu' : '';
        }

        var getTopPerformers = function () {
            vm.loadComplete = false;
            topPerformersService.getTop5Performers(pageRecordModel).then(function (response) {
                var data = response.data.response.MultipleData;
                vm.Performers = data.performers;
                vm.pinnedUsersList = data.pinnedUsers;
                vm.Countries = data.countries;
                vm.Country = '';
                if (data.userGroups.length >= 0) {
                    vm.UserGroups = [];
                    vm.SelectedUserGroups = [];

                    //vm.UserGroups.push({ id: "all", label: "All" });
                    //vm.SelectedUserGroups.push({ id: "all", label: "All" });

                    angular.forEach(data.userGroups, function (value, index) {
                        vm.UserGroups.push({ id: value, label: value });
                        vm.SelectedUserGroups.push({ id: value, label: value });
                    });
                }
                vm.TimeLines = data.timeLines;
                vm.TimeLine = data.timeLines[0];
                vm.PerformerTimeLines = data.timeLines;
                //vm.PerformerTimeLine = data.timeLines[0];
                var getSelectedTimeline = data.timeLines.filter(function (timeline) {
                    if (timeline.stringValue == vm.preselectedTimelineID) {
                        return timeline;
                    }
                })
                vm.PerformerTimeLine = getSelectedTimeline[0];
                $timeout(function () {
                    setupcarousel();
                }, 1);
                vm.loadComplete = true;
            });
        }
        var init = function () {
            getTopPerformers();
        }
        init();

        vm.getTopPerformersByFilters = function () {

            if (vm.Country != undefined) {
                pageRecordModel.Filters.Country = vm.Country;
            }
            else if (pageRecordModel.Filters.hasOwnProperty('Country')) {
                delete pageRecordModel.Filters['Country'];
            }

            if (vm.TimeLine.stringValue != undefined) {
                pageRecordModel.Filters.TimeLineId = vm.TimeLine.stringValue;
            }
            else if (pageRecordModel.Filters.hasOwnProperty('TimeLineId')) {
                delete pageRecordModel.Filters['TimeLineId'];
            }
            if (vm.sortPerformersBy != undefined) {
                pageRecordModel.Filters.sortPerformersBy = vm.sortPerformersBy;
            }
            else if (pageRecordModel.Filters.hasOwnProperty('sortPerformersBy')) {
                delete pageRecordModel.Filters['sortPerformersBy'];
            }
            if (vm.SelectedUserGroups != undefined) {
                var SelectedUserGroups = [];
                angular.forEach(vm.SelectedUserGroups, function (item, countryIndex) {
                    //if (item.id != "all") {
                    SelectedUserGroups.push(item.id);
                    //}
                });
                pageRecordModel.ListFilter = SelectedUserGroups;
            }
            else {
                pageRecordModel.ListFilter = {};
            }
            pageRecordModel.PageNumber = 1;

            vm.loadComplete = false;
            topPerformersService.getTop5Performers(pageRecordModel).then(function (response) {
                changeSortOrder();
                vm.Performers = response.data.response.MultipleData.performers;
                $timeout(function () {
                    setupcarousel();
                }, 1);
                vm.loadComplete = true;
            });
        }
        var changeSortOrder = function () {
            if (vm.sortPerformersBy == "roi") {
                var orderOfDisplay = ['ROI', 'DD', 'WINRate', 'SharpRatio'];
            }
            if (vm.sortPerformersBy == "maxDD") {
                var orderOfDisplay = ['DD', 'ROI', 'WINRate', 'SharpRatio'];
            }
            if (vm.sortPerformersBy == "winRate") {
                var orderOfDisplay = ['WINRate', 'ROI', 'DD', 'SharpRatio'];
            }
            if (vm.sortPerformersBy == "sharpRatio") {
                var orderOfDisplay = ['SharpRatio', 'ROI', 'DD', 'WINRate'];
            }
        }

        vm.getValueAndLabelByIndex = function (index, performer) {

            var value = "";
            var label = "";
            if (vm.sortPerformersBy == "roi") {
                if (index == 0) {
                    value = performer.ROI;
                    label = "ROI%";
                }
                else if (index == 1) {
                    value = performer.DD;
                    label = "Max DD%:";
                }
                else if (index == 2) {
                    value = performer.WINRate;
                    label = "Win%:";
                }
                else if (index == 3) {
                    value = performer.SharpRatio;
                    label = "Sharpe Ratio:";
                }

            }
            else if (vm.sortPerformersBy == "maxDD") {
                if (index == 0) {
                    value = performer.DD;
                    label = "Max DD%";
                }
                else if (index == 1) {
                    value = performer.ROI;
                    label = "ROI%:";
                }
                else if (index == 2) {
                    value = performer.WINRate;
                    label = "Win%:";
                }
                else if (index == 3) {
                    value = performer.SharpRatio;
                    label = "Sharpe Ratio:";
                }
            }
            else if (vm.sortPerformersBy == "winRate") {
                if (index == 0) {
                    value = performer.WINRate;
                    label = "Win%";
                }
                else if (index == 1) {
                    value = performer.ROI;
                    label = "ROI%:";
                }
                else if (index == 2) {
                    value = performer.DD;
                    label = "Max DD%:";
                }
                else if (index == 3) {
                    value = performer.SharpRatio;
                    label = "Sharpe Ratio:";
                }
            }
            else if (vm.sortPerformersBy == "sharpRatio") {
                if (index == 0) {
                    value = performer.SharpRatio;
                    label = "Sharpe Ratio";
                }
                else if (index == 1) {
                    value = performer.ROI;
                    label = "ROI%:";
                }
                else if (index == 2) {
                    value = performer.DD;
                    label = "Max DD%:";
                }
                else if (index == 3) {
                    value = performer.WINRate;
                    label = "Win%:";
                }
            }
            //return value;

            //vm.ValueByIndex = value;
            vm.LabelByIndex = label;
            return value;
        }
        //vm.getLabelByIndex = function (index) {
        //  
        //    return orderOfDisplay[index];

        //    //if (vm.sortPerformersBy == "roi") {
        //    //    return orderOfDisplay[index];
        //    //}
        //    //if (vm.sortPerformersBy == "maxDD") {
        //    //    return orderOfDisplay[index];
        //    //}
        //    //if (vm.sortPerformersBy == "winRate") {
        //    //    return orderOfDisplay[index];
        //    //}
        //    //if (vm.sortPerformersBy == "sharpRatio") {
        //    //    return orderOfDisplay[index];
        //    //}
        //}
        vm.openPerformerDetails = function (performer) {
            vm.showTopFerformers = false;
            vm.showPerformerData = true;
            vm.PerformerData = performer;
            //topPerformersService.getTop5Performers(pageRecordModel).then(function (response) {
            //    vm.Performers = response.data.response.MultipleData.accountDetails;
            //});
        }
        vm.showTopPerformers = function () {
            getTopPerformers();
            vm.showTopFerformers = true;
            vm.showPerformerData = false;
        }
        vm.changePerformersDataByTimelineId = function () {
        }

        vm.getAccountStatsValueByTimeLine = function (column, accountDetail) {
            var accountStat = [];
            angular.forEach(accountDetail.AccountStats, function (value, index) {
                if (value.TimeLineId == vm.TimeLine.stringValue) {
                    accountStat = value;
                }
            });
            return accountStat[column];
        }
        vm.updatePinnedUsers = function (accountDetailId) {
            var selectedAccountDetailsIds = [accountDetailId];
            topPerformersService.updatePinnedUsers(selectedAccountDetailsIds).then(function (response) {
                if (response.data.Success) {
                    toastr.success(response.data.Message);
                    getTopPerformers();
                }
                else {
                    toastr.warning(response.data.Message);
                }
            });
        }

        vm.updateCompareUsers = function (accountDetailId) {  // Storing Data in local storgae...
            var CompareDataFromLocalStorage = [];
            //  CompareDataFromLocalStorage = angular.fromJson(CompareDataFromLocalStorage);
            var accountId = accountDetailId;
            var value = true;
            CompareDataFromLocalStorage.push({ accountId, value });
            localStorage.setItem('CompareData', JSON.stringify(CompareDataFromLocalStorage));
            //toastr.success("Successfully Added to compare");
            localStorage.setItem('previousLocation', 'Performers');
            $location.path("/Compare");
        }

        vm.viewUserDetails = function (accountDetailId) {
            $location.path("/UserDetails/" + accountDetailId);
            localStorage.setItem('previousLocation', 'Performers');
            localStorage.setItem('selectedTimelineId', vm.TimeLine.stringValue)
        }

        var setupcarousel = function (destroy) {
            var owl = $('#topPerformres');
            if (typeof owl.data('owlCarousel') != 'undefined') {
                owl.data('owlCarousel').destroy();
                owl.removeClass('owl-carousel');
            }

            if ($(window).width() > 1025) {

                owl.owlCarousel({
                    stagePadding: 0,
                    loop: false,
                    nav: false,
                    margin: 20,
                    responsive: {
                        1024: { items: 5 },
                        769: { items: 3 },
                        640: { items: 2 },
                        320: { items: 1 }
                    }
                });

            } else if ($(window).width() > 640) {
                owl.owlCarousel({
                    stagePadding: 30,
                    loop: false,
                    nav: false,
                    margin: 20,
                    responsive: {
                        1024: { items: 5 },
                        769: { items: 3 },
                        640: { items: 2 },
                        320: { items: 1 }
                    }
                });
            } else if ($(window).width() > 319) {
                owl.owlCarousel({
                    stagePadding: 25,
                    loop: false,
                    nav: false,
                    margin: 20,
                    responsive: {
                        1024: { items: 5 },
                        769: { items: 3 },
                        640: { items: 2 },
                        320: { items: 1 }
                    }
                });
            }
        };

        vm.getIfUserPinned = function (accountDetailId) {
            if (vm.pinnedUsersList != undefined && vm.pinnedUsersList != null) {

                if (vm.pinnedUsersList.AccountIds.indexOf(accountDetailId) !== -1) {
                    vm.isUserPinned = true;
                }
                else {
                    vm.isUserPinned = false;
                }
            }
            else {
                vm.isUserPinned = false;
            }
        }

        vm.unpinUser = function (accountDetailId) {
            topPerformersService.unpinUser(accountDetailId).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    toastr.success(response.data.Message);
                    getTopPerformers();
                }
                else {
                    toastr.warning(response.data.Message);
                }
            });
        }
        vm.changeSortOrder = function (sortBy) {

        }

        vm.updateExcludeUsers = function (accountId) {
            var selectedAccountDetailsIds = [accountId]
            BootstrapDialog.confirm({
                title: 'DELETE PERFORMER(S)',
                message: 'Are you sure you want to delete selected performer?',
                callback: function (result) {
                    if (result) {
                        topPerformersService.updateExcludeUsers(selectedAccountDetailsIds).then(function (response) {
                            if (response.data.Success) {
                                toastr.success(response.data.Message);
                                getTopPerformers();
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