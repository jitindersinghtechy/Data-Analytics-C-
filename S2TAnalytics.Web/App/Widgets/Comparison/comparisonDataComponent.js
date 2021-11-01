app.component("comparisonData", {
    templateUrl: serviceBase + "api/Dashboard/ComparisonView",
    bindings: { name: '@' },
    controller: ['$scope', 'comparisonDataService', '$location', '$filter', '$compile', '$timeout', function ($scope, comparisonDataService, $location, $filter, $compile, $timeout) {
        var vm = this;
        var comparisonData = [], series = [], selectedSeries = "", selectedCountry = "", selectedCity = "";
        vm.Countries = [];
        vm.Cities = [];
        vm.Accounts = [];
        vm.SelectedCountries = [];
        vm.SelectedCities = [];
        vm.SelectedAccounts = [];
        vm.TimeLines = [];
        vm.SelectedTimelineId = "yDwjeVGRQ8E=";
        vm.loadComplete = false; //Loader
        vm.chartType = "column";
        vm.CountrySettings = {
            showCheckAll: true,
            showUncheckAll: false,
            smartButtonMaxItems: 1,
            smartButtonTextConverter: function (itemText, originalItem) {
                return itemText;
            }
        }
        vm.IsDrillDownVisible = false;
        vm.IsDrillDown2Visible = false;
        vm.CitySettings = {
            showCheckAll: true,
            showUncheckAll: false,
            smartButtonMaxItems: 3,
            smartButtonTextConverter: function (itemText, originalItem) {
                return itemText;
            }
        }
        vm.AccountSettings = {
            showCheckAll: true,
            showUncheckAll: false,
            smartButtonMaxItems: 3,
            smartButtonTextConverter: function (itemText, originalItem) {
                return itemText;
            }
        }
        vm.click = function () {
            var chart = $("#comparisonDataContainer").highcharts();
            $.each(chart.series, function (key, series) {
                series.update(
                    { type: vm.chartType },
                    false
                );
            });
            chart.redraw();
        }

        vm.getClass = function (timeLine) {
            return (vm.Timeline == timeLine) ? 'active-menu' : '';
        }

        $scope.$watch("vm.Timeline", function (newValue, oldValue) {
            if (newValue != undefined && oldValue != newValue && oldValue != undefined && vm.SelectedTimelineId != newValue.stringValue) {
                vm.SelectedTimelineId = newValue.stringValue
                vm.loadComplete = false; //Loader
                if (!vm.IsDrillDownVisible && !vm.IsDrillDown2Visible) {
                    vm.mainChart();
                } else if (vm.IsDrillDownVisible) {
                    vm.drillDownChart1();
                } else if (vm.IsDrillDown2Visible) {
                    vm.drillDownChart2();
                }
            }
        });

        $scope.$watchCollection("vm.SelectedCountries", function (newValues, oldValues) {
            vm.IsDrillDownVisible = false;
            var categories = [];
            angular.forEach(vm.SelectedCountries, function (item, countryIndex) {
                categories.push(item.id);
            });
            bindMainChart(categories);
        });

        $scope.$watchCollection("vm.SelectedCities", function (newValues, oldValues) {
            if (vm.IsDrillDownVisible) {
                var categories = [];
                angular.forEach(vm.SelectedCities, function (item, countryIndex) {
                    categories.push(item.id);
                });
                bindDrillDownChart1(categories);
            }
        });

        $scope.$watchCollection("vm.SelectedAccounts", function (newValues, oldValues) {
            if (vm.IsDrillDown2Visible) {
                var categories = [];
                angular.forEach(vm.SelectedAccounts, function (item, accountIndex) {
                    categories.push(item.id);
                });
                bindDrillDownChart2(categories);
            }
        });

        vm.mainChart = function () {
            vm.loadComplete = false; //Loader
            comparisonDataService.getComparisonData(vm.SelectedTimelineId).then(function (response) {
                comparisonData = response.data.Data;
                vm.TimeLines = response.data.Timelines;
                angular.forEach(vm.TimeLines, function (timeline, index) {
                    if (timeline.stringValue == vm.SelectedTimelineId) {

                        vm.Timeline = timeline;
                    }
                });
                if (vm.Countries.length == 0) {
                    angular.forEach(response.data.Countries, function (country, countryIndex) {
                        vm.Countries.push({ id: country, label: country });
                        vm.SelectedCountries.push({ id: country, label: country });
                    });
                } else {
                    var categories = [];
                    vm.Countries = [];
                    vm.SelectedCountries = [];
                    angular.forEach(response.data.Countries, function (country, countryIndex) {
                        vm.Countries.push({ id: country, label: country });
                        vm.SelectedCountries.push({ id: country, label: country });
                        categories.push(country);
                    });
                    vm.IsDrillDownVisible = false;
                    $("#CompareDataHead1").empty();  // For Back button
                    $("#CompareDataHead1").append(' <img src="http://analyticsv1.azurewebsites.net/images/icon/graph-line-screen.png" alt=""><h3>PERFORMANCE <!--BY LOCATION--></h3>');
                    bindMainChart(categories);
                }
            });
        }

        var bindMainChart = function (categories) {
            series = [];
            var roiData = [], ddData = []; winData = [];
            angular.forEach(categories, function (country, countryIndex) {
                //vm.Countries.push({ id: country, label: country });
                //vm.SelectedCountries.push({ id: country, label: country });
                var countryData = $filter('filter')(comparisonData, { Country: country }, true);
                var ROI = 0; DD = 0; WIN = 0; length = 0;
                angular.forEach(countryData, function (countryObj, countryObjIndex) {
                    if (countryObj.AccountStats[0]) {
                        ROI += countryObj.AccountStats[0].ROI;
                        DD += countryObj.AccountStats[0].DD;
                        WIN += countryObj.AccountStats[0].WINRate;
                    }
                    length += 1;
                });
                roiData.push(ROI / (length == 0 ? 1 : length));
                ddData.push(DD / (length == 0 ? 1 : length));
                winData.push(WIN / (length == 0 ? 1 : length));
            });
            //Aus,Ind,US
            series.push({
                name: "ROI%",
                data: roiData
            });
            series.push({
                name: "WIN%",
                data: winData
            });
            series.push({
                name: "MAX DD%",
                data: ddData
            });
            vm.loadComplete = true; //Loader
            if ($("#comparisonDataContainer").highcharts() != undefined) {
                $("#comparisonDataContainer").highcharts().destroy();
            }
            $timeout(function () {
                Highcharts.chart("comparisonDataContainer", {
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
                        //min: 0,
                        title: {
                            text: 'Percentage'
                        }
                    },
                    plotOptions: {
                        column: {
                            stacking: 'percent'
                        },
                        series: {
                            borderColor: 'none',
                            point: {
                                events: {
                                    click: function () {
                                        vm.selectedCountry = this.category;
                                        selectedSeries = this.series.name, selectedCountry = this.category;
                                        vm.IsDrillDownVisible = true;
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

        var bindDrillDownChart1 = function (categories) {
            series = [];
            var data = [];
            angular.forEach(categories, function (city, cityIndex) {
                var cityData = $filter('filter')(comparisonData, { City: city }, true);
                var value = 0; length = 0;
                angular.forEach(cityData, function (cityObj, cityObjIndex) {
                    if (cityObj.AccountStats[0]) {
                        if (selectedSeries == "ROI%")
                            value += cityObj.AccountStats[0].ROI;
                        if (selectedSeries == "MAX DD%")
                            value += cityObj.AccountStats[0].DD;
                        if (selectedSeries == "WIN%")
                            value += cityObj.AccountStats[0].WINRate;
                    }
                    length += 1;
                });
                data.push(parseFloat((value / (length == 0 ? 1 : length)).toFixed(2)));
            });
            series.push({
                name: selectedSeries,
                data: data
            });
            vm.loadComplete = true; //Loader
            if ($("#comparisonDataContainer").highcharts() != undefined) {
                $("#comparisonDataContainer").highcharts().destroy();
            }
            $timeout(function () {
                Highcharts.chart("comparisonDataContainer", {
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
                    //title: {
                    //    text: 'Stacked column chart'
                    //},
                    xAxis: {
                        categories: categories
                    },
                    yAxis: {
                        //min: 0,
                        title: {
                            text: 'Percentage'
                        }
                    },
                    plotOptions: {
                        column: {
                            stacking: 'percent'
                        },
                        series: {
                            borderColor: 'none',
                            point: {
                                events: {
                                    click: function () {
                                        vm.selectedCity = this.category;
                                        vm.selectedValue = this.category;
                                        selectedCity = this.category;
                                        vm.drillDownChart2();
                                    }
                                }
                            }
                        }
                    },
                    series: series
                });
            }, 10);
            //$("#comparisonDataContainer").append("<button class='widget-back-btn' ng-click='vm.mainChart()'><i class='fa fa-arrow-left'></i> Back</button>");
            // $compile($("#comparisonDataContainer").contents())($scope);
            $("#CompareDataHead1").empty();
            $("#CompareDataHead1").append("<h3 ng-click='vm.mainChart()'><a prevent-default class='back-btn'><i class='fa fa-arrow-circle-left'></i>{{vm.selectedCountry}}</a></h3>");
            $compile($("#CompareDataHead1").contents())($scope);
        }

        vm.drillDownChart1 = function (isBack) {
            vm.loadComplete = false; //Loader
            comparisonDataService.getComparisonData(vm.SelectedTimelineId, selectedCountry).then(function (response) {
                comparisonData = response.data.Data;
                vm.SelectedCities = [], vm.Cities = [];
                angular.forEach(response.data.Cities, function (city, cityIndex) {
                    vm.Cities.push({ id: city, label: city });
                    vm.SelectedCities.push({ id: city, label: city });
                });
                if (isBack) {
                    var categories = [];
                    angular.forEach(vm.SelectedCities, function (item, cityIndex) {
                        categories.push(item.id);
                    });
                    bindDrillDownChart1(categories);
                }
            });
            vm.IsDrillDownVisible = true;
            vm.IsDrillDown2Visible = false;
        }

        var bindDrillDownChart2 = function (categories) {
            var series = [];
            var data = [];
            angular.forEach(categories, function (account, cityIndex) {
                var accountData = $filter('filter')(comparisonData, { Name: account }, true);
                var value = 0; length = 0;
                angular.forEach(accountData, function (accountObj, accountObjIndex) {
                    if (accountObj.AccountStats[0]) {
                        if (selectedSeries == "ROI%")
                            value = accountObj.AccountStats[0].ROI;
                        if (selectedSeries == "MAX DD%")
                            value = accountObj.AccountStats[0].DD;
                        if (selectedSeries == "WIN%")
                            value = accountObj.AccountStats[0].WINRate;
                    }
                });
                data.push(value);
            });
            series.push({
                name: selectedSeries,
                data: data
            });
            vm.loadComplete = true; //Loader
            if ($("#comparisonDataContainer").highcharts() != undefined) {
                $("#comparisonDataContainer").highcharts().destroy();
            }
            $timeout(function () {
                Highcharts.chart("comparisonDataContainer", {
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
                        //min: 0,
                        title: {
                            text: 'Percentage'
                        }
                    },
                    plotOptions: {
                        column: {
                            borderColor: 'none',
                            stacking: 'percent'
                        },
                        series: {
                            borderColor: 'none'
                        }
                    },
                    series: series
                });
            }, 10);
            //$("#comparisonDataContainer").append("<button class='widget-back-btn' ng-click='vm.drillDownChart1(true)'><i class='fa fa-arrow-left'></i>Back</button>");
            //$compile($("#comparisonDataContainer").contents())($scope);    
            $("#CompareDataHead1").empty();
            $("#CompareDataHead1").append("<h3 ng-click='vm.drillDownChart1(true)'><a prevent-default class='back-btn'><i class='fa fa-arrow-circle-left'></i>{{vm.selectedCity}}</a></h3>");
            $compile($("#CompareDataHead1").contents())($scope);
        }

        vm.drillDownChart2 = function () {
            vm.loadComplete = false; //Loader
            comparisonDataService.getComparisonData(vm.SelectedTimelineId, selectedCountry, selectedCity).then(function (response) {
                comparisonData = response.data.Data;
                vm.SelectedAccounts = [], vm.Accounts = [];
                angular.forEach(response.data.Accounts, function (account, accountIndex) {
                    vm.Accounts.push({ id: account, label: account });
                    vm.SelectedAccounts.push({ id: account, label: account });
                });
                vm.IsDrillDownVisible = false;
                vm.IsDrillDown2Visible = true;
            });

        }


        var init = function () {
            vm.mainChart();
        }

        init();
    }],
    controllerAs: 'vm'
});