angular.module('appFilters').filter('sitter_title_filter', function () {
	return function (sitter) {
		var output = "";
		if (!sitter) {
			return output;
		}
		if (sitter.firstName != null && sitter.lastName != null) {
			output = sitter.firstName + " " + sitter.lastName.substr(0, 1) + ".";
		} else if (sitter.mobilePhone != null) {
			output = sitter.mobilePhone;
		} else if (sitter.email != null) {
			output = sitter.email;
		}
		return output;
	}
}).filter('rate', ['$filter', function ($filter) {
	return function (rate) {
		var output =  $filter('currency')(rate, "$", 2) + "/hr";
		return output;
	}
}]).filter('phone', function () {
	return function (phone) {
		return phone.indexOf("+1") === 0 ? phone.substr(2) : phone;
	}
}).filter('hours', function () {
	return function (milliseconds) {
		var output = "";
		if (milliseconds >= 0) {
			output = Math.round(milliseconds/1000/60/60*10)/10 + " Hours";
		}
		return output;
	}
});