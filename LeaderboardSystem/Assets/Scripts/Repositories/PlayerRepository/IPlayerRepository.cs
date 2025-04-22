using System.Threading.Tasks;
using Repositories.Models;

namespace Repositories
{
    public interface IPlayerRepository
    {
        Task<int> AddPlayerAsync(string name, string description);
        Task<bool> UpdatePlayerAsync(Player player);
        Task<Player> GetByIdAsync(int playerId);
        Task<bool> PlayerExist(int playerId);
        Task SaveChangesAsync();
        Task LoadPlayers();
        Task<bool> DeletePlayerAsync(int playerId);
    }
}
