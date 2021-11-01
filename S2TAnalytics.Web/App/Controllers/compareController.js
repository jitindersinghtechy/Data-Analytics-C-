'use strict'
app.controller('compareController', ['$scope', 'compareService', '$location', '$timeout', 'preselectedTimelineID', function ($scope, compareService, $location, $timeout, preselectedTimelineID) {
    var compareVm = this;
    compareVm.previousLocation = "";
    compareVm.CompareUserName = "" ;
    compareVm.loadComplete = false; //Loader
    compareVm.CompareData = [];
    compareVm.top5performer = [];
    compareVm.pineduser = [];
    compareVm.Timelines = [];
    // compareVm.SelectedTimeLine = "yDwjeVGRQ8E=";
    compareVm.preSelectedTimelineId = localStorage.getItem('selectedTimelineId') == null ? preselectedTimelineID : localStorage.getItem('selectedTimelineId');

    compareVm.CompareDataFromLocalStorage = [];
    GetCompareData();
    compareVm.bestPLSlider = {
        value: 0,
        options: {
            floor: 0,
            ceil: 10000,
            showSelectionBar: true,
            disabled: true,
            hideLimitLabels: true,
            getSelectionBarColor: function () {
                return '#99b841';
            },
            onEnd: function () {
                vm.changeGridByFilters();
            },
        }
    };

    compareVm.worstPLSlider = {
        value: 0,
        options: {
            floor: -10000,
            ceil: 0,
            showSelectionBar: true,
            disabled: true,
            hideLimitLabels: true,
            getSelectionBarColor: function () {
                return 'red';
            },
            onEnd: function () {
                vm.changeGridByFilters();
            },
        }
    };
    function GetCompareData() {
        compareVm.loadComplete = false; //Loader
        compareService.getCompareData().then(function (response) {
            if (response.data.response.Success) {
                var data = response.data.response.MultipleData;
                compareVm.top5performer = data.top5performers;
                compareVm.pineduser = data.pinedusers;
                compareVm.Timelines = data.timeLines;
                //compareVm.SelectedTimeLine = compareVm.Timelines[0];

                var getSelectedTimeline = data.timeLines.filter(function (timeline) {
                    if (timeline.stringValue == compareVm.preSelectedTimelineId) {
                        return timeline;
                    }
                })
                compareVm.SelectedTimeLine = getSelectedTimeline[0];
                

                for (var i = 0; i < 5; i++) {
                    compareVm.CompareData.push({ Name: "", ActiveSince: "", LastTradedDate: "", AvgTrade: "", TopPerformers: data.top5performers, PinUsers: data.pinedusers, SelectedTopPerformer: "", SelectedPinnedPerformer: "", AccountNumber: "", AccountNumberModel: "", TimeLineId: "", ROI: "", WINRate: "", DD: "", SharpRatio: "", Balance: "", BestPL: "", WorstPL: "" , AccountId :""});
                }
                CompareByAccountId();
            }
            else {
                alert("Error");
            }
            compareVm.loadComplete = true; //Loader
        })
    };

    function BindDataWithUI(index,data) //Common function...
    {
        var account = compareVm.CompareData[index];
        account.AccountId = data.AccountDetailId;
        account.ActiveSince = data.ActiveSince;
        account.AccountNumber = data.AccountNumber;
        account.Name = data.Name;
        account.LastTradedDate = data.LastTradedDate;
        account.AvgTrade = data.AvgTrade;
        account.BestPL = data.BestPL;
        account.WorstPL = data.WorstPL;
        account.NAV = data.NAV;
        account.ROI = data.ROI;
        account.WINRate = data.WINRate;
        account.DD = data.DD;
        account.SharpRatio = data.SharpRatio;
        account.AccountNumberModel = "";
        account.SelectedPinnedPerformer = "";
        account.SelectedTopPerformer = "";
    }


    compareVm.getClass = function (timeLine) {
        return (compareVm.SelectedTimeLine == timeLine) ? 'active-menu' : '';
    }

    compareVm.Compare = function (account, index) {
        compareVm.loadComplete = false; //Loader
        var AccountNo;

        if (account.AccountNumberModel != "")
            AccountNo = account.AccountNumberModel;
        else if (account.SelectedPinnedPerformer != "")
            AccountNo = account.SelectedPinnedPerformer;
        else if (account.SelectedTopPerformer != "")
            AccountNo = account.SelectedTopPerformer;
        else
            toastr.warning("Select One Option");
        compareService.getSingleAccount(AccountNo, compareVm.SelectedTimeLine.stringValue).then(function (response) {         
            if (response.data.response.Success) {
                var data = response.data.response.MultipleData.CompareModel;
                var flag = 0;
                var tempDataTest = localStorage.getItem('CompareData') == null ? [] : localStorage.getItem('CompareData');
                tempDataTest = angular.fromJson(tempDataTest);
                angular.forEach(tempDataTest, function (mydata, index) {
                   
                    if (mydata.value == true) {
                    
                        if (mydata.accountId == data.AccountDetailId)
                            flag = 1;
                    }
                })             
                if (flag == 0) {
                    if (index == 0 )
                        compareVm.CompareUserName = data.Name;
                    BindDataWithUI(index, data);

                    var CompareDataL_Var = localStorage.getItem('CompareData') == null ? [] : localStorage.getItem('CompareData');
                    CompareDataL_Var = angular.fromJson(CompareDataL_Var);
                    var accountId = data.AccountDetailId;
                    var value = true;

                    CompareDataL_Var.push({ accountId, value });
                    $timeout(function () {
                        bindCharts(response.data.response.MultipleData.performaceData, index);
                    }, 200);
                   
                   
                    localStorage.setItem('CompareData', JSON.stringify(CompareDataL_Var));
                }
                else
                {
                    toastr.warning("Already in compare Queue");
                }
            }
            else {
                alert("Error");
            }
        })
        compareVm.loadComplete = true; //Loader
    };

    function CompareByAccountId() {
        compareVm.loadComplete = false; //Loader
        compareVm.CompareDataFromLocalStorage = localStorage.getItem('CompareData') == null ? [] : localStorage.getItem('CompareData');
        compareVm.CompareDataFromLocalStorage = angular.fromJson(compareVm.CompareDataFromLocalStorage);
        var count = 0;
        localStorage.setItem('CompareData', JSON.stringify([]));
        angular.forEach(compareVm.CompareDataFromLocalStorage, function (data, index) {
            if (data.value == true) {
                count++;
            }
        })
        var myIndex = 0;
        if (count == 1) {
            myIndex++;
            var AccountNo;
            angular.forEach(compareVm.CompareDataFromLocalStorage, function (data, index) {
                if (data.value == true) {
                    AccountNo = data.accountId;
                }
            })
           
            if (compareVm.top5performer[0].AccountDetailId != AccountNo)
                AccountNo = compareVm.top5performer[0].AccountNumber;
            else
                AccountNo = compareVm.top5performer[1].AccountNumber;
            compareService.getSingleAccount(AccountNo, compareVm.SelectedTimeLine.stringValue).then(function (response) {
            
                if (response.data.response.Success) {
                    var data = response.data.response.MultipleData.CompareModel;
                    BindDataWithUI(0, data);
                    $timeout(function () {
                        bindCharts(response.data.response.MultipleData.performaceData, 0);
                    }, 200);
                    var CompareDataL_Var = localStorage.getItem('CompareData') == null ? [] : localStorage.getItem('CompareData');
                  
                    CompareDataL_Var = angular.fromJson(CompareDataL_Var);
                    var accountId = data.AccountDetailId;
                    var value = true;
                    CompareDataL_Var.push({ accountId, value });
                   
                    localStorage.setItem('CompareData', JSON.stringify(CompareDataL_Var));
              
                }
                else {
                    alert("Error");
                }
            })
        }
        var flag = 0;
        if (count <= 5) {
           
            angular.forEach(compareVm.CompareDataFromLocalStorage, function (data, index) {
                if (data.value == true) {
                    compareService.getSingleAccountByAccountID(data.accountId, compareVm.SelectedTimeLine.stringValue).then(function (response) {
                        if (response.data.response.Success) {
                            var data = response.data.response.MultipleData.CompareModel;
                            if (data != null) {
                                if (index == 0)
                                    compareVm.CompareUserName = data.Name;
                                compareVm.previousLocation = localStorage.getItem('previousLocation') == null ? '' : localStorage.getItem('previousLocation');
                                
                                BindDataWithUI(myIndex, data);
                              
                                var CompareDataL_Var = localStorage.getItem('CompareData') == null ? [] : localStorage.getItem('CompareData');
                                CompareDataL_Var = angular.fromJson(CompareDataL_Var);
                                var accountId = data.AccountDetailId;
                                var value = true;
                                CompareDataL_Var.push({ accountId, value });

                                localStorage.setItem('CompareData', JSON.stringify(CompareDataL_Var));
                                var i = myIndex;
                                $timeout(function () {
                                    bindCharts(response.data.response.MultipleData.performaceData, i);
                                }, 200);
                                myIndex++;
                            }
                        }
                        else {
                            alert("Error");
                        }
                    })
                }
            })
        }
        compareVm.loadComplete = true; //Loader

    };

    var bindCharts = function (performaceData, index) {
        var navData = []; var roiData = []; var winData = []; var ddData = []; var sharpeRatioData = [];
        angular.forEach(performaceData.accountStats, function (performace, Index) {
            navData.push(performace.NAV);
            roiData.push(performace.ROI);
            winData.push(performace.WINRate);
            ddData.push(performace.DD);
            //sharpeRatioData.push(performace.SharpRatio);
        });

        for (var i = 0; i < 4; i++) {
            var id = "";
            var series = [];
            if (i == 0) {
                id = "navGraph" + index;
                series.push({
                    name: "",
                    data: navData
                });
            } else if (i == 1) {
                id = "roiGraph" + index;
                series.push({
                    name: "",
                    data: roiData
                });
            } else if (i == 2) {
                id = "winGraph" + index;
                series.push({
                    name: "",
                    data: winData
                });
            } else if (i == 3) {
                id = "maxDDGraph" + index;
                series.push({
                    name: "",
                    data: ddData
                });
            }
            //else if (i == 4) {
            //    id = "sharpeRatioGraph" + index;
            //    series.push({
            //        name: "",
            //        data: sharpeRatioData
            //    });
            //}

            Highcharts.chart(id, {
                chart: {
                    type: 'spline'
                },
                legend: {
                    enabled: false
                },
                title: {
                    text: ''
                },
                xAxis: {
                    visible: false
                },
                yAxis: {
                    visible: false
                },
                tooltip: {
                    //enabled: false,
                    formatter: function() {
                        return '<b>' + Highcharts.numberFormat(this.y, 2) + '</b>';
                    }
                    //pointFormat: '<b>{point.y:,.2f}</b>',
                },
                credits: {
                    enabled: false
                },
                exporting: {
                    enabled: false
                },
                plotOptions: {
                    spline: {
                        marker: {
                            enabled: true
                        }
                    }
                },
                series: series
            });
        }
    }

    compareVm.winSlider = {
        value: 120,
        options: {
            floor: 0,
            ceil: 1000,
            showSelectionBar: true,
            hideLimitLabels: true,
            getSelectionBarColor: function () {
                return '#99b841';
            },
            onEnd: function () {
            },
        }
    };

    compareVm.previousLoaction = function () {
        var loc = localStorage.getItem('previousLocation') == null ? 'Dashboard' : localStorage.getItem('previousLocation');
        $location.path("/" + loc);
    };


    compareVm.topPerformersChange = function (index) {
        compareVm.CompareData[index].SelectedPinnedPerformer = "";
        compareVm.CompareData[index].AccountNumberModel = "";
    };

    compareVm.pinnedPerformerChange = function (index) {
        compareVm.CompareData[index].SelectedTopPerformer = "";
        compareVm.CompareData[index].AccountNumberModel = "";
    };

    compareVm.accountNumberModelChange = function (index) {
        compareVm.CompareData[index].SelectedTopPerformer = "";
        compareVm.CompareData[index].SelectedPinnedPerformer = "";
    };

    compareVm.removeAccount = function (account, index) {
        account.AccountNumber = "";
        var CompareData = compareVm.CompareDataFromLocalStorage;
        if (CompareData.length == 1 || index == 0)
            compareVm.CompareUserName = "";
        compareVm.CompareDataFromLocalStorage = [];
        angular.forEach(CompareData, function (data, Key) {
            if (data.accountId != account.AccountId) {
                var accountId = data.accountId;
                var value = data.value;
                compareVm.CompareDataFromLocalStorage.push({ accountId, value });
            }
        });
        localStorage.setItem('CompareData', JSON.stringify(compareVm.CompareDataFromLocalStorage));
    };

    compareVm.DataByTimeline = function () {
        CompareByAccountId();
    }
}])