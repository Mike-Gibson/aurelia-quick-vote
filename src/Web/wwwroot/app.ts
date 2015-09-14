import 'bootstrap';
import 'bootstrap/css/bootstrap.css!';

import {inject} from 'aurelia-framework';
import {Router} from 'aurelia-router';
import {RouterConfiguration, Redirect} from 'aurelia-router';
import {EventAggregator} from 'aurelia-event-aggregator';
import {LoggedInEvent} from 'common/events';
import {SignalRConnection} from 'common/signalr';

@inject(EventAggregator)
export class App {
  router: Router;
  
  constructor(eventAggregator: EventAggregator) {
    eventAggregator.subscribe(LoggedInEvent, this.loggedIn);
  }
  
  configureRouter(config: RouterConfiguration, router: Router){
    config.title = "Quick Vote";
    config.addPipelineStep('authorize', AuthorizeStep);
    config.map([
      { route: ['','login'], name: 'login',   moduleId: './login',  nav: true,  authenticated: false,  title: 'Login' },
      { route: ['voting'],   name: 'voting',  moduleId: './voting', nav: true,  authenticated: true,   title: 'Vote' }
    ]);

    this.router = router;
  }
  
  private loggedIn = (event: LoggedInEvent) => {
    this.router.navigate('voting');
  }
}

@inject(SignalRConnection)
class AuthorizeStep {
  private signalr: SignalRConnection;
  
  constructor(signalr: SignalRConnection) {
    this.signalr = signalr;
  }
  
  run(routingContext, next) {
    // Check if the route has an "authenticated" key
    // The reason for using `nextInstructions` is because this includes child routes.
    if (routingContext.nextInstructions.some(i => i.config.authenticated)) {
      var isLoggedIn = this.signalr.isLoggedIn();
      
      if (!isLoggedIn) {
        return next.cancel(new Redirect('login', null));
      }
    }

    return next();
  }
}