export function configure(aurelia) {
  aurelia.use
    .standardConfiguration()
    .developmentLogging()
    .eventAggregator()
    .plugin('aurelia-animator-css');

  aurelia.start().then(a => a.setRoot());
}
