var serviceBase = 'http://analyticsv1.azurewebsites.net/';
var appTrendingPerformers = angular.module('trendingPerformersApp', ['angularjs-dropdown-multiselect']);
var interceptorTrendingPerformers = function () {
    return {
        'request': function (config) {
            config.headers['Authorization'] = "Bearer " + $("#scriptTrendingPerformers").attr("token");
            return config;
        }
    }
};
appTrendingPerformers.config(['$httpProvider', '$sceDelegateProvider', function ($httpProvider, $sceDelegateProvider) {
    //$httpProvider.defaults.headers.common = {};
    //$httpProvider.defaults.headers.post = {};
    //$httpProvider.defaults.headers.put = {};
    //$httpProvider.defaults.headers.patch = {};
    $httpProvider.interceptors.push(interceptorTrendingPerformers);
    $sceDelegateProvider.resourceUrlWhitelist([
    // Allow same origin resource loads.
    'self',
    // Allow loading from our assets domain.  Notice the difference between * and **.
    'http://analyticsv1.azurewebsites.net/**']);
}]);

$(document).ready(function () {
    angular.bootstrap($("[app='trendingPerformersApp']")[0], ['trendingPerformersApp']);
});