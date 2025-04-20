using System.Threading.Tasks;
using Repositories.Models;

namespace Repositories
{
    public interface IPlayerRepository
    {
        Task<int> AddPlayerAsync(SavePlayerDto player);
        Task UpdatePlayerAsync(Player player);
        Task<Player> GetByIdAsync(int id);
        Task SaveChangesAsync();
    }
}
