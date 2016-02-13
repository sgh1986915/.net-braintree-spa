angular.module('appDirectives').directive('sitterTimeDate', [
    "$filter",
    "$timeout",
    "$q",
    function ($filter, $timeout, $q) {
        return {
            templateUrl: 'app/components/directives/sittertimedate/sittertimedate.html',
            restrict: 'E',
            replace: true,
            transclude: true,
            scope: {
                startModel: "=",
                endModel: "=",
                showDate: "=",
                restrictPastTime: "="
            },
            link: function (scope, elm, attrs) {
                // Timeout needed for debugging only.
                //$timeout(function () {
                    // Function to get right time option by hours and minutes.
                    var getTimeOption = function (h, m) {
                        var options = $filter('filter')(scope.TimeHelper.timeOptions, {h: h, m: m}, true);
                        return options ? options[0] : scope.TimeHelper.timeOptions[0];
                    };
                    // Function to generate time options.
                    var generateTimeOptions = function () {
                        var options = [];
                        var date = new Date();
                        date.setHours(0, 0, 0, 0);
                        for (var i = 0; i < 48; i++) {
                            var option = {
                                weight: i,
                                label_from: $filter('date')(date, "h:mma"),
                                h: Math.floor(i / 2),
                                m: (i % 2) * 30
                            };
                            options.push(option);
                            date.setMinutes(date.getMinutes() + 30, 0, 0);
                        }
                        return options;
                    };
                    // Function to set right duration.
                    var setDuration = function () {
                        if (scope.startModel instanceof Date && scope.endModel instanceof Date) {
                            scope.TimeHelper.duration = scope.endModel - scope.startModel;
                        } else {
                            scope.TimeHelper.duration = undefined;
                        }
                    };
                    // Function to work on startModel.
                    var processStartModel = function (newVal, oldVal) {
                        // Manage past dates.
                        if (scope.restrictPastTime) {
                            var curTime = new Date();
                            if (newVal < curTime) {
                                newVal = curTime;
                            }
                        }
                        // Process date to fit 30 min interval.
                        if (newVal.getMinutes() % 30 != 0) {
                            newVal.setMinutes(Math.ceil(newVal.getMinutes()/30)*30, 0, 0);
                        }
                        // Set Day.
                        scope.TimeHelper.DayHoursMins.Day = new Date(newVal);
                        // Set timeOption.
                        scope.TimeHelper.DayHoursMins.timeFrom = getTimeOption(newVal.getHours(), newVal.getMinutes());
                        resfreshToLabels();
                        // Set new endDate - not needed now, left empty.
                        /*var newEndDate = angular.copy(newVal);
                        newEndDate.setHours(scope.TimeHelper.DayHoursMins.timeFrom.h + 3, scope.TimeHelper.DayHoursMins.timeFrom.m, 0, 0);
                        scope.endModel = newEndDate;*/
                    };
                    // Function to refresh "to" labels for options.
                    var resfreshToLabels = function () {
                        var startOption = scope.TimeHelper.DayHoursMins.timeFrom;
                        if (startOption) {
                            date = new Date();
                            for (var i = 0; i < scope.TimeHelper.timeOptions.length; i++) {
                                date.setHours(startOption.h + scope.TimeHelper.timeOptions[i].h, startOption.m + scope.TimeHelper.timeOptions[i].m, 0, 0);
                                if (startOption.h + scope.TimeHelper.timeOptions[i].h + (startOption.m + scope.TimeHelper.timeOptions[i].m) / 60 > 24) {
                                    nextDay = " Next Day";
                                } else {
                                    nextDay = "";
                                }
                                scope.TimeHelper.timeOptions[i].label_to = $filter('date')(date, "h:mma") + nextDay;
                            }
                        }
                    };

                    var initScope = function () {
                        var deferred = $q.defer();

                        scope.TimeHelper = {
                            timeOptions: generateTimeOptions(),
                            DayHoursMins: {
                                Day: null,
                                timeFrom: null,
                                timeTo: null
                            },
                            duration: null,
                            Datepicker: {
                                minDate: new Date(),
                                datepickerPopupOptions: {
                                    format: "EEEE, MMMM d, yyyy"
                                },
                                openCalendar: function ($event, calendar) {
                                    $event.preventDefault();
                                    $event.stopPropagation();
                                    if (calendar == "start") {
                                        scope.TimeHelper.Datepicker.opened.end = false;
                                    } else if (calendar == "end") {
                                        scope.TimeHelper.Datepicker.opened.start = false;
                                    }
                                    scope.TimeHelper.Datepicker.opened[calendar] = !scope.TimeHelper.Datepicker.opened[calendar];
                                },
                                opened: {
                                    start: false,
                                    end: false
                                }
                            }
                        };
                        scope.$watch("startModel", function (newVal, oldVal) {
                            if (newVal instanceof Date) {
                                processStartModel(newVal, oldVal);
                            }
                        }, true);
                        scope.$watch("endModel", function (newVal, oldVal) {
                            if (scope.startModel instanceof Date && newVal instanceof Date) {
                                setDuration();
                                // Try to select right time option.
                                var hours = Math.floor((newVal - scope.startModel) / 1000 / 60 / 60);
                                var mins = Math.floor(((newVal - scope.startModel) / 1000 / 60 - hours * 60) / 30) * 30;
                                scope.TimeHelper.DayHoursMins.timeTo = getTimeOption(hours, mins);
                            }
                        }, true);
                        // Watch for changes in hoursmins selectlist to change end/start time models.
                        scope.$watch("TimeHelper.DayHoursMins", function (newVal, oldVal) {
                            if (newVal && newVal.Day instanceof Date && scope.startModel instanceof Date) {
                                // If we changed a Day we need to clear all other fields.
                                if (!(oldVal.Day instanceof Date) || newVal.Day.getDate() != oldVal.Day.getDate()) {
                                    newVal.timeFrom = newVal.timeTo = scope.startModel = scope.endModel = undefined;
                                    setDuration();
                                } else if (newVal.timeFrom && newVal.timeTo) {
                                    if (!oldVal || newVal.timeFrom !== oldVal.timeFrom) {
                                        scope.startModel.setDate(newVal.Day.getDate());
                                        scope.startModel.setHours(newVal.timeFrom.h, newVal.timeFrom.m, 0, 0);
                                    }
                                    if (!oldVal || newVal.timeTo !== oldVal.timeTo) {
                                        scope.endModel = angular.copy(scope.startModel);
                                        scope.endModel.setDate(newVal.Day.getDate());
                                        scope.endModel.setHours(newVal.timeFrom.h + newVal.timeTo.h, newVal.timeFrom.m + newVal.timeTo.m, 0, 0);
                                    }
                                }
                            } else if (newVal.timeFrom && newVal.Day instanceof Date && !(scope.startModel instanceof Date)) {
                                var start = new Date();
                                var a = newVal.Day.getDate();
                                start.setDate(newVal.Day.getDate());
                                start.setHours(newVal.timeFrom.h, newVal.timeFrom.m, 0, 0);
                                scope.startModel = start;
                            }
                        }, true);

                        deferred.resolve(true);
                        return deferred.promise;
                    };

                    var setupScope = function () {
                        // Change startModel to fit 30 mins interval.
                        if (scope.startModel instanceof Date) {
                            scope.startModel.setMinutes(Math.ceil(scope.startModel.getMinutes() / 30)*30, 0, 0);
                        }
                    };

                    initScope().then(setupScope);
                //}, 400);
            }
        }
    }
]).filter('in_future_only', function () {
    return function (options, day) {
        var output = [];
        if (day instanceof Date) {
            var cur_time = new Date();
            var hours = cur_time.getHours();
            var minutes = cur_time.getMinutes();
            // If we use day in future all options should be OK.
            var dayValid = day.getFullYear() > cur_time.getFullYear() ||
                (day.getFullYear() == cur_time.getFullYear() &&
                    (day.getMonth() > cur_time.getMonth() ||
                        (day.getMonth() == cur_time.getMonth() && day.getDate() > cur_time.getDate())
                    )
                );
            if (options) {
                for (var i = 0; i < options.length; i++) {
                    if (dayValid || (options[i].h > hours || (options[i].h == hours && options[i].m > minutes))) {
                        output.push(options[i]);
                    }
                }
            }
        }
        return output;
    }
}).filter('time_option_labels', [
    "$filter",
    function ($filter) {
        return function (options, fromTo, startOption) {
            var date = new Date();
            switch (fromTo) {
                case "from":
                    for (var i = 0; i < options.length; i++) {
                        date.setHours(options[i].h, options[i].m, 0, 0);
                        options[i].label_from = $filter('date')(date, "h:mma");
                    }
                    break;
                case "to":
                    if (startOption) {
                        for (var i = 0; i < options.length; i++) {
                            date.setHours(startOption.h + options[i].h, startOption.m + options[i].m, 0, 0);
                            if (startOption.h + options[i].h + (startOption.m + options[i].m) / 60 > 24) {
                                nextDay = " Next Day";
                            } else {
                                nextDay = "";
                            }
                            options[i].label_to = $filter('date')(date, "h:mma") + nextDay;
                        }
                        break;
                    }
            }
            return options;
        }
    }]);