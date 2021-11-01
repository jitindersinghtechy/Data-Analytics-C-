app.component("topPerformersAndPinnedUsers", {
    templateUrl: serviceBase + "App/Views/Components/TopPerformersAndPinnedUsers.html",
    bindings: { name: '@' },
    controller: ['$scope', 'topPerformersAndPinnedUsersService', '$location', '$timeout', 'preselectedTimelineID',
        function ($scope, topPerformersAndPinnedUsersService, $location, $timeout, preselectedTimelineID) {
            var vm = this;
            var orderOfDisplay = ['ROI', 'DD', 'WINRate', 'SharpRatio'];
            var orderOfDisplayPinnedUser = ['ROI', 'DD', 'WINRate', 'SharpRatio'];
            vm.Performers = [];
            vm.Countries = [];
            vm.TimeLines = [];
            vm.PerformerTimeLines = [];
            vm.UserGroups = [];
            vm.SelectedUserGroups = [];
            vm.UserGroupsPinnedUser = [];
            vm.SelectedUserGroupsPinnedUser = [];
            vm.pinnedUsersList = [];
            vm.sortPerformersBy = "roi";
            vm.sortPinnedPerformersBy = "roi";
            vm.showTopFerformers = true;
            vm.showPerformerData = false;
            vm.PerformerData = [];
            vm.preselectedTimelineID = preselectedTimelineID;
            vm.loadComplete = false; //Loader
            vm.loadCompletePinnedUser = false; //Loader

            var permissions = $scope.$parent.mainVM;
            vm.isPinnedUsers = $scope.$parent.mainVM.isPinnedUsers;
            vm.isExcludeUser = $scope.$parent.mainVM.isExcludeUser;
            vm.isViewDetail = $scope.$parent.mainVM.isViewDetail;
            vm.isCompare = $scope.$parent.mainVM.isCompare;
            vm.isExportData = $scope.$parent.mainVM.isExportData;
            vm.isEmbedWidget = $scope.$parent.mainVM.isEmbedWidget;
            vm.isApiWidget = $scope.$parent.mainVM.isApiWidget;
            vm.isAddReport = $scope.$parent.mainVM.isAddReport;


            var pageRecordModel = {
                Filters: {
                    TimeLineId: vm.preselectedTimelineID,
                    sortPerformersBy: vm.sortPerformersBy
                },
                MultipleListFilter: {},
                ListFilter: {},
            };
            vm.pinnedUserCountries = [];
            vm.pinnedUserTimeLines = [];
            vm.NoPinnedUserFound = true;
            var pinnedUserPageRecordModel = {
                Filters: {
                    TimeLineId: vm.preselectedTimelineID,
                    sortPerformersBy: vm.sortPinnedPerformersBy
                },
                MultipleListFilter: {},
                ListFilter: {},
            };
            vm.showTopPerformers = true;
            vm.showPinnedUsers = false;
            vm.switchView = function (showTopPerformers, showPinnedUsers) {
                //$timeout(function () {
                vm.showPinnedUsers = showPinnedUsers;
                vm.showTopPerformers = showTopPerformers;
                //}, 100);
                if (showTopPerformers == true) {
                    getTopPerformers();
                }
                if (showPinnedUsers == true) {

                    getTopFivePinnedUsers();
                }
            }
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
            vm.UserGroupsPinnedUserSettings = {
                showCheckAll: true,
                showUncheckAll: false,
                smartButtonMaxItems: 1,
                smartButtonTextConverter: function (itemText, originalItem) {
                    return itemText;
                }
            }
            vm.onUserGroupsSelectPinnedUser = function (item) {
                //if (item.id == "all") {
                //    vm.SelectedUserGroupsPinnedUser = [];
                //    angular.forEach(vm.UserGroupsPinnedUser, function (value, index) {
                //        vm.SelectedUserGroupsPinnedUser.push({ id: value.id, label: value.label });
                //    });
                //}
                //else {
                //    vm.SelectedUserGroupsPinnedUser.filter(function (value,index) {
                //        if (value.id == "all") {
                //            var index = vm.SelectedUserGroupsPinnedUser.indexOf(value);
                //            vm.SelectedUserGroupsPinnedUser.splice(index, 1);
                //        }
                //    })
                //}
                vm.getTopPinnedUsersByFilters();
            }
            vm.onUserGroupsDeselectPinnedUser = function (item) {
                //if (item.id == "all") {
                //    vm.SelectedUserGroupsPinnedUser = [];
                //}
                //else {
                //    vm.SelectedUserGroupsPinnedUser.filter(function (value,index) {
                //        if (value.id == "all") {
                //            var index = vm.SelectedUserGroupsPinnedUser.indexOf(value);
                //            vm.SelectedUserGroupsPinnedUser.splice(index, 1);
                //        }
                //    })
                //}
                vm.getTopPinnedUsersByFilters();
            }
            var getTopPerformers = function () {
                vm.loadComplete = false; //Loader
                topPerformersAndPinnedUsersService.getTop5Performers(pageRecordModel).then(function (response) {
                    var data = response.data.response.MultipleData;
                    vm.WidgetId = data.WidgetId;
                    vm.Performers = data.performers;
                    vm.pinnedUsersList = data.pinnedUsers;
                    vm.Countries = data.countries;
                    vm.RORForTimeline = data.riskFreeRate;
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
                    var getSelectedTimeline = data.timeLines.filter(function (timeline) {
                        if (timeline.stringValue == vm.preselectedTimelineID) {
                            return timeline;
                        }
                    })
                    vm.TimeLine = getSelectedTimeline[0];
                    //vm.TimeLine = data.timeLines[0];
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
                    vm.loadComplete = true; //Loader
                });
            }
            var getTopFivePinnedUsers = function () {
                vm.loadCompletePinnedUser = false; //Loader
                topPerformersAndPinnedUsersService.getTopFivePinnedUsers(pinnedUserPageRecordModel).then(function (response) {
                    if (response.data.Success) {
                        var data = response.data.MultipleData;
                        vm.PinnedUsers = data.performers;
                        vm.pinnedUserCountries = data.countries;
                        vm.pinnedUserCountry = '';
                        vm.pinnedUserRORForTimeline = data.riskFreeRate;
                        vm.pinnedUserTimeLines = data.timeLines;
                        //vm.pinnedUserTimeLine = data.timeLines[0];
                        var getSelectedTimeline = data.timeLines.filter(function (timeline) {
                            if (timeline.stringValue == vm.preselectedTimelineID) {
                                return timeline;
                            }
                        })
                        vm.pinnedUserTimeLine = getSelectedTimeline[0];
                        vm.NoPinnedUserFound = false;
                        if (data.userGroups.length >= 0) {
                            vm.UserGroupsPinnedUser = [];
                            vm.SelectedUserGroupsPinnedUser = [];
                            //vm.UserGroupsPinnedUser.push({ id: "all", label: "All" });
                            //vm.SelectedUserGroupsPinnedUser.push({ id: "all", label: "All" });
                            angular.forEach(data.userGroups, function (value, index) {
                                vm.UserGroupsPinnedUser.push({ id: value, label: value });
                                vm.SelectedUserGroupsPinnedUser.push({ id: value, label: value });
                            });
                        }
                        $timeout(function () {
                            setupPinnedUsersCarousel();
                        }, 1);
                    }
                    else {
                        vm.NoPinnedUserFound = true;
                    }
                    vm.loadCompletePinnedUser = true; //Loader
                });
            }
            var init = function () {
                getTopPerformers();
                getTopFivePinnedUsers();
            }
            init();
            vm.getTopPerformersByFilters = function (SelectedUserGroups) {
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

                vm.loadComplete = false; //Loader
                topPerformersAndPinnedUsersService.getTop5Performers(pageRecordModel).then(function (response) {
                    changeSortOrder();
                    vm.Performers = response.data.response.MultipleData.performers;
                    vm.RORForTimeline = response.data.response.MultipleData.riskFreeRate;
                    $timeout(function () {
                        setupcarousel();
                    }, 1);
                    vm.loadComplete = true; //Loader
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
                        value = performer.SharpRatio == null ? 0 : performer.SharpRatio;
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
                        value = performer.SharpRatio == null ? 0 : performer.SharpRatio;
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
                        value = performer.SharpRatio == null ? 0 : performer.SharpRatio;
                        label = "Sharpe Ratio:";
                    }
                }
                else if (vm.sortPerformersBy == "sharpRatio") {
                    if (index == 0) {
                        value = performer.SharpRatio == null ? 0 : performer.SharpRatio;
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
                //vm.ValueByIndex = value;
                vm.LabelByIndex = label;
                return value;
            }
            vm.getTopPinnedUsersByFilters = function (SelectedUserGroups) {

                if (vm.pinnedUserCountry != undefined) {
                    pinnedUserPageRecordModel.Filters.Country = vm.pinnedUserCountry;
                }
                else if (pinnedUserPageRecordModel.Filters.hasOwnProperty('Country')) {
                    delete pinnedUserPageRecordModel.Filters['Country'];
                }
                if (vm.pinnedUserTimeLine.stringValue != undefined) {
                    pinnedUserPageRecordModel.Filters.TimeLineId = vm.pinnedUserTimeLine.stringValue;
                }
                else if (pinnedUserPageRecordModel.Filters.hasOwnProperty('TimeLineId')) {
                    delete pinnedUserPageRecordModel.Filters['TimeLineId'];
                }
                if (vm.sortPinnedPerformersBy != undefined) {
                    pinnedUserPageRecordModel.Filters.sortPerformersBy = vm.sortPinnedPerformersBy;
                }
                else if (pinnedUserPageRecordModel.Filters.hasOwnProperty('sortPerformersBy')) {
                    delete pinnedUserPageRecordModel.Filters['sortPerformersBy'];
                }
                if (vm.SelectedUserGroupsPinnedUser != undefined) {
                    var SelectedUserGroups = [];
                    angular.forEach(vm.SelectedUserGroupsPinnedUser, function (item, countryIndex) {
                        //if (item.id != "all") {
                        SelectedUserGroups.push(item.id);
                        //}
                    });
                    pinnedUserPageRecordModel.ListFilter = SelectedUserGroups;
                }
                else {
                    pinnedUserPageRecordModel.ListFilter = {};
                }
                //if (vm.SelectedUserGroupsPinnedUser != undefined) {
                //    var SelectedUserGroups = [];
                //    angular.forEach(vm.SelectedUserGroupsPinnedUser, function (item, countryIndex) {
                //        SelectedUserGroups.push(item);
                //    });
                //    pageRecordModel.MultipleListFilter.UserGroups = SelectedUserGroups;
                //}
                //else {
                //    delete pageRecordModel.MultipleListFilter['UserGroups'];
                //}

                vm.loadCompletePinnedUser = false; //Loader
                topPerformersAndPinnedUsersService.getTopFivePinnedUsers(pinnedUserPageRecordModel).then(function (response) {
                    changeSortOrderPinnedUser();
                    if (response.data.MultipleData != null) {
                        vm.PinnedUsers = response.data.MultipleData.performers;
                        vm.pinnedUserRORForTimeline = response.data.MultipleData.riskFreeRate;
                    }
                    else {
                        vm.NoPinnedUserFound = true;
                    }
                    $timeout(function () {
                        setupPinnedUsersCarousel();
                    }, 1);
                    vm.loadCompletePinnedUser = true; //Loader
                });
            }
            var changeSortOrderPinnedUser = function () {
                if (vm.sortPinnedPerformersBy == "roi") {
                    var orderOfDisplayPinnedUser = ['ROI', 'DD', 'WINRate', 'SharpRatio'];
                }
                if (vm.sortPinnedPerformersBy == "maxDD") {
                    var orderOfDisplayPinnedUser = ['DD', 'ROI', 'WINRate', 'SharpRatio'];
                }
                if (vm.sortPinnedPerformersBy == "winRate") {
                    var orderOfDisplayPinnedUser = ['WINRate', 'ROI', 'DD', 'SharpRatio'];
                }
                if (vm.sortPinnedPerformersBy == "sharpRatio") {
                    var orderOfDisplayPinnedUser = ['SharpRatio', 'ROI', 'DD', 'WINRate'];
                }
            }
            vm.getValueAndLabelByIndexPinnedUser = function (index, performer) {
                var value = "";
                var label = "";
                if (vm.sortPinnedPerformersBy == "roi") {
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
                        value = performer.SharpRatio == null ? 0 : performer.SharpRatio;
                        label = "Sharpe Ratio:";
                    }
                }
                else if (vm.sortPinnedPerformersBy == "maxDD") {
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
                        value = performer.SharpRatio == null ? 0 : performer.SharpRatio;
                        label = "Sharpe Ratio:";
                    }
                }
                else if (vm.sortPinnedPerformersBy == "winRate") {
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
                        value = performer.SharpRatio == null ? 0 : performer.SharpRatio;
                        label = "Sharpe Ratio:";
                    }
                }
                else if (vm.sortPinnedPerformersBy == "sharpRatio") {
                    if (index == 0) {
                        value = performer.SharpRatio == null ? 0 : performer.SharpRatio;
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

                //vm.ValueByIndexPinnedUser = value;
                vm.LabelByIndexPinnedUser = label;
                return value;
            }
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
                topPerformersAndPinnedUsersService.updatePinnedUsers(selectedAccountDetailsIds).then(function (response) {
                    if (response.data.Success) {
                        toastr.success(response.data.Message);
                    }
                    else {
                        toastr.warning(response.data.Message);
                    }
                    getTopPerformers();
                    getTopFivePinnedUsers();
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
                if (vm.showPinnedUsers == true) {
                    localStorage.setItem('selectedTimelineId', vm.pinnedUserTimeLine.stringValue);
                }
                else {
                    localStorage.setItem('selectedTimelineId', vm.TimeLine.stringValue)
                }

                //toastr.success("Successfully Added to compare");
                localStorage.setItem('previousLocation', 'Dashboard');
                $location.path("/Compare");
            }
            vm.unpinUser = function (pinnedUser) {
                topPerformersAndPinnedUsersService.unpinUser(pinnedUser.AccountDetailId).then(function (response) {
                    var data = response.data;
                    if (data.Success) {
                        toastr.success(response.data.Message);
                        var index = vm.PinnedUsers.indexOf(pinnedUser);
                        vm.PinnedUsers.splice(index, 1);
                        vm.NoPinnedUserFound = vm.PinnedUsers.length == 0 ? true : false;
                    }
                    else {
                        toastr.warning(response.data.Message);
                    }
                    getTopFivePinnedUsers();
                });
            }
            var setupcarousel = function () {
                var owl = $('#topPerformres');
                if (typeof owl.data('owlCarousel') != 'undefined') {
                    owl.data('owlCarousel').destroy();
                    owl.removeClass('owl-carousel');
                }
                owl.owlCarousel({
                    stagePadding: 0,
                    loop: false,
                    nav: false,
                    mouseDrag: false,
                    touchDrag: false,
                    margin: 20,
                    responsive: {
                        1026: { items: 5 },
                        769: {
                            items: 3, nav: true,
                            navText: [
                               "<i class='fa fa-caret-left'></i>",
                               "<i class='fa fa-caret-right'></i>"
                            ]
                        },
                        640: {
                            items: 2, stagePadding: 30, nav: true,
                            navText: [
                                "<i class='fa fa-caret-left'></i>",
                                "<i class='fa fa-caret-right'></i>"
                            ]
                        },
                        320: {
                            items: 1, stagePadding: 30, nav: true,
                            navText: [
                                "<i class='fa fa-caret-left'></i>",
                                "<i class='fa fa-caret-right'></i>"
                            ]
                        }
                    }
                });
                //    if ($(window).width() > 1025) {

                //        owl.owlCarousel({
                //            stagePadding: 0,
                //            loop: false,
                //            nav: false,
                //            margin: 20,
                //            mouseDrag: false,
                //            touchDrag: false,
                //            responsive: {
                //                1024: { items: 5 },
                //                769: { items: 3 },
                //                640: { items: 2 },
                //                320: { items: 1 }
                //            }
                //        });

                //    } else if ($(window).width() > 640) {
                //        owl.owlCarousel({
                //            stagePadding: 30,
                //            loop: false,
                //            nav: false,
                //            margin: 20,
                //            responsive: {
                //                1024: { items: 5 },
                //                769: { items: 3 },
                //                640: { items: 2 },
                //                320: { items: 1 }
                //            }
                //        });
                //    } else if ($(window).width() > 319) {
                //        owl.owlCarousel({
                //            stagePadding: 25,
                //            loop: false,
                //            nav: false,
                //            margin: 20,
                //            responsive: {
                //                1024: { items: 5 },
                //                769: { items: 3 },
                //                640: { items: 2 },
                //                320: { items: 1 }
                //            }
                //        });
                //    }
            };
            var setupPinnedUsersCarousel = function () {
                var owl = $('#pinnedUsers');
                if (typeof owl.data('owlCarousel') != 'undefined') {
                    owl.data('owlCarousel').destroy();
                    owl.removeClass('owl-carousel');
                }
                owl.owlCarousel({
                    stagePadding: 0,
                    loop: false,
                    nav: false,
                    mouseDrag: false,
                    touchDrag: false,
                    margin: 20,
                    responsive: {
                        1026: { items: 5 },
                        769: {
                            items: 3, nav: true,
                            navText: [
                               "<i class='fa fa-caret-left'></i>",
                               "<i class='fa fa-caret-right'></i>"
                            ]
                        },
                        640: {
                            items: 2, stagePadding: 30, nav: true,
                            navText: [
                                "<i class='fa fa-caret-left'></i>",
                                "<i class='fa fa-caret-right'></i>"
                            ]
                        },
                        320: {
                            items: 1, stagePadding: 30, nav: true,
                            navText: [
                                "<i class='fa fa-caret-left'></i>",
                                "<i class='fa fa-caret-right'></i>"
                            ]
                        }
                    }
                });
                //if ($(window).width() > 1025) {

                //    owl.owlCarousel({
                //        stagePadding: 0,
                //        loop: false,
                //        nav: false,
                //        margin: 20,
                //        mouseDrag: false,
                //        touchDrag: false,
                //        responsive: {
                //            1024: { items: 5 },
                //            769: { items: 3 },
                //            640: { items: 2 },
                //            320: { items: 1 }
                //        }
                //    });

                //} else if ($(window).width() > 640) {
                //    owl.owlCarousel({
                //        stagePadding: 30,
                //        loop: false,
                //        nav: false,
                //        margin: 20,
                //        responsive: {
                //            1024: { items: 5 },
                //            769: { items: 3 },
                //            640: { items: 2 },
                //            320: { items: 1 }
                //        }
                //    });
                //} else if ($(window).width() > 319) {
                //    owl.owlCarousel({
                //        stagePadding: 25,
                //        loop: false,
                //        nav: false,
                //        margin: 20,
                //        responsive: {
                //            1024: { items: 5 },
                //            769: { items: 3 },
                //            640: { items: 2 },
                //            320: { items: 1 }
                //        }
                //    });
            }

            vm.viewUserDetails = function (accountDetailId) {
                $location.path("/UserDetails/" + accountDetailId);
                localStorage.setItem('previousLocation', 'Dashboard');

                if (vm.showPinnedUsers == true) {
                    localStorage.setItem('selectedTimelineId', vm.pinnedUserTimeLine.stringValue);
                }
                else {
                    localStorage.setItem('selectedTimelineId', vm.TimeLine.stringValue)
                }

            }
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
                topPerformersAndPinnedUsersService.unpinUser(accountDetailId).then(function (response) {
                    var data = response.data;
                    if (data.Success) {
                        toastr.success(response.data.Message);
                        getTopFivePinnedUsers();
                        getTopPerformers();
                        //vm.userPinned = false;
                    }
                    else {
                        toastr.warning(response.data.Message);
                    }
                });
            }
            vm.changeSortOrder = function (sortBy) {

            }
            vm.changepinnedSortOrder = function (sortBy) {

            }
            vm.updateExcludeUsers = function (accountId) {
                var selectedAccountDetailsIds = [accountId]
                BootstrapDialog.confirm({
                    cssClass: 'black',
                    title: 'DELETE PERFORMER(S)',
                    message: 'Are you sure you want to delete selected performer?',
                    callback: function (result) {
                        if (result) {
                            topPerformersAndPinnedUsersService.updateExcludeUsers(selectedAccountDetailsIds).then(function (response) {
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
            vm.updateExcludePinnedUsers = function (accountId) {
                var selectedAccountDetailsIds = [accountId]
                BootstrapDialog.confirm({
                    title: 'DELETE PERFORMER(S)',
                    message: 'Are you sure you want to delete selected performer?',
                    callback: function (result) {
                        if (result) {
                            topPerformersAndPinnedUsersService.updateExcludeUsers(selectedAccountDetailsIds).then(function (response) {
                                if (response.data.Success) {
                                    toastr.success(response.data.Message);
                                    getTopFivePinnedUsers();
                                }
                                else {
                                    toastr.warning(response.data.Message);
                                }
                            });
                        }
                    }
                });
            }

            vm.ShowScript = function () {
                topPerformersAndPinnedUsersService.getWidgetPermission(vm.WidgetId).then(function (response) {
                    if (response.data.Success) {
                        vm.tags = response.data.Data != null ? response.data.Data.Domain : null;
                        $('#divTopPerformersAndPinnedUsersScript').text('<script type="text/javascript" id="scriptTopPerformersAndPinned" token="' + localStorage.getItem('accessToken') + '" src="http://analyticsv1.azurewebsites.net/App/Widgets/TopPerformersAndPinnedUsers/render.js"></script><top-performers-and-pinned-users app="topPerformersAndPinnedApp"></top-performers-and-pinned-users>');
                        $('#txtTopPerformersAndPinnedUsersScript').val('<script type="text/javascript" id="scriptTopPerformersAndPinned" token="' + localStorage.getItem('accessToken') + '" src="http://analyticsv1.azurewebsites.net/App/Widgets/TopPerformersAndPinnedUsers/render.js"></script><top-performers-and-pinned-users app="topPerformersAndPinnedApp"></top-performers-and-pinned-users>');
                        $("#embedScriptTPAPUPopup").modal('show');
                    }
                });
            }


            vm.EmbedAPI = function () {
                $('#apiLinkText').text(location.host + "/api/Performers/GetTop5Performers");
                $('#EmbedAPIToken').text("Bearer " + localStorage.getItem('accessToken'));
                $('#ExportAPIParams').text(JSON.stringify(pageRecordModel));
                $("#embedAPIPopup").modal('show');
            }
            vm.EmbedAPIPinnedUsers = function () {
                $('#apiLinkText').text(location.host + "/api/Performers/GetTopFivePinnedUsers");
                $('#EmbedAPIToken').text("Bearer " + localStorage.getItem('accessToken'));
                $('#ExportAPIParams').text(JSON.stringify(pinnedUserPageRecordModel));
                $("#embedAPIPopup").modal('show');
            }


            function copyToClipboard(elem) {
                // create hidden text element, if it doesn't already exist
                var targetId = "_hiddenCopyText_";
                var isInput = elem.tagName === "INPUT" || elem.tagName === "TEXTAREA";
                var origSelectionStart, origSelectionEnd;
                if (isInput) {
                    // can just use the original source element for the selection and copy
                    target = elem;
                    origSelectionStart = elem.selectionStart;
                    origSelectionEnd = elem.selectionEnd;
                } else {
                    // must use a temporary form element for the selection and copy
                    target = document.getElementById(targetId);
                    if (!target) {
                        var target = document.createElement("textarea");
                        target.style.position = "absolute";
                        target.style.left = "-9999px";
                        target.style.top = "0";
                        target.id = targetId;
                        document.body.appendChild(target);
                    }
                    target.textContent = elem.textContent;
                }
                // select the content
                var currentFocus = document.activeElement;
                target.focus();
                target.setSelectionRange(0, target.value.length);

                // copy the selection
                var succeed;
                try {
                    succeed = document.execCommand("copy");
                } catch (e) {
                    succeed = false;
                }
                // restore original focus
                if (currentFocus && typeof currentFocus.focus === "function") {
                    currentFocus.focus();
                }

                if (isInput) {
                    // restore prior selection
                    elem.setSelectionRange(origSelectionStart, origSelectionEnd);
                } else {
                    // clear temporary content
                    target.textContent = "";
                }
                return succeed;
            }

            vm.CopyToClipboard = function (elementId) {
                $('#txtTopPerformersAndPinnedUsersScript').css("display", "block");
                copyToClipboard($("#txtTopPerformersAndPinnedUsersScript")[0]);
                $('#txtTopPerformersAndPinnedUsersScript').css("display", "none");
                toastr.success("Copied");
            }

            function validateDomain(tag) {
                var re = /^[a-zA-Z0-9][a-zA-Z0-9-]{1,61}[a-zA-Z0-9](?:\.[a-zA-Z]{2,})+$/;
                return re.test(tag.text);
            }

            vm.checkDomain = function (tag) {
                if (!validateDomain(tag)) {
                    toastr.warning("Domain not in correct format");
                    return false;
                }
                else {
                    return true;
                }
            }

            vm.SaveEmbedWidgetPermission = function () {
                vm.Domain = [];
                angular.forEach(vm.tags, function (tag, index) {
                    vm.Domain.push(tag.text);
                })
                if (vm.Domain.length > 0) {
                    topPerformersAndPinnedUsersService.SaveEmbedWidgetPermission(vm.WidgetId, vm.Domain).then(function (response) {
                        if (response.data) {
                            toastr.success("Applied");
                        }
                    });
                }
            }

        }],
    controllerAs: 'vm'
});