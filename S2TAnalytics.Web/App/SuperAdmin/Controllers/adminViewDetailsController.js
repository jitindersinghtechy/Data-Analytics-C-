'use strict'
app.controller('adminViewDetailsController', ['$scope', 'adminViewDetailsService', '$stateParams', '$location', '$timeout', '$filter',
    function ($scope, adminViewDetailsService, $stateParams, $location, $timeout, $filter) {
        var vm = this;
        vm.UserId = $stateParams.subscriberId;
        vm.User = {}
        vm.Plans = {};
        vm.UserRequest = [];
        vm.PaymentDetailList = [];
        vm.TotalAccounts = "";
        vm.TotalReposrtUsers = "";
        vm.TotalDataSources = "";
        vm.PhoneValidator = true;
        vm.TotalRecords = 0;
        vm.TotalRecordsUserRequest = 0;
        vm.StartingRecordNumber = 1;
        vm.StartingRecordNumberUserRequest = 1;
        vm.ToRecordNumber = 0;
        vm.ToRecordNumberUserRequest = 0;
        vm.PageSizes = [5, 10, 50, 100];
        vm.selectedPageSize = vm.PageSizes[0];
        vm.UserRequestType = {};
        vm.UserRequestStatus = {};
        vm.selectedPageSizeUserRequest = vm.PageSizes[0];
        vm.IsUserActive = true;
        var pageRecordModel = {
            PageNumber: 1,
            PageSize: vm.selectedPageSize,
            SortOrder: "asc",
            SortBy: "",
        };
        var pageRecordModelUserRequest = {
            PageNumber: 1,
            PageSize: vm.selectedPageSizeUserRequest,
            SortOrder: "asc",
            SortBy: "",
        };
        $scope.CurrentPage = 1;
        $scope.PageSize = pageRecordModel.PageSize;
        $scope.TotalPages = 0;
        vm.pageCount = 0;

        $scope.CurrentPageUserRequest = 1;
        $scope.PageSizeUserRequest = pageRecordModelUserRequest.PageSize;
        $scope.TotalPagesUserRequest = 0;
        vm.pageCountUserRequest = 0;
        var init = function () {
            getSubscriberList();
        }
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
            adminViewDetailsService.getInvoiceHistory(vm.UserId, pageRecordModel).then(function (response) {
                vm.PaymentDetailList = response.data.response.MultipleData.PaymentDetail;
                getPageData();
            });
        }
        vm.changePageSize = function () {
            $scope.CurrentPage = 1;
            pageRecordModel.PageSize = vm.selectedPageSize;
            pageRecordModel.PageNumber = 1;
            adminViewDetailsService.getInvoiceHistory(vm.UserId, pageRecordModel).then(function (response) {
                vm.PaymentDetailList = response.data.response.MultipleData.PaymentDetail;
            });
            getPageData();
            $scope.TotalPages = parseInt(vm.TotalRecords / pageRecordModel.PageSize);
            if (vm.TotalRecords % pageRecordModel.PageSize != 0) {
                $scope.TotalPages = $scope.TotalPages + 1;
            }
        }
        vm.changePage = function (value) {
            if (value == -1 && pageRecordModel.PageNumber > 1 || value == 1 && vm.ToRecordNumber < vm.TotalRecords) {
                pageRecordModel.PageNumber = pageRecordModel.PageNumber + (value);
                $scope.CurrentPage = pageRecordModel.PageNumber;
                adminViewDetailsService.getInvoiceHistory(vm.UserId, pageRecordModel).then(function (response) {
                    vm.PaymentDetailList = response.data.response.MultipleData.PaymentDetail;
                    getPageData();
                });
            }
        }
        vm.SetPage = function (n) {
            $scope.CurrentPage = n;
            pageRecordModel.PageNumber = parseInt(n);
            adminViewDetailsService.getInvoiceHistory(vm.UserId, pageRecordModel).then(function (response) {
                vm.PaymentDetailList = response.data.response.MultipleData.PaymentDetail;
                getPageData();
            });
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

        vm.SortPerformersUserRequest = function (sortColumn) {
            vm.sortColumn = sortColumn;
            if (vm.lastSort != sortColumn) {
                vm.orderBy = true;
                vm.lastSort = sortColumn;
            }
            else {
                vm.orderBy = !vm.orderBy;
            }
            vm.sortOrder = vm.orderBy == true ? 'asc' : 'desc';
            pageRecordModelUserRequest.PageNumber = 1;
            pageRecordModelUserRequest.SortBy = sortColumn;
            pageRecordModelUserRequest.SortOrder = vm.sortOrder;
            adminViewDetailsService.getUserRequest(vm.UserId, pageRecordModelUserRequest).then(function (response) {
                vm.UserRequest = response.data.response.MultipleData.UserRequests;
                getPageDataUserRequest();
            });
        }
        vm.changePageSizeUserRequest = function () {
            $scope.CurrentPageUserRequest = 1;
            pageRecordModelUserRequest.PageSize = vm.selectedPageSizeUserRequest;
            pageRecordModelUserRequest.PageNumber = 1;
            adminViewDetailsService.getUserRequest(vm.UserId, pageRecordModelUserRequest).then(function (response) {
                vm.UserRequest = response.data.response.MultipleData.UserRequests;
            });
            getPageDataUserRequest();
            $scope.TotalPagesUserRequest = parseInt(vm.TotalRecords / pageRecordModelUserRequest.PageSize);
            if (vm.TotalRecords % pageRecordModelUserRequest.PageSize != 0) {
                $scope.TotalPagesUserRequest = $scope.TotalPagesUserRequest + 1;
            }
        }
        vm.changePageUserRequest = function (value) {
            if (value == -1 && pageRecordModelUserRequest.PageNumber > 1 || value == 1 && vm.ToRecordNumber < vm.TotalRecords) {
                pageRecordModelUserRequest.PageNumber = pageRecordModelUserRequest.PageNumber + (value);
                $scope.CurrentPageUserRequest = pageRecordModelUserRequest.PageNumber;
                adminViewDetailsService.getUserRequest(vm.UserId, pageRecordModelUserRequest).then(function (response) {
                    vm.UserRequest = response.data.response.MultipleData.UserRequests;
                    getPageDataUserRequest();
                });
            }
        }
        vm.SetPageUserRequest = function (n) {
            $scope.CurrentPageUserRequest = n;
            pageRecordModelUserRequest.PageNumber = parseInt(n);
            adminViewDetailsService.getUserRequest(vm.UserId, pageRecordModelUserRequest).then(function (response) {
                vm.UserRequest = response.data.response.MultipleData.UserRequests;
                getPageDataUserRequest();
            });
        }
        function getPageDataUserRequest() {
            vm.StartingRecordNumberUserRequest = ((pageRecordModelUserRequest.PageNumber - 1) * pageRecordModelUserRequest.PageSize) + 1;
            var toRecordNumber = pageRecordModelUserRequest.PageNumber * pageRecordModelUserRequest.PageSize;
            if (toRecordNumber < vm.TotalRecordsUserRequest) {
                vm.ToRecordNumberUserRequest = toRecordNumber;
            }
            else {
                vm.ToRecordNumberUserRequest = vm.TotalRecordsUserRequest;
            }
        }


        var getSubscriberList = function () {
            adminViewDetailsService.getSubscriberList(vm.UserId).then(function (response) {
                if (response.data.response.Success) {
                    vm.User = response.data.response.MultipleData.ContactInfo;
                    vm.IsUserActive = "" + vm.User.IsActive;
                    vm.Plans = response.data.response.MultipleData.Plans;
                    vm.SubscribersPlans = response.data.response.MultipleData.SubscribersPlans;
                    vm.TotalReposrtUsers = response.data.response.MultipleData.ReposrtUsers;
                    vm.TotalAccounts = response.data.response.MultipleData.Accounts;
                    vm.TotalDataSources = response.data.response.MultipleData.DataSources;
                    //$("#phone").intlTelInput("setCountry", vm.User.PhoneCountryCode);
                    getInvoiceHistory();
                    getUserRequest();
                    vm.selectedPlan = vm.Plans.PlanID;
                    //vm.UserRequest = response.data.response.MultipleData.UserRequest;

                    $("#phone1").intlTelInput("setCountry", vm.User.PhoneCountryCode);
                    //   $("#userCountry").countrySelect("selectCountry", vm.User.ISO);
                }
            });
        }
        var getInvoiceHistory = function () {
            adminViewDetailsService.getInvoiceHistory(vm.UserId, pageRecordModel).then(function (response) {
                vm.PaymentDetailList = response.data.response.MultipleData.PaymentDetail;
                vm.TotalRecords = response.data.response.MultipleData.Total;
                getPageData();
                $scope.TotalPages = parseInt(vm.TotalRecords / pageRecordModel.PageSize);
                if (vm.TotalRecords % pageRecordModel.PageSize != 0) {
                    $scope.TotalPages = $scope.TotalPages + 1;
                }
            });
        }
        vm.sendOverdueReminder = function () {
            adminViewDetailsService.sendOverdueReminder(vm.User).then(function (response) {
                if (response.data.response.Success) {
                    toastr.success(response.data.response.Message);
                }
            });
        }
        vm.checkAll = function () {

            angular.forEach(vm.PaymentDetailList, function (val, ind) {
                val.checked = vm.checkedAll ? true : false;

            })
        }
        vm.singleCheck = function () {

            vm.selectedInvoice.Invoice;
            vm.checkedAll = vm.checkedAll ? false : undefined;
        }
        vm.DownloadInvoice = function () {
            if (vm.selectedInvoice != undefined) {
                var InvoiceIds = $.map(vm.selectedInvoice.Invoice, function (item, key) {
                    if (item)
                        return key;
                });
                if (InvoiceIds.length > 0)
                    adminViewDetailsService.DownloadInvoice(vm.UserId, InvoiceIds).then(function (response) {
                        if (response.data.fileName.length != 0) {
                            angular.forEach(response.data.fileName, function (file, index) {
                                $('#downloadPDF').attr("href", file);
                                $timeout(function () {
                                    $('#downloadPDF').get(0).click()
                                }, 1000);
                            })
                        }
                    });
            }
        }
        var getUserRequest = function () {
            adminViewDetailsService.getUserRequest(vm.UserId, pageRecordModelUserRequest).then(function (response) {
                vm.UserRequest = response.data.response.MultipleData.UserRequests;

                vm.UserRequestType = response.data.response.MultipleData.RequestType;
                vm.UserRequestStatus = response.data.response.MultipleData.RequestStatus;

                vm.TotalRecordsUserRequest = response.data.response.MultipleData.Total;
                getPageDataUserRequest();
                $scope.TotalPagesUserRequest = parseInt(vm.TotalRecordsUserRequest / pageRecordModelUserRequest.PageSize);
                if (vm.TotalRecordsUserRequest % pageRecordModelUserRequest.PageSize != 0) {
                    $scope.TotalPagesUserRequest = $scope.TotalPages.UserRequest + 1;
                }
            });
        }
        vm.getUserRequestTypeById = function (Id) {
            var requestType = "";
            var requestType = $filter('filter')(vm.UserRequestType, { intValue: Id }, true);
            return requestType[0].DisplayName;
        }
        vm.getUserRequestStatusById = function (Id) {
            var requestStatus = "";
            var requestStatus = $filter('filter')(vm.UserRequestStatus, { intValue: Id }, true);
            return requestStatus[0].DisplayName;
        }
        vm.boolToStrUserActive = function (arg) {
            return arg ? 'ACTIVE' : 'INACTIVE'
        };
        vm.updateUserActivation = function () {
            adminViewDetailsService.updateUserActivation(vm.User).then(function (response) {
                if (response.data.response.Success) {
                    toastr.success(response.data.response.Message);
                }
            });
        }
        vm.updateSubscriberContact = function () {
            if (vm.PhoneValidator == true) {
                adminViewDetailsService.updateSubscriberContactInfo(vm.User).then(function (response) {
                    if (response.data.response.Success) {
                        vm.User = response.data.response.MultipleData.ContactInfo;
                        toastr.success("Successfully Updated.");
                    }
                });
            }
        }
        vm.updateSubscriberSummary = function () {
            adminViewDetailsService.updateSubscriberSummary(vm.User, vm.selectedPlan, vm.MonthlyRate).then(function (response) {
                if (response.data.response.Success) {
                    vm.User = response.data.response.MultipleData.ContactInfo;
                    toastr.success("Successfully Updated.");
                }
            });
        }
        vm.updatePlans = function () {
            if (vm.PhoneValidator == true) {
                adminViewDetailsService.updatePlans(vm.UserId, vm.selectedPlan).then(function (response) {
                    if (response.data.response.Success) {
                        toastr.success("Successfully Updated.");
                    }
                });
            }
        }
        init();
    }])

app.directive('userPhone', function () {
    return {
        link: function (scope, element, attrs) {
            element.intlTelInput({
                utilsScript: "/Scripts/utils.js",
                nationalMode: true,
                customPlaceholder: function (selectedCountryPlaceholder, selectedCountryData) {
                    return "e.g. " + selectedCountryPlaceholder;
                }
            });
            element.on("countrychange", function (e, countryData) {
                scope.vm.User.PhoneCountryCode = countryData.iso2;
                scope.vm.User.CountryCode = countryData.dialCode;
            });
            element.blur(function () {
                if ($.trim(element.val()) == "") {
                    scope.vm.User.CountryCode = "";
                    scope.vm.User.PhoneCountryCode = "";
                    scope.vm.PhoneValidator = true;
                }
                if ($.trim(element.val())) {
                    if (element.intlTelInput("isValidNumber")) {
                        scope.vm.PhoneValidator = true;
                    }
                    else {
                        scope.vm.PhoneValidator = false;
                        toastr.warning("Invalid Phone Number");
                    }
                }
            });
        }
    }
})


