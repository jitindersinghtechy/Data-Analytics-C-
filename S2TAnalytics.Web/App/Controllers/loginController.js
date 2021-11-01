'use strict';
app.controller('loginController', ['$scope', '$filter', 'LoginService', 'signUpService', 'setPasswordService', 'contactService', '$location', '$timeout',
    function ($scope, $filter, loginService, signUpService, setPasswordService, contactService, $location, $timeout) {
        var authenticateVM = this;
        var params = $location.path().split('/');
        authenticateVM.loginScreen = localStorage.getItem("isEmailConfirmed") == "true" ? 4 : localStorage.getItem("isSetPassword") == "true" ? 6 : 1;
        localStorage.removeItem("isEmailConfirmed");
        localStorage.removeItem("isSetPassword");
        localStorage.removeItem("planPermissionIds");

        authenticateVM.ForgotEmailId = localStorage.getItem("SetPasswordEmail");
        authenticateVM.User = {
            UserName: "",
            Password: ""
        };
        authenticateVM.load = function () {
            $timeout(function () {
                $('.page-loder').addClass("blur");
            }, 2000);
            $timeout(function () {
                $('.page-loder').fadeOut(500);
            }, 2000);
        }
        authenticateVM.login = function () {
            authenticateVM.isLoginDisabled = true;
            $('.login').append("<div class='preloader show-loder'><img src='../Images/loading-circle.svg' class='img' /></div>");
            loginService.login(authenticateVM.User.UserName, authenticateVM.User.password).then(function (response) {
                authenticateVM.isLoginDisabled = false;
                if (response.error != 'invalid_grant') {
                    localStorage.setItem("fullName", response.fullName)
                    var arr = [];
                    localStorage.setItem('CompareData', JSON.stringify(arr));

                    //var permissions = localStorage.getItem("planPermissionIds");
                    var isDashboard = false;
                    if (response.PlanPermissionIds != undefined && response.PlanPermissionIds != null && response.PlanPermissionIds != "")
                        isDashboard = $filter('filter')(response.PlanPermissionIds.split(","), enumPermissionPlans.Dashboard, true).length > 0;
                    if (response.IsExpired == "True") {
                        localStorage.setItem('IsExpire', response.IsExpired);
                        location.href = "/#/Account";
                    }

                    else if (isDashboard)
                        location.href = "/#/Dashboard";
                    else if (!isDashboard)
                        location.href = "/#/Performers";


                } else {
                    toastr.warning(response.error_description);
                }
                $(".preloader").hide();
            });


        }



        authenticateVM.CreateUser = {
            FirstName: "",
            LastName: "",
            RoleID: "",
            PlanID: "",
            Password: "",
            ConfirmPassword: "",
            EmailID: "",
            PhoneNumber: "",
            PhoneCountryCode: ""
        };
        authenticateVM.SignUpPhoneCheck = true;

        authenticateVM.createNewUser = function () {

            if (authenticateVM.SignUpPhoneCheck == true) {
                if (grecaptcha.getResponse(w1) == "")
                    toastr.warning("Please Response CAPCHA First");
                else {
                    authenticateVM.isSignupDisabled = true;
                    $('.signUp').append("<div class='preloader show-loder'><img src='../Images/loading-circle.svg' class='img' /></div>")
                    signUpService.createNewUser(authenticateVM.CreateUser).then(function (response) {
                        if (response.Success) {
                            grecaptcha.reset(w1);
                            //window.location.href = "/";               
                            authenticateVM.loginScreen = 3;
                        } else {
                            toastr.warning(response.Message);
                        }
                        authenticateVM.isSignupDisabled = false;
                        $(".preloader").hide();
                    });
                }
            }
            else {
                toastr.warning("Invalid Phone Number");
            }
        }

        authenticateVM.createNewUserResponsive = function () {
            if (authenticateVM.SignUpPhoneCheck == true) {
                if (grecaptcha.getResponse(w3) == "")
                    toastr.warning("Please Response CAPCHA First");
                else {
                    authenticateVM.isSignupDisabled = true;
                    $('.signUp').append("<div class='preloader show-loder'><img src='../Images/loading-circle.svg' class='img' /></div>")
                    signUpService.createNewUser(authenticateVM.CreateUser).then(function (response) {
                        if (response.Success) {
                            grecaptcha.reset(w1);
                            //window.location.href = "/";               
                            authenticateVM.loginScreen = 3;
                        } else {
                            toastr.warning(response.Message);
                        }
                        authenticateVM.isSignupDisabled = false;
                        $(".preloader").hide();
                    });
                }
            }
            else {
                toastr.warning("Invalid Phone Number");
            }
        }



        authenticateVM.forgotPassword = function () {
            authenticateVM.isForgotDisabled = true;
            $('.forgotPassword').append("<div class='preloader show-loder'><img src='../Images/loading-circle.svg' class='img' /></div>")
            loginService.forgotPassword(authenticateVM.ForgotEmail).then(function (response) {
                if (response.Success) {
                    authenticateVM.loginScreen = 1;
                    toastr.success(response.Message);
                } else {
                    toastr.warning(response.Message);
                }
                authenticateVM.isForgotDisabled = false;
                $(".preloader").hide();
            });
        }

        authenticateVM.getData = function () {
            authenticateVM.Queries = [];
            contactService.getData().then(function (response) {
                if (response.data.Success) {
                    angular.forEach(response.data.Data, function (query, index) {
                        authenticateVM.Queries.push({ id: query.stringValue, label: query.DisplayName });
                    });
                }
            });
        }

        authenticateVM.ContactUs = function () {
            if (grecaptcha.getResponse(w2) == "")
                toastr.warning("Please Response CAPCHA First");
            else {

                authenticateVM.isContactDisabled = true;
                $('.contact').append("<div class='preloader show-loder'><img src='../Images/loading-circle.svg' class='img' /></div>")
                contactService.ContactUs(authenticateVM.ContactPerson).then(function (response) {
                    if (response.data.Success) {
                        grecaptcha.reset(w2);
                        toastr.success(response.data.Message);
                    } else {
                        toastr.warning(response.data.Message);
                    }
                    authenticateVM.isContactDisabled = false;
                    $(".preloader").hide();
                });
            }
        }

        authenticateVM.setPassword = function () {
            authenticateVM.isSetPasswordDisabled = true;
            $('.setPassword').append("<div class='preloader show-loder'><img src='../Images/loading-circle.svg' class='img' /></div>")
            setPasswordService.SetPassword(authenticateVM.ForgotEmailId, authenticateVM.Password).then(function (response) {
                if (response.Success) {
                    toastr.success(response.Message);
                    authenticateVM.loginScreen = 1;
                }
                else {
                    toastr.error(response.Message);
                }
                authenticateVM.isSetPasswordDisabled = false;
                $(".preloader").hide();
            });
        }

    }]);

