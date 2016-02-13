/*
 * Datepicker has wrong Date formatting on initialization.
 * It is datepicker's bug: https://github.com/angular-ui/bootstrap/issues/2659
 * Fixed due to http://stackoverflow.com/a/26111232
 * TODO: remove on new bootstrap-ui version update.
 */
angular.module('app').directive('datepickerPopup', ['dateFilter', 'datepickerPopupConfig', function (dateFilter, datepickerPopupConfig) {
	return {
		restrict: 'A',
		priority: 1,
		require: 'ngModel',
		link: function(scope, element, attr, ngModel) {
			var dateFormat = attr.datepickerPopup || datepickerPopupConfig.datepickerPopup;
			ngModel.$formatters.push(function (value) {
				return dateFilter(value, dateFormat);
			});
		}
	};
}]);