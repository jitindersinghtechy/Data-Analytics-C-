var serviceBase = 'http://analyticsv1.azurewebsites.net/';
//var serviceBase = 'http://localhost:56024/';
var app = angular.module('comparisonApp', ['angularjs-dropdown-multiselect']);
var interceptor = function () {
    return {
        'request': function (config) {
            config.headers['Authorization'] = "Bearer " + $("#scriptComparison").attr("token");
            return config;
        }
    }
};
app.config(['$httpProvider', '$sceDelegateProvider', function ($httpProvider, $sceDelegateProvider) {
    //$httpProvider.defaults.headers.common = {};
    //$httpProvider.defaults.headers.post = {};
    //$httpProvider.defaults.headers.put = {};
    //$httpProvider.defaults.headers.patch = {};
    $httpProvider.interceptors.push(interceptor);
    $sceDelegateProvider.resourceUrlWhitelist([
    // Allow same origin resource loads.
    'self',
    // Allow loading from our assets domain.  Notice the difference between * and **.
    'http://analyticsv1.azurewebsites.net/**']);
}]);

$(document).ready(function () {
    angular.bootstrap($("[app='comparisonApp']")[0], ['comparisonApp']);
});


