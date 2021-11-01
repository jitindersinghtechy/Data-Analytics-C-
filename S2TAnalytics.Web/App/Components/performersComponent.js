app.component("performerList", {
    templateUrl: serviceBase + "App/Views/Components/Performers.html",
    bindings: { name: '@' },
    controller: ['$scope', 'performerService', '$location', 'preselectedTimelineID', function ($scope, performerService, $location, preselectedTimelineID) {
        var vm = this;
        vm.Performers = [];
        vm.Countries = [];
        vm.SelectedCountries = [];
        vm.UserGroups = [];
        vm.SelectedUserGroups = [];
        vm.TimeLines = [];
        vm.TotalRecords = 0;
        vm.StartingRecordNumber = 1;
        vm.ToRecordNumber = 0;
        vm.PageSizes = [10, 50, 100, 500];
        vm.selectedPageSize = vm.PageSizes[0];
        //vm.TimeLine = "";
        vm.preselectedTimelineID = preselectedTimelineID;
        vm.loadComplete = false; //Loader

        var permissions = $scope.$parent.mainVM;
        vm.isPinnedUsers = $scope.$parent.mainVM.isPinnedUsers;
        vm.isExcludeUser = $scope.$parent.mainVM.isExcludeUser;
        vm.isViewDetail = $scope.$parent.mainVM.isViewDetail;
        vm.isCompare = $scope.$parent.mainVM.isCompare;
        vm.isExportData = $scope.$parent.mainVM.isExportData;
        vm.isEmbedWidget = $scope.$parent.mainVM.isEmbedWidget;
        vm.isAddReport = $scope.$parent.mainVM.isAddReport;

        vm.selectedAccounts = {};
        var pageRecordModel = {
            PageNumber: 1,
            PageSize: 10,
            Filters: {
                TimeLineId: vm.preselectedTimelineID
            },
            MultipleListFilter: {},
            SortOrder: "asc",
            SortBy: "",
        };
        //vm.winSlider = {
        //    value: 0,
        //    options: {
        //        floor: 0,
        //        ceil: 1000,
        //        showSelectionBar: true,
        //        hideLimitLabels: true,
        //        getSelectionBarColor: function () {
        //            return '#99b841';
        //        },
        //        onEnd: function () {
        //            vm.changeGridByFilters();
        //        },
        //    }
        //};
        //vm.roiSlider = {
        //    value: 0,
        //    options: {
        //        floor: 0,
        //        ceil: 1000,
        //        showSelectionBar: true,
        //        hideLimitLabels: true,
        //        getSelectionBarColor: function () {
        //            return '#99b841';
        //        },
        //        onEnd: function () {
        //            vm.changeGridByFilters();
        //        },
        //    }
        //};
        //vm.maxDDSlider = {
        //    value: 0,
        //    options: {
        //        floor: 0,
        //        ceil: 1000,
        //        showSelectionBar: true,
        //        hideLimitLabels: true,
        //        getSelectionBarColor: function () {
        //            return '#99b841';
        //        },
        //        onEnd: function () {
        //            vm.changeGridByFilters();
        //        },
        //    }
        //};
        vm.winSlider = {
            minValue: 0,
            maxValue: 100,
            options: {
                floor: 0,
                ceil: 100,
                step: 1,
                getSelectionBarColor: function () {
                    return '#99b841';
                },
                onEnd: function () {
                    vm.changeGridByFilters();
                },
                noSwitching: true
            }

        };
        vm.roiSlider = {
            minValue: 0,
            maxValue: 100,
            options: {
                floor: 0,
                ceil: 100,
                step: 1,
                getSelectionBarColor: function () {
                    return '#99b841';
                },
                onEnd: function () {
                    vm.changeGridByFilters();
                },
                noSwitching: true
            }

        };
        vm.maxDDSlider = {
            minValue: 0,
            maxValue: 100,
            options: {
                floor: 0,
                ceil: 100,
                step: 1,
                getSelectionBarColor: function () {
                    return '#99b841';
                },
                onEnd: function () {
                    vm.changeGridByFilters();
                },
                noSwitching: true
            }

        };
        vm.CountrySettings = {
            showCheckAll: true,
            showUncheckAll: false,
            smartButtonMaxItems: 1,
            smartButtonTextConverter: function (itemText, originalItem) {
                return itemText;
            }
        }
        vm.onCountrySelect = function (item) {
            vm.changeGridByFilters();
        }
        vm.onCountryDeselect = function (item) {
            vm.changeGridByFilters();
        }

        vm.UserGroupsSettings = {
            showCheckAll: false,
            showUncheckAll: false,
            smartButtonMaxItems: 1,
            smartButtonTextConverter: function (itemText, originalItem) {
                return itemText;
            }
        }
        vm.onUserGroupsSelect = function (item) {
            vm.getTopPerformersByFilters();
        }
        vm.onUserGroupsDeselect = function (item) {
            vm.getTopPerformersByFilters();
        }

        var getAccounts = function () {
            vm.loadComplete = false; //Loader
            performerService.getAccounts(pageRecordModel).then(function (response) {
                
                //vm.TotalRecords = response.data.count;
                if (!response.data.response.Success) {
                    vm.loadComplete = true; 
                    return false;
                }
                var data = response.data.response.MultipleData;
                vm.TotalRecords = response.data.response.MultipleData.count;
                vm.Performers = data.accountDetails;
                //vm.Countries = data.countries;
                if (data.countries.length >= 0) {
                    angular.forEach(data.countries, function (value, index) {
                        vm.Countries.push({ id: value, label: value });
                        //vm.SelectedCountries.push({ id: value, label: value });
                    });
                }
                $('#CountryMultiSelect').multiselect({
                    includeSelectAllOption: true,
                    selectAllText: 'All',
                    numberDisplayed: 50,
                    selectAllNumber: false,
                    buttonText: function (options, select) {
                        if (options.length === 0) {
                            return 'Select Country(s)';
                        }
                        else {
                            var labels = [];
                            options.each(function (index, value) {
                                if (index < 1) {
                                    labels.push($(this).text());
                                }
                                if (index == 1) {
                                    labels.push("...");
                                }
                            });
                            return labels.join(' , ');
                        }
                    },

                    onChange: function (element, checked) {
                        if (checked === true) {
                            vm.SelectedCountries.push(element.context.value);
                        }
                        else {
                            var index = vm.SelectedCountries.indexOf(element.context.value);
                            vm.SelectedCountries.splice(index, 1);
                        }
                        vm.changeGridByFilters();
                    },
                    onSelectAll: function () {
                        angular.forEach(vm.Countries, function (country, index) {
                            vm.SelectedCountries.push(country.id);
                        });
                        vm.changeGridByFilters();
                    },
                    onDeselectAll: function () {
                        vm.SelectedCountries = [];
                        vm.changeGridByFilters();
                    }
                });
                $('#CountryMultiSelect').multiselect('dataprovider', vm.Countries);

                //vm.UserGroups = data.userGroups;
                if (data.userGroups.length >= 0) {
                    angular.forEach(data.userGroups, function (value, index) {
                        vm.UserGroups.push({ id: value, label: value });
                        //vm.SelectedUserGroups.push({ id: value, label: value });
                    });
                }

                $('#UserGroupMultiSelect').multiselect({
                    includeSelectAllOption: true,
                    selectAllText: 'All',
                    numberDisplayed: 50,
                    selectAllNumber: false,
                    buttonText: function (options, select) {
                        if (options.length === 0) {
                            return 'Select User Group(s)';
                        }
                        else {
                            var labels = [];
                            options.each(function (index, value) {
                                //labels.push($(this).text());
                                if (index < 1) {
                                    labels.push($(this).text());
                                }
                                if (index == 1) {
                                    labels.push("...");
                                }
                            });
                            return labels.join(' , ');
                        }
                    },

                    onChange: function (element, checked) {
                        if (checked === true) {
                            vm.SelectedUserGroups.push(element.context.value);
                        }
                        else {
                            var index = vm.SelectedUserGroups.indexOf(element.context.value);
                            vm.SelectedUserGroups.splice(index, 1);
                        }
                        vm.changeGridByFilters();
                    },
                    onSelectAll: function () {
                        angular.forEach(vm.UserGroups, function (userGroup, index) {
                            vm.SelectedUserGroups.push(userGroup.id);
                        });
                        vm.changeGridByFilters();
                    },
                    onDeselectAll: function () {
                        vm.SelectedUserGroups = [];
                        vm.changeGridByFilters();
                    }
                });
                $('#UserGroupMultiSelect').multiselect('dataprovider', vm.UserGroups);
                vm.TimeLines = data.timeLines;

                vm.roiSlider.options.floor = data.minROI;
                vm.roiSlider.options.ceil = data.maxROI;
                vm.roiSlider.maxValue = data.maxROI;
                vm.roiSlider.minValue = data.minROI;

                vm.winSlider.options.floor = data.minWIN;
                vm.winSlider.options.ceil = data.maxWIN;
                vm.winSlider.minValue = data.minWIN;
                vm.winSlider.maxValue = data.maxWIN;

                vm.maxDDSlider.options.floor = data.minDD;
                vm.maxDDSlider.options.ceil = data.maxDD;
                vm.maxDDSlider.maxValue = data.maxDD;
                vm.maxDDSlider.minValue = data.minDD;
                var getSelectedTimeline = data.timeLines.filter(function (timeline) {
                    if (timeline.stringValue == vm.preselectedTimelineID) {
                        return timeline;
                    }
                })
                vm.TimeLine = getSelectedTimeline[0];

                getPageData();
                vm.loadComplete = true; //Loader

            });
            //performerService.getFilteringData(pageRecordModel).then(function (response) {
            //    vm.TotalRecords = response.data.count;
            //    vm.Performers = response.data.records.Data;
            //    getPageData();
            //});
        }

        var init = function () {
            getAccounts();
        }
        init();
        vm.Mycounter = 0;

        //vm.SortPerformers = function (sortColumn, sortOrder) {
        //    $("._sort-icon").show();
        //    // $(event.currentTarget).hide();
        //    vm.Mycounter = vm.Mycounter + 1;
        //    if (vm.Mycounter == 1) {
        //        pageRecordModel.SortOrder = "asc";
        //    }
        //    else if (vm.Mycounter == 2) {
        //        vm.Mycounter = 0;
        //        pageRecordModel.SortOrder = "desc";
        //    }

        //    pageRecordModel.PageNumber = 1;
        //    pageRecordModel.SortBy = sortColumn;
        //   // pageRecordModel.SortOrder = sortOrder;

        //    performerService.getAccounts(pageRecordModel).then(function (response) {
        //        vm.Performers = response.data.response.MultipleData.accountDetails;
        //        getPageData();
        //    });
        //}


        vm.SortPerformers = function (sortColumn) {
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
            vm.loadComplete = false; //Loader

            performerService.getAccounts(pageRecordModel).then(function (response) {
                vm.Performers = response.data.response.MultipleData.accountDetails;
                getPageData();
                vm.loadComplete = true; //Loader

            });
        }

        vm.changePageSize = function () {
            pageRecordModel.PageSize = vm.selectedPageSize;
            pageRecordModel.PageNumber = 1;
            vm.loadComplete = false; //Loader

            performerService.getAccounts(pageRecordModel).then(function (response) {
                vm.Performers = response.data.response.MultipleData.accountDetails;
                getPageData();
                vm.loadComplete = true; //Loader

            });
        }

        vm.changePage = function (value) {
            if (value == -1 && pageRecordModel.PageNumber > 1 || value == 1 && vm.ToRecordNumber < vm.TotalRecords) {
                pageRecordModel.PageNumber = pageRecordModel.PageNumber + (value);
                vm.loadComplete = false; //Loader

                performerService.getAccounts(pageRecordModel).then(function (response) {
                    vm.Performers = response.data.response.MultipleData.accountDetails;
                    getPageData();
                    vm.loadComplete = true; //Loader

                });
            }
        }

        vm.changeGridByFilters = function () {
            if (vm.Country != undefined) {
                pageRecordModel.Filters.Country = vm.Country;
            }
            else if (pageRecordModel.Filters.hasOwnProperty('Country')) {
                delete pageRecordModel.Filters['Country'];
            }

            if (vm.UserGroup != undefined) {
                pageRecordModel.Filters.UserGroup = vm.UserGroup;
            }
            else if (pageRecordModel.Filters.hasOwnProperty('UserGroup')) {
                delete pageRecordModel.Filters['UserGroup'];
            }
            if (vm.TimeLine.stringValue != undefined) {
                pageRecordModel.Filters.TimeLineId = vm.TimeLine.stringValue;
            }
            else if (pageRecordModel.Filters.hasOwnProperty('TimeLineId')) {
                delete pageRecordModel.Filters['TimeLineId'];
            }
            //if (vm.winSlider.value > 0) {
            //    pageRecordModel.Filters.WINRate = vm.winSlider.value;
            //}
            //else if (pageRecordModel.Filters.hasOwnProperty('WINRate')) {
            //    delete pageRecordModel.Filters['WINRate'];
            //}

            if (vm.winSlider.minValue > vm.winSlider.options.floor) {
                pageRecordModel.Filters.WINRateMin = vm.winSlider.minValue;
            }
            else if (pageRecordModel.Filters.hasOwnProperty('WINRateMin')) {
                delete pageRecordModel.Filters['WINRateMin'];
            }
            if (vm.winSlider.maxValue < vm.winSlider.options.ceil) {
                pageRecordModel.Filters.WINRateMax = vm.winSlider.maxValue;
            }
            else if (pageRecordModel.Filters.hasOwnProperty('WINRateMax')) {
                delete pageRecordModel.Filters['WINRateMax'];
            }
            //Roi SLider
            if (vm.roiSlider.minValue > vm.roiSlider.options.floor) {
                pageRecordModel.Filters.ROIMin = vm.roiSlider.minValue;
            }
            else if (pageRecordModel.Filters.hasOwnProperty('ROIMin')) {
                delete pageRecordModel.Filters['ROIMin'];
            }

            if (vm.roiSlider.maxValue < vm.roiSlider.options.ceil) {
                pageRecordModel.Filters.ROIMax = vm.roiSlider.maxValue;
            }
            else if (pageRecordModel.Filters.hasOwnProperty('ROIMax')) {
                delete pageRecordModel.Filters['ROIMax'];
            }
            //DD SLider
            if (vm.maxDDSlider.minValue > vm.maxDDSlider.options.floor) {
                pageRecordModel.Filters.DDMin = vm.maxDDSlider.minValue;
            }
            else if (pageRecordModel.Filters.hasOwnProperty('DDMin')) {
                delete pageRecordModel.Filters['DDMin'];
            }

            if (vm.maxDDSlider.maxValue < vm.maxDDSlider.options.ceil) {
                pageRecordModel.Filters.DDMax = vm.maxDDSlider.maxValue;
            }
            else if (pageRecordModel.Filters.hasOwnProperty('DDMax')) {
                delete pageRecordModel.Filters['DDMax'];
            }
            //if (vm.roiSlider.value > 0) {
            //    pageRecordModel.Filters.ROI = vm.roiSlider.value;
            //}
            //else if (pageRecordModel.Filters.hasOwnProperty('ROI')) {
            //    delete pageRecordModel.Filters['ROI'];
            //}
            //   pageRecordModel.Filters.ROI = vm.roiSlider.value;
            //   pageRecordModel.Filters.DD = vm.maxDDSlider.value;
            //if (vm.maxDDSlider.value > 0) {
            //    pageRecordModel.Filters.DD = vm.maxDDSlider.value;
            //}
            //else if (pageRecordModel.Filters.hasOwnProperty('DD')) {
            //    delete pageRecordModel.Filters['DD'];
            //}
            if (vm.Search != undefined || vm.Search != "") {
                pageRecordModel.Filters.Search = vm.Search;
            }
            else if (pageRecordModel.Filters.hasOwnProperty('Search')) {
                delete pageRecordModel.Filters['Search'];
            }

            if (vm.SelectedCountries != undefined) {
                var SelectedCountries = [];
                angular.forEach(vm.SelectedCountries, function (item, countryIndex) {
                    //SelectedCountries.push(item.id);
                    SelectedCountries.push(item);
                });
                pageRecordModel.MultipleListFilter.Countries = SelectedCountries;
            }
            //else {
            //    delete pageRecordModel.MultipleListFilter['Countries'];
            //}

            if (vm.SelectedUserGroups != undefined) {
                var SelectedUserGroups = [];
                angular.forEach(vm.SelectedUserGroups, function (item, countryIndex) {
                    SelectedUserGroups.push(item);
                });
                pageRecordModel.MultipleListFilter.UserGroups = SelectedUserGroups;
            }
            //else {
            //    delete pageRecordModel.MultipleListFilter['UserGroups'];
            //}

            pageRecordModel.PageNumber = 1;
            vm.loadComplete = false; //Loader

            performerService.getAccounts(pageRecordModel).then(function (response) {
                vm.Performers = response.data.response.MultipleData.accountDetails;
                // vm.TotalRecords = response.data.count;
                vm.TotalRecords = response.data.response.MultipleData.count;
                vm.ToRecordNumber = 0;
                getPageData();
                vm.loadComplete = true; //Loader

            });
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
        vm.updatePinnedUsers = function () {
            var count = 0;
            var a = vm.selectedAccounts;
            var selectedAccountDetailsIds = []
            angular.forEach(vm.selectedAccounts.accountId, function (value, accountId) {
                if (value == true) {
                    selectedAccountDetailsIds.push(accountId);
                    count++;
                }
            })
            if (count == 0) {
                //toastr.warning("Please Select Performer");
            }
            else {
                vm.loadComplete = false; //Loader

                performerService.updatePinnedUsers(selectedAccountDetailsIds).then(function (response) {
                    if (response.data.Success) {
                        toastr.success(response.data.Message);
                    }
                    else {
                        toastr.warning(response.data.Message);
                    }
                    vm.loadComplete = true; //Loader

                });
            }
        }


        vm.updateExcludeUsers = function () {
            var count = 0;
            var a = vm.selectedAccounts;
            var selectedAccountDetailsIds = []
            angular.forEach(vm.selectedAccounts.accountId, function (value, accountId) {
                if (value == true) {
                    selectedAccountDetailsIds.push(accountId);
                    count++;
                }
            })
            if (count != 0) {
                BootstrapDialog.confirm({
                    title: 'DELETE PERFORMER(S)',
                    message: 'Are you sure you want to delete selected performer(s)?',
                    callback: function (result) {
                        // result will be true if button was click, while it will be false if users close the dialog directly.
                        if (result) {

                            vm.loadComplete = false; //Loader

                            performerService.updateExcludeUsers(selectedAccountDetailsIds).then(function (response) {
                                if (response.data.Success) {
                                    toastr.success(response.data.Message);
                                    performerService.getAccounts(pageRecordModel).then(function (response) {
                                        vm.Performers = response.data.response.MultipleData.accountDetails;
                                        vm.TotalRecords = response.data.response.MultipleData.count;
                                        getPageData();
                                        vm.loadComplete = true; //Loader

                                    });
                                }
                                else {
                                    toastr.warning(response.data.Message);
                                }
                            });
                        }
                    }
                });
            }
        }

        vm.UpdateCompareData = function () {
            var count = 0;
            var CompareDataFromLocalStorage = [];
            angular.forEach(vm.selectedAccounts.accountId, function (value, accountId) {
                if (value == true)
                    count++;
            })
            if (count > 5) {
                //toastr.warning("You have selected more then 5");
            }
            else if (count == 0) {
                //toastr.warning("Please Select Performer");
            }
            else {
                //  CompareDataFromLocalStorage = angular.fromJson(CompareDataFromLocalStorage);
                angular.forEach(vm.selectedAccounts.accountId, function (value, accountId) {
                    if (value == true) {
                        var accountId = accountId;
                        var value = true;
                        CompareDataFromLocalStorage.push({ accountId, value });
                    }
                })
                localStorage.setItem('CompareData', JSON.stringify(CompareDataFromLocalStorage));

                localStorage.setItem('selectedTimelineId', vm.TimeLine.stringValue);
                //toastr.success("Successfully Added to compare");
                localStorage.setItem('previousLocation', 'Performers');
                $location.path("/Compare");
            }
        }

        vm.viewUserDetails = function () {
            var Count = 0;
            var selectedPerformer = ""
            angular.forEach(vm.selectedAccounts.accountId, function (value, accountId) {
                if (value == true) {
                    selectedPerformer = accountId;
                    Count++;
                }
            })
            if (Count > 1) {
                //toastr.warning("Please select one only.");
            }
            else if (Count == 0) {
                //toastr.warning("Please Select Performer");
            }
            else {
                $location.path("/UserDetails/" + selectedPerformer);
                localStorage.setItem('previousLocation', 'Performers');
                localStorage.setItem('selectedTimelineId', vm.TimeLine.stringValue);
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

        vm.selectAll = function () {
            if (vm.selectAllPerformers) {
                angular.forEach(vm.Performers, function (performer, index) {
                    performer.IsChecked = true;
                })
            }
            else {
                angular.forEach(vm.Performers, function (performer, index) {
                    performer.IsChecked = false;
                })
            }
        }

        vm.export = function () {
            
            performerService.exportExcel(pageRecordModel);
        }
    }],
    controllerAs: 'vm'
});