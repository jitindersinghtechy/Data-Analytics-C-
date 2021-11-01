var serviceBase = 'http://analyticsv1.azurewebsites.net/';
//var serviceBase = 'http://localhost:56024/';
var appInstrumentGroup = angular.module('instrumentGroupApp', ['angularjs-dropdown-multiselect']);
var interceptorInstrumentGroup = function () {
    return {
        'request': function (config) {
            config.headers['Authorization'] = "Bearer " + $("#scriptInstrumentGroup").attr("token");
            return config;
        }
    }
};
appInstrumentGroup.config(['$httpProvider', '$sceDelegateProvider', function ($httpProvider, $sceDelegateProvider) {
    //$httpProvider.defaults.headers.common = {};
    //$httpProvider.defaults.headers.post = {};
    //$httpProvider.defaults.headers.put = {};
    //$httpProvider.defaults.headers.patch = {};
    $httpProvider.interceptors.push(interceptorInstrumentGroup);
    $sceDelegateProvider.resourceUrlWhitelist([
    // Allow same origin resource loads.
    'self',
    // Allow loading from our assets domain.  Notice the difference between * and **.
    'http://analyticsv1.azurewebsites.net/**']);
}]);

$(document).ready(function () {
    angular.bootstrap($("[app='instrumentGroupApp']")[0], ['instrumentGroupApp']);
});

