var gulp = require('gulp');
var runSequence = require('run-sequence');
var changed = require('gulp-changed');
var plumber = require('gulp-plumber');
var to5 = require('gulp-babel');
var sourcemaps = require('gulp-sourcemaps');
var paths = require('../paths');
var compilerOptions = require('../babel-options');
var assign = Object.assign || require('object.assign');
var ts = require('gulp-typescript');
var replace = require('gulp-replace');

var tsProject = ts.createProject({
    noExternalResolve: true,
    noLib: false,
    typescript: require('typescript'), // version 1.5.0-beta
    target: 'es6',
    experimentalDecorators: true
});

// transpiles changed typescript to es6 to SystemJS format
// the plumber() call prevents 'pipe breaking' caused
// by errors from other gulp plugins
// https://www.npmjs.com/package/gulp-plumber
gulp.task('build-system', function () {
  return gulp.src(paths.source)
    .pipe(plumber())
    .pipe(sourcemaps.init())
    .pipe(ts(tsProject))
    .pipe(replace(/Object.defineProperty\(([^,]*), "name", { value: "\1", configurable: true }\);/, ''))
    .pipe(to5(assign({}, compilerOptions, {modules:'system'})))
    .pipe(replace(/undefined.__decorate/, '__decorate'))
    .pipe(sourcemaps.write({includeContent: false, sourceRoot: '/' + paths.root }))
    .pipe(gulp.dest(paths.output));
});

// copies changed html files to the output directory
gulp.task('build-html', function () {
  return gulp.src(paths.html)
    .pipe(changed(paths.output, {extension: '.html'}))
    .pipe(gulp.dest(paths.output));
});

// this task calls the clean task (located
// in ./clean.js), then runs the build-system
// and build-html tasks in parallel
// https://www.npmjs.com/package/gulp-run-sequence
gulp.task('build', function(callback) {
  return runSequence(
    'clean',
    ['build-system', 'build-html'],
    callback
  );
});