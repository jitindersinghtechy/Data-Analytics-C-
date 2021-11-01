app.component("comparisonData", {
    templateUrl: serviceBase + "App/Views/Components/ComparisonData.html",
    bindings: { name: '@' },
    controller: ['$scope', 'comparisonDataService', '$location', '$filter', '$compile', '$timeout', 'preselectedTimelineID',
        function ($scope, comparisonDataService, $location, $filter, $compile, $timeout, preselectedTimelineID) {
            var vm = this;
            var comparisonData = [], series = [], selectedSeries = "", selectedCountry = "", selectedCity = "";
            vm.Countries = [];
            vm.Cities = [];
            vm.Accounts = [];
            vm.SelectedCountries = [];
            vm.SelectedCities = [];
            vm.SelectedAccounts = [];
            vm.TimeLines = [];
            vm.SelectedTimelineId = preselectedTimelineID;
            vm.loadComplete = false; //Loader
            var permissions = $scope.$parent.mainVM;
            vm.isPinnedUsers = $scope.$parent.mainVM.isPinnedUsers;
            vm.isExcludeUser = $scope.$parent.mainVM.isExcludeUser;
            vm.isViewDetail = $scope.$parent.mainVM.isViewDetail;
            vm.isCompare = $scope.$parent.mainVM.isCompare;
            vm.isExportData = $scope.$parent.mainVM.isExportData;
            vm.isEmbedWidget = $scope.$parent.mainVM.isEmbedWidget;
            vm.isApiWidget = $scope.$parent.mainVM.isApiWidget;
            vm.isAddReport = $scope.$parent.mainVM.isAddReport;

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
                    vm.WidgetId = response.data.WidgetId;
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
                        $("#CompareDataHead1").append(' <img src="../images/icon/graph-line-screen.png" alt=""><h3>PERFORMANCE <!--BY LOCATION--></h3>');
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
                var dataWithCity = []
                var cityNames = [];
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
                    dataWithCity.push({ city: city, value: parseFloat((value / (length == 0 ? 1 : length)).toFixed(2)) });
                });
                var dataWithCitySorted = $filter('orderBy')(dataWithCity, '-value');
                angular.forEach(dataWithCitySorted, function (city, index) {
                    cityNames.push(city.city);
                    data.push(city.value)
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
                            //categories: categories
                            categories: cityNames
                        },
                        yAxis: {
                            //min: 0,
                            title: {
                                text: 'Percentage'
                            }
                        },
                        plotOptions: {
                            column: {
                                //    stacking: 'percent'
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
                comparisonDataService.getComparisonData(vm.SelectedTimelineId, selectedSeries, selectedCountry).then(function (response) {
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
                var dataWithAccount = [];
                var accountsName = [];
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
                    //data.push(value);
                    dataWithAccount.push({ account: account, value: value });
                });
                var dataWithAccountSorted = $filter('orderBy')(dataWithAccount, '-value');
                angular.forEach(dataWithAccountSorted, function (account, index) {
                    accountsName.push(account.account);
                    data.push(account.value)
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
                            //categories: categories
                            categories: accountsName
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
                                //stacking: 'percent'
                            },
                            series: {
                                borderColor: 'none',
                                point: {
                                    events: {
                                        click: function () {
                                            var objAccount = $filter('filter')(comparisonData, { Name: this.category }, true)[0];
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
                //$("#comparisonDataContainer").append("<button class='widget-back-btn' ng-click='vm.drillDownChart1(true)'><i class='fa fa-arrow-left'></i>Back</button>");
                //$compile($("#comparisonDataContainer").contents())($scope);    
                $("#CompareDataHead1").empty();
                $("#CompareDataHead1").append("<h3 ng-click='vm.drillDownChart1(true)'><a prevent-default class='back-btn'><i class='fa fa-arrow-circle-left'></i>{{vm.selectedCity}}</a></h3>");
                $compile($("#CompareDataHead1").contents())($scope);
            }

            vm.drillDownChart2 = function () {
                vm.loadComplete = false; //Loader
                comparisonDataService.getComparisonData(vm.SelectedTimelineId, selectedSeries, selectedCountry, selectedCity).then(function (response) {
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

            vm.ShowScript = function () {

                comparisonDataService.getWidgetPermission(vm.WidgetId).then(function (response) {
                    if (response.data.Success) {
                        vm.tags = response.data.Data != null ? response.data.Data.Domain : null;
                        $('#divComparisonScript').text('<script type="text/javascript" id="scriptComparison" token="' + localStorage.getItem('accessToken') + '" src="http://analyticsv1.azurewebsites.net/App/Widgets/Comparison/render.js"></script><div class="block"><div class="container shadow-none"><div class="col-md-5 small-block balance-block" style="min-height: 438px;"><comparison-data app="comparisonApp"></comparison-data></div></div></div>');
                        $('#txtComparisonScript').val('<script type="text/javascript" id="scriptComparison" token="' + localStorage.getItem('accessToken') + '" src="http://analyticsv1.azurewebsites.net/App/Widgets/Comparison/render.js"></script><div class="block"><div class="container shadow-none"><div class="col-md-5 small-block balance-block" style="min-height: 438px;"><comparison-data app="comparisonApp"></comparison-data></div></div></div>');
                        $("#embedScriptPopup").modal('show');
                    }
                });

            }


            vm.EmbedAPI = function () {
                // api/Dashboard/GetComparison?timelineId=" + timelineId + "&country=" + country + "&city=" + city
                $('#apiLinkTextComparision').text(location.host + "/api/Dashboard/GetComparison?timelineId=" + vm.SelectedTimelineId);
                $('#EmbedAPITokenComparision').text("Bearer " + localStorage.getItem('accessToken'));
                $("#embedAPIPopupComparision").modal('show');
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
                $('#txtComparisonScript').css("display", "block");
                copyToClipboard($("#txtComparisonScript")[0]);
                $('#txtComparisonScript').css("display", "none");
                toastr.success("Copied");
            }

            var init = function () {
                vm.mainChart();
            }

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
                    comparisonDataService.SaveEmbedWidgetPermission(vm.WidgetId, vm.Domain).then(function (response) {
                        if (response.data) {
                            toastr.success("Applied");
                        }
                    });
                }
            }
            init();
        }],
    controllerAs: 'vm'
});