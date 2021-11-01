var app = angular.module('S2TAnalyticsApp', ['validation', 'validation.rule', 'ui.router', 'rzModule', 'angularjs-dropdown-multiselect', 'ngTagsInput']);
var interceptor = function () {
    return {
        'request': function (config) {
            config.headers['Authorization'] = "Bearer " + localStorage.getItem('accessToken');
            return config;
        }
    }
};



var enumPermissionPlans= { Dashboard: "1", AccountDetail: "2", Compare: "3", Pin_Account: "4", Performance_Comparison_Email_Notification: "5", Exclude_Account: "6", Add_Report_User: "7", Embed_Widget: "8", API_Data_for_Widget: "9", Export_Data: "10", }
app.config(['$httpProvider', function ($httpProvider) {
    $httpProvider.interceptors.push(interceptor);
}]);
//app.config(['$validationProvider', function ($validationProvider) {
//    $validationProvider.showSuccessMessage = false;
//    $validationProvider.showErrorMessage = true;
//    $validationProvider.setValidMethod('submit');
//}]);

app.config(['$stateProvider', '$urlRouterProvider', function ($stateProvider, $urlRouterProvider) {
    $urlRouterProvider.otherwise('/login');
    $stateProvider
     .state('main', {
         url: '',
         abstract: true,
         views: {
             '': {
                 templateUrl: serviceBase + "app/Views/Layout.html",
             }
         },
         resolve: { // Any property in resolve should return a promise and is executed before the view is loaded
             hidePreLoader: ['$timeout', function ($timeout) {
                 $timeout(function () {
                     $(".preloader").fadeOut();
                 }, 500);
             }]
         },
     }).state('login', {
         url: "/login",
         templateUrl: serviceBase + "app/Views/Login.html",
         controller: "loginController as authenticateVM"
     }).state('logout', {
         url: "/logout",
         //templateUrl: serviceBase + "app/Views/Login.html",
         templateUrl: '',
         controller: "logoutController"
     }).state('signup', {
         url: "/Signup",
         templateUrl: serviceBase + "app/Views/SignUp.html",
         controller: "signUpController as userVM"
     }).state('emailConfirm', {
         url: "/Confirm/:email/:code",
         //templateUrl: serviceBase + "app/Views/EmailConfirm.html",
         controller: "emailConfirmController as emailConfirmVm"
     }).state('setPassword', {
         url: "/SetPassword/:email/:code",
         //templateUrl: serviceBase + "app/Views/SetPassword.html",
         controller: "setPasswordController as setPwdVm"
     }).state('main.dashboard', {
         url: '/Dashboard',
         views: {
             'content': {
                 templateUrl: serviceBase + "app/Views/Dashboard.html",
                 controller: "dashboardController as dashboardVM"
             }
         }
     }).state('main.performers', {
         url: '/Performers',
         views: {
             'content': {
                 templateUrl: serviceBase + "app/Views/Performers.html",
                 //controller: "performersController as performersVM"
             }
         }
     }).state('main.pinnedUsers', {
         url: '/PinnedUsers',
         views: {
             'content': {
                 templateUrl: serviceBase + "app/Views/PinnedUsers.html",
                 controller: "pinnedUsersController as pinnedUsersVm"
             }
         }
     }).state('main.userDetails', {
         url: '/UserDetails/:accountId',
         views: {
             'content': {
                 templateUrl: serviceBase + "app/Views/UserDetails.html",
                 controller: "userDetailsController as userDetailsVm"
             }
         }
     }).state('main.configure', {
         url: '/Configure',
         views: {
             'content': {
                 templateUrl: serviceBase + "app/Views/Configure.html",
                 controller: "configureController as configureVm"
             }
         }
     }).state('main.comparison', {
         url: '/Compare',
         views: {
             'content': {
                 templateUrl: serviceBase + "app/Views/Compare.html",
                 controller: "compareController as compareVm"
             }
         }

     }).state('main.account', {
         url: '/Account',
         views: {
             'content': {
                 templateUrl: serviceBase + "app/Views/UserAccount.html",
                 controller: "userAccountController as accountVm"
             }
         }

     }).state('main.userAccount', {
         url: '/UserAccount',
         views: {
             'content': {
                 templateUrl: serviceBase + "app/Views/ReportUserAccount.html",
                 controller: "reportUserAccountController as userAccountVm"
             }
         }

     });
}]);

//app.config(['$routeProvider', function ($routeProvider) {
//    $routeProvider.when("/", {
//        controller: "loginController",
//        templateUrl: "/app/Views/login.html",
//        controllerAs: 'authenticateVM'
//    });
//    $routeProvider.when("/Dashboard", {
//        controller: "dashboardController",
//        templateUrl: "/app/Views/Dashboard.html",
//        controllerAs: 'dashboardVM'
//    });
//    $routeProvider.when("/SignUp", {
//        controller: "signUpController",
//        templateUrl: "app/Views/SignUp.html",
//        controllerAs: 'userVM'
//    });
//    $routeProvider.when("/Confirm/:email?/:code?", {
//        controller: "emailConfirmController",
//        templateUrl: "app/Views/EmailConfirm.html",
//        controllerAs: 'emailConfirmVm'
//    });
//    $routeProvider.when("/Dashboard", {
//        controller: "dashboardController",
//        templateUrl: "app/Views/Dashboard.html",
//        controllerAs: 'dashboardVM'
//    });
//    $routeProvider.otherwise({ redirectTo: "" });
//}]);

