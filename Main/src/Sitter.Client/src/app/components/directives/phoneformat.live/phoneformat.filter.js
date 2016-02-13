angular.module('appDirectives').filter('mobile_phone', function () {
    return function (phone) {
        var output = "";
        var matches = phone && phone.match(/^(\+\d{1,3})(\d{3})(\d{3})(\d{4})$/);
        if (matches) {
            output = matches[2] + "-" + matches[3] + "-" + matches[4];
        }
        return output;
    }
});