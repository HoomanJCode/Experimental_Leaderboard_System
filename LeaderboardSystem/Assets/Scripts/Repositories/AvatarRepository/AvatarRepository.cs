using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace Repositories
{
    public class AvatarRepository : IAvatarRepository
    {
        private readonly IStorageAdapter<byte[]> _storage = new PhotoFileAdapter();
        private readonly ConcurrentDictionary<int, Sprite> avatarsCache=new();

        private string MainPath { get; set; } = Path.Combine(Application.persistentDataPath,"Profiles","Avatars");

        public AvatarRepository()
        {
            Directory.CreateDirectory(MainPath);
        }

        public AvatarRepository(string mainPath, IStorageAdapter<byte[]> storage)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(_storage));
            MainPath = mainPath ?? throw new ArgumentNullException(nameof(mainPath));
            Directory.CreateDirectory(MainPath);
        }

        public async Task AddOrUpdateAsync(int playerId,Sprite avatar)
        {
            var path = Path.Combine(MainPath, playerId.ToString());
            avatarsCache.AddOrUpdate(playerId,avatar,(a,b)=> avatar);
            await _storage.SaveAsync(path, SpriteUtilities.SpriteToByte(avatar));
        }

        public async Task<Sprite> GetByIdAsync(int playerId)
        {
            var path = Path.Combine(MainPath, playerId.ToString());
            var cached = avatarsCache.TryGetValue(playerId, out var sprite);
            if (cached) return sprite;
            else if (!await _storage.Exists(path)) return null;
            var imageData = await _storage.LoadAsync(path);
            sprite = SpriteUtilities.BytesToSprite(imageData);
            avatarsCache.TryAdd(playerId,sprite);
            return sprite;
        }

        public async Task DeleteAsync(int playerId)
        {
            var path = Path.Combine(MainPath, playerId.ToString());
            avatarsCache.TryRemove(playerId,out _);
            if (!await _storage.Exists(path)) throw new InvalidOperationException("Player Not Exist!");
            await _storage.Delete(path);
        }

        //todo: design tests for it
        public async Task<bool> HasAvatarAsync(int playerId)
        {
            if (avatarsCache.ContainsKey(playerId)) return true;
            var path = Path.Combine(MainPath, playerId.ToString());
            return await _storage.Exists(path);
        }
    }
}
