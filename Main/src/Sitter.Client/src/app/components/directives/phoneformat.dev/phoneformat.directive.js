angular.module('appDirectives').directive("phoneformat", [
    'constantsFactory',
    function (constantsFactory) {
        return {
            restrict: "A",
            require: "ngModel",
            link: function (scope, elm, attr, ctrl) {
                var phoneParse = function (value) {
                    var numbers = value && value.replace(/-/g, "");
                    if (constantsFactory.mobilePhoneWithCodePattern.test(numbers)) {
                        return numbers;
                    }
                    return undefined;
                };
                var phoneFormat = function (value) {
                    var numbers = value && value.replace(/-/g, "");
                    var matches = numbers && numbers.match(/^(\+\d{1,3})(\d{3})(\d{3})(\d{4})$/);
                    if (matches) {
                        return matches[1] + "-" + matches[2] + "-" + matches[3] + "-" + matches[4];
                    }
                    return undefined;
                };
                ctrl.$parsers.push(phoneParse);
                ctrl.$formatters.push(phoneFormat);
                // Update the error message on blur.
                elm.bind("blur", function () {
                    var value = phoneFormat(elm.val());
                    var isValid = !!value;
                    if (isValid) {
                        ctrl.$setViewValue(value);
                        ctrl.$render();
                    }
                    ctrl.$setValidity("phoneformat", isValid);
                    // Call scope.$apply() since blur event happens "outside of angular".
                    scope.$apply();
                });
            }
        };
    }
]);