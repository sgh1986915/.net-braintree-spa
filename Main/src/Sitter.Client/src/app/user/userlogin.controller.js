angular.module('appControllers').controller('UserLoginController', [
    '$scope',
    'userFactory',
    '$rootScope',
    '$routeParams',
    'toaster',
    '$location',
    function ($scope, userFactory, $rootScope, $routeParams, toaster, $location) {
        // Function to submit login form.
        $scope.login = function () {
            $scope.LoginHelper.Processing = true;
            userFactory.clearCredentials();
            userFactory.login($scope.LoginForm.Form)
                .success(function (data, status, headers, config) {
                    userFactory.setCredentials(data, $scope.LoginForm.Form.rememberMe);
                    $location.search('logout', null); //remove logout parameter from URL
                    if (data.userRole == 'Parent') {
                        $location.path('/parent/myjobs');
                    } else if (data.userRole == 'Sitter'){
                        $location.path('/sitter/myjobs');
                    } else if (data.userRole == 'Admin') {
                        $location.path('/admin/sms-simulator');
                    }
                    $scope.LoginHelper.Processing = false;
                }).error(function (error, status, headers, config) {
                    //toaster.warning("login unsuccessful");
                    $scope.LoginForm.Form.errorResponse = error;
                    $scope.LoginHelper.Processing = false;
                });
        };

        $scope.logout = userFactory.clearCredentials;

        var initScope = function () {
            var watchOnce = $scope.$watch("LoginForm", function (newVal) {
                if (!newVal.Form) {
                    newVal.Form = {
                        errorResponse: '',
                        username: null,
                        password: null,
                        rememberMe: false
                    };
                }
                watchOnce();
            });
            $scope.LoginHelper = {
                Processing: false
            };

            var logout = $location.search().logout;

            if (logout) {
                userFactory.clearCredentials();
                $location.path('/');
            }
        };

        initScope();
    }
]);