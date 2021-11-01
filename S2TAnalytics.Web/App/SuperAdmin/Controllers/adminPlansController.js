'use strict'
app.controller('adminPlansController', ['$scope', 'adminPlansService', '$location', '$timeout', function ($scope, adminPlansService, $location, $timeout) {
    var vm = this;

    vm.Plans = {

    }
    vm.AllplansValues = [];
    vm.AllWidget = [];
    vm.AllPlansWidget = [];
    var init=function()
    {
        GetAllPlans();
    }

    init();

    function GetAllPlans() {
      
        adminPlansService.getAllPlans().then(function (response) {
     
            if (response.data.response.Success) {
                var data = response.data.response.MultipleData;

                vm.AllplansValues = data.plans;

                vm.AllWidget = data.widgets;
               

                angular.forEach(data.plans, function (mydata, index) {
 
                    vm.AllPlansWidget.push({ WidgetsAccess: mydata.WidgetsAccess, PlanID: mydata.PlanID, InfrastructureCost: mydata.InfrastructureCost });
                })
           
            }
            else {
                alert("Error");
            }
        })
    };

    vm.checkPermission=function()
    {
        vm.AllPlansWidget;
    }
    vm.UpdatePlans= function()
    {
    
        adminPlansService.updateAllPlans(vm.AllPlansWidget).then(function (response) {
            if (response.data.response)
            {
                toastr.success("Updated.");
            }
        })
    }
}])