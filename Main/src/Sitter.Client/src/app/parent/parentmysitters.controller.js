angular.module('appControllers').controller('ParentMySittersController', [
	'$scope',
	'toaster',
	'$q',
	'parentFactory',
	'$filter',
	'constantsFactory',
	'popupMessagesFactory',
	'globalsFactory',
	function ($scope, toaster, $q, parentFactory, $filter, constantsFactory, popupMessagesFactory, globalsFactory) {
		// Function to remove invite.
		$scope.removeInvite = function (mobilePhoneInvite) {
			popupMessagesFactory.simpleConfirm({
				body: "Are you sure you want to cancel this invitation?",
				ok: "Yes",
				cancel: "No",
				successCallback: function () {
					var cancelInvite = {mobilePhone:mobilePhoneInvite};
					parentFactory.cancelInviteSitter(cancelInvite).success(function(data){
						if (!data.isSuccess) {
							toaster.warning("Cancel invite unsuccessful " + error.message);
							return;
						}
						toaster.info("Cancelled invite for " + mobilePhoneInvite);
						setupScope();
					}).error(function(error){
						toaster.warning("cancelled invite unsuccessful " + error);
					})
				},
				errorCallback: function () {}
			});
		};
		// Function to prepare sitters data.
		var processSittersData = function (data) {
			data = $filter('orderBy')(data, "sortOrder");
			for (var i = 0; i < data.length; i++) {
				data[i].photoUrlAdapted = data[i].photoUrl ? data[i].photoUrl : $scope.ParentSittersHelper.SitterImageUrl;
			}
			return data;
		};
		// Control listeners for sitters D&D table.
		$scope.dragSittersControlListeners = {
			accept: function (sourceItemHandleScope, destSortableScope) {
				return true;
			},
			itemMoved: function (event) {
			},
			orderChanged: function () {
				// Change sorting.
				$scope.ParentSitters.Sitters.map(function (item, key) {
					item.sortOrder = key + 1;
				});
				$scope.saveSittersList();
			}
		};

		var initScope = function () {
			var deferred = $q.defer();
			// Parent Sitters.
			$scope.ParentSitters = {
				Sitters: []
			};
			// Parent Sitters help variables.
			$scope.ParentSittersHelper = {
				Processing: false,
				SitterImageUrl: constantsFactory.defaultSitterImageUrl,
				mobilePhonePattern: constantsFactory.mobilePhonePattern,
				NewSitterMobile: null,
				inviteNickName: null
			};
			deferred.resolve(true);
			return deferred.promise;
		};

		var setupScope = function () {
			$scope.ParentSittersHelper.Processing = true;
			var deferred = $q.defer();

			parentFactory.getParentSitters().success(function (data) {
				$scope.ParentSitters.Sitters = processSittersData(data.sitters);
                $scope.ParentSitters.SitterInvites = data.sitterInvites;
                $scope.ParentSittersHelper.NewSitterMobile = null;
				$scope.ParentSittersHelper.inviteNickName = null;
				$scope.ParentSittersHelper.Processing = false;
				deferred.resolve(true);
			}).error(function (error) {
				toaster.warning(error.message, 'Unable to retrieve sitters');
				$scope.ParentSittersHelper.Processing = false;
				deferred.reject(false);
			});

			return deferred.promise;
		};
		// Save form.
		$scope.saveSittersList = function () {
			$scope.ParentSittersHelper.Processing = true;
			if ($scope.MySittersForm.$invalid) {
				toaster.error("Form is invalid", 'Please check values', constantsFactory.toasterTimeout.error);
				$scope.ParentSittersHelper.Processing = false;
				return;
			}

			parentFactory.saveParentSitters($scope.ParentSitters.Sitters).success(function (data) {
				setupScope().then(function () {
					$scope.ParentSittersHelper.Processing = false;
				});
				toaster.success("saved", null, constantsFactory.toasterTimeout.success);
			}).error(function (error) {
				toaster.warning(error.message, 'Unable to save sitters');
				$scope.ParentSittersHelper.Processing = false;
			});
		};
		// Marks sitter as "deleted" to delete it after save form
		// OR remove this mark.
        $scope.deleteSitter = function (sitter) {
			popupMessagesFactory.simpleConfirm({
				body: "Are you sure you want to delete " + $filter('sitter_title_filter')(sitter) + " from your list?",
				ok: "Yes",
				cancel: "No",
				successCallback: function () {
					$scope.ParentSitters.Sitters.map(function (item, key) {
						if (item == sitter) {
							item.deleteSitterOnSave = true;
						}
					});
					$scope.saveSittersList();
				}
			});
        };
		// Function to create new invite.
        $scope.sendInvite  = function () {
	        $scope.ParentSittersHelper.Processing = true;

            var sitterInvite = {
	            mobilePhone: $scope.ParentSittersHelper.NewSitterMobile,
				inviteNickName: $scope.ParentSittersHelper.inviteNickName
            };
	        parentFactory.sendSitterInvite(sitterInvite).success(function (response) {
				if (response.isSuccess) {
					setupScope().then(function () {
						$scope.ParentSittersHelper.Processing = false;
					});
					toaster.info("invited " + $scope.ParentSittersHelper.NewSitterMobile);
				} else {
					$scope.ParentSittersHelper.Processing = false;
					toaster.warning("did not invite: " + response.message);
				}
            }).error(function(error){
		        //toaster.warning(error., 'Unable to invite sitter');
	            $scope.ParentSittersHelper.Processing = false;
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