angular.module("appRoutes").config(['$routeProvider', function ($routeProvider) {
	$routeProvider.
		when('/sitter/myjobs', {
			templateUrl: 'app/sitter/sittermyjobs.html',
			controller: 'SitterMyJobsController'
		}).
		when('/sitter/profile', {
			templateUrl: 'app/user/userprofile.html',
			controller: 'UserProfileController'
		}).
		when('/sitter/myclients', {
			templateUrl: 'app/sitter/sittermyclients.html',
			controller: 'SitterMyClientsController'
		});
}]);