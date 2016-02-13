angular.module('appFactories').factory('sitterFactory', [
	'$http',
	function ($http) {
		return {
			getSitterMyClients: function (id) {
				return $http.get('/api/sitter/myclients');
			},
			getSitterJobs: function (id) {
				return $http.get('/api/sitter/myjobs');
			},
			jobInviteResponse: function (jobId, acceptOrDecline) {
				var r = {jobId:jobId, response:acceptOrDecline};
				return $http.post('/api/sitter/jobresponse' , r);
			},
			renegeAcceptedJob: function (jobId) {
				var renig = {jobId:jobId};
				return $http.post('/api/sitter/renegeacceptedjob' , renig);
			},
			requestPayment:function(jobId){
				var rp = {jobId:jobId};
				return $http.post('/api/sitter/requestpayment' , rp);
			}
		};
	}
]);