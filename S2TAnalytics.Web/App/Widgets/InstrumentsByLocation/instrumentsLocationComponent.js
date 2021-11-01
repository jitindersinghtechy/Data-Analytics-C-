
appInstrumentLocation.component("instrumentsByLocation", {
    templateUrl: serviceBase + "api/Dashboard/InstrumentsLocationView",
    bindings: { name: '@' },
    controller: ['$scope', 'instrumentLocationService', '$location', '$filter', '$compile', '$timeout', function ($scope, instrumentService, $location, $filter, $compile, $timeout) {

        var vm = this;
        var instrumentData = [], series = [], Instruments = [], selectedSeries = "", selectedCountry = "", selectedCity = "";
        vm.Countries = [];
        vm.Cities = [];
        vm.Accounts = [];
        vm.SelectedCountries = [];
        vm.chartType = "column";
        vm.loadComplete = false; //Loader
        vm.SelectedTimelineId = "yDwjeVGRQ8E=";
        vm.CountrySettings = {
        showCheckAll: true,
            showUncheckAll: false,
            smartButtonMaxItems: 3,
            smartButtonTextConverter: function (itemText, originalItem) {
                return itemText;
            }
        }
        vm.IsDrillDownVisible = false;
        vm.IsDrillDown2Visible = false;

        vm.SelectedCities = [];
        vm.CitySettings = {
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
             smartButtonMaxItems: 3,
            smartButtonTextConverter: function (itemText, originalItem) {
                return itemText;
            }
        }

        vm.getClass = function (timeLine) {
            return (vm.Timeline == timeLine) ? 'active-menu' : '';
        }

        $scope.$watch("vm.Timeline", function (newValue, oldValue) {
            
            if (newValue != undefined && vm.SelectedTimelineId != newValue.stringValue) {
                vm.SelectedTimelineId = newValue.stringValue;
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

        vm.click = function () {
            var chart = $("#locationInstrumentsContainer").highcharts();
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
            instrumentService.getAllInstruments(vm.SelectedTimelineId).then(function (response) {
                instrumentData = response.data.Data;
                
                vm.TimeLines = response.data.Timelines;
                 angular.forEach(vm.TimeLines, function (timeline, index) {
                    if (timeline.stringValue == vm.SelectedTimelineId) {
                        vm.Timeline = timeline;
                    }
                });
                Instruments = response.data.Instruments;
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
                    $("#LocationCompareDataHead1").empty();  // For Back button
                    $("#LocationCompareDataHead1").append('  <img src="http://analyticsv1.azurewebsites.net/images/icon/dollar-sign-and-piles-of-coins.png" alt=""><h3>TOP INSTRUMENTS</h3>');
                    bindMainChart(categories);
                }
            });
        }

        var bindMainChart = function (categories) {

            series = [];
            angular.forEach(Instruments, function (instrument, instrumentIndex) {
                var data = [];
                angular.forEach(categories, function (country, countryIndex) {
                    var countryData = $filter('filter')(instrumentData, { Country: country }, true);
                    var total = 0, length = 0;
                    angular.forEach(countryData, function (countryObj, countryObjIndex) {
                        var instruments = $filter('filter')(countryObj.InstrumentStatsModel, { InstrumentName: instrument }, true)[0];
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
            if ($("#locationInstrumentsContainer").highcharts() != undefined) {
                $("#locationInstrumentsContainer").highcharts().destroy();
            }
            $timeout(function () {

            Highcharts.chart("locationInstrumentsContainer", {
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
                                    selectedCountry = this.category;
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

        vm.drillDownChart1 = function (isBack) {
            vm.loadComplete = false; //Loader
            instrumentService.getAllInstruments(vm.SelectedTimelineId, selectedSeries, selectedCountry).then(function (response) {
                instrumentData = response.data.Data;
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

                vm.IsDrillDownVisible = true;
                vm.IsDrillDown2Visible = false;
            });

        }

        var bindDrillDownChart1 = function (categories) {
            series = [];
            var data = [];
            angular.forEach(categories, function (city, cityIndex) {
                var cityData = $filter('filter')(instrumentData, { City: city }, true);
                var total = 0, length = 0;
                angular.forEach(cityData, function (cityObj, cityObjIndex) {
                    var instrument = $filter('filter')(cityObj.InstrumentStatsModel, { InstrumentName: instrument }, true)[0];
                    if (instrument) {
                        total += instrument.Volume;
                    }
                    length += 1;
                });
                data.push(total / (length == 0 ? 1 : length));
            });
            series.push({
                name: selectedSeries,
                data: data
            });
            vm.loadComplete = true; //Loader
                        if ($("#locationInstrumentsContainer").highcharts() != undefined) {
                $("#locationInstrumentsContainer").highcharts().destroy();
            }
            $timeout(function () {
            Highcharts.chart("locationInstrumentsContainer", {
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
                                    vm.selectedCity = this.category;
                                    selectedCity = this.category;
                                    vm.drillDownChart2();
                                }
                            }
                        }
                    }
                },
                series: series
            }); }, 10);

            //$("#locationInstrumentsContainer").append("<button class='widget-back-btn' ng-click='vm.mainChart()'><i class='fa fa-arrow-left'></i>Back</button>");
            //$compile($("#locationInstrumentsContainer").contents())($scope);

            $("#LocationCompareDataHead1").empty();
            $("#LocationCompareDataHead1").append("<h3 ng-click='vm.mainChart()'><a prevent-default class='back-btn'><i class='fa fa-arrow-circle-left'></i>{{vm.selectedCountry}}</a></h3>");
            $compile($("#LocationCompareDataHead1").contents())($scope);
        }

        vm.drillDownChart2 = function () {
            vm.loadComplete = false; //Loader
                        instrumentService.getAllInstruments(vm.SelectedTimelineId, selectedSeries, selectedCountry, selectedCity).then(function (response) {
                                            instrumentData = response.data.Data;
                                            vm.SelectedAccounts = [], vm.Accounts = [];
                angular.forEach(response.data.Accounts, function (account, accountIndex) {
                    vm.Accounts.push({ id: account, label: account });
                    vm.SelectedAccounts.push({ id: account, label: account });
                });
                vm.IsDrillDownVisible = false;
                vm.IsDrillDown2Visible = true;

            });

        }

        var bindDrillDownChart2 = function (categories) {
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
            if ($("#locationInstrumentsContainer").highcharts() != undefined) {
                $("#locationInstrumentsContainer").highcharts().destroy();
            }
            $timeout(function () { 
            Highcharts.chart("locationInstrumentsContainer", {
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
                                    var objAccount = $filter('filter')(instrumentData, { Name: this.category }, true)[0];
                                    $location.path("/UserDetails/" + objAccount.AccountDetailId);
                                    //alert('Category: ' + this.category + ', value: ' + this.y);
                                    localStorage.setItem('selectedTimelineId', vm.SelectedTimelineId)
                                }
                            }
                        }
                    }
                },


                series: series
            }); }, 10);


            //$("#locationInstrumentsContainer").append("<button class='widget-back-btn' ng-click='vm.drillDownChart1(true)'><i class='fa fa-arrow-left'></i>Back</button>");
            //$compile($("#locationInstrumentsContainer").contents())($scope);
            $("#LocationCompareDataHead1").empty();
            $("#LocationCompareDataHead1").append("<h3 ng-click='vm.drillDownChart1(true)'><a prevent-default class='back-btn'><i class='fa fa-arrow-circle-left'></i>{{vm.selectedCity}}</a></h3>");
            $compile($("#LocationCompareDataHead1").contents())($scope);
        }

        var init = function () {
            vm.mainChart();
        }

        init();

    }],
    controllerAs: 'vm'
});