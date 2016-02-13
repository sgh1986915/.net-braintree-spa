angular.module('appFactories').factory('adminFactory', [
	'$http',
	function ($http) {
		var root = '/api/smssimulation/';
		return {
			getAllUsers: function(){
				return $http.get(root + 'allusers');
			},
			getAllMessages: function(){
				return $http.get(root + 'allmessages');
			},
			getAllTxtMsgAwaitingResponse: function(){
				return $http.get(root + 'allTxtMsgAwaitingResponse');
			},
			sendSimulatedInboundMessage: function(txtMsg){
				return $http.post(root + 'sendSimulatedInboundMessage', txtMsg);
			},
			getConfigInfo: function(){
				return $http.get(root + 'configInfo');
			}

		};
	}
]);