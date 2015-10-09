using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Framework.Logging;
using System.Threading.Tasks;
using WebAPIApplication.Data;
using System;
using System.Linq;
using System.Collections.Generic;

namespace WebAPIApplication.Hubs
{
    [HubName("voting")]
    public class VotingHub : Hub
    {
        private readonly IVotingRepository _votingRepository;
        private readonly IUnitOfWork _unitOfWork;
        private ILogger<VotingHub> _logger;
        
        private static readonly Dictionary<string, string> LoggedInUsers = new Dictionary<string, string>();
        
        private string CurrentUserName 
        {
            get 
            {
                string username;
                if (LoggedInUsers.TryGetValue(Context.ConnectionId, out username)) 
                {
                    return username;
                } 
                
                return null; 
            }
        }
        
        public VotingHub(ILogger<VotingHub> logger, IVotingRepository votingRepository, IUnitOfWork unitOfWork) 
        {
            _logger = logger;
            _votingRepository = votingRepository;
            _unitOfWork = unitOfWork;
        }
        
        public override async Task OnDisconnected(bool stopCalled)
        {
            if (LoggedInUsers.ContainsKey(Context.ConnectionId))
            {
                var userName = LoggedInUsers[Context.ConnectionId];
                LoggedInUsers.Remove(Context.ConnectionId);
                    
                await _votingRepository.LogoutUser(userName);
                Clients.All.userDisconnected(userName);                
    
                await _unitOfWork.SaveChangesAsync();
            }
            
            await base.OnDisconnected(stopCalled);
        }

        [HubMethodName("Login")]
        public async Task Login(string name)
        {
            await _votingRepository.LoginUser(name);
            Clients.All.userConnected(name);
            
            await _unitOfWork.SaveChangesAsync();
            
            if (!LoggedInUsers.ContainsKey(Context.ConnectionId)) 
            {
                LoggedInUsers.Add(Context.ConnectionId, name);
            }
        }
        
        [HubMethodName("Logout")]
        public async Task Logout()
        {
            await _votingRepository.LogoutUser(CurrentUserName);
            Clients.All.userDisconnected(CurrentUserName);
            
            await _unitOfWork.SaveChangesAsync();
            
            if (LoggedInUsers.ContainsKey(Context.ConnectionId)) 
            {
                LoggedInUsers.Remove(Context.ConnectionId);
            }
        }
        
        [HubMethodName("GetCurrentStatus")]
        public async Task<dynamic> GetCurrentStatus() 
        {
            var currentQuestion = await _votingRepository.GetCurrentQuestion();
            
            var users = await _votingRepository.GetUsers();
            var myVote = currentQuestion == null ? 
                null : 
                currentQuestion.Votes
                    .Where(v => v.User.Name == CurrentUserName)
                    .Select(v => v.Vote)
                    .SingleOrDefault();
            var questionVotes = currentQuestion == null ?
                Enumerable.Empty<QuestionVote>() : 
                currentQuestion.Votes;
            
            return new {
                CurrentQuestion = currentQuestion == null ? 
                    null : 
                    new {
                        Title = currentQuestion.Title,
                        Active = !currentQuestion.HasFinished,
                        Results = (!currentQuestion.HasFinished) ? Enumerable.Empty<dynamic>() : GetResults(currentQuestion)
                    },
                People = users
                    .Select(u => new { Name = u.Name, HasVoted = questionVotes.Any(qv => qv.UserId == u.Id && qv.Vote != null) }),
                MyCurrentVote = myVote
            };
        }
        
        [HubMethodName("Vote")]
        public async Task Vote(string vote) 
        {
            var currentQuestion = await _votingRepository.GetCurrentQuestion();
            
            await _votingRepository.Vote(currentQuestion, CurrentUserName, vote);
            await _unitOfWork.SaveChangesAsync();
            
            Clients.All.userVoted(CurrentUserName);
        }
        
        [HubMethodName("StartVote")]
        public async Task StartVote(string questionTitle)
        {
            var currentQuestion = await _votingRepository.GetCurrentQuestion();            
            if (currentQuestion != null && !currentQuestion.HasFinished)
            {
                throw new InvalidOperationException("Cannot start a new vote, as there is already a vote in progress");
            }
            
            _votingRepository.AddQuestion(new Question {
                Id = Guid.NewGuid(),
                Title = questionTitle,
                DateCreated = DateTimeOffset.Now,
                HasFinished = false
            });
            await _unitOfWork.SaveChangesAsync();
            
            Clients.All.voteStarted(questionTitle);
        }
        
        [HubMethodName("EndVote")]
        public async Task EndVote()
        {
            var currentQuestion = await _votingRepository.GetCurrentQuestion();
            
            if (currentQuestion == null || currentQuestion.HasFinished)
            {
                throw new InvalidOperationException("Cannot end the vote, as there is not an active vote");
            }
            
            currentQuestion.HasFinished = true;            
            await _unitOfWork.SaveChangesAsync();
            
            var allVotes = GetResults(currentQuestion);
            
            Clients.All.voteEnded(allVotes);
        }
        
        private IEnumerable<dynamic> GetResults(Question question) 
        {
            if (question == null)
                return Enumerable.Empty<dynamic>();
            
            return question
                .Votes
                .Select(v => new {
                    Name = v.User.Name, 
                    Vote = v.Vote
                });
        }
    }
}