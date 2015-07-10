import 'bootstrap';
import 'bootstrap/css/bootstrap.css!';

import {Router} from 'aurelia-router';
import {RouterConfiguration} from 'aurelia-router';

export class App {
  router: Router;
  
  configureRouter(config: RouterConfiguration, router: Router){    
    config.map([
      { route: ['','welcome'], name: 'welcome',      moduleId: './welcome',      nav: true, title:'Welcome' },
      { route: 'child-router', name: 'child-router', moduleId: './child-router', nav: true, title:'Child Router' }
    ]);

    (<any>router).title = "Aurelia";

    this.router = router;
  }
}