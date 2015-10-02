using System.Threading.Tasks;
using Microsoft.Data.Entity;

namespace WebAPIApplication.Data
{
    public class DataContext : DbContext, IUnitOfWork
    {
		public DbSet<User> Users { get; set; }
        public DbSet<Question> Questions { get; set; }
		
        public DataContext()
        {
            Database.EnsureCreated();
        }
		
		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);
		}
		
		public Task<int> SaveChangesAsync()
		{
			return base.SaveChangesAsync();
		}
	}
}