'use strict'
app.controller('configureController', ['$scope', 'configureService', '$interval', '$timeout', function ($scope, configureService, $interval, $timeout) {
    var vm = this;
    //vm.ReportUserDetails = {};
    vm.ReportUserDetails = {};
    vm.SelectedUserGroups = [];
    vm.SelectedDatasourceIds = [];
    vm.ReportUserDetails.UserGroups = [];
    vm.ReportUserDetails.DatasourceIds = [];
    vm.userRoles = [];
    vm.dataSourcesWithValues = [];
    vm.UserGroupsWithValues = [];
    vm.UserGroups = [];
    vm.dataSources = [];
    vm.ConfiguredDatasources = [];
    vm.sortOn = "";
    vm.orderBy = false;
    vm.lastSort = "";
    vm.sortOrder = "";
    vm.loadComplete = false;
    vm.TotalRecords = 0;
    vm.StartingRecordNumber = 1;
    vm.ToRecordNumber = 0;
    vm.PageSizes = [10, 50, 100, 500];
    vm.selectedPageSize = vm.PageSizes[0];
    vm.defaultROR = 8.5;
    vm.synchronizingData = false;

    var pageRecordModel = {
        PageNumber: 1,
        PageSize: vm.selectedPageSize,
        Filters: {
        },
        SortOrder: "asc",
        SortBy: "",

    };
    vm.UserGroupSettings = {
        showCheckAll: false,
        showUncheckAll: false,
        smartButtonMaxItems: 10,
        smartButtonTextConverter: function (itemText, originalItem) {
            return itemText;
        }
    }
    vm.UserGroupTexts = {
        buttonDefaultText: 'User Group(s)'
    };
    vm.DataSourceSettings = {
        showCheckAll: false,
        showUncheckAll: false,
        smartButtonMaxItems: 10,
        smartButtonTextConverter: function (itemText, originalItem) {
            return itemText;
        }
    }
    vm.DataSourceTexts = {
        buttonDefaultText: 'Data Source(s)'
    };
    vm.Proceed = function (index, datasource) {
        if (!datasource.IsProceed) {
            datasource.IsProceed = true;
            if (datasource.RateOfReturns == undefined || datasource.RateOfReturns.length == 0) {
                datasource.RateOfReturns = [
                        { DatasourceId: datasource.DatasourceId, TimelineId: 1, RateOfReturn: vm.defaultROR, label: 'Previous Full Year' },
                        { DatasourceId: datasource.DatasourceId, TimelineId: 2, RateOfReturn: vm.defaultROR, label: 'Previous Half Year' },
                        { DatasourceId: datasource.DatasourceId, TimelineId: 3, RateOfReturn: vm.defaultROR, label: 'Previous Quater' },
                        { DatasourceId: datasource.DatasourceId, TimelineId: 4, RateOfReturn: vm.defaultROR, label: 'Previous Month' },
                        { DatasourceId: datasource.DatasourceId, TimelineId: 6, RateOfReturn: vm.defaultROR, label: 'Previous Week' },
                        { DatasourceId: datasource.DatasourceId, TimelineId: 49, RateOfReturn: vm.defaultROR, label: 'Current Year' },
                        { DatasourceId: datasource.DatasourceId, TimelineId: 48, RateOfReturn: vm.defaultROR, label: 'Current Quater' },
                        { DatasourceId: datasource.DatasourceId, TimelineId: 47, RateOfReturn: vm.defaultROR, label: 'Current Week' },
                        { DatasourceId: datasource.DatasourceId, TimelineId: 46, RateOfReturn: vm.defaultROR, label: 'MTD' },
                        { DatasourceId: datasource.DatasourceId, TimelineId: 45, RateOfReturn: vm.defaultROR, label: 'Overall' },
                ];
            }
        } else if (!datasource.IsROR) {
            datasource.IsROR = true;
        } else if (!datasource.IsConnected) {
            saveDatasource(index, datasource);
        }
    }
    var getUsers = function () {
        configureService.getUsers().then(function (response) {
            if (response.data.Success) {
                var data = response.data.MultipleData;
                vm.UserGroupsWithValues = data.userGroups;
                vm.userRoles = data.userRoles;
                vm.ReportUserDetails.RoleID = vm.userRoles[0].stringValue;
                vm.dataSourcesWithValues = data.dataSources;
                vm.UserGroupsForAdd = [];
                vm.dataSourcesForAdd = [];
                angular.forEach(vm.UserGroupsWithValues, function (userGroup, index) {
                    vm.UserGroups.push({ id: userGroup, label: userGroup });
                    vm.UserGroupsForAdd.push({ value: userGroup, label: userGroup });
                });
                angular.forEach(vm.dataSourcesWithValues, function (dataSource, index) {
                    vm.dataSources.push({ id: dataSource.Id, label: dataSource.Name });
                    vm.dataSourcesForAdd.push({ value: dataSource.Id, label: dataSource.Name, selected: true })
                });
                $('#UserGroupMultiSelect').multiselect({
                    includeSelectAllOption: true,
                    selectAllText: 'All',
                    numberDisplayed: 50,
                    selectAllNumber: false,
                    disableIfEmpty: true,
                    disabledText: 'Select User Group(s)',
                    buttonText: function (options, select) {
                        if (this.disabledText.length > 0
                        && (this.disableIfEmpty || select.prop('disabled'))
                        && options.length == 0) {

                            return this.disabledText;
                        }
                        if (options.length === 0) {
                            return 'Select User Group(s)';
                        }
                        else {
                            var labels = [];
                            options.each(function (index, value) {
                                if (index < 1) {
                                    labels.push($(this).text());
                                }
                                if (index == 1) {
                                    labels.push("...");
                                }
                                //labels.push($(this).text());
                            });
                            return labels.join(' , ');
                        }
                    },

                    onChange: function (element, checked) {
                        if (checked === true) {
                            vm.ReportUserDetails.UserGroups.push(element.context.value);
                        }
                        else {
                            var index = vm.ReportUserDetails.UserGroups.indexOf(element.context);
                            vm.ReportUserDetails.UserGroups.splice(index, 1);
                        }
                    },
                    onSelectAll: function () {
                        angular.forEach(vm.UserGroupsForAdd, function (userGroup, index) {
                            vm.ReportUserDetails.UserGroups.push(userGroup.value);
                        });
                    },
                    onDeselectAll: function () {
                        vm.ReportUserDetails.UserGroups = [];
                    }
                });
                $('#UserGroupMultiSelect').multiselect('dataprovider', vm.UserGroupsForAdd);
                $('#DataSourceMultiSelect').multiselect({
                    includeSelectAllOption: true,
                    selectAllText: 'All',
                    numberDisplayed: 50,
                    selectAllNumber: false,
                    disableIfEmpty: true,
                    disabledText: 'Select Data Source(s)',
                    buttonText: function (options, select) {
                        if (this.disabledText.length > 0
                        && (this.disableIfEmpty || select.prop('disabled'))
                        && options.length == 0) {
                            return this.disabledText;
                        }
                        if (options.length === 0) {
                            return 'Select Data Source(s)';
                        }
                        else {
                            var labels = [];
                            options.each(function (index, value) {
                                if (index < 1) {
                                    labels.push($(this).text());
                                }
                                if (index == 1) {
                                    labels.push("...");
                                }
                                // labels.push($(this).text());
                            });
                            return labels.join(' , ');
                        }
                    },
                    //onInitialized: function (select, container) {
                    //    alert('Initialized.');
                    //    if (vm.dataSourcesWithValues.length == 1) {
                    //        vm.ReportUserDetails.DatasourceIds.push(vm.dataSources[0].id);
                    //        //vm.dataSources.push({ id: dataSource.Id, label: dataSource.Name });
                    //    }
                    //},
                    onChange: function (element, checked) {
                        if (checked === true) {
                            vm.ReportUserDetails.DatasourceIds.push(element.context.value);
                        }
                        else {
                            var index = vm.ReportUserDetails.DatasourceIds.indexOf(element.context);
                            vm.ReportUserDetails.DatasourceIds.splice(index, 1);
                        }
                    },
                    onSelectAll: function () {
                        angular.forEach(vm.dataSourcesForAdd, function (dataSource, index) {
                            vm.ReportUserDetails.DatasourceIds.push(dataSource.value);
                        });
                    },
                    onDeselectAll: function () {
                        vm.ReportUserDetails.DatasourceIds = [];
                    }

                });
                $('#DataSourceMultiSelect').multiselect('dataprovider', vm.dataSourcesForAdd);

            }
            //else {
            //    vm.NoPinnedUserFound = true;
            //    //toastr.warning(response.data.Message);
            //}
        });
    }
    //vm.onItemSelect = function (items) {
    //    angular.forEach(items, function (item, index) {
    //        if (item.id == "All") {
    //            angular.forEach(vm.UserGroups, function (userGroup, index) {
    //                userGroup.IsChecked = true;
    //            });
    //        }
    //        else {
    //            angular.forEach(vm.UserGroups, function (userGroup, index) {
    //                userGroup.IsChecked = false;
    //            });
    //        }
    //    });        
    //}
    var getDatasources = function () {
        configureService.getDatasources().then(function (response) {
            var inprocessdatasource = response.data.MultipleData.queueDatasource;
            angular.forEach(response.data.MultipleData.ConfiguredDatasources, function (item, index) {

                item.Datasources = response.data.MultipleData.DataSources;
                if (item.IsConnected) {
                    item.IsConfigured = true;
                    item.IsProceed = false;
                    item.IsROR = false;
                    item.editROR = false;
                } else {
                    item.IsConnected = false;
                    item.IsConfigured = false;
                    item.IsProceed = false;
                    item.IsROR = false;
                    item.editROR = true;
                }
                if (inprocessdatasource != null && item.Id == inprocessdatasource.DatasourceId && (inprocessdatasource.Status == 1 || inprocessdatasource.Status == 2)) {
                    item.InProcess = true;
                    item.IsButtonDisabled = true;
                    vm.DataSourceQueue = inprocessdatasource;
                    $timeout(function () {
                        $("#timer" + index).radialProgress("init", {
                            'size': 100,
                            'fill': 8
                        }).radialProgress("to", { 'perc': 100, 'time': 600000 });
                    }, 100);
                    vm.setSyncInterval = $interval(function () {
                        configureService.checkForSynronizatedDataSource(inprocessdatasource.Id).then(function (response1) {
                            if (response1.data.Success) {
                                vm.DataSourceQueue.Status = response1.data.Data.Status;
                                if (vm.DataSourceQueue.Status == 3 || vm.DataSourceQueue.Status == 5) {
                                    getUsers();
                                    item.InProcess = false;
                                    item.IsConfigured = true;
                                    item.IsConnected = true;
                                    $interval.cancel(vm.setSyncInterval);
                                    $(".preloader").hide();
                                    toastr.success("Datasource configured successfully.");
                                    localStorage.setItem("triggerNotification", true);
                                    localStorage.setItem("triggerDataSource", true);
                                    localStorage.setItem("triggerPlanChange", true);
                                    item.IsButtonDisabled = false;
                                    vm.synchronizingData = false;
                                }
                                if (vm.DataSourceQueue.Status == 4) {
                                    item.InProcess = false;
                                    item.IsFailed = true;
                                    $interval.cancel(vm.setSyncInterval);
                                    $(".preloader").hide();
                                    toastr.error("Synchronization Failed");
                                    localStorage.setItem("triggerNotification", true);
                                    item.IsButtonDisabled = false;
                                    vm.synchronizingData = false;
                                }
                            }
                        })
                    }, 1000);
                }
                vm.ConfiguredDatasources.push(item);
            });
            if (response.data.MultipleData.ConfiguredDatasources.length == 0) {
                vm.ConfiguredDatasources.push({ IsConfigured: false, IsProceed: false, IsROR: false, editROR: false, Datasources: response.data.MultipleData.DataSources });
                //vm.ConfiguredDatasources.push({ IsConfigured: false, IsProceed: false, Datasources: response.data.MultipleData.DataSources });
                //vm.ConfiguredDatasources.push({ IsConfigured: false, IsProceed: false, Datasources: response.data.MultipleData.DataSources });
            } else {
                for (var i = 0; i < response.data.MultipleData.ConfiguredDatasources.length - 1; i++) {
                    vm.ConfiguredDatasources.push({ IsConfigured: false, IsProceed: false, IsROR: false, editROR: false, Datasources: response.data.MultipleData.DataSources });
                }
            }
        });
    }
    vm.AddDataSource = function () {
        var allowNewDatasource = true;
        var NotConfigured = false;
        angular.forEach(vm.ConfiguredDatasources, function (item, index) {
            if (item.InProcess == true || item.IsConfigured == false) {
                allowNewDatasource = false;
            }
            if (item.IsConfigured == false) {
                NotConfigured = true;
            }
        })
        if (allowNewDatasource) {
            vm.ConfiguredDatasources.push({
                IsConfigured: false,
                IsProceed: false,
                IsROR: false,
                editROR: false,
                RateOfReturns: [],
                Datasources: vm.ConfiguredDatasources[0].Datasources
            });
        }
        else {
            if (!NotConfigured)
                toastr.warning("Already one datasource is synchronizing, please wait for it to complete.");
            else
                toastr.warning("There is already one datasource which is not configured, please configure that first.");
        }
    }
    vm.RemoveDataSource = function (index) {
        vm.ConfiguredDatasources.splice(index, 1);
    }
    vm.BackToFirstStep = function (index, datasource) {
        datasource.IsProceed = false;
        datasource.IsROR = false;
        datasource.editROR = false;
        datasource.IsConfigured = false;
        datasource.InProcess = false;
    }
    vm.BackToROR = function (index, datasource) {
        datasource.IsProceed = true;
        datasource.editROR = false;
        datasource.IsROR = false;
        datasource.IsConfigured = false;
        datasource.InProcess = false;
    }
    vm.changeROR = function (index, datasource) {
        if (!vm.synchronizingData) {
            BootstrapDialog.confirm({
                title: 'DISCONNECT DATASOURCE',
                message: 'To change Rate of returns you need to disconnect this datasource?',
                callback: function (result) {
                    // result will be true if button was click, while it will be false if users close the dialog directly.
                    if (result) {
                        var allowNewDatasource = true;
                        var NotConfigured = false;
                        angular.forEach(vm.ConfiguredDatasources, function (item, index) {
                            if (item.InProcess == true || item.IsConfigured == false) {
                                allowNewDatasource = false;
                            }
                            if (item.IsConfigured == false) {
                                NotConfigured = true;
                            }
                        })
                        if (allowNewDatasource) {
                        //if (datasource.InProcess) {
                        datasource.editROR = true;
                        configureService.disconnectDatasource(datasource).then(function (response) {
                            if (response.data.Success) {
                                localStorage.setItem("triggerNotification", true);
                                toastr.success(response.data.Message);
                                datasource.IsConnected = false;
                                datasource.IsConfigured = false;
                                datasource.InProcess = false;
                                datasource.IsProceed = false;
                                datasource.IsROR = false;
                            }
                        });
                        }
                        else {
                            if (!NotConfigured)
                                toastr.warning("Already one datasource is synchronizing, please wait for it to complete.");
                            else
                                toastr.warning("There is already one datasource which is not configured, please configure that first.");
                        }
                    }
                }
            });
        }
    }
    vm.updateROR = function (index, datasource) {
        if (!vm.synchronizingData) {
            vm.synchronizingData = true;
            datasource.InProcess = true;
            datasource.IsButtonDisabled = true;
            $(".test_connection" + index).append("<div class='preloader show-loder'><img src='../Images/loading-circle.svg' class='img' /></div>");
            configureService.updateDataTransformation(datasource).then(function (response) {
                if (response.data.Success) {


                    $(".preloader").hide();
                    vm.DataSourceQueue = response.data.MultipleData.dataSourceQueue;
                    vm.ConfiguredDatasources[index] = response.data.MultipleData.dataSources[index];
                    vm.ConfiguredDatasources[index].Datasources = response.data.MultipleData.dataSourcesEnum
                    vm.ConfiguredDatasources[index].InProcess = true;
                    toastr.info("Data Transformation started.");
                    localStorage.setItem("triggerNotification", true);
                    $timeout(function () {
                        $("#timer" + index).radialProgress("init", {
                            'size': 100,
                            'fill': 8
                        }).radialProgress("to", { 'perc': 100, 'time': 600000 });
                    }, 100);

                    vm.messageCount = 1;

                    vm.setMessageInterval = $interval(function () {
                        if (vm.messageCount == 1) {
                            toastr.success("<strong>Did you know?</strong><br/>You can add multiple report users with unique privileges.");
                            vm.messageCount = 2;
                        }
                        else if (vm.messageCount == 2) {
                            toastr.success("<strong>Did you know?</strong> <br/>You can configure multiple data sources to the platform.");
                            vm.messageCount = 3;
                        }
                        else if (vm.messageCount == 3) {
                            toastr.success("<strong>Did you know?</strong><br/>You can request us to configure your custom data source to the platform.");
                            $interval.cancel(vm.setMessageInterval);
                        }
                    }, 10000)


                    vm.setSyncInterval = $interval(function () {
                        configureService.checkForSynronizatedDataSource(vm.DataSourceQueue.Id).then(function (response1) {
                            if (response1.data.Success) {
                                vm.DataSourceQueue.Status = response1.data.Data.Status;



                                if (vm.DataSourceQueue.Status == 3) {
                                    getUsers();
                                    vm.ConfiguredDatasources[index].InProcess = false;
                                    $interval.cancel(vm.setSyncInterval);
                                    $(".preloader").hide();
                                    datasource.IsConfigured = true;
                                    datasource.IsProceed = false;
                                    datasource.editROR = false;
                                    datasource.IsButtonDisabled = false;

                                    vm.ConfiguredDatasources[index].IsProceed = false;
                                    vm.ConfiguredDatasources[index].IsConfigured = vm.ConfiguredDatasources[index].IsConnected = true;
                                    updateSelectedDataSources(vm.DataSourceQueue.DatasourceId);
                                    vm.dataSources = [];
                                    angular.forEach(response.data.MultipleData.dataSources, function (dataSource, index) {
                                        vm.dataSources.push({ id: dataSource.Id, label: dataSource.Name });
                                    });
                                    toastr.success(response.data.Message);
                                    localStorage.setItem("triggerNotification", true);
                                    localStorage.setItem("triggerDataSource", true);

                                    vm.synchronizingData = false;
                                }
                                if (vm.DataSourceQueue.Status == 4) {
                                    vm.ConfiguredDatasources[index].InProcess = false;
                                    vm.ConfiguredDatasources[index].IsFailed = true;
                                    datasource.editROR = false;

                                    $interval.cancel(vm.setSyncInterval);
                                    $(".preloader").hide();
                                    toastr.error("Synchronization Failed");
                                    datasource.IsButtonDisabled = false;
                                    localStorage.setItem("triggerNotification", true);

                                    vm.synchronizingData = false;
                                }
                            }
                        })
                    }, 1000);
                } else {
                    datasource.IsButtonDisabled = false;
                    $(".preloader").hide();
                    toastr.warning(response.data.Message);

                    vm.synchronizingData = false;
                }
            });
        }
    }
    var init = function () {
        getDatasources();
        getUsers();
        getUsersList();
    }
    var saveDatasource = function (index, datasource) {
        if (!vm.synchronizingData) {
            vm.synchronizingData = true;
            datasource.IsButtonDisabled = true;
            $(".test_connection" + index).append("<div class='preloader show-loder'><img src='../Images/loading-circle.svg' class='img' /></div>");
            configureService.saveDatasource(datasource).then(function (response) {
                if (response.data.Success) {


                    $(".preloader").hide();
                    vm.DataSourceQueue = response.data.MultipleData.dataSourceQueue;
                    vm.ConfiguredDatasources[index] = response.data.MultipleData.dataSources[index];
                    //vm.ConfiguredDatasources[index].IsProceed = true;
                    vm.ConfiguredDatasources[index].Datasources = response.data.MultipleData.dataSourcesEnum
                    vm.ConfiguredDatasources[index].InProcess = true;
                    toastr.info("Data Synchronization started.");
                    localStorage.setItem("triggerNotification", true);


                    $timeout(function () {
                        $("#timer" + index).radialProgress("init", {
                            'size': 100,
                            'fill': 8
                        }).radialProgress("to", { 'perc': 100, 'time': 600000 });
                    }, 100);
                    vm.messageCount = 1;

                    vm.setMessageInterval = $interval(function () {
                        if (vm.messageCount == 1) {
                            toastr.success("<strong>Did you know?</strong><br/>You can add multiple report users with unique privileges.");
                            vm.messageCount = 2;
                        }
                        else if (vm.messageCount == 2) {
                            toastr.success("<strong>Did you know?</strong> <br/>You can configure multiple data sources to the platform.");
                            vm.messageCount = 3;
                        }
                        else if (vm.messageCount == 3) {
                            toastr.success("<strong>Did you know?</strong><br/>You can request us to configure your custom data source to the platform.");
                            $interval.cancel(vm.setMessageInterval);
                        }
                    }, 10000)

                    //$(".container-fluid").append("<div class='preloader' style='color: white;'>Synchronizing...<img src='../Images/loading-circle.svg' class='img' /></div>");
                    vm.setSyncInterval = $interval(function () {
                        //Display the current time.
                        configureService.checkForSynronizatedDataSource(vm.DataSourceQueue.Id).then(function (response1) {
                            if (response1.data.Success) {
                                vm.DataSourceQueue.Status = response1.data.Data.Status;
                                if (vm.DataSourceQueue.Status == 3) {
                                    getUsers();
                                    vm.ConfiguredDatasources[index].InProcess = false;

                                    $interval.cancel(vm.setSyncInterval);
                                    $(".preloader").hide();
                                    datasource.IsConfigured = true;
                                    datasource.IsProceed = false;
                                    datasource.editROR = false;
                                    datasource.IsButtonDisabled = false;
                                    vm.ConfiguredDatasources[index].IsProceed = false;
                                    vm.ConfiguredDatasources[index].IsConfigured = vm.ConfiguredDatasources[index].IsConnected = true;
                                    updateSelectedDataSources(vm.DataSourceQueue.DatasourceId);

                                    vm.dataSources = [];
                                    angular.forEach(response.data.MultipleData.dataSources, function (dataSource, index) {
                                        vm.dataSources.push({ id: dataSource.Id, label: dataSource.Name });
                                    });
                                    toastr.success(response.data.Message);
                                    localStorage.setItem("triggerNotification", true);
                                    localStorage.setItem("triggerDataSource", true);
                                    localStorage.setItem("triggerPlanChange", true);

                                    vm.synchronizingData = false;
                                }
                                if (vm.DataSourceQueue.Status == 4) {
                                    vm.ConfiguredDatasources[index].InProcess = false;
                                    vm.ConfiguredDatasources[index].IsFailed = true;
                                    datasource.editROR = false;
                                    $interval.cancel(vm.setSyncInterval);
                                    $(".preloader").hide();
                                    toastr.error("Synchronization Failed");
                                    datasource.IsButtonDisabled = false;
                                    localStorage.setItem("triggerNotification", true);

                                    vm.synchronizingData = false;
                                }
                            }
                        })
                    }, 1000);

                } else {
                    datasource.IsButtonDisabled = false;
                    $(".preloader").hide();
                    toastr.warning(response.data.Message);
                    vm.synchronizingData = false;
                }
            });
        }
    }
    var updateSelectedDataSources = function (id) {
        configureService.updateSelectedDataSources(id).then(function (response) {
            if (response.data.Success) {
            }
        });
    }
    vm.Search = function () {
        getUsersList();
    }
    vm.DeleteUsers = function () {
        var userEmails = [];
        angular.forEach(vm.UsersList, function (item, index) {
            if (item.IsChecked) {
                userEmails.push(item.EmailID);
            }
        });
        if (userEmails.length == 0) {
            toastr.warning("Please select atleast one user to delete!");
        }
        else {
            BootstrapDialog.confirm({
                title: 'DELETE USER(S)',
                message: 'Are you sure you want to delete selected User(s)?',
                callback: function (result) {
                    // result will be true if button was click, while it will be false if users close the dialog directly.
                    if (result) {
                        configureService.deleteUsers(userEmails).then(function (response) {
                            if (response.data.Success) {
                                toastr.success(response.data.Message);
                                getUsersList();
                            }
                        });
                    }
                }
            });
        }
    }
    vm.RemoveAccess = function (IsBlock) {
        var userEmails = [];
        angular.forEach(vm.UsersList, function (item, index) {
            if (item.IsChecked) {
                userEmails.push(item.EmailID);
            }
        });
        if (userEmails.length == 0) {
            toastr.warning("Please select atleast one user to " + (IsBlock ? "block!" : "Unblock!"));
        }
        else {
            BootstrapDialog.confirm({
                title: 'REMOVE ACCESS',
                message: 'Are you sure you want to block selected User(s)?',
                callback: function (result) {
                    // result will be true if button was click, while it will be false if users close the dialog directly.
                    if (result) {
                        configureService.updateUsersAccess(IsBlock, userEmails).then(function (response) {
                            if (response.data.Success) {
                                toastr.success(response.data.Message);
                                getUsersList();
                            }
                        });
                    }
                }
            });
        }
    }
    vm.DisconnectDatasource = function (datasource) {
        if (!vm.synchronizingData) {
            BootstrapDialog.confirm({
                title: 'DISCONNECT DATASOURCE',
                message: 'Are you sure you want to disconnect this datasource?',
                callback: function (result) {
                    // result will be true if button was click, while it will be false if users close the dialog directly.
                    if (result) {
                        configureService.disconnectDatasource(datasource).then(function (response) {
                            if (response.data.Success) {
                                localStorage.setItem("triggerNotification", true);
                                toastr.success(response.data.Message);
                                datasource.IsConnected = false;
                                datasource.IsConfigured = false;
                                datasource.IsProceed = false;
                                datasource.IsROR = false;
                                datasource.editROR = true;
                            }
                        });
                    }
                }
            });
        }
    }
    vm.CancelSynchronizing = function (datasource) {
        $interval.cancel(vm.setSyncInterval);
        //$(".preloader").hide();
        BootstrapDialog.confirm({
            title: 'CANCEL SYNCHRONIZATION',
            message: 'Are you sure you want to cancel this synchronization?',
            callback: function (result) {
                // result will be true if button was click, while it will be false if users close the dialog directly.
                if (result) {
                    configureService.disconnectDatasource(datasource).then(function (response) {
                        if (response.data.Success) {
                            localStorage.setItem("triggerNotification", true);
                            datasource.InProcess = datasource.IsConnected = false;
                            datasource.IsConfigured = datasource.IsProceed = true;
                        }
                    });
                }
            }
        });
    }
    var getUsersList = function () {
        vm.loadComplete = false;
        pageRecordModel.Filters.Name = vm.Name;
        configureService.getUsersList(pageRecordModel).then(function (response) {
            vm.UsersList = response.data.users.Data;
            vm.TotalRecords = response.data.count;
            vm.selectAll();
            getPageData();
            vm.loadComplete = true;
        });
    }
    init();
    vm.AddUser = function () {
        vm.IsAddButtonDisabled = true;
        $(".addUser").append("<div class='preloader show-loder'><img src='../Images/loading-circle.svg' class='img' /></div>");
        var user = vm.ReportUserDetails;
        //angular.forEach(vm.UserGroupsForAdd, function (item, index) {
        //    if (item.IsSelected) {
        //        user.DatasourceIds.push(item.id);
        //    }
        //});
        angular.forEach(vm.UserGroupsForAdd, function (item, index) {
            if (item.IsSelected) {
                user.UserGroups.push(item.id);
            }
        });
        //var datasources = user.DatasourceIds;
        //var userGroups = user.UserGroups;
        //user.DatasourceIds = [];
        //user.UserGroups = [];
        //angular.forEach(datasources, function (item, index) {
        //    user.DatasourceIds.push(item.id);
        //});
        //angular.forEach(userGroups, function (item, index) {
        //    user.UserGroups.push(item.id);
        //})
        var names = vm.ReportUserDetails.Name.split(" ");
        user.FirstName = names[0];
        user.LastName = names[1] == undefined ? "" : names[1];
        //user.FirstName=vm.Fisr\\
        //vm.selectedUserGroups;
        //vm.selectedDataSources;

        //alert("add");
        configureService.addUser(user).then(function (response) {
            vm.IsAddButtonDisabled = false;
            $(".preloader").hide();
            if (response.data.Success) {
                vm.ReportUserDetails = {
                };
                vm.ReportUserDetails.UserGroups = [];
                vm.ReportUserDetails.DatasourceIds = [];
                $('#UserGroupMultiSelect').multiselect('dataprovider', vm.UserGroupsForAdd);
                $('#DataSourceMultiSelect').multiselect('dataprovider', vm.dataSourcesForAdd);
                getUsersList();
                toastr.success(response.data.Message);
                $("._reset").click();
            } else {
                toastr.warning(response.data.Message);
            }
        });
    }
    vm.selectAll = function () {
        if (vm.selectAllUsers) {
            angular.forEach(vm.UsersList, function (user, index) {
                user.IsChecked = true;
            })
        }
        else {
            angular.forEach(vm.UsersList, function (user, index) {
                user.IsChecked = false;
            })
        }
    }
    vm.checkedUser = function () {
        var count = 0;
        angular.forEach(vm.UsersList, function (user, index) {
            if (user.IsChecked) {
                count = count + 1;
            }
        })
        if (count == vm.UsersList.length) {
            vm.selectAllUsers = true;
        }
        else {
            vm.selectAllUsers = false;
        }
    }
    vm.changePageSize = function () {
        pageRecordModel.PageSize = vm.selectedPageSize;
        pageRecordModel.PageNumber = 1;
        configureService.getUsersList(pageRecordModel).then(function (response) {
            vm.UsersList = response.data.users.Data;
            getPageData();
        });
    }
    vm.changePage = function (value) {
        if (value == -1 && pageRecordModel.PageNumber > 1 || value == 1 && vm.ToRecordNumber < vm.TotalRecords) {
            pageRecordModel.PageNumber = pageRecordModel.PageNumber + (value);
            configureService.getUsersList(pageRecordModel).then(function (response) {
                vm.UsersList = response.data.users.Data;
                getPageData();
            });

        }
    }
    function getPageData() {
        vm.StartingRecordNumber = ((pageRecordModel.PageNumber - 1) * pageRecordModel.PageSize) + 1;
        var toRecordNumber = pageRecordModel.PageNumber * pageRecordModel.PageSize;
        if (toRecordNumber < vm.TotalRecords) {
            vm.ToRecordNumber = toRecordNumber;
        }
        else {
            vm.ToRecordNumber = vm.TotalRecords;
        }
    }
    //vm.SortUsers = function (sortColumn, sortOrder) {
    //    $("._sort-icon").show();
    //    $(event.currentTarget).hide();
    //    pageRecordModel.PageNumber = 1;
    //    pageRecordModel.SortBy = sortColumn;
    //    pageRecordModel.SortOrder = sortOrder;
    //    configureService.getUsersList(pageRecordModel).then(function (response) {
    //        vm.UsersList = response.data.users.Data;
    //         getPageData();
    //    });        
    //}
    vm.SortUsers = function (sortColumn) {
        vm.sortColumn = sortColumn;
        if (vm.lastSort != sortColumn) {
            vm.orderBy = true;
            vm.lastSort = sortColumn;
        }
        else {
            vm.orderBy = !vm.orderBy;
        }
        vm.sortOrder = vm.orderBy == true ? 'asc' : 'desc';

        pageRecordModel.PageNumber = 1;
        pageRecordModel.SortBy = sortColumn;
        pageRecordModel.SortOrder = vm.sortOrder;

        configureService.getUsersList(pageRecordModel).then(function (response) {
            vm.UsersList = response.data.users.Data;
            getPageData();
        });
    }
    vm.changeUserGroup = function () {
        var userEmails = [];
        angular.forEach(vm.UsersList, function (item, index) {
            if (item.IsChecked) {
                userEmails.push(item.EmailID);
            }
        });
        if (userEmails.length != 0) {
            BootstrapDialog.confirm({
                title: 'CHANGE USER GROUP',
                message: 'Are you sure you want to change user group of selected User(s)?',
                callback: function (result) {
                    // result will be true if button was click, while it will be false if users close the dialog directly.
                    if (result) {
                        vm.selectedUserGroups = [];
                        angular.forEach(vm.UserGroups, function (item, index) {
                            if (item.IsChecked) {
                                vm.selectedUserGroups.push(item.id);
                            }
                        });

                        configureService.changeUserGroup(userEmails, vm.selectedUserGroups).then(function (response) {
                            if (response.data.Success) {
                                toastr.success(response.data.Message);
                                getUsersList();
                            }
                        });
                    }
                }
            });
        }
    }
    vm.checkAllUserGroup = function () {
        angular.forEach(vm.UserGroups, function (item, index) {
            if (vm.SelectAllUserGroup)
                item.IsChecked = true;
            else
                item.IsChecked = false;
        });
    }
    vm.changeDataSource = function () {
        var userEmails = [];
        angular.forEach(vm.UsersList, function (item, index) {
            if (item.IsChecked) {
                userEmails.push(item.EmailID);
            }
        });
        if (userEmails.length != 0) {
            BootstrapDialog.confirm({
                title: 'CHANGE DATA SOURCE',
                message: 'Are you sure you want to change data source of selected User(s)?',
                callback: function (result) {
                    // result will be true if button was click, while it will be false if users close the dialog directly.
                    if (result) {
                        vm.selectedDataSourceIds = [];
                        angular.forEach(vm.dataSources, function (item, index) {
                            if (item.IsChecked) {
                                vm.selectedDataSourceIds.push(item.id);
                            }
                        });

                        configureService.changeDataSource(userEmails, vm.selectedDataSourceIds).then(function (response) {
                            if (response.data.Success) {
                                toastr.success(response.data.Message);
                                getUsersList();
                            }
                        });
                    }
                }
            });
        }
    }
    vm.checkAllDataSource = function () {
        angular.forEach(vm.dataSources, function (item, index) {
            if (vm.SelectAllDataSource)
                item.IsChecked = true;
            else
                item.IsChecked = false;
        });
    }
    vm.checkForSynronizatedDataSource = function (Id) {
        vm.setSyncInterval = setInterval(
           configureService.checkForSynronizatedDataSource(Id).then(function (response) {
               if (response.data.Success) {
                   vm.DataSourceQueue.Status = response.data.Data.Status;
                   toastr.success(response.data.Message);

               }
           })
    , 100);
    }
}])