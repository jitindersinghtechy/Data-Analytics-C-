app.component('userDetails', {
    templateUrl: serviceBase + "App/Views/Components/UserDetails.html",
    binding: { name: "@" },
    controller: ['$scope', 'userDetailsService', '$location', '$stateParams', '$timeout', '$compile', 'preselectedTimelineID',
        function ($scope, userDetailsService, $location, $stateParams, $timeout, $compile, preselectedTimelineID) {
        var vm = this;
        vm.accountId = $stateParams.accountId;
        vm.PerformerData = [];
        vm.PerformerTimeLines = [];
        vm.preSelectedTimelineId = localStorage.getItem('selectedTimelineId') == null ? preselectedTimelineID : localStorage.getItem('selectedTimelineId');
        var pageRecordModel = {
            Filters: {
                accountId: vm.accountId,
                TimeLineId: vm.preSelectedTimelineId
            },
        };

        vm.InstrumentalTradeChartData = "";
        vm.MaxInstrumentalTrade = {};

        var permissions = $scope.$parent.mainVM;
        vm.isPinnedUsers = $scope.$parent.mainVM.isPinnedUsers;
        vm.isExcludeUser = $scope.$parent.mainVM.isExcludeUser;
        vm.isViewDetail = $scope.$parent.mainVM.isViewDetail;
        vm.isCompare = $scope.$parent.mainVM.isCompare;
        vm.isExportData = $scope.$parent.mainVM.isExportData;
        vm.isEmbedWidget = $scope.$parent.mainVM.isEmbedWidget;
        vm.isAddReport = $scope.$parent.mainVM.isAddReport;

        vm.previousLocation = "";
        vm.loadComplete = false; //Loader
        //vm.userPinned = false;
        var getUserDetails = function () {
            vm.loadComplete = false; //Loader
            userDetailsService.getUserDetails(pageRecordModel).then(function (response) {
                var data = response.data.MultipleData;
                vm.PerformerData = data.accountDetails;
                vm.PerformerTimeLines = data.timeLines;
                //vm.PerformerTimeLine = data.timeLines[data.timeLines.length - 1];
                //vm.PerformerTimeLine = data.timeLines[0];
                var getSelectedTimeline = data.timeLines.filter(function (timeline) {
                    if (timeline.stringValue == vm.preSelectedTimelineId) {
                        return timeline;
                    }
                })
                vm.PerformerTimeLine = getSelectedTimeline[0];
                vm.loadComplete = true; //Loader
                $timeout(function () {
                    bindPerformanceChart(data.performaceData)
                    bindNavChart(data.performaceData, data.highestNavs);
                    vm.InstrumentalTradeChartData = data.instrumentalTradeChartData;
                    vm.MaxInstrumentalTrade = data.maxInstrumentalTrade;
                    bindInstrumentalTradeChart(data.instrumentalTradeChartData, data.maxInstrumentalTrade);

                }, 10);
                vm.userPinned = data.isUserPinned;
                vm.previousLocation = localStorage.getItem('previousLocation') == null ? '' : localStorage.getItem('previousLocation');
            });
        }
        var init = function () {
            getUserDetails();
        }

        vm.SelctInstrumentLimit = 'Top5';
        vm.getInstrumentsByRecords = function (records) {
            userDetailsService.getUserInstruments(records, pageRecordModel).then(function (response) {
                if (response.data.Success) {
                    var data = response.data.MultipleData;
                    bindInstrumentalTradeChart(data.instrumentalTradeChartData, data.maxInstrumentalTrade);
                }
            });
        }
        vm.getClass = function (records) {
            return (vm.SelctInstrumentLimit == records) ? 'active-menu' : '';
        }

        init();
        vm.performanceChartType = "column";
        vm.NAVChartType = "areaspline";
        vm.instrumentsChartType = "pie";
        var bindPerformanceChart = function (performaceData) {
            series = [];

            var roiData = [], ddData = []; winData = [];
            angular.forEach(performaceData.accountStats, function (performace, Index) {
                roiData.push(performace.ROI);
                ddData.push(performace.DD);
                winData.push(performace.WINRate);
            });
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
            Highcharts.chart("performanceData", {
                chart: {
                    type: vm.performanceChartType
                },
                title: {
                    text: ''
                },
                tooltip: {
                    pointFormat: '{point.series.name}  : <b> {point.y:,.2f}</b>',
                },
                xAxis: {
                    categories: performaceData.categories
                },
                yAxis: {
                    //min: 0,
                    title: {
                        text: ''
                    }
                },
                credits: {
                    enabled: false
                },
                plotOptions: {
                    areaspline: {
                        marker: {
                            enabled: false,
                            symbol: 'circle',
                            radius: 2,
                            states: {
                                hover: {
                                    enabled: true
                                }
                            }
                        },
                    },
                    line: {
                        marker: {
                            enabled: false,
                            symbol: 'circle',
                            radius: 2,
                            states: {
                                hover: {
                                    enabled: true
                                }
                            }
                        },
                    },
                    series: {
                        pointPadding: 0,
                        groupPadding: 0.1,
                        borderColor: 'none',
                    }
                },
                series: series
            });
        }

        vm.TotalBalance = 0;
        var bindNavChart = function (performaceData, highestNavs) {
            userNav = []; details = [];
            var balance = [], deposit = []; withdrawn = []; profitLoss = [];
            var staringBalanceForTime = 0;
            angular.forEach(performaceData.accountStats, function (performace, Index) {
                var totalBalance = performace.Deposit + performace.Withdrawn + performace.ProfitLoss;
                if (Index == 0) {
                    totalBalance += performace.StatringBalance;
                    
                }
                else {
                    totalBalance += staringBalanceForTime;
                }
                staringBalanceForTime = totalBalance;
                
                vm.TotalBalance = totalBalance;
                //userNav.push(totalBalance);
                balance.push(totalBalance);
                deposit.push(performace.Deposit);
                withdrawn.push(performace.Withdrawn);
                //profitLoss.push({ y: performace.ProfitLoss, color: performace.ProfitLoss > 0 ? "#93cc3e" : "#eb000e" });
                profitLoss.push(performace.ProfitLoss);
            });
            //angular.forEach(highestNavs.accountStats, function (performace, Index) {
            //    highestNav.push(performace.NAV);
            //});
            //series.push({
            //    //name: "Highest Balance",
            //    name: "Highest Equity",
            //    data: highestNav
            //});
            userNav.push({
                name: "User Equity",
                data: balance,
                tooltip: {
                    valueDecimals: 2
                }
            });
            userNav.push({
                name: "Deposit",
                data: deposit,
                tooltip: {
                    valueDecimals: 2
                }
            });
            userNav.push({
                name: "Withdrawn",
                data: withdrawn,
                tooltip: {
                    valueDecimals: 2
                }
            });
            userNav.push({
                name: "P/L",
                data: profitLoss,
                threshold: 0,
                negativeColor: '#ec2e2e',
                color: '#2fc32f',
                //negativeColor: '#eb000e',
                //color: '#93cc3e',
                tooltip: {
                    valueDecimals: 2
                }
            });
            //details.push({
            //    name: "Balance",
            //    data: balance
            //});
            //details.push({
            //    name: "Deposit",
            //    data: deposit
            //});
            //details.push({
            //    name: "Withdrawn",
            //    data: withdrawn
            //});
            //details.push({
            //    name: "ProfitLoss",
            //    data: profitLoss
            //});

            //Highcharts.chart('NavChart', {
            //    title: {
            //        text: ''
            //    },
            //    xAxis: {
            //        categories: performaceData.categories
            //    },
            //    labels: {

            //    },
            //    yAxis: {
            //        title: {
            //            text: ''
            //        }
            //    },
            //    credits: {
            //        enabled: false
            //    },
            //    plotOptions: {
            //        //column: {
            //        //    stacking: "normal"
            //        //},
            //        series: {
            //            pointPadding: 0,
            //            groupPadding: 0.1,
            //            borderColor: 'none',
            //        }
            //    },
            //    series: [{
            //        type: 'areaspline',
            //        name: 'Equity',
            //        data: userNav,
            //        color: '#878b69',
            //    }, {
            //        type: 'column',
            //        name: 'Deposit',
            //        data: deposit,
            //        color: '#6191d2',
            //    }, {
            //        type: 'column',
            //        name: 'Withdrawn',
            //        data: withdrawn,
            //        color: '#f8d556',
            //    }, {
            //        type: 'column',
            //        name: 'Balance',
            //        data: balance,
            //        color: '#e8a978',
            //    }, {
            //        type: 'column',
            //        name: 'Profit/Loss',
            //        data: profitLoss,
            //    }, ]
            //});
            Highcharts.chart("NavChart", {
                chart: {
                    type: vm.NAVChartType
                },
                title: {
                    text: ''
                },
                tooltip: {
                    pointFormat: '{point.series.name}  :  <b>$ {point.y:,.2f}</b>',
                },
                xAxis: {
                    categories: performaceData.categories
                },
                yAxis: {
                    title: {
                        text: ''
                    },
                },
                credits: {
                    enabled: false
                },
                plotOptions: {
                    areaspline: {
                        //pointStart: 1940,
                        marker: {
                            enabled: false,
                            symbol: 'circle',
                            radius: 2,
                            states: {
                                hover: {
                                    enabled: true
                                }
                            }
                        },
                    },
                    line: {
                        //pointStart: 1940,
                        marker: {
                            enabled: false,
                            symbol: 'circle',
                            radius: 2,
                            states: {
                                hover: {
                                    enabled: true
                                }
                            }
                        },
                    },
                    series: {
                        pointPadding: 0,
                        groupPadding: 0.1,
                        borderColor: 'none',
                    }
                },
                series: userNav
            });
        }
        //  <!------------------------------------------------------------------------------>




        var bindInstrumentalTradeChart = function (instrumentalTradeChartData, maxInstrumentalTrade) {
            series = [];
            vm.IsDrillDownVisible = false;
            $("#InstrumentsGraphHeader").empty();  // For Back button
            $("#InstrumentsGraphHeader").append(' <img src="/Images/imgs/nav-graph.png" alt=""><h2>INSTRUMENTS</h2>');

            angular.forEach(instrumentalTradeChartData, function (value, Name) {
                series.push([Name, value]);
            });
            Highcharts.chart('InstrumentalTradeChart', {
                chart: {
                    plotBackgroundColor: null,
                    plotBorderWidth: 0,
                    plotShadow: false
                },
                title: {
                    text: '',
                    //text: '<span style="font-size:30px; font-weight:bold;">' + maxInstrumentalTrade.Volume + ' <br/> ' + maxInstrumentalTrade.InstrumentName + '</span>',
                    align: 'center',
                    verticalAlign: 'middle',
                    y: -35
                },
                tooltip: {
                    //pointFormat: '<b>{point.percentage:.2f}%</b>'
                    pointFormat: '<b>Volume</b>:{point.y:,.2f}',
                },
                credits: {
                    enabled: false
                },
                legend: {
                    //align: 'right',
                    //verticalAlign: 'middle',
                    //width: 380,
                    itemWidth: 75,
                    //y: 10
                },
                plotOptions: {
                    areaspline: {
                        marker: {
                            enabled: false,
                            symbol: 'circle',
                            radius: 2,
                            states: {
                                hover: {
                                    enabled: true
                                }
                            }
                        },
                    },
                    line: {
                        marker: {
                            enabled: false,
                            symbol: 'circle',
                            radius: 2,
                            states: {
                                hover: {
                                    enabled: true
                                }
                            }
                        },
                    },
                    pie: {
                        //borderColor: 'none',
                        allowPointSelect: true,
                        cursor: 'pointer',
                        showInLegend: true,
                        dataLabels: {
                            //format: '{x}',
                            //enabled: true,
                            enabled: false,
                            distance: -50,
                            style: {
                                fontWeight: 'bold',
                                color: 'black'
                            }

                        },
                        //center: ['50%', '75%']
                    }
                },
                series: [{
                    type: vm.instrumentsChartType,
                    name: 'SGD',
                    innerSize: '0%',
                    data: series,
                    point: {
                        events: {
                            click: function () {
                                vm.selectedInstrumentName = this.name;
                                vm.BindProfitLoss();
                            }
                        }
                    }

                    //data: [
                    //    ['Firefox', 10.38],
                    //    ['IE', 56.33],
                    //    ['Chrome', 24.03],
                    //    ['Safari', 4.77],
                    //    ['Opera', 0.91],
                    //    {
                    //        name: 'Proprietary or Undetectable',
                    //        y: 0.2,
                    //        dataLabels: {
                    //            enabled: false
                    //        }
                    //    }
                    //]
                }]
            });
        }

        vm.BindProfitLoss = function () {
            series = [];
            userDetailsService.getWinLossData(vm.selectedInstrumentName, vm.preSelectedTimelineId, vm.accountId).then(function (response) {
                var data1 = response.data.Profit;
                series.push(['Profit', data1]);
                var data2 = response.data.Loss;
                series.push(['Loss', data2]);
                bindDrillDownInstrumentsGraph(series);
            });

        }

        var bindDrillDownInstrumentsGraph = function (series) {
            $timeout(function () {
                Highcharts.chart('InstrumentalTradeChart', {
                    colors: ['#93cc3e', '#eb000e'],
                    chart: {
                        plotBackgroundColor: null,
                        plotBorderWidth: 0,
                        plotShadow: false
                    },
                    title: {
                        text: '',
                        align: 'center',
                        verticalAlign: 'middle',
                        y: -35
                    },

                    tooltip: {
                        pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'

                    },
                    credits: {
                        enabled: false
                    },
                    legend: {
                        itemWidth: 75,
                    },
                    plotOptions: {
                        pie: {
                            //borderColor: 'none',
                            allowPointSelect: true,
                            cursor: 'pointer',
                            showInLegend: true,
                            dataLabels: {
                                //format: '{x}',
                                //enabled: true,
                                enabled: false,
                                distance: -50,
                                style: {
                                    fontWeight: 'bold',
                                    color: 'black'
                                }
                            },
                            //center: ['50%', '75%']
                        }
                    },
                    series: [{
                        type: vm.instrumentsChartType,
                        name: 'SGD',
                        innerSize: '0%',
                        colorByPoint: true,
                        data: series
                    }]
                });
            }, 10);
            $("#InstrumentsGraphHeader").empty();
            $("#InstrumentsGraphHeader").append("<h3 ng-click='vm.bindChartInstrumental()'><a prevent-default class='back-btn'><i class='fa fa-arrow-circle-left'></i>{{vm.selectedInstrumentName}}</a></h3>");
            $compile($("#InstrumentsGraphHeader").contents())($scope);
        }

        vm.bindChartInstrumental = function () {
            vm.SelctInstrumentLimit = 'Top5';
            bindInstrumentalTradeChart(vm.InstrumentalTradeChartData, vm.MaxInstrumentalTrade);
        }

        //var bindDrillDownInstrumentsGraph = function () {
        //    series = [];
        //    series.push({
        //        name: 'Win',
        //        data: 40
        //    });
        //    series.push({
        //        name: 'Loss',
        //        data: 60
        //    });
        //    $timeout(function () {
        //        Highcharts.chart('InstrumentalTradeChart', {
        //            chart: {
        //                plotBackgroundColor: null,
        //                plotBorderWidth: 0,
        //                plotShadow: false
        //            },
        //            title: {
        //                text: '',
        //                align: 'center',
        //                verticalAlign: 'middle',
        //                y: -35
        //            },
        //            tooltip: {
        //                pointFormat: '<b>Volume</b>:{point.y:,.2f}',
        //            },
        //            credits: {
        //                enabled: false
        //            },
        //            legend: {
        //                //align: 'right',
        //                //verticalAlign: 'middle',
        //                //width: 380,
        //                itemWidth: 75,
        //                //y: 10
        //            },
        //            plotOptions: {
        //                pie: {
        //                    //borderColor: 'none',
        //                    allowPointSelect: true,
        //                    cursor: 'pointer',
        //                    showInLegend: true,
        //                    dataLabels: {
        //                        //format: '{x}',
        //                        //enabled: true,
        //                        enabled: false,
        //                        distance: -50,
        //                        style: {
        //                            fontWeight: 'bold',
        //                            color: 'black'
        //                        }
        //                    },
        //                    //center: ['50%', '75%']
        //                }
        //            },
        //            series: [{
        //                type: vm.instrumentsChartType,
        //                name: 'SGD',
        //                innerSize: '0%',
        //                data: series
        //                //data: [
        //                //    ['Firefox', 10.38],
        //                //    ['IE', 56.33],
        //                //    ['Chrome', 24.03],
        //                //    ['Safari', 4.77],
        //                //    ['Opera', 0.91],
        //                //    {
        //                //        name: 'Proprietary or Undetectable',
        //                //        y: 0.2,
        //                //        dataLabels: {
        //                //            enabled: false
        //                //        }
        //                //    }
        //                //]
        //            }]
        //        });
        //    }, 10);
        //    $("#InstrumentsGraphHeader").empty();
        //    $("#InstrumentsGraphHeader").append("<h3 ng-click='vm.bindInstrumentalTradeChart()'><a prevent-default class='back-btn'><i class='fa fa-arrow-circle-left'></i>{{vm.selectedCountry}}</a></h3>");
        //}


        vm.getAccountStatsValueByTimeLine = function (column, accountDetail) {
            var accountStat = [];
            angular.forEach(accountDetail.AccountStats, function (value, index) {
                if (value.TimeLineId == vm.PerformerTimeLine.stringValue) {
                    accountStat = value;
                }
            });
            return accountStat[column];
        }
        vm.updatePinnedUsers = function () {
            var selectedAccountDetailsIds = [vm.PerformerData.AccountDetailId];
            userDetailsService.updatePinnedUsers(selectedAccountDetailsIds).then(function (response) {
                if (response.data.Success) {
                    toastr.success(response.data.Message);
                    vm.userPinned = true;
                }
                else {
                    toastr.warning(response.data.Message);
                }
            });
        }
        vm.changePerformersDataByTimelineId = function () {
            if (vm.PerformerTimeLine.stringValue != undefined) {
                pageRecordModel.Filters.TimeLineId = vm.PerformerTimeLine.stringValue;
            }
            else if (pageRecordModel.Filters.hasOwnProperty('TimeLineId')) {
                delete pageRecordModel.Filters['TimeLineId'];
            }
            vm.loadComplete = false; //Loader
            userDetailsService.getUserDetails(pageRecordModel).then(function (response) {
                vm.loadComplete = true; //Loader
                var data = response.data.MultipleData;
                vm.SelctInstrumentLimit = 'Top5';

                $timeout(function () {
                    bindPerformanceChart(data.performaceData)
                    bindNavChart(data.performaceData, data.highestNavs);
                    bindInstrumentalTradeChart(data.instrumentalTradeChartData, data.maxInstrumentalTrade);

                }, 10);
            });
        }
        vm.changePerformanceChartType = function () {
            var chart = $("#performanceData").highcharts();
            $.each(chart.series, function (key, series) {
                series.update(
                    { type: vm.performanceChartType },
                    false
                );
            });
            chart.redraw();
        }
        vm.changeNAVChartType = function () {
            var chart = $("#NavChart").highcharts();
            $.each(chart.series, function (key, series) {
                series.update(
                    { type: vm.NAVChartType },
                    false
                );
            });
            chart.redraw();
        }
        vm.changeInstrumentsChartType = function () {
            var chart = $("#InstrumentalTradeChart").highcharts();
            $.each(chart.series, function (key, series) {
                series.update(
                    { type: vm.instrumentsChartType },
                    false
                );
            });
            chart.redraw();
        }
        vm.closeUserDetails = function () {

        }
        vm.updateCompareUsers = function (accountDetailId) {  // Storing Data in local storgae...
            var CompareDataFromLocalStorage = [];
            //  CompareDataFromLocalStorage = angular.fromJson(CompareDataFromLocalStorage);
            var accountId = accountDetailId;
            var value = true;
            CompareDataFromLocalStorage.push({ accountId, value });
            localStorage.setItem('CompareData', JSON.stringify(CompareDataFromLocalStorage));
            localStorage.setItem('previousLocation', 'Dashboard');
            localStorage.setItem('selectedTimelineId', vm.PerformerTimeLine.stringValue)
            $location.path("/Compare");
        }
        vm.unpinUser = function (accountDetailId) {
            userDetailsService.unpinUser(accountDetailId).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    toastr.success(response.data.Message);
                    vm.userPinned = false;
                }
                else {
                    toastr.warning(response.data.Message);
                }
            });
        }
        vm.previousLoaction = function () {
            var loc = localStorage.getItem('previousLocation') == null ? '/Dashboard' : '/' + localStorage.getItem('previousLocation');
            $location.path(loc);
        };

        vm.getValueByColumnName = function (column, accountDetail) {
            var accountStat = [];
            angular.forEach(accountDetail.AccountStats, function (value, index) {
                if (value.TimeLineId == vm.PerformerTimeLine.stringValue) {
                    accountStat = value;
                }
            });
            var value = parseFloat(accountStat[column])
            if (value < 0) {
                return "-$" + Math.abs(value).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            }
            else {
                return "$" + value.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            }
        }
    }],
    controllerAs: 'vm',
})

















