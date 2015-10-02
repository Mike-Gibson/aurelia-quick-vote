using System.Threading.Tasks;

namespace WebAPIApplication.Data
{
    public interface IUnitOfWork
    {
        int SaveChanges();
        
        Task<int> SaveChangesAsync();
    }
}