app.directive('signupPhone', function () {
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
                scope.authenticateVM.CreateUser.CountryCode = countryData.dialCode;
                scope.authenticateVM.CreateUser.PhoneCountryCode = countryData.iso2;
            });
            element.blur(function () {
                if ($.trim(element.val()) == "") {
                    scope.authenticateVM.CreateUser.CountryCode = "";
                    scope.authenticateVM.CreateUser.PhoneCountryCode = "";
                    scope.authenticateVM.SignUpPhoneCheck = true;
                    scope.authenticateVM.CreateUser.PhoneNumber = "";
                }
                if ($.trim(element.val())) {
                    if (element.intlTelInput("isValidNumber")) {
                        scope.authenticateVM.SignUpPhoneCheck = true;
                        scope.authenticateVM.CreateUser.PhoneNumber = element.val();

                    } else {
                        scope.authenticateVM.SignUpPhoneCheck = false;
                        toastr.warning("Invalid Phone Number");
                    }
                }
            });
        }
    }
})


app.directive('contactPhone', function () {
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
                scope.authenticateVM.ContactPerson.CountryCode = countryData.dialCode;
            });
            element.blur(function () {
                if ($.trim(element.val()) == "") {
                    scope.authenticateVM.User.CountryCode = "";
                    scope.authenticateVM.SignUpPhoneCheck = true;
                    scope.authenticateVM.ContactPerson.PhoneNumber = "";
                }
                if ($.trim(element.val())) {
                    if (element.intlTelInput("isValidNumber")) {
                        scope.authenticateVM.ContactPerson.PhoneNumber = element.val();
                        scope.authenticateVM.SignUpPhoneCheck = true;
                    } else {
                        scope.authenticateVM.SignUpPhoneCheck = false;
                        toastr.warning("Invalid Phone Number");
                    }
                }
            });
        }
    }
})





