using System;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Framework.Logging;

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
            private readonly Dictionary<string, UserInfo> _users = new Dictionary<string, UserInfo>();
            
            public bool AddUser(string connectionId, string name) 
            {
                if (_users.Any(u => u.Value.Name == name))
                    return false;
                
                _users.Add(connectionId, new UserInfo { Name = name });
                return true;
            }
            
            public void ClearVotes() 
            {
               _users.Values.ToList().ForEach(v => v.Vote = null); 
            }
            
            public void Vote(string connectionId, string vote) 
            {
                _users[connectionId].Vote = vote;    
            }
            
            public string NameFor(string connectionId)
            {
                return _users[connectionId].Name;
            }
            
            public Dictionary<string, string> GetVotes() 
            {
                return _users.Values.ToDictionary(v => v.Name, v => v.Vote);
            }
            
            public Dictionary<string, bool> GetUsers() 
            {
                return _users.Values.ToDictionary(v => v.Name, v => v.Vote != null);
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
            
            if (!added) {
                return false;
            }
            
            Clients.All.userConnected(name);
            return true;
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
        
        public void EndVote() 
        {
            var allVotes = _state.GetVotes();
            Clients.All.voteEnded(allVotes);
        }
    }
}