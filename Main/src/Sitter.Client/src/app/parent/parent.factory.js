angular.module('appFactories').factory('parentFactory', [
	'$http',
	function ($http) {
		return {
			getParentJobs: function () {
				return $http.get('/api/parent/myjobs');
			},
			getParentSitters: function () {
				return $http.get('/api/parent/mysitters');
			},
			saveParentSitters: function (data) {
				return $http.post('/api/parent/updatemysitters', data);
			},
			sendSitterInvite: function (data) {
				return $http.post('/api/parent/invitesitter', data);
			},
			cancelInviteSitter: function(data){
				return $http.post('/api/parent/cancelInviteSitter', data);
			},
			getProfile: function () {
				return $http.get('/api/profile');
			},
			saveProfile: function (data) {
				return $http.post('/api/profile', data)
			},
			postJob: function (data) {
				return $http.post('/api/parent/postjob', data)
			},
			//getJobById: function (id) {
			//	return $http.get('/api/job/' + '/' + id);
			//},
			finalizeJobPayment: function(data){
				return $http.post('/api/parent/finalizejobpayment', data);
			},
			cancelJob: function(jobId, role){
				var cancelJobVM = {
					jobId: jobId,
					role: role
				};
				return $http.post('/api/parent/canceljob', cancelJobVM);
			}
		};
	}
]);