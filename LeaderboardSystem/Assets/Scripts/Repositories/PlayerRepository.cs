using System;
using System.Threading.Tasks;
using Repositories.Models;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Linq;

namespace Repositories
{
    //todo: implement database type of PlayerRepository and change this class name to FilePlayerRepository
    public class PlayerRepository : IPlayerRepository
    {
        private string StorageKey => nameof(PlayerRepository);
        private string LastIdStorageKey => $"{StorageKey}_LastId";
        private readonly IStorageAdapter<string> _storage = new TextFileAdapter();
        private string MainPath { get; set; } = "C:\\..."; 
        private int _lastPlayerId;
        public List<Player> Players { get; private set; } = new List<Player>();

        public PlayerRepository()
        {
            LoadPlayers().ConfigureAwait(false);
        }
        public PlayerRepository(string mainPath,IStorageAdapter<string> storage)
        {
            _storage = storage;
            MainPath = mainPath;
            LoadPlayers().ConfigureAwait(false);
        }

        public async Task<int> AddPlayerAsync(SavePlayerDto playerDto)
        {
            int newId = ++_lastPlayerId;
            var newPlayer = new Player(newId, playerDto.Name, playerDto.Description);
            Players.Add(newPlayer);
            //todo: add timing save period
            await SaveChangesAsync();
            return await Task.FromResult(newId);
        }

        public async Task UpdatePlayerAsync(Player player)
        {
            var existingPlayer = Players.FirstOrDefault(p => p.Id == player.Id);
            if (existingPlayer == null)
                throw new InvalidOperationException($"Player with ID {player.Id} not found.");
            existingPlayer.Name = player.Name;
            existingPlayer.Description = player.Description;
            await SaveChangesAsync();
        }

        public async Task<Player> GetByIdAsync(int id)
        {
            var player = Players.FirstOrDefault(p => p.Id == id);
            return player;
        }

        public async Task SaveChangesAsync()
        {
            await _storage.SaveAsync(Path.Combine(MainPath, StorageKey), Serialize());
            await _storage.SaveAsync(Path.Combine(MainPath, LastIdStorageKey), _lastPlayerId.ToString());
        }

        private async Task LoadPlayers()
        {
            var path = Path.Combine(MainPath, StorageKey);
            if (_storage.Exists(path))
            {
                var data = await _storage.LoadAsync(path);
                Players = Deserialize(data) ?? new List<Player>();
            }
            var path2 = Path.Combine(MainPath, LastIdStorageKey);
            if (_storage.Exists(path2))
            {
                var data = await _storage.LoadAsync(path2);
                _lastPlayerId= int.TryParse(data, out var id) ? id : 0;
            }
            if (_lastPlayerId == 0 && Players.Count > 0)
                _lastPlayerId = Players.Max(p => p.Id);
        }
        private string Serialize() => JsonConvert.SerializeObject(Players);

        private List<Player> Deserialize(string data) => JsonConvert.DeserializeObject<List<Player>>(data);

    }
}
