angular.module('appControllers').controller('ParentMyJobsController', [
	'$scope',
	'toaster',
	'$q',
	'parentFactory',
	'$filter',
	'popupMessagesFactory',
	'$modal',
	'jobFactory',
	'globalsFactory',
	'constantsFactory',
	function ($scope, toaster, $q, parentFactory, $filter, popupMessagesFactory, $modal, jobFactory, globalsFactory, constantsFactory) {
		// Function to pay sitter.
		$scope.paySitter = function (jobsm) {
			var modalInstance = $modal.open({
				templateUrl: 'app/parent/parentmyjobs/paysitter.modal.html',
				controller: "PaySitterModalController",
				resolve: {
					jobsm: function () {
						return jobsm;
					}
				},
				size: "sm",
				windowClass: "pay-sitter"
			});
			modalInstance.result.then(
				// Success callback. job - edited job data.
				function (finalize) {
					parentFactory.finalizeJobPayment(finalize).success(function(result){
						toaster.info('finalized payment');
						setupScope();
					}).error(function(error){
						toaster.warning('unable to finalize payment ' + error);
					});
				},
				// Close pop-up callback.
				function () {});
		};

		// Function to prepare jobs data.
		var processJobsData = function (data) {
			for (var i = 0; i < data.length; i++) {
				data[i].dateTimeStart = new Date(data[i].job.start);
				var de = new Date(data[i].dateTimeStart);
				de.setHours(de.getHours() + Math.floor(data[i].job.duration), de.getMinutes() + (data[i].job.duration % 1)*60);
				data[i].dateTimeEnd = de;
				// Collapsed details by default.
				data[i].collapsed = true;
				//jobFactory.defineSubstatus(data[i]);
			}
			return data;
		};
		// Function to handle click on "more"/"less" button.
		$scope.expandCollapseClosedJobs = function (val) {
			$scope.ParentJobsHelper.morePressed += val;
		};
		// Function to cancel job.
		$scope.cancelJob = function (jobId) {
			popupMessagesFactory.simpleConfirm({
				title: "Cancel job",
				body: "Do you really want to cancel the job?",
				ok: "Yes",
				cancel: "No",
				successCallback: function () {
					$scope.ParentJobsHelper.Processing = true;
					parentFactory.cancelJob(jobId, 'parent').success(function(result){
						toaster.info("job cancelled " + jobId);
						$scope.ParentJobsHelper.Processing = false;
						setupScope();
					}).error(function(error){
						toaster.warning("unable to cancel job " + error);
						$scope.ParentJobsHelper.Processing = false;
					});
				},
				errorCallback: function () {}
			});
		};

		var initScope = function () {
			var deferred = $q.defer();
			// Parent Jobs.
			$scope.ParentJobs = [];
			// Parent Jobs help variables.
			$scope.ParentJobsHelper = {
				Processing: false,
				closedJobs: [],
				morePressed: 0,
				closedJobsInitOffset: 3,
				closedJobsPerExpand: 12
			};
			deferred.resolve(true);
			return deferred.promise;
		};
		var setupScope = function () {
			$scope.ParentJobsHelper.Processing = true;
			var deferred = $q.defer();

			parentFactory.getParentJobs().success(function (data) {
				$scope.ParentJobs = processJobsData(data);
				// Set up closedJobs.
				//$scope.ParentJobsHelper.closedJobs = $filter('filter')($scope.ParentJobs, {isOpen: false}, false);
				$scope.ParentJobsHelper.Processing = false;
				deferred.resolve(true);
			}).error(function (error, httpcode) {
				var msg = httpcode == '401' ? "unauthorized" : ""; //FutureDev: put this is an helper method. what is the best way to redirect when unauthorized?
				msg += error.message ? error.message : error;
				toaster.warning('Unable to retrieve jobs. ' + msg);
				$scope.ParentJobsHelper.Processing = false;
				deferred.reject(false);
			});

			return deferred.promise;
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