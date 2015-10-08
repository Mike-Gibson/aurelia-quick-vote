import {computedFrom} from 'aurelia-framework';
import {inject} from 'aurelia-framework';
import http from 'common/http';
import {SignalRConnection} from 'common/signalr'
import * as events from 'common/events';
import {EventAggregator} from 'aurelia-event-aggregator';

@inject(SignalRConnection, EventAggregator)
export class Voting {
  
  currentVote: string;
  newVote: string;
  question: IQuestion;
  people: Array<IPerson>;
  
  get status() {
    var totalPeople = this.people.length;
    var votedPeople = this.people.filter(p => p.hasVoted).length;
    
    return {
      totalPeople: totalPeople,
      votedPeople: votedPeople,
      percentage: Math.round((votedPeople / totalPeople) * 100) + '%'
    }
  }
  
  private signalr: SignalRConnection;
  
  constructor(signalrConnection: SignalRConnection, eventAggregator: EventAggregator) {
    this.signalr = signalrConnection
    this.newVote = '';
    this.currentVote = '';
    this.people = [];
    
    signalrConnection
      .getCurrentStatus()
      .then(status => {
        this.question = (status.CurrentQuestion === null) ? null : { 
          title: status.CurrentQuestion.Title, 
          active: status.CurrentQuestion.Active, 
          results: this.mapQuestionResults(status.CurrentQuestion.Results) 
        };
        
        this.people = status.People.map(p => { return { name: p.Name, hasVoted: p.HasVoted }; });
        this.sortPeople();
        
        if (status.MyCurrentVote !== null) {
          this.currentVote = status.MyCurrentVote;
          this.newVote = status.MyCurrentVote;
        }
      });
    
    eventAggregator.subscribe(events.UserConnectedEvent, this.userConnected);
    eventAggregator.subscribe(events.UserDisconnectedEvent, this.userDisconnected);
    eventAggregator.subscribe(events.UserVotedEvent, this.userVoted);
    eventAggregator.subscribe(events.VoteEndedEvent, this.voteEnded);
    eventAggregator.subscribe(events.VoteStartedEvent, this.voteStarted);
  }
  
  submitVote() {
    this.signalr.vote(this.newVote).then(() => {
      this.currentVote = this.newVote;
    });
  }
  
  userConnected = (message: events.UserConnectedEvent) => {
    var person = this.people.find(p => p.name === message.name);
    
    if (!person) {
      this.people.push({name: message.name, hasVoted: false});
      this.sortPeople();
    }
  }
  
  userDisconnected = (message: events.UserDisconnectedEvent) => {
    var personIndex = this.people.reduce((prev, cur, index) => {
      return (cur.name === message.name) ? index : prev;
    }, -1);
    
    if (personIndex > -1) {
      this.people.splice(personIndex, 1);
    }
  }
  
  userVoted = (message: events.UserVotedEvent) => {
    var person = this.people.find(p => p.name === message.name);
    
    if (person) {
      person.hasVoted = true;
    }
  }
  
  voteEnded = (message: events.VoteEndedEvent) => {
    this.question.active = false;
    this.question.results = this.mapQuestionResults(message.data);
  }
  
  voteStarted = (message: events.VoteStartedEvent) => {
    this.question = {
      title: message.questionTitle,
      active: true,
      results: []
    };
    this.people.forEach(p => p.hasVoted = false);
    this.currentVote = '';
    this.newVote = '';
  }
  
  temp_endVote() {
    this.signalr.endVote();
  }
  
  temp_startVote() {
    this.signalr.startVote('a new question - ' + new Date());
  }
  
  private mapQuestionResults(results: Array<IQuestionResult | { Name: string; Vote: string}>) : Array<IQuestionResult> {
    var input = results || [];
    
    return input.map(data => { 
      return {
        name: data['Name'] || data['name'],
        result: data['Vote'] || data['result']
      };
    }).sort((r1, r2) => {
      if (!r1.result && !r2.result)
        return (r1.name || '').localeCompare(r2.name);
      
      if (r1.result && !r2.result)
        return -1;
        
      if (!r1.result && r2.result)
        return 1;
        
      return (r1.result).localeCompare(r2.result);
    });
  }
  
  private sortPeople() {
    this.people.sort((p1, p2) => p1.name.localeCompare(p2.name));
  }
}

interface IQuestion {
  title: string;
  active: boolean;
  results: Array<IQuestionResult>;
}

interface IQuestionResult {
  name: string;
  result: string;
}

export interface IPerson {
  name: string;
  hasVoted: boolean;
}