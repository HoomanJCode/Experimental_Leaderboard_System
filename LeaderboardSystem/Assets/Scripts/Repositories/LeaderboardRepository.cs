using System.Collections.Generic;
using System.Threading.Tasks;
using Repositories.Models;
using System.IO;
using Newtonsoft.Json;

namespace Repositories
{
    public abstract class LeaderboardRepository : ILeaderboardRepository
    {
        private readonly IStorageAdapter<string> _storage = new TextFileAdapter();
        private readonly string LeaderboardKey;
        private string MainPath => "C:\\...";

        public List<PlayerScore> Scores { get; private set; } = new List<PlayerScore>();

        public LeaderboardRepository(string LeaderboardKey)
        {
            this.LeaderboardKey = LeaderboardKey;
            LoadScores().ConfigureAwait(false);
        }

        public async Task SaveChangesAsync()
        {
            await _storage.SaveAsync(Path.Combine(MainPath, LeaderboardKey), Serialize());
        }

        public async Task DeleteAsync(int playerId)
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
