angular.module('appFactories').factory('jobFactory',
    function () {
        return {

            calculatePrice: function (data) {
                data.bonus = data.bonus ? parseFloat(data.bonus) : 0;
                data.ratePerHour = data.ratePerHour ? parseFloat(data.ratePerHour) : 0;
                var hours = (data.endTime - data.startTime)/(1000*60*60);
                var payToSitter = hours*data.ratePerHour + data.bonus;
                var multiplier = 1.1; // 10%
                var payTotal = payToSitter*multiplier;
                // Now we need to round to 2 digits after ".".
                payToSitter = Math.round(payToSitter*100)/100;
                payTotal = Math.round(payTotal*100)/100;
                return {
                    payToSitter: payToSitter,
                    payTotal: payTotal,
                    duration:hours
                };
            }
        };
    }
);