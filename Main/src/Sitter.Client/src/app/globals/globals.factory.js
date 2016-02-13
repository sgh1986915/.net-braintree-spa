angular.module('appFactories').factory('globalsFactory', [
	'$http',
	'$rootScope',
	'$q',
	'$location',
	'constantsFactory',
	function ($http, $rootScope, $q, $location, constantsFactory) {
		return {
			getGlobalData: function () {
				if ($rootScope.appGlobals) {
					return $rootScope.appGlobals;
				}
				return null;
			},
			// Watch for changes in global variables.
			watchGlobalData: function (scope) {
				scope.appGlobals = this.getGlobalData();
				$rootScope.$watch(this.getGlobalData, function (newVal) {
					scope.appGlobals = newVal;
				}, true);
			},
			// Function to retrieve global data. Called before initScope.
			retrieveGlobalData: function (scope) {
				// Setup appGlobals for scope.
				this.watchGlobalData(scope);
				var deferred = $q.defer();
				// Check for Global User object and redirects if needed.
				// TODO: we can watch everything we want this way, e.g. appGlobals itself.
				if (!scope.appGlobals || !scope.appGlobals.User) {
					deferred.reject({title: 'Auth Error', message: "User is not authenticated"});
					$location.path(constantsFactory.redirectAnonymousPath);
				} else {
					deferred.resolve(true);
				}
				return deferred.promise;
			}
		}
	}
]);