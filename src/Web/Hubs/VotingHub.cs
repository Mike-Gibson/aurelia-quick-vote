using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Framework.Logging;
using System.Threading.Tasks;

namespace WebAPIApplication.Hubs
{
    [HubName("voting")]
    public class VotingHub : Hub
    {
        private static readonly VotingState _state = new VotingState();
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
            try 
            {
                var userName = CurrentUserName;
                Clients.All.userDisconnected(userName);
            } 
            finally 
            {
                _state.TryToDeleteUser(Context.ConnectionId);
            }
            
            return base.OnDisconnected(stopCalled);
        }
        
        [HubMethodName("GetCurrentStatus")]
        public dynamic GetCurrentStatus() 
        {
            return new {
                CurrentQuestion = (_state.CurrentQuestionStatus == VotingState.QuestionStatus.NotStarted) ? null : new {
                    Title = _state.CurrentQuestionTitle,
                    Active = (_state.CurrentQuestionStatus == VotingState.QuestionStatus.Active),
                    Results = (_state.CurrentQuestionStatus == VotingState.QuestionStatus.Active) ? null : _state.GetVotes()
                },
                People = _state.GetCurrentUsers(),
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