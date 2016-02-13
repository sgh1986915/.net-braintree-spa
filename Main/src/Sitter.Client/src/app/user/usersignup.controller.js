angular.module('appControllers').controller('UserSignupController', [
    '$scope',
    'toaster',
    '$q',
    'constantsFactory',
    'userFactory',
    '$routeParams',
    '$location',
    function ($scope, toaster, $q, constantsFactory, userFactory, $routeParams, $location) {
        // Function to choose role in pop-up window.
        $scope.chooseRole = function (role) {
            $scope.SignupForm.Form.user.userRole = role;
        };
        // Function to submit signup.
        $scope.submitSignup = function () {
            $scope.SignupHelper.Processing = true;

            userFactory.signup($scope.SignupForm.Form).success(function (data) {
                $scope.SignupHelper.Processing = false;
                toaster.success('Sign Up successful', null, constantsFactory.toasterTimeout.success);
                userFactory.setCredentials(data.newUserData);
                if (data.newUserData.userRole == 'Parent') {
                    $location.path('/parent/myjobs');
                } else if (data.newUserData.userRole == 'Sitter'){
                    $location.path('/sitter/myjobs');
                } else if (data.newUserData.userRole == 'Admin') {
                    $location.path('/admin/sms-simulator');
                }
            }).error(function (error, httpstatus) {
                $scope.SignupHelper.Processing = false;
                toaster.error('Unable to sign Up', error.message, constantsFactory.toasterTimeout.error);
            });
        };

        var initScope = function () {
            var deferred = $q.defer();
            // Signup form.
            var watchOnce = $scope.$watch("SignupForm", function (newVal) {
                // TODO: make password/repeatPassword widget in separate directive.
                if (!newVal.Form) {
                    var now = new Date();
                    newVal.Form = {
                        user: {
                            userRole: '',
                            firstName: '',
                            lastName: '',
                            mobilePhone: '',
                            email: '',
                            TimezoneOffset: (-1)*now.getTimezoneOffset()/60
                        },
                        sitterSignupInfo: {
                            age: '',
                            parentMobile: '',
                            parentEmail: ''
                        },
                        pass: ''
                    };
                }
                watchOnce();
            });
            // Signup help variables.
            $scope.SignupHelper = {
                Processing: false,
                mobilePhonePattern: constantsFactory.mobilePhonePattern,
                ageOptions: [
                    {value: 12, label: '12'},
                    {value: 13, label: '13'},
                    {value: 14, label: '14'},
                    {value: 15, label: '15'},
                    {value: 16, label: '16'},
                    {value: 17, label: '17'},
                    {value: 18, label: '18'},
                    {value: 19, label: 'Over 18'} // TODO: ask Joseph about this option's value.
                ]
            };
            deferred.resolve(true);
            return deferred.promise;
        };

        initScope();
    }
]);