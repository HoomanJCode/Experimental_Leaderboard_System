using System.Threading.Tasks;
using UnityEngine;

namespace Repositories
{
    public interface IAvatarRepository
    {
        Task AddOrUpdateAsync(int playerId, Sprite avatarSprite);
        Task<Sprite> GetByIdAsync(int playerId);
        Task DeleteAsync(int playerId);
        Task<bool> HasAvatarAsync(int playerId);
    }
}
