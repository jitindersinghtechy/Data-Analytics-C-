appInstrumentGroup.component("instrumentsByGroup", {
    templateUrl: serviceBase + "api/Dashboard/InstrumentsGroupView",
    bindings: { name: '@' },
    controller: ['$scope', 'instrumentService', '$location', '$filter', '$compile', '$timeout', function ($scope, instrumentService, $location, $filter, $compile, $timeout) {

        var vm = this;
        var instrumentData = [], series = [], Instruments = [], selectedSeries = "", selectedGroup = "";
        vm.Groups = [];
        vm.Accounts = [];
        vm.IsDrillDownVisible = false;
        vm.SelectedTimelineId = "yDwjeVGRQ8E=";
        vm.chartType = "column";
        vm.loadComplete = false; //Loader
        vm.SelectedGroups = [];
        vm.GroupSettings = {
            showCheckAll: true,
            showUncheckAll: false,
            smartButtonMaxItems: 3,
            smartButtonTextConverter: function (itemText, originalItem) {
                return itemText;
            }
        }

        vm.SelectedAccounts = [];
        vm.AccountSettings = {
            showCheckAll: true,
            showUncheckAll: false,
            //smartButtonMaxItems: 3,
            //smartButtonTextConverter: function (itemText, originalItem) {
            //    return itemText;
            //}
        }
        vm.getClass = function (timeLine) {
            return (vm.Timeline == timeLine) ? 'active-menu' : '';
        }

        $scope.$watchCollection("vm.SelectedGroups", function (newValues, oldValues) {

            var categories = [];
            angular.forEach(vm.SelectedGroups, function (item, index) {
                categories.push(item.id);
            });
            bindMainChart(categories);

        });

        $scope.$watchCollection("vm.SelectedAccounts", function (newValues, oldValues) {

            if (vm.IsDrillDownVisible) {
                var categories = [];
                angular.forEach(vm.SelectedAccounts, function (item, index) {
                    categories.push(item.id);
                });
                bindDrillDownChart1(categories);
            }

        });

        $scope.$watch("vm.Timeline", function (newValue, oldValue) {
            if (newValue != undefined && vm.SelectedTimelineId != newValue.stringValue) {
                vm.SelectedTimelineId = newValue.stringValue;
                vm.loadComplete = false; //Loader
                if (!vm.IsDrillDownVisible) {
                    vm.mainChart();
                } else if (vm.IsDrillDownVisible) {
                    vm.drillDownChart1();
                }
            }
        });

        vm.click = function () {
            var chart = $("#groupInstrumentsContainer").highcharts();
            $.each(chart.series, function (key, series) {
                series.update(
                    { type: vm.chartType },
                    false
                );
            });
            chart.redraw();
        }
        vm.mainChart = function () {
            vm.loadComplete = false; //Loader
            instrumentService.getInstrumentsByGroup(vm.SelectedTimelineId).then(function (response) {
                instrumentData = response.data.Data;
                vm.TimeLines = response.data.Timelines;
                angular.forEach(vm.TimeLines, function (timeline, index) {
                    if (timeline.stringValue == vm.SelectedTimelineId) {
                        vm.Timeline = timeline;
                    }
                });
                Instruments = response.data.Instruments;
                if (vm.Groups.length == 0) {
                    angular.forEach(response.data.Groups, function (group, index) {
                        vm.Groups.push({ id: group, label: group });
                        vm.SelectedGroups.push({ id: group, label: group });
                    });
                } else {
                    var categories = [];
                    vm.Groups = [];
                    vm.SelectedGroups = [];
                    angular.forEach(response.data.Groups, function (group, index) {
                        vm.Groups.push({ id: group, label: group });
                        vm.SelectedGroups.push({ id: group, label: group });
                        categories.push(group);
                    });
                    vm.IsDrillDownVisible = false;
                    $("#GroupCompareDataHead1").empty();  // For Back button
                    $("#GroupCompareDataHead1").append('  <img src="http://analyticsv1.azurewebsites.net/images/icon/businessman.png" alt=""><h3>TOP INSTRUMENTS BY USER GROUPS</h3>');

                    bindMainChart(categories);
                }
            });
        }

        var bindMainChart = function (categories) {
            series = [];
            angular.forEach(Instruments, function (instrument, instrumentIndex) {
                var data = [];
                angular.forEach(categories, function (group, countryIndex) {
                    var groupData = $filter('filter')(instrumentData, { UserGroup: group }, true);

                    var total = 0, length = 0;
                    angular.forEach(groupData, function (groupObj, groupObjIndex) {
                        var instruments = $filter('filter')(groupObj.InstrumentStatsModel, { InstrumentName: instrument }, true)[0];
                        if (instruments) {
                            total += instruments.Volume;
                        }
                        length += 1;
                    });

                    //data.push(parseFloat((total / (length == 0 ? 1 : length)).toFixed(2)));
                    data.push(parseFloat(total.toFixed(2)));
                });

                series.push({
                    name: instrument,
                    data: data
                });
            });
            vm.loadComplete = true; //Loader
            if ($("#groupInstrumentsContainer").highcharts() != undefined) {
                $("#groupInstrumentsContainer").highcharts().destroy();
            }
            $timeout(function () {
                Highcharts.chart("groupInstrumentsContainer", {
                    credits: {
                        enabled: false
                    },
                    chart: {
                        type: vm.chartType
                    },
                    title: {
                        text: ''
                    },
                    tooltip: {
                        pointFormat: '{point.series.name}  :  <b> {point.y:,.2f}</b>',
                    },
                    xAxis: {
                        categories: categories
                    },
                    yAxis: {
                        min: 0,
                        title: {
                            text: ''
                        }
                    },
                    legend: {
                        //align: 'right',
                        //verticalAlign: 'middle',
                        //width: 380,
                        itemWidth: 75,
                        //y: 10
                    },
                    plotOptions: {
                        column: {
                            stacking: 'normal'
                        },
                        series: {
                            borderColor: 'none',
                            point: {
                                events: {
                                    click: function () {
                                        vm.selectedCountry = this.category;
                                        selectedSeries = this.series.name;
                                        selectedGroup = this.category;
                                        vm.drillDownChart1();
                                    }
                                }
                            }
                        }
                    },
                    series: series
                });
            }, 10);
        }

        vm.drillDownChart1 = function () {
            vm.loadComplete = false; //Loader
            instrumentService.getInstrumentsByGroup(vm.SelectedTimelineId, selectedSeries, selectedGroup).then(function (response) {
                instrumentData = response.data.Data;
                vm.SelectedAccounts = [], vm.Accounts = [];
                angular.forEach(response.data.Accounts, function (account, accountIndex) {
                    vm.Accounts.push({ id: account, label: account });
                    vm.SelectedAccounts.push({ id: account, label: account });
                });
                vm.IsDrillDownVisible = true;
            });
        }

        var bindDrillDownChart1 = function (categories) {
            var series = [];
            var data = [];
            angular.forEach(categories, function (account, accountIndex) {
                var accountData = $filter('filter')(instrumentData, { Name: account }, true);
                var value = 0;
                angular.forEach(accountData, function (accountObj, accountObjIndex) {
                    if (accountObj.InstrumentStatsModel[0]) {
                        value = accountObj.InstrumentStatsModel[0].Volume;
                    }
                });
                data.push(value);
            });
            series.push({
                name: selectedSeries,
                data: data
            });
            vm.loadComplete = true; //Loader
            if ($("#groupInstrumentsContainer").highcharts() != undefined) {
                $("#groupInstrumentsContainer").highcharts().destroy();
            }
            $timeout(function () {
                Highcharts.chart("groupInstrumentsContainer", {
                    credits: {
                        enabled: false
                    },
                    chart: {
                        type: vm.chartType
                    },
                    title: {
                        text: ''
                    },
                    tooltip: {
                        pointFormat: '{point.series.name}  :  <b> {point.y:,.2f}</b>',
                    },
                    xAxis: {
                        categories: categories
                    },
                    legend: {
                        //align: 'right',
                        //verticalAlign: 'middle',
                        //width: 700,
                        itemWidth: 75,
                        //y: 10
                    },
                    yAxis: {
                        min: 0,
                        title: {
                            text: ''
                        }
                    },

                    plotOptions: {
                        column: {
                            stacking: 'normal'
                        },
                        series: {
                            borderColor: 'none'
                        }
                    },


                    series: series
                });
            }, 10);

            //$("#groupInstrumentsContainer").append("<button class='widget-back-btn' ng-click='vm.mainChart()'><i class='fa fa-arrow-left'></i>Back</button>");
            //$compile($("#groupInstrumentsContainer").contents())($scope);

            $("#GroupCompareDataHead1").empty();
            $("#GroupCompareDataHead1").append("<h3 ng-click='vm.mainChart()'><a prevent-default class='back-btn'><i class='fa fa-arrow-circle-left'></i>{{vm.selectedCountry}}</a></h3>");
            $compile($("#GroupCompareDataHead1").contents())($scope);
        }

        var init = function () {
            vm.mainChart();
        }

        init();

    }],
    controllerAs: 'vm'
});