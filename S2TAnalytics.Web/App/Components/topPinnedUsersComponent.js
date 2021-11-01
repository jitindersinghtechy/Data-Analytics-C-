app.component("topFivePinnedUsers", {
    //templateUrl: serviceBase + "App/Views/Components/PinnedUsers.html",
    templateUrl: serviceBase + "App/Views/Components/topPinnedusers.html",
    binding: { name: "@" },
    controller: ["$scope", "topPinnedUsersService", "$location", 'preselectedTimelineID', function ($scope, topPinnedUsersService, $location, preselectedTimelineID) {
        var vm = this;
        vm.Countries = [];
        vm.TimeLines = [];
        vm.NoPinnedUserFound = true;


        var permissions = $scope.$parent.mainVM;
        vm.isPinnedUsers = $scope.$parent.mainVM.isPinnedUsers;
        vm.isExcludeUser = $scope.$parent.mainVM.isExcludeUser;
        vm.isViewDetail = $scope.$parent.mainVM.isViewDetail;
        vm.isCompare = $scope.$parent.mainVM.isCompare;
        vm.isExportData = $scope.$parent.mainVM.isExportData;
        vm.isEmbedWidget = $scope.$parent.mainVM.isEmbedWidget;
        vm.isAddReport = $scope.$parent.mainVM.isAddReport;
        //vm.TimelineId = "yDwjeVGRQ8E=";
        var pageRecordModel = {
            Filters: {
                TimeLineId: preselectedTimelineI
            },
        };
        var getTopFivePinnedUsers = function () {
            topPinnedUsersService.getTopFivePinnedUsers(pageRecordModel).then(function (response) {
                if (response.data.Success) {
                    var data = response.data.MultipleData;
                    vm.WidgetId = data.WidgetId;
                    vm.PinnedUsers = data.performers;
                    vm.Countries = data.countries;
                    vm.TimeLines = data.timeLines;
                    vm.TimeLine = data.timeLines[0];
                    vm.NoPinnedUserFound = false;
                    setTimeout(function () {
                        setupcarousel();
                    }, 100);
                }
                else {
                    vm.NoPinnedUserFound = true;
                    //toastr.warning(response.data.Message);
                }
            });
        }
        var init = function () {
            getTopFivePinnedUsers();
        }
        init();

        vm.getAccountStatsValueByTimeLine = function (column, accountDetail) {
            var accountStat = [];
            angular.forEach(accountDetail.AccountStats, function (value, index) {
                if (value.TimeLineId == vm.TimeLine.stringValue) {
                    accountStat = value;
                }
            });
            return accountStat[column];
        }
        vm.getTopPinnedUsersByFilters = function () {
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

            topPinnedUsersService.getTopFivePinnedUsers(pageRecordModel).then(function (response) {
                vm.PinnedUsers = response.data.MultipleData.performers;
                setTimeout(function () {
                    setupcarousel();
                }, 100);
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
            //toastr.success("Successfully Added to compare");
            localStorage.setItem('previousLocation', 'PinnedUsers');
            $location.path("/Compare");
        }

        vm.unpinUser = function (pinnedUser) {
            topPinnedUsersService.unpinUser(pinnedUser.AccountDetailId).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    toastr.success(response.data.Message);
                    var index = vm.PinnedUsers.indexOf(pinnedUser);
                    vm.PinnedUsers.splice(index, 1);
                }
                else {
                    toastr.warning(response.data.Message);
                }
            });
        }

        var setupcarousel = function () {
            var owl = $('#pinnedUsers');
            if (typeof owl.data('owlCarousel') != 'undefined') {
                owl.data('owlCarousel').destroy();
                owl.removeClass('owl-carousel');
            }

            owl.owlCarousel({
                loop: true,
                autoPlay: 2500,
                items: 5,
                nav: true,
                responsive: {
                    1024: { items: 5 },
                    801: { items: 4 },
                    667: { items: 3 },
                    568: { items: 2 },
                    320: { items: 1 }
                }
            });
        };

        vm.ShowScript = function () {
            topPinnedUsersService.getWidgetPermission(vm.WidgetId).then(function (response) {
                if (response.data.Success) {
                    vm.tags = response.data.Data != null ? response.data.Data.Domain : null;
                    $('#txtTopPinnedUsersScript').text('<script type="text/javascript" id="scriptTopPinnedUsers" token="' + localStorage.getItem('accessToken') + '" src="http://analyticsv1.azurewebsites.net/App/Widgets/TopPinnedUsers/render.js"></script><top-pinned-users app="topPinnedUsersApp"></top-pinned-users>');
                    $("#embedScriptTPUPopup").modal('show');
                }
            });
        }

        vm.CopyToClipboard = function (elementId) {

            copyToClipboard($("#txtTopPinnedUsersScript")[0]);
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
            topPinnedUsersService.SaveEmbedWidgetPermission(vm.WidgetId, vm.Domain).then(function (response) {
                if (response.data) {
                    toastr.success("Applied");
                }
            });


        }
    }],
    controllerAs: "vm"
})