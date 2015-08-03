import 'bootstrap';
import 'bootstrap/css/bootstrap.css!';

import {Router} from 'aurelia-router';
import {RouterConfiguration} from 'aurelia-router';

export class App {
  router: Router;
  
  configureRouter(config: RouterConfiguration, router: Router){    
    config.map([
      { route: ['','login'],   name: 'login',        moduleId: './login',        nav: false, title: 'Login' },
      { route: ['voting'],     name: 'voting',       moduleId: './voting',       nav: false, title: 'Vote' }
    ]);

    (<any>router).title = "Quick Vote";

    this.router = router;
  }
}