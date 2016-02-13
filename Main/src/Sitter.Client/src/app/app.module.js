angular.module('app', [
	'appRoutes',
	'appControllers',
	'appFactories',
	'appFilters',
	'appDirectives',
	'ngAnimate',
	'ipCookie',
	'ui.select',
	'ui.sortable',
	'ui.bootstrap.showErrors',
	'ui.bootstrap',
	'toaster',
	'720kb.background'
]);

angular.module('appFactories', ['ngRoute', 'toaster']);

angular.module('appControllers', ['ngRoute']);

angular.module('appRoutes', ['ngRoute']);

angular.module('appFilters', ['ngRoute']);

angular.module('appDirectives', ['ngRoute']);

angular.module('app').run([
	'$rootScope',
	'ipCookie',
	'$http',
	function ($rootScope, ipCookie, $http) {
		// One time at startup: setAppGlobalsFromCookieAtStartup.
		$rootScope.appGlobals = {};
		$rootScope.appGlobals = ipCookie('appGlobals');
		if ($rootScope.appGlobals) {
			if ($rootScope.appGlobals.User) {
				var authdata = $rootScope.appGlobals.User.userId + ':' + $rootScope.appGlobals.User.token;
				$http.defaults.headers.common['Authorization'] = 'SimpleToken ' + authdata;
			}
		}
	}
]);