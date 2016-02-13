angular.module('appControllers').controller('AdminSmsSimulatorController', [
	'$scope',
	'toaster',
	'$q',
	'adminFactory',
	'constantsFactory',
	'$sce',
	'globalsFactory',
	function ($scope, toaster, $q, adminFactory, constantsFactory, $sce, globalsFactory) {
		// Used for ui-select.
		$scope.trustAsHtml = function (value) {
			return $sce.trustAsHtml(value);
		};
		// Function to refresh messages.
		$scope.refreshMessages = function () {
			var deferred = $q.defer();
			$scope.SmsHelper.Processing = true;
			$q.all([adminFactory.getAllMessages(), adminFactory.getAllTxtMsgAwaitingResponse()]).then(
				// Success callback.
				function (data, status, headers, config) {
					$scope.SmsHelper.allMessages = data[0].data;
					$scope.SmsHelper.awaitingResponseMessages = data[1].data;
					$scope.SmsHelper.Processing = false;
					deferred.resolve(true);
				},
				// Error callback.
				function (error) {
					toaster.error(error.message, "Messages cannot be retrieved.", constantsFactory.toasterTimeout.error);
					$scope.SmsHelper.Processing = false;
					deferred.reject(false);
				});
			return deferred.promise;
		};
		// Function to send message.
		$scope.sendMessage = function () {
			var msg = {
				UserId: $scope.SmsForm.Form.UserTo.id,
				MobilePhone: $scope.SmsForm.Form.UserTo.mobilePhone,
				Message: $scope.SmsForm.Form.Message/*,
				SmsSimulationMode: true,
				IsSimulation: true*/
			};
			adminFactory.sendSimulatedInboundMessage(msg).then(
				// Success callback.
				function (data, status, headers, config) {
					toaster.success("Message has been sended.", null, constantsFactory.toasterTimeout.success);
				},
				// Error callback.
				function (error) {
					toaster.error("Unable to send message.", error.message, constantsFactory.toasterTimeout.error);
				});
		};
		var initScope = function () {
			var deferred = $q.defer();
			// Post Job form handling.
			var watchOnce = $scope.$watch("SmsForm", function (newVal) {
				if (!newVal.Form) {
					newVal.Form = {
						UserTo: '',
						Message: ''
					};
				}
				watchOnce();
			});
			// Post Job help variables.
			$scope.SmsHelper = {
				Processing: false,
				allMessages: [],
				awaitingResponseMessages: [],
				userOptions: [],
				Info: {}
			};
			$scope.SmsHelper.Processing = true;
			// Retrieve parents and sitters info.
			$q.all([adminFactory.getAllUsers(), adminFactory.getConfigInfo()]).then(
				// Success callback.
				function (data, status, headers, config) {
					$scope.SmsHelper.userOptions = data[0].data;
					$scope.SmsHelper.Info = data[1].data;
					deferred.resolve(true);
					$scope.SmsHelper.Processing = false;
				},
				// Error callback.
				function (error) {
					deferred.reject(false);
					$scope.SmsHelper.Processing = false;
				});
			deferred.resolve(true);
			return deferred.promise;
		};

		var setupScope = function () {
			return $scope.refreshMessages();
		};

		globalsFactory.retrieveGlobalData($scope).then(
			// Success.
			function () {
				initScope().then(setupScope);
			},
			// Failure.
			function (error) {
				// TODO: popup with error?
				toaster.error(error.title, error.message, constantsFactory.toasterTimeout.error);
			});
	}
]);











//(function () {
//    'use strict';
//
//    angular
//        .module('app.admin')
//        .controller('Admin', Admin);
//
//    Admin.$inject = ['sitterRepo', 'toaster'];
//
//    function Admin(sitterRepo, toaster) {
//        var vm = this;
//
//        /*-- Activate --*/
//        initParentGrid();
//        getParents();
//        initSittersGrid();
//        getSitters();
//
//        initJobsGrid();
//        getJobs();
//
//        /*-- Functions --*/
//        function initParentGrid() {
//            $("#parentsGrid").kendoGrid({
//                height: 250,
//                sortable: true,
//                selectable: true,
//                resizable: true,
//                editable: false,
//                columns: [
//                    {
//                        field: "id",
//                        title: " ",
//                        width: 55,
//                        template: "<a href='/\\#/parent/profile'>open</a>"
//                    },
//                    {
//                        field: "lastName",
//                        title: "Last Name",
//                        width: "320px"
//                    }
//                ]
//            });
//        }
//
//        function getParents() {
//            sitterRepo.getAllParents().success(function (data) {
//                var grid = $("#parentsGrid").data("kendoGrid");
//                grid.setDataSource(new kendo.data.DataSource({
//                    data: data,
//                    pageSize: 20
//                }));
//                grid.dataSource.read();
//            }).error(function (error) {
//                toaster.warning('Unable to retrieve parentSummaries ' + error.message);
//            });
//        }
//
//        function initSittersGrid() {
//            $("#sittersGrid").kendoGrid({
//                height: 250,
//                sortable: true,
//                selectable: true,
//                resizable: true,
//                editable: false,
//                columns: [
//                    {
//                        field: "id",
//                        title: " ",
//                        width: 55,
//                        template: "<a href='/\\#/sitter/profile'>open</a>"
//                    },
//                    {
//                        field: "lastName",
//                        title: "Last Name",
//                        width: "320px"
//                    }
//                ]
//            });
//        }
//
//        function getSitters() {
//            sitterRepo.getAllSitters().success(function (data) {
//                var grid = $("#sittersGrid").data("kendoGrid");
//                grid.setDataSource(new kendo.data.DataSource({
//                    data: data,
//                    pageSize: 20
//                }));
//                grid.dataSource.read();
//            }).error(function (error) {
//                toaster.warning('Unable to retrieve sitters ' + error.message);
//            });
//        }
//
//        function initJobsGrid() {
//            $("#jobsGrid").kendoGrid({
//                height: 250,
//                sortable: true,
//                selectable: true,
//                resizable: true,
//                editable: false,
//                columns: [
//                    {
//                        field: "id",
//                        title: " ",
//                        width: 55,
//                        template: "<a href='/\\#/parent/jobs/${id}'>open</a>"
//                    },
//                    {
//                        field: "description",
//                        title: "Description",
//                        width: "320px"
//                    }
//                ]
//            });
//        }
//
//        function getJobs() {
//            sitterRepo.getAllJobs().success(function (data) {
//                var grid = $("#jobsGrid").data("kendoGrid");
//                grid.setDataSource(new kendo.data.DataSource({
//                    data: data,
//                    pageSize: 20
//                }));
//                grid.dataSource.read();
//            }).error(function (error) {
//                toaster.warning('Unable to retrieve jobs ' + error.message);
//            });
//        }
//
//    }
//})();