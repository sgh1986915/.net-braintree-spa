angular.module('appControllers').controller('PopupMessagesController', [
	'$scope',
	'$modalInstance',
	'title',
	'body',
	'ok',
	'cancel',
	function ($scope, $modalInstance, title, body, ok, cancel) {
		$scope.okCallback = function () {
			$modalInstance.close(true);
		};
		$scope.cancelCallback = function () {
			$modalInstance.dismiss(false);
		};
		$scope.title = title;
		$scope.body = body;
		$scope.ok = ok;
		$scope.cancel = cancel;
	}
]);