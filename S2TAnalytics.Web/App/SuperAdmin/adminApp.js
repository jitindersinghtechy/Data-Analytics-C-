var app = angular.module('S2TAnalyticsAdminApp', ['validation', 'validation.rule', 'ui.router']);
var interceptor = function () {
    return {
        'request': function (config) {
            config.headers['Authorization'] = "Bearer " + localStorage.getItem('accessToken');
            return config;
        }
    }
};

app.config(['$httpProvider', function ($httpProvider) {
    $httpProvider.interceptors.push(interceptor);
}]);




app.filter('range', function () {

    return function (input, total, currentPage) {
        total = parseInt(total);
        if (total > 0) {
            currentPage = parseInt(currentPage);
            var begin = currentPage - 1;
            var end = begin + 5;

            if (end >= total) {
                begin = total < 5 ? 0 : total - 5;
                end = total;
            }
            for (var i = 1; i <= total; i++) {
                input.push(i.toString());
            }
            //if (end != total) {
            //    input[end - 1] = input[end - 1] + ' ...';
            //}
            var aa = input.slice(begin, end);
            return input.slice(begin, end);
        }
        return input;

    };


});






app.config(['$stateProvider', '$urlRouterProvider', function ($stateProvider, $urlRouterProvider) {
 
    $urlRouterProvider.otherwise('/AdminLogin');
    $stateProvider.state('adminLogin', {
        url: "/AdminLogin",
        templateUrl: serviceBase + "App/SuperAdmin/Views/login.html",
        controller: "adminLoginController as vm"
    }).state('superAdmin', {
        url: '',
        abstract: true,
        views: {
            '': {
                templateUrl: serviceBase + "App/SuperAdmin/Views/Layout.html",
            }
        },
        resolve: { 
            hidePreLoader: ['$timeout', function ($timeout) {
                $timeout(function () {
                    $(".preloader").fadeOut();
                }, 500);
            }]
        },
    }).state('superAdmin.adminDashboard', {
        url: '/AdminDashboard',
        views: {
            'content': {
                templateUrl: serviceBase + "App/SuperAdmin/Views/index.html",
                controller: "adminDashboardController as vm"
            }
        }
    }).state('superAdmin.adminSubscribers', {
        url: '/AdminSubscribers',
        views: {
            'content': {
                templateUrl: serviceBase + "App/SuperAdmin/Views/subscribers.html",
                controller: "adminSubscriberController as vm"
            }
        }
    }).state('superAdmin.adminViewDetails', {
        url: '/AdminViewDetails/:subscriberId',
        views: {
            'content': {
                templateUrl: serviceBase + "App/SuperAdmin/Views/subscribersList.html",
                controller: "adminViewDetailsController as vm"
            }
        }
    }).state('superAdmin.adminPlans', {
        url: '/AdminPlans',
        views: {
            'content': {
                templateUrl: serviceBase + "App/SuperAdmin/Views/managePlans.html",
                controller: "adminPlansController as vm"
            }
        }
    }).state('superAdmin.adminCoupon', {
        url: '/AdminCoupon',
        views: {
            'content': {
                templateUrl: serviceBase + "App/SuperAdmin/Views/coupon.html",
                controller: "adminCouponController as vm"
            }
        }
    }).state('superAdmin.adminNotifications', {
        url: '/AdminNotifications',
        views: {
            'content': {
                templateUrl: serviceBase + "App/SuperAdmin/Views/notification.html",
                controller: "adminNotificationsController as vm"
            }
        }
    });
}]);

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

