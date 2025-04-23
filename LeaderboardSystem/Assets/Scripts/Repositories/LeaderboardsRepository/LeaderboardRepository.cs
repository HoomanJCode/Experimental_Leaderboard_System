using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace Repositories
{
    public abstract class LeaderboardRepository : ILeaderboardRepository
    {
        private readonly IStorageAdapter<string> _storage = new TextFileAdapter();
        private readonly string LeaderboardKey;
        private string MainPath => Path.Combine(Application.persistentDataPath, "Leaderboards");

        protected ConcurrentDictionary<int, int> Scores { get; private set; } = new();

        public LeaderboardRepository(string LeaderboardKey)
        {
            this.LeaderboardKey = LeaderboardKey;
            if (!Directory.Exists(MainPath))
                Directory.CreateDirectory(MainPath);
        }

        private struct PlayerScore
        {
            public PlayerScore(int id, int score) { Id = id; Score = score; }
            public int Id;
            public int Score;
        }
        public async Task SaveChangesAsync()
        {
            var scoresData = Scores.Select(x => new PlayerScore(x.Key, x.Value)).ToArray();
            await _storage.SaveAsync(Path.Combine(MainPath, LeaderboardKey), Serialize(scoresData));
        }

        public async Task LoadScoresAsync()
        {
            var path = Path.Combine(MainPath, LeaderboardKey);
            if (await _storage.Exists(path))
            {
                var data = await _storage.LoadAsync(path);
                var scores = Deserialize(data);
                foreach (var item in scores)
                    Scores.TryAdd(item.Id, item.Score);
            }
        }

        public async Task DeleteFileAsync()
        {
            var path = Path.Combine(MainPath, LeaderboardKey);
            if (await _storage.Exists(path)) await _storage.Delete(path);
        }

        private static string Serialize(PlayerScore[] scores) => JsonConvert.SerializeObject(scores);

        private static PlayerScore[] Deserialize(string data) => JsonConvert.DeserializeObject<PlayerScore[]>(data);
    }
}
