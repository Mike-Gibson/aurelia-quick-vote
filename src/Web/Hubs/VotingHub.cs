using System;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Framework.Logging;
using System.Threading.Tasks;

namespace WebAPIApplication.Hubs
{
    [HubName("voting")]
    public class VotingHub : Hub
    {
        public class UserInfo
        {
            public string Name { get; set; }
            public string Vote { get; set; }    
        }
        
        public class State
        {
            private readonly Dictionary<string, UserInfo> _users = new Dictionary<string, UserInfo> {
                { "dummy-connection-1", new UserInfo { Name = "Temp User 1" }},
                { "dummy-connection-2", new UserInfo { Name = "Temp User 2" }},
            };
            
            public string CurrentQuestionTitle { get; private set; }
            public bool CurrentQuestionActive { get; private set; } 
            
            public bool AddUser(string connectionId, string name) 
            {
                if (_users.Any(u => u.Value.Name == name))
                    return false;
                
                _users.Add(connectionId, new UserInfo { Name = name });
                return true;
            }
            
            public bool DeleteUser(string connectionId) 
            {
                return _users.Remove(connectionId);
            }
            
            public bool HijackUser(string connectionId, string name) 
            {
                var user = _users.SingleOrDefault(u => u.Value.Name == name);
                
                if (default(KeyValuePair<string, UserInfo>).Equals(user))
                    return false;
                
                _users.Remove(user.Key);
                _users.Add(connectionId, user.Value);
                return true;
            }
            
            public void NewVote(string newQuestionTitle) 
            {
                if (this.CurrentQuestionActive) {
                    throw new InvalidOperationException("Cannot start a new vote, as there is already a vote in progress");
                }
                
                this.CurrentQuestionTitle = newQuestionTitle;
                this.CurrentQuestionActive = true;
                _users.Values.ToList().ForEach(v => v.Vote = null);
            }
            
            public void EndVote() 
            {
                if (!this.CurrentQuestionActive) {
                    throw new InvalidOperationException("Cannot end the vote, as there is not an active vote");
                }
                
                this.CurrentQuestionActive = false;
            }
            
            public void Vote(string connectionId, string vote) 
            {
                _users[connectionId].Vote = vote;    
            }
            
            public string NameFor(string connectionId)
            {
                return _users[connectionId].Name;
            }
            
            public IEnumerable<dynamic> GetVotes() 
            {
                return _users.Values.Select(v => new { Name = v.Name, Vote = v.Vote });
            }
            
            public string GetUsersVote(string connectionId) {
                return _users[connectionId].Vote;
            }
            
            public dynamic GetCurrentStatus() {
                 return _users.Values
                    .ToList()
                    .Select(v => new {
                        Name = v.Name,
                        HasVoted = (v.Vote != null)
                    })
                    .ToList();
            }
        }
        
        private static readonly State _state = new State();
        private ILogger<VotingHub> _logger;
        public VotingHub(ILogger<VotingHub> logger) 
        {
            _logger = logger;
        }

        [HubMethodName("Login")]
        public bool Login(string name)
        {
            var added = _state.AddUser(Context.ConnectionId, name);
            var hijacked = added ? false : _state.HijackUser(Context.ConnectionId, name);
            
            if (!added && !hijacked) {
                return false;
            }
            
            Clients.All.userConnected(name);
            return true;
        }
        
        public override Task OnDisconnected(bool stopCalled)
        {
            var userName = CurrentUserName;
            
            _state.DeleteUser(Context.ConnectionId);
            Clients.All.userDisconnected(userName);
            
            return base.OnDisconnected(stopCalled);
        }
        
        [HubMethodName("GetCurrentStatus")]
        public dynamic GetCurrentStatus() 
        {
            return new {
                CurrentQuestion = new {
                    Title = _state.CurrentQuestionTitle,
                    Active = _state.CurrentQuestionActive,
                    Results = _state.CurrentQuestionActive ? null : _state.GetVotes()
                },
                People = _state.GetCurrentStatus(),
                MyCurrentVote = _state.GetUsersVote(Context.ConnectionId)
            };
        }
        
        public string CurrentUserName 
        {
            get { return _state.NameFor(Context.ConnectionId); }
        }
        
        public void Vote(string vote) 
        {
            _state.Vote(Context.ConnectionId, vote);
            Clients.All.userVoted(CurrentUserName);
        }
        
        public void StartVote(string questionTitle)
        {
            _state.NewVote(questionTitle);
            Clients.All.voteStarted(questionTitle);
        }
        
        public void EndVote()
        {
            _state.EndVote();
            var allVotes = _state.GetVotes();
            Clients.All.voteEnded(allVotes);
        }
    }
}