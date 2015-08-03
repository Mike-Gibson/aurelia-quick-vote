import {computedFrom} from 'aurelia-framework';
import {inject} from 'aurelia-framework';
import http from 'common/http';

import SignalRConnection from 'common/signalr'

@inject(SignalRConnection)
export class Voting {
  private signalrConnection: any;
  
  constructor(signalrConnection) {
    this.signalrConnection = signalrConnection
  }
  
  vote() {
    this.signalrConnection.vote('TEST VOTE');
  }  
}