app.config(['$httpProvider', function ($httpProvider) {
    $httpProvider.interceptors.push(['$q', '$rootScope', '$window', '$location', function ($q, $rootScope, $window, $location) {
        return {
            request: function (config) {
                return config;
            },
            requestError: function (rejection) {
                return $q.reject(rejection);
            },
            response: function (response) {
                if (response.status == "401") {
                    $location.path('');
                }
                //the same response/modified/or a new one need to be returned.
                return response;
            },
            responseError: function (rejection) {
                if (rejection.status == "401") {
                    $location.path('');
                }
                return $q.reject(rejection);
            }
        };
    }]);
}]);

app.config(['$validationProvider', function ($validationProvider) {
    $validationProvider.showSuccessMessage = false;
    $validationProvider.showErrorMessage = true;
    $validationProvider.setValidMethod('submit');
    $validationProvider
        .setExpression({
            confirmpassword: function (value, scope, element, attrs, param) {
                if (attrs.pwd != "" && attrs.pwd == value)
                    return true;
                else if (attrs.pwd != "")
                    return false;
                return true;
            },//it Made for Password Length atleast 6 characters
            passwordlength: function (value, scope, element, attrs, param) {
                if (value.length > 5)
                    return true;
                else
                    return false;
            },
            //it Made for Name Length atleast 3 characters
            nameMinLength: function (value, scope, element, attrs, param) {
                if (value.length > 2)
                    return true;
                else
                    return false;
            },
            lessthanhundered: function (value, scope, element, attrs, param) {
                if (value <= 100)
                    return true;
                else
                    return false;
            },
            //it Made for Name Length should not exceed 20 characters
            nameMaxLength: function (value, scope, element, attrs, param) {
                if (value.length < 19)
                    return true;
                else
                    return false;
            },
            morethanzero: function (value, scope, element, attrs, param) {
                if (value > 0) {
                    return true;
                }
                else
                    return false;
            },
            specialsymbols: function (value, scope, element, attrs, param) {

                var regex = new RegExp("^[0-9.]+$");
                if (!regex.test(value)) {
                    //var dhg = value.contains('.');
                    return false;
                } else
                    return true;
            },
            numbersOnly: function (value, scope, element, attrs, param) {
                var regex = new RegExp("^[0-9]{1,6}$");
                if (!regex.test(value)) {
                    //var dhg = value.contains('.');s
                    return false;
                } else
                    return true;
            },
            numbersAndDecimals: function (value, scope, element, attrs, param) {
                var regex = new RegExp("^(([1-9]*)|(([1-9]*)\.([0-9]*)))$");
                if (!regex.test(value)) {
                    return false;
                } else
                    return true;
            },
            objectrequired: function (value, scope, element, attrs, param) {      // This validation is for that controls, those are having object selected in ng-model. Such as for dropdown control.
                var obj = scope.$eval(attrs.ngModel);
                if (obj[attrs.validatecolumn] == undefined || obj[attrs.validatecolumn] == 0 || obj[attrs.validatecolumn] == "") {
                    return false;
                } else {
                    return true;
                }
            },
            contactNo: function (value, scope, element, attrs, param) {
                if (value.length >= 10) {
                    return true;
                }
                else {
                    return false;
                }
            },
            image: function (value, scope, element, attrs, param) {
                if (value != null || scope.Project.ImagePath != null) {
                    return true;
                }
                else {
                    return false;
                }
            },
            minPledge: function (value, scope, element, attrs, param) {
                if (scope.MinPledge <= value) {
                    return true;
                }
                else {
                    return false;
                }
            },
            minCardLength: function (value, scope, element, attrs, param) {
                if (value.length >= 15) {
                    return true;
                }
                else {
                    return false;
                }
            },
            exceedMax: function (value, scope, element, attrs, param) {
                if (value > 100000) {
                    return false;
                }
                else {
                    return true;
                }
            },
            luhnCheck: function (value, scope, element, attrs, param) {
                var len = value.length,
                mul = 0,
                prodArr = [
                    [0, 1, 2, 3, 4, 5, 6, 7, 8, 9],
                    [0, 2, 4, 6, 8, 1, 3, 5, 7, 9]
                ],
                sum = 0;

                while (len--) {
                    sum += prodArr[mul][parseInt(value.charAt(len), 10)];
                    mul ^= 1;
                }
                return sum % 10 === 0 && sum > 0;
            }
        })
        .setDefaultMsg({
            minPledge: {
                error: 'Please add value more than minimum pledge'
            },
            numbersOnly: {
                error: 'Not a valid number'
            },
            numbersAndDecimals: {
                error: 'Not a valid number'
            },
            image: {
                error: 'required'
            },
            confirmpassword: {
                error: 'Password doesnot match'
            },
            passwordlength: {
                error: 'Password must be of minimium six character long'
            },
            lessthanhundered: {
                error: 'Max Percentage should be 100'
            },
            nameMinLength: {
                error: 'Name must be of minimium three character long'
            },
            nameMaxLength: {
                error: 'Name can not be twenty character long'
            },
            morethanzero: {
                error: 'Not Valid',
            },
            specialsymbols: {
                error: 'only enter decimal value',
            },
            objectrequired: {
                error: 'This should be Required!!',
            },
            contactNo:
            {
                error: 'Contact no. must be of minimum ten digit long'
            },
            minCardLength:
            {
                error: 'Card Number not valid'
            },
            exceedMax:
            {
                error: 'Value cannot be more than 100,000'
            },
            luhnCheck:
            {
                error: 'Card Number not valid'
            }
        });
}]);
app.constant('preselectedTimelineID', 'ASysKUKPbRY=')//timeline id 49 (Current Year)
