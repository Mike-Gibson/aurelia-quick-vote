var path = require('path');

var appRoot = 'src/Web/wwwroot/';
var outputRoot = 'src/Web/wwwroot/';

module.exports = {
  root: '/',
  source: [
    appRoot + '**/*.ts',
    'jspm_packages/**/*.d.ts' 
  ],
  html: appRoot + '**/*.html',
  style: appRoot + '**/*.css',
  output: outputRoot
};