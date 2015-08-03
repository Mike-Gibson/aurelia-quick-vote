var gulp = require('gulp');
var paths = require('../paths');
var del = require('del');
var vinylPaths = require('vinyl-paths');

// deletes all files in the output path
gulp.task('clean', function() {
  return gulp.src([
    paths.output + '**/*.js', 
    '!' + paths.output + 'config.js',
    '!' + paths.output + 'jspm_packages/**/*.js',
    '!' + paths.output + 'Scripts/**/.js', 
    ])
    .pipe(vinylPaths(del));
});
