using System;
using System.IO;
using System.Threading.Tasks;
using Repositories.Models;

namespace Repositories
{
    public class AvatarRepository : IAvatarRepository
    {
        private readonly IStorageAdapter<byte[]> _storage;
        private readonly string mainPath;

        public AvatarRepository(IStorageAdapter<byte[]> storage, string folderPath)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            mainPath=folderPath;
        }

        public async Task AddAsync(PlayerAvatar avatar)
        {
            if (_storage.Exists(avatar.PlayerId.ToString()))
                throw new InvalidOperationException($"Avatar for player {avatar.PlayerId} already exists");

            await _storage.SaveAsync(Path.Combine(mainPath, avatar.PlayerId.ToString()), avatar.ProfileImage);
        }

        public async Task UpdateAsync(PlayerAvatar avatar)
        {
            if (!_storage.Exists(avatar.PlayerId.ToString()))
                throw new InvalidOperationException($"Avatar for player {avatar.PlayerId} not found");

            await _storage.SaveAsync(Path.Combine(mainPath, avatar.PlayerId.ToString()), avatar.ProfileImage);
        }

        public async Task<PlayerAvatar> GetByIdAsync(int playerId)
        {
            var imageData = await _storage.LoadAsync(Path.Combine(mainPath, playerId.ToString()));
            return new PlayerAvatar(playerId, imageData);
        }

        public async Task DeleteAsync(int playerId)
        {
            _storage.Delete(playerId.ToString());
        }
    }
}
