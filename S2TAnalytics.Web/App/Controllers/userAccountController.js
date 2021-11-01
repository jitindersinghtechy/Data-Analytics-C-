'use strict'
app.controller('userAccountController', ['$scope', 'userAccountService', '$timeout', function ($scope, userAccountService, $timeout) {
    var vm = this;
    vm.selectedBillingAddressId = "";
    vm.savePanel = false;
    vm.ShowBilling = true;
    vm.ShowInvoice = false
    vm.addPaymentMethod = false;
    vm.Card = {};
    vm.userBillingInfo = {};
    vm.SubscriptionPlan = {};
    vm.User = {};
    vm.ActiveCard = "";
    vm.Plans = [];
    vm.Plans1 = [{
        PlanID: 1,
        Widgets: ["Dashboard with drilldown (Trader statistics)", "View Details account details", "Compare account performance", "Pin account"]
    },
    {
        PlanID: 2,
        Widgets: ["Dashboard with drilldown (Trader statistics)", "View Details account details", "Compare account performance", "Pin account"]
    },
    {
        PlanID: 3,
        Widgets: ["Dashboard with drilldown (Trader statistics)", "View Details account details", "Compare account performance", "Pin account", "Exclude accounts", "Add report users", "Embed dashboard widgets", "API Data for the dashboard widgets", "Export dashboard, performers", "Account statement"]
    },
    {
        PlanID: 4,
        Widgets: ["Dashboard with drilldown (Trader statistics)", "View Details account details", "Compare account performance", "Pin account", "Exclude accounts", "Add report users", "Embed dashboard widgets", "API Data for the dashboard widgets", "Export dashboard, performers", "Account statement"]
    }]


    vm.GetSubscriptionPlans = function () {
        userAccountService.getSubscriptionPlans().then(function (response) {
            if (response.data.Success) {
                vm.Plans = response.data.MultipleData.Plans;
                getDefaultData();
            }

        })
    };
    var init = function () {
        vm.GetSubscriptionPlans();
    }

    vm.saveCard = function () {
        vm.IsSaveDisabled = true;
        $(".saveCard").append("<div class='preloader show-loder'><img src='../Images/loading-circle.svg' class='img' /></div>");
        var expiryDate = vm.Card.ExpiryDate.split("/");
        vm.Card.ExpirationMonth = expiryDate[0];
        vm.Card.ExpirationYear = expiryDate[1];
        userAccountService.saveCard(vm.Card).then(function (response) {
            vm.IsSaveDisabled = false;
            $(".preloader").hide();
            if (response.data.Success) {
                vm.Card = {};
                toastr.success(response.data.Message);
                vm.UserCards = response.data.Data;
                $("#_reset").click();
            } else {
                toastr.warning(response.data.Message);
            }

        });
    }

    $scope.$on('subscribedPlan', function (event, args) {

        vm.SubscriptionPlan = args.data;
        angular.forEach(vm.Plans, function (item, index) {
            if (item.PlanId == args.data.PlanID) {
                vm.SubscriptionPlan.Widgets = item.Widgets;
            }
        });

    });


    $scope.$on('userInvoiceHistory', function (event, args) {
        getDefaultData();
    });

    var getDefaultData = function () {
        userAccountService.getDefaultData().then(function (response) {
            if (response.data.Success) {
                vm.UserCards = response.data.MultipleData.userCards;
                vm.PaymentTypes = response.data.MultipleData.paymentTypes;
                vm.SubscriptionPlan = response.data.MultipleData.userPlan;
                vm.userInvoiceHistory = response.data.MultipleData.userInvoiceHistory;
                vm.emailNotificationSettingsEnum = response.data.MultipleData.emailNotificationSettingsEnum;
                //   vm.emailNotificationSettings = response.data.MultipleData.emailNotificationSettings;
                angular.forEach(response.data.MultipleData.emailNotificationSettings, function (item, index) {
                    angular.forEach(vm.emailNotificationSettingsEnum, function (item1, index1) {
                        if (item.NotificationSettingsId == item1.intValue) {
                            item1.HasAccess = true;
                        }
                    });
                });

                angular.forEach(vm.Plans, function (item, index) {
                    if (item.PlanId == vm.SubscriptionPlan.PlanID) {
                        vm.SubscriptionPlan.Widgets = item.Widgets;
                        $scope.$parent.mainVM.PrevPlan = vm.SubscriptionPlan.Name;
                    }
                });

                vm.User = response.data.MultipleData.user;
                setActiveCard();

                $("#phone").intlTelInput("setCountry", vm.User.PhoneCountryCode);
                $("#userCountry").countrySelect("selectCountry", vm.User.ISO);
                //  $("#phone").intlTelInput("setCountry", vm.User.ISO);
            }
        });
    }

    var setActiveCard = function () {

        angular.forEach(vm.UserCards, function (item, index) {
            if (item.IsActive) {
                vm.ActiveCard = item.CardNumber;
            }
        });
        vm.ActiveCard = vm.ActiveCard == "" ? "No Card is Active" : vm.ActiveCard;
    }
    //var getUserCards = function () {
    //    userAccountService.getUserCards().then(function (response) {
    //        vm.UserCards = response.data.Data;
    //    });
    //}



    vm.deleteCards = function (index) {
        vm.SelectedCards = [];
        angular.forEach(vm.UserCards, function (userCard, index) {
            if (userCard.IsChecked) {
                if (userCard.CardNumber == vm.ActiveCard)
                    toastr.warning("Active card can't be deleted.");
                else
                    vm.SelectedCards.push(userCard.Id);
            }
        })
        if (vm.SelectedCards.length > 0) {
            BootstrapDialog.confirm({
                title: 'DELETE CARD(S)',
                message: 'Are you sure you want to delete selected Card(s)?',

                callback: function (result) {
                    // result will be true if button was click, while it will be false if users close the dialog directly.
                    if (result) {
                        userAccountService.deleteCards(vm.SelectedCards).then(function (response) {
                            if (response.data.Success) {
                                vm.UserCards = response.data.Data;
                                toastr.success(response.data.Message);
                                setActiveCard();
                            } else {
                                toastr.warning(response.data.Message);
                            }
                        });
                    }
                }
            });
        }
    }

    vm.activateCard = function (cardNumber, cardId, isActive) {
        vm.ActiveCard = cardNumber;
        userAccountService.activateCard(cardId, isActive).then(function (response) {
            if (response.data.Success) {

                vm.UserCards = response.data.Data;
            }
        });
    }

    vm.saveBillingAddress = function () {
        vm.IsSaveBillingDisabled = true;
        $(".saveBillingAddress").append("<div class='preloader show-loder'><img src='../Images/loading-circle.svg' class='img' /></div>");
        var countryData = $("#country").countrySelect("getSelectedCountryData");
        vm.userBillingInfo.Country = countryData.name;
        vm.userBillingInfo.ISO = countryData.iso2;
        var countrycode = vm.userBillingInfo.CountryCode;
        userAccountService.saveBillingAddress(vm.userBillingInfo).then(function (response) {
            vm.IsSaveBillingDisabled = false;
            $(".preloader").hide();
            if (response.data.Success) {
                vm.userBillingInfo = {};
                vm.UserCards = response.data.Data;
                getUserBillingAddresses();
                vm.savePanel = false;
                $(".resetBilling").click();
                vm.userBillingInfo.CountryCode = countrycode;
            }
        });
    }

    var getUserBillingAddresses = function () {
        userAccountService.getUserBillingAddresses().then(function (response) {
            if (response.data.Success) {

                vm.UserBillingAddresses = response.data.Data;
            }
        });
    }

    getUserBillingAddresses();


    vm.activateBillingAddress = function (addressId, isActive) {
        userAccountService.activateBillingAddress(addressId, isActive).then(function (response) {
            if (response.data.Success) {
                vm.emailNotificationSettings = response.data.Data;
                toastr.success(response.data.Message);
            }
        });
    }
    vm.checkBillingAddress = function (addressId) {
        angular.forEach(vm.UserBillingAddresses, function (billingAddress, index) {
            if (billingAddress.Id == addressId) {
                billingAddress.IsChecked ? billingAddress.IsChecked = true : billingAddress.IsChecked = false;
                //vm.selectedBillingAddressId = billingAddress.Id;
            }
            else {
                billingAddress.IsChecked = false;
            }
        })
    }
    vm.getUserBillingAddressById = function () {
        angular.forEach(vm.UserBillingAddresses, function (billingAddress, index) {
            if (billingAddress.IsChecked) {
                vm.selectedBillingAddressId = billingAddress.Id;
            }
        })
        if (vm.selectedBillingAddressId != "") {
            userAccountService.getUserBillingAddressById(vm.selectedBillingAddressId).then(function (response) {
                if (response.data.Success) {
                    vm.userBillingInfo = response.data.Data;
                    $("#country").countrySelect("selectCountry", vm.userBillingInfo.ISO);

                    vm.savePanel = true;
                    vm.selectedBillingAddressId = "";
                }
            });
        }
    }

    vm.changeSettings = function (settingsId, hasAccess) {
        userAccountService.changeSettings(settingsId, hasAccess).then(function (response) {
            if (response.data.Success) {

                //vm.UserBillingAddresses = response.data.Data;
            }
        });
    }
    function validateEmail(email) {
        var re = /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        return re.test(email);
    }
    vm.changeUserDetails = function () {
        if (vm.User.Name == "") {
            toastr.warning("Name can't be empty.");
            return;
        }
        if (vm.User.EmailID == "") {

            toastr.warning("Email can't be empty.");
            return;
        }
        if (!validateEmail(vm.User.EmailID)) {
            toastr.warning("Email not in correct format.");
            return;
        }

        var userCountryData = $("#userCountry").countrySelect("getSelectedCountryData");
        vm.User.Country = userCountryData.name;
        vm.User.ISO = userCountryData.iso2;
        var names = vm.User.Name.split(" ");
        vm.User.FirstName = names[0];
        vm.User.LastName = names[1];

        userAccountService.changeUserDetails(vm.User).then(function (response) {
            if (response.data.Success) {
                vm.User = response.data.Data;
                $("#userCountry").countrySelect("selectCountry", vm.User.ISO);
                toastr.success(response.data.Message);
                vm.editName = false;
                vm.editEmail = false;
            }
            else {
                toastr.warning(response.data.Message);
            }

        });

    }

    vm.changePassword = function () {
        vm.IsSubmitDisabled = true;
        $(".change-password").append("<div class='preloader show-loder'><img src='../Images/loading-circle.svg' class='img' /></div>");
        userAccountService.changePassword(vm.User.EmailID, vm.OldPassword, vm.NewPassword).then(function (response) {
            vm.IsSubmitDisabled = false;
            $(".preloader").hide();
            if (response.data.Success) {

                toastr.success(response.data.Message);
                $("#changePassword").modal('hide');
                $(".resetPassword").click();
            }
            else {
                toastr.warning(response.data.Message);
            }

        });
    }

    vm.generateInvoice = function () {
        if (vm.selectedDate != undefined && vm.selectedDate != "") {
            var invoiceDate = vm.selectedDate.split("/");
            vm.invoiceMonth = invoiceDate[0];
            vm.invoiceYear = invoiceDate[1];
            userAccountService.generateInvoice(vm.invoiceMonth, vm.invoiceYear).then(function (response) {
                if (response.data.fileName.length != 0) {
                    angular.forEach(response.data.fileName, function (file, index) {

                        $('#downloadPDF').attr("href", file);
                        $timeout(
                            $('#downloadPDF').get(0).click()
                            , 10000);

                    })
                    //toastr.success(response.data.Message);
                }
                else {
                    toastr.warning("No Invoice to download.");
                }
            });
        }
        else {
            toastr.warning("Please choose date to generate invoice.");
        }
    }

    vm.emailInvoice = function () {
        if (vm.selectedDate != undefined && vm.selectedDate != "") {
            var invoiceDate = vm.selectedDate.split("/");
            vm.invoiceMonth = invoiceDate[0];
            vm.invoiceYear = invoiceDate[1];
            userAccountService.generateInvoice(vm.invoiceMonth, vm.invoiceYear).then(function (response) {
                if (response.data.fileName.length != 0) {
                    userAccountService.emailInvoice(response.data.fileName, vm.invoiceMonth, vm.invoiceYear).then(function (response) {
                        if (response.data.Success) {
                            toastr.success(response.data.Message);
                        }
                        else {
                            toastr.warning(response.data.Message);
                        }
                    });
                }
                else {
                    toastr.warning("No Invoice to send.");
                }
            });
        }
        else {
            toastr.warning("Please choose date to generate invoice.");
        }
    }


    init();


}]);

