angular.module('appControllers').controller('SitterMyJobsController', [
    '$scope',
    'toaster',
    '$q',
    'sitterFactory',
    '$filter',
    'popupMessagesFactory',
    'globalsFactory',
    function ($scope, toaster, $q, sitterFactory, $filter, popupMessagesFactory, globalsFactory) {
        // Function to prepare jobs data.
        var processJobsData = function (data) {
            for (var i = 0; i < data.length; i++) {
                data[i].dateTimeStart = new Date(data[i].job.start);
                var de = new Date(data[i].dateTimeStart);
                de.setHours(de.getHours() + data[i].job.duration);
                data[i].dateTimeEnd = de;

                // Collapsed details by default.
                data[i].collapsed = true;
            }
            return data;
        };
        // Function to handle click on "more"/"less" button.
        $scope.expandCollapseClosedJobs = function (val) {
            $scope.SitterJobsHelper.morePressed += val;
        };
        // Function to cancel job.
        $scope.cancelJob = function (job) {
            popupMessagesFactory.simpleConfirm({
                title: "Cancel job",
                body: "Are you sure you want to cancel this job?",
                ok: "Yes",
                cancel: "No",
                successCallback: function () {
                    sitterFactory.renegeAcceptedJob(job.id).success(function(data) {
                        popupMessagesFactory.simpleConfirm({
                            body: "Sorry to hear you have to cancel this job. Parent1 will be notified that you can no longer babysit on " + $filter('date')(job.start, "M/dd/yy") + ".",
                            ok: "Ok",
                            successCallback: setupScope,
                            errorCallback: setupScope
                        });
                    }).error(function(re){

                    });
                }
            });
        };
        // Function to accept job.
        $scope.acceptJob = function (jobId) {
            //  Accept job logic goes here...
            // ... and then resolving by this popup:
            sitterFactory.jobInviteResponse(jobId, 'accept').success(function(data) {
                popupMessagesFactory.simpleConfirm({
                    title: "Job accepted",
                    body: "Congratulations! Parent will be notified that you accepted this job.",
                    ok: "OK"
                });
                setupScope();
            }).error(function(re) {

            });

        };
        // Function to decline job.
        $scope.declineJob = function (jobId) {
            sitterFactory.jobInviteResponse(jobId, 'decline').success(function(data){
                popupMessagesFactory.simpleConfirm({
                    title: "Job declined",
                    body: "No problem! Parent will be notified that you have declined this job.",
                    ok: "OK"
                });
                setupScope();
            }).error(function(re){

            });
        };
        // Function to request payment for job.
        $scope.requestPaymentForJob = function (jobId) {
            //  Request payment for job logic goes here...
            // ... and then resolving by this popup:
            popupMessagesFactory.simpleConfirm({
                title: "Payment has been requested",
                body: "Job well done? Awesome! Parent will be notified of the payment request.",
                ok: "OK",
                successCallback: function () {
                    sitterFactory.requestPayment(jobId).success(function(data){
                        popupMessagesFactory.simpleConfirm({body:"Text message sent to parent requesting payment",ok:"Ok"});
                    }).error(function(re){
                        logger.warning("Error while requesting parent " + re);
                    });
                }
            });
        };

        var initScope = function () {
            var deferred = $q.defer();
            // Parent Jobs.
            $scope.SitterJobs = [];
            // Parent Jobs help variables.
            $scope.SitterJobsHelper = {
                Processing: false,
                closedJobs: [],
                morePressed: 0,
                closedJobsInitOffset: 3,
                closedJobsPerExpand: 12
            };
            deferred.resolve(true);
            return deferred.promise;
        };

        var setupScope = function () {
            $scope.SitterJobsHelper.Processing = true;
            var deferred = $q.defer();

            sitterFactory.getSitterJobs().success(function (data) {
                $scope.SitterJobs = processJobsData(data);
                // Set up closedJobs.
                $scope.SitterJobsHelper.closedJobs = $filter('filter')($scope.SitterJobs, {state: "Closed"}, false);
                $scope.SitterJobsHelper.Processing = false;
                deferred.resolve(true);
            }).error(function (error, httpcode) {
                var msg = httpcode == '401' ? "unauthorized" : ""; //FutureDev: put this is an helper method. what is the best way to redirect when unauthorized?
                msg += error.message ? error.message : error;
                toaster.warning('Unable to retrieve jobs. ' + msg);
                $scope.SitterJobsHelper.Processing = false;
                deferred.reject(false);
            });

            return deferred.promise;
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