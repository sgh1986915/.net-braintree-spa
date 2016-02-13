angular.module('appControllers').controller('PaymentInfoModalController', [
	'$scope',
	'$modalInstance',
	'$q',
	'constantsFactory',
	function ($scope, $modalInstance, $q, constantsFactory) {
		var initScope = function () {
			var deferred = $q.defer();
			// Post Job form handling.

			$scope.info = "info";


			return deferred.promise;
		};

		var setupScope = function () {
			// It probably will be needed later e.g. to load full job data.
		};

		$scope.SubmitPaymentInfo = function () {
			var info = {
				info:$scope.info
			};
			$modalInstance.close(info);
		};

		initScope().then(setupScope);
	}
]);