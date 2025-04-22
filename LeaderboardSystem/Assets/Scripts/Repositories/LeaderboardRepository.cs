using System.Collections.Generic;
using System.Threading.Tasks;
using Repositories.Models;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace Repositories
{
    public abstract class LeaderboardRepository : ILeaderboardRepository
    {
        private readonly IStorageAdapter<string> _storage = new TextFileAdapter();
        private readonly string LeaderboardKey;
        private string MainPath => Path.Combine(Application.persistentDataPath, "Leaderboards");

        public List<PlayerScore> Scores { get; private set; } = new List<PlayerScore>();

        public LeaderboardRepository(string LeaderboardKey)
        {
            this.LeaderboardKey = LeaderboardKey;
            if (!Directory.Exists(MainPath))
                Directory.CreateDirectory(MainPath);
            //LoadScores().ConfigureAwait(false);
        }

        public async Task SaveChangesAsync()
        {
            await _storage.SaveAsync(Path.Combine(MainPath, LeaderboardKey), Serialize());
        }

        public async Task DeleteAsync()
        {
            var path = Path.Combine(MainPath, LeaderboardKey);
            if (await _storage.Exists(path)) await _storage.Delete(path);
        }

        private async Task LoadScores()
        {
            var path=Path.Combine(MainPath, LeaderboardKey);
            if (await _storage.Exists(path))
            {
                var data = await _storage.LoadAsync(path);
                Scores = Deserialize(data) ?? new List<PlayerScore>();
            }
        }

        private string Serialize() => JsonConvert.SerializeObject(Scores);

        private List<PlayerScore> Deserialize(string data) => JsonConvert.DeserializeObject<List<PlayerScore>>(data);
    }
}
