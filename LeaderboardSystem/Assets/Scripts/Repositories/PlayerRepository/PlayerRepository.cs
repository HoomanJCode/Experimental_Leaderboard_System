using System;
using System.Threading.Tasks;
using Repositories.Models;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using UnityEngine;
using System.Collections.Concurrent;

namespace Repositories
{
    //todo: implement database type of PlayerRepository and change this class name to FilePlayerRepository
    public class PlayerRepository : IPlayerRepository
    {
        private string StorageKey => nameof(PlayerRepository);
        private string LastIdStorageKey => $"{StorageKey}_LastId";
        private readonly IStorageAdapter<string> _storage = new TextFileAdapter();
        private string MainPath { get; set; } = Path.Combine(Application.persistentDataPath, "Profiles");
        private int _lastPlayerId;
        private readonly ConcurrentDictionary<int,Player> Players = new();

        public PlayerRepository()
        {
            if (!Directory.Exists(MainPath))
                Directory.CreateDirectory(MainPath);
        }
        public PlayerRepository(string mainPath,IStorageAdapter<string> storage)
        {
            _storage = storage;
            MainPath = mainPath;
        }

        public async Task<int> AddPlayerAsync(string name,string description)
        {
            int newId = ++_lastPlayerId;
            var newPlayer = new Player(newId, name, description);
            Players.TryAdd(newId, newPlayer);
            return await Task.FromResult(newId);
        }

        public async Task<bool> UpdatePlayerAsync(Player player)
        {
            if (!Players.TryGetValue(player.Id, out var currentPlayer))
                throw new InvalidOperationException($"Player with ID {player.Id} not found.");
            var newPlayer = new Player(player.Id, player.Name, player.Description);
            return await Task.FromResult(Players.TryUpdate(player.Id, newPlayer, currentPlayer));
        }

        public async Task<Player> GetByIdAsync(int playerId)
        {
            if (!Players.TryGetValue(playerId, out var currentPlayer))
                throw new InvalidOperationException($"Player with ID {playerId} not found.");
            return await Task.FromResult(currentPlayer);
        }


        //todo: design tests for it
        public async Task<bool> DeletePlayerAsync(int playerId)
        {
            if (!Players.ContainsKey(playerId)) throw new InvalidOperationException();
            return await Task.FromResult(Players.TryRemove(playerId, out _));
        }

        //todo: design tests for it
        public Task<bool> PlayerExist(int playerId) => Task.FromResult(Players.ContainsKey(playerId));
        public async Task SaveChangesAsync()
        {
            var playersClone = Players.Values.ToArray().Clone() as Player[];
            await _storage.SaveAsync(Path.Combine(MainPath, LastIdStorageKey), _lastPlayerId.ToString());
            await _storage.SaveAsync(Path.Combine(MainPath, StorageKey), SerializedPlayersData(playersClone));
        }

        public async Task LoadPlayersAsync()
        {
            var path = Path.Combine(MainPath, StorageKey);
            if (await _storage.Exists(path))
            {
                var data = await _storage.LoadAsync(path);
                var loadedPlayers = DeserializePlayersData(data);
                foreach (var item in loadedPlayers)
                    if (!Players.TryAdd(item.Id, item)) throw new InvalidDataException($"Error On Adding {item}");
            }
            var path2 = Path.Combine(MainPath, LastIdStorageKey);
            if (await _storage.Exists(path2))
            {
                var data = await _storage.LoadAsync(path2);
                _lastPlayerId= int.TryParse(data, out var id) ? id : 0;
            }
            if (_lastPlayerId == 0 && Players.Count > 0)
                _lastPlayerId = Players.Values.Max(p => p.Id);
        }
        private static string SerializedPlayersData(Player[] data) => JsonConvert.SerializeObject(data);

        private static Player[] DeserializePlayersData(string data) => JsonConvert.DeserializeObject<Player[]>(data);
    }
}
