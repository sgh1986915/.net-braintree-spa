angular.module('appControllers').controller('UserLandingController', [
	'$scope',
	'toaster',
	'$q',
	'constantsFactory',
	'globalsFactory',
	'$location',
	function ($scope, toaster, $q, constantsFactory, globalsFactory, $location) {

		var initScope = function () {
			$scope.step = 1;
		};

		initScope();
	}
]);