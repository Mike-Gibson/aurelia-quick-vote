import 'jquery';
import 'dfrencham/ms-signalr-client';
import {inject} from 'aurelia-framework';
import {EventAggregator} from 'aurelia-event-aggregator';
import * as events from 'common/events';

@inject(EventAggregator)
export class SignalRConnection {
  private eventAggregator: EventAggregator;
  private connection: HubConnection;
  private hub: HubProxy;
  
  private loggedIn: boolean;
  private username: string;
  
  constructor(eventAggregator: EventAggregator) {
    this.eventAggregator = eventAggregator;
    this.loggedIn = false;
    
    var connection = $.hubConnection("/signalr", { useDefaultPath: false });
    var hub = connection.createHubProxy('voting');
    
    this.wireUpHubMethods(hub);
    
    this.hub = hub;
    this.connection = connection;
    
    this.attemptAutoLogin();
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
            .done(() => {
              //alert('logged in: ' + result);
              this.loggedIn = true;
              this.username = name;
              
              this.persistedUsername = name;
              resolve('Logged in');
              this.eventAggregator.publish(new events.LoggedInEvent());
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
  
  logout() {
    if (!this.loggedIn) {
      throw new Error('Not logged in');
    }
    
    return this.hub.invoke('logout')
      .then(() => {
        this.loggedIn = false;
        this.username = '';
        this.persistedUsername = null;
      })
      .fail(function (error) {
        alert('error while logging out - ' + error);
      });
  }
  
  getCurrentStatus(): Promise<any> {
    return new Promise((resolve, reject) => {
      this.hub.invoke('getCurrentStatus')
        .done(data => {
          resolve(data);
        }).fail(error => {
          alert('getCurrentStatus error - ' + error);
          reject(error);
        });
      });
  }
  
  vote(vote: string) {
    return this.hub.invoke('vote', vote)
      .fail(function (error) {
        alert('error while voting - ' + error);
      });
  }
  
  // TODO: Should be authorised
  endVote() {
    return this.hub.invoke('endVote')
      .fail(function (error) {
        alert('error while ending vote - ' + error);
      });
  }
  
  // TODO: Should be authorised
  startVote(questionTitle: string) {
    return this.hub.invoke('startVote', questionTitle)
      .fail(function (error) {
        alert('error while starting vote - ' + error);
      });
  }
  
  private wireUpHubMethods(hub: HubProxy) {
  	hub.on('userConnected', (name: string) => {
      this.eventAggregator.publish(new events.UserConnectedEvent(name));
    });
    
    hub.on('userDisconnected', (name: string) => {
      this.eventAggregator.publish(new events.UserDisconnectedEvent(name));
    });
    
    hub.on('userVoted', (name: string) => {
      this.eventAggregator.publish(new events.UserVotedEvent(name));
    });
    
    hub.on('voteStarted', (questionTitle: string) => {
      this.eventAggregator.publish(new events.VoteStartedEvent(questionTitle));
    });
    
    hub.on('voteEnded', (results: any) => {
      var data: events.IQuestionResult[] = results.map(r => { return { name: r.Name, result: r.Vote }; });
      this.eventAggregator.publish(new events.VoteEndedEvent(data));
    });
  }
  
  private get persistedUsername() {
    return localStorage.getItem('username');
  }
  
  private set persistedUsername(value: string) {
    localStorage.setItem('username', value);
    
    if (value == null) {
      localStorage.removeItem('username');
      return;
    }
  }
  
  private attemptAutoLogin() {
    if (this.persistedUsername !== null) {
      this.login(this.persistedUsername)
    }    
  }
}

export default SignalRConnection;