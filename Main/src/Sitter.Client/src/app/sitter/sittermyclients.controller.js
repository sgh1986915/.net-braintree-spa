angular.module('appControllers').controller('SitterMyClientsController', [
    '$scope',
    'toaster',
    '$q',
    'sitterFactory',
    '$filter',
    'constantsFactory',
    'popupMessagesFactory',
    'globalsFactory',
    function ($scope, toaster, $q, sitterFactory, $filter, constantsFactory, popupMessagesFactory, globalsFactory) {
        // Function to prepare clients data.
        var processClientsData = function (data) {
            for (var i = 0; i < data.length; i++) {
                data[i].photoUrlAdapted = data[i].photoUrl ? data[i].photoUrl : $scope.SitterClientsHelper.ClientImageUrl;
            }
            return data;
        };

        var initScope = function () {
            var deferred = $q.defer();
            // Sitters clients.
            $scope.SitterClients = {
                Clients: []
            };
            // Parent Sitters help variables.
            $scope.SitterClientsHelper = {
                Processing: false,
                ClientImageUrl: constantsFactory.defaultParentImageUrl,
                mobilePhonePattern: constantsFactory.mobilePhonePattern,
                NewClientMobile: null
            };
            deferred.resolve(true);
            return deferred.promise;
        };

        var setupScope = function () {
            $scope.SitterClientsHelper.Processing = true;
            var deferred = $q.defer();

            sitterFactory.getSitterMyClients().success(function (data) {
                $scope.SitterClients.Clients = processClientsData(data.myClients);
                $scope.SitterClientsHelper.NewClientMobile = null;
                $scope.SitterClientsHelper.Processing = false;
                deferred.resolve(true);
            }).error(function (error) {
                toaster.warning(error.message, 'Unable to retrieve sitters');
                $scope.SitterClientsHelper.Processing = false;
                deferred.reject(false);
            });

            return deferred.promise;
        };
        // Function to create new invite.
        $scope.sendInvite  = function () {
            //$scope.SitterClientsHelper.Processing = true;

            var clientInvite = {
                mobilePhone: $scope.SitterClientsHelper.NewClientMobile
            };
            /* SEND INVITE LOGIC.
            sitterFactory.sendInvite(clientInvite).success(function (response) {
                if (response.added) {
                    setupScope().then(function () {
                        $scope.SitterClientsHelper.Processing = false;
                    });
                    toaster.info("invited " + $scope.NewClientMobile);
                }else{
                    $scope.SitterClientsHelper.Processing = false;
                    toaster.warning("did not invite: " + response.message);
                }
            }).error(function(error){
                $scope.ParentSittersHelper.Processing = false;
            });
            */
        };

        globalsFactory.retrieveGlobalData($scope).then(
            // Success.
            function () {
                initScope().then(setupScope);
            },
            // Failure.
            function (error) {
                // TODO: popup with error?
                toaster.error(error.title, error.message, constantsFactory.toasterTimeout.error);
            });
    }
]);