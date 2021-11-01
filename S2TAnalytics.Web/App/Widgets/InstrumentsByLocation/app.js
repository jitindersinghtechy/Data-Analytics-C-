var serviceBase = 'http://analyticsv1.azurewebsites.net/';
//var serviceBase = 'http://localhost:56024/';
var appInstrumentLocation = angular.module('instrumentLocationApp', ['angularjs-dropdown-multiselect']);
var interceptorInstrumentLocation = function () {
    return {
        'request': function (config) {
            config.headers['Authorization'] = "Bearer " + $("#scriptInstrumentLocation").attr("token");
            return config;
        }
    }
};
appInstrumentLocation.config(['$httpProvider', '$sceDelegateProvider', function ($httpProvider, $sceDelegateProvider) {
    //$httpProvider.defaults.headers.common = {};
    //$httpProvider.defaults.headers.post = {};
    //$httpProvider.defaults.headers.put = {};
    //$httpProvider.defaults.headers.patch = {};
    $httpProvider.interceptors.push(interceptorInstrumentLocation);
    $sceDelegateProvider.resourceUrlWhitelist([
    // Allow same origin resource loads.
    'self',
    // Allow loading from our assets domain.  Notice the difference between * and **.
    'http://analyticsv1.azurewebsites.net/**']);
}]);

$(document).ready(function () {
    angular.bootstrap($("[app='instrumentLocationApp']")[0], ['instrumentLocationApp']);
});
