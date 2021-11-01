'use strict';
app.controller('dashboardController', ['$scope', 'dashboardService', '$location', function ($scope, dashboardService, $location) {
    var vm = this;
    vm.showTopPerformers = true;
    vm.showPinnedUsers = false;


    vm.switchView = function (showTopPerformers, showPinnedUsers) {
        vm.showTopPerformers = showTopPerformers;
        vm.showPinnedUsers = showPinnedUsers;
    }
    //dashboardVM.AccountDetails = {};

    //dashboardVM.GetAccounts = function () {
    //    dashboardService.GetAccounts().then(function (response) {
    //        dashboardVM.AccountDetails = response.Data;
    //    });
    //}
    //dashboardVM.GetAccounts();


    //dashboardVM.showDailyStats = function (accountDetailsId) {
    //}
    //dashboardVM.showTransactionHistories = function (accountDetailsId) {
    //}
}]);