app.directive('cardStatus', function () {
    return {
        link: function (scope, element, attrs) {
            if (attrs.cardStatus == "true") {
                element.bootstrapToggle('on');
            }
            else {
                element.bootstrapToggle('off');
            }
            element.change(function (e) {

                var isActive = $(e.currentTarget).is(":checked");
                if (isActive) {
                    scope.accountVm.activateCard(isActive ? scope.userCard.CardNumber : "No Card is Active", scope.userCard.Id, isActive)
                    toastr.success("Successfully Updated.");
                }
                else {
                    element.bootstrapToggle('on');
                    toastr.success("Successfully Updated.");
                }
            });
        }
    };
});

//app.directive('monthPicker', function () {
//    return {
//        link: function (scope, element, attrs) {
//            if (attrs.id == "invoiceMonth") {
//                element.MonthPicker({
//                    Button: false, OnAfterChooseMonth: function () {
//                        scope.accountVm.selectedDate = $(this).val();
//                    }
//                });
//            }
//            else {
//                element.MonthPicker({
//                    Button: false, OnAfterChooseMonth: function () {
//                        scope.accountVm.Card.ExpiryDate = $(this).val();
//                    }
//                });
//            }
//        }
//    };
//});

app.directive('monthPicker', function () {
    return {
        link: function (scope, element, attrs) {

            var date = new Date();
            var month = date.getMonth();
            var today = (month < 10 ? '0' : '') + (month + 1) + "/" + date.getFullYear();

            $('#invoiceMonth').val(today);
            if (attrs.id == "invoiceMonth") {
                element.datepicker({
                    autoclose: true,
                    minViewMode: 1,
                    format: 'mm/yyyy',
                    setDate: new Date()

                }).on('changeDate', function (selected) {
                    scope.accountVm.selectedDate = $(this).val();
                });
                attrs.value = today;
            }
            else {
                element.datepicker({
                    autoclose: true,
                    minViewMode: 1,
                    format: 'mm/yyyy'
                }).on('changeDate', function (selected) {

                    scope.accountVm.Card.ExpiryDate = $(this).val();
                });
            }
        }
    };
});

