app.component("instrumentsByLocation", {
    templateUrl: serviceBase + "App/Views/Components/InstrumentsLocation.html",
    bindings: { name: '@' },
    controller: ['$scope', 'instrumentService', '$location', '$filter', '$compile', '$timeout', 'preselectedTimelineID',
        function ($scope, instrumentService, $location, $filter, $compile, $timeout, preselectedTimelineID) {
            var vm = this;
            var instrumentData = [], series = [], Instruments = [], selectedSeries = "", selectedCountry = "", selectedCity = "";
            vm.Countries = [];
            vm.Cities = [];
            vm.Accounts = [];
            vm.SelectedCountries = [];
            vm.chartType = "column";
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

            vm.SelectedTimelineId = preselectedTimelineID;
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
                    vm.WidgetId = response.data.WidgetId;
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
                        $("#LocationCompareDataHead1").append('  <img src="../images/icon/dollar-sign-and-piles-of-coins.png" alt=""><h3>TOP INSTRUMENTS</h3>');
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
                    });
                }, 10);

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
                    });
                }, 10);


                //$("#locationInstrumentsContainer").append("<button class='widget-back-btn' ng-click='vm.drillDownChart1(true)'><i class='fa fa-arrow-left'></i>Back</button>");
                //$compile($("#locationInstrumentsContainer").contents())($scope);
                $("#LocationCompareDataHead1").empty();
                $("#LocationCompareDataHead1").append("<h3 ng-click='vm.drillDownChart1(true)'><a prevent-default class='back-btn'><i class='fa fa-arrow-circle-left'></i>{{vm.selectedCity}}</a></h3>");
                $compile($("#LocationCompareDataHead1").contents())($scope);
            }

            var init = function () {
                vm.mainChart();
            }

            vm.ShowScript = function () {
                instrumentService.getWidgetPermission(vm.WidgetId).then(function (response) {
                    if (response.data.Success) {
                        vm.tags = response.data.Data != null ? response.data.Data.Domain : null;
                        $('#txtInstrumentLocationScript').val('<script type="text/javascript" id="scriptInstrumentLocation" token="' + localStorage.getItem('accessToken') + '" src="http://analyticsv1.azurewebsites.net/App/Widgets/InstrumentsByLocation/render.js"></script><div class="block"><div class="container shadow-none"><div class="col-md-5 small-block balance-block" style="min-height: 438px;"><instruments-by-location app="instrumentLocationApp"></instruments-by-location></div></div></div>');
                        $('#divInstrumentLocationScript').text('<script type="text/javascript" id="scriptInstrumentLocation" token="' + localStorage.getItem('accessToken') + '" src="http://analyticsv1.azurewebsites.net/App/Widgets/InstrumentsByLocation/render.js"></script><div class="block"><div class="container shadow-none"><div class="col-md-5 small-block balance-block" style="min-height: 438px;"><instruments-by-location app="instrumentLocationApp"></instruments-by-location></div></div></div>');
                        $("#embedScriptILPopup").modal('show');
                    }
                });
            }
            vm.EmbedAPI = function () {
                //api/Dashboard/GetInstrumentStats?timelineId=" + timelineId + "&instrument=" + instrument + "&country=" + country + "&city=" + city
                $('#apiLinkTextIL').text(location.host + "/api/Dashboard/GetInstrumentStats?timelineId=" + vm.SelectedTimelineId);
                $('#EmbedAPITokenIL').text("Bearer " + localStorage.getItem('accessToken'));
                $("#embedAPIPopupIL").modal('show');
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
                $("#txtInstrumentLocationScript").css("display", "block");
                copyToClipboard($("#txtInstrumentLocationScript")[0]);
                $("#txtInstrumentLocationScript").css("display", "none");
                toastr.success("Copied");
            }

            init();

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
                    instrumentService.SaveEmbedWidgetPermission(vm.WidgetId, vm.Domain).then(function (response) {
                        if (response.data) {
                            toastr.success("Applied");
                        }
                    });
                }
            }

        }],
    controllerAs: 'vm'
});