using System.Threading.Tasks;
using Repositories.Models;

namespace Repositories
{
    public interface IPlayerRepository
    {
        Task AddAsync(Player player);
        Task UpdateAsync(Player player);
        Task<Player> GetByIdAsync(int id);
        Task DeleteAsync(int id);
        Task SaveChangesAsync();
    }
}