app.directive('emailNotificationSettingsId', function () {
    return {
        link: function (scope, element, attrs) {
            if (attrs.emailNotificationSettingsId == 'true') {
                element.bootstrapToggle('on');
            } else {
                element.bootstrapToggle('off');
            }
            //if (scope.accountVm.emailNotificationSettings == null) {
            //    element.bootstrapToggle('off');
            //}
            //else {
            //    angular.forEach(scope.accountVm.emailNotificationSettings, function (emailSetting, index) {
            //        if (emailSetting.NotificationSettingsId == attrs.emailNotificationSettingsId) {
            //            if (emailSetting.HasAccess) {
            //                element.bootstrapToggle('on');
            //            } else {
            //                element.bootstrapToggle('off');
            //            }
            //        }
            //    });
            //}
            //}
            element.change(function (e) {
                var hasAccess = $(e.currentTarget).is(":checked");
                scope.accountVm != undefined ? scope.accountVm.changeSettings(scope.emailNotification.intValue, hasAccess) : scope.userAccountVm.changeSettings(scope.emailNotification.intValue, hasAccess);
                toastr.success("Successfully Updated.");
            });

        }
    };
});

app.directive('billingAddressStatus', function () {
    return {
        link: function (scope, element, attrs) {
            if (attrs.billingAddressStatus == "true") {
                element.bootstrapToggle('on');
            }
            else {
                element.bootstrapToggle('off');
            }
            element.change(function (e) {
                var isActive = $(e.currentTarget).is(":checked");
                scope.accountVm.activateBillingAddress(scope.userBillingAddress.Id, isActive);
            });
        }
    };
});

