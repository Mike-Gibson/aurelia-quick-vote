import {bindable} from 'aurelia-framework';
import {inject} from 'aurelia-framework';
import {Router} from 'aurelia-router';
import {SignalRConnection} from 'common/signalr';

@inject(SignalRConnection)
export class NavBar {
  signalr: SignalRConnection;
  
  @bindable router: Router = null;
  
  constructor(signalr: SignalRConnection) {
    this.signalr = signalr;
  }
  
  get isLoggedIn() {
    return this.signalr.isLoggedIn();
  }
  
  get name() {
    return this.signalr.name;
  } 
  
  logout() {
    alert('Not Implemented');
  } 
}