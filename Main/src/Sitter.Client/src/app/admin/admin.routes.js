angular.module("appRoutes").config(['$routeProvider', function ($routeProvider) {
	$routeProvider.
		when('/admin/sms-simulator', {
			templateUrl: 'app/admin/adminsmssimulator.html',
			controller: 'AdminSmsSimulatorController'
		});
}]);
