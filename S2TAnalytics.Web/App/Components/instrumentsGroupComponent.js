app.component("instrumentsByGroup", {
    templateUrl: serviceBase + "App/Views/Components/InstrumentsGroup.html",
    bindings: { name: '@' },
    controller: ['$scope', 'instrumentService', '$location', '$filter', '$compile', '$timeout', 'preselectedTimelineID',
        function ($scope, instrumentService, $location, $filter, $compile, $timeout, preselectedTimelineID) {
            var vm = this;
            var instrumentData = [], series = [], Instruments = [], selectedSeries = "", selectedGroup = "";
            vm.Groups = [];
            vm.Accounts = [];
            vm.IsDrillDownVisible = false;
            vm.SelectedTimelineId = preselectedTimelineID;
            vm.chartType = "column";
            vm.loadComplete = false; //Loader
            vm.SelectedGroups = [];

            var permissions = $scope.$parent.mainVM;
            vm.isPinnedUsers = $scope.$parent.mainVM.isPinnedUsers;
            vm.isExcludeUser = $scope.$parent.mainVM.isExcludeUser;
            vm.isViewDetail = $scope.$parent.mainVM.isViewDetail;
            vm.isCompare = $scope.$parent.mainVM.isCompare;
            vm.isExportData = $scope.$parent.mainVM.isExportData;
            vm.isEmbedWidget = $scope.$parent.mainVM.isEmbedWidget;
            vm.isApiWidget = $scope.$parent.mainVM.isApiWidget;
            vm.isAddReport = $scope.$parent.mainVM.isAddReport;

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
                    vm.WidgetId = response.data.WidgetId;
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
                        $("#GroupCompareDataHead1").append('  <img src="../images/icon/businessman.png" alt=""><h3>TOP INSTRUMENTS BY USER GROUPS</h3>');

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

                //$("#groupInstrumentsContainer").append("<button class='widget-back-btn' ng-click='vm.mainChart()'><i class='fa fa-arrow-left'></i>Back</button>");
                //$compile($("#groupInstrumentsContainer").contents())($scope);

                $("#GroupCompareDataHead1").empty();
                $("#GroupCompareDataHead1").append("<h3 ng-click='vm.mainChart()'><a prevent-default class='back-btn'><i class='fa fa-arrow-circle-left'></i>{{vm.selectedCountry}}</a></h3>");
                $compile($("#GroupCompareDataHead1").contents())($scope);
            }

            vm.ShowScript = function () {
                instrumentService.getWidgetPermission(vm.WidgetId).then(function (response) {
                    if (response.data.Success) {
                        vm.tags = response.data.Data != null ? response.data.Data.Domain : null;
                        
                        $('#divInstrumentsGroupScript').text('<script type="text/javascript" id="scriptInstrumentGroup" token="' + localStorage.getItem('accessToken') + '" src="http://analyticsv1.azurewebsites.net/App/Widgets/InstrumentsByGroup/render.js"></script><div class="block"><div class="container shadow-none"><div class="col-md-5 small-block balance-block" style="min-height: 438px;"> <instruments-by-group app="instrumentGroupApp"></instruments-by-group></div></div></div>');
                        $('#txtInstrumentsGroupScript').val('<script type="text/javascript" id="scriptInstrumentGroup" token="' + localStorage.getItem('accessToken') + '" src="http://analyticsv1.azurewebsites.net/App/Widgets/InstrumentsByGroup/render.js"></script><div class="block"><div class="container shadow-none"><div class="col-md-5 small-block balance-block" style="min-height: 438px;"> <instruments-by-group app="instrumentGroupApp"></instruments-by-group></div></div></div>');
                        $("#embedScriptIGPopup").modal('show');
                    }
                });
            }
            vm.EmbedAPI = function () {
                //api/Dashboard/GetInstrumentStatsByGroup?timelineId=" + timelineId + "&instrument=" + instrument + "&userGroup=" + userGroup

                $('#apiLinkTextIG').text(location.host + "/api/Dashboard/GetInstrumentStatsByGroup?timelineId=" + vm.SelectedTimelineId);
                $('#EmbedAPITokenIG').text("Bearer " +localStorage.getItem('accessToken'));
                $("#embedAPIPopupIG").modal('show');

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
                $('#txtInstrumentsGroupScript').css("display", "block");
                copyToClipboard($("#txtInstrumentsGroupScript")[0]);
                $('#txtInstrumentsGroupScript').css("display", "none");
                toastr.success("Copied");
            }

            var init = function () {
                vm.mainChart();
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