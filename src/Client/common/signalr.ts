//import $ from 'jquery';
import 'dfrencham/ms-signalr-client';

export class SignalRConnection {
  private connection: any;
  private hub: any;
  
  login(name: string) {
    if (this.connection || this.hub) {
      throw new Error('already connected');
    }
    
    var connection = $.hubConnection("http://localhost:9001/signalr", { useDefaultPath: false });
    var hub = connection.createHubProxy('voting');
    
    hub.on('userConnected', function(name) {
      console.log(name);
    });
    
    hub.on('userVoted', function(name) {
      console.log(name + ' voted!');
    });
    
    this.hub = hub;
    this.connection = connection;
    
    connection.start()
      .done(function(){ alert('Now connected, connection ID=' + connection.id); 
    
        hub.invoke('login', name)
        .done(function (result) {
          alert('logged in: ' + result);
        }).fail(function (error) {
          alert('error while logging in - ' + error);
        });
        
        
      }).fail(function(){ alert('Could not connect'); });
  }
  
  vote(vote: string) {
    this.hub.invoke('vote', 'TEST VOTE')
      .done(function () {
        alert('voted!');
      }).fail(function (error) {
        alert('error while voting - ' + error);
      });

  }    
}

export default new SignalRConnection();