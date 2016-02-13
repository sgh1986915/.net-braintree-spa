module.exports = function() {
    var appPath = './src/';
    var bowerPath = appPath + 'bowerlib/';
    var scriptsPath = appPath + 'app/';
    var lessPath = appPath + 'content/less/';
    var buildPath = appPath + 'build/';
    var thirdPartyPath = appPath + 'thirdparty/';
    var contentPath = appPath + 'content/';

    var config = {
        thirdPartyPath: thirdPartyPath,
        contentPath: contentPath,
        appPath: appPath,
        bowerPath: bowerPath,
        newScriptsPath: buildPath + 'app/',
        scriptsPath: scriptsPath,//scriptsPath,  ---> now we firstly copy all js files into build
        lessPath: lessPath,
        buildPath: buildPath,
        compile_timestamp: "?compile_timestamp=" + (new Date()).getTime(),
        twigPath: scriptsPath + 'index.twig',
        // ALL contrib JS libs.
        vendorFiles: {
            js: [
                bowerPath + 'angular/angular.js',
                bowerPath + 'angular-cookie/angular-cookie.js',
                bowerPath + 'angular-bootstrap-show-errors/src/showErrors.js',
                bowerPath + 'angular-animate/angular-animate.js',
                bowerPath + 'moment/moment.js',
                bowerPath + 'extras.angular.plus/ngplus-overlay.js',
                bowerPath + 'angular-route/angular-route.js',
                bowerPath + 'angular-ui-select/dist/select.js',
                bowerPath + 'extras.angular.plus/ngplus-overlay.js',
                bowerPath + 'ng-sortable/dist/ng-sortable.js',
                bowerPath + 'angular-bootstrap/ui-bootstrap.js',
                bowerPath + 'angular-bootstrap/ui-bootstrap-tpls.js',
                bowerPath + 'angularjs-toaster/toaster.js',
                bowerPath + 'angular-background/src/js/angular-background.js',
            ],
            css: [
                bowerPath + 'bootstrap/dist/css/bootstrap.css',
                bowerPath + 'bootstrap/dist/css/bootstrap-theme.css',
                bowerPath + 'font-awesome/css/font-awesome.min.css',
                bowerPath + 'angularjs-toaster/toaster.css',
                bowerPath + 'angular-ui-select/dist/select.css',
                bowerPath + 'ng-sortable/dist/ng-sortable.css',
                appPath + 'thirdparty/selectize/selectize.default.css',
                bowerPath + 'ng-sortable/dist/ng-sortable.style.css',
            ],
            fonts: [
                bowerPath + 'bootstrap/fonts/glyphicons-halflings-regular.woff',
                bowerPath + 'bootstrap/fonts/glyphicons-halflings-regular.ttf',
            ]
        },
        scriptFiles: {
            js: [
                scriptsPath + 'app.module.js',
                scriptsPath + 'admin/**/*.js',
                scriptsPath + 'globals/**/*.js',
                scriptsPath + 'admin/**/*.js',
                scriptsPath + 'pagemarkup/**/*.js',
                scriptsPath + 'parent/**/*.js',
                scriptsPath + 'shell/**/*.js',
                scriptsPath + 'sitter/**/*.js',
                scriptsPath + 'user/**/*.js',
                scriptsPath + 'components/constants/**/*.js',
                scriptsPath + 'components/directives/sittertimedate/*.js',
                scriptsPath + 'components/directives/emailphoneformat/*.js',
                scriptsPath + 'components/job/**/*.js',
                scriptsPath + 'components/popupmessages/**/*.js',
            ],
            jsLive: [
                scriptsPath + 'components/directives/phoneformat.live/*.js',
            ],
            jsDev: [
                scriptsPath + 'components/directives/phoneformat.dev/*.js',
            ],
            jsGoogleDev: [
                scriptsPath + 'components/google/dev/**/*.js',
            ],
            jsGoogleLive: [
                scriptsPath + 'components/google/live/**/*.js',
            ]
        },
        lessFiles: [lessPath + 'app.less']
    };

    return config;
};
