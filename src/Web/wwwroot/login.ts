import {inject} from 'aurelia-framework';
import {Router} from 'aurelia-router';
import {SignalRConnection} from 'common/signalr'

@inject(SignalRConnection, Router)
export class Login{
  name: string;
  private signalrConnection: SignalRConnection
  private router: Router; 
  
  constructor(signalrConnection, router: Router) {
    this.signalrConnection = signalrConnection;
    this.router = router;
    this.name = '';
  }
  
  canActivate() {
    return !this.signalrConnection.isLoggedIn();
  }
  
  login() {
    this.signalrConnection
      .login(this.name)
      .then(() => {        
        this.router.navigate('voting');
      })
      .catch(error => alert('Could not connect :( - ' + error));
  }  
}
