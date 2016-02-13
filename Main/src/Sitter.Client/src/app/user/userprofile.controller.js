angular.module('appControllers').controller('UserProfileController', [
	'$scope',
	'toaster',
	'$q',
	'userFactory',
	'constantsFactory',
	'globalsFactory',
	'$location',
	'$modal',
	function ($scope, toaster, $q, userFactory, constantsFactory, globalsFactory, $location, $modal) {
		var initScope = function () {
			var deferred = $q.defer();
			// Parent Profile help variables.
			var now = new Date();
			$scope.ParentProfileHelper = {
				Processing: false,
				browserTimeOffset: now.getTimezoneOffset()/60,
				currentTimeWithOffset: null,
				ChangePass: {
					oldPass: null,
					newPass: null,
					newPassRepeat: null
				},
				ChangePassPressed: false,
				ParentImageUrl: constantsFactory.defaultParentImageUrl,
				mobilePhonePattern: constantsFactory.mobilePhonePattern
			};
			$scope.$watch('ParentProfile.Form.timezoneOffset', function (newVal) {
				if (angular.isNumber(newVal)) {
					$scope.ParentProfileHelper.currentTimeWithOffset = new Date();
					var newHours = $scope.ParentProfileHelper.currentTimeWithOffset.getHours() + Math.round(newVal) + Math.round($scope.ParentProfileHelper.browserTimeOffset);
					var newMinutes = $scope.ParentProfileHelper.currentTimeWithOffset.getMinutes() + (newVal % 1)*60 + ($scope.ParentProfileHelper.browserTimeOffset % 1)*60;
					$scope.ParentProfileHelper.currentTimeWithOffset.setHours(newHours, newMinutes);
				} else {
					$scope.ParentProfileHelper.currentTimeWithOffset = null;
				}
			}, true);
			deferred.resolve(true);
			return deferred.promise;
		};

		var setupScope = function () {
			$scope.ParentProfileHelper.Processing = true;
			var deferred = $q.defer();
			// Once ParentProfile scope variable appears it will mean that appropriate form rendered so we can fill the data.
			var watchOnce = $scope.$watch("ParentProfile", function (newVal) {
				if (newVal.Form === undefined) {
					userFactory.getProfile().success(function (data) {
						$scope.ParentProfile.Form = data;
						$scope.ParentProfileHelper.ParentImageUrl = data.photoUrl ? data.photoUrl : constantsFactory.defaultParentImageUrl;
						$scope.ParentProfile.$setPristine();
						$scope.ParentProfileHelper.Processing = false;
						deferred.resolve(true);
					}).error(function (error) {
						toaster.warning('Unable to retrieve profile ' + error);
						$scope.ParentProfileHelper.Processing = false;
						deferred.reject(false);
					});
					watchOnce();
				}
			});

			return deferred.promise;
		};

		$scope.changePassword = function () {
			$scope.ParentProfileHelper.ChangePassPressed = true;
		};
		// Save password.
		$scope.savePassword = function () {
			$scope.ParentProfileHelper.Processing = true;
			if ($scope.ParentProfileHelper.ChangePassPressed) {
				var dataToSend = {};
				dataToSend.AppUser = angular.copy($scope.ParentProfile.Form);
				dataToSend.ChangePass = $scope.ParentProfileHelper.ChangePass;
				// Validate confirm password.
				// Save.
				userFactory.saveProfile(dataToSend).success(function (data) {
					setupScope().then(function () {
						$scope.ParentProfileHelper.Processing = false;
					});
					toaster.success("saved", null, constantsFactory.toasterTimeout.success);
				}).error(function (error) {
					toaster.warning(error.message, 'Unable to save profile');
					$scope.ParentProfileHelper.Processing = false;
				});
			}
		};
		// Save profile.
		$scope.saveProfile = function () {
			$scope.ParentProfileHelper.Processing = true;
			if ($scope.ParentProfile.$invalid) {
				$scope.ParentProfileHelper.Processing = false;
				toaster.error("Form is invalid", "Please fix errors in form", constantsFactory.toasterTimeout.error);
				return;
			}
			var dataToSend = {};
			dataToSend.AppUser = angular.copy($scope.ParentProfile.Form);
			// Save.
			userFactory.saveProfile(dataToSend).success(function (data) {
				setupScope().then(function () {
					$scope.ParentProfileHelper.Processing = false;
				});
				toaster.success("saved", null, constantsFactory.toasterTimeout.success);
			}).error(function (error) {
				toaster.warning(error.message, 'Unable to save profile');
				$scope.ParentProfileHelper.Processing = false;
			});
		};

		$scope.editPaymentInfo = function(){
			var modalInstance = $modal.open({
				templateUrl: 'app/user/paymentinfo.modal.html',
				controller: "PaymentInfoModalController",
				resolve: {
					//jobsm: function () {
					//	return "";
					//}
				},
				size: "sm",
				windowClass: "pay-sitter"
			});
		}

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

		$scope.imageChange = function(element){

			if (element.files.length < 1) {
				return;
			}

			userFactory.getSecureUploadUrl().success(function(data){
				//TODO: uplaod to AWS
				// This code might help, but you'll have to research: https://gist.github.com/zxbodya/3cdabd9172bcc89f8ac5
				toaster.success("got secure upload URL, next step is to upload to AWS....");
			}).error(function(error){
				toaster.error(error.title,"unable to upload profile picture " + error.message, constantsFactory.toasterTimeout.error);
			})

		}


		//$scope.getProfilePic = function(){
		//	userFactory.getUserPicture().success(function(data){
		//		var f = data;
		//	}).error(function(error){
		//		toaster.error(error.title,"unable to get profile picture " + error.message, constantsFactory.toasterTimeout.error);
		//	})
		//}
        //
		//// on activate
		//$scope.getProfilePic();

	}
]);