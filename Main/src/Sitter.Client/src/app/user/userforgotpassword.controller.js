angular.module('appControllers').controller('UserForgotPasswordController', [
	'$scope',
	'userFactory',
	'$rootScope',
	'$routeParams',
	'toaster',
	'constantsFactory',
	function ($scope, userFactory, $rootScope, $routeParams, toaster, constantsFactory) {
		// Function to submit mobilePhone.
		$scope.changeRequest = function () {
			$scope.ForgotPasswordHelper.Processing = true;
			userFactory.forgotPasswordChangeRequest($scope.ChangeRequestFormValues)
				.success(function (data, status, headers, config) {
					$scope.ForgotPasswordHelper.hash = data.hash;
					$scope.ForgotPasswordHelper.step = 2;
					$scope.ForgotPasswordHelper.Processing = true;
				}).error(function (error, status, headers, config) {
					toaster.error("Mobile Phone doesn't exist", error, constantsFactory.toasterTimeout.error);
					$scope.ForgotPasswordHelper.Processing = true;
				});
		};
		// Function to submit code.
		$scope.sendCode = function () {
			$scope.SendCodeFormValues.Hash = $scope.ForgotPasswordHelper.hash;
			$scope.ForgotPasswordHelper.Processing = true;
			userFactory.forgotPasswordSendCode($scope.SendCodeFormValues)
				.success(function (data, status, headers, config) {
					$scope.ForgotPasswordHelper.hash = data.hash;
					$scope.ForgotPasswordHelper.step = 3;
					$scope.ForgotPasswordHelper.Processing = true;
				}).error(function (error, status, headers, config) {
					toaster.error("Code is invalid or hash expired", error, constantsFactory.toasterTimeout.error);
					$scope.ForgotPasswordHelper.Processing = true;
				});
		};
		// Function to submit new password.
		$scope.sendPassword = function () {
			$scope.SendPasswordFormValues.Hash = $scope.ForgotPasswordHelper.hash;
			$scope.ForgotPasswordHelper.Processing = true;
			userFactory.forgotPasswordChangePassword($scope.SendPasswordFormValues)
				.success(function (data, status, headers, config) {
					$scope.ForgotPasswordHelper.step = 4;
					$scope.ForgotPasswordHelper.Processing = true;
				}).error(function (error, status, headers, config) {
					toaster.error("", error, constantsFactory.toasterTimeout.error);
					$scope.ForgotPasswordHelper.Processing = true;
				});
		};

		var initScope = function () {
			$scope.ForgotPasswordHelper = {
				processing: false,
				hash: null,
				step: 1,
				forgotPasswordCodePattern: constantsFactory.forgotPasswordCodePattern
			};
			$scope.ChangeRequestFormValues = {
				Mobile: null
			};
			$scope.SendCodeFormValues = {
				Code: null,
				Hash: null
			};
			$scope.SendPasswordFormValues = {
				password: null
			};
		};

		initScope();
	}
]);