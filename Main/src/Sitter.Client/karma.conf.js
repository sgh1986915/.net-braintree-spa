// Karma configuration
// Generated on Fri Nov 14 2014 15:35:59 GMT-0600 (Central Standard Time)

module.exports = function (config) {
    config.set({

        // base path that will be used to resolve all patterns (eg. files, exclude)
        basePath: '',

        // frameworks to use
        // available frameworks: https://npmjs.org/browse/keyword/karma-adapter
        frameworks: ['mocha', 'chai'],


        //'src/bowerlib/**/*.js',
        //'src/**/*min.js',

        // list of files / patterns to load in the browser
        files: [

            'node_modules/mocha/mocha.js',
            'node_modules/chai/chai.js',

            /* Application vendor dependencies */
            'src/bowerlib/jquery/dist/jquery.js',
            'src/bowerlib/signalr/jquery.signalR.js',
            'src/bowerlib/angular/angular.js',
            'src/bowerlib/angular-animate/angular-animate.js',
            'src/bowerlib/angular-route/angular-route.js',
            'src/bowerlib/angular-sanitize/angular-sanitize.js',
            'src/bowerlib/bootstrap/dist/js/bootstrap.js',
            'src/bowerlib/toastr/toastr.js',
            'src/bowerlib/moment/moment.js',

            'src/bowerlib/angular-mocks/angular-mocks.js',
            'src/bowerlib/extras.angular.plus/ngplus-overlay.js',
            'src/thirdparty/kendoui/2014.3.1119/js/kendo.all.min.js',

            /* MOCHA TEST */
            'src/app/app.module.js',
            'src/app/**/*.module.js',
            'src/app/**/*.js',
            'test/unit/**/*_spec.js'
        ],


        // list of files to exclude
        exclude: [],


        // preprocess matching files before serving them to the browser
        // available preprocessors: https://npmjs.org/browse/keyword/karma-preprocessor
        preprocessors: {
            'src/app/**/*.js': 'coverage'
        },

        //'test/unit/**/*_spec.js': ['browserify'],
        //'src/bowerlib/angular/angular.js': ['browserify']

        // test results reporter to use
        // possible values: 'dots', 'progress'
        // available reporters: https://npmjs.org/browse/keyword/karma-reporter
        reporters: ['progress', 'junit', 'html', 'coverage', 'teamcity'],

        htmlReporter: {
            outputFile: 'report/results/unit.html'
        },

        junitReporter: {
            outputFile: 'report/results/unit.xml',
            suite: ''
        },
        coverageReporter: {
            type: 'html',
            dir: 'report/results/coverage'
        },

        // web server port
        port: 9876,


        // enable / disable colors in the output (reporters and logs)
        colors: true,


        // level of logging
        // possible values: config.LOG_DISABLE || config.LOG_ERROR || config.LOG_WARN || config.LOG_INFO || config.LOG_DEBUG
        logLevel: config.LOG_INFO,


        // enable / disable watching file and executing tests whenever any file changes
        autoWatch: true,


        // start these browsers
        // available browser launchers: https://npmjs.org/browse/keyword/karma-launcher
        browsers: ['PhantomJS'],


        // Continuous Integration mode
        // if true, Karma captures browsers, runs the tests and exits
        singleRun: true,


        // browserify configuration
        browserify: {
            debug: true
        }
    });
};
