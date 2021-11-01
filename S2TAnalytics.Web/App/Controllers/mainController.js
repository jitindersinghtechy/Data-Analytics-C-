'use strict';
app.controller('mainController', ['$rootScope', 'mainService', '$scope', '$filter', '$location', '$window', '$http', function ($rootScope, mainService, $scope, $filter, $location, $window, $http) {
    var vm = this;
    if (localStorage.getItem("planPermissionIds") == null) {
        $(".preloader").show();
        $window.localStorage.clear();
        $location.path('/login');
    }
    else {
        vm.Notifications = [];
        vm.NotificationCount = 0;
        vm.selectedMenu = "Dashboard";
        vm.PrevPlan = "";
        vm.NewPlan = "";
        vm.fullName = localStorage.getItem("fullName");
        vm.UserRoleId = localStorage.getItem('roleId');
        vm.Plans = [];
        vm.TermLengths = [];
        vm.SelectedTermLength = "1";
        vm.TotalAmount = 0;
        vm.PlanAmount = 0;
        vm.PaymentFees = 0;
        vm.serviceAmount = 0;
        vm.Discount = 0;
        vm.UserCreditAmount = 0;
        vm.TotalUserCreditAmount = 0;
        vm.InfrastructureCost = 0;
        vm.TermLengthStringValue = "";
        vm.Promocode = "";
        vm.noPaymentAdded = false;


        var permissionPlanIds = localStorage.getItem("planPermissionIds").split(",");
        var checkPermissionPlan = function (planFeature) {
            switch (planFeature) {
                case enumPermissionPlans.Dashboard:
                    return $filter('filter')(permissionPlanIds, planFeature, true).length > 0;
                case enumPermissionPlans.AccountDetail:
                    return $filter('filter')(permissionPlanIds, planFeature, true).length > 0;
                case enumPermissionPlans.Compare:
                    return $filter('filter')(permissionPlanIds, planFeature, true).length > 0;
                case enumPermissionPlans.Pin_Account:
                    return $filter('filter')(permissionPlanIds, planFeature, true).length > 0;
                case enumPermissionPlans.Performance_Comparison_Email_Notification:
                    return $filter('filter')(permissionPlanIds, planFeature, true).length > 0;
                case enumPermissionPlans.Exclude_Account:
                    return $filter('filter')(permissionPlanIds, planFeature, true).length > 0;
                case enumPermissionPlans.Add_Report_User:
                    return $filter('filter')(permissionPlanIds, planFeature, true).length > 0;
                case enumPermissionPlans.Embed_Widget:
                    return $filter('filter')(permissionPlanIds, planFeature, true).length > 0;
                case enumPermissionPlans.API_Data_for_Widget:
                    return $filter('filter')(permissionPlanIds, planFeature, true).length > 0;
                case enumPermissionPlans.Export_Data:
                    return $filter('filter')(permissionPlanIds, planFeature, true).length > 0;

            }
        }

        vm.isDashBoard = checkPermissionPlan(enumPermissionPlans.Dashboard);
        vm.isPinnedUsers = checkPermissionPlan(enumPermissionPlans.Pin_Account);
        vm.isExcludeUser = checkPermissionPlan(enumPermissionPlans.Exclude_Account);
        vm.isEmbedWidget = checkPermissionPlan(enumPermissionPlans.Embed_Widget);
        vm.isAddReport = checkPermissionPlan(enumPermissionPlans.Add_Report_User);
        vm.isViewDetail = checkPermissionPlan(enumPermissionPlans.AccountDetail);
        vm.isCompare = checkPermissionPlan(enumPermissionPlans.Compare);
        vm.isExportData = checkPermissionPlan(enumPermissionPlans.Export_Data);
        vm.isEmailNotification = checkPermissionPlan(enumPermissionPlans.Performance_Comparison_Email_Notification);
        vm.isApiWidget = checkPermissionPlan(enumPermissionPlans.API_Data_for_Widget);



        //vm.Plan1 = {
        //    PlanID: 1,
        //    Price: 2000,
        //    Widgets: ["Dashboard with drilldown (Trader statistics)", "View Details account details", "Compare account performance", "Pin account"]

        //};
        //vm.Plan2 = {
        //    PlanID: 2,
        //    Price: 8000,
        //    Widgets: ["Dashboard with drilldown (Trader statistics)", "View Details account details", "Compare account performance", "Pin account"]
        //};
        //vm.Plan3 = {
        //    PlanID: 3,
        //    Price: 15000,
        //    Widgets: ["Dashboard with drilldown (Trader statistics)", "View Details account details", "Compare account performance", "Pin account", "Exclude accounts", "Add report users", "Embed dashboard widgets", "Export dashboard"]
        //};

        vm.Plans1 = [{
            PlanID: 1,
            Price: 100,
            Name: "Silver",
            Widgets: ["Dashboard with drilldown (Trader statistics)", "View Details account details", "Compare account performance", "Pin account"]
        },
       {
           PlanID: 2,
           Price: 200,
           Name: "GOld",
           Widgets: ["Dashboard with drilldown (Trader statistics)", "View Details account details", "Compare account performance", "Pin account"]
       },
       {
           PlanID: 3,
           Price: 300,
           Name: "Platinum",
           Widgets: ["Dashboard with drilldown (Trader statistics)", "View Details account details", "Compare account performance", "Pin account", "Exclude accounts", "Add report users", "Embed dashboard widgets", "API Data for the dashboard widgets", "Export dashboard, performers", "Account statement"]
       }]

        vm.signout = function () {
            $(".preloader").show();
            $window.localStorage.clear();
            $location.path('/login');

        }
        vm.requestSuccess = false;
        vm.IsRequestDisabled = false;
        vm.getClass = function (path) {
            return ($location.path().substr(0, path.length) === path) ? 'active' : '';
        }

        $scope.$watch(function () {
            return $location.path();
        }, function (newValue, oldValue) {
            vm.IsExportVisible = (newValue == "/Configure" || newValue == "/Account" || newValue == "/UserAccount") ? false : true;
        });

        $scope.$watch(function () {
            return localStorage.getItem("triggerNotification");
        }, function (newValue, oldValue) {
            if (newValue == "true") {
                getNotifications().then(function (response) {
                    vm.Notifications = response.data.MultipleData.notifications;
                    vm.NotificationCount = response.data.MultipleData.notificationCount;
                    localStorage.setItem("triggerNotification", false);
                });
            }
        });

        getNotifications().then(function (response) {
            vm.Notifications = response.data.MultipleData.notifications;
            vm.NotificationCount = response.data.MultipleData.notificationCount;
        });

        function getNotifications() {
            return $http.get(serviceBase + "api/Configure/GetNotifications").success(getNotificationsComplete).error(getNotificationsFailed);
            function getNotificationsComplete(response) {
                return response;
            }
            function getNotificationsFailed(err, status) {

            }
        }

        vm.getAllNotifications = function () {
            return $http.get(serviceBase + "api/Configure/GetAllNotifications").success(getAllNotificationsComplete).error(getAllNotificationsFailed);
            function getAllNotificationsComplete(response) {
                vm.allNotifications = true;
                vm.Notifications = response.Data;
            }
            function getAllNotificationsFailed(err, status) {

            }
        }

        vm.readNotifications = function () {
            return $http.post(serviceBase + "api/Configure/ReadNotifications").success(readNotificationsComplete).error(readNotificationsFailed);
            function readNotificationsComplete(response) {
                if (response.Success) {
                    vm.NotificationCount = 0;
                    return response;
                }
            }
            function readNotificationsFailed(err, status) {

            }
        }

        vm.ChangeDatasources = function () {
            var ids = [];
            angular.forEach(vm.DataSources, function (item, index) {
                if (item.IsChecked) {
                    ids.push(item.Id);
                }
            });
            changeDatasources(ids).then(function (response) {
                if (response.data) {
                    $window.location.reload();
                }
            });
        }

        var changeDatasources = function (ids) {
            return $http.post(serviceBase + "api/Configure/FilterDatasource", ids).success(changeDatasourcesComplete).error(changeDatasourcesFailed);
            function changeDatasourcesComplete(response) {
                if (response.Success) {
                    vm.NotificationCount = 0;
                    return response;
                }
            }
            function changeDatasourcesFailed(err, status) {

            }
        }

        getDataSources().then(function (response) {
            vm.DataSources = response.data.Data;
            if (vm.UserRoleId != 3 && vm.DataSources.length == 0) {
                vm.NoDataSource = true;
                $location.path('/Configure');
            }
            else {
                vm.NoDataSource = false;
                angular.forEach(response.data.MultipleData.selectedDatsources, function (id, index) {
                    angular.forEach(vm.DataSources, function (item, index) {
                        if (id == item.Id) {
                            item.IsChecked = true;
                        }
                    });
                });
            }
        });

        vm.clearSingleNotification = function (id, index) {
            removeSingleNotification(id).then(function (response) {

                if (response.data.Success) {
                    vm.Notifications.splice(index, 1);
                }
            });
        }

        vm.clearAllNotification = function () {
            BootstrapDialog.confirm({
                title: 'REMOVE ALL NOTIFICATION',
                message: 'Are you sure you want remove all notification?',
                callback: function (result) {
                    // result will be true if button was click, while it will be false if users close the dialog directly.
                    if (result) {
                        removeAllNotification().then(function (response) {
                            if (response.data.Success) {
                                vm.Notifications = "";
                            }
                        });
                    }
                }
            });
        }

        //Remove Notification
        function removeSingleNotification(id) {
            return $http.get(serviceBase + "api/Configure/RemoveSingleNotification/" + id).success(GetNotificationsComplete).error(GetNotificationsFailed);
            function GetNotificationsComplete(response) {
                return response;
            }
            function GetNotificationsFailed(err, status) {
            }
        }
        function removeAllNotification() {
            return $http.get(serviceBase + "api/Configure/RemoveAllNotification").success(GetNotificationsComplete).error(GetNotificationsFailed);
            function GetNotificationsComplete(response) {
                return response;
            }
            function GetNotificationsFailed(err, status) {
            }
        }
        $scope.$watch(function () {
            return localStorage.getItem("triggerDataSource");
        }, function (newValue, oldValue) {
            if (newValue == "true") {
                getDataSources().then(function (response) {
                    vm.DataSources = response.data.Data;
                    if (vm.UserRoleId != 3 && vm.DataSources.length == 0) {
                        vm.NoDataSource = true;
                        $location.path('/Configure');
                    }
                    else {
                        vm.NoDataSource = false;
                    }
                    localStorage.setItem("triggerDataSource", false);
                });
            }
        });
        vm.IsDataSource = function (e) {
            vm.NoDataSource ? e.preventDefault() : '';
            localStorage.getItem('IsExpire') != null ? localStorage.getItem('IsExpire') == "True" ? e.preventDefault() : '' : '';
        }
        function getDataSources() {
            return $http.get(serviceBase + "api/Configure/GetDataSourcesForUser").success(GetDataSourcesComplete).error(GetDataSourcesFailed);
            function GetDataSourcesComplete(response) {
                return response;
            }
            function GetDataSourcesFailed(err, status) {
            }
        }
        vm.clearAllDataSources = function () {
            BootstrapDialog.confirm({
                title: 'REMOVE ALL DATASOURCES',
                message: 'Are you sure you want remove all data sources?',
                callback: function (result) {
                    // result will be true if button was click, while it will be false if users close the dialog directly.
                    if (result) {
                        removeAllDataSource().then(function (response) {
                            if (response.data.Success) {
                                vm.DataSources = "";
                            }
                        });
                    }
                }
            });
        }
        vm.clearSingleDataSource = function (id, index) {
            removeSingleDataSource(id).then(function (response) {
                if (response.data.Success) {
                    vm.DataSources.splice(index, 1);
                }
            });
        }
        vm.planScreeen = 1;
        vm.planPrice = 0;
        vm.preSelectedPlanId = 0;
        vm.preRequestPlan = function ($event, plan) {
            if (plan.PlanId == 3) {
                vm.requestPlan($event, plan);
            }
            else {
                vm.planScreeen = 2;
                vm.PlanName = plan.Name;
                vm.PlanTermLength = plan.PlanTermLength;
                vm.planPrice = plan.Price;
                vm.InfrastructureCost = plan.InfrastructureCost;
                var termlength = { intValue: "1", DisplayName: "1 Month" }
                return $http.get(serviceBase + "api/UserAccount/PreRequestPlan/" + plan.PlanId).success(PreRequestPlanComplete).error(PreRequestPlanFailed);
                function PreRequestPlanComplete(response) {
                    if (response.Success) {
                        vm.TotalUserCreditAmount = response.Data;
                        vm.preSelectedPlanId = plan.PlanId;
                        vm.changeTermLength(termlength, vm.planPrice * vm.accountsCount);
                    }
                }
                function PreRequestPlanFailed(err, status) {
                }
            }
        }
        $('.subscriptionModal').on('hidden.bs.modal', function () {
            vm.planScreeen = 1;
            vm.planPrice = 0;
            vm.preSelectedPlanId = 0;
        })
        vm.requestPlan = function ($event, plan) {      //vm.requestPlan = function ($event, planId) {   
            var planId = vm.preSelectedPlanId;
            if (vm.Discount == 0)
                vm.Promocode = "";
            if (planId == 1 || planId == 2) {
                BootstrapDialog.confirm({
                    title: 'SELECT PLAN',
                    message: 'Are you sure you want to subscribe this plan?',
                    callback: function (result) {
                        // result will be true if button was click, while it will be false if users close the dialog directly.
                        if (result) {
                            var data = { PlanId: planId, Promocode: vm.Promocode, TermLengthId: parseInt(vm.SelectedTermLength) }
                            vm.IsRequestDisabled = true;
                            $($event.currentTarget).append("<div class='preloader show-loder'><img src='../Images/loading-circle.svg' class='img' /></div>");
                            return $http.post(serviceBase + "api/UserAccount/RequestPlan", data).success(requestPlanComplete).error(requestPlanFailed);
                            function requestPlanComplete(response) {
                                if (response.Success) {
                                    vm.IsRequestDisabled = false;
                                    vm.requestSuccess = true;
                                    $rootScope.$broadcast('userInvoiceHistory', {
                                        data: "1"
                                    });
                                    if (response.MultipleData) {
                                        $rootScope.$broadcast('subscribedPlan', {
                                            data: response.MultipleData.userPlan
                                        });
                                        vm.PlanId = response.MultipleData.userPlan.PlanID;
                                        vm.NewPlan = response.MultipleData.userPlan.Name;
                                        vm.TotalDays = response.MultipleData.userPlan.Days;
                                        vm.planScreeen = 3;
                                    }
                                    localStorage.setItem('planPermissionIds', response.Data);
                                    permissionPlanIds = localStorage.getItem("planPermissionIds").split(",");
                                    vm.isDashBoard = checkPermissionPlan(enumPermissionPlans.Dashboard);
                                    vm.isPinnedUsers = checkPermissionPlan(enumPermissionPlans.Pin_Account);
                                    vm.isExcludeUser = checkPermissionPlan(enumPermissionPlans.Exclude_Account);
                                    vm.isEmbedWidget = checkPermissionPlan(enumPermissionPlans.Embed_Widget);
                                    vm.isAddReport = checkPermissionPlan(enumPermissionPlans.Add_Report_User);
                                    vm.isViewDetail = checkPermissionPlan(enumPermissionPlans.AccountDetail);
                                    vm.isCompare = checkPermissionPlan(enumPermissionPlans.Compare);
                                    vm.isExportData = checkPermissionPlan(enumPermissionPlans.Export_Data);
                                    vm.isEmailNotification = checkPermissionPlan(enumPermissionPlans.Performance_Comparison_Email_Notification);
                                    vm.isApiWidget = checkPermissionPlan(enumPermissionPlans.API_Data_for_Widget);
                                    toastr.success(response.Message);
                                    localStorage.setItem("triggerNotification", true);
                                }
                                else {
                                    vm.IsRequestDisabled = false;
                                    vm.requestSuccess = false;
                                    toastr.warning(response.Message);
                                    vm.noPaymentAdded = true;
                                    //toastr.warning(response.Message + "<br/><a href ='#' prevent-default ng-click='vm.closePlanPopup()' data-dismiss='modal'> Click here to add one payment method</a>");
                                }
                                $(".preloader").remove();
                                //$(".fade.bs-example-modal-lg").modal('hide');
                            }
                            function requestPlanFailed(err, status) {
                            }
                        }
                    }
                });
            }
            else {
                BootstrapDialog.confirm({
                    title: 'SELECT PLAN',
                    message: 'Are you sure you want to request for this plan?',
                    callback: function (result) {
                        // result will be true if button was click, while it will be false if users close the dialog directly.
                        if (result) {
                            vm.IsRequestDisabled = true;
                            var data = { PlanId: plan.PlanId, Promocode: vm.Promocode, TermLengthId: parseInt(vm.SelectedTermLength) }
                            $($event.currentTarget).append("<div class='preloader show-loder'><img src='../Images/loading-circle.svg' class='img' /></div>");
                            return $http.post(serviceBase + "api/UserAccount/RequestPlan", data).success(requestPlanComplete).error(requestPlanFailed);
                            function requestPlanComplete(response) {
                                if (response.Success) {
                                    vm.IsRequestDisabled = false;
                                    vm.requestSuccess = true;
                                    vm.planScreeen = 3;
                                    if (response.MultipleData) {
                                        $rootScope.$broadcast('subscribedPlan', {
                                            data: response.MultipleData.userPlan
                                        });
                                        vm.PlanId = response.MultipleData.userPlan.PlanID;
                                        vm.NewPlan = response.MultipleData.userPlan.Name;
                                        vm.TotalDays = response.MultipleData.userPlan.Days;
                                        vm.planScreeen = 3;
                                    }
                                    toastr.success(response.Message);
                                    localStorage.setItem("triggerNotification", true);
                                }
                                else {
                                    vm.IsRequestDisabled = false;
                                    vm.requestSuccess = false;
                                    toastr.warning(response.Message);
                                }
                                $(".preloader").remove();
                                //$(".fade.bs-example-modal-lg").modal('hide');
                            }
                            function requestPlanFailed(err, status) {
                            }
                        }
                    }
                });
            }
        }
        vm.goToAccountSetting = function () {
            debugger;
            vm.noPaymentAdded = false;
            $(".fade.bs-example-modal-lg").modal('hide');
            $location.path('/Account');
        }
        vm.closePlanPopup = function () {
            $(".fade.bs-example-modal-lg").modal('hide');
            $location.path('/Account');
        }
        //remove DataSource
        function removeSingleDataSource(id) {
            return $http.get(serviceBase + "api/Configure/RemoveSingleDataSource/" + id).success(GetDataSourcesComplete).error(GetDataSourcesFailed);
            function GetDataSourcesComplete(response) {
                return response;
            }
            function GetDataSourcesFailed(err, status) {
            }
        }
        function removeAllDataSource() {
            return $http.get(serviceBase + "api/Configure/RemoveAllDataSource").success(GetDataSourcesComplete).error(GetDataSourcesFailed);
            function GetDataSourcesComplete(response) {
                return response;
            }
            function GetDataSourcesFailed(err, status) {
            }
        }
        $('ul#dataSource_ul').click(function (e) {
            $('li#dataSource_li').addClass('open');
            e.stopPropagation();
        });
        $('ul#notification_ul').click(function (e) {
            $('li#notification_li').addClass('open');
            e.stopPropagation();
        });
        //$('#country').click(function () {
        //    $(this).toggleClass('open');
        //});
        //$('.multi-select-menu.dropdown-menu li.submit a').click(function () {
        //    $('#country').removeClass('open');
        //});
        // Plan Details
        function getPlanDetails() {
            return $http.get(serviceBase + "api/UserAccount/GetPlanDetails").success(getPlanDetailsComplete).error(getPlanDetailsFailed);
            function getPlanDetailsComplete(response) {
                return response;
            }
            function getPlanDetailsFailed(err, status) {
            }
        }
        $scope.$watch(function () {
            return localStorage.getItem("triggerPlanChange");
        }, function (newValue, oldValue) {
            if (newValue == "true") {
                getPlanDetails().then(function (response) {
                    vm.PlanId = response.data.MultipleData.PlanId;
                    vm.DaysLeft = response.data.MultipleData.DaysLeft;
                    vm.TotalDays = response.data.MultipleData.TotalDays;
                    vm.accountsCount = response.data.MultipleData.accounts;
                    localStorage.setItem("triggerPlanChange", false);
                });
            }
        });
        getPlanDetails().then(function (response) {
            vm.PlanId = response.data.MultipleData.PlanId;
            vm.DaysLeft = response.data.MultipleData.DaysLeft;
            vm.accountsCount = response.data.MultipleData.accounts;
            vm.TotalDays = response.data.MultipleData.TotalDays;
        });
        vm.download = function () {
            BootstrapDialog.confirm({
                title: 'DOWNLOAD PDF',
                message: 'Do you want to download pdf ?',
                callback: function (result) {
                    if (result) {
                        generatePdf();
                    }
                }
            });
        }
        function generatePdf() {
            var location = $location.path().split("/")[1];
            var html = $("<div/>");
            html = html.html($('body > div').html());
            html.find("nav, footer").remove();
            html.find(".owl-stage-outer a").remove();
            var fileName = "";
            switch (location) {
                case "Dashboard":
                    html.find("#comparisonDataContainer").html("<img src='data:image/png;base64," + getBase64("comparisonDataContainer") + "' />");
                    html.find("#navMapDataContainer").html("<img src='data:image/png;base64," + getBase64("navMapDataContainer") + "' />");
                    html.find("#locationInstrumentsContainer").html("<img src='data:image/png;base64," + getBase64("locationInstrumentsContainer") + "' />");
                    html.find("#groupInstrumentsContainer").html("<img src='data:image/png;base64," + getBase64("groupInstrumentsContainer") + "' />");
                    fileName = "Dashboard";
                    break;
                case "Performers":
                    fileName = "performers";
                    break;
                case "PinnedUsers":
                    fileName = "Pinned Users";
                    break;
                case "UserDetails":
                    html.find("#performanceData").html("<img src='data:image/png;base64," + getBase64("performanceData") + "' />");
                    html.find("#NavChart").html("<img src='data:image/png;base64," + getBase64("NavChart") + "' />");
                    html.find("#InstrumentalTradeChart").html("<img src='data:image/png;base64," + getBase64("InstrumentalTradeChart") + "' />");

                    fileName = "User Details";
                    break;
                case "Compare":
                    fileName = "Comparison";
                    break;
                default:
                    break;
            }
            $('body').append("<div id='pdfHtml'>" + "<style>#pdfHtml{background: #fff;}</style>" + html.html() + "</div>");
            if (fileName != "") {
                var pdf = new jsPDF('p', 'pt', 'letter');
                pdf.addHTML($("#pdfHtml")[0], function () {
                    pdf.save(fileName + ".pdf");
                    $("#pdfHtml").remove();
                });
                $("#pdfHtml").css("display", "none");
            }
        }
        function getBase64(id) {
            var svg = (document.getElementById(id).children[0]).innerHTML;
            // svg as image
            var base_image = new Image();
            var svgString = "data:image/svg+xml," + svg;
            base_image.src = svgString;
            var canvas = document.createElement('canvas');
            canvas.width = base_image.width;
            canvas.height = base_image.height;
            var context = canvas.getContext('2d').drawImage(base_image, 0, 0, canvas.width, canvas.height);
            var base64img = canvas.toDataURL("image/png").split("data:image/png;base64,")[1];
            return base64img;
        }
        var init = function () {
            if (localStorage.getItem("roleId") != "3")
                vm.GetSubscriptionPlans();
        }
        vm.GetSubscriptionPlans = function () {
            mainService.getSubscriptionPlans().then(function (response) {
                if (response.data.Success) {
                    vm.Plans = response.data.MultipleData.Plans;
                    vm.TermLengths = response.data.MultipleData.TermLength;
                    vm.TotalUserCreditAmount = response.data.MultipleData.UserCreditAmount;
                    vm.Discount = 0;
                }
                else {
                    toastr.warning("Error");
                }
            })
        };
        vm.getTermLengthDisplayName = function (termlength) {
            if (termlength.intValue == 1) {
                return "MONTHLY(per user)"
            }
            else if (termlength.intValue == 2) {
                return "YEARLY(per user)"
            }
        }
        vm.changeTermLength = function (termLength, total) {
            vm.PaymentFees = 0;
            vm.serviceAmount = 0;
            vm.PlanAmount = total;

            vm.TotalAmount = vm.PlanAmount - vm.Discount;
            vm.serviceAmount = (vm.PlanAmount * 20) / 100;
            vm.TotalAmount = vm.PlanAmount + vm.InfrastructureCost + vm.serviceAmount;
            vm.TermLengthStringValue = termLength.DisplayName + " Plan";
            vm.SelectedTermLength = "" + termLength.intValue;

            if (vm.TotalUserCreditAmount != 0) {
                if (vm.TotalUserCreditAmount > vm.TotalAmount) {
                    vm.UserCreditAmount = vm.TotalAmount;
                    vm.TotalAmount = 0;
                }
                else {
                    vm.UserCreditAmount = vm.TotalUserCreditAmount;
                    vm.TotalAmount = vm.TotalAmount - vm.UserCreditAmount;
                }
            }
            else {
                vm.UserCreditAmount = vm.TotalUserCreditAmount;
            }
            if (vm.TotalAmount != 0) {
                vm.PaymentFees = vm.TotalAmount * 0.029 + 0.30;
                
            }
            vm.TotalAmount = vm.TotalAmount + vm.PaymentFees;
        }
        vm.getPromocodeDiscount = function () {
            if (vm.Promocode != "") {
                mainService.getPromocodeDiscount(vm.Promocode).then(function (response) {
                    if (response.data.Success) {
                        toastr.success("Promo Code applied successfully.");
                        vm.Discount = response.data.Data;
                        if (vm.Discount > vm.PlanAmount)
                            vm.Discount = vm.PlanAmount;
                        vm.serviceAmount = 0;
                        vm.TotalAmount = vm.PlanAmount - vm.Discount;
                        vm.serviceAmount = (vm.PlanAmount * 20) / 100;
                        vm.TotalAmount = vm.TotalAmount + vm.InfrastructureCost + vm.serviceAmount;
                        vm.PaymentFees = 0;                        
                        if (vm.TotalUserCreditAmount != 0) {
                            if (vm.TotalUserCreditAmount > vm.TotalAmount) {
                                vm.UserCreditAmount = vm.TotalAmount;
                                vm.TotalAmount = 0;
                            }
                            else {
                                vm.UserCreditAmount = vm.TotalUserCreditAmount;
                                vm.TotalAmount = vm.TotalAmount - vm.UserCreditAmount;
                            }
                        }

                        if (vm.TotalAmount != 0) {
                            vm.PaymentFees = vm.TotalAmount * 0.029 + 0.30;
                        }
                        vm.TotalAmount = vm.TotalAmount + vm.PaymentFees ;
                    }
                    else {
                        toastr.warning("Invalid Promo Code");
                    }
                });
            }
        }
        vm.removePromocode = function () {
            BootstrapDialog.confirm({
                title: 'REMOVE PROMO CODE',
                message: 'Are you sure you want to remove promo code?',
                callback: function (result) {
                    // result will be true if button was click, while it will be false if users close the dialog directly.
                    if (result) {
                        vm.serviceAmount = 0;
                        vm.Discount = 0;
                        vm.TotalAmount = vm.PlanAmount - vm.Discount;
                        vm.serviceAmount = (vm.PlanAmount * 20) / 100;
                        vm.TotalAmount = vm.TotalAmount + vm.InfrastructureCost + vm.serviceAmount;
                        vm.PaymentFees = 0;

                        if (vm.TotalUserCreditAmount != 0) {
                            if (vm.TotalUserCreditAmount > vm.TotalAmount) {
                                vm.UserCreditAmount = vm.TotalAmount;
                                vm.TotalAmount = 0;
                            }
                            else {
                                vm.UserCreditAmount = vm.TotalUserCreditAmount;
                                vm.TotalAmount = vm.TotalAmount - vm.UserCreditAmount;
                            }
                        }
                        if (vm.TotalAmount != 0) {
                            vm.PaymentFees = vm.TotalAmount * 0.029 + 0.30;
                        }
                        vm.TotalAmount = vm.TotalAmount + vm.PaymentFees ;
                        vm.Discount = 0;
                        $scope.$apply();
                    }
                }
            });
        }
        init();
    }
}]);
