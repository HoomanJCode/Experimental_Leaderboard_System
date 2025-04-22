using System.Threading.Tasks;
using Repositories.Models;

namespace Repositories
{
    public interface IAvatarRepository
    {
        Task AddAsync(PlayerAvatar avatar);
        Task UpdateAsync(PlayerAvatar avatar);
        Task<PlayerAvatar> GetByIdAsync(int playerId);
        Task DeleteAsync(int playerId);
        Task<bool> HasAvatarAsync(int playerId);
    }
}
