
app.component("topPerformers", {
    templateUrl: serviceBase + "App/Views/Components/topPerformers.html",
    bindings: { name: '@' },
    controller: ['$scope', 'topPerformersService', '$location', '$timeout', 'preselectedTimelineID',
        function ($scope, topPerformersService, $location, $timeout, preselectedTimelineID) {
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
            vm.preselectedTimelineID = preselectedTimelineID;
            vm.loadComplete = false; //Loader

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
                    vm.WidgetId = data.WidgetId
                    vm.Performers = data.performers;
                    vm.pinnedUsersList = data.pinnedUsers;
                    vm.RORForTimeline = data.riskFreeRate;
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
                    //vm.TimeLine = data.timeLines[0];
                    //vm.PerformerTimeLine = data.timeLines[0];
                    var getSelectedTimeline = data.timeLines.filter(function (timeline) {
                        if (timeline.stringValue == vm.preselectedTimelineID) {
                            return timeline;
                        }
                    })
                    vm.TimeLine = getSelectedTimeline[0];
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
                    vm.RORForTimeline = response.data.response.MultipleData.riskFreeRate;
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
                localStorage.setItem('selectedTimelineId', vm.TimeLine.stringValue);
                //toastr.success("Successfully Added to compare");
                localStorage.setItem('previousLocation', 'Performers');
                $location.path("/Compare");
            }

            vm.viewUserDetails = function (accountDetailId) {
                $location.path("/UserDetails/" + accountDetailId);
                localStorage.setItem('previousLocation', 'Performers');
                localStorage.setItem('selectedTimelineId', vm.TimeLine.stringValue);
            }

            var setupcarousel = function (destroy) {
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
                //}



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

            vm.ShowScript = function () {

                topPerformersService.getWidgetPermission(vm.WidgetId).then(function (response) {
                    if (response.data.Success) {
                        vm.tags = response.data.Data != null ? response.data.Data.Domain : null;
                        $('#txtTrendingScript').val('<script type="text/javascript" id="scriptTrendingPerformers" token="' + localStorage.getItem('accessToken') + '" src="http://analyticsv1.azurewebsites.net/App/Widgets/TrendingPerformers/render.js"></script><div class="block _performers"><div class="container"><top-performers app="trendingPerformersApp"></top-performers></div></div>');
                        $('#divTrendingScript').text('<script type="text/javascript" id="scriptTrendingPerformers" token="' + localStorage.getItem('accessToken') + '" src="http://analyticsv1.azurewebsites.net/App/Widgets/TrendingPerformers/render.js"></script><div class="block _performers"><div class="container"><top-performers app="trendingPerformersApp"></top-performers></div></div>');
                        $("#embedScriptTrendingPopup").modal('show');
                    }
                });

            }
            vm.EmbedAPI = function () {
                $('#apiLinkText').text(location.host + "/api/Performers/GetTop5Performers");
                $('#EmbedAPIToken').text("Bearer " + localStorage.getItem('accessToken'));
                $('#ExportAPIParams').text(JSON.stringify(pageRecordModel));
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
                $('#txtTrendingScript').css("display", "block");
                copyToClipboard($("#txtTrendingScript")[0]);
                $('#txtTrendingScript').css("display", "none");
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
                    topPerformersService.SaveEmbedWidgetPermission(vm.WidgetId, vm.Domain).then(function (response) {
                        if (response.data) {
                            toastr.success("Applied");
                        }
                    });
                }
            }

        }],
    controllerAs: 'vm'
});