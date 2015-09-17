using System;
using System.Collections.Generic;
using System.Linq;

namespace WebAPIApplication.Hubs
{
    public class VotingState
    {
        public enum QuestionStatus
        {
            NotStarted,
            Active,
            Finished
        }
        
        private readonly Dictionary<string, UserInfo> _users = new Dictionary<string, UserInfo> {
            { "dummy-connection-1", new UserInfo { Name = "Temp User 1" }},
            { "dummy-connection-2", new UserInfo { Name = "Temp User 2" }},
        };
        
        public string CurrentQuestionTitle { get; private set; }
        public QuestionStatus CurrentQuestionStatus { get; private set; } 
        
        public bool AddUser(string connectionId, string name) 
        {
            if (_users.Any(u => u.Value.Name == name))
                return false;
            
            _users.Add(connectionId, new UserInfo { Name = name });
            return true;
        }
        
        public bool TryToDeleteUser(string connectionId) 
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
            if (this.CurrentQuestionStatus == QuestionStatus.Active) {
                throw new InvalidOperationException("Cannot start a new vote, as there is already a vote in progress");
            }
            
            this.CurrentQuestionTitle = newQuestionTitle;
            this.CurrentQuestionStatus = QuestionStatus.Active;
            _users.Values.ToList().ForEach(v => v.Vote = null);
        }
        
        public void EndVote() 
        {
            if (this.CurrentQuestionStatus != QuestionStatus.Active) {
                throw new InvalidOperationException("Cannot end the vote, as there is not an active vote");
            }
            
            this.CurrentQuestionStatus = QuestionStatus.Finished;
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
        
        public IEnumerable<User> GetCurrentUsers() {
                return _users.Values
                .ToList()
                .Select(v => new User {
                    Name = v.Name,
                    HasVoted = (v.Vote != null)
                })
                .ToList();
        }
        
        public class User
        {
            public string Name { get; set; }
            public bool HasVoted { get; set; }    
        }
        
        private class UserInfo
        {
            public string Name { get; set; }
            public string Vote { get; set; }    
        }
    }
}