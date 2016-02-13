angular.module("appRoutes").config(['$routeProvider', function ($routeProvider) {
	$routeProvider.
		when('/', {
			templateUrl: 'app/user/userlanding.html',
			controller: 'UserLandingController'
		}).
		when('/user/profile', {
			templateUrl: 'app/user/userprofile.html',
			controller: 'UserProfileController'
		}).
		when('/user/signup', {
			templateUrl: 'app/user/usersignup.html',
			controller: 'UserSignupController'
		}).
		when('/user/forgotpassword', {
			templateUrl: 'app/user/userforgotpassword.html',
			controller: 'UserForgotPasswordController'
		}).
		when('/user/login', {
			templateUrl: 'app/user/userlogin.html',
			controller: 'UserLoginController'
		});
}]);
