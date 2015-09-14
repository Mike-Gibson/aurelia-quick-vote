export class LoggedInEvent { }

export class UserVotedEvent {
	name: string;
	
	constructor(name: string) {
		this.name = name;
	}	
}

export class UserConnectedEvent {
	name: string;
	
	constructor(name: string) {
		this.name = name;
	}	
}

export class UserDisconnectedEvent {
	name: string;
	
	constructor(name: string) {
		this.name = name;
	}	
}

export interface IQuestionResult {
  name: string;
  result: string;
}

export class VoteEndedEvent {
	data: IQuestionResult[];
	
	constructor(data: IQuestionResult[]) {
		this.data = data;
	}	
}

export class VoteStartedEvent {
	questionTitle: string;
	
	constructor(questionTitle: string) {
		this.questionTitle = questionTitle;
	}	
}