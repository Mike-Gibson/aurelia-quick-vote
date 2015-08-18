import 'jquery';
import 'dfrencham/ms-signalr-client';

export class SignalRConnection {
  private connection: HubConnection;
  private hub: HubProxy;
  
  private loggedIn: boolean;
  private username: string;
  
  constructor() {
    this.loggedIn = false;
    
    var connection = $.hubConnection("/signalr", { useDefaultPath: false });
    var hub = connection.createHubProxy('voting');
    
    this.wireUpHubMethods(hub);
    
    this.hub = hub;
    this.connection = connection;
  }
  
  isLoggedIn() {
    return this.loggedIn;
  }
  
  get name() {
    return this.username;
  }
  
  login(name: string) {
    if (this.loggedIn) {
      throw new Error('already logged in');
    }
    
    var promise = new Promise((resolve, reject) => {
      this.connection
        .start()
        .done(() => {
          // alert('Now connected, connection ID=' + connection.id);          
          this.hub.invoke('login', name)
            .done((loggedIn: boolean) => {
              //alert('logged in: ' + result);
              this.loggedIn = loggedIn;
              this.username = name;
              
              if (loggedIn) {
                resolve('Logged in');
              } else {
                this.connection.stop();
                reject('Connected, but could not login');
              }
            })
            .fail(error => {            
              //alert('error while logging in - ' + error);
              reject('Could not login: ' + error);
            });
        })
	    .fail(() => { 
          //alert('Could not connect');
          reject('Could not connect') 
        });
    })
    
    return promise;
  }
  
  vote(vote: string) {
    this.hub.invoke('vote', 'TEST VOTE')
      .done(function () {
        alert('voted!');
      }).fail(function (error) {
        alert('error while voting - ' + error);
      });
  }    
  
  private wireUpHubMethods(hub: HubProxy) {
  	hub.on('userConnected', function(name) {
      console.log(name);
    });
    
    hub.on('userVoted', function(name) {
      console.log(name + ' voted!');
    });
  }
}

export default new SignalRConnection();