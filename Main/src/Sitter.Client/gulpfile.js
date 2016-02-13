/************************************************************
 * COMMANDS:
 *  DEV:
 *      watch_dev - compiles vendors/scripts and watches
 *      for changes inside scripts directory.
 *  LIVE:
 *      run_live - compiles vendors/scripts
 ************************************************************/

var config = require('./gulp.config')();
var gulp = require('gulp'),
    runSequence = require('run-sequence'),
    less = require('gulp-less'),
    twig = require('gulp-twig'),
    uglify = require('gulp-uglify'),
    concat = require('gulp-concat'),
    clean = require('gulp-clean'),
    concatSourcemap = require('gulp-concat-sourcemap'),
    sourcemaps = require('gulp-sourcemaps'),
    argv = require('yargs').argv,
    gulpif = require('gulp-if');

var live = argv.live,
    dev = argv.dev;
// By default we use dev environment.
if (!argv.dev && !argv.live) {
    dev = true;
}

/*
 * COMMON TASKS.
 */

gulp.task('clean_build', function () {
    return gulp.src(config.buildPath, {read: false})
        .pipe(clean());
});

gulp.task('copy_app', function () {
    return gulp.src(config.scriptsPath + "**/*.html", {read: true})
        .pipe(gulp.dest(config.newScriptsPath));
});

gulp.task('copy_thirdparty', function () {
    return gulp.src(config.thirdPartyPath + "**/*.*", {read: true})
        .pipe(gulp.dest(config.buildPath + "thirdparty"));
});

gulp.task('copy_content', function () {
    return gulp.src(config.contentPath + "**/*.*", {read: true})
        .pipe(gulp.dest(config.buildPath + "content"));
});

gulp.task('build_html', function () {
    return gulp.src(config.twigPath)
        .pipe(twig({
            data: {
                compile_timestamp: live ? config.compile_timestamp : ""
            }
        }))
        .pipe(gulp.dest(config.buildPath));
});

gulp.task('fonts', function () {
    return gulp.src(config.vendorFiles.fonts)
        .pipe(gulp.dest(config.buildPath + 'fonts'));
});

gulp.task('vendors_js', function () {
    return gulp.src(config.vendorFiles.js)
        .pipe(concat('vendors.js'))
        .pipe(gulpif(live, uglify()))
        .pipe(gulp.dest(config.buildPath));
});

gulp.task('vendors_css', function () {
    return gulp.src(config.vendorFiles.css)
        .pipe(concat('vendors.css'))
        .pipe(gulp.dest(config.buildPath + 'css'));
});

gulp.task('custom_js', function () {
    return gulp.src(
        [].concat(
            config.scriptFiles.js,
            live ? config.scriptFiles.jsGoogleLive : config.scriptFiles.jsGoogleDev,
            live ? config.scriptFiles.jsLive : config.scriptFiles.jsDev
        ))
        .pipe(gulpif(live, concat('app.js')))
        .pipe(gulpif(dev, concatSourcemap('app.js', {sourcesContent: true})))
        .pipe(gulpif(live, uglify()))
        .pipe(gulp.dest(config.buildPath));
});

gulp.task('custom_css', function () {
    return gulp.src(config.lessFiles)
        .pipe(less())
        .pipe(gulp.dest(config.buildPath + 'css'));
});

gulp.task('run', function (callback) {
    return runSequence('clean_build', 'copy_app', 'copy_thirdparty', 'copy_content', 'build_html', 'fonts', 'vendors_js', 'vendors_css', 'custom_js', 'custom_css', callback);
});

/*
 * DEV ENVIRONMENT.
 */

gulp.task('watcher', function (callback) {
    gulp.watch(config.scriptsPath + '**/*.*', ['copy_app', 'custom_js']);
    gulp.watch(config.lessPath + '**/*.*', ['custom_css']);
});

gulp.task('watch', function () {
    // No need to add --dev key. "watch" task is always for dev environment.
    dev = true;
    return runSequence('run', 'watcher');
});