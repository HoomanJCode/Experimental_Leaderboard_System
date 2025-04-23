using System;
using System.Collections;
using System.Threading.Tasks;
using Repositories;
using Repositories.Models;
using UnityEngine;

namespace Services
{
    public class PlayersAuthenticationService : IServiceSetup
    {
        private static PlayersAuthenticationService _instance;
        public static PlayersAuthenticationService Instance => _instance ??= new PlayersAuthenticationService();
        // Dependencies
        private readonly IPlayerRepository _playerRepository;
        private readonly IAvatarRepository _avatarRepository;
        private bool playersLoaded = false;
        private float saveTimer = -1;
        private void SaveChanges()
        {
            saveTimer = 4;
        }

        private IEnumerator MemChangesEnumerator()
        {
            yield return _playerRepository.LoadPlayersAsync().UntileComplete();
            playersLoaded = true;
            while (true)
            {
                yield return new WaitForSeconds(1);
                if (saveTimer < 0) continue;
                if (saveTimer == 0) yield return _playerRepository.SaveChangesAsync().UntileComplete();
                saveTimer--;
            }
        }
        // Private constructor for singleton
        public PlayersAuthenticationService()
        {
            // Initialize repositories (could be injected via DI in a real scenario)
            _playerRepository = new PlayerRepository();
            _avatarRepository = new AvatarRepository();
            CoroutineRunner.Singletone.StartCoroutine(MemChangesEnumerator());
        }


        /// <summary>
        /// Adds a new player to the system
        /// </summary>
        public async Task<Player> RegisterPlayer(string name, string description, Sprite avatar = null)
        {
            var addedPlayerId = await _playerRepository.AddPlayerAsync(name, description);
            SaveChanges();
            if (avatar != null)
                await _avatarRepository.AddOrUpdateAsync(addedPlayerId, avatar);
            return new Player(addedPlayerId, name, description);
        }

        /// <summary>
        /// Removes a player from the system
        /// </summary>
        public async Task<bool> RemovePlayer(int playerId)
        {
            if (!await _playerRepository.PlayerExist(playerId)) return false;
            if (await _avatarRepository.HasAvatarAsync(playerId))
                await _avatarRepository.DeleteAsync(playerId);
            await _playerRepository.DeletePlayerAsync(playerId);
            SaveChanges();
            return true;
        }

        /// <summary>
        /// Updates an existing player
        /// </summary>
        public async Task<bool> UpdatePlayer(int playerId, string name, string description, Sprite avatar = null)
        {
            if (!await _playerRepository.PlayerExist(playerId)) return false;
            await _playerRepository.UpdatePlayerAsync(new Player(playerId, name, description));
            SaveChanges();
            if (avatar != null) await UpdatePlayerAvatar(playerId, avatar);
            return true;
        }
        /// <summary>
        /// Updates an existing player Avatar
        /// </summary>
        public async Task<bool> UpdatePlayerAvatar(int playerid, Sprite avatar)
        {
            if (avatar == null)
                throw new ArgumentNullException(nameof(avatar));
            if (!await _playerRepository.PlayerExist(playerid)) return false;
            if (await _avatarRepository.HasAvatarAsync(playerid))
                await _avatarRepository.AddOrUpdateAsync(playerid, avatar);
            return true;
        }

        /// <summary>
        /// Retrieves a player by their ID
        /// </summary>
        public async Task<Player> GetPlayerById(int playerId) => await _playerRepository.GetByIdAsync(playerId);
        public async Task<bool> PlayerExist(int playerId) => await _playerRepository.PlayerExist(playerId);
        public async Task<Sprite> GetPlayerAvatarById(int playerId) => await _avatarRepository.GetByIdAsync(playerId);

        public async Task WaitCheckForSetup()
        {
            while (!playersLoaded)
                await Task.Delay(100);
        }
    }
}