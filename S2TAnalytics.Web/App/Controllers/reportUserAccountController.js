'use strict'
app.controller('reportUserAccountController', ['$scope', 'userAccountService', function ($scope, userAccountService) {
    var vm = this; 
    vm.User = {};

    var getDefaultData = function () {
        userAccountService.getDefaultData().then(function (response) {
            if (response.data.Success) {
             
                vm.emailNotificationSettingsEnum = response.data.MultipleData.emailNotificationSettingsEnum;
                angular.forEach(response.data.MultipleData.emailNotificationSettings, function (item, index) {
                    angular.forEach(vm.emailNotificationSettingsEnum, function (item1, index1) {
                        if (item.NotificationSettingsId == item1.intValue) {
                            item1.HasAccess = true;
                        }
                    });
                });

                vm.User = response.data.MultipleData.user;
                
            }
        });
    }

   
   
    getDefaultData();

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
            if (response.data.Success) {

                toastr.success(response.data.Message);
                $(".resetPassword").click();
                $("#changePassword").modal('hide');
            }
            else {
                toastr.warning(response.data.Message);
            }
            vm.IsSubmitDisabled = false;
            $(".preloader").hide();
        });
    }

    vm.changeSettings = function (settingsId, hasAccess) {
        userAccountService.changeSettings(settingsId, hasAccess).then(function (response) {
            if (response.data.Success) {
            }
        });
    }
    
}]);

//app.directive('emailNotificationSettingsId', function () {
//    return {
//        link: function (scope, element, attrs) {
//            if (attrs.emailNotificationSettingsId == 'true') {
//                element.bootstrapToggle('on');
//            } else {
//                element.bootstrapToggle('off');
//            }           

//            element.change(function (e) {
//                var hasAccess = $(e.currentTarget).is(":checked");
//                scope.userAccountVm.changeSettings(scope.emailNotification.intValue, hasAccess);
//            });
//        }
//    };
//});

app.directive('userAccountPhone', function () {
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

                scope.userAccountVm.User.CountryCode = countryData.dialCode;

            });
            element.blur(function () {
                if ($.trim(element.val()) == "") {
                    scope.userAccountVm.User.CountryCode = "";
                    scope.userAccountVm.editPhone = false;
                    scope.userAccountVm.changeUserDetails();
                }
                if ($.trim(element.val())) {
                    if (element.intlTelInput("isValidNumber")) {
                        scope.userAccountVm.editPhone = false;
                        scope.userAccountVm.changeUserDetails();
                    } else {
                        toastr.warning("Invalid Phone Number");
                    }
                }
            });
        }
    }
})


app.directive('datePicker2', function () {
    return {
        link: function (scope, element, attrs) {
            element.datepicker({
                autoclose: true
            });
            element.change(function () {
                scope.userAccountVm.changeUserDetails();
            })
        }
    }
})