app.directive('setFocus', function () {

    return {
        link: function (scope, element, attrs) {
            element.bind('click', function () {
                document.querySelector('#' + attrs.setFocus).focus();
            })
        }
    }
})




app.directive('datePicker', function () {

    return {
        link: function (scope, element, attrs) {
            element.datepicker({
                autoclose: true
            });
            element.change(function () {
                scope.accountVm.changeUserDetails();
            })
        }
    }
})


app.directive('countryPhone', function () {
    return {
        link: function (scope, element, attrs) {
            element.intlTelInput({
                utilsScript: "/Scripts/utils.js",
                nationalMode: true,
                customPlaceholder: function (selectedCountryPlaceholder, selectedCountryData) {
                    return "e.g. " + selectedCountryPlaceholder;
                },

            });

            element.on("countrychange", function (e, countryData) {

                scope.accountVm.User.CountryCode = countryData.dialCode;
                scope.accountVm.User.PhoneCountryCode = countryData.iso2;

            });
            element.blur(function () {
                if ($.trim(element.val()) == "") {
                    scope.accountVm.User.CountryCode = "";
                    scope.accountVm.editPhone = false;
                    scope.accountVm.changeUserDetails();
                }
                if ($.trim(element.val())) {
                    if (element.intlTelInput("isValidNumber")) {
                        scope.accountVm.editPhone = false;
                        scope.accountVm.changeUserDetails();
                    } else {
                        toastr.warning("Invalid Phone Number");
                    }
                }
            });
        }
    }
})




app.directive('billingPhone', function () {
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
                scope.accountVm.userBillingInfo.CountryCode = countryData.dialCode;
            });
            element.blur(function () {
                if ($.trim(element.val())) {
                    if (element.intlTelInput("isValidNumber")) {
                    } else {
                        element.val("");
                        scope.accountVm.userBillingInfo.PhoneNumber = "";
                        toastr.warning("Invalid Phone Number");
                    }
                }
            });
        }
    }
})


