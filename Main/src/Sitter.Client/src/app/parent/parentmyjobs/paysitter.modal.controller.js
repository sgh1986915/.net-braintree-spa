angular.module('appControllers').controller('PaySitterModalController', [
	'$scope',
	'$modalInstance',
	'$q',
	'constantsFactory',
	'jobFactory',
	'jobsm',
	function ($scope, $modalInstance, $q, constantsFactory, jobFactory, jobsm) {
		var initScope = function () {
			var deferred = $q.defer();
			// Post Job form handling.
			$scope.$watch("PaySitter", function (newVal, oldVal) {
				// This way we can add some data when Form PaySitter element already created.
				if (newVal.Form == undefined) {
					newVal.Form = angular.copy(jobsm);
				} else {
					// If bonus is set let's refresh other values.
					var payment = jobFactory.calculatePrice({
						startTime: newVal.Form.dateTimeStart,
						endTime: newVal.Form.dateTimeEnd,
						bonus: newVal.Form.bonus,
						ratePerHour: newVal.Form.estRatePerHour
					});
					newVal.Form.estPaidToSitter = payment.payToSitter;
					newVal.Form.estPaidByParent = payment.payTotal;
					$scope.duration =  payment.duration;     //todo: artem, please code review ...
					$scope.bonus =  newVal.Form.bonus;
				}
			}, true);
			// Post Job help variables.
			$scope.PaySitterHelper = {
				// Original jobsm object.
				jobsm: jobsm,
				AcceptedSitterInvite: jobsm.acceptedSitterInvite,
				totalToSitter: null,
				totalCharged: null
			};
			$scope.jobId =  jobsm.job.id;
			$scope.duration =  jobsm.job.duration;
			$scope.bonus =  0;

			return deferred.promise;
		};

		var setupScope = function () {
			// It probably will be needed later e.g. to load full job data.
		};

		$scope.Close = function () {
			$modalInstance.dismiss(false);
		};

		$scope.SubmitPayment = function () {
			var finalize = {
				duration:$scope.duration,
				bonus:$scope.bonus,
				jobId:$scope.jobId
			};
			$modalInstance.close(finalize);
		};

		initScope().then(setupScope);
	}
]);