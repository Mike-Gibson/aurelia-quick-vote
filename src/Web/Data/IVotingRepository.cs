using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Data.Entity;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


using Microsoft.Data.Entity.Metadata;

namespace WebAPIApplication.Data
{
	public class User
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public bool LoggedIn { get; set; }
	}
	
	public class Question
	{
		public Guid Id { get; set; }
		public string Title { get; set; }
		public DateTimeOffset DateCreated { get; set; }
		public bool HasFinished { get; set; }
		
		public virtual ICollection<QuestionVote> Votes { get; set;}
		
		public Question() 
		{
			Votes = new Collection<QuestionVote>();
		}
	}
	
	public class QuestionVote
	{
		public Guid Id { get; set; }
		
		//[Index("UC_QuestionID_UserID", 1, IsUnique = true)]
		public Guid QuestionId { get; set; }
		
		//[Index("UC_QuestionID_UserID", 2, IsUnique = true)]
		public Guid UserId { get; set; }
		
		public string Vote { get; set; }
		
		public virtual Question Question { get; set; }
		public virtual User User { get; set; }
	}
	
	public interface IVotingRepository
	{
		Task<IEnumerable<User>> GetUsers();
		Task<Question> GetCurrentQuestion();
		void AddQuestion(Question question);
		Task LoginUser(string name);
		Task LogoutUser(string name);
		Task Vote(Question question, string userName, string vote);
	}
	
	public class VotingRepository : IVotingRepository
	{
		private DataContext _context;
		public VotingRepository(DataContext context)
		{
			_context = context;
		}
	
		public async Task<IEnumerable<User>> GetUsers()
		{
			return await _context.Users
				.Where(u => u.LoggedIn)
				.OrderBy(u => u.Name)
				.ToListAsync();
		}
		
		public async Task LoginUser(string name)
		{
			var user = await _context.Users.SingleOrDefaultAsync(u => u.Name == name);
			
			if (user == null) {
				user = new User {
					Id = Guid.NewGuid(),
					Name = name
				};
				_context.Users.Add(user);
			}
			
			user.LoggedIn = true;
		}
		
		public async Task LogoutUser(string name)
		{
			var user = await _context.Users.SingleOrDefaultAsync(u => u.Name == name);
			
			if (user == null) 
				return;
			
			user.LoggedIn = false;
		}
		
		public async Task<Question> GetCurrentQuestion()
		{
			return await _context.Questions
				.OrderByDescending(q => q.DateCreated)
				.FirstOrDefaultAsync();
		}
		
		public async Task<int> SaveChangesAsync() 
		{
			return await _context.SaveChangesAsync();
		}

        public void AddQuestion(Question question)
        {
            _context.Questions.Add(question);
        }

        public async Task Vote(Question question, string userName, string vote)
        {
			var user = await _context.Users.SingleAsync(u => u.Name == userName);
			var questionVote = question.Votes.SingleOrDefault(v => v.UserId == user.Id);
			
			if (questionVote == null) 
			{
				questionVote = new QuestionVote 
				{
					Id = Guid.NewGuid(),
					User = user,
					Vote = vote					
				};
			}
			
			question.Votes.Add(questionVote);
        }
    }
}