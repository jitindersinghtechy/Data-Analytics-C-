
appPerformersAndPinned.component("topPerformersAndPinnedUsers", {
    templateUrl: serviceBase + "api/Dashboard/PerformersAndPinnedUsersView",
    bindings: { name: '@' },
    controller: ['$scope', 'topPerformersAndPinnedUsersService', '$location', '$timeout', function ($scope, topPerformersAndPinnedUsersService, $location, $timeout) {
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
        vm.preselectedTimelineID = "yDwjeVGRQ8E=";
        vm.loadComplete = false; //Loader
        vm.loadCompletePinnedUser = false; //Loader
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
                vm.loadComplete = true; //Loader
            });
        }
        var getTopFivePinnedUsers = function () {
            //vm.loadCompletePinnedUser = false; //Loader
            topPerformersAndPinnedUsersService.getTopFivePinnedUsers(pinnedUserPageRecordModel).then(function (response) {
                if (response.data.Success) {
                    var data = response.data.MultipleData;
                    vm.PinnedUsers = data.performers;
                    vm.pinnedUserCountries = data.countries;
                    vm.pinnedUserCountry = '';
                    
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
                    value = performer.SharpRatio;
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
                    value = performer.SharpRatio;
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
                    value = performer.SharpRatio;
                    label = "Sharpe Ratio:";
                }
            }
            else if (vm.sortPinnedPerformersBy == "sharpRatio") {
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

            //owl.owlCarousel({
            //    loop: true,
            //    autoPlay: 2500,
            //    items: 5,
            //    nav: true,
            //    responsive: {
            //        1024: { items: 5 },
            //        801: { items: 4 },
            //        667: { items: 3 },
            //        568: { items: 2 },
            //        320: { items: 1 }
            //    }
            //});
        };
        var setupPinnedUsersCarousel = function () {
            var owl = $('#pinnedUsers');
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

            //owl.owlCarousel({
            //    loop: true,
            //    autoPlay: 2500,
            //    items: 5,
            //    nav: true,
            //    responsive: {
            //        1024: { items: 5 },
            //        801: { items: 4 },
            //        667: { items: 3 },
            //        568: { items: 2 },
            //        320: { items: 1 }
            //    }
            //});
        };
        vm.viewUserDetails = function (accountDetailId) {
            $location.path("/UserDetails/" + accountDetailId);
            localStorage.setItem('previousLocation', 'Dashboard');
            
            if (vm.showPinnedUsers == true) {
                localStorage.setItem('selectedTimelineId', vm.pinnedUserTimeLine.stringValue);
            }
            else{
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
                //cssClass: 'black',
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
    }],
    controllerAs: 'vm'
});