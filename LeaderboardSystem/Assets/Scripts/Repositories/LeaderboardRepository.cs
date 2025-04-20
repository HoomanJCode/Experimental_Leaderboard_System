using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repositories.Models;
using System;
using Unity.VisualScripting;
using System.IO;

namespace Repositories
{
    public class LeaderboardRepository : ILeaderboardRepository
    {
        private readonly IStorageAdapter<string> _storage;
        private readonly string LeaderboardKey;
        private readonly string mainPath;

        public List<PlayerScore> Scores { get; private set; } = new List<PlayerScore>();

        public LeaderboardRepository(IStorageAdapter<string> storage, string LeaderboardKey, string folderPath)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            mainPath = folderPath;
            this.LeaderboardKey = LeaderboardKey;
            LoadScores().ConfigureAwait(false);
        }

        public async Task SaveChangesAsync()
        {
            await _storage.SaveAsync(Path.Combine(mainPath, LeaderboardKey), SerializeScores());
        }

        public async Task DeleteAsync(int playerId)
        {
            var path = Path.Combine(mainPath, LeaderboardKey);
            if (_storage.Exists(path)) _storage.Delete(path);
        }

        private async Task LoadScores()
        {
            var path=Path.Combine(mainPath, LeaderboardKey);
            if (_storage.Exists(path))
            {
                var data = await _storage.LoadAsync(path);
                Scores = DeserializeScores(data) ?? new List<PlayerScore>();
            }
        }

        private string SerializeScores() => Scores.Serialize().ToSafeString();

        private List<PlayerScore> DeserializeScores(string data) =>JsonUtility.FromJson<List<PlayerScore>>(data);
    }
}
