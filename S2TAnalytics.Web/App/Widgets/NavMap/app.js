var serviceBase = 'http://analyticsv1.azurewebsites.net/';
//var serviceBase = 'http://localhost:56024/';
var appNavMap = angular.module('navMapApp', ['angularjs-dropdown-multiselect']);
var interceptorNavMap = function () {
    return {
        'request': function (config) {
            config.headers['Authorization'] = "Bearer " + $("#scriptNavMap").attr("token");
            return config;
        }
    }
};
appNavMap.config(['$httpProvider', '$sceDelegateProvider', function ($httpProvider, $sceDelegateProvider) {
    //$httpProvider.defaults.headers.common = {};
    //$httpProvider.defaults.headers.post = {};
    //$httpProvider.defaults.headers.put = {};
    //$httpProvider.defaults.headers.patch = {};
    $httpProvider.interceptors.push(interceptorNavMap);
    $sceDelegateProvider.resourceUrlWhitelist([
    // Allow same origin resource loads.
    'self',
    // Allow loading from our assets domain.  Notice the difference between * and **.
    'http://analyticsv1.azurewebsites.net/**']);
}]);

$(document).ready(function () {
    angular.bootstrap($("[app='navMapApp']")[0], ['navMapApp']);
});