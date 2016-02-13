angular.module('appDirectives').directive("emailphoneformat", [
    'constantsFactory',
    function (constantsFactory) {
        return {
            restrict: "A",
            require: "ngModel",
            link: function (scope, elm, attr, ctrl) {
                var emailPhoneParse = function (value) {
                    // Try to parse as number.
                    var numbers = value && value.replace(/-/g, "");
                    if (constantsFactory.mobilePhonePattern.test(numbers)) {
                        return constantsFactory.defaultMobilePhoneCode + numbers;
                    }
                    // Try to parse as email.
                    if (constantsFactory.emailPattern.test(value)) {
                        return value;
                    }
                    return undefined;
                };
                var emailPhoneFormat = function (value) {
                    // Try to format as number.
                    var numbers = value && value.replace(/-/g, "");
                    var matches = numbers && numbers.match(/^(\+\d{1,3})?(\d{3})(\d{3})(\d{4})$/);
                    if (matches) {
                        return matches[2] + "-" + matches[3] + "-" + matches[4];
                    }
                    // Try to format as email.
                    if (constantsFactory.emailPattern.test(value)) {
                        return value;
                    }
                    return undefined;
                };
                ctrl.$parsers.push(emailPhoneParse);
                ctrl.$formatters.push(emailPhoneFormat);
                // Update the error message on blur.
                elm.bind("blur", function () {
                    var value = emailPhoneFormat(elm.val());
                    var isValid = !!value;
                    if (isValid) {
                        ctrl.$setViewValue(value);
                        ctrl.$render();
                    }
                    ctrl.$setValidity("emailphoneformat", isValid);
                    // Call scope.$apply() since blur event happens "outside of angular".
                    scope.$apply();
                });
            }
        };
    }
]);