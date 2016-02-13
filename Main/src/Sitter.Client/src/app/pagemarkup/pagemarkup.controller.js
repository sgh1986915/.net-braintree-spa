angular.module('appControllers').controller('PageMarkupController', [
	'$scope',
	'toaster',
	'$q',
	'$location',
	'constantsFactory',
	'$route',
	// IMPORTANT: injecting $route provider is necessary to have ngView works fine inside ng-included templates.
	function ($scope, toaster, $q, $location, constantsFactory, $route) {
		var initScope = function () {
			// Define page markup.
			$scope.getPageMarkupTemplate = function () {
				var path = "";
				switch ($location.path()) {
					case "/user/signup":
					case "/":
					case "/user/login":
					case "/user/forgotpassword":
						path = 'app/pagemarkup/pagemarkup-splash.html';
						break;
					default:
						path = 'app/pagemarkup/pagemarkup-dashboard.html';
						break;
				}
				return path;
			};
		};

		initScope();
	}
]);