using System;
using System.IO;
using System.Threading.Tasks;
using Repositories.Models;
using UnityEngine;

namespace Repositories
{
    public class AvatarRepository : IAvatarRepository
    {
        private readonly IStorageAdapter<byte[]> _storage = new PhotoFileAdapter();

        private string MainPath { get; set; } = "C:\\...";

        public AvatarRepository()
        {
        }

        public AvatarRepository(string mainPath, IStorageAdapter<byte[]> storage)
        {
            _storage = storage;
            MainPath = mainPath;
        }

        public async Task AddAsync(PlayerAvatar avatar)
        {
            var path = Path.Combine(MainPath, avatar.PlayerId.ToString());
            if (_storage.Exists(path))
                throw new InvalidOperationException($"Avatar for player {avatar.PlayerId} already exists");

            await _storage.SaveAsync(path, avatar.ProfileImage);
        }

        public async Task UpdateAsync(PlayerAvatar avatar)
        {
            var path = Path.Combine(MainPath, avatar.PlayerId.ToString());
            if (!_storage.Exists(path))
                throw new InvalidOperationException($"Avatar for player {avatar.PlayerId} not found");

            await _storage.SaveAsync(path, avatar.ProfileImage);
        }

        public async Task<PlayerAvatar> GetByIdAsync(int playerId)
        {
            var path = Path.Combine(MainPath, playerId.ToString());
            if (!_storage.Exists(path)) return null;
            var imageData = await _storage.LoadAsync(path);
            return new PlayerAvatar(playerId, imageData);
        }

        public async Task DeleteAsync(int playerId)
        {
            var path = Path.Combine(MainPath, playerId.ToString());
            if (!_storage.Exists(path)) throw new InvalidOperationException("Player Not Exist!");
            _storage.Delete(path);
        }
    }
}
