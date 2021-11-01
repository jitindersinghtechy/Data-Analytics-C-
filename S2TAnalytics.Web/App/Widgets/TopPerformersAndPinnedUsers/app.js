var serviceBase = 'http://analyticsv1.azurewebsites.net/';
//var serviceBase = 'http://localhost:56024/';
var appPerformersAndPinned = angular.module('topPerformersAndPinnedApp', ['angularjs-dropdown-multiselect']);
var interceptorPerformersAndPinned = function () {
    return {
        'request': function (config) {
            config.headers['Authorization'] = "Bearer " + $("#scriptTopPerformersAndPinned").attr("token");
            return config;
        }
    }
};
appPerformersAndPinned.config(['$httpProvider', '$sceDelegateProvider', function ($httpProvider, $sceDelegateProvider) {
    //$httpProvider.defaults.headers.common = {};
    //$httpProvider.defaults.headers.post = {};
    //$httpProvider.defaults.headers.put = {};
    //$httpProvider.defaults.headers.patch = {};
    $httpProvider.interceptors.push(interceptorPerformersAndPinned);
    $sceDelegateProvider.resourceUrlWhitelist([
    // Allow same origin resource loads.
    'self',
    // Allow loading from our assets domain.  Notice the difference between * and **.
    'http://analyticsv1.azurewebsites.net/**']);
}]);

$(document).ready(function () {
    angular.bootstrap($("[app='topPerformersAndPinnedApp']")[0], ['topPerformersAndPinnedApp']);
});