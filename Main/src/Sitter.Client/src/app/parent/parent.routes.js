angular.module("appRoutes").config(['$routeProvider', function ($routeProvider) {
	$routeProvider.
		when('/parent/post-job', {
			templateUrl: 'app/parent/parentpostjob.html',
			controller: 'ParentPostJobController'
		}).
		when('/parent/myjobs', {
			templateUrl: 'app/parent/parentmyjobs/parentmyjobs.html',
			controller: 'ParentMyJobsController'
		}).
		//// TODO: Not used anywhere.
		//when('/parent/jobs/:jobId', {
		//	templateUrl: 'app/parent/parentjobdetail.html',
		//	controller: 'ParentJobDetailsController'
		//}).
		when('/parent/mysitters', {
			templateUrl: 'app/parent/parentmysitters.html',
			controller: 'ParentMySittersController'
		}).
		when('/parent/payment', {
			templateUrl: 'app/parent/parentpayment.html',
			controller: 'ParentPaymentController'
		}).
		when('/parent/profile', {
			templateUrl: 'app/user/userprofile.html',
			controller: 'UserProfileController'
		});
}]);
