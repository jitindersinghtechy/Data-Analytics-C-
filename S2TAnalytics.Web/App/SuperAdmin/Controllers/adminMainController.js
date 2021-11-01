'use strict'
app.controller('adminMainController', ['$rootScope','$scope', 'adminMainService', '$location', '$window', '$http', '$timeout',
function ($rootScope,$scope, adminMainService, $location, $window, $http, $timeout) {
    var vm = this;
    vm.Notifications = [];
    vm.NotificationCount = 0;
    //vm.selectedMenu = "Dashboard";
    vm.PrevPlan = "";
    vm.NewPlan = "";

     vm.OrganizationName = "";
    vm.fullName = localStorage.getItem("fullName");
    vm.UserRoleId = localStorage.getItem('roleId');
    vm.signout = function () {
        $(".preloader").show();
        $window.localStorage.clear();
        $location.path('/AdminLogin');
    }
    vm.getClass = function (path) {
        return ($location.path().substr(0, path.length) === path) ? 'active' : '';
    }

    vm.searchByName = function () {

        $location.path('/AdminSubscribers');
        $rootScope.$broadcast('searchOrganizationName', {
            data: vm.OrganizationName
        });
       
    }


    vm.getNotifications = function()
    {
        adminMainService.getNotifications().then(function (response) {
            if (response.data.response.Success) {
                vm.Notifications = response.data.response.Data;
                vm.NotificationCount = vm.Notifications.length;
            }
        });
    }

    var init =function()
    {
        vm.getNotifications();
    }


     vm.readNotifications = function () {
         adminMainService.readNotifications().then(function (response) {
             if (response.data.Success) {
                 vm.NotificationCount = 0;
             }
         });
     }

     vm.removeSingleNotifications = function (id,index) {
         adminMainService.removeSingleNotifications(id).then(function (response) {
             if (response.data.Success) {
                 vm.Notifications.splice(index, 1);
             }
         });
     }





    init();
}])