import {computedFrom} from 'aurelia-framework';
import {inject} from 'aurelia-framework';
import http from 'common/http';
import {Router} from 'aurelia-router';

import SignalRConnection from 'common/signalr'

@inject(SignalRConnection, Router)
export class Login{
  name: string;
  private signalrConnection: any
  private router: Router; 
  
  constructor(signalrConnection, router: Router) {
    this.signalrConnection = signalrConnection;
    this.router = router;
    this.name = '';
  }
  
  login() {
    this.signalrConnection.login(this.name);
    this.router.navigate('voting');
  }  
}
