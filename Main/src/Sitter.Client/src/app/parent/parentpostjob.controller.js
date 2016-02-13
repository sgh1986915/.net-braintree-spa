angular.module('appControllers').controller('ParentPostJobController', [
	'$scope',
	'toaster',
	'$q',
	'parentFactory',
	'constantsFactory',
	'$location',
	'globalsFactory',
	function ($scope, toaster, $q, parentFactory, constantsFactory, $location, globalsFactory) {
		// Function to prepare sitters data.
		// Returns Obj in format {'id_<SITTER_ID>': {<SITTER OBJ>}, ...}
		var processSittersData = function (data) {
			var sittersObj = {};
			for (var i = 0; i < data.length; i++) {
				data[i].photoUrlAdapted = data[i].photoUrl ? data[i].photoUrl : $scope.PostJobHelper.SitterImageUrl;
				// The last step: save sitter data inside sittersObj.
				sittersObj['id_' + data[i].id] = data[i];
			}
			return sittersObj;
		};
		// Function to select sitter. Selects all sitters if no argument.
		$scope.selectSitter = function (id) {
			if (id === undefined) {
				// Check if there are all sitters.
				var hasOwnProp = 0;
				for (var prop in $scope.PostJob.Form.InvitesObject) {
					if ($scope.PostJob.Form.InvitesObject.hasOwnProperty(prop)) {
						hasOwnProp++;
					}
				}
				if (hasOwnProp == $scope.PostJobHelper.ParentSittersQuantity) {
					$scope.PostJob.Form.InvitesObject = {};
				} else {
					$scope.PostJob.Form.InvitesObject = angular.copy($scope.PostJobHelper.ParentSitters);
				}
			} else {
				// Deselect sitter if selected before.
				if ($scope.PostJob.Form.InvitesObject[id]) {
					delete $scope.PostJob.Form.InvitesObject[id];
				} else if ($scope.PostJobHelper.ParentSitters[id]) {
					$scope.PostJob.Form.InvitesObject[id] = $scope.PostJobHelper.ParentSitters[id];
				}
			}
			$scope.PostJob.$setDirty();
		};

		var initScope = function () {
			var deferred = $q.defer();
			// Post Job form handling.
			var watchOnce = $scope.$watch("PostJob", function (newVal) {
				//var curDate = new Date();
				newVal.Form = {
					dateTimeStart: null,
					dateTimeEnd: null,
					Invites: [],
					InvitesObject: {},
					Notes: null,
					AdditionalJobs: []
				};
				watchOnce();
			});
			// Post Job help variables.
			$scope.PostJobHelper = {
				SelectAllSitters: false,
				Processing: false,
				SitterImageUrl: constantsFactory.defaultSitterImageUrl,
				ParentSitters: [],
				ParentSittersQuantity: 0,
				AdditionalOptionsCollapsed: true,
				AdditionalJob: {
					Description: null,
					Amount: null
				}
			};
			// Watch for changes in InvitesObject to reflect them in Invites array.
			$scope.$watch('PostJob.Form.InvitesObject', function (newVal, oldVal) {
				if (newVal) {
					var sittersArray = [];
					for (key in newVal) {
						sittersArray.push(newVal[key]);
					}
					$scope.PostJob.Form.JobInvites = sittersArray;
					// If we selected all available sitters we should reflect it in appropriate variable to show checkbox.
					if ($scope.PostJobHelper.ParentSittersQuantity == $scope.PostJob.Form.JobInvites.length
						&& $scope.PostJobHelper.ParentSittersQuantity != 0) {
						$scope.PostJobHelper.SelectAllSitters = true;
					} else {
						$scope.PostJobHelper.SelectAllSitters = false;
					}
				} else {
					$scope.PostJob.Form.JobInvites = [];
				}
			}, true);
			// Get sitters.
			parentFactory.getParentSitters().success(function (data) {
				$scope.PostJobHelper.ParentSitters = processSittersData(data.sitters);
				// Set sitters quantity.
				$scope.PostJobHelper.ParentSittersQuantity = 0;
				for (var prop in $scope.PostJobHelper.ParentSitters) {
					if ($scope.PostJobHelper.ParentSitters.hasOwnProperty(prop)) {
						$scope.PostJobHelper.ParentSittersQuantity++;
					}
				}
				$scope.PostJob.$setPristine();
				$scope.PostJobHelper.Processing = false;
				deferred.resolve(true);
			}).error(function (error) {
				toaster.warning(error.message, 'Unable to retrieve sitters');
				$scope.PostJobHelper.Processing = false;
				deferred.reject(false);
			});

			return deferred.promise;
		};

		var setupScope = function () {
			// Maybe we will need it later e.g. load Enums?
		};
		// Save form.
		$scope.saveForm = function () {
			$scope.PostJobHelper.Processing = true;

			// Additional validation: check that start time <= end time.
			// TODO: maybe put this logic into directive for timedate?
			if ($scope.PostJob.Form.dateTimeStart > $scope.PostJob.Form.dateTimeEnd) {
				toaster.error("", "Start time couldn't be greater than End time.", constantsFactory.toasterTimeout.error);
				$scope.PostJob.$setPristine();
				$scope.PostJobHelper.Processing = false;
				return;
			}
			// Additional validation: check that user choosen at least 1 sitter.
			if (!$scope.PostJob.Form.JobInvites.length) {
				toaster.error("", "Choose at least 1 sitter.", constantsFactory.toasterTimeout.error);
				$scope.PostJob.$setPristine();
				$scope.PostJobHelper.Processing = false;
				return;
			}

			for (var i = 0; i < $scope.PostJob.Form.JobInvites.length; i++) {
				var jobInvite = $scope.PostJob.Form.JobInvites[i];
				jobInvite.sitterId = jobInvite.id;
				jobInvite.ratePerHour = jobInvite.rate;

			}

			$scope.PostJob.Form.duration = ($scope.PostJob.Form.dateTimeEnd - $scope.PostJob.Form.dateTimeStart) /(60*60*1000);

			// Saving.
			parentFactory.postJob($scope.PostJob.Form).success(function (data) {
				if (data.hasError) {
					toaster.warning('Post Job did not succeed. ' + data.error);
				}else{
					toaster.success("saved", null, constantsFactory.toasterTimeout.success);
					$location.path('/parent/myjobs');
				}
			}).error(function (error) {
				toaster.warning(error.message, 'Unable to Save ');
			}).then(function(){
				$scope.PostJobHelper.Processing = false;
			});
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