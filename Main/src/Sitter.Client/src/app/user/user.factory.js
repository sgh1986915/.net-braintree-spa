angular.module('appFactories').factory('userFactory', [
	'$http',
	'$rootScope',
	'ipCookie',
	'constantsFactory',
	function ($http, $rootScope, ipCookie, constantsFactory) {
		return {
			forgotPasswordChangeRequest: function (data) {
				return $http.post('/api/forgotpassword/changerequest', data);
			},
			forgotPasswordSendCode: function (data) {
				return $http.post('/api/forgotpassword/sendcode', data);
			},
			forgotPasswordChangePassword: function (data) {
				return $http.post('/api/forgotpassword/changepassword', data);
			},
			getProfile: function () {
				return $http.get('/api/profile/getUserProfile');
			},
			saveProfile: function (data) {
				return $http.post('/api/profile', data)
			},
			login: function (data) {
				var authRequest = {
					UserName: data.username,
					Pass: data.password
				};
				return $http.post('/api/auth', authRequest);
			},
			setCredentials: function (response, remember) {
				var authdata = response.userId + ':' + response.token; // Futuredev: use userId
				if (!$rootScope.appGlobals) {
					$rootScope.appGlobals = {};
				}
				//$rootScope.appGlobals = {};
				$http.defaults.headers.common['Authorization'] = 'SimpleToken ' + authdata; //TODO: use userId instead of email.
				//$rootScope.appGlobals.userId = response.userId; // Futuredev: use userId
				//$rootScope.appGlobals.token = response.token;
				//$rootScope.appGlobals.userDisplayName = response.userDisplayName;
				//$rootScope.appGlobals.userRole = response.userRole;
				// TODO: maybe it makes sense to return full user data like photo and so on ?
				$rootScope.appGlobals.User = {
					userRole: response.userRole,
					token: response.token,
					userId: response.userId,
					photo: '/content/images/profile-64.png',
					userName: response.userDisplayName
				};
				if (remember) {
					ipCookie('appGlobals', $rootScope.appGlobals, {expires: constantsFactory.accountCookieExpiration, expirationUnit: 'hours'});
				} else {
					ipCookie('appGlobals', $rootScope.appGlobals);
				}
			},
			clearCredentials: function () {
				$rootScope.appGlobals = {};
				$http.defaults.headers.common.Authorization = 'SimpleToken ';
				ipCookie.remove('appGlobals');
			},
			signup: function (signupInfo) {
				return $http.post('/api/signup', signupInfo);
			},
			getUserData: function () {
				if ($rootScope.appGlobals) {
					if ($rootScope.appGlobals.User) {
						return $rootScope.appGlobals.User;
					}
				}
				return null;
			},
			getSecureUploadUrl:function(){
				return $http.get('/api/profile/getSecureUploadUrl');
			}
			//,
			//getUserPicture:function(){
			//	return $http.get('/api/profile/getUserPicture');
			//}
		};
	}
]);