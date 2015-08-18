import {computedFrom} from 'aurelia-framework';
import {inject} from 'aurelia-framework';
import http from 'common/http';

import {SignalRConnection} from 'common/signalr'

@inject(SignalRConnection)
export class Voting {
  private signalr: SignalRConnection;
  
  constructor(signalrConnection: SignalRConnection) {
    this.signalr = signalrConnection
  }
  
  vote() {
    this.signalr.vote('TEST VOTE');
  }
}