angular.module('appFactories').factory('popupMessagesFactory', [
	'$modal',
	function ($modal) {
		return {
			/*
			 * data = {
			 *  title: <String>,
			 *  body: <String>,
			 *  ok: <String>,
			 *  cancel: <String>,
			 *  successCallback: <Function>,
			 *  errorCallback: <Function>
			 * }
			 */
			simpleConfirm: function (data) {
				var modalInstance = $modal.open({
					templateUrl: 'app/components/popupmessages/simpleconfirm.html',
					controller: "PopupMessagesController",
					resolve: {
						title: function () {
							return data.title;
						},
						body: function () {
							return data.body;
						},
						ok: function () {
							return data.ok;
						},
						cancel: function () {
							return data.cancel;
						}
					},
					size: data.size ? data.size : "sm",
					windowClass: "simple-confirm"
				});
				modalInstance.result.then(data.successCallback, data.errorCallback);
				return modalInstance;
			}
		};
	}
]);