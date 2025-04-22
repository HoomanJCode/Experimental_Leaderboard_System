using System.Threading.Tasks;
using Repositories.Models;

namespace Repositories
{
    public interface IPlayerRepository
    {
        Task<int> AddPlayerAsync(SavePlayerDto player);
        Task UpdatePlayerAsync(Player player);
        Task<Player> GetByIdAsync(int playerId);
        Task<bool> Exist(int playerId);
        Task SaveChangesAsync();
        Task DeleteAsync(int playerId);
    }
}
