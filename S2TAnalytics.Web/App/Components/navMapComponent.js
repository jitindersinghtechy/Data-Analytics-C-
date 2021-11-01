app.component("navMap", {
    templateUrl: serviceBase + "App/Views/Components/NavMapData.html",
    bindings: { name: '@' },
    //controller: ['$scope', 'navMapService', '$location', '$filter', '$compile', 'countryIso', function ($scope, navMapService, $location, $filter, $compile, countryIso) {
    controller: ['$scope', 'navMapService', '$location', '$filter', '$compile', '$timeout', 'preselectedTimelineID',
        function ($scope, navMapService, $location, $filter, $compile, $timeout, preselectedTimelineID) {
            var vm = this;
            var navData = [], series = [], selectedSeries = "", selectedCountry = "", selectedCity = "";
            vm.Countries = [];
            vm.Cities = [];
            vm.Accounts = [];
            vm.SelectedCountries = [];

            var permissions = $scope.$parent.mainVM;
            vm.isPinnedUsers = $scope.$parent.mainVM.isPinnedUsers;
            vm.isExcludeUser = $scope.$parent.mainVM.isExcludeUser;
            vm.isViewDetail = $scope.$parent.mainVM.isViewDetail;
            vm.isCompare = $scope.$parent.mainVM.isCompare;
            vm.isExportData = $scope.$parent.mainVM.isExportData;
            vm.isEmbedWidget = $scope.$parent.mainVM.isEmbedWidget;
            vm.isApiWidget = $scope.$parent.mainVM.isApiWidget;
            vm.isAddReport = $scope.$parent.mainVM.isAddReport;

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
            vm.SelectedTimelineId = preselectedTimelineID;
            vm.loadComplete = false; //Loader
            //vm.chartType = "Map";
            vm.chartType = "map";
            vm.SelectedCities = [];
            vm.drillDownlevel = 0;
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

            vm.click = function () {
                var chart = $("#navMapDataContainer").highcharts();
                if (vm.drillDownlevel == 0) {
                    var categories = [];
                    angular.forEach(vm.SelectedCountries, function (item, countryIndex) {
                        categories.push(item.id);
                    });
                    bindMainChart(categories);
                }
                else if (vm.drillDownlevel == 1) {
                    var categories = [];
                    angular.forEach(vm.SelectedCities, function (item, countryIndex) {
                        categories.push(item.id);
                    });
                    bindDrillDownChart1(categories);
                }
                else if (vm.drillDownlevel == 2) {
                    $.each(chart.series, function (key, series) {
                        series.update(
                            { type: vm.chartType },
                            false
                        );
                    });
                    chart.redraw();
                }
            }
            /// Logic for Timeline ///
            //if (!vm.IsDrillDownVisible && !vm.IsDrillDown2Visible) {
            //    vm.mainChart();
            //} else if (vm.IsDrillDownVisible) {
            //    vm.drillDownChart1();
            //} else if (vm.IsDrillDown2Visible) {
            //    vm.drillDownChart2();
            //}

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
                    vm.drillDownlevel = 1;
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
                navMapService.getNavMapData(vm.SelectedTimelineId).then(function (response) {
                    vm.WidgetId = response.data.WidgetId;
                    vm.TimeLines = response.data.Timelines;
                    angular.forEach(vm.TimeLines, function (timeline, index) {
                        if (timeline.stringValue == vm.SelectedTimelineId) {
                            vm.Timeline = timeline;
                        }
                    });
                    navData = response.data.Data;
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
                        $("#NavCompareDataHead1").empty();  // For Back button
                        //$("#NavCompareDataHead1").append('<img src="../images/icon/currency-value.png" alt=""><h3>Balance Graph</h3>');
                        $("#NavCompareDataHead1").append('<img src="../images/icon/currency-value.png" alt=""><h3>Equity</h3>');
                        vm.drillDownlevel = 0;

                        bindMainChart(categories);
                    }
                });
            }

            var bindMainChart = function (categories) {
                series = [];
                var data = [];
                if (vm.chartType != "map") {
                    angular.forEach(categories, function (country, countryIndex) {
                        var nav = 0;
                        var countryData = $filter('filter')(navData, { Country: country }, true);
                        angular.forEach(countryData, function (countryObj, countryObjIndex) {
                            nav += countryObj.AccountStats[0].NAV;
                        });

                        data.push(nav);
                    });
                    series.push({
                        //name: "Balance",
                        name: "Equity",
                        data: data
                    });
                    vm.loadComplete = true; //Loader
                    if ($("#navMapDataContainer").highcharts() != undefined) {
                        $("#navMapDataContainer").highcharts().destroy();
                    }
                    $timeout(function () {
                        Highcharts.chart("navMapDataContainer", {
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
                                title: ''
                                //min: 0,
                                //title: {
                                //    text: 'Percentage'
                                //}
                            },
                            plotOptions: {
                                column: {
                                    //stacking: 'percent'
                                    stacking: 'normal'
                                },
                                series: {
                                    borderColor: 'none',
                                    point: {
                                        events: {
                                            click: function () {
                                                vm.selectedCountry = this.category;
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
                else {
                    var nav = 0;
                    angular.forEach(categories, function (country, countryIndex) {
                        //vm.Countries.push({ id: country, label: country });
                        //vm.SelectedCountries.push({ id: country, label: country });
                        nav = 0;
                        var countryData = $filter('filter')(navData, { Country: country }, true);

                        var countryCode = "";
                        angular.forEach(countryData, function (countryObj, countryObjIndex) {
                            nav += countryObj.AccountStats[0].NAV;
                            countryCode = countryObj.CountryCode
                        });
                        //var countryCode = isoCountries[country.toLowerCase().replace(/\b[a-z]/g, function(letter) {
                        //    return letter.toUpperCase();
                        //})];

                        data.push({ code: countryCode, value: nav, name: country });

                    });
                    vm.loadComplete = true; //Loader
                    if ($("#navMapDataContainer").highcharts() != undefined) {
                        $("#navMapDataContainer").highcharts().destroy();
                    }
                    $timeout(function () {
                        //// Initiate the chart
                        $('#navMapDataContainer').highcharts('Map', {

                            title: {
                                text: ' '
                            },
                            tooltip: {
                                pointFormat: '{point.name}, {point.parentState}<br>' +
                                    'Equity: {point.value:.2f}'
                            },
                            mapNavigation: {
                                enabled: true,
                                //buttonOptions: {
                                //    verticalAlign: 'bottom'
                                //}
                            },

                            //mapNavigation: {
                            //    enabled: true,
                            //    buttonOptions: {
                            //        verticalAlign: 'bottom'
                            //    }
                            //},

                            //colorAxis: {
                            //    min: 1,
                            //    max: 5000,
                            //    type: 'logarithmic'
                            //},

                            series: [{
                                data: data,
                                mapData: Highcharts.maps.world,
                                joinBy: 'code',
                                name: 'Countries',
                                states: {
                                    hover: {
                                        color: '#BADA55'
                                    }
                                },
                                dataLabels: {
                                    enabled: true,
                                    format: '{point.name}',
                                    style: {
                                        color: '#616060'
                                    }
                                },
                                //tooltip: {
                                //    valueSuffix: '/km²'
                                //}
                            }],
                            plotOptions: {
                                series: {
                                    point: {
                                        events: {
                                            click: function () {
                                                vm.selectedCountry = this.name;
                                                selectedCountry = this.name;
                                                vm.drillDownChart1();
                                            }
                                        }
                                    }
                                }
                            }
                        });
                    }, 10);
                }
            }
            vm.drillDownChart1 = function (isBack) {
                vm.loadComplete = false; //Loader
                navMapService.getNavMapData(vm.SelectedTimelineId, "", selectedCountry).then(function (response) {
                    navData = response.data.Data;
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
                        vm.drillDownlevel = 1;

                        bindDrillDownChart1(categories);
                    }
                });
                vm.IsDrillDownVisible = true;
                vm.IsDrillDown2Visible = false;
            }

            var bindDrillDownChart1 = function (categories) {
                series = [];
                var data = [];
                var countryCode = "";
                if (vm.chartType != "map") {
                    angular.forEach(categories, function (city, cityIndex) {
                        var cityData = $filter('filter')(navData, { City: city }, true);
                        var nav = 0;
                        angular.forEach(cityData, function (cityObj, cityObjIndex) {
                            nav += cityObj.AccountStats[0].NAV;
                        });
                        data.push(nav);
                        //var stateCode = "nt";
                        //data.push({
                        //    "hc-key": "au-" + stateCode,
                        //    "value": nav
                        //});
                    });
                    series.push({
                        name: "Equity",
                        data: data
                    });
                    vm.loadComplete = true; //Loader
                    if ($("#navMapDataContainer").highcharts() != undefined) {
                        $("#navMapDataContainer").highcharts().destroy();
                    }
                    $timeout(function () {
                        Highcharts.chart("navMapDataContainer", {
                            credits: {
                                enabled: false
                            },
                            chart: {
                                type: vm.chartType
                                //    type: 'line'
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
                                title: '',
                                //min: 0,
                                //title: {
                                //    text: 'Percentage'
                                //}
                            },

                            plotOptions: {
                                column: {
                                    borderColor: 'none',
                                    stacking: 'normal'
                                },
                                series: {
                                    borderColor: 'none',
                                    point: {
                                        events: {
                                            click: function () {
                                                vm.selectedCity = this.category;
                                                selectedCity = this.category;
                                                selectedSeries = "NAV";
                                                vm.drillDownChart2();
                                            }
                                        }
                                    }
                                }
                            },
                            series: series
                        });
                    }, 10);
                }
                else {
                    angular.forEach(categories, function (city, cityIndex) {
                        if (city != "N/A") {
                            var cityData = $filter('filter')(navData, { City: city }, true);
                            var nav = 0; var cityLat = 0; var cityLong = 0;
                            angular.forEach(cityData, function (cityObj, cityObjIndex) {
                                nav += cityObj.AccountStats[0].NAV;
                                cityLat = cityObj.CityLat;
                                cityLong = cityObj.CityLong;
                                if (countryCode == "") {
                                    countryCode = cityObj.CountryCode.toLowerCase();
                                }
                            });
                            data.push({
                                name: city,
                                lat: cityLat,
                                lon: cityLong,
                                nav: nav

                            });
                        }
                    }); vm.loadComplete = true; //Loader
                    if ($("#navMapDataContainer").highcharts() != undefined) {
                        $("#navMapDataContainer").highcharts().destroy();
                    }
                    $timeout(function () {
                        Highcharts.mapChart('navMapDataContainer', {

                            title: {
                                text: ''
                            },

                            mapNavigation: {
                                enabled: true
                            },

                            tooltip: {
                                headerFormat: '',
                                pointFormat: '<b>{point.name}</b><br>Equity: {point.nav:.2f}'
                            },
                            plotOptions: {
                                column: {
                                    borderColor: 'none',
                                    stacking: 'normal'
                                },
                                series: {
                                    borderColor: 'none',
                                    point: {
                                        events: {
                                            click: function () {
                                                vm.selectedCity = this.name;
                                                selectedCity = this.name;
                                                selectedSeries = "NAV";
                                                vm.drillDownChart2();
                                            }
                                        }
                                    }
                                },
                            },

                            series: [{
                                // Use the gb-all map with no data as a basemap
                                mapData: Highcharts.maps['countries/' + countryCode + '/' + countryCode + '-all'],
                                name: 'Basemap',
                                //borderColor: '#A0A0A0',
                                borderColor: '#d1d1d1',
                                //nullColor: 'rgba(200, 200, 200, 0.3)',
                                nullColor: '#f7f7f7',
                                showInLegend: false,
                            }, {
                                name: 'Separators',
                                type: 'mapline',
                                data: Highcharts.geojson(Highcharts.maps['countries/' + countryCode + '/' + countryCode + '-all'], 'mapline'),
                                //color: '#707070',
                                color: '#f7f7f7',
                                showInLegend: false,
                                enableMouseTracking: false
                            }, {
                                // Specify points using lat/lon
                                //type: 'mappoint',
                                //name: 'Cities',
                                //color: Highcharts.getOptions().colors[1],
                                //data: data,
                                type: 'mapbubble',
                                dataLabels: {
                                    enabled: true,
                                    format: '{point.name}',
                                    style: {
                                        color: '#616060'
                                    }
                                },
                                name: 'Cities',
                                data: data,
                                maxSize: '15%',
                                color: Highcharts.getOptions().colors[0],
                            }]
                        });
                    }, 10);
                }

                //$('#navMapDataContainer').highcharts('Map', {
                //    title: {
                //        text: ''
                //    },
                //    mapNavigation: {
                //        enabled: true,
                //        //buttonOptions: {
                //        //    verticalAlign: 'bottom'
                //        //}
                //    },
                //    colorAxis: {
                //        min: 0
                //    },
                //    plotOptions: {
                //        column: {
                //            borderColor: 'none',
                //            stacking: 'normal'
                //        },
                //        series: {
                //            borderColor: 'none',
                //            point: {
                //                events: {
                //                    click: function () {
                //                        selectedCity = this.category;
                //                        selectedSeries = "NAV";
                //                        vm.drillDownChart2();
                //                    }
                //                }
                //            }
                //        }
                //    },
                //    series: [{
                //        name: 'Basemap',
                //        mapData: map,
                //        borderColor: '#606060',
                //        nullColor: H.getOptions().colors[3],
                //        showInLegend: false
                //    }, {
                //        name: 'Separators',
                //        type: 'mapline',
                //        data: H.geojson(map, 'mapline'),
                //        color: '#101010',
                //        enableMouseTracking: false
                //    }]
                //    //series: [{
                //    //    data: data,
                //    //    type: 'mappoint',
                //    //    name: 'Cities',
                //    //    //mapData: Highcharts.maps['countries/au/au-all'],
                //    //    //joinBy: 'hc-key',
                //    //    //name: 'Random data',
                //    //    //states: {
                //    //    //    hover: {
                //    //    //        color: '#a4edba'
                //    //    //    }
                //    //    //},
                //    //    dataLabels: {
                //    //        enabled: true,
                //    //        format: '{point.name}'
                //    //    }
                //    //}]
                //});
                //var mapData =
                //angular.forEach(categories, function (city, cityIndex) {
                //    if (city != "N/A") {
                //        var cityData = $filter('filter')(navData, { City: city }, true);
                //        var nav = 0; var cityLat = 0; var cityLong = 0; var countryCode = "";
                //        angular.forEach(cityData, function (cityObj, cityObjIndex) {
                //            nav += cityObj.AccountStats[0].NAV;
                //            cityLat = cityObj.CityLat;
                //            cityLong = cityObj.CityLong;
                //            countryCode = cityObj.CountryCode.toLowerCase();
                //        });
                //        map = H.maps['countries/' + countryCode + '/' + countryCode + '-all'];
                //        data.push({
                //            name: city,
                //            lat: cityLat,
                //            lon: cityLong,
                //            value: nav
                //        });
                //        //data.push(nav);
                //        //var stateCode = "nt";
                //        //data.push({
                //        //    "hc-key": "au-" + stateCode,
                //        //    "value": nav
                //        //});
                //    }
                //});
                ////series.push({
                ////    name: "NAV",
                ////    data: data
                ////});
                //$('#navMapDataContainer').highcharts('Map', {
                //    title: {
                //        text: ''
                //    },
                //    mapNavigation: {
                //        enabled: true,
                //        //buttonOptions: {
                //        //    verticalAlign: 'bottom'
                //        //}
                //    },
                //    colorAxis: {
                //        min: 0
                //    },
                //    plotOptions: {
                //        column: {
                //            borderColor: 'none',
                //            stacking: 'normal'
                //        },
                //        series: {
                //            borderColor: 'none',
                //            point: {
                //                events: {
                //                    click: function () {
                //                        selectedCity = this.category;
                //                        selectedSeries = "NAV";
                //                        vm.drillDownChart2();
                //                    }
                //                }
                //            }
                //        }
                //    },
                //    series: [{
                //        name: 'Basemap',
                //        mapData: map,
                //        borderColor: '#606060',
                //        nullColor: H.getOptions().colors[3],
                //        showInLegend: false
                //    }, {
                //        name: 'Separators',
                //        type: 'mapline',
                //        data: H.geojson(map, 'mapline'),
                //        color: '#101010',
                //        enableMouseTracking: false
                //    }]
                //    //series: [{
                //    //    data: data,
                //    //    type: 'mappoint',
                //    //    name: 'Cities',
                //    //    //mapData: Highcharts.maps['countries/au/au-all'],
                //    //    //joinBy: 'hc-key',
                //    //    //name: 'Random data',
                //    //    //states: {
                //    //    //    hover: {
                //    //    //        color: '#a4edba'
                //    //    //    }
                //    //    //},
                //    //    dataLabels: {
                //    //        enabled: true,
                //    //        format: '{point.name}'
                //    //    }
                //    //}]
                //});
                //Highcharts.chart("navMapDataContainer", {
                //    credits: {
                //        enabled: false
                //    },
                //    chart: {
                //        type: vm.chartType
                //        //    type: 'line'
                //    },
                //    title: {
                //        text: ''
                //    },
                //    tooltip: {
                //        pointFormat: '{point.series.name}  :  <b> {point.y:,.2f}</b>',
                //    },
                //    xAxis: {
                //        categories: categories
                //    },
                //    yAxis: {
                //        title: '',
                //        //min: 0,
                //        //title: {
                //        //    text: 'Percentage'
                //        //}
                //    },
                //    plotOptions: {
                //        column: {
                //            borderColor: 'none',
                //            stacking: 'normal'
                //        },
                //        series: {
                //            borderColor: 'none',
                //            point: {
                //                events: {
                //                    click: function () {
                //                        selectedCity = this.category;
                //                        selectedSeries = "NAV";
                //                        vm.drillDownChart2();
                //                    }
                //                }
                //            }
                //        }
                //    },
                //    series: series
                //});
                //$("#navMapDataContainer").append("<button ng-click='vm.mainChart()'><i class='fa fa-arrow-left'></i>Back</button>");
                //$compile($("#navMapDataContainer").contents())($scope);
                $("#NavCompareDataHead1").empty();
                $("#NavCompareDataHead1").append("<h3 ng-click='vm.mainChart()'><a prevent-default class='back-btn'><i class='fa fa-arrow-circle-left'></i>{{vm.selectedCountry}}</a></h3>");
                $compile($("#NavCompareDataHead1").contents())($scope);
                //var data = [], navValues = [], nav = 0; lat = 0; long = 0, i = 0, j = 0;
                //angular.forEach(categories, function (city, cityIndex) {
                //    //vm.Countries.push({ id: country, label: country });
                //    //vm.SelectedCountries.push({ id: country, label: country });
                //    nav = 0;
                //    var cityData = $filter('filter')(navData, { City: city }, true);
                //    angular.forEach(cityData, function (cityObj, cityObjIndex) {
                //        nav += cityObj.AccountStats[0].NAV;
                //    });
                //    navValues.push(nav);
                //    $.ajax({
                //        url: "https://maps.googleapis.com/maps/api/geocode/json?address=" + encodeURIComponent(city),
                //        async: false,
                //        success: function (result) {
                //            lat = result.results[0].geometry.location.lat;
                //            long = result.results[0].geometry.location.lng;
                //            data.push({ nav: navValues[j], name: categories[j], capital: result.results[0].address_components[0].long_name, lat: lat, lon: long });
                //            if (i == j) {
                //                var H = Highcharts;
                //                var map = Highcharts.maps['countries/' + selectedCountry.toLowerCase() + '/' + selectedCountry.toLowerCase() + '-all'];
                //                $('#navMapDataContainer').highcharts('Map', {
                //                    title: {
                //                        text: ' '
                //                    },
                //                    tooltip: {
                //                        pointFormat: '{point.capital}, {point.parentState}<br>' +
                //                            'NAV: {point.nav}'
                //                    },
                //                    series: [{
                //                        name: 'Basemap',
                //                        mapData: map,
                //                        borderColor: '#606060',
                //                        nullColor: 'rgba(200, 200, 200, 0.2)',
                //                        showInLegend: false
                //                    }, {
                //                        type: 'mapbubble',
                //                        dataLabels: {
                //                            enabled: true,
                //                            format: '{point.capital}'
                //                        },
                //                        name: 'Cities',
                //                        data: data,
                //                        maxSize: '12%',
                //                        color: H.getOptions().colors[0]
                //                    }],
                //                    plotOptions: {
                //                        series: {
                //                            point: {
                //                                events: {
                //                                    click: function () {
                //                                        selectedCity = this.name;
                //                                        selectedSeries = "NAV";
                //                                        vm.drillDownChart2();
                //                                    }
                //                                }
                //                            }
                //                        }
                //                    }
                //                });
                //                $("#navMapDataContainer").append("<button ng-click='vm.mainChart()'><i class='fa fa-arrow-left'></i>Back</button>");
                //                $compile($("#navMapDataContainer").contents())($scope);
                //            }
                //            j++;
                //        }
                //    });
                //    i++;
                //});
                //if (categories.length == 0) {
                //    var H = Highcharts;
                //    var map = Highcharts.maps['countries/' + selectedCountry.toLowerCase() + '/' + selectedCountry.toLowerCase() + '-all'];
                //    $('#navMapDataContainer').highcharts('Map', {
                //        //title: {
                //        //    text: 'Highmaps lat/lon demo'
                //        //},
                //        tooltip: {
                //            pointFormat: '{point.capital}, {point.parentState}<br>' +
                //                'NAV: {point.nav}'
                //        },
                //        series: [{
                //            name: 'Basemap',
                //            mapData: map,
                //            borderColor: '#606060',
                //            nullColor: 'rgba(200, 200, 200, 0.2)',
                //            showInLegend: false
                //        }, {
                //            type: 'mapbubble',
                //            dataLabels: {
                //                enabled: true,
                //                format: '{point.capital}'
                //            },
                //            name: 'Cities',
                //            data: data,
                //            maxSize: '12%',
                //            color: H.getOptions().colors[0]
                //        }],
                //        plotOptions: {
                //            series: {
                //                point: {
                //                    events: {
                //                        click: function () {
                //                            selectedCity = this.name;
                //                            selectedSeries = "NAV";
                //                            vm.drillDownChart2();
                //                        }
                //                    }
                //                }
                //            }
                //        }
                //    });
                //    $("#navMapDataContainer").append("<button class='widget-back-btn' ng-click='vm.mainChart()'><i class='fa fa-arrow-left'></i>Back</button>");
                //    $compile($("#navMapDataContainer").contents())($scope);
                //}
                // Add series with state capital bubbles
            }

            vm.drillDownChart2 = function () {
                vm.loadComplete = false; //Loader
                navMapService.getNavMapData(vm.SelectedTimelineId, "", selectedCountry, selectedCity).then(function (response) {
                    navData = response.data.Data;
                    vm.SelectedAccounts = [], vm.Accounts = [];
                    angular.forEach(response.data.Accounts, function (account, accountIndex) {
                        vm.Accounts.push({ id: account, label: account });
                        vm.SelectedAccounts.push({ id: account, label: account });
                    });
                    vm.drillDownlevel = 2;
                    vm.chartType = 'column';
                    vm.IsDrillDownVisible = false;
                    vm.IsDrillDown2Visible = true;
                });

            }

            var bindDrillDownChart2 = function (categories) {
                series = [];
                var data = [];
                var dataWithAccount = [];
                var accountsName = [];
                angular.forEach(categories, function (account, cityIndex) {
                    var accountData = $filter('filter')(navData, { Name: account }, true);
                    var nav = 0;
                    angular.forEach(accountData, function (accountObj, accountObjIndex) {
                        if (accountObj.AccountStats[0]) {
                            nav += accountObj.AccountStats[0].NAV;
                        }
                    });
                    //data.push(nav);
                    dataWithAccount.push({ account: account, value: nav });

                });
                var dataWithAccountSorted = $filter('orderBy')(dataWithAccount, '-value');
                angular.forEach(dataWithAccountSorted, function (account, index) {
                    accountsName.push(account.account);
                    data.push(account.value)
                });
                series.push({
                    //name: selectedSeries,
                    name: "Equity",
                    data: data
                });
                vm.loadComplete = true; //Loader
                if ($("#navMapDataContainer").highcharts() != undefined) {
                    $("#navMapDataContainer").highcharts().destroy();
                }
                $timeout(function () {
                    Highcharts.chart("navMapDataContainer", {
                        credits: {
                            enabled: false
                        },
                        chart: {
                            type: vm.chartType
                            //type: 'line'
                        },
                        title: {
                            text: ''
                        },
                        tooltip: {
                            pointFormat: '{point.series.name}  :  <b> {point.y:,.2f}</b>',
                        },
                        xAxis: {
                            categories: accountsName
                            //categories: categories
                        },
                        yAxis: {
                            title: ''
                            //min: 0,
                            //title: {
                            //    text: 'Percentage'
                            //}
                        },

                        plotOptions: {
                            column: {
                                borderColor: 'none',
                                stacking: 'normal'
                            },
                            series: {
                                borderColor: 'none',
                                point: {
                                    events: {
                                        click: function () {
                                            var objAccount = $filter('filter')(navData, { Name: this.category }, true)[0];
                                            $location.path("/UserDetails/" + objAccount.AccountDetailId);
                                            //alert('Category: ' + this.category + ', value: ' + this.y);
                                            localStorage.setItem('selectedTimelineId', vm.SelectedTimelineId)
                                        }
                                    }
                                }
                            }
                        },


                        series: series
                    });
                }, 10);
                //var series = [];

                //var data = [];
                //angular.forEach(categories, function (account, cityIndex) {
                //    var accountData = $filter('filter')(navData, { Name: account }, true);
                //    var value = 0; length = 0;
                //    angular.forEach(accountData, function (accountObj, accountObjIndex) {

                //        if (accountObj.AccountStats[0]) {
                //            value = accountObj.AccountStats[0].NAV;
                //        }
                //    });

                //    data.push(value);

                //});

                //series.push({
                //    name: selectedSeries,
                //    data: data
                //});


                //Highcharts.chart("navMapDataContainer", {
                //    credits: {
                //        enabled: false
                //    },
                //    chart: {
                //        type: 'column'
                //    },
                //    title: {
                //        text: ''
                //    },
                //    xAxis: {
                //        categories: categories
                //    },
                //    yAxis: {
                //        min: 0,
                //        title: {
                //            text: ''
                //        }
                //    },

                //    plotOptions: {
                //        column: {
                //            stacking: 'normal'
                //        },
                //        series: {
                //            borderColor: 'none',
                //            point: {
                //                events: {
                //                    click: function () {


                //                        var objAccount = $filter('filter')(navData, { Name: this.category }, true)[0];
                //                        $location.path("/UserDetails/" + objAccount.AccountDetailId);


                //                        //alert('Category: ' + this.category + ', value: ' + this.y);
                //                    }
                //                }
                //            }
                //        }
                //    },


                //    series: series
                //});

                //$("#navMapDataContainer").append("<button class='widget-back-btn' ng-click='vm.drillDownChart1(true)'>Back</button>");
                //$compile($("#navMapDataContainer").contents())($scope);
                $("#NavCompareDataHead1").empty();
                $("#NavCompareDataHead1").append("<h3 ng-click='vm.drillDownChart1(true)'><a prevent-default class='back-btn'><i class='fa fa-arrow-circle-left'></i>{{vm.selectedCity}}</a></h3>");
                $compile($("#NavCompareDataHead1").contents())($scope);
            }

            var init = function () {
                vm.mainChart();
            }

            vm.ShowScript = function () {
                navMapService.getWidgetPermission(vm.WidgetId).then(function (response) {
                    if (response.data.Success) {
                        vm.tags = response.data.Data != null ? response.data.Data.Domain : null;
                        $('#txtNavMapScript').val('<script type="text/javascript" id="scriptNavMap" token="' + localStorage.getItem('accessToken') + '" src="http://analyticsv1.azurewebsites.net/App/Widgets/NavMap/render.js"></script><div class="block"><div class="container shadow-none"><div class="col-md-5 small-block balance-block" style="min-height: 438px;"><nav-map app="navMapApp"></nav-map></div></div></div>');
                        $('#divNavMapScript').text('<script type="text/javascript" id="scriptNavMap" token="' + localStorage.getItem('accessToken') + '" src="http://analyticsv1.azurewebsites.net/App/Widgets/NavMap/render.js"></script><div class="block"><div class="container shadow-none"><div class="col-md-5 small-block balance-block" style="min-height: 438px;"><nav-map app="navMapApp"></nav-map></div></div></div>');
                        $("#embedScriptNMPopup").modal('show');
                    }
                });
            }

            vm.EmbedAPI = function () {
                //api/Dashboard/GetNavMapData?timelineId=" + timelineId + "&userGroup=" + userGroup + "&country=" + country + "&city=" + city
                $('#apiLinkTextMap').text(location.host + "/api/Dashboard/GetNavMapData?timelineId=" + vm.SelectedTimelineId);
                $('#EmbedAPITokenMap').text("Bearer " + localStorage.getItem('accessToken'));
                $("#embedAPIPopupMap").modal('show');
            }
            function copyToClipboard(elem) {
                // create hidden text element, if it doesn't already exist
                var targetId = "_hiddenCopyText_";
                var isInput = elem.tagName === "INPUT" || elem.tagName === "TEXTAREA";
                var origSelectionStart, origSelectionEnd;
                if (isInput) {
                    // can just use the original source element for the selection and copy
                    target = elem;
                    origSelectionStart = elem.selectionStart;
                    origSelectionEnd = elem.selectionEnd;
                } else {
                    // must use a temporary form element for the selection and copy
                    target = document.getElementById(targetId);
                    if (!target) {
                        var target = document.createElement("textarea");
                        target.style.position = "absolute";
                        target.style.left = "-9999px";
                        target.style.top = "0";
                        target.id = targetId;
                        document.body.appendChild(target);
                    }
                    target.textContent = elem.textContent;
                }
                // select the content
                var currentFocus = document.activeElement;
                target.focus();
                target.setSelectionRange(0, target.value.length);

                // copy the selection
                var succeed;
                try {
                    succeed = document.execCommand("copy");
                } catch (e) {
                    succeed = false;
                }
                // restore original focus
                if (currentFocus && typeof currentFocus.focus === "function") {
                    currentFocus.focus();
                }

                if (isInput) {
                    // restore prior selection
                    elem.setSelectionRange(origSelectionStart, origSelectionEnd);
                } else {
                    // clear temporary content
                    target.textContent = "";
                }
                return succeed;
            }

            vm.CopyToClipboard = function (elementId) {
                $('#txtNavMapScript').css("display", "block");
                copyToClipboard($("#txtNavMapScript")[0]);
                $('#txtNavMapScript').css("display", "none");
                toastr.success("Copied");
            }

            init();

            var isoCountries = {
                'Afghanistan': 'AF',
                'Aland Islands': 'AX',
                'Albania': 'AL',
                'Algeria': 'DZ',
                'American Samoa': 'AS',
                'Andorra': 'AD',
                'Angola': 'AO',
                'Anguilla': 'AI',
                'Antarctica': 'AQ',
                'Antigua And Barbuda': 'AG',
                'Argentina': 'AR',
                'Armenia': 'AM',
                'Aruba': 'AW',
                'Australia': 'AU',
                'Austria': 'AT',
                'Azerbaijan': 'AZ',
                'Bahamas': 'BS',
                'Bahrain': 'BH',
                'Bangladesh': 'BD',
                'Barbados': 'BB',
                'Belarus': 'BY',
                'Belgium': 'BE',
                'Belize': 'BZ',
                'Benin': 'BJ',
                'Bermuda': 'BM',
                'Bhutan': 'BT',
                'Bolivia': 'BO',
                'Bosnia And Herzegovina': 'BA',
                'Botswana': 'BW',
                'Bouvet Island': 'BV',
                'Brazil': 'BR',
                'British Indian Ocean Territory': 'IO',
                'Brunei Darussalam': 'BN',
                'Bulgaria': 'BG',
                'Burkina Faso': 'BF',
                'Burundi': 'BI',
                'Cambodia': 'KH',
                'Cameroon': 'CM',
                'Canada': 'CA',
                'Cape Verde': 'CV',
                'Cayman Islands': 'KY',
                'Central African Republic': 'CF',
                'Chad': 'TD',
                'Chile': 'CL',
                'China': 'CN',
                'Christmas Island': 'CX',
                'Cocos (Keeling) Islands': 'CC',
                'Colombia': 'CO',
                'Comoros': 'KM',
                'Congo': 'CG',
                'Congo, Democratic Republic': 'CD',
                'Cook Islands': 'CK',
                'Costa Rica': 'CR',
                'Cote D\'Ivoire': 'CI',
                'Croatia': 'HR',
                'Cuba': 'CU',
                'Cyprus': 'CY',
                'Czech Republic': 'CZ',
                'Denmark': 'DK',
                'Djibouti': 'DJ',
                'Dominica': 'DM',
                'Dominican Republic': 'DO',
                'Ecuador': 'EC',
                'Egypt': 'EG',
                'El Salvador': 'SV',
                'Equatorial Guinea': 'GQ',
                'Eritrea': 'ER',
                'Estonia': 'EE',
                'Ethiopia': 'ET',
                'Falkland Islands': 'FK',
                'Faroe Islands': 'FO',
                'Fiji': 'FJ',
                'Finland': 'FI',
                'France': 'FR',
                'French Guiana': 'GF',
                'French Polynesia': 'PF',
                'French Southern Territories': 'TF',
                'Gabon': 'GA',
                'Gambia': 'GM',
                'Georgia': 'GE',
                'Germany': 'DE',
                'Ghana': 'GH',
                'Gibraltar': 'GI',
                'Greece': 'GR',
                'Greenland': 'GL',
                'Grenada': 'GD',
                'Guadeloupe': 'GP',
                'Guam': 'GU',
                'Guatemala': 'GT',
                'Guernsey': 'GG',
                'Guinea': 'GN',
                'Guinea-Bissau': 'GW',
                'Guyana': 'GY',
                'Haiti': 'HT',
                'Heard Island & Mcdonald Islands': 'HM',
                'Holy See (Vatican City State)': 'VA',
                'Honduras': 'HN',
                'Hong Kong': 'HK',
                'Hungary': 'HU',
                'Iceland': 'IS',
                'India': 'IN',
                'Indonesia': 'ID',
                'Iran, Islamic Republic Of': 'IR',
                'Iraq': 'IQ',
                'Ireland': 'IE',
                'Isle Of Man': 'IM',
                'Israel': 'IL',
                'Italy': 'IT',
                'Jamaica': 'JM',
                'Japan': 'JP',
                'Jersey': 'JE',
                'Jordan': 'JO',
                'Kazakhstan': 'KZ',
                'Kenya': 'KE',
                'Kiribati': 'KI',
                'Korea': 'KR',
                'Kuwait': 'KW',
                'Kyrgyzstan': 'KG',
                'Lao People\'s Democratic Republic': 'LA',
                'Latvia': 'LV',
                'Lebanon': 'LB',
                'Lesotho': 'LS',
                'Liberia': 'LR',
                'Libyan Arab Jamahiriya': 'LY',
                'Liechtenstein': 'LI',
                'Lithuania': 'LT',
                'Luxembourg': 'LU',
                'Macao': 'MO',
                'Macedonia': 'MK',
                'Madagascar': 'MG',
                'Malawi': 'MW',
                'Malaysia': 'MY',
                'Maldives': 'MV',
                'Mali': 'ML',
                'Malta': 'MT',
                'Marshall Islands': 'MH',
                'Martinique': 'MQ',
                'Mauritania': 'MR',
                'Mauritius': 'MU',
                'Mayotte': 'YT',
                'Mexico': 'MX',
                'Micronesia, Federated States Of': 'FM',
                'Moldova': 'MD',
                'Monaco': 'MC',
                'Mongolia': 'MN',
                'Montenegro': 'ME',
                'Montserrat': 'MS',
                'Morocco': 'MA',
                'Mozambique': 'MZ',
                'Myanmar': 'MM',
                'Namibia': 'NA',
                'Nauru': 'NR',
                'Nepal': 'NP',
                'Netherlands': 'NL',
                'Netherlands Antilles': 'AN',
                'New Caledonia': 'NC',
                'New Zealand': 'NZ',
                'Nicaragua': 'NI',
                'Niger': 'NE',
                'Nigeria': 'NG',
                'Niue': 'NU',
                'Norfolk Island': 'NF',
                'Northern Mariana Islands': 'MP',
                'Norway': 'NO',
                'Oman': 'OM',
                'Pakistan': 'PK',
                'Palau': 'PW',
                'Palestinian Territory, Occupied': 'PS',
                'Panama': 'PA',
                'Papua New Guinea': 'PG',
                'Paraguay': 'PY',
                'Peru': 'PE',
                'Philippines': 'PH',
                'Pitcairn': 'PN',
                'Poland': 'PL',
                'Portugal': 'PT',
                'Puerto Rico': 'PR',
                'Qatar': 'QA',
                'Reunion': 'RE',
                'Romania': 'RO',
                'Russian Federation': 'RU',
                'Rwanda': 'RW',
                'Saint Barthelemy': 'BL',
                'Saint Helena': 'SH',
                'Saint Kitts And Nevis': 'KN',
                'Saint Lucia': 'LC',
                'Saint Martin': 'MF',
                'Saint Pierre And Miquelon': 'PM',
                'Saint Vincent And Grenadines': 'VC',
                'Samoa': 'WS',
                'San Marino': 'SM',
                'Sao Tome And Principe': 'ST',
                'Saudi Arabia': 'SA',
                'Senegal': 'SN',
                'Serbia': 'RS',
                'Seychelles': 'SC',
                'Sierra Leone': 'SL',
                'Singapore': 'SG',
                'Slovakia': 'SK',
                'Slovenia': 'SI',
                'Solomon Islands': 'SB',
                'Somalia': 'SO',
                'South Africa': 'ZA',
                'South Georgia And Sandwich Isl.': 'GS',
                'Spain': 'ES',
                'Sri Lanka': 'LK',
                'Sudan': 'SD',
                'Suriname': 'SR',
                'Svalbard And Jan Mayen': 'SJ',
                'Swaziland': 'SZ',
                'Sweden': 'SE',
                'Switzerland': 'CH',
                'Syrian Arab Republic': 'SY',
                'Taiwan': 'TW',
                'Tajikistan': 'TJ',
                'Tanzania': 'TZ',
                'Thailand': 'TH',
                'Timor-Leste': 'TL',
                'Togo': 'TG',
                'Tokelau': 'TK',
                'Tonga': 'TO',
                'Trinidad And Tobago': 'TT',
                'Tunisia': 'TN',
                'Turkey': 'TR',
                'Turkmenistan': 'TM',
                'Turks And Caicos Islands': 'TC',
                'Tuvalu': 'TV',
                'Uganda': 'UG',
                'Ukraine': 'UA',
                'United Arab Emirates': 'AE',
                'United Kingdom': 'GB',
                'United States': 'US',
                'United States Outlying Islands': 'UM',
                'Uruguay': 'UY',
                'Uzbekistan': 'UZ',
                'Vanuatu': 'VU',
                'Venezuela': 'VE',
                'Vietnam': 'VN',
                'Virgin Islands, British': 'VG',
                'Virgin Islands, U.S.': 'VI',
                'Wallis And Futuna': 'WF',
                'Western Sahara': 'EH',
                'Yemen': 'YE',
                'Zambia': 'ZM',
                'Zimbabwe': 'ZW'
            };

            function validateDomain(tag) {
                var re = /^[a-zA-Z0-9][a-zA-Z0-9-]{1,61}[a-zA-Z0-9](?:\.[a-zA-Z]{2,})+$/;
                return re.test(tag.text);
            }

            vm.checkDomain = function (tag) {
                if (!validateDomain(tag)) {
                    toastr.warning("Domain not in correct format");
                    return false;
                }
                else {
                    return true;
                }
            }

            vm.SaveEmbedWidgetPermission = function () {
                vm.Domain = [];
                angular.forEach(vm.tags, function (tag, index) {
                    vm.Domain.push(tag.text);
                })
                if (vm.Domain.length > 0) {
                    navMapService.SaveEmbedWidgetPermission(vm.WidgetId, vm.Domain).then(function (response) {
                        if (response.data) {
                            toastr.success("Applied");
                        }
                    });
                }
            }

        }],
    controllerAs: 'vm'
});

